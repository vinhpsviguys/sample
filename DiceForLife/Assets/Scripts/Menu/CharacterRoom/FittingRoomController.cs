using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FittingRoomController : MonoBehaviour
{
    public static int lastIDSelected;
    private TypeEquipmentCharacter _typeEquipSelected;
    private ItemInforController _inforItem;


    [SerializeField]
    private Image _defaultWeaponSlot, _defaultShieldSlot;
    private ClassCharacter _myClass;

    [SerializeField] private GameObject _itemUIPrefabs;
    private ItemUIScript[] _itemUIs;
    [SerializeField]
    private RectTransform _headItem, _weaponItem, _shieldItem, _torsoItem, _legItem, _glovesItem, _bootsItem, _beltItem, _amuletItem, _ringItem, _buffItem, _avatarItem;

    [SerializeField]
    private FilterBag _fliterBag;
    [SerializeField]
    private PlayerShowFittingRoom _playerSprite;

    private EquipmentsCharacter _characterEquip = null;
    private TypeEquipmentCharacter _typeItem;

    private Vector3 _offsetOptionNormal = new Vector3(0, -100);
    private Vector3 _offsetOptionSmall = new Vector3(0, -50);

    private EquipmentItem _itemSelected;
    private EquipmentItem _tempEquip;



    private void Awake()
    {
        this.RegisterListener(EventID.EquipItemToPlayer, (param) => LoadAllItemsInCharacter());
        _itemUIs = new ItemUIScript[12];
    }

    void Start()
    {
        _myClass = CharacterInfo._instance._baseProperties._classCharacter;
        if (_myClass == ClassCharacter.Assassin || _myClass == ClassCharacter.Wizard || _myClass == ClassCharacter.Orc)//neu dung vu khi 2 tay
        {
            _defaultShieldSlot.sprite = _defaultWeaponSlot.sprite;
        }

        _inforItem = ItemInforController.instance;
        lastIDSelected = -1;
        if (CharacterInfo._instance == null || ControllerItemsInGame._instance == null)
        {
            Debug.LogError("Lỗi to đùng");
            return;
        }
        _characterEquip = SplitDataFromServe._equipmentCurrentHero;
        if (_characterEquip != null) LoadAllItemsInCharacter();
    }

    void OnEnable()
    {
        EquipmentsCharacter.ChangeItemEvent += EquipmentsCharacter_ChangeItemEvent;

        if (_characterEquip != null) LoadAllItemsInCharacter();
        ItemInforController.OnBtnClick += BtnItemInforClick;
    }

    private void BtnItemInforClick()
    {
        //Debug.Log(param);
        string param = ItemInforController.Message;
        if (param.Equals("RemoveEquipment"))
        {
            BtnRemoveItem();
        }
        else if (param.Equals("ReplaceEquipment"))
        {
            _fliterBag.SetFilter(_typeEquipSelected);
        }
    }

    void OnDisable()
    {
        EquipmentsCharacter.ChangeItemEvent -= EquipmentsCharacter_ChangeItemEvent;
        ItemInforController.OnBtnClick -= BtnItemInforClick;
    }

    private void LoadAllItemsInCharacter()
    {
        //Debug.Log("Load all item");
        for (int i = 0; i < 12; i++)
        {
            
            _tempEquip = MappingIndexToEquipment(i);
            if (_tempEquip != null)
            {
                SetItemImg(i, _tempEquip);
            }
            else
            {
                if (_itemUIs[i] != null) _itemUIs[i].gameObject.SetActive(false);
            }
        }
    }
    private void SetItemImg(int i, EquipmentItem _item)
    {
        if (_itemUIs[i] == null)
        {
            GameObject _tempGameObject = Instantiate(_itemUIPrefabs);
            _tempGameObject.transform.SetParent(MappingIndexToRect(i));
            float _ratio = MappingIndexToRect(i).sizeDelta.x / 145f;
            _tempGameObject.transform.localScale = Vector3.one * _ratio;
            _tempGameObject.transform.localPosition = Vector3.zero;
            _tempGameObject.GetComponent<Image>().raycastTarget = false;
            _itemUIs[i] = _tempGameObject.GetComponent<ItemUIScript>();
        }
        _itemUIs[i].gameObject.SetActive(true);
        _itemUIs[i].SetData(0, _item.idItemInit, _item.levelUpgraded, _item.rarelyItem, 
            (_item.typeItem == TypeEquipmentCharacter.Buff ? 1 : (_item.typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
    }

    void EquipmentsCharacter_ChangeItemEvent()
    {
        SetItemImg(MappingSlotToIndex(_characterEquip._lastEquipmentChangeSlot), _characterEquip._lastEquipmentChangeItem);
    }

    public void BtnItemClick(int id)
    {
        lastIDSelected = id;
        _typeEquipSelected = MappingData.ConvertIdSlotToTypeEquipment(id, _myClass);
        //Debug.Log(id + " "+ _typeEquipSelected.ToString());
        if (_typeEquipSelected == TypeEquipmentCharacter.None) return;//slot trống

        _itemSelected = MappingIndexToEquipment(id);
        if (_itemSelected == null) // chưa có item mặc
        {
            _inforItem._myState = StateInforItem.None;
            _fliterBag.SetFilter(_typeEquipSelected);
        }
        //else if (_itemSelected.idItemInit == 0)
        //{
        //    _inforItem._myState = StateInforItem.ShowInforEquipment;
        //    _fliterBag.SetActive(true);
        //}
        else //có item rồi
        {
            _inforItem.SetData(_itemSelected, _itemUIs[id]._iconImg.sprite);
            _inforItem.SetState(StateInforItem.ShowInforEquipment);
        }
    }
    public void BtnRemoveItem()
    {
        _typeItem = MappingData.ConvertIdSlotToTypeEquipment(lastIDSelected, _myClass);
#if UNITY_EDITOR
        Debug.Log("Removing " + _typeItem);
#endif
        if (_characterEquip.isHaveItem(_typeItem))
        {
            StartCoroutine(ServerAdapter.ExecuteRemoveItemOnHero(_typeItem, _itemSelected.idItem, result =>
             {
                 if (result.StartsWith("Error"))
                 {
                     Debug.Log(result);
                     TextNotifyScript.instance.SetData(result);
                 }
                 else
                 {
#if UNITY_EDITOR
                    Debug.Log("Remove successed " + _typeItem);
#endif
                    EquipmentItem tempItemRemove = _characterEquip.MappingTypeItemToItem(_typeItem);
                     if (_myClass == ClassCharacter.Assassin || _myClass == ClassCharacter.Wizard || _myClass == ClassCharacter.Orc)
                     {
                         if (tempItemRemove.typeItem == TypeEquipmentCharacter.OffhandWeapon)
                         {
                             tempItemRemove.typeItem = TypeEquipmentCharacter.Weapon;
                         }
                     }
                     SplitDataFromServe._listEquipmentInBag.Add(tempItemRemove);
                     _characterEquip.EquipItem(_typeItem, new EquipmentItem(0, _typeItem));
                     //CharacterInfo._instance._baseProperties.RemoveBonusItem(_itemSelected);

                    //Update UI
                    //MappingIndexToImage(lastIDSelected).sprite = null;
                    //MappingIndexToImage(lastIDSelected).gameObject.SetActive(false);
                    if (_itemUIs[lastIDSelected] != null) _itemUIs[lastIDSelected].gameObject.SetActive(false);
                    //reset Selected
                    switch (MappingIndexToTypeEquipment(lastIDSelected))
                     {
                         case TypeEquipmentCharacter.Head:
                             _characterEquip.headItem = null;
                             break;
                         case TypeEquipmentCharacter.Weapon:
                             _characterEquip.weaponItem = null;
                             break;
                         case TypeEquipmentCharacter.Shield:
                             _characterEquip.shieldItem = null;
                             break;
                         case TypeEquipmentCharacter.OffhandWeapon:
                             _characterEquip.shieldItem = null;
                             break;
                         case TypeEquipmentCharacter.Torso:
                             _characterEquip.torsoItem = null;
                             break;
                         case TypeEquipmentCharacter.Leg:
                             _characterEquip.legItem = null;
                             break;
                         case TypeEquipmentCharacter.Gloves:
                             _characterEquip.glovesItem = null;
                             break;
                         case TypeEquipmentCharacter.Boots:
                             _characterEquip.bootsItem = null;
                             break;
                         case TypeEquipmentCharacter.Belt:
                             _characterEquip.beltItem = null;
                             break;
                         case TypeEquipmentCharacter.Amulet:
                             _characterEquip.amuletItem = null;
                             break;
                         case TypeEquipmentCharacter.Ring:
                             _characterEquip.ringItem = null;
                             break;
                         case TypeEquipmentCharacter.Avatar:
                             _characterEquip.avatarItem = null;
                             break;
                     }
                     lastIDSelected = -1;

                     ItemInforController.instance.MakeEvent("UpdateEquipment");
                 }
             }));
        }
        else Debug.Log("Không có gì đâu");
    }

    private RectTransform MappingIndexToRect(int id)
    {
        switch (id)
        {
            case 0: return _headItem;
            case 1: return _weaponItem;
            case 2: return _shieldItem;
            case 3: return _torsoItem;
            case 4: return _legItem;
            case 5: return _glovesItem;
            case 6: return _bootsItem;
            case 7: return _beltItem;
            case 8: return _amuletItem;
            case 9: return _ringItem;
            case 10: return _buffItem;
            case 11: return _avatarItem;
        }
        return null;
    }

    private int MappingSlotToIndex(TypeEquipmentCharacter slot)
    {
        switch (slot)
        {
            case TypeEquipmentCharacter.Head: return 0;
            case TypeEquipmentCharacter.Weapon: return 1;
            case TypeEquipmentCharacter.Shield: return 2;
            case TypeEquipmentCharacter.OffhandWeapon: return 2;
            case TypeEquipmentCharacter.Torso: return 3;
            case TypeEquipmentCharacter.Leg: return 4;
            case TypeEquipmentCharacter.Gloves: return 5;
            case TypeEquipmentCharacter.Boots: return 6;
            case TypeEquipmentCharacter.Belt: return 7;
            case TypeEquipmentCharacter.Amulet: return 8;
            case TypeEquipmentCharacter.Ring: return 9;
            case TypeEquipmentCharacter.Buff: return 10;
            case TypeEquipmentCharacter.Avatar: return 11;
        }
        Debug.LogError("Sao lai xay ra cai nay ????");
        return 0;
    }

    private EquipmentItem MappingIndexToEquipment(int id)
    {
        switch (id)
        {
            case 0: return _characterEquip.headItem;
            case 1: return _characterEquip.weaponItem;
            case 2: return _characterEquip.shieldItem;
            case 3: return _characterEquip.torsoItem;
            case 4: return _characterEquip.legItem;
            case 5: return _characterEquip.glovesItem;
            case 6: return _characterEquip.bootsItem;
            case 7: return _characterEquip.beltItem;
            case 8: return _characterEquip.amuletItem;
            case 9: return _characterEquip.ringItem;
            case 10: return _characterEquip.buffItem;
            case 11: return _characterEquip.avatarItem;
        }
        return null;
    }

    private TypeEquipmentCharacter MappingIndexToTypeEquipment(int id)
    {
        switch (id)
        {
            case 0: return TypeEquipmentCharacter.Head;
            case 1: return TypeEquipmentCharacter.Weapon;
            case 2:
                if (_myClass == ClassCharacter.Assassin || _myClass == ClassCharacter.Wizard || _myClass == ClassCharacter.Orc)
                    return TypeEquipmentCharacter.OffhandWeapon;
                else
                    return TypeEquipmentCharacter.Shield;
            case 3: return TypeEquipmentCharacter.Torso;
            case 4: return TypeEquipmentCharacter.Leg;
            case 5: return TypeEquipmentCharacter.Gloves;
            case 6: return TypeEquipmentCharacter.Boots;
            case 7: return TypeEquipmentCharacter.Belt;
            case 8: return TypeEquipmentCharacter.Amulet;
            case 9: return TypeEquipmentCharacter.Ring;
            case 10: return TypeEquipmentCharacter.Buff;
            case 11: return TypeEquipmentCharacter.Avatar;
        }
        return TypeEquipmentCharacter.None;
    }
}
