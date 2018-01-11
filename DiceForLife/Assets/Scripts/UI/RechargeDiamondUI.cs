using DG.Tweening;
using UnityEngine;

public class RechargeDiamondUI : MonoBehaviour {
    [SerializeField] private Transform _border;


    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(CompleteZoomOut);
    }
    void CompleteZoomOut()
    {
        _border.localScale = Vector3.one;
    }
    public void CloseThisDialog()
    {
        _border.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(CompleteZoomIn);
    }
    void CompleteZoomIn()
    {
        this.gameObject.SetActive(false);
    }


    public void GetDiamond(int _numberDiamond)
    {
        StartCoroutine(ServerAdapter.AddCustomValue(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, "diamond", _numberDiamond, result => {
            if (result.StartsWith("Error"))
            {
                Debug.Log("Do nothing");
            }
            else
            {
                CharacterInfo._instance._baseProperties.Diamond += 5000;
                this.PostEvent(EventID.OnPropertiesChange);
            }
        }));
    }
}
