using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationTest : MonoBehaviour
{
    Player mPlayer;
    private Animator animator;
    private bool isLoad = false;
    // Start is called before the first frame update
    void Start()
    {
        //mPlayer = TownManager.Instance.myPlayer;
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLoad && TownManager.Instance.myPlayer)
        {
            mPlayer = TownManager.Instance.myPlayer;
            animator = mPlayer.GetComponent<Animator>();
            isLoad = true;
        }


        if (isLoad)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("111111111111");
                Animation("Anim4");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("22222222222222222");
                Animation("Anim5");
            }
        }
        
    }

    public void Animation(string animName)
    {
        if (animator)
        {
            Debug.Log("3333333333333");
            animator.SetTrigger(animName);
        }
     
    }
}
