using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatCommandManager : MonoBehaviour
{
    public static Dictionary<string, System.Action<object>> chatCommandMap { get; private set; }

    [SerializeField] private UIChat uichat;

    void Awake()
    {
        chatCommandMap = new Dictionary<string, System.Action<object>>
        {
            { "/FunctionA", args => FunctionA((string)args) },
            { "/FunctionD", args => FunctionD((string)args) },
            { "/shop", args => ShowItems() },
            { "/stat", args => ShowStats() },
            { "/help", args => ShowCommands() },
            { "/equipment", args => ShowEquipment() },
            //{ "FunctionB", args => FunctionB((int)args, (float)args) },
            //{ "FunctionC", args => FunctionC((bool)args, (string)args) }
            { "/inven", args => ShowInventory() }
        };
    }

    private void FunctionA(string message)
    {
        string nickname = "not use param";
        if (message.Length > 0) message = $"[ClientCmd]: {message}";
        else message = $"[System]: Invalid cmd: message is needed";

        bool myChat = false;

        uichat.PushMessage(nickname, message, myChat);

        Debug.Log($"FunctionA executed with message: {message}");
    }

    private void FunctionD(string message)
    {
        string nickname = "not use param";
        if (message.Length > 0) message = $"[ClientCmd]: {message}";
        else message = $"[System]: Invalid cmd: message is needed";
        bool myChat = false;

        uichat.PushMessage(nickname, message, myChat);

        Debug.Log($"FunctionD executed with message: {message}");
    }

    private IEnumerator AddMessagesWithDelay(List<string> messages)
    {
        foreach (var message in messages)
        {
            uichat.PushMessage("[System]", message, true);
            yield return new WaitForSeconds(0.05f); // 0.05�� �����
        }
    }

    private void ShowItems()
    {
        var items = DataLoader.Instance.Items;  // DataLoader���� �ε�� ������ ����Ʈ ��������

        if (items != null && items.Count > 0)  // ������ ����Ʈ�� null�� �ƴϰ�, �������� �����ϴ� ���
        {
            List<string> messages = new List<string>();

            foreach (var item in items)  // �� �����ۿ� ����
            {
                if (!item.can_sell)  // can_sell�� false�� �������� �ǳʶٱ�
                {
                    continue;
                }

                // �⺻ �޽��� ���� (ID�� �̸�)
                string message = $"[System] Name: {item.item_name}";

                // �� �ʵ忡 ���� 0�� �ƴ� ��쿡�� �޽����� �߰�
                if (!string.IsNullOrEmpty(item.item_description) && item.item_description != "0")
                {
                    message += $", Description: {item.item_description}";
                }
                if (item.item_id != 0)
                {
                    message += $", ItemId: {item.item_id}";
                }
                if (item.item_hp != 0)
                {
                    message += $", HP: {item.item_hp}";
                }
                if (item.item_mp != 0)
                {
                    message += $", MP: {item.item_mp}";
                }
                if (item.item_attack != 0)
                {
                    message += $", Attack: {item.item_attack}";
                }
                if (item.item_defense != 0)
                {
                    message += $", Defense: {item.item_defense}";
                }
                if (item.item_magic != 0)
                {
                    message += $", Magic: {item.item_magic}";
                }
                if (item.item_speed != 0)
                {
                    message += $", Speed: {item.item_speed}";
                }
                if (item.item_cost != 0)
                {
                    message += $", Cost: {item.item_cost}";
                }
                if (item.require_level != 0)
                {
                    message += $", Require Level: {item.require_level}";
                }
                if (item.item_avoidance != 0)
                {
                    message += $", Avoidance: {item.item_avoidance}";
                }
                if (item.item_critical != 0)
                {
                    message += $", Critical: {item.item_critical}";
                }

                messages.Add(message);
            }

            StartCoroutine(AddMessagesWithDelay(messages));
        }
    }

    private void ShowStats()
    {
        var player = TownManager.Instance.myPlayer;  // TownManager���� ���� �÷��̾� ��������

        if (player != null)  // �÷��̾ �����ϴ� ���
        {
            string message = $"[System] StatInfo: {player.statInfo}";

            // ä�� UI�� �޽��� �߰�
            uichat.PushMessage("[System]", message, false);
            // ����� �α� ���
            Debug.Log(message);
        }
    }
    private void ShowCommands()
    {
        string nickname = "not use param";
        string commandsList =
                $"[System]: 명령어 목록은 다음과 같습니다\n" +
                $"/shop : 상품 목록을 보여줍니다\n" +
                $"/stat: 캐릭터 스텟을 보여줍니다\n" +
                $"/w 닉네임 메세지: 귓속말을 보냅니다.\n" +
                $"/t: 팀에게 채팅을 보냅니다.\n" +
                $"/createTeam: 팀을 생성합니다.\n" +
                $"/joinTeam nickname: 팀에 가입합니다.\n" +
                $"/inviteTeam nickname: 팀에게 초대합니다.\n" +
                $"/acceptTeam nickname: 초대를 수락합니다.\n" +
                $"/kickMember nickname: 팀원을 추방합니다.\n" +
                $"/memlist: 팀 멤버를 조회합니다.\n" +
                $"/buyItem itemName quantity: 아이템을 수량만큼 구매합니다.\n" +
                $"/sellItem itemName quantity: 아이템을 수량만큼 판매합니다.\n" +
                $"/equip itemId또는 /eq itemId: 장비를 장착합니다.\n" +
                $"/unequip itemType, /ueq itemType: 장비를 탈착합니다.\n";
             
        bool myChat = false;

        uichat.PushMessage(nickname, commandsList, myChat);
    }

    //    private void FunctionB(int number, float value)
    //    {
    //        Debug.Log($"FunctionB executed with number: {number} and value: {value}");
    //    }

    private void ShowInventory()
    {
        Player currentPlayer = TownManager.Instance.myPlayer;

        if (currentPlayer != null)
        {
            List<string> messages = new List<string>();

            foreach (var inventoryItem in currentPlayer.InventoryItems)
            {
                if (inventoryItem.Quantity > 0 && inventoryItem.ItemData != null)
                {
                    string message = $"[System] Item ID: {inventoryItem.ItemData.item_id}, Name: {inventoryItem.ItemData.item_name}, Type: {inventoryItem.ItemData.item_type}, Quantity: {inventoryItem.Quantity}";

                    // �� �ʵ忡 ���� 0�� �ƴ� ��쿡�� �޽����� �߰�
                    if (!string.IsNullOrEmpty(inventoryItem.ItemData.item_description) && inventoryItem.ItemData.item_description != "0")
                    {
                        message += $", Description: {inventoryItem.ItemData.item_description}";
                    }
                    if (inventoryItem.ItemData.item_hp != 0)
                    {
                        message += $", HP: {inventoryItem.ItemData.item_hp}";
                    }
                    if (inventoryItem.ItemData.item_mp != 0)
                    {
                        message += $", MP: {inventoryItem.ItemData.item_mp}";
                    }
                    if (inventoryItem.ItemData.item_attack != 0)
                    {
                        message += $", Attack: {inventoryItem.ItemData.item_attack}";
                    }
                    if (inventoryItem.ItemData.item_defense != 0)
                    {
                        message += $", Defense: {inventoryItem.ItemData.item_defense}";
                    }
                    if (inventoryItem.ItemData.item_magic != 0)
                    {
                        message += $", Magic: {inventoryItem.ItemData.item_magic}";
                    }
                    if (inventoryItem.ItemData.item_speed != 0)
                    {
                        message += $", Speed: {inventoryItem.ItemData.item_speed}";
                    }
                    //if (inventoryItem.ItemData.item_cost != 0)
                    //{
                    //    message += $", Cost: {inventoryItem.ItemData.item_cost}";
                    //}
                    if (inventoryItem.ItemData.require_level != 0)
                    {
                        message += $", Require Level: {inventoryItem.ItemData.require_level}";
                    }
                    if (inventoryItem.ItemData.item_avoidance != 0)
                    {
                        message += $", Avoidance: {inventoryItem.ItemData.item_avoidance}";
                    }
                    if (inventoryItem.ItemData.item_critical != 0)
                    {
                        message += $", Critical: {inventoryItem.ItemData.item_critical}";
                    }

                    messages.Add(message);
                    //Debug.Log(message);
                }
            }

            if (messages.Count > 0)
            {
                StartCoroutine(AddMessagesWithDelay(messages));
            }
            else
            {
                string message = "No items in inventory.";
                uichat.PushMessage("[System]", message, false);
                Debug.Log(message);
            }
        }
    }

    private void ShowEquipment()
    {
        Player currentPlayer = TownManager.Instance.myPlayer;
        EquippedItems equippedItems = currentPlayer.equippedItems;

        string message = "[System]: 장착 아이템 목록은 다음과 같습니다\n";

        message += equippedItems.Weapon != null ? $"무기: {equippedItems.Weapon.item_name}\n": $"무기: 없음\n";
        message += equippedItems.Armor != null ? $"갑옷: {equippedItems.Armor.item_name}\n" : $"갑옷: 없음\n";
        message += equippedItems.Gloves != null ? $"장갑: {equippedItems.Gloves.item_name}\n" : $"장갑: 없음\n";
        message += equippedItems.Shoes != null ? $"신발: {equippedItems.Shoes.item_name}\n" : $"신발: 없음\n";
        message += equippedItems.Accessory != null ? $"액세서리: {equippedItems.Accessory.item_name}\n" : $"액세서리: 없음\n";

        uichat.PushMessage("[System]", message, false);

    }

    //    private void FunctionB(int number, float value)
    //    {
    //        Debug.Log($"FunctionB executed with number: {number} and value: {value}");
    //    }

    //    private void FunctionC(bool flag, string text)
    //    {
    //        Debug.Log($"FunctionC executed with flag: {flag} and text: {text}");
    //    }
}
