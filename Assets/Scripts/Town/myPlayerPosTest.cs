using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class myPlyerPosTest : MonoBehaviour
{

    Transform t;
    MeshRenderer msr;

    void Start()
    {
        t = GetComponent<Transform>();
        msr= GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        // TownManager의 싱글톤 인스턴스를 통해 myPlayer에 접근
        Player currentPlayer = TownManager.Instance.myPlayer;
        
        if(currentPlayer != null)
        {

            float distance = Vector3.Distance(currentPlayer.transform.position, t.position);

            if (distance <= 5.0f){

                msr.enabled = false;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EnterDungeon();
                }
            }
            else
            {
                msr.enabled = true;
            }
        }
        
    }

    private void EnterDungeon()
    {
        C_EnterHatchery enterPacket = new C_EnterHatchery { };
        GameManager.Network.Send(enterPacket);
    }
}
