using UnityEngine;
public enum StateInforItem
{
    None,
    ShowInforEquipment, UseEquipment, ReplaceEquipment, SelectEquipment, CompareEquipment, SellEquipment,
    SelectItem, DeselectItem, ShowInForItem, SellItem
}

public class ItemInforController : MonoBehaviour
{
    public static ItemInforController instance;
    [SerializeField] private GameObject _inforTabPrefabs;
    private GameObject _tempGameObject;
    private ItemInforScript infor1, infor2;
    internal StateInforItem _myState;
    EquipmentItem _equipment1, _equipment2;
    Item _item1;
    Sprite _spriteEquip1, _spriteEquip2;
    Sprite _spriteItem1;

    private Vector3 _posiTab1 = new Vector3(-415, 0);
    private Vector3 _posiTab2 = new Vector3(415, 0);

    public static string Message;
    public delegate void ActionEvent();
    public static event ActionEvent OnBtnClick;


    internal void SetData(EquipmentItem item, Sprite _img)
    {
        _equipment1 = item;
        _spriteEquip1 = _img;
    }
    internal void SetData(Item item, Sprite _img)
    {
        _item1 = item;
        _spriteItem1 = _img;
    }
    internal void SetDataNewEquipment(EquipmentItem item, Sprite _img)
    {
        _equipment2 = item;
        _spriteEquip2 = _img;
    }
    internal void SetState(StateInforItem _state)
    {
        _myState = _state;
        this.gameObject.SetActive(true);
    }
    void Awake()
    {
        //infor1.gameObject.SetActive(false);
        //infor2.gameObject.SetActive(false);
    }
    void OnEnable()
    {
        //infor1.transform.GetChild(2).gameObject.SetActive(true);
        if (infor1 == null)
        {
            _tempGameObject = Instantiate(_inforTabPrefabs);
            _tempGameObject.transform.SetParent(this.transform);
            _tempGameObject.transform.localScale = Vector3.one;
            infor1 = _tempGameObject.GetComponent<ItemInforScript>();
        }
        if (infor2 == null)
        {
            _tempGameObject = Instantiate(_inforTabPrefabs);
            _tempGameObject.transform.SetParent(this.transform);
            _tempGameObject.transform.localScale = Vector3.one;
            infor2 = _tempGameObject.GetComponent<ItemInforScript>();
        }
        switch (_myState)
        {
            case StateInforItem.None:
            case StateInforItem.ShowInforEquipment:
            case StateInforItem.SelectEquipment:
            case StateInforItem.UseEquipment:
            case StateInforItem.SellEquipment:
                infor2.gameObject.SetActive(false);
                infor1.transform.localPosition = Vector3.zero;
                infor1.SetData(_equipment1, _spriteEquip1, _myState);
                infor1.gameObject.SetActive(true);
                break;
            case StateInforItem.SelectItem:
            case StateInforItem.DeselectItem:
            case StateInforItem.ShowInForItem:
            case StateInforItem.SellItem:
                infor2.gameObject.SetActive(false);
                infor1.transform.localPosition = Vector3.zero;
                infor1.SetData(_item1, true, _spriteItem1, _myState);
                infor1.gameObject.SetActive(true);
                break;
            case StateInforItem.ReplaceEquipment:
                infor1.transform.localPosition = _posiTab1;
                infor1.SetData(_equipment1, _spriteEquip1, StateInforItem.None);
                infor1.gameObject.SetActive(true);

                infor2.transform.localPosition = _posiTab2;
                infor2.SetData(_equipment2, _spriteEquip2, StateInforItem.ReplaceEquipment);
                infor2.gameObject.SetActive(true);
                break;
            case StateInforItem.CompareEquipment:
                infor1.transform.localPosition = _posiTab1;
                infor1.SetData(_equipment1, _spriteEquip1, StateInforItem.None);
                infor1.gameObject.SetActive(true);

                infor2.transform.localPosition = _posiTab2;
                infor2.SetData(_equipment2, _spriteEquip2, StateInforItem.None);
                infor2.gameObject.SetActive(true);
                break;
        }
    }
    void OnDisable()
    {
        infor1.gameObject.SetActive(false);
        infor2.gameObject.SetActive(false);
    }
    public void ClosePopup()
    {
        //FittingRoomController.lastIDSelected = -1;
        this.gameObject.SetActive(false);
    }
    public void BtnItemInforClicked(StateInforItem state, int idBtn)
    {
        switch (state)
        {
            case StateInforItem.ShowInforEquipment:
                if (idBtn == 0)//Btn replace item
                {
                    Message = "ReplaceEquipment";
                }
                else if (idBtn == 1)// Btn Remove Item
                {
                    Message = "RemoveEquipment";
                }
                break;
            case StateInforItem.ReplaceEquipment:
                Message = "ChangeEquipment";
                break;

            case StateInforItem.SelectEquipment:
                Message = "SelectEquipment";
                break;
            case StateInforItem.SelectItem:
                Message = "SelectItem";
                break;
            case StateInforItem.DeselectItem:
                Message = "DeSelectItem";
                break;
            case StateInforItem.UseEquipment:
                Message = "ChangeEquipment";
                break;
            case StateInforItem.SellEquipment:
                if (idBtn == 0)//Btn replace item
                {
                    Message = "ReplaceEquipmentFromBag";
                }
                else if (idBtn == 1)// Btn Sell Item
                {
                    Message = "SellEquipment";
                }
                break;
            case StateInforItem.SellItem:
                if (idBtn == 0)//Btn replace item
                {
                    Message = "UseItem";
                }
                else if (idBtn == 1)// Btn Sell Item
                {
                    Message = "SellItem";
                }
                break;
        }
        OnBtnClick();
        this.gameObject.SetActive(false);
    }
    internal void MakeEvent(string eventName)
    {
        Message = eventName;
        OnBtnClick();
    }

}
