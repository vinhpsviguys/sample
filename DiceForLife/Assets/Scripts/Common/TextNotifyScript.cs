using DG.Tweening;
using UnityEngine;

public class TextNotifyScript : MonoBehaviour
{
    public static TextNotifyScript instance;
    [SerializeField] private SpriteRenderer _bgMes;
    private Transform _bgTransform;
    [SerializeField] private TextMesh _txtMes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.tag = "DontDestroyObject";
            _bgTransform = _bgMes.transform;
            transform.localPosition = Vector3.zero;
            _txtMes.GetComponent<MeshRenderer>().sortingOrder = _bgMes.sortingOrder;
            CompleteZoomOut();

        }
        else DestroyImmediate(this.gameObject);
    }
    internal void SetData(string text)
    {
        _txtMes.text = text;
        this.gameObject.SetActive(true);
        _bgTransform.DOKill();
        _bgMes.DOKill();
        _bgTransform.localScale = Vector3.zero;
        _bgMes.color = Color.white;
        _bgTransform.DOScale(1, 0.3f).OnComplete(CompleteZoomIn);
    }

    void CompleteZoomIn()
    {
        _bgTransform.DOScale(1, 1f).OnComplete(CompleteShow);
    }
    void CompleteShow()
    {
        _bgTransform.DOScale(0.8f, 0.3f).OnComplete(CompleteZoomOut);
        _bgMes.DOFade(0, 0.3f);
    }
    void CompleteZoomOut()
    {
        this.gameObject.SetActive(false);
    }

}