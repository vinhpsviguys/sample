using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotifyVersion : MonoBehaviour
{
    [SerializeField] private Transform _border;
    [SerializeField] private Text _txtNotify;

    void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        _txtNotify.text = LoadingResource.Instance._dataNotify;
    }
    public void BtnClosePopup()
    {
        _border.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(CompleteClosePopup);
    }
    private void CompleteClosePopup()
    {
        gameObject.SetActive(false);
    }
}
