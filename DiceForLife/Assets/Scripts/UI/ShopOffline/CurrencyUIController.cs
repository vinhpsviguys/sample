using System;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUIController : MonoBehaviour
{
    [SerializeField] private Text _txtGold, _txtDiamond;
    //[SerializeField] private GameObject _moreGoldPrefabs, _moreDiamondPrefabs;

    //private GameObject _moreGoldObject, _moreDiamondObject;
    // Use this for initialization
    Action<object> _UpdateTextValueEventRef;

    private void OnEnable()
    {
        _UpdateTextValueEventRef = (param) => UpdateCurrency();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        UpdateCurrency();
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
    }
    private void UpdateCurrency()
    {
        _txtGold.text = CharacterInfo._instance._baseProperties.Gold.ToString();
        //eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
        _txtDiamond.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
    }
    //public void BtnGoldClick()
    //{

    //}
    //public void BtnDiamondClick()
    //{

    //}
}
