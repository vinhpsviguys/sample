using DG.Tweening;
using UnityEngine;
public class PopupFbUI : MonoBehaviour
{
    [SerializeField] private Transform _border;
    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    public void BtnConnectFB()
    {

    }
    public void CloseThisDialog()
    {
        _border.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(CompleteCloseDialog);
    }
    void CompleteCloseDialog()
    {
        this.gameObject.SetActive(false);

    }
}
