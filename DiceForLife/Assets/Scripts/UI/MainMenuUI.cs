using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI _instance;

    [Serializable]
    public class EditorReferences
    {
        public Transform _PanelParent;
        public ItemInforController _itemInfor;

        public GameObject _settingPanelPrefabs;
        public GameObject _playerInforPanelPrefabs;
        public GameObject _upgradeEquipPanelPrefabs;
        public GameObject _dailyChallengePanelPrefabs;
        public GameObject _characterPanelPrefabs;
        public GameObject _skillPanelPrefabs;
        public GameObject _inboxPanelPrefabs;
        public GameObject _worldChatPanelPrefabs;
    }
    [SerializeField]
    public MainMenuUI.EditorReferences references = new MainMenuUI.EditorReferences();

    public GameObject _luckyWheelPanel, _newPanels, _leaderboardPanel, _achievementPanel;
    //[SerializeField]
    public GameObject _addGoldPanel, _addGemPanel, _addEventPointPanel;

    private GameObject _settingPanel, _panelPlayerInfor, _upgradePanel, _dailyChallengePanel, _characterPanel, _skillPanel, _inboxPanel, _chatPanel;

    //[SerializeField]
    public GameObject _noticePanel, _friendPanel, _guildPanel, _questPanel, _getFreeGemPanel, _eventPanel;
    public GameObject _tarvernPanel, _shopPanel, _goldStorePanel, _storagePanel;
    //[SerializeField]
    public GameObject _errorPopup;

    public Image avatarCharacter;
    public Text lvlTextOnMain, nameTextOnMain, heathTextOnMain, expTextOnMain, skillPointTextOnMain;
    public Text goldNumber, eventPointNumber, diamondNumber;

    Action<object> _UpdateTextValueEventRef;

    bool isSocketOff = false;
    void Awake()
    {
        _instance = this;
        ItemInforController.instance = references._itemInfor;
    }
    // Use this for initialization
    void Start()
    {

        _UpdateTextValueEventRef = (param) => UpdateTextValue();
        this.RegisterListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
    }

    private void Update()
    {
        if (!SocketIOController.sfs.mySocket.IsOpen && !isSocketOff && SocketIOController.Instance.loggedSocket)
        {
            isSocketOff = true;
            WaitingPanelScript._instance.ShowWaiting(true);
        }
    }

    public void BtnHouseClick(int idHouse)
    {
        switch (idHouse)
        {
            case 1://House Treasury
                TextNotifyScript.instance.SetData("Coming soon!");
                break;
            case 2://House Shop
                SceneManager.LoadScene("Shop");
                break;
            case 3://House Arena
                SceneLoader._instance.LoadScene(3);
                //SceneManager.LoadScene("FindMatch");
                break;
            case 4: //House Dungeon
                SceneLoader._instance.LoadScene(5);
                //SceneManager.LoadScene("Campaign");
                break;
            case 5: //House Pet
                TextNotifyScript.instance.SetData("Coming soon!");
                break;
            case 6: //House Force
                ActivePanel(ref _upgradePanel, references._upgradeEquipPanelPrefabs);
                break;
            case 7: //House Tower
                TextNotifyScript.instance.SetData("Coming soon!");
                break;
        }
    }
    public void BtnStaticClick(int idBtn)
    {
        switch (idBtn)
        {
            case 1://Infor hero
                ActivePanel(ref _panelPlayerInfor, references._playerInforPanelPrefabs);
                break;
            case 2://Btn Inbox
                ActivePanel(ref _inboxPanel, references._inboxPanelPrefabs);
                break;
            case 3://Btn Archivement
                _achievementPanel.SetActive(true);
                break;
            case 4: //BtnLucky Wheel
                _luckyWheelPanel.SetActive(true);
                break;
            case 5: //Btn Event
                _eventPanel.SetActive(true);
                break;
            case 6: //Btn Boss Arena

                break;
            case 7: //Btn Chat
                ActivePanel(ref _chatPanel, references._worldChatPanelPrefabs);
                break;

            case 8: //Btn Buy gems
                break;

            case 9: //Btn Inventory
                ActivePanel(ref _characterPanel, references._characterPanelPrefabs);
                break;
            case 10: //Btn Skill
                ActivePanel(ref _skillPanel, references._skillPanelPrefabs);
                break;
            case 11: //Btn Friend
                _friendPanel.SetActive(true);
                break;
            case 12: //Btn Guild
                break;
            case 13: //Btn Daily Challeged
                ActivePanel(ref _dailyChallengePanel, references._dailyChallengePanelPrefabs);
                break;
            case 14: //Btn Daily Mision
                _questPanel.SetActive(true);
                break;
            case 15: //Btn Setting
                ActivePanel(ref _settingPanel, references._settingPanelPrefabs);
                break;
            case 16: //Btn Introduce
                break;
        }
    }
    private void ActivePanel(ref GameObject _input, GameObject _prefabs)
    {
        if (_input == null)
        {
            _input = Instantiate(_prefabs) as GameObject;
            _input.transform.SetParent(references._PanelParent);
            _input.transform.localScale = Vector3.one;

            RectTransform _rect = _input.GetComponent<RectTransform>();
            _rect.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            float sizeX = references._PanelParent.GetComponent<CanvasScaler>().referenceResolution.x;
            //Debug.Log(sizeX);
            //Debug.Log(sizeX * Screen.height / Screen.width);
            _rect.sizeDelta = new Vector2(sizeX, sizeX * Screen.height / Screen.width);
            //_rect.sizeDelta = new Vector2(Screen.width, Screen.height);
        }
        _input.SetActive(true);
    }

    public void OpenRechargeDiamondPanel()
    {
        _addGemPanel.SetActive(true);
    }
    public void OpenRechargeGoldPanel()
    {
        _addGoldPanel.SetActive(true);
    }
    public void UpdateTextValue()
    {
        lvlTextOnMain.text = CharacterInfo._instance._baseProperties.Level.ToString();
        nameTextOnMain.text = CharacterInfo._instance._baseProperties.name.ToString();
        heathTextOnMain.text = "Heath : " + CharacterInfo._instance._baseProperties.hp.ToString();
        expTextOnMain.text = "Exp : " + CharacterInfo._instance._baseProperties.Exp.ToString();
        skillPointTextOnMain.text = "Skill point : " + CharacterInfo._instance._skillPoints.ToString();

        goldNumber.text = CharacterInfo._instance._baseProperties.Gold.ToString();
        eventPointNumber.text = CharacterInfo._instance._baseProperties.EventPoint.ToString();
        diamondNumber.text = CharacterInfo._instance._baseProperties.Diamond.ToString();
        //Debug.Log((int)CharacterInfo._instance._baseProperties._classCharacter);
        avatarCharacter.sprite = CharacterItemInGame.Instance._avatarRect[(int)CharacterInfo._instance._baseProperties._classCharacter];
    }
    public void ShowErrorPopup(int idError)
    {
        Constant.ID_ERROR = idError;
        _errorPopup.SetActive(true);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnPropertiesChange, _UpdateTextValueEventRef);
    }
}
