using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using CoreLib;

public enum STATEINWAITING
{
    NONE,
    REQUEST_ID,
    DOWNLOAD_DATA,
    CONFIRM_LOADDATA,
    RECONNECT
}

public class WatingRoomController : MonoBehaviour
{

    public static WatingRoomController Instance;
    public Dictionary<int, GameObject> mySkillObjDic = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> listAllSkillClassDic = new Dictionary<int, GameObject>();

    public bool isFindEnemy = false;
    public bool isInSomeRoom = false;
    public bool isInWaitingRoom = true;

    public STATEINWAITING state_waitingroom;

    Action<object> _RefreshRoom, _InitMECharacterEventRef, _InitEnemyCharacterEventRef;

    public Timeouter reconnectTimout = null;


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
    // Use this for initialization
    void Start()
    {
        isInWaitingRoom = true;
        state_waitingroom = STATEINWAITING.NONE;

        _InitMECharacterEventRef = (param) => UpdateWatingRoomUI();
        this.RegisterListener(EventID.OnInitMECharacter, _InitMECharacterEventRef);

        _InitEnemyCharacterEventRef = (param) => UpdateWatingRoomUI();
        this.RegisterListener(EventID.OnInitEnemyCharacter, _InitEnemyCharacterEventRef);

        _RefreshRoom = (param) => RefreshRoom();
        this.RegisterListener(EventID.RefreshRoom, _RefreshRoom);

        CharacterManager.Instance.CreatePlayerProperties();
        isFindEnemy = false;
        isInWaitingRoom = true;
        //CreateMeSkill();
        //UpdateWatingRoomUI();
        SocketIOController.Instance.EnableListenersInWaitingRoom();
        this.PostEvent(EventID.InitListRoom);
        SocketIOController.Instance.isRefresh = true;// check if the previous exists or not
        CharacterInfo._instance.keyPlayer = 0;
    }



    //IEnumerator ExecuteCountdownTime()
    //{
    //    finishCountdown = false;
    //    int countdown = 15;
    //    WaitingRoomUI.Instance._countDownTime.text = countdown.ToString();
    //    while (countdown > 0)
    //    {
    //        if (!isHaveEnemy) break;
    //            yield return new WaitForSeconds(1.0f);
    //            countdown--;
    //            WaitingRoomUI.Instance._countDownTime.text = countdown.ToString();
    //    }
    //    if (isHaveEnemy)
    //    {
    //        WaitingRoomUI.Instance._countDownTime.text = "Start!";
    //        finishCountdown = true;
    //        if (CharacterManager.Instance._meCharacter.keyPlayer == 1)
    //        {
    //            isReady = true;
    //            this.PostEvent(EventID.ChangeReadyStateWaitingRoom);
    //        }
    //    } else
    //    {
    //        WaitingRoomUI.Instance._countDownTime.text = "";
    //        WaitingRoomUI.Instance.loadingPanel.SetActive(true);
    //    }
    //}
    //private void LateUpdate()
    //{
    //    if (CharacterManager.Instance._meCharacter.keyPlayer == 0 && isReady && finishCountdown && startGame==false)
    //    {
    //        WatingRoomController.Instance.isInWaitingRoom = false;
    //        Application.LoadLevel("BattleMonster");
    //        this.PostEvent(EventID.StartBattle);
    //        startGame = true;
    //    }
    //}

    private void Update()
    {
        if (reconnectTimout != null)
        {
            reconnectTimout.update();
        }
    }

    public void UpdateWatingRoomUI()
    {
        if (CharacterManager.Instance._meCharacter != null)
        {

            CharacterManager.Instance._meCharacter.transform.position = new Vector3(WaitingRoomUI.Instance._characterBound.position.x, -2f, WaitingRoomUI.Instance._characterBound.position.z);
            WaitingRoomUI.Instance._namePlayer.text = CharacterManager.Instance._meCharacter._baseProperties.name.ToString();
            WaitingRoomUI.Instance._levelPLayer.text = "Level " + CharacterManager.Instance._meCharacter._baseProperties.Level.ToString();
            WaitingRoomUI.Instance._avatarPlayer.sprite = CharacterItemInGame.Instance._avatarRect[(int)(CharacterManager.Instance._meCharacter._baseProperties._classCharacter)];
            WaitingRoomUI.Instance._energyPoint.text = CharacterManager.Instance._meCharacter._baseProperties.EnergyPoint.ToString();
            WaitingRoomUI.Instance._rankPoint.text = CharacterManager.Instance._meCharacter._baseProperties.PvpPoint.ToString();
        }


    }

    void StartFuction()
    {

    }

    //public void UpdateStartBtnState()
    //{

    //    if (CharacterManager.Instance._meCharacter.keyPlayer == 1 && isHaveEnemy && isReady)
    //    {
    //        WaitingRoomUI.Instance.startGame.image.sprite = WaitingRoomUI.Instance.startGameState[0];
    //    }
    //    else if (CharacterManager.Instance._meCharacter.keyPlayer == 1 && isHaveEnemy && !isReady)
    //    {
    //        WaitingRoomUI.Instance.startGame.image.sprite = WaitingRoomUI.Instance.startGameState[1];
    //    }
    //    else if (CharacterManager.Instance._meCharacter.keyPlayer == 0 && isHaveEnemy && isReady)
    //    {
    //        WaitingRoomUI.Instance.startGame.image.sprite = WaitingRoomUI.Instance.startGameState[3];
    //    }
    //    else if (CharacterManager.Instance._meCharacter.keyPlayer == 0 && isHaveEnemy && !isReady)
    //    {
    //        WaitingRoomUI.Instance.startGame.image.sprite = WaitingRoomUI.Instance.startGameState[2];
    //    }

