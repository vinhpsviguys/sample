using System.Collections.Generic;
using UnityEngine;
public class UpgradeBag : MonoBehaviour
{
    public enum STATE_BAG { NONE = 0, SHOW_EQUIP, SHOW_LUCKYMATERIAL, SHOW_RUNESTONE };
    internal STATE_BAG _myStateBag;

    private GroupEQuipment _myTypeBag;

    //equipments
    private List<EquipmentItem> _myItemEquipments;
    private int _lastIDEquipment;

    //lucky materials
    private List<Item> _myItemLuckyMaterials;
    private int _lastIDLuckyMaterial;
    private int _groupLuckyMaterialRequired = -1;

    //Runestone
    private List<Item> _myRunestones;
    private int _lastIDRunestone;
    private int _groupRunestoneRequired = -9999;
    private int _levelRunestoneRequired = -9999;
    private List<int> _listIdRunestoneAcepted;

    private ItemInforController _itemInfor;

    private int _currentIDSelected;

    //create View
    private List<ItemUIScript> _myItems;
    [SerializeField]
    private RectTransform _scrollViewParent;
    [SerializeField]
    private GameObject _itemPrefabs;
    private GameObject _tempObject;


    private bool isActiveStorageUI = false;

    public static string Message;
    public delegate void ActionEvent();
    public static event ActionEvent OnBtnClick;

    void Awake()
    {
        _myItems = new List<ItemUIScript>();
        _lastIDEquipment = -1;
    }
    void Start()
    {
        _itemInfor = ItemInforController.instance;
    }
    void OnEnable()
    {
        isActiveStorageUI = true;
        LoadEquipment(0);
        ItemUIScript.OnItemClicked += ItemClick;
        ItemInforController.OnBtnClick += BtnItemInforClick;
    }
    void OnDisable()
    {
        isActiveStorageUI = false;
        ItemUIScript.OnItemClicked -= ItemClick;
        ItemInforController.OnBtnClick -= BtnItemInforClick;
    }
    private void BtnItemInforClick()
    {
        string param = ItemInforController.Message;
        if (!isActiveStorageUI) return;
        if (param.Equals("SelectEquipment"))//Chọn đồ
        {
            _lastIDEquipment = _currentIDSelected;
        }
        else if (param.Equals("SelectItem"))//Chọn lucky material
        {
            if (_myStateBag == STATE_BAG.SHOW_LUCKYMATERIAL)
                _lastIDLuckyMaterial = _currentIDSelected;
            else if (_myStateBag == STATE_BAG.SHOW_RUNESTONE)
                _lastIDRunestone = _currentIDSelected;
        }
        else if (param.Equals("DeSelectItem"))//Chọn lucky material
        {
            _lastIDLuckyMaterial = -1;
        }
        Message = param;
        OnBtnClick();
        //this.PostEvent(EventID.UpgradeBagSelected, param);
    }
    internal void LoadEquipment(int IDLOAD)
    {
        _myStateBag = STATE_BAG.SHOW_EQUIP;
        if (_myItemEquipments != null) _myItemEquipments.Clear();
        else _myItemEquipments = new List<EquipmentItem>();
        _myTypeBag = GroupEQuipment.Equipment;
        LoadAllItemEquipAndInBag(IDLOAD);
        CreateViewEquipment();
        //StartCoroutine(LoadEquipmentInBag());
    }
    internal void LoadLuckyMaterial(int _groupRequired = 0)
    {
        //if (_myStateBag == STATE_BAG.SHOW_LUCKYMATERIAL) return;

        _myStateBag = STATE_BAG.SHOW_LUCKYMATERIAL;
        _lastIDLuckyMaterial = -1;

        if (_groupLuckyMaterialRequired != _groupRequired) _myItemLuckyMaterials = null;
        _groupLuckyMaterialRequired = _groupRequired;

        if (_myItemLuckyMaterials == null)
        {
            _myItemLuckyMaterials = new List<Item>();
            //Debug.Log(SplitDataFromServe._listItemInBag.Count);
            for (int i = 0; i < SplitDataFromServe._listItemInBag.Count; i++)
            {
                ITEMTYPE _type = SplitDataFromServe._listItemInBag[i].GetTypeItem();
                if (_type == ITEMTYPE.LUCKY_MATERIAL_REINFORCEMENT || _type == ITEMTYPE.SPECIAL_LUCKY_MATERIAL_REINFORCEMENT)
                {
                    if (_groupRequired != 0)
                    {
                        int _typeItem = int.Parse(SplitDataFromServe._listItemInBag[i].getValue("idit").ToString());
                        if (_type == ITEMTYPE.LUCKY_MATERIAL_REINFORCEMENT && _typeItem == _groupRequired)//1->10
                        {
                            _myItemLuckyMaterials.Add(SplitDataFromServe._listItemInBag[i]);
                        }
                        else if (_type == ITEMTYPE.SPECIAL_LUCKY_MATERIAL_REINFORCEMENT && _typeItem == _groupRequired + 17)//18-27
                        {
                            _myItemLuckyMaterials.Add(SplitDataFromServe._listItemInBag[i]);
                        }
                    }
                    else //special item
                    {
                        _myItemLuckyMaterials.Add(SplitDataFromServe._listItemInBag[i]);
                    }
                }
            }
            //Debug.Log(_myItemLuckyMaterials.Count);
        }
        CreateViewLuckMaterial();
        //StartCoroutine(LoadLuckyMaterialInBag());
    }
    internal void LoadRunestones(int _groupRequired = 0, int _levelRequired = 0)
    {
        _myStateBag = STATE_BAG.SHOW_RUNESTONE;
        _lastIDRunestone = -1;

        //if (_levelRequired != 0) Debug.Log("Require runestone level " + _levelRequired);
        if (_myRunestones != null) _myRunestones.Clear();
        else _myRunestones = new List<Item>();


        if (_groupRunestoneRequired != _groupRequired)
        {
            CalculateListRunestoneAcepted(_groupRequired);
        }
        _groupRunestoneRequired = _groupRequired;
        _levelRunestoneRequired = _levelRequired;

        for (int i = 0; i < SplitDataFromServe._listGemInBag.Count; i++)
        {
            int idInit = int.Parse(SplitDataFromServe._listGemInBag[i].getValue("idig").ToString());
            if (_listIdRunestoneAcepted.Contains(idInit))
            {
                int _level = int.Parse(SplitDataFromServe._listGemInBag[i].getValue("level").ToString());
                if (_groupRunestoneRequired == -1)//lọc đá để hợp thành
                {
                    if (_level < 10)//max level không được add vào bag nữa
                    {
                        _myRunestones.Add(SplitDataFromServe._listGemInBag[i]);
                    }
                }
                else
                {
                    if (_levelRequired == 0 || _levelRequired == _level)
                        _myRunestones.Add(SplitDataFromServe._listGemInBag[i]);
                }
            }
        }
        CreateViewRunestone();
    }

