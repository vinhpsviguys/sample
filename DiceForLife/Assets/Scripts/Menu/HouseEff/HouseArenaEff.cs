using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class HouseArenaEff : MonoBehaviour
{
    private Image _myImg;
    void Awake()
    {
        _myImg = GetComponent<Image>();
        FadeIn();
    }

    void FadeIn()
    {
        _myImg.DOFade(0, 2).OnComplete(FadeOut);
    }
    void FadeOut()
    {
        _myImg.DOFade(1, 2).OnComplete(FadeIn);
    }

    void LateUpdate()
    {
        transform.Rotate(0, 0, 1);
    }
}
