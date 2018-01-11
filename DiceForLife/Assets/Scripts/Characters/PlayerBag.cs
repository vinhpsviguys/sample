using System;
using System.Collections.Generic;
using UnityEngine;

public enum GroupEQuipment
{
    Equipment, Runestone
}
public class PlayerBag : MonoBehaviour
{
    public static PlayerBag _instance;
    internal List<EquipmentItem> _myItems;
    internal int MAX_CAPACITY = 40;
    private IDictionary<int, int> _mapBagToFliter;

    Action<object> _AddItemToBagEventRef, _RemoveItemToBagEventRef;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.tag = "DontDestroyObject";
            _mapBagToFliter = new Dictionary<int, int> { };

            _myItems = SplitDataFromServe._listEquipmentInBag;
            //Loading data bag

            //fake items;

            ActionSortBag();

            _AddItemToBagEventRef = (param) => AddItemToBag((EquipmentItem)param);
            this.RegisterListener(EventID.AddItemToBag, _AddItemToBagEventRef);
            _RemoveItemToBagEventRef = (param) => RemoveItemToBag((EquipmentItem)param);
            this.RegisterListener(EventID.RemoveItemToBag, _RemoveItemToBagEventRef);
        }
        //else DestroyObject(this.gameObject);
    }



    public List<EquipmentItem> FliterByTypeEquipment(TypeEquipmentCharacter _type)
    {
        _mapBagToFliter.Clear();
        List<EquipmentItem> _newList = new List<EquipmentItem>();
        int count = 0;
        _myItems.ForEach((EquipmentItem item) =>
        {
            if (_type != TypeEquipmentCharacter.OffhandWeapon)
            {
                if (_type == TypeEquipmentCharacter.Weapon)
                {
                    if (item.typeItem == _type || _type== TypeEquipmentCharacter.OffhandWeapon)
                    {
                        _mapBagToFliter.Add(count, _newList.Count);
                        _newList.Add(item);
                    }
                } else
                {
                    if (item.typeItem == _type)
                    {
                        _mapBagToFliter.Add(count, _newList.Count);
                        _newList.Add(item);
                    }
                }

            }
            else
            {
                if (item.typeItem == TypeEquipmentCharacter.Weapon || item.typeItem == TypeEquipmentCharacter.OffhandWeapon)
                {
                    _mapBagToFliter.Add(count, _newList.Count);
                    _newList.Add(item);
                }
            }
            count++;
        });
        return _newList;
    }

    public List<EquipmentItem> FliterByGroupEquipment(GroupEQuipment _group)
    {
        List<EquipmentItem> _newList = new List<EquipmentItem>();
        _mapBagToFliter.Clear();
        List<TypeEquipmentCharacter> _listTypeFlit = MappingData.GetListTypeEquiptmentByGroup(_group);
        int count = 0;

        if (_group == GroupEQuipment.Equipment)
        {
            _myItems.ForEach((EquipmentItem item) =>
            {
                if (_listTypeFlit.Contains(item.typeItem))
                {
                    _mapBagToFliter.Add(count, _newList.Count);
                    _newList.Add(item);
                }
                count++;
            });
        }
        //else if (_group == GroupEQuipment.Runestone)
        //{
        //    List<Item> _newList = new List<Item>();
        //    SplitDataFromServe._listGemInBag.ForEach((Item item) =>
        //    {
        //        //if (_listTypeFlit.Contains(item.typeItem))
        //        {
        //            _mapBagToFliter.Add(count, _newList.Count);
        //            _newList.Add(item);
        //        }
        //        count++;
        //    });
        //}
        return _newList;
    }

    public void RemoveEquipmentFromFliterBag(int _index)
    {
        foreach (var _map in _mapBagToFliter)
        {
            if (_map.Value == _index)
            {
                _myItems.RemoveAt(_map.Key);
                return;
            }
        }
        Debug.Log("Không tìm thấy");
    }

    public void AddItemToBag(EquipmentItem _item)
    {

        _myItems.Add(_item);

    }
    public void RemoveItemToBag(EquipmentItem _item)
    {
        Debug.Log("remove item");
        _myItems.Remove(_item);
    }

    internal void ActionSortBag()
    {

        int size = _myItems.Count;
        EquipmentItem _tempItem;
        bool isHaveSwap = true;
        for (int i = 0; i < size - 1; i++)
        {
            if (!isHaveSwap) break;
            isHaveSwap = false;
            for (int j = i + 1; j < size; j++)
            {
                if (_myItems[i].valueItem < _myItems[j].valueItem)
                {
                    //swap
                    isHaveSwap = true;
                    _tempItem = _myItems[j];
                    _myItems[j] = _myItems[i];
                    _myItems[i] = _tempItem;
                }
            }

        }
    }
    internal void ActionSplitItem()
    {
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.AddItemToBag, _AddItemToBagEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.RemoveItemToBag, _RemoveItemToBagEventRef);
    }
}
