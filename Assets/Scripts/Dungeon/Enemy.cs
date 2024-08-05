using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	int maxHealth;
	int curHealth;

	[SerializeField] Animator animator;
	[SerializeField] UIMonsterInformation monsterUI;

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
}
