using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCollision : MonoBehaviour {


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Character")
        {
            Debug.Log("aaaa");
            Destroy(this.gameObject);
        }
    }

  
}
