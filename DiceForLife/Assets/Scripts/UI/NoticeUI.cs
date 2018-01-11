using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeUI : MonoBehaviour {

    public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
