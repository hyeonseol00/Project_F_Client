using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject charList;
    [SerializeField] private Button[] charBtns;
    
    [SerializeField] private Button btnConfirm;
	[SerializeField] private Button btnRegister;
	[SerializeField] private Button btnBack;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text txtMessage;
    private TMP_Text nicknamePlaceHolder;
    private TMP_Text passwordPlaceHolder;
	[SerializeField] private TMP_Text confirmBtnPlaceHolder;


	private int classIdx = 0;

    private string serverUrl;
    private string nickname;
    private string port;
    
    void Start()
    {
        nicknamePlaceHolder = inputNickname.placeholder.GetComponent<TMP_Text>();
        passwordPlaceHolder = inputPassword.placeholder.GetComponent<TMP_Text>();
        btnBack.onClick.AddListener(SetServerUI);
        btnRegister.onClick.AddListener(RequestRegist);
        
        SetServerUI();
        
        for (int i = 0; i < charBtns.Length; i++)
        {
            int idx = i;
            charBtns[i].onClick.AddListener(() =>
            {
                SelectCharacter(idx);
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputNickname.IsActive())
                btnConfirm.onClick.Invoke();
        }
    }

    void SelectCharacter(int idx)
    {
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(false);
        
        classIdx = idx;
        
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(true);
    }

    void SetServerUI()
    {
        txtMessage.color = UnityEngine.Color.white;
        txtMessage.text = "Welcome!";

        inputNickname.text = string.Empty;
        nicknamePlaceHolder.text = "서버주소를 입력해주세요!";

		inputPassword.text = string.Empty;
		passwordPlaceHolder.text = "포트번호를 입력해주세요!";

        confirmBtnPlaceHolder.text = "확인";

        btnRegister.gameObject.SetActive(false);
		charList.gameObject.SetActive(false);
        // btnBack.gameObject.SetActive(false);
        
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmServer);
    }

    void SetNicknameUI()
    {
        txtMessage.color = UnityEngine.Color.white;
        txtMessage.text = "Welcome!";

        inputNickname.text = string.Empty;
        nicknamePlaceHolder.text = "닉네임을 입력해주세요.";

		inputPassword.text = string.Empty;
		passwordPlaceHolder.text = "비밀번호를 입력해주세요.";

		confirmBtnPlaceHolder.text = "로그인";

		btnRegister.gameObject.SetActive(true);
		charList.gameObject.SetActive(true);
        // btnBack.gameObject.SetActive(true);
        
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmNickname);
    }

    void ConfirmServer()
    {        
        txtMessage.color = UnityEngine.Color.red;
        // if (string.IsNullOrEmpty(inputNickname.text))
        // {
        //     txtMessage.text = "서버주소를 입력해주세요!!";
        //     return;
        // }

        serverUrl = inputNickname.text;
        port = inputPassword.text;

        try
        {
            TownManager.Instance.ConnectServer(serverUrl, port);
        }
        catch
		{
			txtMessage.text = "서버 연결에 실패했습니다.";
			return;
		}

		SetNicknameUI();
    }
    
    void ConfirmNickname()
    {
        txtMessage.color = UnityEngine.Color.red;
        
        if (inputNickname.text.Length < 2)
        {
            txtMessage.text = "이름을 2글자 이상 입력해주세요!";
            return;
        }

        if (inputNickname.text.Length > 10)
        {
            txtMessage.text = "이름을 10글자 이하로 입력해주세요!";
            return;
		}

		if (inputPassword.text.Length < 2)
		{
			txtMessage.text = "비밀번호를 6글자 이상 입력해주세요!";
			return;
		}

		RequestLogIn();
    }

    void RequestRegist()
	{
        C_Register registerPacket = new C_Register
        {
            Nickname = inputNickname.text,
            Password = inputPassword.text
		};

		GameManager.Network.Send(registerPacket);
	}

    public void SuccessRegist()
	{
		txtMessage.color = UnityEngine.Color.white;
		txtMessage.text = "회원가입에 성공했습니다!";
		inputNickname.text = string.Empty;
	}

    public void FailRegist()
	{
		txtMessage.color = UnityEngine.Color.red;
		txtMessage.text = "회원가입에 실패했습니다.";
		inputNickname.text = string.Empty;
		inputPassword.text = string.Empty;
	}

    void RequestLogIn()
	{
		C_LogIn logInPacket = new C_LogIn
		{
			Nickname = inputNickname.text,
			Password = inputPassword.text
		};

		GameManager.Network.Send(logInPacket);
	}

	public void FailLogIn()
	{
		txtMessage.color = UnityEngine.Color.red;
		txtMessage.text = "로그인에 실패했습니다.";
	}

	public void StartGame()
	{
		nickname = inputNickname.text;

		TownManager.Instance.GameStart(nickname, classIdx);
		gameObject.SetActive(false);
	}
}
