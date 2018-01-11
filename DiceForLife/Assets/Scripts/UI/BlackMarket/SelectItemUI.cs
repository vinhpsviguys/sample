using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SelectItemUI : MonoBehaviour, IPointerDownHandler
{
    public GameObject _itemPopupPanel,_itemPopupInfo, _itemPopupNumber;
    public Transform _contentItem;

    private void OnEnable()
    {
        _itemPopupPanel.SetActive(false);
        _itemPopupInfo.SetActive(true);
        _itemPopupNumber.SetActive(false);
        foreach (Transform child in _contentItem)
        {
            child.GetComponent<Button>().onClick.AddListener(ShowInfoItem);
        }
    }
    private void OnDisable()
    {
        foreach (Transform child in _contentItem)
        {
            child.GetComponent<Button>().onClick.RemoveListener(ShowInfoItem);
        }
    }

    public void ShowInfoItem()
    {
        _itemPopupPanel.SetActive(true);
    }

    public void ChooseNumberItemToSell()
    {
        _itemPopupNumber.SetActive(true);
    }

    public void ConfirmSellItem()
    {
        this.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter as GameObject;
        Debug.Log(enterObj);
        if (enterObj.name == "selecItemPanel")
        {
            this.gameObject.SetActive(false);
        }
        if (enterObj.name == "ItemPopupPanel")
        {
            _itemPopupPanel.SetActive(false);
        }
    }
    
}
