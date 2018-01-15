using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogScript : MonoBehaviour {


    private Toggle toggle;
	// Use this for initialization
	void Start () {
        toggle = GetComponent<Toggle>();
        toggle.isOn = NewAdapter.isLog;
	}

    public void click() {

        NewAdapter.isLog = toggle.isOn;
        Debug.Log(NewAdapter.isLog);
    }
}
