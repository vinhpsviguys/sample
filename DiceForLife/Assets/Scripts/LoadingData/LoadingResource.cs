using CoreLib;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

class LoadingResource : MonoBehaviour
{
    public static LoadingResource Instance;

    public Text _txtState;
    [SerializeField] private Text _txtVersion;
    [SerializeField]
    private GameObject _myPopupError, _myPopupNotify, _btnNotifyPopup, _myPopupChangeAccount, _myUILogin;
    [SerializeField]
    private Image imgProgess;

    private bool isHaveError;
    public static string dataError = "";
    public static int ID_ERROR = -1;
    private int countLoadedmyAsset = 0;
    private int countLoadedPrefabs = 0;
    private bool isNeedUpdateVersion;
    internal string _dataNotify;

    private string[] _myAsset = new string[] { "abnormalstatusdata", "equipments", "gems", "items", "monsterdatacaimpaign", "monsterprefabcaimpaign", "skilldata", "character", "barbarian", "assassin", "marksman", "orc", "paladin", "sorceress", "wizard", "cleric" };


    void Awake()
    {
        Instance = this;
        Constant.init();
        _myPopupError.SetActive(false);
        _txtVersion.text = String.Format("Version {0}", Application.version);
    }

    IEnumerator Start()
    {
        FakeProgress(0f);
        isHaveError = false;
        _txtState.text = "Checking internet...";
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _txtState.text = "Error. Check internet connection!";
            ShowPopupError(-1);
        }
        else
        {
            yield return StartCoroutine(ConnectServer());
            if (isHaveError)
            {
                ShowPopupError(0);
                yield return null;
            }
            else
            {
                yield return StartCoroutine(LoadInitData());
                if (isHaveError)
                {
                    ShowPopupError(2);
                    yield return null;
                }
                else
                {
                    yield return StartCoroutine(LoadGameResource());
                    if (isHaveError)
                    {
                        ShowPopupError(4);
                        yield return null;
                    }
                }
            }
        }
    }

    public void BtnClick(int id = 0)
    {
        switch (id)
        {
            case 0://Btn AutoLogin
                _myUILogin.SetActive(false);
                StartCoroutine(LoginByDefault());
                break;
            case 1://BtnNotify
                _myPopupNotify.SetActive(true);
                break;
            case 2://Btn Change Account
                _myPopupChangeAccount.SetActive(true);
                break;
        }
    }

    IEnumerator ConnectServer()
    {
        _txtState.text = "Connecting to getway server...";
        yield return new WaitForSeconds(1);
        isNeedUpdateVersion = false;
        int _currentVersion = PlayerPrefs.GetInt("CURRENT_VERSION", 1);
        yield return StartCoroutine(ServerAdapter.CheckServer(result =>
        {
            if (result.StartsWith("Error"))
            {
                isHaveError = true;
                dataError = result;
            }
            else
            {
                var N = CoreLib.JSON.Parse(result);
                if (N["version"] != null)
                {
                    int _serverVersion = N["version"].AsInt;
                    if (_serverVersion != _currentVersion)
                    {
                        isNeedUpdateVersion = true;
                    }
                }
                if (N["notify"] != null)
                {
                    _dataNotify = N["notify"].Value;
                    //if (_serverVersion > 1)
                    _myPopupNotify.SetActive(true);
                    //else _btnNotifyPopup.SetActive(false);
                }
                if (N["status"] != null)
                {
                    if (N["status"].Value.Equals("maintain"))
                    {
                        isHaveError = true;
                        dataError = "Server is maintaining. Please come back later!";
                    }
                }
            }

        }));
    }
    IEnumerator LoadInitData()
    {
        _txtState.text = "Loading game init data...";
        if (!PlayerPrefs.HasKey(Constant.AllINIT))
        {
            yield return StartCoroutine(ServerAdapter.LoadInitData(result =>
             {
                 if (result.StartsWith("Error"))
                 {
                     Debug.LogError("Load Init data failed!");
                 }
                 else
                 {
                     PlayerPrefs.SetString(Constant.AllINIT, result);
                     SplitDataFromServe.ReadInitData(result);

                 }
             }));
        }
        else
        {
            SplitDataFromServe.ReadInitData((PlayerPrefs.GetString(Constant.AllINIT)));

        }
        yield return null;
    }

    IEnumerator LoadGameResource()
    {
        if (!LoadingResourceController._instance.isLoaded)
        {
            countLoadedmyAsset = 0;
            //Debug.Log("load game resource");
            _txtState.text = "Loading game resources...";
            yield return new WaitForSeconds(1);
            int totalAsset = _myAsset.Length;

            StartCoroutine(ControllerItemsInGame._instance.GetIconRareItem());
            for (int i = 0; i < totalAsset; i++)
            {
                StartCoroutine(LoadingResourceController._instance.DownloadAndCache(_myAsset[i], result =>
                {
                    countLoadedmyAsset++;
                    FakeProgress(i * 1f / totalAsset);
                    if (result == false) isHaveError = true;
                    if (countLoadedmyAsset == totalAsset) StartCoroutine(CharacterItemInGame.Instance.GetTextAssetsMonsterCaimpaign());
                }));
            }
        }
        else
        {
            _txtState.text = "Tap to start";
            _myUILogin.SetActive(true);
        }
    }

    #region Loading Resouces
    public void LoadSkillToDictionary()
    {
        if (CharacterItemInGame.Instance._listTextAssetSkill.Count > 0)
        {
            int totalTextAssetSkill = CharacterItemInGame.Instance._listTextAssetSkill.Count;
            for (int i = 0; i < totalTextAssetSkill; i++)
            {
                string text = CharacterItemInGame.Instance._listTextAssetSkill[i].ToString();
                text = text.Replace("\r\n", "");
                int idSkill = CoreLib.JSONNode.Parse(text)["idInit"];
                NewSkill skill = new NewSkill(CoreLib.JSONNode.Parse(text));
                skill.addField("nick", "Skill" + idSkill);
                SplitDataFromServe.skillInit.Add("Skill" + idSkill, skill);
            }
        }
        //LoginUI.Instance._numerSkillDic.text = "Number assets " + CharacterItemInGame.Instance._listTextAssetSkill.Count;
        Adapter.skills = SplitDataFromServe.skillInit;
        StartCoroutine(CharacterItemInGame.Instance.GetTextAssetsAbsFiles());
    }
    internal void LoadAbsToDictionary()
    {
        if (CharacterItemInGame.Instance._listTextAssetAbnormal.Count > 0)
        {
            for (int i = 0; i < CharacterItemInGame.Instance._listTextAssetAbnormal.Count; i++)
            {
                string text = CharacterItemInGame.Instance._listTextAssetAbnormal[i].ToString();
                text = text.Replace("\r\n", "");
                int idAbs = CoreLib.JSONNode.Parse(text)["idInit"];
                AbnormalStatus Abs = new AbnormalStatus(CoreLib.JSONNode.Parse(text));
                Abs.addField("nick", "AS" + idAbs);
                SplitDataFromServe.absInit.Add("AS" + idAbs, Abs);
                SplitDataFromServe._heroAbs.Add(Abs);
            }
        }
        Adapter.abs = SplitDataFromServe.absInit;
        LoadEquippedData();
    }
    private void LoadEquippedData()
    {
        _txtState.text = "Loading shop init data...";
        if (!PlayerPrefs.HasKey(Constant.SHOPINIT))
        {
            StartCoroutine(ServerAdapter.LoadShopInit(result =>
            {
                if (result.StartsWith("Error"))
                {
                    Debug.LogError("Load shopinit failed!");
                }
                else
                {
                    PlayerPrefs.SetString(Constant.SHOPINIT, result);

                    SplitDataFromServe.ReadShopInitData(result);
                    StartCoroutine(CharacterItemInGame.Instance.ExecuteGetMonsterResource());
                }
            }));
        }
        else
        {
            SplitDataFromServe.ReadShopInitData(PlayerPrefs.GetString(Constant.SHOPINIT));
            StartCoroutine(CharacterItemInGame.Instance.ExecuteGetMonsterResource());
        }
    }
    public void ExecuteGetCharacterPrefabs()
    {
        _txtState.text = "Loading character resources...";
        countLoadedPrefabs = 0;
        int totalPrefabs = SplitDataFromServe._heroInits.Length;
        for (int i = 0; i < totalPrefabs; i++)
        {
            StartCoroutine(LoadingResourceController._instance.LoadAssetBundleObjectAsync(CharacterItemInGame.Instance.listClass[i].ToLower(), CharacterItemInGame.Instance.listClass[i], result =>
            {
                CharacterItemInGame.Instance._characterPrefabs.Add(CharacterItemInGame.Instance.listClass[countLoadedPrefabs], result);
                countLoadedPrefabs++;
                if (countLoadedPrefabs == totalPrefabs)
                {
#if UNITY_EDITOR
                    Debug.Log("Loading Resources is completed!");
#endif
                    LoadingResourceController._instance.isLoaded = true;
                    _txtState.text = "Tap to start";
                    _myUILogin.SetActive(true);
                }
            }));
        }
    }
    #endregion

    internal IEnumerator LoginByAccount(string user, string pass, Action<bool> isLoginComplete)
    {
        _txtState.text = "Verifying account...";
        yield return new WaitForSeconds(1);
        string deviceID = SystemInfo.deviceUniqueIdentifier;

        yield return StartCoroutine(ServerAdapter.SwitchAccount(user, pass, deviceID, result =>
        {
            if (result.StartsWith("Error"))
            {
                isHaveError = true;
                dataError = result;
                ShowPopupError(5);
                isLoginComplete(false);
            }
            else
            {
                isLoginComplete(true);
                ReadDataAccount(result);
            }
        }));
    }
    internal IEnumerator LoginByDefault()
    {
        _txtState.text = "Verifying account...";
        yield return new WaitForSeconds(1);

        string deviceID = SystemInfo.deviceUniqueIdentifier;
        int typePlatform = 0;
#if UNITY_ANROID || UNITY_EDITOR
        typePlatform = 0;
#elif UNITY_IOS
        typePlatform = 1;
#endif
        string defaultName = string.Format("Guest{0}", deviceID.Substring(0, 6));
        string defaultUserName = PlayerPrefs.GetString(Constant.USER_NAME);
        string defaultPassword = PlayerPrefs.GetString(Constant.PASSWORD);

        yield return StartCoroutine(ServerAdapter.Login(defaultUserName, defaultPassword, defaultName, deviceID, typePlatform, result =>
        {
            if (result.StartsWith("Error"))
            {
                isHaveError = true;
                dataError = result;
                ShowPopupError(5);
            }
            else
            {
                ReadDataAccount(result);
            }
        }));
    }
    void ReadDataAccount(string _data)
    {
        var N = JSON.Parse(_data);
        if (N["hero"].Count == 0)
        {
            SplitDataFromServe.ReadLoginData(_data);
            StartCoroutine(SceneLoader._instance.LoadNewScene(1));
        }
        else if (N["hero"].Count > 0)
        {
            SplitDataFromServe.ReadLoginData(_data);

            int idHero = int.Parse(SplitDataFromServe._myAccount.idhplayed);
            if (idHero == 0) StartCoroutine(SceneLoader._instance.LoadNewScene(1));
            else
            {
                StartCoroutine(LoadDataDetailHero(int.Parse(SplitDataFromServe._myAccount.idhplayed), SplitDataFromServe._myAccount.idcode));
                PlayerPrefabsController.SetStringData(Constant.IDHERO_CURRENTPLAY, SplitDataFromServe._myAccount.idhplayed);
            }
            //if (!PlayerPrefs.HasKey(Constant.IDHERO_CURRENTPLAY))
            //{
            //    StartCoroutine(LoadDataDetailHero(int.Parse(SplitDataFromServe._myAccount.idhplayed), SplitDataFromServe._myAccount.idcode));
            //    PlayerPrefabsController.SetStringData(Constant.IDHERO_CURRENTPLAY, SplitDataFromServe._myAccount.idhplayed);
            //}
            //else
            //{
            //    Debug.Log(SplitDataFromServe._myAccount.idhplayed);
            //    if (int.Parse(SplitDataFromServe._myAccount.idhplayed) == 0)
            //    {
            //        Debug.Log("vv1");
            //        StartCoroutine(SceneLoader._instance.LoadNewScene(1));
            //    }
            //    else if (int.Parse(SplitDataFromServe._myAccount.idhplayed) != 0)
            //    {
            //        Debug.Log("Load lại nhân vật đã chơi");
            //        //StartCoroutine(LoadDataDetailHero(int.Parse(PlayerPrefabsController.GetStringData(Constant.IDHERO_CURRENTPLAY)), SplitDataFromServe._myAccount.idcode));
            //        StartCoroutine(LoadDataDetailHero(int.Parse(SplitDataFromServe._myAccount.idhplayed), SplitDataFromServe._myAccount.idcode));
            //    }
            //}
        }
    }

    IEnumerator LoadDataDetailHero(int _idhero, string _idcode)
    {
        StartCoroutine(ServerAdapter.ListSkillOfHero(_idhero, _idcode, result =>
        {
            if (result.StartsWith("Error"))
            {
                Debug.Log("Do nothing");
            }
            else
            {
                SplitDataFromServe.ReadSkillHeroData(result.ToString());
            }
        }));


        yield return StartCoroutine(ServerAdapter.LoadDetailHero(_idcode, _idhero, result =>
        {
            if (result.StartsWith("Error"))
            {
                Debug.Log("Do nothing");
                ShowPopupError(6);
            }
            else
            {
                SplitDataFromServe.ReadDetailDataHeroCurrentPlay(result);
                SplitDataFromServe.ReadItemInBagData(result);
                StartCoroutine(SceneLoader._instance.LoadNewScene(2));
            }
        }));
    }


    public void ShowPopupError(int idError)
    {
        Constant.ID_ERROR = idError;
        _myPopupError.SetActive(true);
    }
    public void SaveJsonDataToFile(string fileName, string jsonString)
    {
        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), jsonString);
    }
    public string LoadJsonDataFromFile(string fileName)
    {
        return File.ReadAllText(Path.Combine(Application.persistentDataPath, fileName));
    }

    public IEnumerator LoadDataProgress(WWW www)
    {
        imgProgess.fillAmount = 0;
        while (www.progress < 0.9f)
        {
            imgProgess.fillAmount = www.progress;
            yield return null;
        }
        yield return new WaitForEndOfFrame();

    }
    public IEnumerator LoadDataProgress(UnityWebRequest www)
    {
        imgProgess.fillAmount = 0;
        while (!www.isDone)
        {
            imgProgess.fillAmount = www.downloadProgress;
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }
    public void FakeProgress(float _rate)
    {
        imgProgess.fillAmount = _rate;
    }
}