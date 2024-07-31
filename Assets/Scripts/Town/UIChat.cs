using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField] private Scrollbar scroll;
    [SerializeField] private RectTransform rectBg;

    [SerializeField] private Transform chatItemRoot; // ��ü ä�� ������ ��Ʈ
    [SerializeField] private Transform teamChatItemRoot; // �� ä�� ������ ��Ʈ
    [SerializeField] private Transform dmChatItemRoot; // DM ä�� ������ ��Ʈ
    [SerializeField] private Transform sysChatItemRoot; // �ý��� ä�� ������ ��Ʈ

    [SerializeField] private TMP_Text txtChatItemBase;

    [SerializeField] private TMP_InputField inputChat;
    [SerializeField] private Button btnSend;

    [SerializeField] private Button btnToggle;
    [SerializeField] private Transform icon;

    [SerializeField] private Transform alarm;
    [SerializeField] private Button tabAll;
    [SerializeField] private Button tabTeam;
    [SerializeField] private Button tabDM;
    [SerializeField] private Button tabSys;

    private float _baseChatItemWidth;

    private Player player;

    private bool isOpen = true;
    private int currentTab = 0; // 0 all 1 team 2 dm 3 system 
    private List<Transform> chatRoots;
    private Transform curChatRoots;

    private List<MessageData> allMessages = new List<MessageData>();
    private List<MessageData> teamMessages = new List<MessageData>();
    private List<MessageData> dmMessages = new List<MessageData>();
    private List<MessageData> sysMessages = new List<MessageData>();

    private void Start()
    {
        chatRoots = new List<Transform> { chatItemRoot, teamChatItemRoot, dmChatItemRoot, sysChatItemRoot };
        curChatRoots = chatItemRoot;
        _baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;

        player = TownManager.Instance.myPlayer;

        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);
        tabAll.onClick.AddListener(delegate { SwitchTab(0); }); // ��ü ä��
        tabTeam.onClick.AddListener(delegate { SwitchTab(1); }); // �� ä��
        tabDM.onClick.AddListener(delegate { SwitchTab(2); }); // dm ä��
        tabSys.onClick.AddListener(delegate { SwitchTab(3); }); // system ä��
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputChat.IsActive())
            {
                SendMessage();
            }
            else
            {
                inputChat.ActivateInputField();
            }
        }
    }

    private void ToggleChatWindow()
    {
        if (isOpen)
        {
            rectBg.DOSizeDelta(new Vector2(100, 40), 0.3f);
            icon.DORotate(new Vector3(0, 0, 180), 0.3f);
        }
        else
        {
            alarm.gameObject.SetActive(false);
            rectBg.DOSizeDelta(new Vector2(550, 500), 0.3f);
            icon.DORotate(new Vector3(0, 0, 0), 0.3f);
        }

        isOpen = !isOpen;
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(inputChat.text))
            return;

        player.SendPlayerMessage(inputChat.text);

        inputChat.text = String.Empty;
        inputChat.ActivateInputField();
    }

    public void PushMessage(string nickName, string msg, bool myChat)
    {
        if (!isOpen)
        {
            alarm.gameObject.SetActive(true);
            alarm.DOShakePosition(1f, 10);
        }

        Transform parent = null;
        List<MessageData> messageList = new List<MessageData>(); ;
        Color messageColor = Color.white;

        if (msg.StartsWith("[All]"))
        {
            parent = chatItemRoot;
            messageList = allMessages;
            messageColor = myChat ? Color.green : Color.white; // ��ü ä���� �⺻ ����
        }
        else if (msg.StartsWith("[Team]"))
        {
            parent = teamChatItemRoot;
            messageList = teamMessages;
            messageColor = new Color(135 / 255f, 206 / 255f, 235 / 255f);
        }
        else if (msg.StartsWith("[DM]"))
        {
            parent = dmChatItemRoot;
            messageList = dmMessages;
            messageColor = Color.magenta;
        }
        else if (msg.StartsWith("[System]"))
        {
            parent = sysChatItemRoot;
            messageList = sysMessages;
            messageColor = new Color(1f, 0.64f, 0f); // ��Ȳ��
        }

        var msgItem = Instantiate(txtChatItemBase);
        msgItem.color = messageColor;
        msgItem.text = $"{msg}";

        // 넣을 탭이랑 현재 보고있는 탭이 같으면 액티브를 true로 한다. 아니면 false.
        if(curChatRoots == parent) msgItem.gameObject.SetActive(true);
        else msgItem.gameObject.SetActive(false);

        // 전체 탭에서 시스템같은 다른 탭에 나와야할게 안나옴 해결코드
        if (curChatRoots == chatRoots[0])
        {
            msgItem.transform.SetParent(chatRoots[0], false);
            msgItem.gameObject.SetActive(true);
        }
        else msgItem.transform.SetParent(parent, false);

        msgItem.transform.SetAsLastSibling(); // ������ �ڽ����� ����

        var messageData = new MessageData { Message = msgItem, Timestamp = DateTime.Now };
        messageList.Add(messageData);
        allMessages.Add(messageData);

        StartCoroutine(SetTextSize(msgItem));
        StartCoroutine(ScrollToBottom());
    }

    public void SwitchTab(int tab)
    {
        currentTab = tab;
        Debug.Log(tab);

        foreach (var root in chatRoots)
        {
            root.gameObject.SetActive(true);
        }

        foreach (var root in chatRoots)
        {
            foreach (Transform child in root)
            {
                child.gameObject.SetActive(false);
            }
        }

        switch (tab)
        {
            case 0:
                foreach (var msg in allMessages)
                {
                    msg.Message.gameObject.SetActive(true);
                    msg.Message.transform.SetParent(chatItemRoot, false);

                    curChatRoots = chatRoots[0];
                }
                break;
            case 1:
                foreach (var msg in teamMessages)
                {
                    msg.Message.gameObject.SetActive(true);
                    msg.Message.transform.SetParent(teamChatItemRoot, false);

                    curChatRoots = chatRoots[1];
                }
                break;
            case 2:
                foreach (var msg in dmMessages)
                {
                    msg.Message.gameObject.SetActive(true);
                    msg.Message.transform.SetParent(dmChatItemRoot, false);

                    curChatRoots = chatRoots[2];
                }
                break;
            case 3:
                foreach (var msg in sysMessages)
                {
                    msg.Message.gameObject.SetActive(true);
                    msg.Message.transform.SetParent(sysChatItemRoot, false);

                    curChatRoots = chatRoots[3];
                }
                break;
        }

        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        scroll.value = 0;
    }

    IEnumerator SetTextSize(TMP_Text textComp)
    {
        if (textComp == null)
        {
            Debug.LogError("textComp is null");
            yield break;
        }

        while (textComp.textInfo == null)
        {
            yield return null;
        }

        if (textComp.textInfo.lineCount > 1)
            textComp.rectTransform.sizeDelta = new Vector2(_baseChatItemWidth, textComp.preferredHeight + 12);
    }
}

[Serializable]
public class MessageData
{
    public TMP_Text Message;
    public DateTime Timestamp;
}
