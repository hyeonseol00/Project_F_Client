using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private UIPotionsInformation uIPotionsInformation;

    enum PotionType
    {
        HpPotion = 0, MpPotion, Elixer_S, Elixer_M, Elixer_L
    }
    // Update is called once per frame
    void Update()
    {

        foreach (int potionType in Enum.GetValues(typeof(PotionType)))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + potionType))
            {
                uIPotionsInformation.TryUsePotion(potionType);
            }
        }

        if(Input.GetKeyDown(KeyCode.B)){
            HatcheryManager.Instance.EnterSecondPhase(10.0f, 10.0f);
        }

        if (Input.GetKeyDown(KeyCode.N)){
            HatcheryManager.Instance.EnterThridPhase(30.1f);
        }
    }
}
