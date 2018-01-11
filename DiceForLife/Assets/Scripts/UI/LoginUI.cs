using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Vinh write
/// </summary>
public class LoginUI : MonoBehaviour {
    public static LoginUI Instance;


    public GameObject loginObj;
    public GameObject signupObj;
    public GameObject popupWarningObj;


    public InputField userNameLogin;
    public InputField passWordLogin;

    public InputField userNameSignup;
    public InputField passWordSignup;

    public Button openLoginUIBtn;
    public Button closeLoginObjBtn;

    public Button openRegisterUIBtn;
    public Button closeRegisterUIBtn;


    public Button closePopupWarningObjBtn;
    public Button forgetPassBtn;
    public Button signupBtn;
    public Button loginBtn;
    public Button clearInputBtn;


    public Text _numerSkillDic, _numerSkillEquip;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        openLoginUIBtn.onClick.AddListener(OpenLoginObj);
        closeLoginObjBtn.onClick.AddListener(CloseLoginObj);
        openRegisterUIBtn.onClick.AddListener(openRegisterObj);
        closeRegisterUIBtn.onClick.AddListener(CloseRegisterObj);
        closePopupWarningObjBtn.onClick.AddListener(ClosePopupWarningObj);
        forgetPassBtn.onClick.AddListener(()=>OpenPopupWarningObj("Forget password ? Please contact admin to get your password!!"));
        signupBtn.onClick.AddListener(RegisterAccount);
        loginBtn.onClick.AddListener(LoginAccount);
        clearInputBtn.onClick.AddListener(ClearTextField);
    }
	
    void OpenLoginObj()
    {
        loginObj.SetActive(true);
    }
    void CloseLoginObj()
    {
        loginObj.SetActive(false);
    }

    void openRegisterObj()
    {
        signupObj.SetActive(true);
    }

	void CloseRegisterObj()
    {
        signupObj.SetActive(false);
    }

    void ClosePopupWarningObj()
    {
        popupWarningObj.SetActive(false);
    }

    public void OpenPopupWarningObj(string mess)
    {
        popupWarningObj.SetActive(true);
        popupWarningObj.transform.GetChild(0).GetComponent<Text>().text = mess;
    }

    void LoginAccount()
    {

    }
    void RegisterAccount()
    {

    }
    void ClearTextField()
    {
        userNameSignup.text = "";
        passWordSignup.text = "";
    }
}
