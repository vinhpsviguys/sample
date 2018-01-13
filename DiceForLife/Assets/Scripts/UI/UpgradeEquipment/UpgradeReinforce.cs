using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeReinforce : MonoBehaviour
{
    [SerializeField] private UpgradeBag _myBag;
    [SerializeField] private EffectPatternUpgrade _effectPatternUpgrade;

    private EquipmentItem _mainEquip, _newEquipAfterUpgrade;
    private ItemInforController _itemInfor;

    private Item _itemAlchemyMaterial;
    private Item _LuckyMaterial;
    private int _number_lucky_event = 0;
    private int _number_lucky_material = 0;
    private int _total_lucky_Reinforce = 0;

    [SerializeField] private Text _txtLuckyRate;
    [SerializeField] private Image _imgMainSlot;
    [SerializeField] private ItemUIScript _myItemUI;

    [SerializeField] private Image _alchemyMaterialImg;
    [SerializeField] private Text _txtNumberAlchemyMaterial;

    [SerializeField] private Image _luckyMaterialImg;
    [SerializeField] private Sprite _defaultluckyMaterialSprite;

    [SerializeField] private Image _eventLuckyImg;
    [SerializeField] private Sprite _lockEventSprite;

    private int _numberAlchemyMaterial;
    private bool isNeedRequest = false;
    private int _groupLevelEquipment = -1;
    private bool isSpecialUpgrade;
    private float _tempValueIndex;

    void Start()
    {
        isNeedRequest = true;
        _itemInfor = ItemInforController.instance;
    }

    private void OnEnable()
    {
        if (isNeedRequest) _myBag.LoadEquipment(0);
        ResetMainEquipment();
        ResetMatertialLucky();

        _alchemyMaterialImg.sprite = _defaultluckyMaterialSprite;
        _txtNumberAlchemyMaterial.text = string.Empty;

        _number_lucky_event = SplitDataFromServe._eventInGame.event_lucky;
        if (_number_lucky_event == 0) _eventLuckyImg.sprite = _lockEventSprite;
        else _eventLuckyImg.sprite = null;

        _txtLuckyRate.text = string.Empty;
        _groupLevelEquipment = -1;
        UpgradeBag.OnBtnClick += BtnItemInforClick;
    }
    private void OnDisable()
    {
        UpgradeBag.OnBtnClick -= BtnItemInforClick;
    }

    private void BtnItemInforClick()
    {
        string param = UpgradeBag.Message;

        if (ForgeUI.ID_UPGRADE != 0) return;
        if (param.Equals("SelectEquipment"))//Chọn đồ
        {
            SetMainEquipment();
            ResetMatertialLucky();
            CaculateIndexAfterReinforce();
            CaculateLuckyRate();
        }
        else if (param.Equals("SelectItem"))//Chọn lucky material
        {
            SetMatertialLucky(_myBag.GetLuckyMaterialSelected());
            CaculateLuckyRate();
        }
        else if (param.Equals("DeSelectItem"))//Chọn lucky material
        {
            ResetMatertialLucky();
            CaculateLuckyRate();
        }
    }

    void CaculateIndexAfterReinforce()
    {
        if (isSpecialUpgrade)
        {
            if (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar)
            {
                int _newLevel = _mainEquip.levelUpgraded + 1;
                _newEquipAfterUpgrade.setValue("39", _newLevel * 0.02f);
                _newEquipAfterUpgrade.setValue("98", _newLevel * 0.01f);
                _newEquipAfterUpgrade.setValue("99", _newLevel * 0.01f);
                _newEquipAfterUpgrade.setValue("100", _newLevel * 0.005f);
                _newEquipAfterUpgrade.setValue("101", _newLevel * 0.005f);
                _newEquipAfterUpgrade.setValue("999", (_newLevel - 1) / 2 + 1);
            }
            else
            {

                int _idAttribute = MappingData.ConvertIdBuffToAttribute(_mainEquip.idItemInit);
                _tempValueIndex = float.Parse(_mainEquip.getValue(_idAttribute.ToString()).ToString());
                _newEquipAfterUpgrade.setValue(_idAttribute.ToString(), _tempValueIndex + (_mainEquip.idItemInit <= 8 ? 0.05f : 0.1f));
            }
        }
        for (int i = 1; i <= 28; i++)
        {
            _tempValueIndex = float.Parse(_mainEquip.getValue(i.ToString()).ToString());
            if (_tempValueIndex != 0)
            {
                _newEquipAfterUpgrade.setValue(i.ToString(), _tempValueIndex * 1.1f);
            }
        }
        _newEquipAfterUpgrade.levelUpgraded += 1;
    }

    void CaculateLuckyRate()
    {
        if (_mainEquip == null)
        {
            _total_lucky_Reinforce = 0;
            _txtLuckyRate.text = string.Empty;
        }
        else
        {
            int _normalLucky = 0;
            if (isSpecialUpgrade)
            {
                if (_mainEquip.typeItem == TypeEquipmentCharacter.Buff)
                    _normalLucky = Constant.UPGRADE_REINFORCE_SUCCESSFULRATE_BOOK[_mainEquip.levelUpgraded];
                else _normalLucky = Constant.UPGRADE_REINFORCE_SUCCESSFULRATE_AVATAR[_mainEquip.levelUpgraded];
            }
            else _normalLucky = Constant.UPGRADE_REINFORCE_SUCCESSFULRATE[_mainEquip.levelUpgraded];

            _total_lucky_Reinforce = _normalLucky + _number_lucky_material + _number_lucky_event;
            if (_total_lucky_Reinforce > 100) _total_lucky_Reinforce = 100;
            _txtLuckyRate.text = string.Format("Successful rate: {0}%", _total_lucky_Reinforce);
        }
    }

    public void ShowInforUpgrade()
    {
        if (isRollingInforce) return;
        if (_mainEquip == null) return;
        _myBag.LoadEquipment(0);

        _itemInfor.SetData(_mainEquip, _myItemUI._iconImg.sprite);
        _itemInfor.SetDataNewEquipment(_newEquipAfterUpgrade, _myItemUI._iconImg.sprite);
        _itemInfor.SetState(StateInforItem.CompareEquipment);
    }
    public void HideInforUpgrade()
    {
        if (isRollingInforce) return;
        if (_mainEquip == null) return;
        _itemInfor.gameObject.SetActive(false);
    }

    internal void ResetMainEquipment()
    {
        _mainEquip = null;

        _myItemUI.gameObject.SetActive(false);
        _imgMainSlot.color = new Color32(255, 255, 255, 255);

        CaculateLuckyRate();
    }
    internal void SetMainEquipment()
    {
        _mainEquip = _myBag.GetEquipmentSelected();

        _myItemUI.gameObject.SetActive(true);
        _myItemUI.SetData(0, _mainEquip.idItemInit, _mainEquip.levelUpgraded, _mainEquip.rarelyItem,
                (_mainEquip.typeItem == TypeEquipmentCharacter.Buff ? 1 : (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
        _imgMainSlot.color = new Color32(255, 255, 255, 10);

        _itemAlchemyMaterial = null;
        _numberAlchemyMaterial = 0;
        if (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar || _mainEquip.typeItem == TypeEquipmentCharacter.Buff)
        {
            isSpecialUpgrade = true;

            for (int i = 0; i < SplitDataFromServe._listGemInBag.Count; i++)
            {
                if (SplitDataFromServe._listGemInBag[i].GetTypeItem() == ITEMTYPE.RUNESTONE_SPECIALALCHEMY)
                {
                    _itemAlchemyMaterial = SplitDataFromServe._listGemInBag[i];
                    _numberAlchemyMaterial = int.Parse(_itemAlchemyMaterial.getValue("quantity").ToString());
                }
            }
            _txtNumberAlchemyMaterial.text = string.Format("{0}/1", _numberAlchemyMaterial);
            StartCoroutine(LoadIconRunestone(25, _alchemyMaterialImg));
            if (_groupLevelEquipment != 0) ResetMatertialLucky();
            _groupLevelEquipment = 0;
        }
        else
        {
            isSpecialUpgrade = false;
            for (int i = 0; i < SplitDataFromServe._listGemInBag.Count; i++)
            {
                if (SplitDataFromServe._listGemInBag[i].GetTypeItem() == ITEMTYPE.RUNESTONE_MASTERIALALCHEMY)
                {
                    _itemAlchemyMaterial = SplitDataFromServe._listGemInBag[i];
                    _numberAlchemyMaterial = int.Parse(_itemAlchemyMaterial.getValue("quantity").ToString());
                }
            }
            _txtNumberAlchemyMaterial.text = string.Format("{0}/1", _numberAlchemyMaterial);
            StartCoroutine(LoadIconRunestone(23, _alchemyMaterialImg));
            int newGroup = (_mainEquip.levelRequired - 1) / 10 + 1;
            if (newGroup != _groupLevelEquipment) ResetMatertialLucky();
            _groupLevelEquipment = newGroup;
        }
        _newEquipAfterUpgrade = new EquipmentItem(_mainEquip);
    }

    internal void ResetMatertialLucky()
    {
        _LuckyMaterial = null;
        _number_lucky_material = 0;
        _luckyMaterialImg.sprite = _defaultluckyMaterialSprite;
        _luckyMaterialImg.SetNativeSize();
        _myBag.DeSelectMaterial();
        CaculateLuckyRate();
    }
    internal void SetMatertialLucky(Item _lucky_Material)
    {
        _LuckyMaterial = _lucky_Material;
        _luckyMaterialImg.sprite = _myBag.GetIconLuckyMaterialSelected();
        if (_LuckyMaterial.GetTypeItem() == ITEMTYPE.LUCKY_MATERIAL_REINFORCEMENT)
            _number_lucky_material = 5;
        else if (_LuckyMaterial.GetTypeItem() == ITEMTYPE.SPECIAL_LUCKY_MATERIAL_REINFORCEMENT)
            _number_lucky_material = 10;
        CaculateLuckyRate();
    }

    IEnumerator LoadIconEquipment(EquipmentItem _item, Image _target)
    {
        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForEquipment(_item, value => { _target.sprite = value; _target.SetNativeSize(); }));
    }
    IEnumerator LoadIconItem(Item _item, Image _target)
    {
        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItem(_item, value => { _target.sprite = value; _target.SetNativeSize(); }));
    }
    IEnumerator LoadIconRunestone(int id, Image _target)
    {
        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(id, value => { _target.sprite = value; _target.SetNativeSize(); }));
    }

    public void BtnLuckyMaterial()
    {
        if (isRollingInforce) return;
        if (_mainEquip == null) return;
        if (_LuckyMaterial == null)
        {
            _myBag.LoadLuckyMaterial(_groupLevelEquipment);
        }
        else
        {
            _myBag.LoadLuckyMaterial(_groupLevelEquipment);
            _itemInfor._myState = StateInforItem.DeselectItem;
            _itemInfor.SetData(_LuckyMaterial, _luckyMaterialImg.sprite);
            _itemInfor.gameObject.SetActive(true);
        }
    }
    public void BtnReinforceEquipment()
    {
        if (isRollingInforce) return;
        if (_mainEquip == null)
        {
            //TextNotifyScript.instance.SetData("Please insert the Equipment into reinforcement window!");
            return;
        }
        if (_numberAlchemyMaterial <= 0)
        {
            TextNotifyScript.instance.SetData("Not enough material to upgrade!");
            return;
        }
        isRollingInforce = true;
        _timeRolling = _effectPatternUpgrade._timeRolling;
        isHaveResult = false;
        _resultUpgrade = string.Empty;
        _effectPatternUpgrade.StartRolling();

        if (isSpecialUpgrade)
        {
            if (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar)
            {
                StartCoroutine(ServerAdapter.UpgradeReinforceAvatar(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
            _mainEquip.idItem, int.Parse(_itemAlchemyMaterial.getValue("idhg").ToString()),
            (_LuckyMaterial == null ? 0 : int.Parse(_LuckyMaterial.getValue("idht").ToString())),
            result =>
            {
                isHaveResult = true;
                _resultUpgrade = result;
            }));
            }
            else
            {
                StartCoroutine(ServerAdapter.UpgradeReinforceBook(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
        _mainEquip.idItem, int.Parse(_itemAlchemyMaterial.getValue("idhg").ToString()),
        (_LuckyMaterial == null ? 0 : int.Parse(_LuckyMaterial.getValue("idht").ToString())),
        result =>
        {
            isHaveResult = true;
            _resultUpgrade = result;
        }));
            }
        }
        else StartCoroutine(ServerAdapter.UpgradeReinforceEquipment(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
            _mainEquip.idItem, int.Parse(_itemAlchemyMaterial.getValue("idhg").ToString()),
            (_LuckyMaterial == null ? 0 : int.Parse(_LuckyMaterial.getValue("idht").ToString())),
            result =>
         {
             isHaveResult = true;
             _resultUpgrade = result;
         }));
    }
    private bool isRollingInforce;
    private float _timeRolling = 0;
    private bool isHaveResult;
    private string _resultUpgrade;

    private void LateUpdate()
    {
        if (isRollingInforce)
        {
            _timeRolling -= Time.deltaTime;
            if (_timeRolling < 0 && isHaveResult)
            {
                ShowResultUpgrade(_resultUpgrade);
            }
        }
    }

    private void ShowResultUpgrade(string result)
    {
        if (result.StartsWith("Error"))
        {
            Debug.Log(result); TextNotifyScript.instance.SetData(result);
        }
        else
        {
            var N = JSON.Parse(result);
            //if (N["result"].Value.Equals("success"))
            {
                _effectPatternUpgrade.ShowEffectUpgrade(N["result"].Value.Equals("success"));
            }
            if (isSpecialUpgrade)
            {
                if (_mainEquip.typeItem == TypeEquipmentCharacter.Buff) ReadNewBuff(_mainEquip, N);
                else ReadNewAvatar(_mainEquip, N);
            }
            else ReadNewAttributes(_mainEquip, N);

            _numberAlchemyMaterial -= 1;
            if (_numberAlchemyMaterial <= 0)
                SplitDataFromServe._listGemInBag.Remove(_itemAlchemyMaterial);
            else _itemAlchemyMaterial.setValue("quantity", _numberAlchemyMaterial);
            _txtNumberAlchemyMaterial.text = string.Format("{0}/1", _numberAlchemyMaterial);

            if (_LuckyMaterial != null)
            {
                int _numberLuckyMaterial = int.Parse(_LuckyMaterial.getValue("quantity").ToString());
                _numberLuckyMaterial -= 1;
                if (_numberLuckyMaterial <= 0)
                {
                    SplitDataFromServe._listItemInBag.Remove(_LuckyMaterial);
                    ResetMatertialLucky();
                }
                else _LuckyMaterial.setValue("quantity", _numberLuckyMaterial);

            }
            _myBag.UpdateEquipmentUpgraded(_groupLevelEquipment);

            //reselectItem
            SetMainEquipment();
            ResetMatertialLucky();
            CaculateIndexAfterReinforce();
            CaculateLuckyRate();
        }
        isRollingInforce = false;
        _effectPatternUpgrade.EndRolling();
    }
    private void ReadNewAttributes(EquipmentItem item, JSONNode N)
    {
        item.levelUpgraded = N["levelupgraded"].AsInt;
        item.setValue("1", float.Parse(N["mindamage"].Value.ToString()));
        item.setValue("2", float.Parse(N["maxdamage"].Value.ToString()));
        item.setValue("3", float.Parse(N["minmagic"].Value.ToString()));
        item.setValue("4", float.Parse(N["maxmagic"].Value.ToString()));
        item.setValue("5", float.Parse(N["critmax"].Value.ToString()));
        item.setValue("6", float.Parse(N["multicastmax"].Value.ToString()));
        item.setValue("7", float.Parse(N["min_incrdamage"].Value.ToString()));
        item.setValue("8", float.Parse(N["max_incrdamage"].Value.ToString()));
        item.setValue("9", float.Parse(N["min_incrmagic"].Value.ToString()));
        item.setValue("10", float.Parse(N["max_incrmagic"].Value.ToString()));
        item.setValue("11", float.Parse(N["min_rate"].Value.ToString()));
        item.setValue("12", float.Parse(N["max_rate"].Value.ToString()));
        item.setValue("13", float.Parse(N["minparryrate"].Value.ToString()));
        item.setValue("14", float.Parse(N["maxparryrate"].Value.ToString()));
        item.setValue("15", float.Parse(N["min_block"].Value.ToString()));
        item.setValue("16", float.Parse(N["max_block"].Value.ToString()));
        item.setValue("17", float.Parse(N["minphydef"].Value.ToString()));
        item.setValue("18", float.Parse(N["maxphydef"].Value.ToString()));
        item.setValue("19", float.Parse(N["minmagicdef"].Value.ToString()));
        item.setValue("20", float.Parse(N["maxmagicdef"].Value.ToString()));
        item.setValue("21", float.Parse(N["minphyreduction"].Value.ToString()));
        item.setValue("22", float.Parse(N["maxphyreduction"].Value.ToString()));
        item.setValue("23", float.Parse(N["minmagicreduction"].Value.ToString()));
        item.setValue("24", float.Parse(N["maxmagicreduction"].Value.ToString()));
        item.setValue("25", float.Parse(N["minphyabsorb"].Value.ToString()));
        item.setValue("26", float.Parse(N["maxphyabsorb"].Value.ToString()));
        item.setValue("27", float.Parse(N["minmagicabsorb"].Value.ToString()));
        item.setValue("28", float.Parse(N["maxmagicabsorb"].Value.ToString()));
        item.setValue("listidproperty", N["listidproperty"].ToString());
    }

    private void ReadNewBuff(EquipmentItem item, JSONNode N)
    {
        item.levelUpgraded = N["level"].AsInt;

        int _idRate = MappingData.ConvertIdBuffToAttribute(item.idItemInit);
        item.setValue(_idRate.ToString(), N["rate"].AsFloat);
    }
    private void ReadNewAvatar(EquipmentItem item, JSONNode N)
    {
        item.levelUpgraded = N["level"].AsInt;
        //Thêm các dòng trắng
        item.setValue("97", N["hp"].AsFloat);
        item.setValue("98", N["physical_damage"].AsFloat);
        item.setValue("99", N["magical_damage"].AsFloat);
        item.setValue("100", N["physical_absorption"].AsFloat);
        item.setValue("101", N["magical_absorption"].AsFloat);
        item.setValue("999", N["maxlist"].AsInt);
    }
}
