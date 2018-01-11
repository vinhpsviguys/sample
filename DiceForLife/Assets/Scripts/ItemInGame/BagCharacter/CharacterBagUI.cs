using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CharacterBagUI : MonoBehaviour
{
    public Transform _myBorder;
    public Text _txtCapacity;
    //public ItemInforController _inforItem;

    private List<EquipmentItem> _myItems;
    private int numberEquipment;


    //controll items
    public RectTransform _scrollViewParent;
    private List<ItemUIScript> _myItemsUI;
    private ItemUIScript _tempItemUI;
    public GameObject _itemPrefabs;
    private GameObject _tempObject;
    private int numberInRow = 10;
    private int sizeViewX = 1100;
    private int sizeViewY = 120;
    private int startXItem;
    private int stepXItem = 110;
    private int startYItem;
    private int numberRow;


    void Awake()
    {
        _myItemsUI = new List<ItemUIScript>();
    }
    void OnEnable()
    {
        _myItems = PlayerBag._instance._myItems;
        foreach (EquipmentItem _temp in _myItems)
        {
            Debug.Log(_temp.getValue("listidproperty"));
        }
        numberEquipment = _myItems.Count;
        _txtCapacity.text = string.Format("{0}/{1}", numberEquipment, PlayerBag._instance.MAX_CAPACITY);

        _myBorder.localScale = Vector3.one * 0.8f;
        _myBorder.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        CreateView();
        ItemUIScript.OnItemClicked += ItemUIScript_OnItemClicked;
    }

    private void ItemUIScript_OnItemClicked()
    {
        if (this.gameObject.activeSelf)
        {
            ItemInforController.instance.SetState(StateInforItem.None);
        }
    }
    private void OnDisable()
    {

        ItemUIScript.OnItemClicked -= ItemUIScript_OnItemClicked;
    }

    void CreateView()
    {
        int numberItem = _myItems.Count;
        startXItem = stepXItem / 2;
        numberRow = (numberItem - 1) / numberInRow + 1;
        startYItem = -sizeViewY / 2;
        _scrollViewParent.sizeDelta = new Vector2(sizeViewX, numberRow * sizeViewY);
        for (int i = 0; i < numberItem; i++)
        {
            if (i < _myItemsUI.Count)
            {
                _myItemsUI[i].gameObject.SetActive(true);
            }
            else
            {
                _tempObject = Instantiate(_itemPrefabs);
                _tempObject.transform.SetParent(_scrollViewParent.transform);
                _tempObject.transform.localScale = Vector3.one;
                _myItemsUI.Add(_tempObject.GetComponent<ItemUIScript>());
            }
            _myItemsUI[i].idItemSlot = i;
            //_myItemsUI[i].SetData();
            _myItemsUI[i].transform.localPosition = new Vector3(startXItem + (i % numberInRow) * stepXItem, startYItem - (i / numberInRow) * sizeViewY);
        }
        for (int i = numberItem; i < _myItems.Count; i++) _myItemsUI[i].gameObject.SetActive(false);
    }

    void Update()
    {

    }
    public void BtnBackClick()
    {
        _myBorder.localScale = Vector3.one;
        _myBorder.DOScale(0.5f, 0.3f).SetEase(Ease.InOutBack).OnComplete(CompleteInOutBack);
    }
    void CompleteInOutBack()
    {
        this.gameObject.SetActive(false);
    }

}
