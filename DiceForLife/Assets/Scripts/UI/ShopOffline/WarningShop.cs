using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WarningShop : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
        ShopUI._instance._buyItemPanel.SetActive(false);
    }
}
