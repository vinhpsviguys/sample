using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    public static StorageUI _instance;


    private EquipmentsCharacter _equipmentsCharacter;
    [SerializeField] private RewardUseItemInBag _rewardItem;
    [SerializeField]
    private Sprite _btnOn, _btnOff;
    [SerializeField]
    private Button _equipmentButton, _itemButton, _gemButton;

    [SerializeField]
    private GameObject _equipmentTabObj, _itemTabObj, _gemTabObj;

    [SerializeField]
    private ScrollRect _scrollRectBagEquipment, _scrollRectBagItem, _scrollRectBagGem;
    [SerializeField]
    private Transform _contenBagEquipment, _contentBagItem, _contentBagGem;
    [SerializeField]
    private GameObject _bagHeroPanel;
    [SerializeField]
    private ItemInfoStorage _infoItemPanel;
    [SerializeField]
    private Text _capacity;

    private ItemInforController _itemInfor;

    [SerializeField]
    private GameObject _filterBag;

    [SerializeField]
    private GameObject _itemPrefabs;
    private GameObject _tempObject;
    private List<ItemUIScript> _listSlotEquipment;
    private List<ItemUIScript> _listSlotItem;
    private List<ItemUIScript> _listSlotGem;


    //private ItemSlotProperties _tempItemSlot;
    private int numberCapacity = 0;
    private int numberEquipmentInBag = 0;
    private int numberItemInBag = 0;
    private int numberGemInBag = 0;
    private bool createdSlot = false;


    private int idTabSelected;
    private int idItemSelected;

    private bool isActiveStorageUI = false;
    bool isCanUse;
    int _myCurrentLevel;
    private List<int> _listItemCanUse = new List<int> { 31, 65, 66, 67, 68, 70 };

    private TypeEquipmentCharacter _myTypeBag;
    private EquipmentItem _myEquipment, _tempItem;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            _listSlotEquipment = new List<ItemUIScript>();
            _listSlotItem = new List<ItemUIScript>();
            _listSlotGem = new List<ItemUIScript>();
        }
        else DestroyImmediate(this.gameObject);
    }
    void Start()
    {
        _itemInfor = ItemInforController.instance;
    }
    private void OnEnable()
    {
        _equipmentsCharacter = CharacterInfo._instance._myEquipments;
        _myCurrentLevel = int.Parse(SplitDataFromServe._heroCurrentPLay.level);
        _bagHeroPanel.transform.localPosition = new Vector3(_bagHeroPanel.transform.localPosition.x, 1050);
        _bagHeroPanel.transform.DOLocalMoveY(0, 0.35f).SetEase(Ease.OutBack);

        _equipmentButton.onClick.AddListener(OpenEquipmentTab);
        _itemButton.onClick.AddListener(OpenItemTab);
        _gemButton.onClick.AddListener(OpenGemTab);

        numberCapacity = CharacterInfo._instance._baseProperties.SlotChest;
        CreateSlotViewInEquipmentBag();

        idTabSelected = -1;
        numberEquipmentInBag = -1;
        numberItemInBag = -1;
        numberGemInBag = -1;

        OpenEquipmentTab();
        isActiveStorageUI = true;
        ItemUIScript.OnItemClicked += ItemClick;
        ItemInforController.OnBtnClick += BtnItemInforClick;
    }
    private void OnDisable()
    {
        isActiveStorageUI = false;
        ItemUIScript.OnItemClicked -= ItemClick;
        ItemInforController.OnBtnClick -= BtnItemInforClick;
    }

    void OpenEquipmentTab()
    {
        if (idTabSelected == 0) return;
        idTabSelected = 0;
        _equipmentButton.image.sprite = _btnOn;
        _itemButton.image.sprite = _btnOff;
        _gemButton.image.sprite = _btnOff;

        _equipmentTabObj.SetActive(true);
        _itemTabObj.SetActive(false);
        _gemTabObj.SetActive(false);
        DisplayEquipmentInBag();
    }
    void OpenItemTab()
    {
        if (idTabSelected == 1) return;
        idTabSelected = 1;
        _equipmentButton.image.sprite = _btnOff;
        _itemButton.image.sprite = _btnOn;
        _gemButton.image.sprite = _btnOff;

        _equipmentTabObj.SetActive(false);
        _itemTabObj.SetActive(true);
        _gemTabObj.SetActive(false);
        DisplayItemInBag();
    }
    void OpenGemTab()
    {
        if (idTabSelected == 2) return;
        idTabSelected = 2;
        _equipmentButton.image.sprite = _btnOff;
        _itemButton.image.sprite = _btnOff;
        _gemButton.image.sprite = _btnOn;

        _equipmentTabObj.SetActive(false);
        _itemTabObj.SetActive(false);
        _gemTabObj.SetActive(true);
        DisplayGemInBag();
    }

    private void CreateSlotViewInEquipmentBag()
    {
        if (!createdSlot)
        {
            for (int i = 0; i < numberCapacity; i++)
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.parent = _contenBagEquipment;
                _tempObject.transform.localScale = Vector3.one;
                _tempObject.name = i.ToString();
                _tempObject.tag = "EmptySlotBag";
                _listSlotEquipment.Add(_tempObject.GetComponent<ItemUIScript>());
                _listSlotEquipment[i].idItemSlot = i;
                _listSlotEquipment[i].SetEmpty();
            }
            _scrollRectBagEquipment.verticalNormalizedPosition = 1f;
            createdSlot = true;
        }
    }

    internal void DisplayEquipmentInBag()
    {
        int _numberItem = SplitDataFromServe._listEquipmentInBag.Count;
        for (int i = 0; i < _numberItem; i++)
        {
            _tempObject = _listSlotEquipment[i].gameObject;
            _tempObject.tag = "EquipmentSlotBag";
            _listSlotEquipment[i].SetData(0, SplitDataFromServe._listEquipmentInBag[i].idItemInit, SplitDataFromServe._listEquipmentInBag[i].levelUpgraded, SplitDataFromServe._listEquipmentInBag[i].rarelyItem,
                 (SplitDataFromServe._listEquipmentInBag[i].typeItem == TypeEquipmentCharacter.Buff ? 1 : (SplitDataFromServe._listEquipmentInBag[i].typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
        }
        for (int i = _numberItem; i < numberEquipmentInBag; i++) _listSlotEquipment[i].SetEmpty();
        numberEquipmentInBag = _numberItem;

        _capacity.text = string.Format("{0}/{1}", numberEquipmentInBag, numberCapacity);
    }
    internal void DisplayItemInBag(bool isForceUpdate = false)
    {
        numberItemInBag = SplitDataFromServe._listItemInBag.Count;
        for (int i = 0; i < numberItemInBag; i++)
        {
            if (i < _listSlotItem.Count)
            {
                _listSlotItem[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.parent = _contentBagItem;
                _tempObject.transform.localScale = Vector3.one;
                _tempObject.name = i.ToString();
                _tempObject.tag = "ItemSlotBag";
                _listSlotItem.Add(_tempObject.GetComponent<ItemUIScript>());
            }
            _listSlotItem[i].idItemSlot = i;
            _listSlotItem[i].SetData(1, int.Parse(SplitDataFromServe._listItemInBag[i].getValue("idit").ToString()),
                int.Parse(SplitDataFromServe._listItemInBag[i].getValue("quantity").ToString()), 0);
        }
        for (int i = _listSlotItem.Count - 1; i >= numberItemInBag; i--)
        {
            GameObject.Destroy(_listSlotItem[i].gameObject);
            _listSlotItem.RemoveAt(i);
        }
        _scrollRectBagItem.verticalNormalizedPosition = 1f;
        _capacity.text = string.Empty;
    }
    internal void DisplayGemInBag(bool isForceUpdate = false)
    {
        numberGemInBag = SplitDataFromServe._listGemInBag.Count;
        for (int i = 0; i < numberGemInBag; i++)
        {
            if (i < _listSlotGem.Count)
            {
                _listSlotGem[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.parent = _contentBagGem;
                _tempObject.transform.localScale = Vector3.one;
                _tempObject.name = i.ToString();
                _tempObject.tag = "GemSlotBag";
                _listSlotGem.Add(_tempObject.GetComponent<ItemUIScript>());
            }
            _listSlotGem[i].idItemSlot = i;
            _listSlotGem[i].SetData(2, int.Parse(SplitDataFromServe._listGemInBag[i].getValue("idig").ToString()),
                int.Parse(SplitDataFromServe._listGemInBag[i].getValue("quantity").ToString()), 0);
        }
        for (int i = _listSlotGem.Count - 1; i >= numberGemInBag; i--)
        {
            GameObject.Destroy(_listSlotGem[i].gameObject);
            _listSlotGem.RemoveAt(i);
        }
        _scrollRectBagGem.verticalNormalizedPosition = 1f;

        _capacity.text = string.Empty;
    }

    public void ItemClick()
    {
        int id = ItemUIScript.idClicked;
        if (_filterBag.activeSelf) return;
        if (!isActiveStorageUI) return;
        idItemSelected = id;
        if (idTabSelected == 0)//equipment
        {
            //_itemInfor._myState = StateInforItem.SelectEquipment;
            _itemInfor.SetData(SplitDataFromServe._listEquipmentInBag[id], _listSlotEquipment[id]._iconImg.sprite);
            _itemInfor.SetState(StateInforItem.SellEquipment);
        }
        else if (idTabSelected == 1)//item
        {
            int _currentId = int.Parse(SplitDataFromServe._listItemInBag[id].getValue("idit").ToString());
            if (_listItemCanUse.Contains(_currentId)) isCanUse = true;
            else isCanUse = false;
            if (!isCanUse)
            {
                _infoItemPanel.SetData(false, SplitDataFromServe._listItemInBag[id], _listSlotItem[id]._iconImg.sprite);
            }
            else
            {
                _itemInfor.SetData(SplitDataFromServe._listItemInBag[id], _listSlotItem[id]._iconImg.sprite);
                _itemInfor.SetState(StateInforItem.SellItem);
            }
        }
        else if (idTabSelected == 2)//runestone
        {
            _infoItemPanel.SetData(true, SplitDataFromServe._listGemInBag[id], _listSlotGem[id]._iconImg.sprite);
        }
    }
    private void BtnItemInforClick()
    {
        string param = ItemInforController.Message;
        if (!isActiveStorageUI) return;

        if (param.Equals("UseItem"))
        {
            StartCoroutine(ServerAdapter.UseItemInShop(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
            int.Parse(SplitDataFromServe._listItemInBag[idItemSelected].getValue("idht").ToString()), 1,
                     result =>
                     {
                         if (result.StartsWith("Error"))
                         {
                             TextNotifyScript.instance.SetData(result);
                         }
                         else
                         {
                             var N = SimpleJSON.JSON.Parse(result);
                             int idItem = int.Parse(SplitDataFromServe._listItemInBag[idItemSelected].getValue("idit").ToString());
                             if (idItem == 31) //dùng thẻ protect
                             {
                                 long timeSticks = long.Parse(N["timeprotection"].Value);
                                 DateTime _timeActive = new DateTime(timeSticks * 10000000 + new DateTime(1969, 12, 31, 12, 0, 0).Ticks);
                                 _rewardItem.SetData("Used Item: " + SplitDataFromServe._listItemInBag[idItemSelected].getValue("name").ToString(), "Time Active: " + _timeActive.ToLocalTime());
                             }
                             else if (idItem >= 65 && idItem <= 68) //dùng thẻ
                             {
                                 long timeSticks = long.Parse(N["timedaypremium"].Value);
                                 DateTime _timeActive = new DateTime(timeSticks * 10000000 + new DateTime(1969, 12, 31, 12, 0, 0).Ticks);
                                 _rewardItem.SetData("Used Item: " + SplitDataFromServe._listItemInBag[idItemSelected].getValue("name").ToString(), "Time Active: " + _timeActive.ToLocalTime());
                             }
                             else if (idItem == 70)//reset skill point
                             {
                                 SplitDataFromServe._heroSkill.Clear();
                                 int numberSkillPoint = N["skillpoint"].AsInt;
                                 CharacterInfo._instance._skillPoints += numberSkillPoint;
                                 _rewardItem.SetData("Used Item: " + SplitDataFromServe._listItemInBag[idItemSelected].getValue("name").ToString(), "Skill point return: " + numberSkillPoint);
                             }
                             SplitDataFromServe.SubItemInBag(idItemSelected, 1);
                             DisplayItemInBag();
                         }
                     }));
        }
        else if (param.Equals("SellItem"))
        {
            _infoItemPanel.SetData(false, SplitDataFromServe._listItemInBag[idItemSelected], _listSlotItem[idItemSelected]._iconImg.sprite);
        }
        else if (param.Equals("UpdateEquipment")) { DisplayEquipmentInBag(); }
        else if (param.Equals("SellEquipment"))//Chọn đồ
        {
            StartCoroutine(ServerAdapter.SellEquipment(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, SplitDataFromServe._listEquipmentInBag[idItemSelected].idItem,
                  result =>
                  {
                      if (result.StartsWith("Error"))
                      {
                          TextNotifyScript.instance.SetData(result);
                      }
                      else
                      {
                          var N = SimpleJSON.JSON.Parse(result);
                          CharacterInfo._instance._baseProperties.Gold += N["plusgold"].AsInt;
                          this.PostEvent(EventID.OnPropertiesChange);

                          SplitDataFromServe._listEquipmentInBag.RemoveAt(idItemSelected);
                          DisplayEquipmentInBag();
                      }
                  }));
        }
        else if (param.Equals("ReplaceEquipmentFromBag"))
        {
            _myEquipment = SplitDataFromServe._listEquipmentInBag[idItemSelected];
            isCanUse = true;
            _myTypeBag = _myEquipment.typeItem;
            if (_myEquipment.levelRequired <= _myCurrentLevel && SplitDataFromServe._listSuitableEquipment.Contains(_myEquipment.idTypeEquipment))
            {
                if (_myEquipment.typeItem == TypeEquipmentCharacter.Weapon)
                {
                    if (!CharacterInfo._instance._baseProperties._classCharacter.Equals(MappingData.GetSuitableClassForWeapon(_myEquipment.idTypeEquipment)))
                    {
                        isCanUse = false;
                    }
                }
            }
            else isCanUse = false;

            //Debug.Log("is can use this equipment " + isCanUse);
            if (isCanUse)
            {
                StartCoroutine(ServerAdapter.ExecuteChangeEquipment(_myEquipment, result =>
                {
                    if (result.StartsWith("Error"))
                    {
                        TextNotifyScript.instance.SetData(result);
                    }
                    else
                    {
                        _tempItem = _equipmentsCharacter.MappingTypeItemToItem(_myTypeBag);
                        if (_tempItem != null)
                        {
                            SplitDataFromServe._listEquipmentInBag.Add(_tempItem);
                        }
                        _equipmentsCharacter.EquipItem(_myTypeBag, _myEquipment);
                        SplitDataFromServe._listEquipmentInBag.Remove(_myEquipment);
                        //CharacterInfo._instance._baseProperties.AddBonusItem(_myEquipItems[id]);
                        DisplayEquipmentInBag();
                    }
                }));
            }
        }

    }

    public void CloseThisDialog()
    {
        _bagHeroPanel.transform.DOLocalMoveY(1100, 0.35f).SetEase(Ease.InBack).OnComplete(OnCompleteGoOut);
    }
    private void OnCompleteGoOut()
    {
        this.gameObject.SetActive(false);

    }
}
