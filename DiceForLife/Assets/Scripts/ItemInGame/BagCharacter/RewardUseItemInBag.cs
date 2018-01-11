using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RewardUseItemInBag : MonoBehaviour
{
    [SerializeField] private Transform _myBorder;
    [SerializeField] private Text _txt1, _txt2;
    [SerializeField] private Button _btnOK;


    internal void SetData(string _text1, string text2)
    {
        _txt1.text = _text1;
        _txt2.text = text2;
        this.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        _myBorder.localScale = Vector3.zero;
        _myBorder.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        _btnOK.onClick.AddListener(() => ClosePopup());
    }
    void ClosePopup()
    {
        _btnOK.onClick.RemoveListener(() => ClosePopup());
        _myBorder.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(DisablePopup);
    }
    void DisablePopup()
    {
        this.gameObject.SetActive(false);
    }

}