    private void LoadAllItemEquipAndInBag(int IDLOAD)
    {
        if (IDLOAD == 0 || IDLOAD == 1)//upgrade /addrunstone
        {
            if (CharacterInfo._instance._myEquipments.headItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.headItem);
            }
            if (CharacterInfo._instance._myEquipments.weaponItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.weaponItem);
            }
            if (CharacterInfo._instance._myEquipments.shieldItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.shieldItem);
            }
            if (CharacterInfo._instance._myEquipments.torsoItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.torsoItem);
            }
            if (CharacterInfo._instance._myEquipments.legItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.legItem);
            }
            if (CharacterInfo._instance._myEquipments.beltItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.beltItem);
            }
            if (CharacterInfo._instance._myEquipments.glovesItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.glovesItem);
            }
            if (CharacterInfo._instance._myEquipments.bootsItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.bootsItem);
            }
            if (CharacterInfo._instance._myEquipments.amuletItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.amuletItem);
            }
            if (CharacterInfo._instance._myEquipments.ringItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.ringItem);
            }

            if (IDLOAD == 0)//upgrade
                if (CharacterInfo._instance._myEquipments.buffItem != null)
                {
                    _myItemEquipments.Add(CharacterInfo._instance._myEquipments.buffItem);
                }

            if (CharacterInfo._instance._myEquipments.avatarItem != null)
            {
                _myItemEquipments.Add(CharacterInfo._instance._myEquipments.avatarItem);
            }
        }

        foreach (EquipmentItem tempItem in PlayerBag._instance._myItems)
        {
            _myItemEquipments.Add(tempItem);
        }
    }
    private void CalculateListRunestoneAcepted(int _group)
    {
        if (_listIdRunestoneAcepted == null) _listIdRunestoneAcepted = new List<int>();
        else _listIdRunestoneAcepted.Clear();
        switch (_group)
        {
            case -1://load runestone to combine
                for (int i = 1; i <= 15; i++)
                    _listIdRunestoneAcepted.Add(i);
                break;
            case 0://load all runestone
                for (int i = 1; i <= 24; i++)
                    if (i != 23) _listIdRunestoneAcepted.Add(i);
                break;
            case 1://physic weapon
                _listIdRunestoneAcepted.Add(1);
                _listIdRunestoneAcepted.Add(2);
                _listIdRunestoneAcepted.Add(3);
                _listIdRunestoneAcepted.Add(4);
                _listIdRunestoneAcepted.Add(9);
                _listIdRunestoneAcepted.Add(20);//blessing
                _listIdRunestoneAcepted.Add(24);//protection
                break;
            case 2://magic weapon
                _listIdRunestoneAcepted.Add(1);
                _listIdRunestoneAcepted.Add(2);
                _listIdRunestoneAcepted.Add(3);
                _listIdRunestoneAcepted.Add(5);
                _listIdRunestoneAcepted.Add(15);
                _listIdRunestoneAcepted.Add(20);
                _listIdRunestoneAcepted.Add(24);
                break;
            case 3://shield
                _listIdRunestoneAcepted.Add(1);
                _listIdRunestoneAcepted.Add(2);
                _listIdRunestoneAcepted.Add(3);
                _listIdRunestoneAcepted.Add(7);//endurane
                _listIdRunestoneAcepted.Add(10);//break weapon
                _listIdRunestoneAcepted.Add(20);
                _listIdRunestoneAcepted.Add(24);
                break;
            case 4://head,torso,pants
                _listIdRunestoneAcepted.Add(1);
                _listIdRunestoneAcepted.Add(2);
                _listIdRunestoneAcepted.Add(3);
                _listIdRunestoneAcepted.Add(6);//agility
                _listIdRunestoneAcepted.Add(20);
                _listIdRunestoneAcepted.Add(24);
                break;

            case 5://belt,gloves,boots
                _listIdRunestoneAcepted.Add(1);
                _listIdRunestoneAcepted.Add(2);
                _listIdRunestoneAcepted.Add(3);
                _listIdRunestoneAcepted.Add(8);//health
                _listIdRunestoneAcepted.Add(20);
                _listIdRunestoneAcepted.Add(24);
                break;

            case 6://ring,amulet
                _listIdRunestoneAcepted.Add(11);//frezzing reduce
                _listIdRunestoneAcepted.Add(14);//poisoning reduce
                _listIdRunestoneAcepted.Add(12);//electric shock
                _listIdRunestoneAcepted.Add(13);//burn reduce
                _listIdRunestoneAcepted.Add(20);
                _listIdRunestoneAcepted.Add(24);
                break;
            case 7://avatar
                _listIdRunestoneAcepted.Add(16);
                _listIdRunestoneAcepted.Add(17);
                _listIdRunestoneAcepted.Add(18);
                _listIdRunestoneAcepted.Add(19);
                _listIdRunestoneAcepted.Add(20);
                _listIdRunestoneAcepted.Add(21);
                break;
        }
    }


    void CreateViewEquipment()
    {
        int numberItem = _myItemEquipments.Count;
        for (int i = 0; i < numberItem; i++)
        {
            if (i < _myItems.Count)
            {
                _myItems[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.SetParent(_scrollViewParent.transform);
                _tempObject.transform.localScale = Vector3.one;
                _myItems.Add(_tempObject.GetComponent<ItemUIScript>());
                _myItems[i].idItemSlot = i;
                _myItems[i].gameObject.name = i.ToString();
            }
            _myItems[i].SetData(0, _myItemEquipments[i].idItemInit, _myItemEquipments[i].levelUpgraded, _myItemEquipments[i].rarelyItem,
                (_myItemEquipments[i].typeItem == TypeEquipmentCharacter.Buff ? 1 : (_myItemEquipments[i].typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
        }
        for (int i = numberItem; i < _myItems.Count; i++) _myItems[i].gameObject.SetActive(false);
    }
    void CreateViewLuckMaterial()
    {
        int numberItem = _myItemLuckyMaterials.Count;
        for (int i = 0; i < numberItem; i++)
        {
            if (i < _myItems.Count)
            {
                _myItems[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.SetParent(_scrollViewParent.transform);
                _tempObject.transform.localScale = Vector3.one;
                _myItems.Add(_tempObject.GetComponent<ItemUIScript>());
                _myItems[i].idItemSlot = i;
            }
            _myItems[i].SetData(1, int.Parse(_myItemLuckyMaterials[i].getValue("idit").ToString()), int.Parse(_myItemLuckyMaterials[i].getValue("quantity").ToString()), 0);
        }
        for (int i = numberItem; i < _myItems.Count; i++) _myItems[i].gameObject.SetActive(false);
    }
    void CreateViewRunestone()
    {
        int numberItem = _myRunestones.Count;
        for (int i = 0; i < numberItem; i++)
        {
            if (i < _myItems.Count)
            {
                _myItems[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.SetParent(_scrollViewParent.transform);
                _tempObject.transform.localScale = Vector3.one;
                _myItems.Add(_tempObject.GetComponent<ItemUIScript>());
                _myItems[i].idItemSlot = i;
            }
            _myItems[i].SetData(2, int.Parse(_myRunestones[i].getValue("idig").ToString()), int.Parse(_myRunestones[i].getValue("quantity").ToString()), 0);
        }
        for (int i = numberItem; i < _myItems.Count; i++) _myItems[i].gameObject.SetActive(false);
    }
    public void ItemClick()
    {
        int id = ItemUIScript.idClicked;
        if (!isActiveStorageUI) return;
        _currentIDSelected = id;
        if (_myStateBag == STATE_BAG.SHOW_EQUIP)//item hợp lệ
        {
            _itemInfor.SetData(_myItemEquipments[id], _myItems[_currentIDSelected]._iconImg.sprite);
            _itemInfor.SetState(StateInforItem.SelectEquipment);
        }
        else if (_myStateBag == STATE_BAG.SHOW_LUCKYMATERIAL)
        {
            _itemInfor.SetData(_myItemLuckyMaterials[id], _myItems[_currentIDSelected]._iconImg.sprite);
            _itemInfor.SetState(StateInforItem.SelectItem);
        }
        else if (_myStateBag == STATE_BAG.SHOW_RUNESTONE)
        {
            _itemInfor.SetData(_myRunestones[id], _myItems[_currentIDSelected]._iconImg.sprite);
            _itemInfor.SetState(StateInforItem.SelectItem);
        }
    }
    internal EquipmentItem GetEquipmentSelected()
    {
        return _myItemEquipments[_lastIDEquipment];
    }
    internal Item GetLuckyMaterialSelected()
    {
        if (_lastIDLuckyMaterial < 0)
        {
            Debug.Log(_lastIDLuckyMaterial);
            return null;
        }
        if (_lastIDLuckyMaterial >= _myItemLuckyMaterials.Count)
        {
            Debug.Log("Errorr:" + _lastIDLuckyMaterial + "/" + _myItemLuckyMaterials.Count);
            return null;
        }
        return _myItemLuckyMaterials[_lastIDLuckyMaterial];
    }
    internal Sprite GetIconEquipmentSelected()
    {
        return _myItems[_lastIDEquipment]._iconImg.sprite;
    }
    internal Sprite GetIconLuckyMaterialSelected()
    {
        return _myItems[_lastIDLuckyMaterial]._iconImg.sprite;
    }
    internal Item GetRunestoneSelected()
    {
        return _myRunestones[_lastIDRunestone];
    }
    internal Sprite GetIconRunestoneSelected()
    {
        return _myItems[_lastIDRunestone]._iconImg.sprite;
    }
    internal void DeSelectMaterial()
    {
        _lastIDLuckyMaterial = -1;
    }
    internal void DeSelectRunestone()
    {
        _lastIDLuckyMaterial = -1;
    }

    internal void UpdateEquipmentUpgraded(int _grouplevel = -1)
    {
        if (_myStateBag == STATE_BAG.SHOW_EQUIP)//item hợp lệ
        {
            //LoadEquipment();
            _myItems[_lastIDEquipment].ReupdateLevelUpgraded(_myItemEquipments[_lastIDEquipment].levelUpgraded);
        }
        else if (_myStateBag == STATE_BAG.SHOW_LUCKYMATERIAL)
        {
            LoadLuckyMaterial(_grouplevel);
        }
        else if (_myStateBag == STATE_BAG.SHOW_RUNESTONE)
        {
            LoadRunestones(_groupRunestoneRequired, _levelRunestoneRequired);
        }
    }
}
