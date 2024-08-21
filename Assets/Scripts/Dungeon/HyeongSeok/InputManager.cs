using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private UIPotionsInformation uIPotionsInformation;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            uIPotionsInformation.usePotion(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            uIPotionsInformation.usePotion(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            uIPotionsInformation.usePotion(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            uIPotionsInformation.usePotion(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            uIPotionsInformation.usePotion(4);
        }
    }
}
