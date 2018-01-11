using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIScript : MonoBehaviour
{
    internal int idItemSlot = 0;

    bool isEmpty;
    private int typeItem;//0 là equipment, 1 là material, 2 là gem
    private int typeEquipment;
    private int idItem;
    private int quantity;
    private int rare;

    [SerializeField] private Image _bgImg;
    public Image _iconImg;
    [SerializeField] private Text _myTxt;

    private float _defaultSizeBG = 145;
    private float _defaultSizeIcon = 105;


    public static int idClicked;
    public delegate void ActionEvent();
    public static event ActionEvent OnItemClicked;


    bool isResizeObject;
    private void Start()
    {
        isResizeObject = false;
    }
    internal void SetData(int typeItem, int idItem, int quantity, int _rare, int typeEquipment = 0)
    {
        if (!isEmpty && this.typeItem == typeItem && this.idItem == idItem && this.quantity == quantity && this.rare == _rare && this.typeEquipment == typeEquipment)
        {
            return;
        }
        isEmpty = false;
        this.typeItem = typeItem;
        this.typeEquipment = typeEquipment;
        this.idItem = idItem;
        this.quantity = quantity;
        this.rare = _rare;

        _iconImg.enabled = true;
        _bgImg.sprite = ControllerItemsInGame._instance._rareBorderItems[_rare];

        StartCoroutine(LoadIcon());

        if (typeItem == 0)
        {
            if (quantity == 0) _myTxt.text = string.Empty;
            else _myTxt.text = string.Format("+{0}", quantity);
        }
        else _myTxt.text = quantity.ToString();
    }

    internal void ReupdateLevelUpgraded(int levelUpgraded)
    {
        if (levelUpgraded == 0) _myTxt.text = string.Empty;
        else _myTxt.text = string.Format("+{0}", levelUpgraded);
    }
    internal void SetEmpty()
    {
        isEmpty = true;
        _myTxt.text = string.Empty;
        _iconImg.enabled = false;
        _bgImg.sprite = ControllerItemsInGame._instance._rareBorderItems[6];
    }

    IEnumerator LoadIcon()
    {
        if (typeItem == 0)
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForEquipmentByID(idItem, typeEquipment, value =>
             {
                 _iconImg.sprite = value;

             }));
        else if (typeItem == 1)
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItemByID(idItem, value =>
            {
                _iconImg.sprite = value;
            }));
        else
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(idItem, value =>
            {
                _iconImg.sprite = value;
            }));
        if (!isResizeObject)
        {
            //float _newSizeIcon = 105 * _bgImg.GetComponent<RectTransform>().sizeDelta.x / 145;
            //_iconImg.GetComponent<RectTransform>().sizeDelta = Vector2.one * _newSizeIcon;
            _iconImg.transform.localScale = Vector3.one * (_bgImg.GetComponent<RectTransform>().sizeDelta.x / 145);
            isResizeObject = true;
        }
    }

    public void ItemClicked()
    {
        if (!isEmpty)
        {
            idClicked = idItemSlot;
            OnItemClicked();
            //this.PostEvent(EventID.ItemUIClicked, idItemSlot);
        }
    }
}
