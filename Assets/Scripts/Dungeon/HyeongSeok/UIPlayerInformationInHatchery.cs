using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Town.Data;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInformationInHatchery : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv;
    [SerializeField] private TMP_Text txtName;

    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private Image imgHpBack;

    [SerializeField] private TMP_Text txtMp;
    [SerializeField] private Image imgMpFill;
    [SerializeField] private Image imgMpBack;

    [SerializeField] private UIPotionsInformation uIPotionsInformation;

    private float fullHP;
    private float curHP;

    private float fullMP;
    private float curMP;

    private float fillWidth = 441;
    private float fillHeight = 40;

    public int playerId { get; private set; }

    public void Set(PlayerInfo playerInfo)
    {
        //Debug.Log($"playerInfo: {playerInfo}");
        SetPlayerId(playerInfo.PlayerId);
        SetName(playerInfo.Nickname);
        SetLevel(playerInfo.StatInfo.Level);
        SetFullHP(playerInfo.StatInfo.MaxHp);
        SetFullMP(playerInfo.StatInfo.MaxMp);
        SetCurHP(playerInfo.StatInfo.Hp);
        SetCurMP(playerInfo.StatInfo.Mp);
    }

    public void SetPlayerId(int playerId)
    {
        this.playerId = playerId;
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
    }

    public void SetLevel(int level)
    {
        txtLv.text = $"Lv.{level}";
    }

    public void SetFullHP(float hp)
    {
        fullHP = hp;

        SetCurHP(curHP);

        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public bool SetCurHP(float hp)
    {
        bool isAttcked = curHP > hp;

        curHP = Mathf.Min(hp, fullHP);

        txtHp.text = $"{curHP} / {fullHP}";

        float per = curHP / fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);

        return isAttcked;
    }

    public void SetFullMP(float mp, bool recover = true)
    {
        fullMP = mp;

        SetCurMP(curMP);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }

    public void SetCurMP(float mp)
    {
        curMP = Mathf.Min(mp, fullMP);

        txtMp.text = $"{curMP} / {fullMP}";

        float per = curMP / fullMP;
        imgMpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }
    public void SetPotions(Inventory Inven, int Level)
    {
        var _potions = Inven.Items.Where(Item => 46 <= Item.Id && Item.Id <= 50);

        Dictionary<int, int> Potions = new Dictionary<int, int>();
        foreach (var potion in _potions)
        {
            //Debug.Log($"{potion.Id}, {potion.Quantity}");
            Potions.Add(potion.Id, potion.Quantity);
        }

        uIPotionsInformation.SetPotionsUI(Potions, Level);
    }
}
