using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button btn;
    [SerializeField] private Button rightBtn;
    [SerializeField] private TextMeshProUGUI btnText;

    private MyPlayer mPlayer;

    string[] animName = { "Happy", "Sad", "Hi", "Dance1", "Dance2", "Dance3", "Dance4", "Dance5", "Dance6" };
    private int animIdx = 0;

    void Start()
    {

        btn.onClick.AddListener(() =>
        {
            PlayAnimation();
        });

        leftBtn.onClick.AddListener(() =>
        {
            animIdx = animIdx - 1 >= 0 ? animIdx - 1 : animName.Length - 1;
            btnText.text = animName[animIdx];
        });

        rightBtn.onClick.AddListener(() =>
        {
            animIdx = (animIdx + 1) % animName.Length;
            btnText.text = animName[animIdx];
        });

        mPlayer = TownManager.Instance.myPlayer.mPlayer;
    }
    
    private void PlayAnimation()
    {
        if (mPlayer == null)
            return;

        mPlayer.AnimationExecute(animIdx);
    }
}
