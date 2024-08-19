using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const int GROUND_LAYER = 9;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GROUND_LAYER || other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }

}
