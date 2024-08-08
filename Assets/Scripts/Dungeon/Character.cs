using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
	[SerializeField] private UINameChat uiNameChat;

	public MyPlayer mPlayer { get; private set; }

	private string nickname;

	private GameObject character;
	private Animator animator;

	public int PlayerId { get; private set; }
	public bool IsMine { get; private set; }
	private bool isInit = false;

	private Vector3 lastPos;
	private float elapsedFromMovePacket;

	private bool isDead = false;

	void Start()
	{
		character = transform.Find("Player1").gameObject;
		animator = character.GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		if (isInit == false)
			return;

		if (IsMine)
			return;

		elapsedFromMovePacket += Time.deltaTime;
		if (elapsedFromMovePacket <= 0.25f)
			animator.SetFloat("Move", 1.0f);
		else
			animator.SetFloat("Move", 0.0f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "MonsterWeapon" && IsMine)
		{
			C_PlayerHit playerHitPacket = new C_PlayerHit { };

			GameManager.Network.Send(playerHitPacket);
		}
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

	public void Move(Vector3 move, Quaternion rot)
	{
		transform.position = move;
		character.transform.rotation = rot;
		elapsedFromMovePacket = 0.0f;
	}

	public void AttackMotion()
	{
		animator.SetBool("Anim1", true);
	}

	public void HitMotion()
	{
		animator.SetBool("Hit", true);
	}

	public void DeadMotion()
	{
		animator.SetBool("Die", true);
		isDead = true;
	}

	public bool getIsDead()
    {
		return isDead;
	}
}
