using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyAnimation : MonoBehaviour {

    private Animator anim;
    void Start()

    {

        anim = GetComponent<Animator>();

    }

    public void DestroyObjectWhenAnimationEnd()
    {
        Destroy(this.gameObject);
    }
    
}
