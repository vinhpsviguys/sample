using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
public class ForgeUI : MonoBehaviour
{
    [SerializeField] private Text _txtGold;
    [SerializeField] private Transform _mainBag, _mainUpgrade;

    Action<object> _UpdateTextValueEventRef;

    [SerializeField] private GameObject _tabReinforce, _tabAddStone, _tabCombine, _tabDismantle;
    [SerializeField] private GameObject _objReinforce, _objAddStone, _objCombine, _objDismantle;

    public static int ID_UPGRADE = 0;

    private void OnEnable()
    {
        BtnClick(1);
        UpdateTextValue();
        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);


        _mainBag.localPosition = new Vector3(1500, _mainBag.localPosition.y);
        _mainBag.DOLocalMoveX(450, 0.5f).SetEase(Ease.OutBack);
        _mainUpgrade.localPosition = new Vector3(-1400, _mainUpgrade.localPosition.y);
        _mainUpgrade.DOLocalMoveX(-500, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
    }

    public void UpdateTextValue()
    {
        _txtGold.text = CharacterInfo._instance._baseProperties.Gold.ToString();
    }

    void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }

    public void BtnClick(int id)
    {
        switch (id)
        {
            case 0: //BtnBack
                _mainBag.DOLocalMoveX(1500, 0.3f).SetEase(Ease.InBack);
                _mainUpgrade.DOLocalMoveX(-1400, 0.3f).SetEase(Ease.InBack).OnComplete(CloseThisDialog);
                break;
            case 1://Btn Reinforce;
                ID_UPGRADE = 0;
                _tabReinforce.SetActive(true);
                _tabAddStone.SetActive(false);
                _tabCombine.SetActive(false);
                _tabDismantle.SetActive(false);

                _objReinforce.SetActive(true);
                _objAddStone.SetActive(false);
                _objCombine.SetActive(false);
                _objDismantle.SetActive(false);
                break;

            case 2://Btn AddStone;
                ID_UPGRADE = 1;
                _tabReinforce.SetActive(false);
                _tabAddStone.SetActive(true);
                _tabCombine.SetActive(false);
                _tabDismantle.SetActive(false);

                _objReinforce.SetActive(false);
                _objAddStone.SetActive(true);
                _objCombine.SetActive(false);
                _objDismantle.SetActive(false);
                break;
            case 3://Btn Combine;
                ID_UPGRADE = 2;
                _tabReinforce.SetActive(false);
                _tabAddStone.SetActive(false);
                _tabCombine.SetActive(true);
                _tabDismantle.SetActive(false);
                _objReinforce.SetActive(false);
                _objAddStone.SetActive(false);
                _objCombine.SetActive(true);
                _objDismantle.SetActive(false);
                break;
            case 4://Btn Dismantle;
                ID_UPGRADE = 3;
                _tabReinforce.SetActive(false);
                _tabAddStone.SetActive(false);
                _tabCombine.SetActive(false);
                _tabDismantle.SetActive(true);
                _objReinforce.SetActive(false);
                _objAddStone.SetActive(false);
                _objCombine.SetActive(false);
                _objDismantle.SetActive(true);
                break;
        }
    }
}
