using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
	[SerializeField] private UINameChat uiNameChat;

	public Avatar avatar { get; private set; }
	public MyPlayer mPlayer { get; private set; }

	private string nickname;

	private UIChat uiChat;

	private Animator animator;

	public int PlayerId { get; private set; }
	public bool IsMine { get; private set; }
	private bool isInit = false;

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

		if (!IsMine)
		{
			Destroy(gameObject.GetComponent<NavMeshAgent>());

			GameObject cameraArm = transform.Find("CameraArm").gameObject;
			Destroy(cameraArm);

			ThirdPersonController thirdPersonController = gameObject.GetComponent<ThirdPersonController>();
			Destroy(thirdPersonController);

			GameObject weapon = transform.Find("Player1").Find("Weapon").gameObject;
			Destroy(weapon);
		}

		isInit = true;
	}


	private void Update()
	{
		if (isInit == false)
			return;

		if (IsMine)
			return;
	}
}
