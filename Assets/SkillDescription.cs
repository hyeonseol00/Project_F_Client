using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterClass { SpearMan = 0, SwordMan, CrossbowMan, HammerMan, Mage };

public class SkillDescription : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private BoxCollider weapon;
    [SerializeField] private ThirdPersonController thirdPersonControllerScript;
    [SerializeField] private Attack attackScript;
    [SerializeField] private Transform weaponPrefab;
    [SerializeField] private Transform CharacterPrefab;

    private Coroutine activatedSkillCoroutine = null;
    private Coroutine coolTimeCoroutine = null;
    private Image disableImage;
    private Image disableImage2;
    private Image skillIconImage;

    private TMP_Text skillmanaCostTxt;
    private TMP_Text MpText;

    public CharacterClass Class;

    // skill coolTime
    private const float SPEAR_MAN_COOLTIME = 15.0f;
    private const float SWORD_MAN_COOLTIME = 20.0f;
    private const float CROSSBOW_MAN_COOLTIME = 15.0f;
    private const float HAMMER_MAN_COOLTIME = 15.0f;
    private const float MAGE_COOLTIME = 20.0f;

    // skill activeTime
    private const float SPEAR_MAN_ACTIVETIME = 5.0f;
    private const float SWORD_MAN_ACTIVETIME = 10.0f;
    private const float CROSSBOW_MAN_ACTIVETIME = 7.5f;
    private const float HAMMER_MAN_ACTIVETIME = 10.0f;
    private const float MAGE_ACTIVETIME = 10.0f;

    // mana
    private const int SPEAR_MAN_MANACOST = 100;
    private const int SWORD_MAN_MANACOST = 200;
    private const int CROSSBOW_MAN_MANACOST = 300;
    private const int HAMMER_MAN_MANACOST = 400;
    private const int MAGE_MANACOST = 500;

    private const float fillHeight = 150;
    private const float fillWidth = 150;

    private const float MAX_RATE = 0.2f;    // 최대 공속

    private int manaCost;

    private void Start()
    {
        disableImage = GameObject.Find("UIBattle/SkillIcon/DisableImage").GetComponent<Image>();
        disableImage2 = GameObject.Find("UIBattle/SkillIcon/DisableImage2").GetComponent<Image>();
        skillIconImage = GameObject.Find("UIBattle/SkillIcon/Image").GetComponent<Image>();
        //skillmanaCostTxt = GameObject.Find("UIBattle/SkillIcon/txtSkillCost").GetComponent<TMP_Text>();
        //MpText = GameObject.Find("UIPlayerInfo/MyInfo/Image/imgMpBack/image").GetComponent<TMP_Text>();

        switch (Class)
        {
            case CharacterClass.SpearMan:
                manaCost = SPEAR_MAN_MANACOST;
                break;
            case CharacterClass.SwordMan:
                manaCost = SWORD_MAN_MANACOST;
                break;
            case CharacterClass.CrossbowMan:
                manaCost = CROSSBOW_MAN_MANACOST;
                break;
            case CharacterClass.HammerMan:
                manaCost = HAMMER_MAN_MANACOST;
                break;
            case CharacterClass.Mage:
                manaCost = MAGE_MANACOST;
                break;
            default:
                break;   
        }

        skillmanaCostTxt.text = manaCost.ToString();
    }

    //private void Update()
    //{
    //    if (int.Parse(MpText.text) < manaCost)
    //    {
    //        disableImage2.enabled = true;
    //    }
    //    else
    //    {
    //        disableImage2.enabled = false;
    //    }
    //}

    public void trySkill()
    {
        C_TrySkill trySkillPkt = null;
        if (activatedSkillCoroutine == null && coolTimeCoroutine == null && !disableImage2.enabled)
        {
            switch (Class)
            {
                case CharacterClass.SpearMan:
                    trySkillPkt = new C_TrySkill
                    {
                        SkillTime = SPEAR_MAN_ACTIVETIME
                    };
                    GameManager.Network.Send(trySkillPkt);
                    //activatedSkillCoroutine = StartCoroutine("ProcessSpearManSkill", SPEAR_MAN_ACTIVETIME);
                    break;
                case CharacterClass.SwordMan:
                    trySkillPkt = new C_TrySkill
                    {
                        SkillTime = SWORD_MAN_ACTIVETIME
                    };
                    GameManager.Network.Send(trySkillPkt);
                    //activatedSkillCoroutine = StartCoroutine("ProcessSwordManSkill", SWORD_MAN_ACTIVETIME);
                    break;
                case CharacterClass.CrossbowMan:
                    trySkillPkt = new C_TrySkill
                    {
                        SkillTime = CROSSBOW_MAN_ACTIVETIME
                    };
                    GameManager.Network.Send(trySkillPkt);
                    //activatedSkillCoroutine = StartCoroutine("ProcessCrossbowManSkill", CROSSBOW_MAN_ACTIVETIME);
                    break;
                case CharacterClass.HammerMan:
                    trySkillPkt = new C_TrySkill
                    {
                        SkillTime = HAMMER_MAN_ACTIVETIME
                    };
                    GameManager.Network.Send(trySkillPkt);
                    //activatedSkillCoroutine = StartCoroutine("ProcessHammerManSkill", HAMMER_MAN_ACTIVETIME);
                    break;
                case CharacterClass.Mage:
                    trySkillPkt = new C_TrySkill
                    {
                        SkillTime = MAGE_ACTIVETIME
                    };
                    GameManager.Network.Send(trySkillPkt);
                    //activatedSkillCoroutine = StartCoroutine("ProcessMageSkill", MAGE_ACTIVETIME);
                    break;
                default:
                    break;
            }     
        }
        else
        {
            Debug.Log("쿨타임 중이거나, 스킬이 활성화된 상태이거나, 마나가 부족합니다");
        }

    }

    public void useSkill()
    {    
        switch (Class)
        {
            case CharacterClass.SpearMan:
                activatedSkillCoroutine = StartCoroutine("ProcessSpearManSkill", SPEAR_MAN_ACTIVETIME);
                break;
            case CharacterClass.SwordMan:
                activatedSkillCoroutine = StartCoroutine("ProcessSwordManSkill", SWORD_MAN_ACTIVETIME);
                break;
            case CharacterClass.CrossbowMan:
                activatedSkillCoroutine = StartCoroutine("ProcessCrossbowManSkill", CROSSBOW_MAN_ACTIVETIME);
                break;
            case CharacterClass.HammerMan:
                activatedSkillCoroutine = StartCoroutine("ProcessHammerManSkill", HAMMER_MAN_ACTIVETIME);
                break;
            case CharacterClass.Mage:
                activatedSkillCoroutine = StartCoroutine("ProcessMageSkill", MAGE_ACTIVETIME);
                break;
            default:
                break;
        }
    }

    // =====================클래스 스킬 시작============================
    private IEnumerator ProcessSpearManSkill(float activatingTime)
    {
        var originalRate = attackScript.rate;

        // 스킬 활성화 코드
        Debug.Log("창술사 스킬 활성화!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 0.5f, 0.5f); // 스킬 사용 중 표시

        // 이속, 공속 상승
        thirdPersonControllerScript.speed *= 5f;   
        attackScript.rate = attackScript.rate / 3f < MAX_RATE ? MAX_RATE : attackScript.rate / 3f;    
        // 공격 범위 조정
        weapon.center = new Vector3(0.0f, 0.9f, 3.0f);          
        weapon.size = new Vector3(1.5f, 1.5f, 5.0f);
        weaponPrefab.localScale = new Vector3(20.0f, 20.0f, 20.0f);
        yield return new WaitForSeconds(activatingTime);


        // 스킬 비활성화 코드
        Debug.Log("창술사 스킬 끝!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);// 스킬 사용 중 색깔 제자리로

        // 이속, 공속 정상화
        thirdPersonControllerScript.speed /= 5f;
        attackScript.rate = originalRate;
        // 공격 범위 정상화
        weapon.center = new Vector3(0.0f, 0.9f, 1.0f);
        weapon.size = new Vector3(1.5f, 1.5f, 1.5f);
        weaponPrefab.localScale = new Vector3(5.0f, 5.0f, 5.0f);

        // 쿨타임 시작
        activatedSkillCoroutine = null;
        yield return coolTimeCoroutine = StartCoroutine("coolTime", SPEAR_MAN_COOLTIME);
    }

    private IEnumerator ProcessSwordManSkill(float activatingTime)
    {
        // 스킬 활성화 코드
        Debug.Log("전사 스킬 활성화!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 0.5f, 0.5f); // 스킬 사용 중 표시
        yield return new WaitForSeconds(activatingTime);

        // 스킬 비활성화 코드
        Debug.Log("전사 스킬 끝!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);// 스킬 사용 중 색깔 제자리로

        // 쿨타임 시작
        activatedSkillCoroutine = null;
        yield return coolTimeCoroutine = StartCoroutine("coolTime", SWORD_MAN_COOLTIME);
    }

    private IEnumerator ProcessCrossbowManSkill(float activatingTime)
    {
        var originalRate = attackScript.rate;
        
        // 스킬 활성화 코드
        Debug.Log("석궁 스킬 활성화!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 0.5f, 0.5f); // 스킬 사용 중 표시
        attackScript.rate = MAX_RATE;
        yield return new WaitForSeconds(activatingTime);

        // 스킬 비활성화 코드
        Debug.Log("석궁 스킬 끝!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f); // 스킬 사용 중 색깔 제자리로
        attackScript.rate = originalRate;

        // 쿨타임 시작
        activatedSkillCoroutine = null;
        yield return coolTimeCoroutine = StartCoroutine("coolTime", CROSSBOW_MAN_COOLTIME);
    }

    private IEnumerator ProcessHammerManSkill(float activatingTime)
    {

        // 스킬 활성화 코드
        Debug.Log("해머 스킬 활성화!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 0.5f, 0.5f); // 스킬 사용 중 표시

        // 공속 감소, 이속 감소, 캐릭터 크기 증가
        attackScript.rate *= 2.5f;
        thirdPersonControllerScript.speed /= 2f;
        CharacterPrefab.localScale *= 2.0f;
        yield return new WaitForSeconds(activatingTime);

        // 스킬 비활성화 코드
        Debug.Log("해머 스킬 끝!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);// 스킬 사용 중 색깔 제자리로

        // 공속, 이속, 캐릭터 크기 정상화
        attackScript.rate /= 2.5f;
        thirdPersonControllerScript.speed *= 2f;
        CharacterPrefab.localScale /= 2.0f;

        // 쿨타임 시작
        activatedSkillCoroutine = null;
        yield return coolTimeCoroutine = StartCoroutine("coolTime", HAMMER_MAN_COOLTIME);
    }

    private IEnumerator ProcessMageSkill(float activatingTime)
    {
        // 스킬 활성화 코드
        Debug.Log("마법사 스킬 활성화!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 0.5f, 0.5f); // 스킬 사용 중 표시
        attackScript.bullet.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        yield return new WaitForSeconds(activatingTime);

        // 스킬 비활성화 코드
        Debug.Log("마법사 스킬 끝!");
        skillIconImage.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);// 스킬 사용 중 색깔 제자리로
        attackScript.bullet.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        // 쿨타임 시작
        activatedSkillCoroutine = null;
        yield return coolTimeCoroutine = StartCoroutine("coolTime", MAGE_COOLTIME);
    }

    // =====================클래스 스킬 끝============================

    private IEnumerator coolTime(float coolTime)
    {
        float remainingTime = coolTime;

        disableImage.enabled = true;
        while (remainingTime - Time.deltaTime > 0)
        {
            // 남은 시간 갱신
            remainingTime -= Time.deltaTime;
            disableImage.rectTransform.sizeDelta = new Vector2(fillHeight * remainingTime / coolTime, fillWidth);
            yield return null; // 다음 프레임까지 대기
        }
        disableImage.enabled = false;
        coolTimeCoroutine = null;
    }
}
