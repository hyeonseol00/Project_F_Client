using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
	[SerializeField] BoxCollider meleeArea;
    [SerializeField] Animator animator;


    IEnumerator TryAttack()
    {
        animator.SetBool("Attack2", true);

        yield return new WaitForSeconds(0.41f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
    }

    public void TryBossAttack()
    {
        StopCoroutine("TryAttack");
        StartCoroutine("TryAttack");
    }
}
