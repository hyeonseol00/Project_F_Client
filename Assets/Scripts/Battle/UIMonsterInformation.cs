using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterInformation : MonoBehaviour
{
    private Transform camTr;

    [SerializeField] private GameObject checkArrow;

    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgNameBg;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private TMP_Text txtNameOnMonster;
    [SerializeField] private GameObject UpgradedMonsterFX1;
    [SerializeField] private GameObject UpgradedMonsterFX2;

    public float fillWidth = 180;
    public float fillHeight = 30;

    private float fullHP;
    private float curHP;

    private void Start()
    {
        if (Camera.main)
            camTr = Camera.main.transform;

    }

    private void Update()
    {
        if(camTr != null)
            transform.rotation = camTr.rotation;
    }

    public void SetMainCamera()
    {
        camTr = Camera.main.transform;
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
        if(nickname == "팬텀나이트" || nickname == "리저드킹" || nickname == "나이트로드")
        {
            txtNameOnMonster.text = nickname;
            if (nickname != "팬텀나이트")
            {
                UpgradedMonsterFX1.SetActive(true);
                UpgradedMonsterFX2.SetActive(true);
            }
        }
        
        imgNameBg.rectTransform.sizeDelta = new Vector2(txtName.preferredWidth + 50, 50);
    }

    public void SetFullHP(float hp, bool recover = true)
    {
        fullHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHP(hp);
    }

    public void SetCurHP(float hp)
    {
        curHP = Mathf.Min(hp, fullHP);
        txtHp.text = hp.ToString("0");

        float per = curHP / fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);
    }

    public float GetCurHP()
    {
        return curHP;
    }
}
