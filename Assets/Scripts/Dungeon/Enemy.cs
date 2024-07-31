using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public int maxHealth;
	public int curHealth;

	[SerializeField] Animator animator;
	[SerializeField] UIMonsterInformation monsterUI;

	private void Start()
	{
		monsterUI.SetFullHP(maxHealth, true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Weapon")
		{
			Attack attack = other.GetComponent<Attack>();
			curHealth -= attack.damage;
			monsterUI.SetCurHP(curHealth);

			animator.SetBool("Hit", true);
		}
	}
}
