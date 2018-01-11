using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class HeroInfoUI : MonoBehaviour
{
    [SerializeField] private Transform _mainBorder, _changeNameBorder, _changeAvatarBorder;
    public Text nameText;
    public Text classNameText;
    public Text levelText;

    public Text expText;

    public Button _okChangeName;
    public Button _closeBtn;
    public Button _changeAvatarBtn;
    public Button _renameBtn;
    public Button _closeChangeNameBtn;
    public Button _closeChangeAvatarBtn;
    public GameObject _changeNamePanel;
    public GameObject _changeAvatar;

    public InputField _nameHero;
    // Use this for initialization

    private void OnEnable()
    {
        _mainBorder.gameObject.SetActive(true);
        _mainBorder.localScale = Vector3.zero;
        _mainBorder.DOScale(1, 0.35f).SetEase(Ease.OutBack);


        _closeBtn.onClick.AddListener(CloseThisDialog);
        _renameBtn.onClick.AddListener(OpenChangeNamePanel);
        _closeChangeNameBtn.onClick.AddListener(CLoseChangeNamePanel);
        _changeAvatarBtn.onClick.AddListener(OpenChangeAvatarPanel);
        _closeChangeAvatarBtn.onClick.AddListener(CloseChangeAvatarPanel);
        UpdateTextValue();
    }
    private void OnDisable()
    {
        _closeBtn.onClick.RemoveListener(CloseThisDialog);
        _renameBtn.onClick.RemoveListener(OpenChangeNamePanel);
        _closeChangeNameBtn.onClick.RemoveListener(CLoseChangeNamePanel);
        _changeAvatarBtn.onClick.RemoveListener(OpenChangeAvatarPanel);
        _closeChangeAvatarBtn.onClick.RemoveListener(CloseChangeAvatarPanel);
    }
    // Update is called once per frame
    void Update()
    {


    }
    void UpdateTextValue()
    {
        nameText.text = CharacterInfo._instance._baseProperties.name.ToString();
        classNameText.text = CharacterInfo._instance._baseProperties._classCharacter.ToString();
        levelText.text = "Level : " + CharacterInfo._instance._baseProperties.Level.ToString();
        expText.text = "Exp : " + CharacterInfo._instance._baseProperties.Exp.ToString();
    }
    void OpenChangeNamePanel()
    {
        _changeNamePanel.SetActive(true);
        _changeNameBorder.localScale = Vector3.zero;
        _changeNameBorder.DOScale(1, 0.35f).SetEase(Ease.OutBack);
    }
    void CLoseChangeNamePanel()
    {
        _changeNamePanel.SetActive(false);
    }
    void OpenChangeAvatarPanel()
    {
        _changeAvatar.SetActive(true);
        _changeAvatarBorder.localScale = Vector3.zero;
        _changeAvatarBorder.DOScale(1, 0.35f).SetEase(Ease.OutBack);
    }
    void CloseChangeAvatarPanel()
    {
        _changeAvatar.SetActive(false);
    }
    void CloseThisDialog()
    {
        _mainBorder.DOScale(0.3f, 0.35f).SetEase(Ease.InBack).OnComplete(DisablePanel);
    }
    private void DisablePanel()
    {
        this.gameObject.SetActive(false);
    }
    public void ChangeNameHero()
    {
        string newName = _nameHero.text;
        if (!string.IsNullOrEmpty(newName) && newName.Length >= 6 && newName.Length <= 20)
        {
            StartCoroutine(ServerAdapter.ChangeName(newName, CharacterInfo._instance._baseProperties.idCodeHero, CharacterInfo._instance._baseProperties.idHero, result =>
            {

                if (result.StartsWith("Error"))
                {
                    MainMenuUI._instance.ShowErrorPopup(9);
                }
                else
                {
                    SplitDataFromServe._heroCurrentPLay.name = newName;
                    CharacterInfo._instance._baseProperties.name = newName;
                    _changeNamePanel.SetActive(false);
                    nameText.text = CharacterInfo._instance._baseProperties.name.ToString();
                    this.PostEvent(EventID.OnPropertiesChange);
                    //SplitDataFromServe.ReadInitData(result);
                }
            }));
        }
        else
        {
            MainMenuUI._instance.ShowErrorPopup(8);
        }
    }
}
