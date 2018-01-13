using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDismantle : MonoBehaviour
{
    private ItemInforController _itemInfor;

    [SerializeField] private UpgradeBag _myBag;
    [SerializeField] private EffectPatternUpgrade _effectPatternUpgrade;

    private EquipmentItem _mainEquip;

    private Item _itemDismantle1; //slot 1
    private int _numberItemDismantle;
    private int _numberGemDismantle;//slot 2

    [SerializeField]
    private Image _imgMainSlot;
    [SerializeField]
    private ItemUIScript _myItemUI;

    [SerializeField] private RewardDismantle _reward;

    private int _levelEquipmentID;
    private int _rareEquipmentID;

    private bool isRollingInforce;
    private float _timeRolling = 0;
    private bool isHaveResult;
    private string _resultUpgrade;
    void Start()
    {
        _itemInfor = ItemInforController.instance;
    }
    private void OnEnable()
    {
        _myBag.LoadEquipment(2);
        ResetMainEquipment();
        UpgradeBag.OnBtnClick += BtnItemInforClick;
    }
    private void OnDisable()
    {
        UpgradeBag.OnBtnClick -= BtnItemInforClick;
    }

    private void BtnItemInforClick()
    {
        string param = UpgradeBag.Message;
        if (ForgeUI.ID_UPGRADE != 3) return;
        if (param.Equals("SelectEquipment"))//Chọn đồ
        {
            SetMainEquipment();
        }
    }

    internal void ResetMainEquipment()
    {
        _mainEquip = null;
        _levelEquipmentID = -1;
        _rareEquipmentID = 0;
        _myItemUI.gameObject.SetActive(false);
        _imgMainSlot.color = new Color32(255, 255, 255, 255);

    }
    internal void SetMainEquipment()
    {
        _mainEquip = _myBag.GetEquipmentSelected();

        _myItemUI.gameObject.SetActive(true);
        _myItemUI.SetData(0, _mainEquip.idItemInit, _mainEquip.levelUpgraded, _mainEquip.rarelyItem,
                (_mainEquip.typeItem == TypeEquipmentCharacter.Buff ? 1 : (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
        _imgMainSlot.color = new Color32(255, 255, 255, 10);

        _levelEquipmentID = (_mainEquip.levelRequired - 1) / 10 + 1;
        _rareEquipmentID = _mainEquip.rarelyItem;

        //CaculateDismantle();
    }

    public void BtnClick(int id)
    {
        switch (id)
        {
            case 0://Btn MainEqquip
                if (_mainEquip != null)
                {
                    _itemInfor.SetData(_mainEquip, _myItemUI._iconImg.sprite);
                    _itemInfor.SetState(StateInforItem.None);
                }
                break;

            case 1: //Btn 1 slot 
                break;
        }
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

    public void BtnDismantleEquipment()
    {
        if (isRollingInforce) return;
        if (_mainEquip != null)
        {
            isRollingInforce = true;
            _timeRolling = _effectPatternUpgrade._timeRolling;
            isHaveResult = false;
            _resultUpgrade = string.Empty;
            _effectPatternUpgrade.StartRolling();

            StartCoroutine(ServerAdapter.UpgradeDismantleEquipment(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
                 _mainEquip.idItem, result =>
                 {
                     isHaveResult = true;
                     _resultUpgrade = result;
                 }));
        }
    }

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
            TextNotifyScript.instance.SetData(result);
            //Debug.Log(result);
        }
        else
        {
            Debug.Log(result);
            //{"gem":{"idh":"64", "idcode":"OBS29D5", "idig":"12", "quantity":"1", "level":"1", "timemili":"1513140477", "idhg":"138"}, "item":[  ], "diamond":[  ]}
            var N = JSON.Parse(result);
            bool isGem = false;
            if (N["gem"]["idig"] != null)
            {
                //có gem
                isGem = true;
                //update gem
                bool isExistThisGem = false;
                SplitDataFromServe._listGemInBag.ForEach(
                    Item =>
                    {
                        if (Item.getValue("idig").ToString() == N["gem"]["idig"].Value)
                        {
                            int oldQtt = int.Parse(Item.getValue("quantity").ToString());
                            Item.setValue("quantity", oldQtt + N["gem"]["quantity"].AsInt);
                            isExistThisGem = true;
                        }
                    }
                    );
                if (!isExistThisGem)
                {
                    SplitDataFromServe._listGemInBag.Add(new Item(N["gem"]["idhg"].AsInt, N["gem"]["idig"].AsInt, N["gem"]["quantity"].AsInt, N["gem"]["level"].AsInt, N["gem"]["sellprice"].AsInt, N["gem"]["uplevel"].AsInt));
                }
            }
            if (N["item"]["idit"] != null)
            {
                //có item
                if (isGem) Debug.Log("Sao co ca item va gem the ????");

                //update item
                bool isExistThisItem = false;
                SplitDataFromServe._listItemInBag.ForEach(
                    Item =>
                    {
                        //Debug.Log(Item.getValue("idit").ToString() + "&" + N["item"]["idit"]);
                        if (Item.getValue("idit").ToString() == N["item"]["idit"].Value)
                        {
                            Debug.Log("Tồn tại item");
                            int oldQtt = int.Parse(Item.getValue("quantity").ToString());
                            Item.setValue("quantity", oldQtt + N["item"]["quantity"].AsInt);
                            isExistThisItem = true;
                        }
                    }
                    );
                if (!isExistThisItem)
                {
                    SplitDataFromServe._listItemInBag.Add(new Item(N["item"]["idht"].AsInt, N["item"]["idit"].AsInt, string.Empty, N["item"]["quantity"].AsInt, N["item"]["sellprice"].AsInt, N["item"]["levelrequired"].AsInt));
                }
            }

            int numberDiamond = N["diamond"].AsInt;

            if (isGem)
                _reward.SetReward(isGem, N["gem"]["idig"].AsInt, N["gem"]["quantity"].AsInt, numberDiamond);
            else
                _reward.SetReward(isGem, N["item"]["idit"].AsInt, N["item"]["quantity"].AsInt, numberDiamond);

            //remove đồ
            SplitDataFromServe._listEquipmentInBag.Remove(_mainEquip);
            ResetMainEquipment();
            _myBag.LoadEquipment(2);
            //Add gem
            if (numberDiamond > 0)
            {
                CharacterInfo._instance._baseProperties.Diamond += numberDiamond;
                this.PostEvent(EventID.OnPropertiesChange);
            }
        }
        isRollingInforce = false;
        _effectPatternUpgrade.EndRolling();
    }

}
