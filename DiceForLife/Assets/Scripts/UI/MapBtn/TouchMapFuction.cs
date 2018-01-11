using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TouchMapFuction : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler{

    public Sprite _inActiveState, _activeState;


    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = _activeState;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = _inActiveState;
    }
}
