using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;


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
  
        this.inven = inven;
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

            Debug.Log($"words: {commandType} {data}");

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
        if(msg.StartsWith("[All]")) uiNameChat.PushText(msg);
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

    public void AddItemToInven(ItemInfo item)
    {
        for (int i = 0; i < inven.Items.Count; i++)
        {
            if (inven.Items[i].Id == item.Id)
            {
                inven.Items[i].Quantity += item.Quantity;
                return;
            }
        }

        inven.Items.Add(item);
        return;
    }

    public void SubItemToInven(ItemInfo item)
    {
        for (int i = 0; i < inven.Items.Count; i++)
        {
            if (inven.Items[i].Id == item.Id)
            {
                inven.Items[i].Quantity -= item.Quantity;
                if (inven.Items[i].Quantity == 0)
                {
                    inven.Items.RemoveAt(i);
                }
                return;
            }
        }

        return;
    }
}
