using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Button btnBattle;
    [SerializeField] private Button[] btnList;
    [SerializeField] private TMP_Text goldTxt;

    private MyPlayer mPlayer;
    Player currentPlayer;


    void Start()
    {
        currentPlayer = TownManager.Instance.myPlayer;

        // TownManager�� �̱��� �ν��Ͻ��� ���� myPlayer�� ����
       

        Debug.Log(currentPlayer.gold);

        goldTxt.text =  currentPlayer.gold.ToString();

        for (int i = 0; i < btnList.Length; i++)
        {
            int idx = i;
            btnList[i].onClick.AddListener(() =>
            {
                PlayAnimation(idx);
            });
        }
        
        mPlayer = TownManager.Instance.myPlayer.mPlayer;
    }
    
    private void PlayAnimation(int idx)
    {
        if (mPlayer == null)
            return;

        mPlayer.AnimationExecute(idx);
    }
}
