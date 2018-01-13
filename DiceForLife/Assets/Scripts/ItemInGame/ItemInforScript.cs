using DG.Tweening;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInforScript : MonoBehaviour
{
    private EquipmentItem _myEquipment;
    private Item _myItem;
    private Sprite _icon;

    public Text _txtName, _txtSlot, _txtType, _txtLevel, _txtValue;
    public Image _iconItem, _rareBorder;

    public Transform _AttributeTransform;

    public GameObject _BtnOne, _BtnTwo;
    public Text _txtBtnOne, _txtBtnTwo;

    private List<Text> _myAttributesTxt;
    private RectTransform _myContent;
    public ScrollRect myScrollRect;
    private int numberAttribute;

    //private int basePopup = 210;
    //private int maxPopup = 650;
    //private int offsetBtn = 0;
    private int stepAttribute = 60;
    private int startAttribute = -345;
    private int minimumSizeScroll = 650;
    private int widthScrollSize = 784;

    public GameObject txtAttributePrefabs;
    private GameObject _tempGameObject;
    private Text _tempText;
    private bool isCanClick;

    private StateInforItem _myState;
    private ItemInforController _myDady;

    private List<string> propertyString = new List<string>();
    private bool isItem;

    private Color32 _colorAddAtt = new Color32(60, 134, 255, 255);


    private void Awake()
    {
        _myContent = _AttributeTransform.GetComponent<RectTransform>();
        _myAttributesTxt = new List<Text>();
    }

    void Start()
    {
        _myDady = ItemInforController.instance;
    }
    public void SetData(EquipmentItem _item, Sprite _avatar, StateInforItem _statePopup)
    {
        _myItem = null;
        _myEquipment = _item;
        if (_myEquipment == null)
        {
            Debug.LogError("_myEquipment null");
            return;
        }
        _icon = _avatar;
        _myState = _statePopup;
    }
    public void SetData(Item _item, bool isItem, Sprite _avatar, StateInforItem _statePopup)
    {
        _myEquipment = null;
        _myItem = _item;
        this.isItem = isItem;
        _icon = _avatar;
        _myState = _statePopup;
    }

    void OnEnable()
    {
        isCanClick = false;
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(OncompleteEnable);
        Invoke("ResetScrollView", 0.1f);

        if (_myEquipment != null)
        {
            _rareBorder.sprite = ControllerItemsInGame._instance._rareBorderItems[_myEquipment.rarelyItem];
            GetListPropertiesDisplay(_myEquipment);
            _txtName.text = _myEquipment.nameItem + (_myEquipment.levelUpgraded == 0 ? string.Empty : string.Format("(+{0})", _myEquipment.levelUpgraded));
            _txtName.color = MappingData.ConvertRarelyToColor(_myEquipment.rarelyItem);
            _txtSlot.text = string.Format("Slot: {0}", _myEquipment.typeItem);

            _txtValue.gameObject.SetActive(false);
            if (_myEquipment.classItem == ClassCharacterItem.None)
            {
                if (_myEquipment.typeItem == TypeEquipmentCharacter.Weapon || _myEquipment.typeItem == TypeEquipmentCharacter.OffhandWeapon)
                {
                    _txtType.text = string.Format("Type: {0}", getNameWeapon(_myEquipment.idTypeEquipment));
                    _txtValue.gameObject.SetActive(true);

                    if (CharacterInfo._instance._baseProperties._classCharacter.Equals(MappingData.GetSuitableClassForWeapon(_myEquipment.idTypeEquipment)))
                    {
                        _txtValue.color = _txtSlot.color;
                    }
                    else _txtValue.color = Color.red;
                    _txtValue.text = string.Format("Class required: {0}", MappingData.GetSuitableClassForWeapon(_myEquipment.idTypeEquipment));
                }
                else
                {
                    _txtType.text = string.Format("Type: {0}", "Buff");
                }
            }
            else _txtType.text = string.Format("Type: {0}", _myEquipment.classItem.ToString());

            if (CharacterInfo._instance._baseProperties.Level >= _myEquipment.levelRequired)
            {
                _txtLevel.color = _txtSlot.color;
            }
            else _txtLevel.color = Color.red;
            _txtLevel.text = string.Format("Level required: {0}", _myEquipment.levelRequired.ToString());

            if (_icon != null) _iconItem.sprite = _icon;
            else StartCoroutine(ReLoadIcon());
        }
        else if (_myItem != null)
        {
            _rareBorder.sprite = ControllerItemsInGame._instance._rareBorderItems[0];
            _txtName.text = _myItem.getValue("name").ToString();
            _txtSlot.text = string.Format("Quantity: {0}", _myItem.getValue("quantity").ToString());
            _txtType.text = string.Format("Level: {0}", _myItem.getValue("level").ToString());
            _txtLevel.text = string.Format("Level required: 1");
            _txtValue.text = string.Empty;

            propertyString.Clear();
            if (_myItem.isContains("idit"))
            {
                string[] _property = _myItem.getValue("descripton").ToString().Split(new string[] { @"\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < _property.Length; i++)
                    propertyString.Add(_property[i]);
            }
            else
            {
                string[] _property = _myItem.getValue("description").ToString().Split(new string[] { @"\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < _property.Length; i++)
                    propertyString.Add(_property[i]);
            }


            if (_icon != null) _iconItem.sprite = _icon;
            else StartCoroutine(ReLoadIcon());
        }

        ShowAttibutes();
        ActiveBtns();
    }
    void OncompleteEnable()
    {
        isCanClick = true;
    }

    IEnumerator ReLoadIcon()
    {
        if (_myEquipment != null)
        {
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForEquipment(_myEquipment,
                    value =>
                    {
                        _iconItem.sprite = value;
                    }));
        }
        else
        {
            if (isItem)
            {
                yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItem(_myItem,
                        value =>
                        {
                            _iconItem.sprite = value;
                        }));
            }
            else
            {
                yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGems(_myItem,
                        value =>
                        {
                            _iconItem.sprite = value;
                        }));
            }
        }
    }
    void ShowAttibutes()
    {
        numberAttribute = propertyString.Count;
        ResizeScrollView(numberAttribute);
        for (int i = 0; i < numberAttribute; i++)
        {
            if (i < _myAttributesTxt.Count)
            {
                _myAttributesTxt[i].gameObject.SetActive(true);
                //_tempText = _myAttributesTxt[i];
            }
            else
            {
                _tempGameObject = Instantiate(txtAttributePrefabs) as GameObject;
                _tempGameObject.transform.SetParent(_AttributeTransform);
                _tempGameObject.transform.localScale = Vector3.one;
                //_tempGameObject.transform.localPosition = new Vector3(widthScrollSize / 2, startAttribute - stepAttribute * i);
                _tempText = _tempGameObject.GetComponent<Text>();
                _myAttributesTxt.Add(_tempText);
            }
            _myAttributesTxt[i].transform.localPosition = new Vector3(widthScrollSize / 2 + 20, startAttribute - stepAttribute * i);
            _myAttributesTxt[i].text = propertyString[i].ToString();

            if (_myEquipment == null)
            {
                _myAttributesTxt[i].color = Color.white;
            }
            else if (propertyString[i].ToString()[0] == '+')
            {
                _myAttributesTxt[i].color = _colorAddAtt;
            }
            else
            {
                _myAttributesTxt[i].color = Color.white;
            }
        }
        for (int i = numberAttribute; i < _myAttributesTxt.Count; i++)
        {
            _myAttributesTxt[i].gameObject.SetActive(false);
        }
    }
    //void ShowAttributeItem()
    //{
    //    if (_myAttributesTxt == null) _myAttributesTxt = new List<Text>();
    //    numberAttribute = 1;
    //    ResizeScrollView(numberAttribute);
    //    for (int i = 0; i < numberAttribute; i++)
    //    {
    //        if (i < _myAttributesTxt.Count)
    //        {
    //            _myAttributesTxt[i].gameObject.SetActive(true);
    //            _tempText = _myAttributesTxt[i];
    //            _myAttributesTxt[i].transform.localPosition = new Vector3(widthScrollSize / 2, startAttribute - stepAttribute * i);
    //        }
    //        else
    //        {
    //            _tempGameObject = Instantiate(txtAttributePrefabs) as GameObject;
    //            _tempGameObject.transform.SetParent(_AttributeTransform);
    //            _tempGameObject.transform.localScale = Vector3.one;
    //            _tempGameObject.transform.localPosition = new Vector3(widthScrollSize / 2, startAttribute - stepAttribute * i);
    //            _tempText = _tempGameObject.GetComponent<Text>();
    //            _myAttributesTxt.Add(_tempText);
    //        }


    //    }
    //    for (int i = numberAttribute; i < _myAttributesTxt.Count; i++)
    //    {
    //        _myAttributesTxt[i].gameObject.SetActive(false);
    //    }
    //}
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            myScrollRect.verticalNormalizedPosition = myScrollRect.verticalNormalizedPosition - 0.01f;
    }
    void ResizeScrollView(int numberAttribute)
    {
        if (numberAttribute <= 5)
        {
            _myContent.sizeDelta = new Vector2(widthScrollSize, minimumSizeScroll);
        }
        else
        {
            _myContent.sizeDelta = new Vector2(widthScrollSize, minimumSizeScroll + stepAttribute * (numberAttribute - 5));
        }
    }
    void ResetScrollView()
    {
        myScrollRect.verticalNormalizedPosition = 1f;
    }
    private void ActiveBtns()
    {
        switch (_myState)
        {
            case StateInforItem.None:
            case StateInforItem.ShowInForItem:
                _BtnOne.SetActive(false);
                _BtnTwo.SetActive(false);
                break;
            case StateInforItem.ShowInforEquipment:
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Replace";
                _BtnOne.transform.localPosition = new Vector3(-150, 0);
                _BtnTwo.SetActive(true);
                _txtBtnTwo.text = "Remove";
                _BtnTwo.transform.localPosition = new Vector3(150, 0);

                if (_myEquipment != null && _myEquipment.typeItem == TypeEquipmentCharacter.Avatar)//Avatar không tháo được
                {
                    _BtnOne.SetActive(false);
                    _BtnTwo.SetActive(false);
                }

                break;
            case StateInforItem.ReplaceEquipment:
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Replace";
                _BtnOne.transform.localPosition = Vector3.zero;
                _BtnTwo.SetActive(false);
                break;

            case StateInforItem.SellEquipment:
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Equip";
                _BtnOne.transform.localPosition = new Vector3(-150, 0);
                _BtnTwo.SetActive(true);
                _txtBtnTwo.text = "Sell";
                _BtnTwo.transform.localPosition = new Vector3(150, 0);
                break;
            case StateInforItem.SelectEquipment:
            case StateInforItem.SelectItem:
                _BtnOne.transform.parent.gameObject.SetActive(true);
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Select";
                _BtnOne.transform.localPosition = Vector3.zero;
                _BtnTwo.SetActive(false);
                break;
            case StateInforItem.DeselectItem:
                _BtnOne.transform.parent.gameObject.SetActive(true);
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Remove";
                _BtnOne.transform.localPosition = Vector3.zero;
                _BtnTwo.SetActive(false);
                break;

            case StateInforItem.UseEquipment:
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Use";
                _BtnOne.transform.localPosition = Vector3.zero;
                _BtnTwo.SetActive(false);
                break;
            case StateInforItem.SellItem:
                _BtnOne.SetActive(true);
                _txtBtnOne.text = "Use";
                _BtnOne.transform.localPosition = new Vector3(-150, 0);
                _BtnTwo.SetActive(true);
                _txtBtnTwo.text = "Sell";
                _BtnTwo.transform.localPosition = new Vector3(150, 0);
                break;
        }
    }
    public void BtnClick(int idBtn)
    {
        _myDady.BtnItemInforClicked(_myState, idBtn);
    }

    void GetListPropertiesDisplay(EquipmentItem _item)
    {
        propertyString.Clear();
        switch (_item.typeItem)
        {
            case TypeEquipmentCharacter.Weapon:
            case TypeEquipmentCharacter.OffhandWeapon:
                if (!float.Parse(_myEquipment.getValue("2").ToString()).Equals(0))
                    propertyString.Add(string.Format("Physical Damage: {0} - {1}", _fixValue(1), _fixValue(2)));
                if (!float.Parse(_myEquipment.getValue("4").ToString()).Equals(0))
                    propertyString.Add(string.Format("Magical Damage: {0} - {1}", _fixValue(3), _fixValue(4)));
                if (!float.Parse(_myEquipment.getValue("5").ToString()).Equals(0))
                    propertyString.Add(string.Format("Critical Chance: {0}%", _fixValue(5, true)));
                if (!float.Parse(_myEquipment.getValue("6").ToString()).Equals(0))
                    propertyString.Add(string.Format("Multicast Chance: {0}%", _fixValue(6, true)));
                if (!float.Parse(_myEquipment.getValue("12").ToString()).Equals(0))
                    propertyString.Add(string.Format("Attack Rate: {0} - {1}", _fixValue(11), _fixValue(12)));
                if (!float.Parse(_myEquipment.getValue("8").ToString()).Equals(0))
                    propertyString.Add(string.Format("Physical damage Reinforce: {0}% - {1}%", _fixValue(7, true), _fixValue(8, true)));
                if (!float.Parse(_myEquipment.getValue("10").ToString()).Equals(0))
                    propertyString.Add(string.Format("Magical damage Reinforce: {0}% - {1}%", _fixValue(9, true), _fixValue(10, true)));
                break;
            case TypeEquipmentCharacter.Shield:
                propertyString.Add(string.Format("Physical Defense: {0} - {1} ", _fixValue(17), _fixValue(18)));
                propertyString.Add(string.Format("Magical Defense: {0} - {1}", _fixValue(19), _fixValue(20)));
                propertyString.Add(string.Format("Block Chance: {0}%", _fixValue(16, true)));
                propertyString.Add(string.Format("Physical Reduction: {0}% - {1}%", _fixValue(21, true), _fixValue(22, true)));
                propertyString.Add(string.Format("Magical Reduction: {0}% - {1}%", _fixValue(23, true), _fixValue(24, true)));
                break;
            case TypeEquipmentCharacter.Head:
            case TypeEquipmentCharacter.Torso:
            case TypeEquipmentCharacter.Leg:
            case TypeEquipmentCharacter.Belt:
            case TypeEquipmentCharacter.Boots:
            case TypeEquipmentCharacter.Gloves:
                propertyString.Add(string.Format("Physical Defense: {0} - {1}", _fixValue(17), _fixValue(18)));
                propertyString.Add(string.Format("Magical Defense: {0} - {1}", _fixValue(19), _fixValue(20)));
                propertyString.Add(string.Format("Parry Chance: {0} - {1}", _fixValue(13), _fixValue(14)));
                propertyString.Add(string.Format("Physical Reduction: {0}% - {1}%", _fixValue(21, true), _fixValue(22, true)));
                propertyString.Add(string.Format("Magical Reduction: {0}% - {1}%", _fixValue(23, true), _fixValue(24, true)));
                break;

            case TypeEquipmentCharacter.Ring:
            case TypeEquipmentCharacter.Amulet:
                propertyString.Add(string.Format("Physical Absorption: {0}% - {1}%", _fixValue(25, true), _fixValue(26, true)));
                propertyString.Add(string.Format("Magical Absorption: {0}% - {1}%", _fixValue(27, true), _fixValue(28, true)));
                break;
            case TypeEquipmentCharacter.Avatar:
                propertyString.Add(string.Format("Health: {0}%", _fixValue(39, true)));
                propertyString.Add(string.Format("Physical damage: {0}%", _fixValue(98, true)));
                propertyString.Add(string.Format("Magical damamge: {0}%", _fixValue(99, true)));
                propertyString.Add(string.Format("Physical absoption: {0}%", _fixValue(100, true)));
                propertyString.Add(string.Format("Magical absorption: {0}%", _fixValue(101, true)));

                propertyString.Add(string.Format("Slots of Adding Magical Properties: {0} Unit", int.Parse(_item.getValue("999").ToString())));
                break;
            case TypeEquipmentCharacter.Buff:

                int _idRate = MappingData.ConvertIdBuffToAttribute(_myEquipment.idItemInit);
                string _valueRate = _fixValue(_idRate, true);
                switch (_item.idItemInit)
                {
                    case 1: propertyString.Add(string.Format("Damage against Assassin: {0}%", _valueRate)); break;
                    case 2: propertyString.Add(string.Format("Damage against Paladin: {0}%", _valueRate)); break;
                    case 3: propertyString.Add(string.Format("Damage against Zealot: {0}%", _valueRate)); break;
                    case 4: propertyString.Add(string.Format("Damage against Sorceress: {0}%", _valueRate)); break;
                    case 5: propertyString.Add(string.Format("Damage against Wizard: {0}%", _valueRate)); break;
                    case 6: propertyString.Add(string.Format("Damage against Marksman: {0}%", _valueRate)); break;
                    case 7: propertyString.Add(string.Format("Damage against Orc: {0}%", _valueRate)); break;
                    case 8: propertyString.Add(string.Format("Damage against Barbarian: {0}%", _valueRate)); break;
                    case 9: propertyString.Add(string.Format("Knock back resistance: {0}%", _valueRate)); break;
                    case 10: propertyString.Add(string.Format("Immobilization resistance: {0}%", _valueRate)); break;
                    case 11: propertyString.Add(string.Format("Blindness resistance: {0}%", _valueRate)); break;
                    case 12: propertyString.Add(string.Format("Dementia resistance: {0}%", _valueRate)); break;
                    case 13: propertyString.Add(string.Format("Disease resistance: {0}%", _valueRate)); break;
                    case 14: propertyString.Add(string.Format("Fear resistance: {0}%", _valueRate)); break;
                    case 15: propertyString.Add(string.Format("Sleep resistance: {0}%", _valueRate)); break;
                    case 16: propertyString.Add(string.Format("Glamour resistance: {0}%", _valueRate)); break;
                    case 17: propertyString.Add(string.Format("Stun resistance: {0}%", _valueRate)); break;
                    case 18: propertyString.Add(string.Format("Hypnotic resistance: {0}%", _valueRate)); break;
                    case 19: propertyString.Add(string.Format("Impotent resistance: {0}%", _valueRate)); break;
                    case 20: propertyString.Add(string.Format("Rot resistance: {0}%", _valueRate)); break;
                    case 21: propertyString.Add(string.Format("Pain resistance: {0}%", _valueRate)); break;
                    case 22: propertyString.Add(string.Format("Bleed resistance: {0}%", _valueRate)); break;
                    case 23: propertyString.Add(string.Format("Crazy resistance: {0}%", _valueRate)); break;
                    case 24: propertyString.Add(string.Format("Dull resistance: {0}%", _valueRate)); break;
                    default: Debug.Log("Sao không có cái nào trùng"); break;
                }
                break;
        }
        try
        {
            var N = SimpleJSON.JSON.Parse(_item.getValue("listidproperty").ToString());
            foreach (KeyValuePair<string, JSONNode> _temp in N.AsObject)
            {
                propertyString.Add("+" + Constant.getPropertiesName(_temp.Key.ToString()) + ": " + Constant.displayTypeProperty(int.Parse(_temp.Key.ToString()), float.Parse(_temp.Value.AsFloat.ToString())));
            }
        }
        catch (Exception e)
        {

        }

        propertyString.Add(string.Format("Sell price: {0}", _item.priceItem));
    }

    private string _fixValue(int _att, bool isPercent = false)
    {
        if (!isPercent) return float.Parse(_myEquipment.getValue(_att.ToString()).ToString()).ToString("f1");
        return (float.Parse(_myEquipment.getValue(_att.ToString()).ToString()) * 100).ToString("f2");
    }

    string getNameWeapon(int idWeapon)
    {
        switch (idWeapon)
        {
            case 4: return "Dagger";
            case 5: return "Fireball";
            case 6: return "Axe";
            case 7: return "Hammer";
            case 8: return "Staff";
            case 9: return "Bow";
            case 10: return "Mace";
            case 11: return "Sword";
        }
        return "Unknown";
    }

}
