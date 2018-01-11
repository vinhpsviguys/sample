using DG.Tweening;
using UnityEngine;
public class CloudEff : MonoBehaviour {

    private float _posiX;
    // Use this for initialization
    void Start()
    {
        _posiX = transform.localPosition.x;
        MoveDown();
    }
    void MoveDown()
    {
        transform.DOLocalMoveX(_posiX - 150, 10).SetEase(Ease.Linear).OnComplete(MoveUp);
    }
    void MoveUp()
    {
        transform.DOLocalMoveX(_posiX, 10).SetEase(Ease.Linear).OnComplete(MoveDown);
    }
}
