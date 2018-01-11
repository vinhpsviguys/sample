using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUI : MonoBehaviour {

	public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
