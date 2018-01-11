using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerItemsInGame : MonoBehaviour
{
    public static ControllerItemsInGame _instance;
    private IDictionary<string, Sprite> _myIconsEquipment;
    private IDictionary<string, Sprite> _myIconsItem;
    private IDictionary<string, Sprite> _myIconsGems;
    private Sprite tempSprite;
    private string tempKey;
    private const string nameAssetBundleEquipment = "equipments";
    private const string nameAssetBundleItem = "items";
    private const string nameAssetBundleGems = "gems";
    public static bool isLoadedXML = false;
    internal Sprite[] _rareBorderItems;
    private bool isFoundImg = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.tag = "DontDestroyObject";
            _myIconsEquipment = new Dictionary<string, Sprite> { };
            _myIconsItem = new Dictionary<string, Sprite> { };
            _myIconsGems = new Dictionary<string, Sprite> { };

        }
        else DestroyImmediate(this.gameObject);
    }

    internal IEnumerator GetIconRareItem()
    {
        if (_rareBorderItems == null)
        {
            _rareBorderItems = new Sprite[7];
            for (int i = 0; i < 7; i++)
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleItem, "ItemUIRare" + (i + 1), value => _rareBorderItems[i] = value));
        }
    }

    internal IEnumerator GetIconForGems(Item _item, System.Action<Sprite> result)
    {
        if (_item == null) yield return null;
        else
        {
            isFoundImg = false;
            foreach (var icon in _myIconsGems)
            {
                if (icon.Key.Contains("Gems_" + _item.getValue("idig").ToString()))//nếu đã load rồi
                {
                    //Debug.Log("Đã load rồi");
                    if (icon.Value == null) Debug.LogError("Sao load rồi mà vẫn null");
                    result(icon.Value);
                    isFoundImg = true;
                    yield return null;
                }
            }
            if (!isFoundImg)
            {
                Sprite _sprite = null;
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleGems, _item.getValue("idig").ToString(), value => _sprite = value));
                if (_sprite == null) Debug.Log("Không tìm thấy");
                foreach (var icon in _myIconsGems)
                {
                    if (!icon.Key.Contains("Gems_" + _item.getValue("idig").ToString()))//nếu đã load rồi
                    {
                        _myIconsGems.Add("Gems_" + _item.getValue("idig").ToString(), _sprite);
                    }
                }

                result(_sprite);
                yield return null;
            }
        }
    }
    internal IEnumerator GetIconForGemsByID(int _idItem, System.Action<Sprite> result)
    {
        if (_idItem == 0) yield return null;
        else
        {
            isFoundImg = false;
            foreach (var icon in _myIconsGems)
            {
                if (icon.Key.Contains("Gems_" + _idItem.ToString()))//nếu đã load rồi
                {
                    //Debug.Log("Đã load rồi");
                    if (icon.Value == null) Debug.LogError("Sao load rồi mà vẫn null");
                    result(icon.Value);
                    isFoundImg = true;
                    yield return null;
                }
            }
            if (!isFoundImg)
            {

                //XmlNode _myItemXml = _dataEquipment.getDataByValue("id", _item.idItem.ToString(), "posi", _item.typeItem.ToString());
                //if (_myItemXml == null) yield return null;
                Sprite _sprite = null;
                //Debug.Log(_item.idItemInit.ToString());
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleGems, _idItem.ToString(), value => _sprite = value));
                if (_sprite == null) Debug.Log("Không tìm thấy");
                foreach (var icon in _myIconsGems)
                {
                    if (!icon.Key.Contains("Gems_" + _idItem.ToString()))//nếu đã load rồi
                    {
                        _myIconsGems.Add("Gems_" + _idItem.ToString(), _sprite);
                    }
                }

                result(_sprite);
                yield return null;
            }
        }
    }

    internal IEnumerator GetIconForItem(Item _item, System.Action<Sprite> result)
    {
        if (_item == null) yield return null;
        else
        {
            isFoundImg = false;
            foreach (var icon in _myIconsItem)
            {
                if (icon.Key.Contains("Item_" + _item.getValue("idit").ToString()))//nếu đã load rồi
                {
                    //Debug.Log("Đã load rồi");
                    if (icon.Value == null) Debug.LogError("Sao load rồi mà vẫn null");
                    result(icon.Value);
                    isFoundImg = true;
                    yield return null;
                }
            }
            if (!isFoundImg)
            {

                //XmlNode _myItemXml = _dataEquipment.getDataByValue("id", _item.idItem.ToString(), "posi", _item.typeItem.ToString());
                //if (_myItemXml == null) yield return null;
                Sprite _sprite = null;
                //Debug.Log(_item.idItemInit.ToString());
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleItem, _item.getValue("idit").ToString(), value => _sprite = value));
                if (_sprite == null) Debug.Log("Không tìm thấy");
                foreach (var icon in _myIconsItem)
                {
                    if (!icon.Key.Contains("Item_" + _item.getValue("idit").ToString()))//nếu đã load rồi
                    {
                        _myIconsItem.Add("Item_" + _item.getValue("idit").ToString(), _sprite);
                    }
                }

                result(_sprite);
                yield return null;
            }
        }
    }
    internal IEnumerator GetIconForItemByID(int _idItem, System.Action<Sprite> result)
    {
        if (_idItem == 0) yield return null;
        else
        {
            isFoundImg = false;
            foreach (var icon in _myIconsItem)
            {
                if (icon.Key.Contains("Item_" + _idItem.ToString()))//nếu đã load rồi
                {
                    //Debug.Log("Đã load rồi");
                    if (icon.Value == null) Debug.LogError("Sao load rồi mà vẫn null");
                    result(icon.Value);
                    isFoundImg = true;
                    yield return null;
                }
            }
            if (!isFoundImg)
            {

                //XmlNode _myItemXml = _dataEquipment.getDataByValue("id", _item.idItem.ToString(), "posi", _item.typeItem.ToString());
                //if (_myItemXml == null) yield return null;
                Sprite _sprite = null;
                //Debug.Log(_item.idItemInit.ToString());
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleItem, _idItem.ToString(), value => _sprite = value));
                if (_sprite == null) Debug.Log("Không tìm thấy");
                foreach (var icon in _myIconsItem)
                {
                    if (!icon.Key.Contains("Item_" + _idItem.ToString()))//nếu đã load rồi
                    {
                        _myIconsItem.Add("Item_" + _idItem.ToString(), _sprite);
                    }
                }

                result(_sprite);
                yield return null;
            }
        }
    }


    internal IEnumerator GetIconForEquipment(EquipmentItem _item, System.Action<Sprite> result)
    {
        if (_item == null || _item.idItem == 0 || _item.typeItem == TypeEquipmentCharacter.None)
        {
            Debug.Log("null object");
            yield return null;
        }
        string _nameItem = "Item_";
        if (_item.typeItem == TypeEquipmentCharacter.Avatar) _nameItem = "Avatar_";
        else if (_item.typeItem == TypeEquipmentCharacter.Buff) _nameItem = "Buff_";

        if (_item == null)
        {
            yield return null;
        }
        else
        {
            isFoundImg = false;
            foreach (var icon in _myIconsEquipment)
            {
                if (icon.Key.Contains(_nameItem + _item.idItemInit.ToString()))//nếu đã load rồi
                {
                    Debug.Log("Đã load rồi");
                    if (icon.Value == null) Debug.LogError("Sao load rồi mà vẫn null");
                    result(icon.Value);
                    isFoundImg = true;
                    Debug.Log("da tra ve");
                    yield return null;
                }
            }
            if (!isFoundImg)
            {
                Sprite _sprite = null;
                int idItemTemp = _item.idItemInit;
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleEquipment, _nameItem + idItemTemp.ToString(), value => _sprite = value));
                if (_sprite == null) Debug.Log("Không tìm thấy");
                foreach (var icon in _myIconsEquipment)
                {
                    if (!icon.Key.Contains(_nameItem + _item.idItemInit.ToString()))//nếu đã load rồi
                    {
                        _myIconsEquipment.Add(_nameItem + _item.idItemInit.ToString(), _sprite);
                    }
                }
                result(_sprite);
                yield return null;
            }
        }
    }
    internal IEnumerator GetIconForEquipmentByID(int _iditem, int idType, System.Action<Sprite> result)
    {
        if (_iditem == 0) yield return null;
        else
        {
            isFoundImg = false;

            string _nameItem = "Item_";
            if (idType == 1) _nameItem = "Buff_";
            else if (idType == 2) _nameItem = "Avatar_";

            foreach (var icon in _myIconsEquipment)
            {
                if (icon.Key.Contains(_nameItem + _iditem.ToString()))//nếu đã load rồi
                {
                    Debug.Log("Đã load rồi");
                    if (icon.Value == null) Debug.LogError("Sao load rồi mà vẫn null");
                    result(icon.Value);
                    isFoundImg = true;
                    yield return null;
                }
            }
            if (!isFoundImg)
            {
                Sprite _sprite = null;
                yield return StartCoroutine(LoadingResourceController._instance.LoadAssetBundleSpriteAsync(nameAssetBundleEquipment, _nameItem + _iditem, value => _sprite = value));
                if (_sprite == null) Debug.Log("Không tìm thấy");
                foreach (var icon in _myIconsEquipment)
                {
                    if (!icon.Key.Contains(_nameItem + _iditem.ToString()))//nếu đã load rồi
                    {
                        _myIconsEquipment.Add(_nameItem + _iditem.ToString(), _sprite);
                    }
                }

                result(_sprite);
                yield return null;
            }
        }
    }

    //private string FormatItemToKey(int _id, TypeEquipmentCharacter _type)
    //{   //AABB
    //    string Prefixes = MappingData.ConvertTypeItemToString(_type);
    //    string Suffixes = _id > 9 ? _id.ToString() : ("0" + _id.ToString());
    //    return string.Format("{0}{1}", Prefixes, Suffixes);
    //}
}

