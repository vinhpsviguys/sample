using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class WaitingRoomUI : MonoBehaviour {

    //public Text _countDownTime;

    public Transform _characterBound;
    public Text _namePlayer;
    public Text _levelPLayer;
    public Image _avatarPlayer;
    public Text _energyPoint;
    public Text _rankPoint;
    public Text _findingText;

    public Button startGame;
    public Button refeshRoom;
    public Button historyBattle;
    public Button pickSkill;
    public Button reconnectGame;
    public Button abaddonGame;

    public Sprite[] startGameState;


    public ScrollRect ScrollChat;
    public Button sendContentChat;
    public Transform contentChatArea;
    //public Transform listenerContent;
    public InputField inputChat;

    public GameObject _rankPanel, _historyPanel, _skillPanel;

    public ScrollRect scrollLog;
    public Transform _parentLog;
    public UnityEngine.Object _textLog;

    public static WaitingRoomUI Instance;

    private bool showListSkill = false;

    public bool checkReconnectUI = false;

    bool isSocketOff = false;
    //public bool canCreateRoom = true;

    //public bool checkFind = false;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;


        
    }
    // Use this for initializa
    private void Start()
    {
       //scrollLog = GameObject.Find("ScrollviewLog").GetComponent<ScrollRect>();
       //_parentLog = GameObject.Find("ContentLog").transform;
        sendContentChat.onClick.AddListener(SendMessageChat);
        startGame.onClick.AddListener(FindEnemy);
        refeshRoom.onClick.AddListener(RefreshRoom);
        historyBattle.onClick.AddListener(OpenBattlerecord);
        pickSkill.onClick.AddListener(OpenPickSkill);
        //reconnectGame.onClick.AddListener(ReconnectGame); remove for not using
        //abaddonGame.onClick.AddListener(AbaddonGame);
    }
    public void OnEnable()
    {
        WaitingRoomUI.Instance.SetLog("id user cua chinh minh " + SocketInitConnection.idUserCurrent);
        checkReconnectUI = false;
        //canCreateRoom = true;
        Debug.Log(PlayerPrefs.GetString("idroom","")+"|");
        if( PlayerPrefs.GetString("idroom","") == "")
        {
            startGame.gameObject.SetActive(true);
            reconnectGame.gameObject.SetActive(false);
            abaddonGame.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetString("idroom") != "")
        {
            startGame.gameObject.SetActive(false);
            reconnectGame.gameObject.SetActive(true);
            abaddonGame.gameObject.SetActive(true);
        }
        //this.PostEvent(EventID.InitListRoom);
        _findingText.gameObject.SetActive(false);
        _rankPanel.transform.DOLocalMoveX(500, 0.5f).SetEase(Ease.InOutElastic);
    }

    private void Update()
    {
        if (!SocketIOController.sfs.mySocket.IsOpen && !isSocketOff)
        {
            isSocketOff = true;
            WaitingPanelScript._instance.ShowWaiting(true);
        }
    }

    public void UpdateUI() {
        if (PlayerPrefs.GetString("idroom", "") == "")
        {
            startGame.gameObject.SetActive(true);
            reconnectGame.gameObject.SetActive(false);
            abaddonGame.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetString("idroom") != "")
        {
            startGame.gameObject.SetActive(false);
            reconnectGame.gameObject.SetActive(true);
            abaddonGame.gameObject.SetActive(true);
        }
    }

    void OpenPickSkill()
    {
        if (!WatingRoomController.Instance.isFindEnemy)
            _skillPanel.SetActive(true);
    }

    //void ReconnectGame() {
    //    Debug.Log("Init list room to update UI");
    //    WaitingRoomUI.Instance.SetLog("init list room to update UI");
    //    //checkFind = true;
    //    this.PostEvent(EventID.InitListRoom);
    //}

    //void AbaddonGame() {
    //}

    void OpenBattlerecord()
    {
        if (!WatingRoomController.Instance.isFindEnemy)
            _historyPanel.SetActive(true);
    }

    void RefreshRoom()
    {
        this.PostEvent(EventID.RefreshRoom);
    }
 
    public void FindEnemy()
    {
        //SetLog("checkfind " + checkFind);
        SetLog("isfindenemy " + WatingRoomController.Instance.isFindEnemy);
        if (!WatingRoomController.Instance.isFindEnemy && !WatingRoomController.Instance.isInSomeRoom)
        {
            WatingRoomController.Instance.isFindEnemy = true;
            startGame.GetComponent<Image>().sprite = startGameState[1];
            _findingText.gameObject.SetActive(true);
            _rankPanel.transform.DOLocalMoveX(1500, 0.5f).SetEase(Ease.InOutElastic);
            WaitingRoomUI.Instance.SetLog("socket is open " + SocketIOController.sfs.mySocket.IsOpen);
            if (SocketIOController.sfs.mySocket.IsOpen)
            {
                Debug.Log("Init list room");
                WaitingRoomUI.Instance.SetLog("init list room");
                //checkFind = true;
                this.PostEvent(EventID.InitListRoom);
            }
            else {// if disconnected or not login
                WaitingPanelScript._instance.ShowWaiting(true);
                //this.PostEvent(EventID.OnLoginSocketIO);
            }
            //else {
            //    SocketIOController.Instance.OnRefreshListRoom();
            //}
            //AutoFindEnemy();
                
        } else if (WatingRoomController.Instance.isFindEnemy && WatingRoomController.Instance.isInSomeRoom)
        {
            WatingRoomController.Instance.isFindEnemy = false;
            startGame.GetComponent<Image>().sprite = startGameState[0];
            _findingText.gameObject.SetActive(false);
            _rankPanel.transform.DOLocalMoveX(500, 0.5f).SetEase(Ease.InOutElastic);
            //checkFind = false;
            this.PostEvent(EventID.LeaveRoom);
            
            //DisableAutoFindEnemy();
        }
    }

    public void CancelFind()
    {
        SocketIOController.Instance.hasSomeoneInRoom = false;
        SocketIOController.Instance.idSocketUserEnteredRoom = "";
        SocketIOController.Instance.candicate = false;
        WatingRoomController.Instance.isFindEnemy = false;
        startGame.GetComponent<Image>().sprite = startGameState[0];
        _findingText.gameObject.SetActive(false);
        _rankPanel.transform.DOLocalMoveX(500, 0.5f).SetEase(Ease.InOutElastic);
        //checkFind = false;
        this.PostEvent(EventID.LeaveRoom);
    }
    // auto finding enemy
    // Author: LINHVT
    private void AutoFindEnemy() {
        Debug.Log("AutoFindEnemy");
        Sequence mySequence = DOTween.Sequence();
        mySequence.SetTarget(gameObject);
        mySequence.AppendCallback(() => {

            Debug.Log("Callback of AutoFind");
            try
            {
                if (SocketInitConnection.gameSpaceCurrent == "")
                {
                    Debug.Log("Not IsOpen "+(SocketIOController.sfs == null));
                    this.PostEvent(EventID.OnLoginSocketIO);
                }
                else
                {
                    Debug.Log("IsOpen");
                    SocketIOController.Instance.OnRefreshListRoom();
                }
            }
            catch (Exception e) {
                Debug.Log(""+e.Message);
            }
            
        }).AppendInterval(30);
        mySequence.SetLoops(-1);
    }

    //Disable Auto Finding Enemy
    private void DisableAutoFindEnemy() {
        DOTween.Kill(gameObject);
    }

    public void SendMessageChat()
    {
        string mess = inputChat.text;
        if (!string.IsNullOrEmpty(mess))
        {
            WatingRoomController.Instance.CreateContentChat(CharacterManager.Instance._meCharacter._baseProperties.idHero, mess);
            this.PostEvent(EventID.OnSendMessage, mess);
        }
    }
    public void BackToMainMenu()
    {
        if (!WatingRoomController.Instance.isFindEnemy)
        {
            //this.PostEvent(EventID.OnDisconnectSocketIO);
            //Destroy(SocketIOController.Instance.gameObject);
            Destroy(GameObject.Find("CharacterManager"));
            Destroy(CharacterManager.Instance._meCharacter.gameObject);
            if (CharacterManager.Instance._enemyCharacter != null)
            {
                Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
            }
            SocketIOController.Instance.ClearListenCreatOrJoinRoom();
            //Application.LoadLevel(2);
            SceneLoader._instance.LoadScene(2);
        }
        
    }

    public void FailedToConnect() {
        Destroy(GameObject.Find("CharacterManager"));
        Destroy(CharacterManager.Instance._meCharacter.gameObject);
        if (CharacterManager.Instance._enemyCharacter != null)
        {
            Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
        }
        SceneLoader._instance.LoadScene(3);
        //Application.LoadLevel("FindMatch");
    }
   
    public void SetLog(string content)
    {
        GameObject _log = Instantiate(_textLog, Vector3.zero, Quaternion.identity) as GameObject;
        _log.transform.parent = _parentLog;
        _log.transform.localScale = Vector3.one;
        _log.GetComponent<Text>().text = content;
    }

    


    public void ReconnectRoom()
    {
        SocketIOController.Instance.ReconnectCallback();
    }

    public void ClearIdRoom()
    {
        PlayerPrefs.SetString("idroom", "");
        startGame.gameObject.SetActive(true);
        reconnectGame.gameObject.SetActive(false);
        abaddonGame.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        //SocketIOController.Instance.DisableListenersInWaitingRoom();
    }
}
