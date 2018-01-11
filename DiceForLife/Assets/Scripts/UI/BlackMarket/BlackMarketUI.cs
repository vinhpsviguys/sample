using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackMarketUI : MonoBehaviour {

    public GameObject _sellPanel, _buyPanel;
    public Button _sellBtn, _buyBtn;

    public GameObject _selectItemPanel;


    public Text goldNumber, eventPointNumber, diamondNumber;

    Action<object> _UpdateTextValueEventRef;


    private void OnEnable()
    {
        UpdateTextValue();
        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        OpenBuyTabPanel();
        _sellBtn.onClick.AddListener(OpenSellTabPanel);
        _buyBtn.onClick.AddListener(OpenBuyTabPanel);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        _sellBtn.onClick.RemoveListener(OpenSellTabPanel);
        _buyBtn.onClick.RemoveListener(OpenBuyTabPanel);
    }

    public void UpdateTextValue()
    {
        goldNumber.text = CharacterInfo._instance._baseProperties.Gold.ToString();
        eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
        diamondNumber.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
    }


    void OpenSellTabPanel()
    {
        _sellPanel.SetActive(true);
        _buyPanel.SetActive(false);
    }
    void OpenBuyTabPanel()
    {
        _sellPanel.SetActive(false);
        _buyPanel.SetActive(true);
    }

    public void OpenSelectItemPanel()
    {
        _selectItemPanel.SetActive(true);
    }

    public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