    //}

    public void OnHoverToUI(GameObject skillObj, int idSkill)
    {

        // Event when hover mouse to button
        EventTrigger trigger = skillObj.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;

        // create the listener and attach to callback
        EventTrigger.TriggerEvent pointerEnterEvent = new EventTrigger.TriggerEvent();
        string des = CharacterItemInGame.Instance.LoadDataSkill(idSkill).nameSkill + " \n " + CharacterItemInGame.Instance.LoadDataSkill(idSkill).descriptionSkill + "\n" + " Action required: " + CharacterItemInGame.Instance.LoadDataSkill(idSkill).actionPointRequired;
        pointerEnterEvent.AddListener((x) =>
        {
            //WaitingRoomUI.Instance.ShowDialog(des);
        });

        pointerEnter.callback = pointerEnterEvent;

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;

        // create the listener and attach to callback
        EventTrigger.TriggerEvent pointerExitEvent = new EventTrigger.TriggerEvent();
        pointerExitEvent.AddListener((x) =>
        {
            //WaitingRoomUI.Instance.HideDialog();
        });

        pointerExit.callback = pointerExitEvent;

        // add this to delegates list, ensure delegates is not null
        if (trigger.triggers == null)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
    }

    public void CreateContentChat(int idUser, string mess)
    {
        if (CharacterManager.Instance._meCharacter._baseProperties.idHero == idUser)
        {
            GameObject chatObj = Instantiate(Resources.Load("Prefabs/ChatContent") as GameObject);
            chatObj.transform.parent = WaitingRoomUI.Instance.contentChatArea;
            chatObj.transform.localScale = new Vector3(1f, 1f, 1f);
            chatObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = CharacterInfo._instance._baseProperties.Level.ToString();
            chatObj.transform.GetChild(1).GetComponent<Text>().text = CharacterInfo._instance._baseProperties.name.ToString();
            chatObj.transform.GetChild(1).GetComponent<Text>().color = new Color(0f, 169f / 255f, 71f / 255f, 255f / 255f);
            chatObj.transform.GetChild(2).GetComponent<Text>().text = mess;
            if (WaitingRoomUI.Instance.contentChatArea.childCount >= 20)
            {
                Destroy(WaitingRoomUI.Instance.contentChatArea.GetChild(0).gameObject);
            }
            WaitingRoomUI.Instance.inputChat.text = "";
            StartCoroutine(ScollDownToBottom());
        }

    }
    public void CreateContentEnemyChat(int idUser, string mess)
    {
        if (CharacterManager.Instance._meCharacter._baseProperties.idHero != idUser)
        {
            GameObject chatObj = Instantiate(Resources.Load("Prefabs/ChatContent") as GameObject);
            chatObj.transform.parent = WaitingRoomUI.Instance.contentChatArea;
            chatObj.transform.localScale = new Vector3(1f, 1f, 1f);
            chatObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = CharacterManager.Instance._enemyCharacter._baseProperties.Level.ToString();
            chatObj.transform.GetChild(1).GetComponent<Text>().text = CharacterManager.Instance._enemyCharacter._baseProperties.name.ToString();
            chatObj.transform.GetChild(1).GetComponent<Text>().color = new Color(195f / 255f, 0f, 0f, 255f / 255f);
            chatObj.transform.GetChild(2).GetComponent<Text>().text = mess;
            if (WaitingRoomUI.Instance.contentChatArea.childCount >= 20)
            {
                Destroy(WaitingRoomUI.Instance.contentChatArea.GetChild(0).gameObject);
            }
            WaitingRoomUI.Instance.inputChat.text = "";
            StartCoroutine(ScollDownToBottom());
        }

    }

    public void RefreshRoom()
    {
        Debug.Log("refresh room");
        this.PostEvent(EventID.OnDisconnectSocketIO);
        if (CharacterManager.Instance._enemyCharacter != null)
        {
            Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
            CharacterManager.Instance._enemyCharacter = null;
        }
        this.PostEvent(EventID.OnLoginSocketIO);
        //UpdateWatingRoomUI();
    }


    IEnumerator ScollDownToBottom()
    {
        yield return new WaitForSeconds(0.01f);
        WaitingRoomUI.Instance.ScrollChat.verticalNormalizedPosition = 0f;
    }

    private void OnDestroy()
    {
        isInWaitingRoom = false;
        SocketIOController.Instance.DisableListenersInWaitingRoom();
        EventDispatcher.Instance.RemoveListener(EventID.OnInitMECharacter, _InitMECharacterEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.OnInitEnemyCharacter, _InitEnemyCharacterEventRef);
        EventDispatcher.Instance.RemoveListener(EventID.RefreshRoom, _RefreshRoom);
    }
}
