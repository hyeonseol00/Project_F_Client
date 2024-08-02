using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatCommandManager : MonoBehaviour
{
    public static Dictionary<string, System.Action<object>> chatCommandMap { get; private set; }

    [SerializeField] private UIChat uichat;

    // Start is called before the first frame update
    void Awake()
    {
        chatCommandMap = new Dictionary<string, System.Action<object>>
        {
            { "/FunctionA", args => FunctionA((string)args) },
            { "/FunctionD", args => FunctionD((string)args) },
            { "/shop", args => ShowItems() },
            { "/stat", args => ShowStats() },
            //{ "FunctionB", args => FunctionB((int)args, (float)args) },
            //{ "FunctionC", args => FunctionC((bool)args, (string)args) }
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
            yield return new WaitForSeconds(0.05f); // 0.05�� ������
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
    //    private void FunctionB(int number, float value)
    //    {
    //        Debug.Log($"FunctionB executed with number: {number} and value: {value}");
    //    }

    //    private void FunctionC(bool flag, string text)
    //    {
    //        Debug.Log($"FunctionC executed with flag: {flag} and text: {text}");
    //    }
}
