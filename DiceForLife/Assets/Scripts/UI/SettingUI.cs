using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SettingUI : MonoBehaviour
{

    [SerializeField] private Transform _border;
    [SerializeField] private GameObject _connectFbPanel;
    [SerializeField] private Button _soundBtn, _musicBtn, _facebookBtn;
    [SerializeField] private Sprite _soundOn, _soundOff, _musicOn, _musicOff;

    bool soundState = false;
    bool musicState = false;

    [SerializeField] private Image _iconAvatar;
    [SerializeField] private Text _txtname, _txtemail, _txtBtnLogin;
    [SerializeField] private PopupSignupAccount _popupSignUp;
    private string _userName;
    private bool isHaveAccount;
    private void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.25f).SetEase(Ease.OutBack);

        ControlSound();
        ControlMusic();
        _soundBtn.onClick.AddListener(ControlSound);
        _musicBtn.onClick.AddListener(ControlMusic);
        _facebookBtn.onClick.AddListener(OpenFacebookLogin);

        _userName = PlayerPrefs.GetString(Constant.USER_NAME);
        //Debug.Log(_userName);
        isHaveAccount = !string.IsNullOrEmpty(_userName);
        _iconAvatar.sprite = CharacterItemInGame.Instance._avatarRect[(int)CharacterInfo._instance._baseProperties._classCharacter];
        _txtname.text = CharacterInfo._instance._baseProperties.name.ToString();
        _txtemail.text = _userName;
        if (isHaveAccount)
        {
            _txtBtnLogin.text = "Change Password";
        }
        else _txtBtnLogin.text = "Sign up";
    }

    private void OnDisable()
    {
        _soundBtn.onClick.RemoveListener(ControlSound);
        _musicBtn.onClick.RemoveListener(ControlMusic);
        _facebookBtn.onClick.RemoveListener(OpenFacebookLogin);
    }

    public void ControlSound()
    {
        if (soundState)
        {
            soundState = false;
            _soundBtn.image.sprite = _soundOff;
        }
        else
        {
            soundState = true;
            _soundBtn.image.sprite = _soundOn;
        }
    }
    public void ControlMusic()
    {
        if (musicState)
        {
            musicState = false;
            _musicBtn.image.sprite = _musicOff;
        }
        else
        {
            musicState = true;
            _musicBtn.image.sprite = _musicOn;
        }
    }
    public void OpenFacebookLogin()
    {
        _connectFbPanel.SetActive(true);
    }

    public void BtnAccount(int id)
    {
        if (id == 0)
        {
            if (isHaveAccount)
                _popupSignUp.ShowPopup(2);
            else _popupSignUp.ShowPopup(0);
        }
        else if (id == 1)
        {
            _popupSignUp.ShowPopup(1);
        }
    }
    internal void SignUpComplete(string _newUsername)
    {
       this._userName = _newUsername;
        _txtemail.text = _newUsername;
        isHaveAccount = true;
        _txtBtnLogin.text = "Change Password";
    }


    public void CloseThisDialog()
    {
        _border.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(CompleteCloseDialog);
    }
    void CompleteCloseDialog()
    {
        this.gameObject.SetActive(false);

    }
}
