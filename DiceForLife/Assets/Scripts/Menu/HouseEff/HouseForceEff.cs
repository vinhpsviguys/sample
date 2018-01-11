using DG.Tweening;
using UnityEngine;
public class HouseForceEff : MonoBehaviour
{
    private float _posiY;
    // Use this for initialization
    void Start()
    {
        _posiY = transform.localPosition.y;
        MoveDown();
    }
    void MoveDown()
    {
        transform.DOLocalMoveY(_posiY - 50, 5).SetEase(Ease.Linear).OnComplete(MoveUp);
    }
    void MoveUp()
    {
        transform.DOLocalMoveY(_posiY, 5).SetEase(Ease.Linear).OnComplete(MoveDown);
    }
}
