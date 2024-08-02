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

    //    private void FunctionB(int number, float value)
    //    {
    //        Debug.Log($"FunctionB executed with number: {number} and value: {value}");
    //    }

    //    private void FunctionC(bool flag, string text)
    //    {
    //        Debug.Log($"FunctionC executed with flag: {flag} and text: {text}");
    //    }
}
