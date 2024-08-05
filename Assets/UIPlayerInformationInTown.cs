using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInformationInTown : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv;
    [SerializeField] private TMP_Text txtName;

    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private Image imgHpBack;

    [SerializeField] private TMP_Text txtMp;
    [SerializeField] private Image imgMpFill;
    [SerializeField] private Image imgMpBack;

    [SerializeField] private TMP_Text txtGold;

    private float fullHP;
    private float curHP;

    private float fullMP;
    private float curMP;

    private float fillWidth = 441;
    private float fillHeight = 40;


    public void SetLevel(int level)
    {
        txtLv.text = $"Lv.{level}";
    }

    public void SetGold(int gold)
    {
        txtGold.text = $"Gold: {gold}";
    }

    public void Set(PlayerInfo playerInfo)
    {
        //Debug.Log($"playerInfo: {playerInfo}");
        SetName(playerInfo.Nickname);
        SetLevel(playerInfo.StatInfo.Level);
        SetFullHP(playerInfo.StatInfo.MaxHp);
        SetFullMP(playerInfo.StatInfo.MaxMp);
        SetCurHP(playerInfo.StatInfo.Hp);
        SetCurMP(playerInfo.StatInfo.Mp);
        SetGold(playerInfo.Gold);
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
    }

    public void SetFullHP(float hp, bool recover = true)
    {
        fullHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHP(hp);

        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public void SetCurHP(float hp)
    {
        curHP = Mathf.Min(hp, fullHP);
        txtHp.text = hp.ToString("0");

        float per = curHP / fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }


    public void SetFullMP(float mp, bool recover = true)
    {
        fullMP = mp;
        txtMp.text = mp.ToString("0");

        if (recover)
            SetCurMP(mp);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }

    public void SetCurMP(float mp)
    {
        curMP = Mathf.Min(mp, fullHP);
        txtMp.text = mp.ToString("0");

        float per = curMP / fullMP;
        imgMpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }
}