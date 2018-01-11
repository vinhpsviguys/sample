using DG.Tweening;
using UnityEngine;
public class RechargeGoldUI : MonoBehaviour
{
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

    public void GetFreeGold()
    {
        StartCoroutine(ServerAdapter.AddCustomValue(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, "gold", 5000, result =>
        {
            if (result.StartsWith("Error"))
            {
                Debug.Log("Do nothing");
            }
            else
            {
                CharacterInfo._instance._baseProperties.Gold += 5000;
                this.PostEvent(EventID.OnPropertiesChange);
            }
        }));
    }

    public void GetGoldByGems(int _gems)
    {
        if (CharacterInfo._instance._baseProperties.Diamond >= _gems)
        {
            StartCoroutine(ServerAdapter.ReduceCustomValue(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, "diamond", _gems, result1 =>
            {
                if (result1.StartsWith("Error"))
                {
                    Debug.Log("Do nothing");
                }
                else
                {
                    StartCoroutine(ServerAdapter.AddCustomValue(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, "gold", 5000, result2 =>
                    {
                        if (result2.StartsWith("Error"))
                        {
                            Debug.Log("Do nothing");
                        }
                        else
                        {
                            CharacterInfo._instance._baseProperties.Gold += 5000;
                            CharacterInfo._instance._baseProperties.Diamond -= _gems;
                            this.PostEvent(EventID.OnPropertiesChange);
                        }
                    }));
                }
            }));
        }
    }
}
