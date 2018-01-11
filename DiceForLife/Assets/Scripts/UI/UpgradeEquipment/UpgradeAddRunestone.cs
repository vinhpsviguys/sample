using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeAddRunestone : MonoBehaviour
{
    [SerializeField] private UpgradeBag _myBag;
    [SerializeField] private EffectPatternUpgrade _effectPatternUpgrade;

    private EquipmentItem _mainEquip, _newEquipAfterUpgrade;

    private Item _itemRunstone;
    private ItemInforController _itemInfor;

    [SerializeField]
    private Image _imgMainSlot;
    [SerializeField]
    private ItemUIScript _myItemUI;

    [SerializeField]
    private Image _slotRunestoneImg;
    [SerializeField] private Sprite _defaultRunestoneSprite;

    private int _groupRunestones = 0;
    private int _levelEquipment;

    private void OnEnable()
    {
        _myBag.LoadEquipment(1);
        ResetMainEquipment();
        ResetRuneStone();
        UpgradeBag.OnBtnClick += BtnItemInforClick;
    }
    void Start()
    {
        _itemInfor = ItemInforController.instance;
    }
    private void OnDisable()
    {
        UpgradeBag.OnBtnClick -= BtnItemInforClick;
    }

    private void BtnItemInforClick()
    {
        string param = UpgradeBag.Message;
        if (ForgeUI.ID_UPGRADE != 1) return;
        if (param.Equals("SelectEquipment"))//Chọn đồ
        {
            SetMainEquipment();
            ResetRuneStone();
        }
        else if (param.Equals("SelectItem"))//Chọn lucky material
        {
            SetRunestone(_myBag.GetRunestoneSelected());
        }
        else if (param.Equals("DeSelectItem"))//Chọn lucky material
        {
            ResetRuneStone();
        }
    }

    float _tempValueIndex;
    void CaculateIndexAfterReinforce()
    {
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

    internal void ResetMainEquipment()
    {
        _mainEquip = null;
        _myItemUI.gameObject.SetActive(false);
        _imgMainSlot.color = new Color32(255, 255, 255, 255);
    }
    internal void SetMainEquipment()
    {
        _mainEquip = _myBag.GetEquipmentSelected();
        _levelEquipment = (_mainEquip.levelRequired - 1) / 10 + 1;
        _myItemUI.gameObject.SetActive(true);
        _myItemUI.SetData(0, _mainEquip.idItemInit, _mainEquip.levelUpgraded, _mainEquip.rarelyItem,
            (_mainEquip.typeItem == TypeEquipmentCharacter.Buff ? 1 : (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar ? 2 : 0)));
        _imgMainSlot.color = new Color32(255, 255, 255, 10);
        CalculateGroupRunestone();
        _newEquipAfterUpgrade = new EquipmentItem(_mainEquip);
    }
    private void CalculateGroupRunestone()
    {
        switch (_mainEquip.typeItem)
        {
            case TypeEquipmentCharacter.Weapon:
            case TypeEquipmentCharacter.OffhandWeapon:

                float _damePhysic = float.Parse(_mainEquip.getValue("1").ToString());
                if (_damePhysic > 0) // physic weapon
                {
                    _groupRunestones = 1;
                }//magic weapon
                else _groupRunestones = 2;
                break;
            case TypeEquipmentCharacter.Shield:
                _groupRunestones = 3;
                break;
            case TypeEquipmentCharacter.Head:
            case TypeEquipmentCharacter.Torso:
            case TypeEquipmentCharacter.Leg:
                _groupRunestones = 4;
                break;

            case TypeEquipmentCharacter.Belt:
            case TypeEquipmentCharacter.Gloves:
            case TypeEquipmentCharacter.Boots:
                _groupRunestones = 5;
                break;

            case TypeEquipmentCharacter.Ring:
            case TypeEquipmentCharacter.Amulet:
                _groupRunestones = 6;
                break;
            case TypeEquipmentCharacter.Avatar:
                _groupRunestones = 7;
                break;
        }
    }
    internal void ResetRuneStone()
    {
        _itemRunstone = null;
        _slotRunestoneImg.sprite = _defaultRunestoneSprite;
        _slotRunestoneImg.SetNativeSize();
        _myBag.DeSelectRunestone();
    }
    internal void SetRunestone(Item _runestone)
    {
        _itemRunstone = _runestone;
        _slotRunestoneImg.sprite = _myBag.GetIconRunestoneSelected();
        _slotRunestoneImg.SetNativeSize();
    }

    IEnumerator LoadIconEquipment(EquipmentItem _item, Image _target)
    {
        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForEquipment(_item, value => { _target.sprite = value; _target.SetNativeSize(); }));
    }
    IEnumerator LoadRunestone(Item _item, Image _target)
    {
        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGems(_item, value => { _target.sprite = value; _target.SetNativeSize(); }));
    }

    public void BtnRunestone()
    {
        if (isRollingInforce) return;
        if (_mainEquip == null) return;
        if (_itemRunstone != null)
        {
            _itemInfor._myState = StateInforItem.DeselectItem;
            _itemInfor.SetData(_itemRunstone, _slotRunestoneImg.sprite);
            _itemInfor.gameObject.SetActive(true);
        }
        _myBag.LoadRunestones(_groupRunestones, _levelEquipment);
    }
    public void BtnMainEquipment()
    {
        if (isRollingInforce) return;
        _myBag.LoadEquipment(1);
    }
    public void BtnAddingRunestone()
    {
        if (isRollingInforce) return;
        if (_mainEquip != null && _itemRunstone != null)
        {
            isRollingInforce = true;
            _timeRolling = _effectPatternUpgrade._timeRolling; ;
            isHaveResult = false;
            _resultUpgrade = string.Empty;
            _effectPatternUpgrade.StartRolling();
            if (_mainEquip.typeItem == TypeEquipmentCharacter.Avatar)
            {
                StartCoroutine(ServerAdapter.UpgradeAddingRunestoneAvatar(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
                _mainEquip.idItem, int.Parse(_itemRunstone.getValue("idhg").ToString()),
                result =>
                {
                    isHaveResult = true;
                    _resultUpgrade = result;
                }));
            }
            else
            {
                StartCoroutine(ServerAdapter.UpgradeAddingRunestone(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
                _mainEquip.idItem, int.Parse(_itemRunstone.getValue("idhg").ToString()),
                result =>
                {
                    isHaveResult = true;
                    _resultUpgrade = result;
                }));
            }
        }
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
            Debug.Log(result);
        }
        else
        {
            var N = SimpleJSON.JSON.Parse(result);
            string _resultUpgrade = N["result"].Value;
            int _numberRunestone = int.Parse(_itemRunstone.getValue("quantity").ToString());
            _numberRunestone -= 1;
            if (_numberRunestone <= 0)
            {
                SplitDataFromServe._listGemInBag.Remove(_itemRunstone);
            }
            else _itemRunstone.setValue("quantity", _numberRunestone);

            _mainEquip.setValue("listidproperty", N["properties"].ToString());

            _itemInfor._myState = StateInforItem.None;
            _itemInfor.SetData(_mainEquip, _myItemUI._iconImg.sprite);
            _itemInfor.gameObject.SetActive(true);

            _myBag.UpdateEquipmentUpgraded();

            //reset MainEquip
            SetMainEquipment();
            ResetRuneStone();
        }
        isRollingInforce = false;
        _effectPatternUpgrade.EndRolling();
    }
}
