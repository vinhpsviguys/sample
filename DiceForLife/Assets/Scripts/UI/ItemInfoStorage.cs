using DG.Tweening;
using SimpleJSON;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoStorage : MonoBehaviour, IPointerDownHandler
{
    //[SerializeField]
    private Transform _border;
    private StorageUI _myStorage;


    [SerializeField]
    private Image _itemImg, _itemBoundItem;
    [SerializeField]
    private InputField _numberQuantityItem;
    [SerializeField]
    private Text _itemNameText;
    [SerializeField]
    private Text _txtQuantity, _txtLevel;
    [SerializeField]
    private Text _desItemText;
    [SerializeField]
    private Text _priceItem;
    [SerializeField]
    private Button _sellBtn, _subBtnQtt, _addBtnQtt;


    private Item _itemCached;
    private bool isGem;
    private int _maxQuantity, _numberSell, _pricePerOne;
    private int idHeroItem;

    internal void SetData(bool isGems, Item _item, Sprite _iconItem)
    {
        this.isGem = isGems;
        _itemCached = _item;
        _itemImg.sprite = _iconItem;
        _itemImg.SetNativeSize();
        _itemBoundItem.sprite = ControllerItemsInGame._instance._rareBorderItems[0];

        if (isGem)
        {
            idHeroItem = int.Parse(_itemCached.getValue("idhg").ToString());
            _desItemText.text = _itemCached.getValue("description").ToString().Replace(@"\n", "\n");
            _txtLevel.text = string.Format("Level: {0}", _itemCached.getValue("level").ToString());
        }
        else
        {
            idHeroItem = int.Parse(_itemCached.getValue("idht").ToString());
            _desItemText.text = _itemCached.getValue("descripton").ToString().Replace(@"\n", "\n");
            _txtLevel.text = string.Format("Level required: {0}", _itemCached.getValue("levelrequired").ToString());
        }
        //Debug.Log(_desItemText.text);
        _itemNameText.text = _itemCached.getValue("name").ToString();
        _maxQuantity = int.Parse(_itemCached.getValue("quantity").ToString());
        _pricePerOne = int.Parse(_itemCached.getValue("sellprice").ToString());
        _numberSell = _maxQuantity;

        _priceItem.text = (_pricePerOne * _numberSell).ToString();
        _txtQuantity.text = string.Format("Quantity: {0}", _maxQuantity);
        _numberQuantityItem.text = _numberSell.ToString();

        this.gameObject.SetActive(true);
    }

    private void Awake()
    {
        _border = transform.GetChild(0);
        _myStorage = transform.parent.GetComponent<StorageUI>();
    }
    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.35f).SetEase(Ease.OutBack);
        _sellBtn.onClick.AddListener(SellItem);
        _numberQuantityItem.onValueChanged.AddListener(delegate { UpdateNumberItemField(); });
        _subBtnQtt.onClick.AddListener(AddQttFuction);
        _addBtnQtt.onClick.AddListener(SubQttFuction);
    }
    private void OnDisable()
    {
        _sellBtn.onClick.RemoveListener(SellItem);
        _subBtnQtt.onClick.RemoveListener(AddQttFuction);
        _addBtnQtt.onClick.RemoveListener(SubQttFuction);
        _numberQuantityItem.onValueChanged.RemoveListener(delegate { UpdateNumberItemField(); });
    }

    void AddQttFuction()
    {
        if (_numberSell < _maxQuantity)
        {
            _numberSell++;
            _priceItem.text = (_pricePerOne * _numberSell).ToString();
            _numberQuantityItem.text = _numberSell.ToString();
        }
    }
    void SubQttFuction()
    {
        if (_numberSell > 1)
        {
            _numberSell--;
            _priceItem.text = (_pricePerOne * _numberSell).ToString();
            _numberQuantityItem.text = _numberSell.ToString();
        }
    }

    void UpdateNumberItemField()
    {
        _numberSell = int.Parse(_numberQuantityItem.text);
        if (_numberSell <= 0) _numberSell = 1;
        else if (_numberSell > _maxQuantity) _numberSell = _maxQuantity;

        _priceItem.text = (_pricePerOne * _numberSell).ToString();
        _numberQuantityItem.text = _numberSell.ToString();
    }


    //IEnumerator LoadImgItem(Image _img, int _idInitItem, int _typeItem)
    //{
    //    if (_typeItem == 1)
    //    {
    //        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForEquipmentByID(_idInitItem, value => _img.sprite = value));
    //    }
    //    else if (_typeItem == 2)
    //    {
    //        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItemByID(_idInitItem, value => _img.sprite = value));
    //    }
    //    else if (_typeItem == 3)
    //    {
    //        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(_idInitItem, value => _img.sprite = value));
    //    }
    //}


    void SellItem()
    {
        if (isGem)
        {
            StartCoroutine(ServerAdapter.SellGem(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, idHeroItem, _numberSell,
               result =>
               {
                   if (result.StartsWith("Error"))
                   {
                       TextNotifyScript.instance.SetData(result);
                   }
                   else
                   {
                       var N = JSON.Parse(result);
                       if (N["quantity"].AsInt == 0)
                       {
                           SplitDataFromServe._listGemInBag.Remove(_itemCached);
                       }
                       else _itemCached.setValue("quantity", _maxQuantity - _numberSell);

                       CharacterInfo._instance._baseProperties.Gold += N["goldplus"].AsInt;

                       this.PostEvent(EventID.OnPropertiesChange);

                       _myStorage.DisplayGemInBag(true);
                   }
               }));
        }
        else
        {
            StartCoroutine(ServerAdapter.SellItem(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, idHeroItem, _numberSell,
                result =>
      {
          if (result.StartsWith("Error"))
          {
              TextNotifyScript.instance.SetData(result);
          }
          else
          {
              if (_numberSell == _maxQuantity)
              {
                  SplitDataFromServe._listItemInBag.Remove(_itemCached);
              }
              else _itemCached.setValue("quantity", _maxQuantity - _numberSell);
              CharacterInfo._instance._baseProperties.Gold += _numberSell * _pricePerOne;
              this.PostEvent(EventID.OnPropertiesChange);

              _myStorage.DisplayItemInBag(true);
          }
      }));
        }
        ClosePopup();
    }

    public void ClosePopup()
    {
        _border.DOScale(0, 0.35f).SetEase(Ease.InBack).OnComplete(CompleteClose);
    }
    void CompleteClose()
    {
        this.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter as GameObject;
        if (enterObj.name == "infoItemPanel")
        {
            //this.gameObject.SetActive(false);
            ClosePopup();
        }
    }
}
