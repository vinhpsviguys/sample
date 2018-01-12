using DG.Tweening;
using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyItemShop : MonoBehaviour, IPointerDownHandler
{
    Item cachedItem = null;
    public GameObject _numberItemBound, _acceptItemBound;
    public Image _itemImgNumber, _itemImgAccept, _typePriceImg;
    public InputField _numberItemBuy;
    public Button subBtn, addBtn, buyBtn, acceptBtn;
    public Text priceItem, numberItemGet;
    public Sprite _goldSprite, _diamondSprite;
    int qtt = 1;
    private Transform _border;

    private int idCurrency, price;
    private bool isRunestone;

    private void Awake()
    {
        _border = transform.GetChild(0);
    }
    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.35f).SetEase(Ease.OutBack);
        qtt = 1;
        _numberItemBound.SetActive(true);
        _acceptItemBound.SetActive(false);
        cachedItem = ShopUI._instance.LoadItemFromId(ShopUI._instance._cachedIDItemBuy);

        if (cachedItem.getValue("type").ToString() == "Gold")
        {
            idCurrency = 0;
            _typePriceImg.sprite = _goldSprite;
        }
        else if ((cachedItem.getValue("type").ToString() == "Gem"))
        {
            idCurrency = 1;
            _typePriceImg.sprite = _diamondSprite;
        }
        price = int.Parse(cachedItem.getValue("price").ToString());


        StartCoroutine(loadInfoItem());
        _numberItemBuy.onValueChanged.AddListener(delegate { ChangeQtt(); });
        _numberItemBuy.onEndEdit.AddListener(delegate { ChangeQttWhenEndEdit(); });
        _numberItemBuy.text = qtt.ToString();
        subBtn.onClick.AddListener(SubQtt);
        addBtn.onClick.AddListener(AddQtt);
        buyBtn.onClick.AddListener(BuyItemFuction);
        acceptBtn.onClick.AddListener(AcceptFuction);
    }
    private void OnDisable()
    {
        _numberItemBuy.onValueChanged.RemoveListener(delegate { ChangeQtt(); });
        _numberItemBuy.onEndEdit.RemoveListener(delegate { ChangeQttWhenEndEdit(); });
        subBtn.onClick.RemoveListener(SubQtt);
        addBtn.onClick.RemoveListener(AddQtt);
        buyBtn.onClick.RemoveListener(BuyItemFuction);
        acceptBtn.onClick.RemoveListener(AcceptFuction);
    }

    void ChangeQttWhenEndEdit()
    {
        if (string.IsNullOrEmpty(_numberItemBuy.text))
        {
            qtt = 1;
            _numberItemBuy.text = qtt.ToString();
        }
    }
    void ChangeQtt()
    {
        if (!string.IsNullOrEmpty(_numberItemBuy.text))
        {
            qtt = int.Parse(_numberItemBuy.text);
            if (qtt <= 0)
            {
                qtt = 1;
            }
            _numberItemBuy.text = qtt.ToString();
            priceItem.text = (price * qtt).ToString();
        }
    }

    void AddQtt()
    {
        qtt++;
        _numberItemBuy.text = qtt.ToString();
        priceItem.text = (price * qtt).ToString();
    }
    void SubQtt()
    {
        if (qtt > 1)
        {
            qtt--;
        }
        _numberItemBuy.text = qtt.ToString();
        priceItem.text = (price * qtt).ToString();
    }

    IEnumerator loadInfoItem()
    {
        priceItem.text = price.ToString(); if (int.Parse(cachedItem.getValue("typeitem").ToString()) == 0)//gem
        {
            isRunestone = true;
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(int.Parse(cachedItem.getValue("idtemp").ToString()), value => { _itemImgNumber.sprite = value; _itemImgAccept.sprite = value; }));
        }
        else
        {
            isRunestone = false;
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItemByID(int.Parse(cachedItem.getValue("idtemp").ToString()), value => { _itemImgNumber.sprite = value; _itemImgAccept.sprite = value; }));
        }
    }


    void BuyItemFuction()
    {
        if (idCurrency == 0)
        {
            if (CharacterInfo._instance._baseProperties.Gold >= price * qtt)
                ExecuteBuyItemFuction();
            else ShopUI._instance.OpenWarningPanel("You haven't enought gold to buy this item");
        }
        else if (idCurrency == 1)
        {
            if (CharacterInfo._instance._baseProperties.Diamond >= price * qtt)
                ExecuteBuyItemFuction();
            else ShopUI._instance.OpenWarningPanel("You haven't enought diamond to buy this item");
        }
    }

    void ExecuteBuyItemFuction()
    {
#if UNITY_EDITOR
        Debug.Log("BUY item " + ShopUI._instance._cachedIDItemBuy);
#endif
        StartCoroutine(ServerAdapter.BuyItemInShop(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, ShopUI._instance._cachedIDItemBuy, qtt, result =>
        {
            if (result.StartsWith("Error"))
            {
                Debug.Log("do nothing");
            }
            else
            {
                var N = JSON.Parse(result);

                if (idCurrency == 0)
                {
                    CharacterInfo._instance._baseProperties.Gold -= price * qtt;
                }
                else if (idCurrency == 1)
                {
                    CharacterInfo._instance._baseProperties.Diamond -= price * qtt;
                }

                if (N["typeid"].Value == "gem")//đây là runstone
                {
                    Item _boughtItem = new Item(N["idhg"].AsInt, N["idig"].AsInt, N["quantity"].AsInt, 1, N["sellprice"].AsInt, N["uplevel"].AsInt);
                    bool existedItem = false;
                    foreach (Item _temp in SplitDataFromServe._listGemInBag)
                    {
                        if (int.Parse(_temp.getValue("idig").ToString()) == int.Parse(_boughtItem.getValue("idit").ToString()))
                        {
                            _temp.setValue("quantity", int.Parse(_boughtItem.getValue("quantity").ToString()));
                            existedItem = true;
                            break;
                        }
                    }
                    if (!existedItem)
                    {
                        SplitDataFromServe._listGemInBag.Add(_boughtItem);
                    }
                }
                else if (N["typeid"].Value == "item")// đây là item
                {
                    Item _boughtItem = new Item(N["idht"].AsInt, N["idit"].AsInt, cachedItem.getValue("name").ToString(), N["quantity"].AsInt, int.Parse(cachedItem.getValue("price").ToString()), N["levelrequired"].AsInt, cachedItem.getValue("descripton").ToString(), int.Parse(cachedItem.getValue("price").ToString()));
                    bool existedItem = false;
                    foreach (Item _temp in SplitDataFromServe._listItemInBag)
                    {
                        if (int.Parse(_temp.getValue("idit").ToString()) == int.Parse(_boughtItem.getValue("idit").ToString()))
                        {
                            _temp.setValue("quantity", int.Parse(_boughtItem.getValue("quantity").ToString()));
                            existedItem = true;
                            break;
                        }
                    }

                    if (!existedItem)
                    {
                        SplitDataFromServe._listItemInBag.Add(_boughtItem);
                    }
                }

                this.PostEvent(EventID.OnPropertiesChange);
                _numberItemBound.SetActive(false);
                _acceptItemBound.SetActive(true);
                if (qtt == 1)
                {
                    numberItemGet.gameObject.SetActive(false);
                }
                else if (qtt > 1)
                {
                    numberItemGet.gameObject.SetActive(true);
                    numberItemGet.text = qtt.ToString();
                }
            }
        }));
    }

    void AcceptFuction()
    {
        this.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter as GameObject;
        if (enterObj.name == this.name)
        {
            _border.DOScale(0, 0.35f).SetEase(Ease.InBack).OnComplete(CompleteHide);
        }
        else if (enterObj.name == "inputNumberItem")
        {
            //this.gameObject.SetActive(false);
            _border.DOScale(0, 0.35f).SetEase(Ease.InBack).OnComplete(CompleteHide);
        }
        else if (enterObj.name == "acceptItemBound")
        {
            //this.gameObject.SetActive(false);
            _border.DOScale(0, 0.35f).SetEase(Ease.InBack).OnComplete(CompleteHide);
        }
    }
    void CompleteHide()
    {
        this.gameObject.SetActive(false);

    }
}
