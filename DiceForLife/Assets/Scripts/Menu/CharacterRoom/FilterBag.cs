using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FilterBag : MonoBehaviour
{
    private TypeEquipmentCharacter _myTypeBag;
    private List<EquipmentItem> _myEquipItems;
    private EquipmentsCharacter _equipmentsCharacter;
    private EquipmentItem _tempItem;
    private ItemInforController _itemInfor;

    [SerializeField]
    private Transform _myBorder;


    private int _lastIndexSelected;
    private List<ItemUIScript> _myItems;

    [SerializeField]
    private RectTransform _scrollViewParent;
    [SerializeField]
    private GameObject _itemPrefabs;
    private GameObject _tempObject;

    private string _weaponCharacter = string.Empty;
    private int idClassHero;

    private bool isActive;
    void Awake()
    {
        _myItems = new List<ItemUIScript>();
        _lastIndexSelected = -1;
    }
    void Start()
    {
        _itemInfor = ItemInforController.instance;
    }
    void OnEnable()
    {
        isActive = true;
        _myBorder.transform.localScale = Vector3.one * 0.5f;
        _myBorder.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        CreateView();

        ItemUIScript.OnItemClicked += ItemUIScript_OnItemClicked;
        ItemInforController.OnBtnClick += BtnItemInforClick;
    }
    void OnDisable()
    {
        isActive = false;
        ItemUIScript.OnItemClicked -= ItemUIScript_OnItemClicked;
        ItemInforController.OnBtnClick -= BtnItemInforClick;
    }

    internal void SetFilter(TypeEquipmentCharacter _typeFilter)
    {
        Debug.Log("type filter " + _typeFilter);
        _myTypeBag = _typeFilter;
        FliterByTypeEquipment(_myTypeBag);
        gameObject.SetActive(true);
    }
    public void FliterByTypeEquipment(TypeEquipmentCharacter _type)
    {
        int _myCurrentLevel = int.Parse(SplitDataFromServe._heroCurrentPLay.level);


        if (_myEquipItems == null) _myEquipItems = new List<EquipmentItem>();
        else _myEquipItems.Clear();

        SplitDataFromServe._listEquipmentInBag.ForEach((EquipmentItem item) =>
        {
            //Debug.Log(item.idTypeEquipment + " | " + item.typeItem);

            if (item.levelRequired <= _myCurrentLevel && SplitDataFromServe._listSuitableEquipment.Contains(item.idTypeEquipment))
            {
                if (_type == TypeEquipmentCharacter.Weapon || _type == TypeEquipmentCharacter.OffhandWeapon)//vũ khí
                {
                    if (item.typeItem == TypeEquipmentCharacter.Weapon || item.typeItem == TypeEquipmentCharacter.OffhandWeapon)
                    {
                        _myEquipItems.Add(item);
                    }
                }
                else if (item.typeItem == _type)
                {
                    _myEquipItems.Add(item);
                }
            }
        });
    }

    private void BtnItemInforClick()
    {
        string param = ItemInforController.Message;
        if (param.Equals("ChangeEquipment")) ChangeEquipment();
    }
    void CreateView()
    {
        int numberItem = _myEquipItems.Count;
        //Debug.Log(_myEquipItems.Count);
        for (int i = 0; i < numberItem; i++)
        {
            if (i < _myItems.Count)
            {
                _myItems[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.parent = _scrollViewParent.transform;
                _tempObject.transform.localScale = Vector3.one;
                _tempObject.transform.localPosition = Vector3.zero;
                _tempObject.name = i.ToString();

                _myItems.Add(_tempObject.GetComponent<ItemUIScript>());
                _myItems[i].idItemSlot = i;
            }
            _myItems[i].SetData(0, _myEquipItems[i].idItemInit, _myEquipItems[i].levelUpgraded, _myEquipItems[i].rarelyItem,
                (_myEquipItems[i].typeItem == TypeEquipmentCharacter.Buff ? 1 : (_myEquipItems[i].typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
        }

        for (int i = _myItems.Count - 1; i >= numberItem; i--)
        {
            GameObject.Destroy(_myItems[i].gameObject);
            _myItems.RemoveAt(i);
        }
        // _scrollViewParent.verticalNormalizedPosition = 1f;
    }
    public void BtnExitClick()
    {
        this.gameObject.SetActive(false);
    }
    public void ItemUIScript_OnItemClicked()
    {
        int id = ItemUIScript.idClicked;
        if (!isActive) return;
        if (id >= _myEquipItems.Count)
        {
            Debug.Log("The nao day"); return;
        }

        _lastIndexSelected = id;
        if (_itemInfor._myState == StateInforItem.ShowInforEquipment || _itemInfor._myState == StateInforItem.ReplaceEquipment)
        {
            _itemInfor.SetDataNewEquipment(_myEquipItems[id], _myItems[id]._iconImg.sprite);
            _itemInfor.SetState(StateInforItem.ReplaceEquipment);
        }
        else
        {
            _itemInfor.SetData(_myEquipItems[id], _myItems[id]._iconImg.sprite);
            _itemInfor.SetState(StateInforItem.UseEquipment);
        }
    }

    public void ChangeEquipment()
    {
        /* 3 class cần tham gia thay đổi
         * EquipmentsCharacer cần mặc trang bị mới
         * PlayerBag cần nhận thêm trang bị cũ
         * FittingRoomController cần update hình ảnh
         */
        int id = _lastIndexSelected;
        if (_equipmentsCharacter == null) _equipmentsCharacter = CharacterInfo._instance._myEquipments;
        if (_equipmentsCharacter.IsCanEquipItem(_myEquipItems[id]))
        {
            StartCoroutine(ServerAdapter.ExecuteChangeEquipment(_myEquipItems[id], result =>
            {
                if (result.StartsWith("Error"))
                {
                    TextNotifyScript.instance.SetData(result);
                }
                else
                {
                    //Debug.Log(_myTypeBag);
                    _tempItem = _equipmentsCharacter.MappingTypeItemToItem(_myTypeBag);
                    //Debug.Log(_tempItem);
                    if (_tempItem != null)
                    {
                        SplitDataFromServe._listEquipmentInBag.Add(_tempItem);
                        //CharacterInfo._instance._baseProperties.RemoveBonusItem(_tempItem);
                    }
                    _equipmentsCharacter.EquipItem(_myTypeBag, _myEquipItems[id]);
                    if (_myEquipItems[id].typeItem == TypeEquipmentCharacter.OffhandWeapon)
                    {
                        _myEquipItems[id].typeItem = TypeEquipmentCharacter.Weapon;
                    }
                    SplitDataFromServe._listEquipmentInBag.Remove(_myEquipItems[id]);
                    //CharacterInfo._instance._baseProperties.AddBonusItem(_myEquipItems[id]);

                    ItemInforController.instance.MakeEvent("UpdateEquipment");
                    this.gameObject.SetActive(false);
                }
            }));
        }
        else
        {
            MainMenuUI._instance.ShowErrorPopup(7);
        }
    }
}
