using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public float speed;

	[SerializeField] Animator animator;
	[SerializeField] UIMonsterInformation monsterUI;
	[SerializeField] RectTransform winPopup;

	Vector3 unitVector;
	bool isDead = false;

	private void Update()
	{
		Move();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Weapon")
		{
			// Attack attack = other.GetComponent<Attack>();
			// curHealth -= attack.damage;
			// monsterUI.SetCurHP(curHealth);

			C_AttackBoss attackBossPacket = new C_AttackBoss { };

			GameManager.Network.Send(attackBossPacket);

			// animator.SetBool("Hit", true);
		}
	}

	public void HitAnimation()
	{
		animator.SetBool("Hit", true);
	}

	public void SetCoordinates(Vector3 move, Vector3 eRot)
	{
		if (isDead)
			return;

		var pos = move;
		pos.y = transform.position.y;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(eRot);
	}

	public void SetUnitVector(Vector3 vec)
	{
		unitVector = vec;
	}

	private void Move()
	{
		if (isDead)
			return;

		var lastPos = transform.position;
		transform.position += unitVector * Time.deltaTime * speed;

		animator.SetFloat("Move", Vector3.Distance(lastPos, transform.position));

		if (transform.position.y <= -30.0f)
		{
			var pos = new Vector3(transform.position.x, -19.5f, transform.position.z);
			transform.position = pos;
		}

	}

	public void Dead()
	{
		isDead = true;
		animator.SetBool("Die", true);
		winPopup.gameObject.SetActive(true);
	}
}
