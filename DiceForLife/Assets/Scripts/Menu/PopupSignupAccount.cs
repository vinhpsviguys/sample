using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PopupSignupAccount : MonoBehaviour
{
    [SerializeField] private Text _txtTitle, _txtLabel1, _txtLable2, _txtLabel3;
    [SerializeField] private InputField _input1, _input2, _input3;
    [SerializeField] private SettingUI _parentControll;
    private int statePopup;
    private string _value1, _value2, _value3;
    private string _rememberPassword, _rememberName;

    internal void ShowPopup(int idPopup)
    {
        statePopup = idPopup;
        _input1.text = string.Empty;
        _input2.text = string.Empty;
        _input3.text = string.Empty;
        if (idPopup == 0)//Popup Sign up
        {
            _txtTitle.text = "Sign up";

            _txtLabel1.text = "Mail";
            _input1.contentType = InputField.ContentType.EmailAddress;
            _input1.characterLimit = 0;

            _txtLable2.text = "Password";
            _input2.contentType = InputField.ContentType.Password;
            _input2.characterLimit = 12;

            _txtLabel3.gameObject.SetActive(true);
            _input3.gameObject.SetActive(true);
            _txtLabel3.text = "Confirm Password";
            _input3.contentType = InputField.ContentType.Password;
            _input3.characterLimit = 12;
        }
        else if (idPopup == 1)//Popup SwitchAccount
        {
            _txtTitle.text = "Switch Account";

            _txtLabel1.text = "Mail";
            _input1.contentType = InputField.ContentType.EmailAddress;
            _input1.characterLimit = 0;

            _txtLable2.text = "Password";
            _input2.contentType = InputField.ContentType.Password;
            _input2.characterLimit = 12;
            _txtLabel3.gameObject.SetActive(false);
            _input3.gameObject.SetActive(false);
        }
        else if (idPopup == 2)//Popup Change Password
        {
            _txtTitle.text = "Change Password";

            _txtLabel1.text = "Old Password";
            _input1.contentType = InputField.ContentType.Password;
            _input1.characterLimit = 12;

            _txtLable2.text = "New Password";
            _input2.contentType = InputField.ContentType.Password;
            _input2.characterLimit = 12;

            _txtLabel3.gameObject.SetActive(true);
            _input3.gameObject.SetActive(true);
            _txtLabel3.text = "Confirm Password";
            _input3.contentType = InputField.ContentType.Password;
            _input3.characterLimit = 12;
        }

        _rememberPassword = PlayerPrefs.GetString(Constant.PASSWORD);
        _rememberName = PlayerPrefs.GetString(Constant.USER_NAME);
        this.gameObject.SetActive(true);
    }

    public void BtnClick(int id)
    {
        if (id == 0) this.gameObject.SetActive(false);
        else
        {
            _value1 = _input1.text;
            _value2 = _input2.text;
            _value3 = _input3.text;
            if (statePopup == 0)//sign up
            {
                if (IsValidEmail(_value1))//email valid
                {
                    //Debug.Log("mail is valid");
                    if (_value2.Length >= 6)//password length
                    {
                        if (_value2 == _value3)//password match
                        {
                            StartCoroutine(ServerAdapter.SignUpAccount(_value1, _value2, SystemInfo.deviceUniqueIdentifier, result =>
                            {
                                if (result.StartsWith("Error"))
                                {
                                    TextNotifyScript.instance.SetData("Sign up failed!" + result);
                                }
                                else
                                {
                                    PlayerPrefs.SetString(Constant.USER_NAME, _value1);
                                    PlayerPrefs.SetString(Constant.PASSWORD, _value2);
                                    if (_parentControll == null) _parentControll = transform.parent.GetComponent<SettingUI>();
                                    _parentControll.SignUpComplete(_value1);
                                    BtnClick(0);
                                }
                            }));
                        }
                        else TextNotifyScript.instance.SetData("Passwords do not match!");
                    }
                    else TextNotifyScript.instance.SetData("Passwords must be at least 6 characters long!");
                }
                else TextNotifyScript.instance.SetData("Email is not valid!");
            }
            else if (statePopup == 1)//switch Account
            {
                if (IsValidEmail(_value1))//email valid
                {
                    if (_value2.Length >= 6)//password length
                    {
                        StartCoroutine(ServerAdapter.SwitchAccount(_value1, _value2, SystemInfo.deviceUniqueIdentifier, result =>
                         {
                             if (result.StartsWith("Error"))
                             {
                                 Debug.Log("Login failed!");
                                 TextNotifyScript.instance.SetData("Switch account errors!");
                             }
                             else
                             {
                                 PlayerPrefs.SetString(Constant.USER_NAME, _value1);
                                 PlayerPrefs.SetString(Constant.PASSWORD, _value2);
                                 StartCoroutine(SceneLoader._instance.LoadNewScene(0));
                             }
                         }));
                    }
                    else TextNotifyScript.instance.SetData("Passwords must be at least 6 characters long!");
                }
                else TextNotifyScript.instance.SetData("Email is not valid!");
            }
            else if (statePopup == 2)//change Password
            {
                if (_rememberPassword.Equals("") || _rememberPassword.Length < 6 || _rememberPassword.Length > 12)
                {
                    Debug.LogError("Sao lai vao day");
                }
                if (_value1.Equals(_rememberPassword))//password valid
                {
                    if (_value2 == _value3)//password match
                    {
                        if (_value2.Length >= 6)//password length
                        {
                            StartCoroutine(ServerAdapter.ChangePassword(_rememberName, _value1, _value2, result =>
                             {
                                 if (result.StartsWith("Error"))
                                 {
                                     TextNotifyScript.instance.SetData("Change password failed!" + result);
                                 }
                                 else
                                 {
                                     PlayerPrefs.SetString(Constant.PASSWORD, _value2);
                                     this.gameObject.SetActive(false);
                                 }
                             }));
                        }
                        else TextNotifyScript.instance.SetData("Passwords must be at least 6 characters long!");
                    }
                    else TextNotifyScript.instance.SetData("Passwords do not match!");
                }
                else TextNotifyScript.instance.SetData("Old password is not correct!");
            }
        }
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
}
