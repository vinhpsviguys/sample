using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoDestroyUI : MonoBehaviour {

    public float lifetime=1f;
    private void Start()
    {
        moveUp();
    }

    void moveUp()
    {
        
        this.transform.DOMoveY(this.transform.position.y + 2.5f, 2.5f).OnComplete(() => Destroy(this.gameObject));
    }
}
