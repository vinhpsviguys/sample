using UnityEngine;
using UnityEngine.UI;

public class SlotEquipInFittingRoom : MonoBehaviour
{
    [SerializeField] private Image _iconItem;
    [SerializeField] private Image _imgRare;
    [SerializeField] private Text _txtUpgraded;
    internal void SetData(Sprite _imgSprite, int idRare, int levelUpgraded)
    {
        _iconItem.sprite = _imgSprite;
        _iconItem.SetNativeSize();
        _imgRare.sprite = ControllerItemsInGame._instance._rareBorderItems[idRare];
        if (levelUpgraded == 0) _txtUpgraded.gameObject.SetActive(false);
        else _txtUpgraded.text = string.Format("+{0}", levelUpgraded);
        this.gameObject.SetActive(true);
    }
}
