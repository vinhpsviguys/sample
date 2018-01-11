using DG.Tweening;
using UnityEngine;
public class HouseTowerEff : MonoBehaviour
{
    private float _delay;
    private SpriteRenderer _img;
    // Use this for initialization
    void Start()
    {
        _img = GetComponent<SpriteRenderer>();
        _delay = Random.Range(0.2f, 0.5f);
        Invoke("Fade", _delay);
    }
    void Fade()
    {
        _img.DOFade(0, 1.5f).SetEase(Ease.Linear).OnComplete(UnFade);
    }
    void UnFade()
    {
        _img.DOFade(1, 1.5f).SetEase(Ease.Linear).OnComplete(Fade);
    }
}
