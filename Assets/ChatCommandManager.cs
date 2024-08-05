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
            yield return new WaitForSeconds(0.05f); // 0.05초 딜레이
        }
    }

    private void ShowItems()
    {
        var items = DataLoader.Instance.Items;  // DataLoader에서 로드된 아이템 리스트 가져오기

        if (items != null && items.Count > 0)  // 아이템 리스트가 null이 아니고, 아이템이 존재하는 경우
        {
            List<string> messages = new List<string>();

            foreach (var item in items)  // 각 아이템에 대해
            {
                if (!item.can_sell)  // can_sell이 false인 아이템은 건너뛰기
                {
                    continue;
                }

                // 기본 메시지 생성 (ID와 이름)
                string message = $"[System] Name: {item.item_name}";

                // 각 필드에 대해 0이 아닌 경우에만 메시지에 추가
                if (!string.IsNullOrEmpty(item.item_description) && item.item_description != "0")
                {
                    message += $", Description: {item.item_description}";
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
        var player = TownManager.Instance.myPlayer;  // TownManager에서 현재 플레이어 가져오기

        if (player != null)  // 플레이어가 존재하는 경우
        {
            string message = $"[System] StatInfo: {player.statInfo}";

            // 채팅 UI에 메시지 추가
            uichat.PushMessage("[System]", message, false);
            // 디버그 로그 출력
            Debug.Log(message);
        }
    }

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
                    string message = $"[System] Item ID: {inventoryItem.ItemData.item_id}, Name: {inventoryItem.ItemData.item_name}, Quantity: {inventoryItem.Quantity}";

                    // 각 필드에 대해 0이 아닌 경우에만 메시지에 추가
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
        //    private void FunctionB(int number, float value)
        //    {
        //        Debug.Log($"FunctionB executed with number: {number} and value: {value}");
        //    }

        //    private void FunctionC(bool flag, string text)
        //    {
        //        Debug.Log($"FunctionC executed with flag: {flag} and text: {text}");
        //    }
}
