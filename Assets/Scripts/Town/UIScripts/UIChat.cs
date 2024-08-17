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

    [SerializeField] private Transform chatItemRoot;
    [SerializeField] private TMP_Text txtChatItemBase;

    [SerializeField] private TMP_InputField inputChat;
    [SerializeField] private Button btnSend;

    [SerializeField] private Button btnToggle;
    [SerializeField] private Transform icon;

    [SerializeField] private Transform alarm;

    [SerializeField] private Button btnAllTab;
    [SerializeField] private Button btnTeamTab;
    [SerializeField] private Button btnSystemTab;
    [SerializeField] private Button btnDMTab;

    private float _baseChatItemWidth;

    private Player player;

    private bool isOpen = true;

    private List<GameObject> chatList = new List<GameObject>();

    enum TabType { All, Team, System, DM };

    private const int MAX_MESSAGES = 50;

    private void Start()
    {
        _baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;

        player = TownManager.Instance.myPlayer;

        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);

        btnAllTab.onClick.AddListener(() => filterChat(TabType.All));
        btnTeamTab.onClick.AddListener(() => filterChat(TabType.Team));
        btnSystemTab.onClick.AddListener(() => filterChat(TabType.System));
        btnDMTab.onClick.AddListener(() => filterChat(TabType.DM));
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

        StopAllCoroutines();

        var msgItem = Instantiate(txtChatItemBase, chatItemRoot);

        msgItem.text = $"{msg}";

        //      AdjustTextContainerHeight(msgItem); // 적절한 높이로 msgItem를 조절

        if (msg.StartsWith("[All]"))
        {
            msgItem.color = myChat ? Color.green : Color.white;
        }
        else if (msg.StartsWith("[Team]"))
        {
            msgItem.color = new Color(135 / 255f, 206 / 255f, 235 / 255f);
        }
        else if (msg.StartsWith("[DM]"))
        {
            msgItem.color = Color.magenta;
        }
        else if (msg.StartsWith("[System]"))
        {
            msgItem.color = new Color(1f, 0.64f, 0f);
        }
        else
        {
            msgItem.color = Color.red;
        }

        msgItem.gameObject.SetActive(true);

        StartCoroutine(SetTextSize(msgItem));
        StartCoroutine(ScrollToBottom());

        chatList.Add(msgItem.gameObject);

        if(chatList.Count > MAX_MESSAGES)
        {
            Destroy(chatList[0]);
            // Debug.Log($"{chatList[0].GetComponent<TMP_Text>().text}");
            chatList.RemoveAt(0);
        }
    }

    //    private void AdjustTextContainerHeight(TMP_Text tmpText)
    //    {
    //        // 텍스트가 업데이트된 후 레이아웃을 강제로 갱신
    //        Canvas.ForceUpdateCanvases();
    //        tmpText.ForceMeshUpdate();

    //        // 텍스트 내용에 맞는 이상적인 높이 계산
    //        float preferredHeight = tmpText.preferredHeight;

    //        // 텍스트 컨테이너(RectTransform)의 높이를 이상적인 높이로 설정
    //        var sizeDelta = tmpText.rectTransform.sizeDelta;
    //        sizeDelta.y = preferredHeight;
    //        tmpText.rectTransform.sizeDelta = sizeDelta;
    //    }

    private void filterChat(TabType TabNo)
    {

        switch (TabNo)
        {
            case TabType.All:
                foreach (GameObject txtChat in chatList)
                {
                    var txtContainer = txtChat.GetComponent<TMP_Text>();
                    txtContainer.gameObject.SetActive(true);
                }
                break;
            case TabType.Team:
                foreach (GameObject txtChat in chatList)
                {
                    var txtContainer = txtChat.GetComponent<TMP_Text>();

                    if (txtContainer.text.StartsWith("[Team]")) txtContainer.gameObject.SetActive(true);
                    else txtContainer.gameObject.SetActive(false);

                }
                break;
            case TabType.System:
                foreach (GameObject txtChat in chatList)
                {
                    var txtContainer = txtChat.GetComponent<TMP_Text>();

                    if (txtContainer.text.StartsWith("[System]")) txtContainer.gameObject.SetActive(true);
                    else txtContainer.gameObject.SetActive(false);

                }
                break;
            case TabType.DM:
                foreach (GameObject txtChat in chatList)
                {
                    var txtContainer = txtChat.GetComponent<TMP_Text>();

                    if (txtContainer.text.StartsWith("[DM]")) txtContainer.gameObject.SetActive(true);
                    else txtContainer.gameObject.SetActive(false);

                }
                break;
        }

    }
    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        scroll.value = 0;
    }

    IEnumerator SetTextSize(TMP_Text textComp)
    {
        yield return new WaitForEndOfFrame();

        if (textComp.textInfo.lineCount > 1)
            textComp.rectTransform.sizeDelta = new Vector2(_baseChatItemWidth, textComp.preferredHeight + 12);
    }
}