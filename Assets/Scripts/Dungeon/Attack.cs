using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float rate = 1f;

    [SerializeField] BoxCollider meleeArea;
    [SerializeField] Animator animator;

    [SerializeField] bool isMelee = true;
    [SerializeField] TrailRenderer trailEffect;
    [SerializeField] Transform bulletPos;
    public GameObject bullet;
    [SerializeField] float bulletSpeed = 50.0f;
    [SerializeField] float beforeCastDelay = 0.41f; // 선딜레이

    bool isAttackReady;
    float attackDelay;

    private const float ATTACK_ENABLE_TIME = 0.1f;

    public Coroutine AttackCoroutine { get; set; }

    public void StartAttack(bool isMine, Vector3 unitDir)
    {
        AttackCoroutine = StartCoroutine(TryAttack(isMine, unitDir));
    }

    public IEnumerator TryAttack(bool isMine, Vector3 unitDir)
    {
        // 애니메이션 시작.
        animator.SetFloat("AttackSpeed", 1/rate);
        animator.SetBool("Anim1", true);
        
        yield return new WaitForSeconds(beforeCastDelay * rate);

        if (isMelee)
        {
            meleeArea.enabled = true;
            yield return new WaitForSeconds(ATTACK_ENABLE_TIME * rate);
            meleeArea.enabled = false;
        }
        else
        {
            // 탄환 생성 및 발사
            GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            instantBullet.SetActive(true);

            Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = unitDir * bulletSpeed;

            // 다른 사람이 쏜 총알이면 Bullet 태그 뺌.
            if (!isMine) instantBullet.tag = "Untagged";

        }
        AttackCoroutine = null;
    }

    void SendTryAttackPkt()
    {
        // 공격 시도 패킷 전송
        C_TryAttack tryAttackPacket;

        if (isMelee)
        {
            tryAttackPacket = new C_TryAttack { };
        }
        else
        {
            // 조준선 기준 뱡향벡터 계산.
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000f);
            Vector3 unitDir = (targetPoint - bulletPos.position).normalized;

            tryAttackPacket = new C_TryAttack
            {
                RotX = unitDir.x,
                RotY = unitDir.y,
                RotZ = unitDir.z,
            };
        }

        GameManager.Network.Send(tryAttackPacket);
    }

    void Update()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = rate < attackDelay;

        var isDead = HatcheryManager.Instance.myPlayer.isDead;
        if (Input.GetButtonDown("Fire1") && !isDead && isAttackReady)
        {
            //Debug.Log($"attackDelay {attackDelay}");
            SendTryAttackPkt();
            attackDelay = 0.0f;
        }
    }
}
