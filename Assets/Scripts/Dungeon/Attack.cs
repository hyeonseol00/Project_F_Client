using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    float rate = 1.0f;
    [SerializeField] BoxCollider meleeArea;
    [SerializeField] Animator animator;

    [SerializeField] bool isMelee = true;
    [SerializeField] TrailRenderer trailEffect;
    [SerializeField] Transform bulletPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed = 50.0f;
    [SerializeField] float beforeCastDelay = 0.41f;

    bool isAttackReady;
    float attackDelay;
    

    IEnumerator TryAttack()
    {
        animator.SetBool("Anim1", true);

        // 다른 유저에게 공격 모션 패킷 전송
        C_TryAttack tryAttackPacket = new C_TryAttack { };
        GameManager.Network.Send(tryAttackPacket);

        yield return new WaitForSeconds(beforeCastDelay);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
    }

    IEnumerator TryShot()
    {
        animator.SetBool("Anim1", true);

        // 다른 유저에게 공격 모션 패킷 전송
        C_TryAttack tryAttackPacket = new C_TryAttack { };
        GameManager.Network.Send(tryAttackPacket);

        yield return new WaitForSeconds(beforeCastDelay);

        // 조준선 기준 뱡향벡터 계산.
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000f);
        Vector3 unitDir = (targetPoint - bulletPos.position).normalized;

        // 탄환 생성 및 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);

        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = unitDir * bulletSpeed;

        yield return null;
    }

    void Update()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = rate < attackDelay;

        var isDead = HatcheryManager.Instance.myPlayer.isDead;
        if (Input.GetButtonDown("Fire1") && isAttackReady && !isDead)
        {
            if (isMelee)    // 근거리
            {
                StopCoroutine("TryAttack");
                StartCoroutine("TryAttack");
                attackDelay = 0.0f;
            }
            else           // 원거리
            {
                StopCoroutine("TryShot");
                StartCoroutine("TryShot");
                attackDelay = 0.0f;
            }

        }
    }
}
