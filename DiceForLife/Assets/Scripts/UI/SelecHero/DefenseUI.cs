using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseUI : MonoBehaviour {

    public Button _closeBtn;

    public Text goldNumber, eventPointNumber, diamondNumber;

    Action<object> _UpdateTextValueEventRef;
    // Use this for initialization
    void Start () {
		
	}
    private void OnEnable()
    {
        UpdateTextValue();
        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        _closeBtn.onClick.AddListener(CloseThisDialog);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        _closeBtn.onClick.RemoveListener(CloseThisDialog);
    }

    public void UpdateTextValue()
    {
        goldNumber.text = CharacterInfo._instance._baseProperties.Gold.ToString();
        eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
        diamondNumber.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
    }

    void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
