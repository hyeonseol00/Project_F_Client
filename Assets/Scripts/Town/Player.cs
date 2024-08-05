using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Town.Data;


public class Player : MonoBehaviour
{
    [SerializeField] private UINameChat uiNameChat;

    public Avatar avatar { get; private set; }
    public MyPlayer mPlayer { get; private set; }

    private string nickname;

    private UIChat uiChat;

    private Vector3 goalPos;
    private Quaternion goalRot;

    private Animator animator;

    public int PlayerId { get; private set; }
    public bool IsMine { get; private set; }
    private bool isInit = false;

    public int gold { get; private set; }
    public StatInfo statInfo { get; private set; }
    public List<InventoryItem> InventoryItems { get; private set; } = new List<InventoryItem>();

    public EquippedItems equippedItems;

    private Vector3 lastPos;

    void Start()
    {
        avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }

    public void SetNickname(string nickname)
    {
        this.nickname = nickname;
        uiNameChat.SetName(nickname);
    }

    public void SetEquipment(Equipment equipment)
    {
        Item _weapon = DataLoader.Instance?.GetItemById(equipment.Weapon);
        Item _armor = DataLoader.Instance?.GetItemById(equipment.Armor);
        Item _gloves = DataLoader.Instance?.GetItemById(equipment.Gloves);
        Item _shoes = DataLoader.Instance?.GetItemById(equipment.Shoes);
        Item _accessory = DataLoader.Instance?.GetItemById(equipment.Accessory);
        this.equippedItems = new EquippedItems(_weapon, _armor, _gloves, _shoes, _accessory);

    }

    public void SetIsMine(bool isMine)
    {
        IsMine = isMine;

        if (IsMine)
        {
            mPlayer = gameObject.AddComponent<MyPlayer>();
        }
        else
            Destroy(gameObject.GetComponent<NavMeshAgent>());

        uiChat = TownManager.Instance.UiChat;
        isInit = true;
    }

    public void SetGold(int gold)
    {
        this.gold = gold;
    }

    public void SetStatInfo(StatInfo statInfo)
    {
        this.statInfo = statInfo;
    }

    public void SetInventory(Inventory inven)
    {
        InventoryItems.Clear();

        if (inven.Items != null)
        {
            foreach (var item in inven.Items)
            {
                var itemData = DataLoader.Instance?.GetItemById(item.Id);
                //Debug.Log(itemData);

                InventoryItem inventoryItem = new InventoryItem
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    ItemData = itemData
                };

                InventoryItems.Add(inventoryItem);
            }
        }

        //Debug.Log("Inventory set with items:");
        foreach (var item in InventoryItems)
        {
            //Debug.Log($"ID: {item.Id}, Quantity: {item.Quantity}, ItemData: {item.ItemData?.item_name}");
        }
    }

    private void Update()
    {
        if (isInit == false)
            return;

        if (IsMine)
            return;

        if (Vector3.Distance(transform.position, goalPos) > 0.5f)
            transform.position = goalPos;
        else
            transform.position = Vector3.Lerp(transform.position, goalPos, Time.deltaTime * 10);

        if (goalRot != Quaternion.identity)
        {
            float t = Mathf.Clamp(Time.deltaTime * 10, 0, 0.99f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
        }

        CheckMove();
    }

    public void SendPlayerMessage(string msg)
    {
        if (!IsMine) return;

        // if message is client cmd, access this
        if (msg[0] == '/')
        {
            int firstSpaceIdx = msg.IndexOf(' ');

            string commandType = firstSpaceIdx != -1 ? msg.Substring(0, firstSpaceIdx) : msg;
            string data = firstSpaceIdx != -1 ? msg.Substring(firstSpaceIdx + 1) : "";

            //Debug.Log($"words: {commandType} {data}");

            if (ChatCommandManager.chatCommandMap == null)
            {
                Debug.Log("chatCommandMap is null...");
                return;
            }

            if (ChatCommandManager.chatCommandMap.TryGetValue(commandType, out System.Action<object> action))
            {
                action.Invoke(data);
                return;
            }
            else
            {
                Debug.Log($"this is not clientCmd");
            }
        }

        C_Chat chatPacket = new C_Chat
        {
            PlayerId = PlayerId,
            SenderName = nickname,
            ChatMsg = msg
        };

        GameManager.Network.Send(chatPacket);
    }

    public void RecvMessage(string msg)
    {
        if (msg.StartsWith("[All]")) uiNameChat.PushText(msg);
        uiChat.PushMessage(nickname, msg, IsMine);
    }

    public void Move(Vector3 move, Quaternion rot)
    {
        goalPos = move;
        goalRot = rot;
    }

    public void Animation(int animCode)
    {
        if (animator)
            animator.SetTrigger(animCode);
    }

    void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);


        lastPos = transform.position;
        
    }

    public void ProcessBuyItemEvent(ItemInfo item)
    {
        // 아이템이 이미 존재하는지 확인
        InventoryItem existingItem = InventoryItems.Find(item => item.ItemData.item_id == item.Id);

        if (existingItem != null)
        {
            // 아이템이 존재하면 수량 증가
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            // 아이템이 존재하지 않으면 새로운 아이템 추가
            var itemData = DataLoader.Instance?.GetItemById(item.Id);
            //Debug.Log(itemData);

            InventoryItem inventoryItem = new InventoryItem
            {
                Id = item.Id,
                Quantity = item.Quantity,
                ItemData = itemData
            };

            InventoryItems.Add(inventoryItem);
        }

        Debug.Log($"myPlayer Inven is  {InventoryItems}");
        Debug.Log($"gold is  {gold}");
    }

    public void ProcessSellItemEvent(ItemInfo item)
    {
        // 아이템이 이미 존재하는지 확인
        InventoryItem existingItem = InventoryItems.Find(invItem => invItem.ItemData.item_id == item.Id);

        if (existingItem != null)
        {
            // 아이템의 수량 감소
            existingItem.Quantity -= item.Quantity;

            // 아이템의 수량이 0 이하이면 인벤토리에서 제거
            if (existingItem.Quantity <= 0)
            {
                InventoryItems.Remove(existingItem);
            }
        }
        else
        {
            Debug.LogWarning($"Item with ID {item.Id} does not exist in the inventory.");
        }

        Debug.Log($"myPlayer Inven is  {InventoryItems}");
        Debug.Log($"gold is  {gold}");
    }

}
