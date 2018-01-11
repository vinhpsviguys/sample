using DG.Tweening;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
public class PopupLoginAccount : MonoBehaviour
{
    [SerializeField] private Transform _border, _borderRetrive;
    [SerializeField] private InputField _nameInput, _passwordInput;

    private string _name, _password;
    private bool isWaitingLogin;
    void OnEnable()
    {
        _border.localScale = Vector3.zero;
        _border.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        string defaultUserName = PlayerPrefs.GetString(Constant.USER_NAME);
        string defaultPassword = PlayerPrefs.GetString(Constant.PASSWORD);
        _nameInput.text = defaultUserName;
        _passwordInput.text = defaultPassword;
        isWaitingLogin = false;
    }
    private string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
    bool IsValidEmail(string email)
    {
        if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
        else return false;
    }
    public void BtnLogin()
    {
        if (isWaitingLogin) return;
        isWaitingLogin = true;
        _name = _nameInput.text;
        _password = _passwordInput.text;

        if (_name.Equals("") || _password.Equals("")) return;
        else if (!IsValidEmail(_name))
        {
            PopupErrorController.Instance.ShowErrorWithContent("Email is not valid!");
        }
        else if (_password.Length < 6)
        {
            PopupErrorController.Instance.ShowErrorWithContent("Passwords must be at least 6 characters long!");
        }
        else StartCoroutine(LoadingResource.Instance.LoginByAccount(_name, _password, result =>
          {
              if (result)
              {
                  PlayerPrefs.SetString(Constant.USER_NAME, _name);
                  PlayerPrefs.SetString(Constant.PASSWORD, _password);
                  BtnClosePopup();
              }
              else
              {
                  isWaitingLogin = false;
              }
          }));
    }
    public void BtnForgetPass()
    {
        if (isWaitingLogin) return;
        _borderRetrive.parent.gameObject.SetActive(true);
        _borderRetrive.localScale = Vector3.zero;
        _borderRetrive.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    public void BtnClosePopupRetrive()
    {
        _borderRetrive.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(CompleteClosePopupRetrive);
    }
    private void CompleteClosePopupRetrive()
    {
        _borderRetrive.parent.gameObject.SetActive(false);
    }


    public void BtnClosePopup()
    {
        if (isWaitingLogin) return;
        _border.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(CompleteClosePopup);
    }
    private void CompleteClosePopup()
    {
        gameObject.SetActive(false);
    }
}
