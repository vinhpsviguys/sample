using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemStorage : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    private GameObject itemBeingDragged;
    private Vector3 startPos;
    private Transform dragingItemClone;

    private void Start()
    {
        dragingItemClone = GameObject.FindGameObjectWithTag("DraggingItem").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        itemBeingDragged = eventData.pointerDrag as GameObject;
        if (itemBeingDragged.tag == "EquipmentSlotBag" || itemBeingDragged.tag == "ItemSlotBag")
        {
            dragingItemClone.gameObject.GetComponent<Image>().sprite = itemBeingDragged.transform.GetChild(1).GetComponent<Image>().sprite;
           
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        dragingItemClone.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000f));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("drag");
        //Destroy(dragingItemClone.GetComponent<Image>());
    }

  

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("drop");
       //GameObject dropObj= eventData.pointerEnter as GameObject;
       // Debug.Log(dropObj.name);
    }
}
