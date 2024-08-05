using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 10;
    float rate = 1.0f;
	[SerializeField] BoxCollider meleeArea;
    [SerializeField] Animator animator;

	bool isAttackReady;

	float attackDelay;

	IEnumerator TryAttack()
    {
        animator.SetBool("Anim1", true);

		C_TryAttack tryAttackPacket = new C_TryAttack { };

		GameManager.Network.Send(tryAttackPacket);

		yield return new WaitForSeconds(0.41f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.1f);
		meleeArea.enabled = false;
	}

    void Update()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = rate < attackDelay;

        if (Input.GetButtonDown("Fire1") && isAttackReady)
        {
            StopCoroutine("TryAttack");
            StartCoroutine("TryAttack");
            attackDelay = 0.0f;
        }
    }
}
