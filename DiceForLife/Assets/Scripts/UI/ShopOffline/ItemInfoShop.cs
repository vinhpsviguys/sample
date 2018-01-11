using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoShop : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image _imgItem;
    [SerializeField] private Text _nameItemTxt, _infoItemTxt;
    private RectTransform _border;

    private string _descript;
    private int _minHeigh = 600;
    private int _maxHeigh = 1000;
    private int numberRow;
    private void Awake()
    {
        _border = transform.GetChild(0).GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.35f).SetEase(Ease.OutBack);
        StartCoroutine(loadInfoItem());
    }

    IEnumerator loadInfoItem()
    {
        Item cachedItem = ShopUI._instance.LoadItemFromId(ShopUI._instance._cachedIDItemInfo);
        _nameItemTxt.text = cachedItem.getValue("name").ToString();

        _descript = cachedItem.getValue("descripton").ToString().Replace(@"\r", "");
        _infoItemTxt.text = _descript.Replace(@"\n", "\n");

        numberRow = _descript.Length / 50;
        _border.sizeDelta = new Vector2(_border.sizeDelta.x, numberRow < 3 ? _minHeigh : _minHeigh + numberRow * 50);

        if (int.Parse(cachedItem.getValue("typeid").ToString()) == 1)
        {
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItemByID(int.Parse(cachedItem.getValue("idtemp").ToString()), value => _imgItem.sprite = value));
        }
        else if (int.Parse(cachedItem.getValue("typeid").ToString()) == 2)
        {
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(int.Parse(cachedItem.getValue("idtemp").ToString()), value => _imgItem.sprite = value));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter as GameObject;
        if (enterObj.name == this.name)
        {
            _border.DOScale(0, 0.35f).SetEase(Ease.InBack).OnComplete(CompleteHide);
        }
    }
    void CompleteHide()
    {
        this.gameObject.SetActive(false);

    }
}
