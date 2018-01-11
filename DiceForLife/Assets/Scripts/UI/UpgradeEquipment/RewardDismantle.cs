using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
internal class RewardDismantle : MonoBehaviour
{
    [SerializeField] private Transform _border;
    [SerializeField] private Image _iconImg;
    [SerializeField] private Text _txtNumberItem, _txtNumberGem;
    [SerializeField] private Transform _itemTransform;
    [SerializeField] private GameObject _gemObject;

    private bool isRunestone;
    private int idItem;
    private int quantityItem;
    private int numberGem;

    internal void SetReward(bool isRunestone, int idItem, int quantity, int numberGem = 0)
    {
        Debug.Log(idItem);
        this.isRunestone = isRunestone;
        this.idItem = idItem;
        this.quantityItem = quantity;
        this.numberGem = numberGem;

        if (numberGem == 0)
        {
            _gemObject.SetActive(false);
            _itemTransform.localPosition = new Vector3(0, _itemTransform.localPosition.y);
        }
        else
        {
            _gemObject.SetActive(true);
            _txtNumberGem.text = numberGem.ToString();
            _itemTransform.localPosition = new Vector3(-100, _itemTransform.localPosition.y);
        }
        _txtNumberItem.text = quantity.ToString();

        this.gameObject.SetActive(true);
    }
    IEnumerator LoadIcon()
    {
        _iconImg.gameObject.SetActive(false);
        if (isRunestone)
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForGemsByID(idItem, value =>
            {
                _iconImg.gameObject.SetActive(true);
                _iconImg.sprite = value;
                _iconImg.SetNativeSize();
            }));
        else
            yield return StartCoroutine(ControllerItemsInGame._instance.GetIconForItemByID(idItem, value =>
            {
                _iconImg.gameObject.SetActive(true);
                _iconImg.sprite = value;
                _iconImg.SetNativeSize();
            }));
    }
    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.35f).SetEase(Ease.OutBack);
        StartCoroutine(LoadIcon());
    }
    public void CloseDialog()
    {
        _border.DOScale(0, 0.35f).SetEase(Ease.InBack).OnComplete(OnCompleteClose);
    }
    void OnCompleteClose()
    {
        this.gameObject.SetActive(false);
    }
}