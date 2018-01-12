using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCombine : MonoBehaviour
{
    private ItemInforController _itemInfor;
    [SerializeField] private UpgradeBag _myBag;
    [SerializeField] private EffectPatternUpgrade _effectPatternUpgrade;

    private Item _itemRunstone;
    private Item _itemRuneMaterial;

    [SerializeField]
    private Image _mainRunestoneImg, _materialRunestoneImg;
    [SerializeField] private Sprite _defaultRunestoneSprite;

    [SerializeField]
    private Text _txtNumberMaterialRunestone, _txtNumberGoldNeed;

    [SerializeField]
    private Image _imgFillMaterialRunstone;

    [SerializeField]
    private InputField _txtNumberCombine;

    private int _numberMaterialRunestoneHave, _numberMaterialRunstoneNeed, _numberCombine, _numberGoldNeeded, _numberMaxCanCombine;

    private int _levelMaterialRunstone, _levelMainRunestone, _idigRunestone;
    private void OnEnable()
    {
        UpgradeBag.OnBtnClick += BtnItemInforClick;
        _myBag.LoadRunestones(-1);
        ResetRunestoneSelected();
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
        if (ForgeUI.ID_UPGRADE != 2) return;
        if (param.Equals("SelectItem"))//Chọn đồ
        {
            SelectRunestoneCombine();
        }
    }
    internal void SelectRunestoneCombine()
    {
        _itemRuneMaterial = _myBag.GetRunestoneSelected();
        _levelMaterialRunstone = int.Parse(_itemRuneMaterial.getValue("level").ToString());
        _idigRunestone = int.Parse(_itemRuneMaterial.getValue("idig").ToString());
        if (_levelMaterialRunstone >= 10)
        {
            Debug.LogError("Runestone không thể hợp thành nữa");
            return;
        }
        _itemRunstone = new Item(_itemRuneMaterial);
        _levelMainRunestone = _levelMaterialRunstone + 1;
        _itemRunstone.setValue("level", _levelMainRunestone);

        _materialRunestoneImg.sprite = _myBag.GetIconRunestoneSelected();
        _mainRunestoneImg.sprite = _myBag.GetIconRunestoneSelected();
        _numberMaterialRunestoneHave = int.Parse(_itemRuneMaterial.getValue("quantity").ToString());
        _numberMaxCanCombine = _numberMaterialRunestoneHave / 3;
        _numberCombine = _numberMaxCanCombine;
        CalculateMaterial();
    }
    internal void ResetRunestoneSelected()
    {
        _itemRunstone = null;
        _itemRuneMaterial = null;
        _txtNumberCombine.text = string.Empty;
        _txtNumberGoldNeed.text = string.Empty;
        _txtNumberMaterialRunestone.text = string.Empty;
        _mainRunestoneImg.sprite = _defaultRunestoneSprite;
        _materialRunestoneImg.sprite = _defaultRunestoneSprite;
        _imgFillMaterialRunstone.fillAmount = 0;
        _numberCombine = 0;
        _numberMaterialRunestoneHave = 0;
        _numberMaterialRunstoneNeed = 0;
        _numberGoldNeeded = 0;
        _numberMaxCanCombine = 0;
        _levelMaterialRunstone = 0;
        _levelMainRunestone = 0;
        _idigRunestone = 0;
    }

    IEnumerator LoadRunestone(Item _item, Image _target)
    {
        yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGems(_item, value => { _target.sprite = value; _target.SetNativeSize(); }));
    }

    public void BtnRunestone()
    {
        if (isRollingInforce) return;

        _itemInfor._myState = StateInforItem.None;
        _itemInfor.SetData(_itemRunstone, _mainRunestoneImg.sprite);
        _itemInfor.gameObject.SetActive(true);
    }

    public void BtnChangeNumberCombine(bool isAdd)
    {
        if (isRollingInforce) return;
        if (isAdd)
        {
            if (_numberCombine < _numberMaxCanCombine)
            {
                _numberCombine += 1;
                CalculateMaterial();
            }
        }
        else
        {
            if (_numberCombine > 0)
            {
                _numberCombine -= 1;
                CalculateMaterial();
            }
        }
    }
    public void OnEndEditNumber()
    {
        if (_itemRuneMaterial == null) return;
        if (_txtNumberCombine.text.Length == 0)
        {
            _numberCombine = _numberMaxCanCombine;
        }
        else
        {
            int _value = int.Parse(_txtNumberCombine.text);
            if (_value <= 0 || _value >= _numberMaxCanCombine) _numberCombine = _numberMaxCanCombine;
            else _numberCombine = _value;
        }
        CalculateMaterial();
    }

    public void ShowMaterialRunstone()
    {
        if (_itemRuneMaterial != null)
        {
            _itemInfor.SetData(_itemRuneMaterial, _materialRunestoneImg.sprite);
            _itemInfor.SetState(StateInforItem.ShowInForItem);
        }
    }
    public void ShowMainRunstone()
    {
        if (_itemRunstone != null)
        {
            _itemInfor.SetData(_itemRunstone, _mainRunestoneImg.sprite);
            _itemInfor.SetState(StateInforItem.ShowInForItem);
        }
    }

    private void CalculateMaterial()
    {
        _txtNumberCombine.text = _numberCombine.ToString();
        _numberMaterialRunstoneNeed = _numberCombine * 3;
        _txtNumberMaterialRunestone.text = string.Format("{0}/{1}", _numberMaterialRunstoneNeed, _numberMaterialRunestoneHave);

        if (_numberMaterialRunestoneHave <= 0) _imgFillMaterialRunstone.fillAmount = 0;
        else _imgFillMaterialRunstone.fillAmount = 1.0f * _numberMaterialRunstoneNeed / _numberMaterialRunestoneHave;
        _numberGoldNeeded = Constant.UPGRADE_COMBINE_PRICE[_levelMaterialRunstone] * _numberCombine;
        _txtNumberGoldNeed.text = _numberGoldNeeded.ToString();
    }

    public void BtnUpgradeCombine()
    {
        if (isRollingInforce) return;
        if (_numberCombine > 0)
        {
            if (CharacterInfo._instance._baseProperties.Gold < _numberGoldNeeded)
            {
                Debug.LogError("Không có vàng mà đòi đú");
            }
            else
            {
                isRollingInforce = true;
                _timeRolling = _effectPatternUpgrade._timeRolling;
                isHaveResult = false;
                _resultUpgrade = string.Empty;
                _effectPatternUpgrade.StartRolling();
                StartCoroutine(ServerAdapter.UpgradeCombineRunestone(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero,
                   int.Parse(_itemRuneMaterial.getValue("idhg").ToString()), _numberMaterialRunstoneNeed, _numberGoldNeeded,
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
            //string _resultUpgrade = N["result"].Value;
            //Debug.Log(N);

            CharacterInfo._instance._baseProperties.Gold -= _numberGoldNeeded;
            this.PostEvent(EventID.OnPropertiesChange);


            //change material
            _numberMaterialRunestoneHave -= _numberMaterialRunstoneNeed;
            if (_numberMaterialRunestoneHave <= 0)
            {
                SplitDataFromServe._listGemInBag.Remove(_itemRuneMaterial);
            }
            else _itemRuneMaterial.setValue("quantity", _numberMaterialRunestoneHave);

            //change main
            bool isExistRunestone = false;
            int _countGems = SplitDataFromServe._listGemInBag.Count;
            for (int i = 0; i < _countGems; i++)
            {
                int _idig = int.Parse(SplitDataFromServe._listGemInBag[i].getValue("idig").ToString());
                if (_idig == _idigRunestone)
                {
                    int _level = int.Parse(SplitDataFromServe._listGemInBag[i].getValue("level").ToString());
                    if (_level == _levelMainRunestone)
                    {
                        isExistRunestone = true;
                        int _quantity = int.Parse(SplitDataFromServe._listGemInBag[i].getValue("quantity").ToString());
                        SplitDataFromServe._listGemInBag[i].setValue("quantity", _quantity + _numberCombine);
                        break;
                    }
                }
            }
            if (!isExistRunestone)
            {
                SplitDataFromServe._listGemInBag.Add(new Item(N["idhg"].AsInt, _idigRunestone, _numberCombine, _levelMainRunestone, N["sellprice"].AsInt, N["uplevel"].AsInt));
            }

            _myBag.LoadRunestones(-1);//reaload bag
            ResetRunestoneSelected();
        }
        isRollingInforce = false;
        _effectPatternUpgrade.EndRolling();
    }
}
