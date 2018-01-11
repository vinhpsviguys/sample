using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{

    public static ShopUI _instance;

    public Button _goldShopBtn, _diamondShopBtn;
    public GameObject _goldShopPanel, _diamondShopPanel;

    public Text goldNumber, eventPointNumber, diamondNumber, warningText;

    public Transform _contentGoldShop, _contentDiamondShop, _pageToggleGold, _pageToggleDiamond;
    public GameObject _itemToggle, _listItemPagePrefabs, _itemSellByGoldPrefabs, _itemSellByDiamondPrefabs;

    public GameObject _infoItemPanel, _buyItemPanel, _warningPanel;


    Action<object> _UpdateTextValueEventRef;

    public int _cachedIDItemInfo, _cachedIDItemBuy;
    bool loadedItemInfo = false;
    GameObject _tempPageObject, _tempToggleObject;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        _instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }



    List<Item> SortItemByTypePrice(string _typePrice)
    {
        List<Item> _tempListItem = new List<Item>();
        foreach (Item _tempItem in SplitDataFromServe._listItemInShop)
        {
            if (_tempItem.getValue("type").ToString() == _typePrice)
            {
                _tempListItem.Add(_tempItem);
            }
        }

        return _tempListItem;
    }

    void DisplayItemInShop()
    {
        List<Item> _ListItemGold = SortItemByTypePrice("Gold");
        int numberPageGold = _ListItemGold.Count / 10 + 1;
        for (int i = 1; i <= numberPageGold; i++)
        {
            _tempPageObject = Instantiate(_listItemPagePrefabs);
            _tempPageObject.transform.SetParent(_contentGoldShop);
            _tempPageObject.transform.localScale = Vector3.one;

            _tempToggleObject = Instantiate(_itemToggle);
            _tempToggleObject.transform.SetParent(_pageToggleGold);
            _tempToggleObject.transform.localScale = Vector3.one;

            DisplayItemInPageType(i, _ListItemGold, _tempPageObject.transform, "Gold");
        }

        List<Item> _ListItemDiamond = SortItemByTypePrice("Gem");
        int numberPageDiamond = _ListItemDiamond.Count / 10 + 1;
        for (int i = 1; i <= numberPageDiamond; i++)
        {
            _tempPageObject = Instantiate(_listItemPagePrefabs);
            _tempPageObject.transform.SetParent(_contentDiamondShop);
            _tempPageObject.transform.localScale = Vector3.one;

            _tempToggleObject = Instantiate(_itemToggle);
            _tempToggleObject.transform.SetParent(_pageToggleDiamond);
            _tempToggleObject.transform.localScale = Vector3.one;

            DisplayItemInPageType(i, _ListItemDiamond, _tempPageObject.transform, "Gem");
        }
    }

    void DisplayItemInPageType(int indexPage, List<Item> _ListItem, Transform _pageContent, string _typePrice)
    {
        if (_ListItem.Count / 10 + 1 > indexPage)
        {
            for (int i = (indexPage - 1) * 10; i < (indexPage - 1) * 10 + 10; i++)
            {
                GameObject _tempItemObject = null;
                switch (_typePrice)
                {
                    case "Gold":
                        _tempItemObject = Instantiate(_itemSellByGoldPrefabs);
                        break;
                    case "Gem":
                        _tempItemObject = Instantiate(_itemSellByDiamondPrefabs);
                        break;
                }

                _tempItemObject.transform.SetParent(_pageContent);
                _tempItemObject.transform.localScale = Vector3.one;
                StartCoroutine(LoadItemImgAndPrice(_tempItemObject, _ListItem[i]));
            }
        }
        else if (_ListItem.Count / 10 + 1 == indexPage)
        {
            for (int i = (indexPage - 1) * 10; i < (indexPage - 1) * 10 + _ListItem.Count % 10; i++)
            {
                GameObject _tempItemObject = null;
                switch (_typePrice)
                {
                    case "Gold":
                        _tempItemObject = Instantiate(_itemSellByGoldPrefabs);
                        break;
                    case "Gem":
                        _tempItemObject = Instantiate(_itemSellByDiamondPrefabs);
                        break;
                }
                _tempItemObject.transform.SetParent(_pageContent);
                _tempItemObject.transform.localScale = Vector3.one;
                StartCoroutine(LoadItemImgAndPrice(_tempItemObject, _ListItem[i]));
            }
        }
    }
    IEnumerator LoadItemImgAndPrice(GameObject _itemObj, Item _item)
    {
        _itemObj.GetComponent<Button>().onClick.AddListener(() => OpenItemInfoPanel(int.Parse(_item.getValue("ids").ToString())));
        _itemObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OpenBuyItemPanel(int.Parse(_item.getValue("ids").ToString())));
        _itemObj.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = _item.getValue("price").ToString();
        if (int.Parse(_item.getValue("typeid").ToString()) == 1)
        {
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItemByID(int.Parse(_item.getValue("idtemp").ToString()), value => _itemObj.transform.GetChild(0).GetComponent<Image>().sprite = value));
        }
        else if (int.Parse(_item.getValue("typeid").ToString()) == 2)
        {
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(int.Parse(_item.getValue("idtemp").ToString()), value => _itemObj.transform.GetChild(0).GetComponent<Image>().sprite = value));
        }
    }

    public Item LoadItemFromId(int id)
    {
        foreach (Item _it in SplitDataFromServe._listItemInShop)
        {
            if (int.Parse(_it.getValue("ids").ToString()) == id)
            {
                //if (int.Parse(_it.getValue("typeid").ToString()) == 1)//item
                //{
                //    return new Item(int.Parse(_it.getValue("ids").ToString()), int.Parse(_it.getValue("idtemp").ToString()), _it.getValue("name").ToString(), 9999,
                //        int.Parse(_it.getValue("price").ToString()), int.Parse(_it.getValue("levelrequired").ToString()));
                //}
                //else//runestone
                //{
                //    return new Item(int.Parse(_it.getValue("ids").ToString()), int.Parse(_it.getValue("idtemp").ToString()), 9999, int.Parse(_it.getValue("levelrequired").ToString()),
                //        int.Parse(_it.getValue("price").ToString()));
                //}
                return _it;
            }
        }
        return null;
    }

    private void OnEnable()
    {
        _infoItemPanel.SetActive(false);
        _buyItemPanel.SetActive(false);
        _warningPanel.SetActive(false);
        if (loadedItemInfo == false)
        {
            DisplayItemInShop();
            loadedItemInfo = true;
        }
        UpdateTextValue();
        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        OpenGoldShopPanel();
        _goldShopBtn.onClick.AddListener(OpenGoldShopPanel);
        _diamondShopBtn.onClick.AddListener(OpenDiamondShopPanel);
    }

    public void UpdateTextValue()
    {
        goldNumber.text = CharacterInfo._instance._baseProperties.Gold.ToString();
        eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
        diamondNumber.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
    }


    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
        _goldShopBtn.onClick.RemoveListener(OpenGoldShopPanel);
        _diamondShopBtn.onClick.RemoveListener(OpenDiamondShopPanel);
    }

    void OpenGoldShopPanel()
    {
        _goldShopPanel.SetActive(true);
        _diamondShopPanel.SetActive(false);
    }
    void OpenFameShopPanel()
    {
        _goldShopPanel.SetActive(false);
        _diamondShopPanel.SetActive(false);
    }
    void OpenDiamondShopPanel()
    {
        _goldShopPanel.SetActive(false);
        _diamondShopPanel.SetActive(true);
    }

    void OpenItemInfoPanel(int _idItem)
    {
        _cachedIDItemInfo = _idItem;
        _infoItemPanel.SetActive(true);
    }
    void OpenBuyItemPanel(int _idItem)
    {
        _cachedIDItemBuy = _idItem;
        _buyItemPanel.SetActive(true);
    }

    public void OpenWarningPanel(string des)
    {
        _warningPanel.SetActive(true);
        warningText.text = des;
    }

    public void CloseThisDialog()
    {
        //this.gameObject.SetActive(false);
        //SceneManager.LoadScene("MainMenu");
        SceneLoader._instance.LoadScene(2);
    }
}
