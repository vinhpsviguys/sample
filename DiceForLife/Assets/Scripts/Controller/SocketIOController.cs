using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO;
using UnityEngine.UI;
using System;
using System.Linq;
using CoreLib;

public class SocketIOController : MonoBehaviour
{

    public static SocketIOController Instance;

    public static LibSocketIO sfs;
    private string gameSpace = "KimCuong";
    private bool hasRoomReady = false;

    public string roomName = "";
    public string idName = "";
    //LinhVT
    public bool isRefresh = false;
    //
    bool hasCreatedEnemy = false;
    public bool isReconnect = false;

    Action<object> _OnCreateTimeoutLoadingData, _OnInitListRoomEventRef, _OnLeaveRoomEventRef, _SendStartBattleRequestEventRef, _sendMessageEventRef, _ClearAllListenSocketIOEventRef, _OnReconnectBattleScene;

    public const string REQUEST_INFO_OF_ENEMY = "RequestInfoOfEnemy";
    public const string REPONSE_INFO = "ResponseInfo";

    public const string SEND_INFODATA = "SendInfoData";
    public const string SEND_CHOOSEUSERENTERED = "SendChooseUserEntered";
    public const string PARSE_INFODATA = "ResponseInfoData";
    public const string REQUEST_CONFIRM_LOADING = "RequestConfirmOfEnemy";
    public const string RESPONSE_CONFIRMLOADING = "ResponseConfirmLoading";

    public Timeouter socketTimout = null;
    public bool hasSomeoneInRoom;
    public string idSocketUserEnteredRoom = "";
    public bool candicate;
    public bool loggedSocket = false;
    public ArrayList sendingQueue = new ArrayList();
    public ArrayList sendingQueueConfirmLoading = new ArrayList();

    private void Awake()
    {
        Debug.Log("Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            this.gameObject.tag = "DontDestroyObject";
        }
        else
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }


    }

    // Use this for initialization
    void Start()
    {
        loggedSocket = false;
        Application.runInBackground = true;

        _OnInitListRoomEventRef = (param) => InitListRoom();
        this.RegisterListener(EventID.InitListRoom, _OnInitListRoomEventRef);


        _OnCreateTimeoutLoadingData = (param) => CreateTimeOutForComfirmLoadingData();
        this.RegisterListener(EventID.CreateTimeoutConfirmLoadData, _OnCreateTimeoutLoadingData);

        _SendStartBattleRequestEventRef = (param) => SendStartBattleRequest();
        this.RegisterListener(EventID.StartBattle, _SendStartBattleRequestEventRef);

        _sendMessageEventRef = (param) => sendMessage((string)param);
        this.RegisterListener(EventID.OnSendMessage, _sendMessageEventRef);

        _ClearAllListenSocketIOEventRef = (param) => ClearAllListenSocketIO();
        this.RegisterListener(EventID.OnDisconnectSocketIO, _ClearAllListenSocketIOEventRef);

        _OnLeaveRoomEventRef = (param) => LeaveRoom();
        this.RegisterListener(EventID.LeaveRoom, _OnLeaveRoomEventRef);

        _OnReconnectBattleScene = (param) => LoadBattleMonsterSceneWhenReconnect();
        this.RegisterListener(EventID.ReconnectBattleScene, _OnReconnectBattleScene);
        OnConnectGame();

    }

    // refresh the list room in the waitting room
    // author: LinhVT
    private void Update()
    {
        if (socketTimout != null)
        {
            socketTimout.update();
        }
        if (sendingQueue.Count > 0 && candicate)
        {
            for (int i = 0; i < sendingQueue.Count; i++)
            {
                var data = sendingQueue[i] as Dictionary<string, object>;
                var idUser = data["idsocketuserfrom"] as string;
                SendInfoDataOfSlave(idUser);
                sendingQueue.Clear();
                break;
                //sendingQueue.RemoveAt(i);
                //i--;
            }
        }
        if(sendingQueueConfirmLoading.Count >0 && WatingRoomController.Instance.state_waitingroom == STATEINWAITING.CONFIRM_LOADDATA)
        {
            for (int i = 0; i < sendingQueueConfirmLoading.Count; i++)
            {
                var data = sendingQueueConfirmLoading[i] as Dictionary<string, object>;
                WaitingRoomUI.Instance.SetLog("Master nhan requets confirm loading thanh cong tu slave");
                SendConfirmLoadingData();
                WatingRoomController.Instance.isInWaitingRoom = false;
                SceneLoader._instance.LoadScene(4);
                WaitingRoomUI.Instance.SetLog("Master vao scene danh nhau");
                //Application.LoadLevel("BattleMonster");
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    WatingRoomController.Instance.reconnectTimout = null;
                    WaitingPanelScript._instance.ShowWaiting(false);
                });
                sendingQueueConfirmLoading.Clear();
                break;
                //sendingQueue.RemoveAt(i);
                //i--;
            }
        }
    }


    public void OnConnectGame()
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        sfs = new LibSocketIO();
        sfs.Connect();

        sfs.SetNamespaceSocket("/");
        sfs.mySocket.Once("name_set", OnNameSetLogin);
        Debug.Log("login" + CharacterInfo._instance._baseProperties.name);
        sfs.OnLogin(CharacterInfo._instance._baseProperties.name, gameSpace);

        socketTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
        {
            if (times < timesLimit)
            {
            }
            else
            {

                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    socketTimout = null;
                    Application.Quit();
                });
            }

        });
    }
    void OnNameSetLogin(Socket socket, Packet packet, params object[] args)// when login game
    {
        var data = args[0] as Dictionary<string, object>;
        var username = data["name"] as string;
        var iduser = data["iduser"] as string;
        Debug.Log("idsocketuser" + data["idsocketuser"] as string);
        SocketInitConnection.Connection = sfs.socketManagerRef;
        SocketInitConnection.getSocketCurrent = sfs.mySocket;
        SocketInitConnection.namePlayerCurrent = username;
        SocketInitConnection.idUserCurrent = iduser;
        CharacterInfo._instance.idUserSocketIO = SocketInitConnection.idUserCurrent;

        SocketInitConnection.gameSpaceCurrent = "SpaceGame:" + gameSpace; // tren server them tien to SpaceGame: nen bat buoc pai cong them vao
        Debug.Log(SocketInitConnection.gameSpaceCurrent);


        // clear All Listeners
        sfs.mySocket.Off();
        // cmd for all user in room
        sfs.mySocket.On("user_cmd", OnCmdVariable);
        //Debug.Log("lang nghe cmd toan phong");
        //// private cmd
        sfs.mySocket.On("private_cmd", OnCmdPrivateVariable);
        //Debug.Log("lang nghe private cmd");
        ////gui cmd chi 1 string, ko pai gui dang data nhu tren
        sfs.mySocket.On("message_cmd", OnCmdMessage); // lang nghe su kien cmd_message 

        //gui chat voi nhau
        sfs.mySocket.On("message_user", OnLitenMessage);

        sfs.mySocket.On("disconnect", OnDisconnect);

        sfs.mySocket.On(SocketIOEventTypes.Error, OnError);

        Debug.Log("Login thanh cong");
        loggedSocket = true;
        socketTimout = null;
        WaitingPanelScript._instance.ShowWaiting(false);
    }

    void CreateInfoPlayOnSocketIO()
    {


        Dictionary<string, object> dataInfoPlayer = new Dictionary<string, object>();
        dataInfoPlayer.Add("idPlayer", CharacterManager.Instance._meCharacter._baseProperties.idHero);

        sfs.mySocket.Emit("addmoreinfo_user", dataInfoPlayer);

        CharacterInfo._instance.idUserSocketIO = SocketInitConnection.idUserCurrent;
    }

    public void OnRefreshListRoom()
    {
        Debug.Log("OnRefreshListRoom" + isRefresh + " " + (sfs) + " " + SocketIOController.sfs);
        if (!isRefresh)
        {
            isRefresh = true;

            if (CharacterManager.Instance._meCharacter.keyPlayer == 1) // means that Player has my own room
            {
                Debug.Log("LeaveRoom" + isRefresh + " " + (sfs) + " " + SocketIOController.sfs);
                sfs.LeaveRoom();
                sfs.InitListRoomsGame();
            }
            else
            {
                Debug.Log("InitListRoomsGame" + isRefresh + " " + (sfs) + " " + SocketIOController.sfs);
                sfs.InitListRoomsGame();
            }
        }
    }

    public void EnableListenersInWaitingRoom()
    {

        Debug.Log("EnableListenersInWaitingRoom");
        sfs.mySocket.On("rooms_list", AutoJoinRoom);

        //listen cannot joint room
        sfs.mySocket.On("room_cannot_joint", cannotJointRoom);

        //listen joint room
        sfs.mySocket.On("room_success_joint", successJointRoom);

        sfs.mySocket.On("changepos_user", OnUserVariableUpdate);  // Lang nghe thong tin va thay doi thong tin ban gui cho toi de hien thị

        sfs.mySocket.On("user_ineedupostion", SendPostionForNewUser); // Co mot nguoi vua moi vao dang can thong tin cua toi, ma muon toi gui thong tin cho ho -> ( chinh la lenh uservariable_inroom ), khi send hay dung lenh user_changepos

        //sfs.mySocket.Emit("uservariable_inroom"); // Yeu cau: hay gui cho toi cac thong tin cua ban o trong room nay : chi gui 1 lan khoi tao

        //Lang nghe su thay doi ve chu phong
        sfs.mySocket.On("user_key_room", OnSetKeyRoom);

        //lang nghe playgame de vao tran
        sfs.mySocket.On("user_playandclosejoint", OnPlayGame);

        sfs.mySocket.On("user_entered", OnAnotherUserEnterRoom);
        //Event message
        //gui cmd : giao tiep dong bo voi mn trong room


        //destroy waitting player leave or dis
        sfs.mySocket.On("user_leave", OnLeaveWaittingPlayer); // lang nghe co ngoi nao thoat
        sfs.mySocket.On("user_disconnect", OnDisconnected);
    }

    void DisableListenersWhenJoiningGame()
    {
        sfs.mySocket.Off("room_list");
    }

    void EnableListernsWhenLeavingGame()
    {
        sfs.mySocket.On("room_list", AutoJoinRoom);
    }

    public void DisableListenersInWaitingRoom()
    {
        Debug.Log("DisableListenersInWaitingRoom");

        sfs.mySocket.Off("rooms_list");

        //listen cannot joint room
        sfs.mySocket.Off("room_cannot_joint");

        //listen joint room
        sfs.mySocket.Off("room_success_joint");

        sfs.mySocket.Off("changepos_user", OnUserVariableUpdate);  // Lang nghe thong tin va thay doi thong tin ban gui cho toi de hien thị

        sfs.mySocket.Off("user_ineedupostion", SendPostionForNewUser); // Co mot nguoi vua moi vao dang can thong tin cua toi, ma muon toi gui thong tin cho ho -> ( chinh la lenh uservariable_inroom ), khi send hay dung lenh user_changepos

        //sfs.mySocket.Emit("uservariable_inroom"); // Yeu cau: hay gui cho toi cac thong tin cua ban o trong room nay : chi gui 1 lan khoi tao

        //Lang nghe su thay doi ve chu phong
        sfs.mySocket.Off("user_key_room", OnSetKeyRoom);

        //lang nghe playgame de vao tran
        sfs.mySocket.Off("user_playandclosejoint", OnPlayGame);

        //Event message
        //gui cmd : giao tiep dong bo voi mn trong room

        sfs.mySocket.Off("user_entered", OnAnotherUserEnterRoom);
        //destroy waitting player leave or dis
        sfs.mySocket.Off("user_leave", OnLeaveWaittingPlayer); // lang nghe co ngoi nao thoat
        sfs.mySocket.Off("user_disconnect", OnDisconnected);
    }

    public void EnableListenersInBattleScene()
    {
        Debug.Log("EnableListenersInBattleScene");

        sfs.mySocket.On("user_key_room", OnSetKeyRoom);
        sfs.mySocket.On("user_leave", OnLeaveWaittingPlayer); // lang nghe co ngoi nao thoat
        sfs.mySocket.On("user_disconnect", OnDisconnected);
    }

    public void DisableListenersInBattleScene()
    {
        Debug.Log("DisableListenersInBattleScene");
        sfs.mySocket.Off("user_key_room");
        sfs.mySocket.Off("user_leave"); // lang nghe co ngoi nao thoat
        sfs.mySocket.Off("user_disconnect");
    }

    void InitListRoom()
    {
        WaitingPanelScript._instance.ShowWaiting(true);
        sfs.InitListRoomsGame();
        WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
        {
            if (times < timesLimit)
            {
            }
            else
            {

                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    WatingRoomController.Instance.reconnectTimout = null;
                    WaitingPanelScript._instance.ShowWaiting(false);
                    WaitingRoomUI.Instance.BackToMainMenu();
                });
            }

        });
    }

    private void AutoJoinRoom(Socket socket, Packet packet, params object[] args)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            WatingRoomController.Instance.reconnectTimout = null;
            WaitingPanelScript._instance.ShowWaiting(false);
        });
        hasRoomReady = false;
        if (WatingRoomController.Instance.isInWaitingRoom && !isReconnect)
        {
            Debug.Log("autojoinroom");
            WaitingRoomUI.Instance.SetLog("autojoinroom");
            var dataObj = args[0] as Dictionary<string, object>;
            string gameSpaceServer = "";
            if (dataObj.ContainsKey("spacegame")) { gameSpaceServer = dataObj["spacegame"] as string; }
            if (gameSpaceServer == SocketInitConnection.gameSpaceCurrent)
            {

                List<object> data = dataObj["listroom"] as List<object>;
                Debug.Log("number room: " + data.Count);
                WaitingRoomUI.Instance.SetLog("number room: " + data.Count);
                foreach (object objRoom in data)
                {
                    var bb = objRoom as Dictionary<string, object>;
                    int countPlayer = 0;
                    string state = "";
                    if (bb.ContainsKey("countplayer")) { countPlayer = int.Parse(bb["countplayer"] as string); }
                    if (bb.ContainsKey("state")) { state = bb["state"] as string; }

                    Debug.Log(bb["name"]);
                    Debug.Log(bb["countplayer"]);
                    Debug.Log(bb["state"]);
                    bool roomOk1 = (PlayerPrefs.GetString("idroom", "") == bb["idroom"] as string) && isRefresh;
                    bool roomOk2 = (countPlayer == 1 && state == "open") && !isRefresh && bb["name"] as string != CharacterManager.Instance._meCharacter._baseProperties.name;
                    Debug.Log("room ok 1 " + roomOk1 + " room ok 2" + roomOk2);
                    if (roomOk1 || roomOk2)// change condition of hasing room
                    {
                        if (bb.ContainsKey("name")) { roomName = bb["name"] as string; }
                        if (bb.ContainsKey("idroom")) { idName = bb["idroom"] as string; }
                        hasRoomReady = true;
                        break;
                        //sfs.JointRoomInName(roomName, idName);
                    }
                }
                // not check : if room don't exist, hidden reconnect button and not create room


                if (isRefresh)
                {
                    // first time refresh list room for update UI
                    Debug.Log("Refresh UI");
                    isRefresh = false;
                    PlayerPrefs.SetString("idroom", hasRoomReady ? PlayerPrefs.GetString("idroom", "") : "");
                    UnityMainThreadDispatcher.Instance().Enqueue(() => {
                        WatingRoomController.Instance.reconnectTimout = null;
                        WaitingPanelScript._instance.ShowWaiting(false);
                        WaitingRoomUI.Instance.UpdateUI();
                    });
                }
                else //create or join room
                    AutoCreateOrJoinRoom();
            }
        }
    }

    public void CreateRoom(string _name)
    {
        //Create the Room
        RoomSetting settings = new RoomSetting();
        settings.nameRoom = _name;
        settings.maxUsers = 2;

        sfs.CreateRoomInName(settings);
        //ClearListenCreatOrJoinRoom();

        //set key
        //PlayerLocalInit.keyPlayerSet = 1;
    }

    public void LeaveRoom()
    {
        hasCreatedEnemy = false;
        hasRoomReady = false;
        sfs.LeaveRoom();
        WaitingPanelScript._instance.ShowWaiting(true);
        WatingRoomController.Instance.reconnectTimout = new Timeouter(10 * 1000, 1, (long times, int timesLimit) =>
        {
            if (times < timesLimit)
            {
            }
            else
            {

                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    WatingRoomController.Instance.reconnectTimout = null;
                    WaitingPanelScript._instance.ShowWaiting(false);
                    WaitingRoomUI.Instance.BackToMainMenu();
                });
            }

        });
        EnableListernsWhenLeavingGame();
        WatingRoomController.Instance.state_waitingroom = STATEINWAITING.NONE;



    }

    public void AutoCreateOrJoinRoom()
    {
        WaitingRoomUI.Instance.SetLog("has room ready " + hasRoomReady);
        if (hasRoomReady)
        {
            WatingRoomController.Instance.state_waitingroom = STATEINWAITING.REQUEST_ID;
            Debug.Log("join room");
            WaitingRoomUI.Instance.SetLog("join room");
            sfs.JointRoomInName(roomName, idName);
            //WaitingRoomUI.Instance.SetLog("join in to room name: " + roomName +" room id: "+ idName + "successfully");
            CharacterInfo._instance.keyPlayer = 0;
            CharacterManager.Instance._meCharacter.keyPlayer = 0;
            //sfs.mySocket.Emit("uservariable_inroom"); // Yeu cau: hay gui cho toi cac thong tin cua ban o trong room nay : chi gui 1 lan khoi tao

            //Dictionary<string, object> dataLocalPosition = new Dictionary<string, object>();
            //dataLocalPosition.Add("key", CharacterManager.Instance._meCharacter.keyPlayer);
            //dataLocalPosition.Add("nameplayer", CharacterManager.Instance._meCharacter._baseProperties.name);
            //dataLocalPosition.Add("idplayer", CharacterManager.Instance._meCharacter._baseProperties.idHero);
            //dataLocalPosition.Add("idCode", CharacterManager.Instance._meCharacter._baseProperties.idCodeHero);
            //Debug.Log("gui data cua nguoi moi vao");
            //WaitingRoomUI.Instance.SetLog("gui data cua nguoi moi vao ");
            //send cho nguoi yeu cau vi tri cua minh trong phòng ( vao sau )
            //sfs.mySocket.Emit("user_changepos", dataLocalPosition);
            // create timeout of slave for waiting response id of master

        }
        else
        {
            WatingRoomController.Instance.state_waitingroom = STATEINWAITING.REQUEST_ID;
            Debug.Log("create room");
            WaitingRoomUI.Instance.SetLog("create room");
            CreateRoom(CharacterManager.Instance._meCharacter._baseProperties.name);
            CharacterInfo._instance.keyPlayer = 1;
            CharacterManager.Instance._meCharacter.keyPlayer = 1;
            //WaitingRoomUI.Instance.canCreateRoom = false;
            //WaitingPanelScript._instance.ShowWaiting(false);
        }
    }


    public void CreateTimeOutForComfirmLoadingData()
    {
        WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
        {
            if (times < timesLimit)
            {
                if (CharacterInfo._instance.keyPlayer == 0)
                {
                    WaitingRoomUI.Instance.SetLog("Send confirm loading data from slave");
                    RequestConfirmLoadingData();
                }
            }
            else
            {

                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    WatingRoomController.Instance.reconnectTimout = null;
                    WaitingPanelScript._instance.ShowWaiting(false);
                    WaitingRoomUI.Instance.BackToMainMenu();
                });
            }

        });
    }

    public void SendStartBattleRequest()
    {
        SendReadyConfirmFromHost();
    }



    public void OnUserVariableUpdate(Socket socket, Packet packet, params object[] args)
    {
        if (!hasCreatedEnemy)
        {
            Debug.Log("Lang nghe ban gui cho toi");
            int idplayer = 0;
            string iduser = "";
            string idcode = "";
            var player = args[0] as Dictionary<string, object>;
            if (player.ContainsKey("idplayer")) { idplayer = Convert.ToInt32(player["idplayer"]); }
            if (player.ContainsKey("iduser")) { iduser = player["iduser"] as string; }
            if (player.ContainsKey("idCode")) { idcode = player["idCode"] as string; }
            Debug.Log(CharacterManager.Instance);
            CharacterManager.Instance.CreateEnemyProperties(idplayer, idcode);
            hasCreatedEnemy = true;
        }

    }

    private void SendPostionForNewUser(Socket socket, Packet packet, params object[] args)
    {

        var data = args[0] as Dictionary<string, object>;

        string iduserneed = "";
        if (data.ContainsKey("iduserneed")) { iduserneed = data["iduserneed"] as string; } // co yeu cau send position cua ban cho nguoi khac

        if (iduserneed != "") // ton tai id nguoi yeu cau gui
        {
            Dictionary<string, object> dataLocalPosition = new Dictionary<string, object>();
            dataLocalPosition.Add("key", CharacterManager.Instance._meCharacter.keyPlayer);
            dataLocalPosition.Add("nameplayer", CharacterManager.Instance._meCharacter._baseProperties.name);
            dataLocalPosition.Add("idplayer", CharacterManager.Instance._meCharacter._baseProperties.idHero);
            dataLocalPosition.Add("idCode", CharacterManager.Instance._meCharacter._baseProperties.idCodeHero);
            dataLocalPosition.Add("iduser", SocketInitConnection.idUserCurrent);
            sfs.mySocket.Emit("user_changepos", dataLocalPosition);
        }
    }
    void OnSetKeyRoom(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnSetKeyRoom");
        var data = args[0] as Dictionary<string, object>;

        string iduser = "";
        if (data.ContainsKey("iduserkey")) { iduser = data["iduserkey"] as string; }
        Debug.Log("id user set key room" + iduser);
        Debug.Log("id cua chinh minh " + SocketInitConnection.idUserCurrent);

        if (!WatingRoomController.Instance.isInWaitingRoom)
        {
            BattleSceneController.Instance.adapter.OnSetKeyRoom(iduser == SocketInitConnection.idUserCurrent);
        }

    }
    public void OnPlayGame(Socket socket, Packet packet, params object[] args)
    {
        WatingRoomController.Instance.isInWaitingRoom = false;
        //DisableListenersInWaitingRoom();
        //EnableListenersInBattleScene();
        SceneLoader._instance.LoadScene(4);
        //Application.LoadLevel("BattleMonster");

        //ClearListenWattingRoom();
        //DisableListenersInWaitingRoom();
    }
    void OnListenKickRoomPlayer(Socket socket, Packet packet, params object[] args)
    {
    }

    void OnDisconnected(Socket socket, Packet packet, params object[] args)
    {
        //ClearAllListenSocketIO();
    }

    void OnLeaveWaittingPlayer(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("OnLeaveWaittingPlayer " + WatingRoomController.Instance.isInWaitingRoom);
        Debug.Log("Is in waiting room " + WatingRoomController.Instance.isInWaitingRoom);
        WatingRoomController.Instance.isInSomeRoom = false;
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            WatingRoomController.Instance.reconnectTimout = null;
            WaitingPanelScript._instance.ShowWaiting(false);
        });

        if (WatingRoomController.Instance == null && !WatingRoomController.Instance.isInWaitingRoom)
        {
          
            var data = args[0] as Dictionary<string, object>;

            string name = "", iduser = "";

            if (data.ContainsKey("name")) { name = data["name"] as string; }
            if (data.ContainsKey("iduser")) { iduser = data["iduser"] as string; }
            Debug.Log("id user leave" + iduser);
            Debug.Log("id cua chinh minh " + SocketInitConnection.idUserCurrent);
            BattleSceneController.Instance.adapter.OnLeavePlayer(iduser == SocketInitConnection.idUserCurrent);
            if (iduser == SocketInitConnection.idUserCurrent)
            {
                return;
            }
            else
            {
                //BattleSceneController.Instance.ShowGameOverPanel(BattleSceneController.Instance.me.playerId);
                //Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
                //CharacterManager.Instance._enemyCharacter = null;

                //nguoi con lại trong phong se hien thi bang thong bao thang
                // cong diem, vang, do dac o day
                //se goi ham OnLeaveGameButtonClick tren bang thong bao thang
                //dialogChienthang.SetActive(true);
                //endGame = true;
                // xu ly luc dut ket noi

            }
        }
        /////vinh
        else if (WatingRoomController.Instance != null && WatingRoomController.Instance.isInWaitingRoom)
        {
            var data = args[0] as Dictionary<string, object>;

            string name = "", iduser = "";

            if (data.ContainsKey("name")) { name = data["name"] as string; }
            if (data.ContainsKey("iduser")) { iduser = data["iduser"] as string; }
            if (CharacterManager.Instance._enemyCharacter != null)
            {
                Destroy(CharacterManager.Instance._enemyCharacter.gameObject);
                CharacterManager.Instance._enemyCharacter = null;
            }
            if (CharacterManager.Instance._meCharacter.keyPlayer == 0)
            {
                CharacterManager.Instance._meCharacter.keyPlayer = 1;
            }
            //if (iduser == SocketInitConnection.idUserCurrent)
            //{
            WaitingPanelScript._instance.ShowWaiting(false);
            //WaitingRoomUI.Instance.canCreateRoom = true;
            Debug.Log("chinh minh leaver room");
            //}

            if (WatingRoomController.Instance.state_waitingroom == STATEINWAITING.DOWNLOAD_DATA || WatingRoomController.Instance.state_waitingroom == STATEINWAITING.CONFIRM_LOADDATA)
            {
                //if (iduser != SocketInitConnection.idUserCurrent)
                //{
                WaitingRoomUI.Instance.CancelFind();
                //}
            }
            // if STATE of download, if not i leave room, i leave room
            // if STATE of connect, if not i leave room, i leave room

            //this.PostEvent(EventID.OnInitEnemyCharacter);

            // refresh room after leaving my own room
            //if (isRefresh) {
            //    sfs.InitListRoomsGame();
            //}
        }


    }

    private void OnCmdPrivateVariable(Socket socket, Packet packet, params object[] args)
    {
        string type_cmd = args[0] as string;
        var data = args[1] as Dictionary<string, object>;
        WaitingRoomUI.Instance.SetLog("NHAN CMD PRIVATE" + type_cmd + " data " + data.Keys.Count);
        if (WatingRoomController.Instance.isInWaitingRoom)
        {
            switch (type_cmd)
            {
                case SEND_CHOOSEUSERENTERED:
                    if (WatingRoomController.Instance.state_waitingroom == STATEINWAITING.REQUEST_ID)
                    {
                        if (candicate)
                        {
                            WaitingRoomUI.Instance.SetLog("send tra host");
                            var idUser = data["idsocketuserfrom"] as string;
                            SendInfoDataOfSlave(idUser);
                        }
                        else
                        {
                            WaitingRoomUI.Instance.SetLog("add vao queue");
                            sendingQueue.Add(data);
                        }
                    }
                    break;
                case SEND_INFODATA:
                    if (WatingRoomController.Instance.state_waitingroom == STATEINWAITING.REQUEST_ID)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            WaitingPanelScript._instance.ShowWaiting(false);
                            WatingRoomController.Instance.reconnectTimout = null;
                        });
                        if (CharacterInfo._instance.keyPlayer == 1)
                        {
                            SendInfoDataOfHost();
                        }
                        ParseInfoData(type_cmd, data);
                    }
                    break;
            }
        }

    }


    private void OnCmdVariable(Socket socket, Packet packet, params object[] args)
    {
        string type_cmd = args[0] as string;
        var data = args[1] as Dictionary<string, object>;
        Debug.Log("NHAN CMD " + type_cmd + " data " + data.Keys.Count + "waitingroom flag " + WatingRoomController.Instance.isInWaitingRoom);
        if (WatingRoomController.Instance.isInWaitingRoom)
        {
            Debug.Log("dang o watingroom");
            switch (type_cmd)
            {
                case REQUEST_CONFIRM_LOADING:
                    if (CharacterInfo._instance.keyPlayer == 1)
                    {
                        if (WatingRoomController.Instance.state_waitingroom == STATEINWAITING.CONFIRM_LOADDATA)
                        {
                            WaitingRoomUI.Instance.SetLog("Master nhan requets confirm loading thanh cong tu slave");
                            SendConfirmLoadingData();
                            WatingRoomController.Instance.isInWaitingRoom = false;
                            SceneLoader._instance.LoadScene(4);
                            WaitingRoomUI.Instance.SetLog("Master vao scene danh nhau");
                            //Application.LoadLevel("BattleMonster");
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                WatingRoomController.Instance.reconnectTimout = null;
                                WaitingPanelScript._instance.ShowWaiting(false);
                            });
                        } else if (WatingRoomController.Instance.state_waitingroom != STATEINWAITING.CONFIRM_LOADDATA)
                        {
                            sendingQueueConfirmLoading.Add(data);
                        }
                    }
                    break;
                case RESPONSE_CONFIRMLOADING:
                    if (WatingRoomController.Instance.state_waitingroom == STATEINWAITING.CONFIRM_LOADDATA)
                    {
                        WatingRoomController.Instance.isInWaitingRoom = false;
                        WaitingRoomUI.Instance.SetLog("Slave vao scene danh nhau");
                        SceneLoader._instance.LoadScene(4);
                        //Application.LoadLevel("BattleMonster");
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            WatingRoomController.Instance.reconnectTimout = null;
                            WaitingPanelScript._instance.ShowWaiting(false);
                        });
                    }
                    break;
                case REPONSE_INFO:// slave handles
                    if (WatingRoomController.Instance.state_waitingroom == STATEINWAITING.RECONNECT)
                    {
                        WaitingRoomUI.Instance.SetLog("Slave nhan response info khi reconnect");
                        ParseInfoOfEnemy(type_cmd, data);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => {
                            WatingRoomController.Instance.reconnectTimout = null;
                            WaitingPanelScript._instance.ShowWaiting(false);
                        });
                    }
                    break;
            }
            // if Done B receiver, create timeout for confirm response from slave
            // if confirm receiver, start from Master
        }
        else
        {
            Debug.Log("khong o watingroom " + type_cmd);
            if (type_cmd != "stateplayerwaitingroom")
            {
                var idUser = data["from"] as string;
                Debug.Log("id user socket of character " + CharacterInfo._instance.idUserSocketIO);
                Debug.Log("id user " + idUser);
                if (CharacterInfo._instance.idUserSocketIO != idUser)
                {

                    switch (type_cmd)
                    {
                        case REQUEST_INFO_OF_ENEMY:// master handles
                            ReponseInfo();

                            break;
                        default:
                            if(BattleSceneController.Instance!=null)
                            BattleSceneController.Instance.adapter.receivePacket(type_cmd, MyDictionary<string, object>.convertToMyDictionary(data));
                            break;
                    }


                }
            }
        }


    }

    void OnCmdMessage(Socket socket, Packet packet, params object[] args)
    {
        // sub receiver data battle begin flag from host
        var data = args[0] as Dictionary<string, object>;

        if (CharacterManager.Instance._meCharacter.keyPlayer == 0)
        {

            string ready = "";
            if (data.ContainsKey("readyfromhost")) { ready = data["readyfromhost"] as string; }
            if (ready == "ok")
            {
                WaitingRoomUI.Instance.SetLog("ready from host ok");
                SendConfirmReadyFromSlave();
            }
        }
        else if (CharacterManager.Instance._meCharacter.keyPlayer == 1)
        {
            string confirm = "";
            if (data.ContainsKey("confirmfromslave")) { confirm = data["confirmfromslave"] as string; }
            if (confirm == "ok")
            {
                WaitingRoomUI.Instance.SetLog("confirm from slave ok");
                sfs.mySocket.Emit("playandclosejoint_user");
                WatingRoomController.Instance.isInWaitingRoom = false;
                //DisableListenersInWaitingRoom();
                //EnableListenersInBattleScene();
                //Application.LoadLevel("BattleMonster");
                SceneLoader._instance.LoadScene(4);

                //ClearListenWattingRoom();
            }
        }

    }
    public void sendMessage(string message)
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("message", message);
        sfs.mySocket.Emit("message", gameData);

    }
    void OnLitenMessage(Socket socket, Packet packet, params object[] args)
    {
        var data = args[0] as Dictionary<string, object>;
        string message = "";
        string type = "";
        Debug.Log(data);
        if (data.ContainsKey("message")) { message = data["message"] as string; }
        if (data.ContainsKey("type")) { type = data["type"] as string; }
        if (type != "me")
        {
            WatingRoomController.Instance.CreateContentEnemyChat(CharacterManager.Instance._enemyCharacter._baseProperties.idHero, message);
        }
    }


    void SendReadyConfirmFromHost()
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("readyfromhost", "ok");
        sfs.mySocket.Emit("cmd_message", gameData);
    }

    void SendConfirmReadyFromSlave()
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("confirmfromslave", "ok");
        sfs.mySocket.Emit("cmd_message", gameData);
    }

    //cannot joint room
    private void cannotJointRoom(Socket socket, Packet packet, params object[] args)
    {
        var data = args[0] as Dictionary<string, object>;

        //string name = "";
        //string nameroom = "";
        string type = "";

        //if (data.ContainsKey("name")) { name = data["name"] as string; }
        //if (data.ContainsKey("nameroom")) { nameroom = data["nameroom"] as string; }
        if (data.ContainsKey("type")) { type = data["type"] as string; }

        if (type == "error") // show popup error : ko vao dc room ~ tu lam
            Debug.Log("Ban ko vao duoc room: ");

        if (WatingRoomController.Instance.isInWaitingRoom)
        {
            PlayerPrefs.SetString("idroom", "");
            PlayerPrefs.SetString("nameroom", "");
            WaitingRoomUI.Instance.startGame.gameObject.SetActive(true);
            WaitingRoomUI.Instance.reconnectGame.gameObject.SetActive(false);
            WaitingRoomUI.Instance.abaddonGame.gameObject.SetActive(false);
        }

        EnableListernsWhenLeavingGame();
    }

    //private void someoneJointRoom(Socket socket, Packet packet, params object[] args)
    //{
    //    Debug.Log("Co ai do phong thanh cong");
    //    var data = args[0] as Dictionary<string, object>;
    //    string name = "", idroom = "", nameroom = "";

    //    if (data.ContainsKey("name")) { name = data["name"] as string; }
    //    if (data.ContainsKey("idroom")) { idroom = data["idroom"] as string; }
    //    if (data.ContainsKey("nameroom")) { nameroom = data["nameroom"] as string; }
    //}


    public void ReconnectCallback()
    {
        try
        {
            isReconnect = true;
            Debug.Log("reconnect room");
            WaitingPanelScript._instance.ShowWaiting(true);
            sfs.JointRoomInName(PlayerPrefs.GetString("nameroom"), PlayerPrefs.GetString("idroom"));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    private void OnAnotherUserEnterRoom(Socket socket, Packet packet, params object[] args)
    {
        if (isReconnect) return;
        var data = args[0] as Dictionary<string, object>;
        Debug.Log("Nguoi khac vao phong thanh cong");

        if (CharacterInfo._instance.keyPlayer == 1)
        {
            if (!hasSomeoneInRoom)
            {
                hasSomeoneInRoom = true;
                if (data.ContainsKey("idsocketuserjoint")) { idSocketUserEnteredRoom = data["idsocketuserjoint"] as string; }
                Debug.Log("id user socket vua join vao room " + idSocketUserEnteredRoom);
                SendConfirmChooseUserEntered();
                WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
                {
                    if (times < timesLimit)
                    {
                        WaitingPanelScript._instance.ShowWaiting(true);
                        //RequestInfoOfEnemy();
                    }
                    else
                    {
                        Debug.Log("Time out master");
                        UnityMainThreadDispatcher.Instance().Enqueue(() => {
                            WatingRoomController.Instance.reconnectTimout = null;
                            WaitingPanelScript._instance.ShowWaiting(false);
                            WaitingRoomUI.Instance.CancelFind();
                        });
                    }

                });

                /// gui cmd chinh xac cho thang vua moi vao
            }

        }
    }



    private void successJointRoom(Socket socket, Packet packet, params object[] args)
    {

        Debug.Log("Vao phong thanh cong");
        WatingRoomController.Instance.isInSomeRoom = true;
        var data = args[0] as Dictionary<string, object>;
        string name = "", idroom = "", nameroom = "";

        if (data.ContainsKey("name")) { name = data["name"] as string; }
        if (data.ContainsKey("idroom")) { idroom = data["idroom"] as string; }
        if (data.ContainsKey("nameroom")) { nameroom = data["nameroom"] as string; }
        Debug.Log("id room " + idroom);
        Debug.Log("name room " + nameroom);
        WaitingRoomUI.Instance.SetLog("join in to room name: " + nameroom + " room id: " + idroom + "successfully");
        PlayerPrefs.SetString("idroom", idroom);
        PlayerPrefs.SetString("nameroom", nameroom);
        //khoi tao InitConnetIO
        SocketInitConnection.nameRoomCurrent = nameroom;
        SocketInitConnection.idRoomCurrent = idroom; //sinh ra tren server
        if (isReconnect)
        {
            WaitingPanelScript._instance.ShowWaiting(false);
            WatingRoomController.Instance.state_waitingroom = STATEINWAITING.RECONNECT;
            // create timout
            // request
            WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
            {
                if (times < timesLimit)
                {
                    WaitingPanelScript._instance.ShowWaiting(true);
                    RequestInfoOfEnemy();
                }
                else
                {

                    Debug.Log("cancel to reconnect due to timeout");

                    SocketIOController.Instance.LogInWaitingUI("cancel to reconnect due to timeout");
                    SocketIOController.Instance.LoginBattleUI("cancel to reconnect due to timeout");

                    UnityMainThreadDispatcher.Instance().Enqueue(() => {
                        isReconnect = false;
                        WatingRoomController.Instance.reconnectTimout = null;
                        WaitingRoomUI.Instance.ClearIdRoom();
                        WaitingPanelScript._instance.ShowWaiting(false);
                    });
                }

            });


        }
        else
        {

            //check if master do
            // create timeout for waiting connecting request of slave
            if (CharacterInfo._instance.keyPlayer == 0)
            {
                candicate = true;
                //ReponseInfo();
                WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
                {
                    if (times < timesLimit)
                    {
                        WaitingPanelScript._instance.ShowWaiting(true);
                        //SendInfoData();
                    }
                    else
                    {
                        Debug.Log("Time out slave");
                        UnityMainThreadDispatcher.Instance().Enqueue(() => {
                            WatingRoomController.Instance.reconnectTimout = null;
                            WaitingPanelScript._instance.ShowWaiting(false);
                            WaitingRoomUI.Instance.CancelFind();
                        });
                    }

                });
            }
            else
            {
                hasSomeoneInRoom = false;
            }

        }
        ClearListenCreatOrJoinRoom();
        WaitingPanelScript._instance.ShowWaiting(false);
    }




    private void RequestConfirmLoadingData()
    {
        string type = REQUEST_CONFIRM_LOADING;
        MyDictionary<string, object> data = new MyDictionary<string, object>();
        data.Add("from", CharacterInfo._instance.idUserSocketIO);
        //socket.Emit("cmd", pack.type, pack.data);
        sfs.mySocket.Emit("cmd", type, data);
        WaitingRoomUI.Instance.SetLog("Gui confirm load data nhan vat thanh cong tu slave cho master");
    }

    private void SendConfirmLoadingData()
    {
        string type = RESPONSE_CONFIRMLOADING;
        MyDictionary<string, object> data = new MyDictionary<string, object>();
        data.Add("from", CharacterInfo._instance.idUserSocketIO);
        //socket.Emit("cmd", pack.type, pack.data);
        sfs.mySocket.Emit("cmd", type, data);
        WaitingRoomUI.Instance.SetLog("Master gui response confirm load data cho slave");
    }

    private void SendConfirmChooseUserEntered()
    {
        string type = SEND_CHOOSEUSERENTERED;
        MyDictionary<string, object> data = new MyDictionary<string, object>();
        data.Add("idsocketuserfrom", CharacterInfo._instance.idUserSocketIO);
        data.Add("idsocketuserto", idSocketUserEnteredRoom);
        sfs.mySocket.Emit("cmd_private", type, data);
        WaitingRoomUI.Instance.SetLog("send data from user" + SocketInitConnection.idUserCurrent + " to user" + idSocketUserEnteredRoom);
        WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
        {
            if (times < timesLimit)
            {
                WaitingPanelScript._instance.ShowWaiting(true);
            }
            else
            {
                Debug.Log("Time out master");
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    WatingRoomController.Instance.reconnectTimout = null;
                    WaitingPanelScript._instance.ShowWaiting(false);
                    WaitingRoomUI.Instance.CancelFind();
                });
            }

        });

    }

    private void SendInfoDataOfHost()
    {
        string type = SEND_INFODATA;
        MyDictionary<string, object> data = new MyDictionary<string, object>();
        data.Add("from", CharacterInfo._instance.idUserSocketIO);
        data.Add("idplayer", CharacterManager.Instance._meCharacter._baseProperties.idHero);
        data.Add("idCode", CharacterManager.Instance._meCharacter._baseProperties.idCodeHero);
        data.Add("idsocketuserto", idSocketUserEnteredRoom);
        sfs.mySocket.Emit("cmd_private", type, data);
    }

    private void SendInfoDataOfSlave(string idSocketUserSendTo)
    {
        string type = SEND_INFODATA;
        MyDictionary<string, object> data = new MyDictionary<string, object>();
        data.Add("from", CharacterInfo._instance.idUserSocketIO);
        data.Add("idplayer", CharacterManager.Instance._meCharacter._baseProperties.idHero);
        data.Add("idCode", CharacterManager.Instance._meCharacter._baseProperties.idCodeHero);
        data.Add("idsocketuserto", idSocketUserSendTo);
        sfs.mySocket.Emit("cmd_private", type, data);
        WatingRoomController.Instance.reconnectTimout = new Timeouter(30 * 1000, 1, (long times, int timesLimit) =>
        {
            if (times < timesLimit)
            {
                WaitingPanelScript._instance.ShowWaiting(true);
            }
            else
            {
                Debug.Log("Time out master");
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    WatingRoomController.Instance.reconnectTimout = null;
                    WaitingPanelScript._instance.ShowWaiting(false);
                    WaitingRoomUI.Instance.CancelFind();
                });
            }

        });
    }
    private void ParseInfoData(string type, Dictionary<string, object> data)
    {
        WatingRoomController.Instance.state_waitingroom = STATEINWAITING.DOWNLOAD_DATA;
        int idplayer = Convert.ToInt32(data["idplayer"]);
        string idcode = data["idCode"] as string;
        Debug.Log(CharacterManager.Instance);
        CharacterManager.Instance.CreateEnemyProperties(idplayer, idcode);
    }

    private void RequestInfoOfEnemy()
    {
        string type = REQUEST_INFO_OF_ENEMY;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("from", CharacterInfo._instance.idUserSocketIO);
        //socket.Emit("cmd", pack.type, pack.data);
        sfs.mySocket.Emit("cmd", type, data);
        WaitingRoomUI.Instance.SetLog("Slave send request yeu cau info khi reconnect");
    }
    private void ReponseInfo()
    {
        string type = REPONSE_INFO;
        MyDictionary<string, object> data = new MyDictionary<string, object>();
        data.Add("from", CharacterInfo._instance.idUserSocketIO);
        data.Add("idplayer", CharacterManager.Instance._meCharacter._baseProperties.idHero);
        data.Add("idCode", CharacterManager.Instance._meCharacter._baseProperties.idCodeHero);
        sfs.mySocket.Emit("cmd", type, data);
        SocketIOController.Instance.LogInWaitingUI("ReponseInfo");
        SocketIOController.Instance.LoginBattleUI("ReponseInfo");
    }

    private void ParseInfoOfEnemy(string type, Dictionary<string, object> data)
    {
        SocketIOController.Instance.LogInWaitingUI("RequestInfoOfEnemy " + isReconnect);
        SocketIOController.Instance.LoginBattleUI("RequestInfoOfEnemy " + isReconnect);
        if (!isReconnect) return;
        int idplayer = Convert.ToInt32(data["idplayer"]);

        string idcode = data["idCode"] as string;
        CharacterManager.Instance.CreateEnemyProperties(idplayer, idcode);

    }

    //private void ResponseConfirmLoadData()
    //{
    //    string type = REQUEST_INFO_OF_ENEMY;
    //    MyDictionary<string, object> data = new MyDictionary<string, object>();
    //    data.Add("from", CharacterInfo._instance.idUserSocketIO);
    //    //socket.Emit("cmd", pack.type, pack.data);
    //    sfs.mySocket.Emit("cmd", type, data);
    //}


    void LoadBattleMonsterSceneWhenReconnect()
    {
        isReconnect = false;
        WaitingRoomUI.Instance.SetLog("Transfer to BattleMonster");
        Debug.Log("Transfer to BattleMonster");
        SocketIOController.Instance.LogInWaitingUI("Transfer to BattleMonster");
        SocketIOController.Instance.LoginBattleUI("Transfer to BattleMonster");
        //DisableListenersInWaitingRoom();
        //EnableListenersInBattleScene();
        //Application.LoadLevel("BattleMonster");
        SceneLoader._instance.LoadScene(4);
    }

    private void successAddInfoPlayer(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Set thanh cong Info");
    }

    public void OnDisconnect(Socket socket, Packet packet, params object[] args)// disconnect when network is trouble
    {
        Debug.Log("Disconnect...");
        // Removes all event-handlers
        sfs.mySocket.Off();

    }

    public void OnError(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("vao loi Exception!");
        // Removes all event-handlers
        //sfs.mySocket.Off();
        sfs.mySocket.Disconnect();
    }


    public void ClearAllListenSocketIO()
    {
        Debug.Log("disconnect");
        sfs.mySocket.Disconnect();
    }

    public void ClearListenCreatOrJoinRoom()
    {

        DisableListenersWhenJoiningGame();
        //sfs.mySocket.Off("rooms_list");
        //sfs.mySocket.Off("room_cannot_joint");
        //sfs.mySocket.Off("room_success_joint");
    }

    public void ClearListenWattingRoom()
    {
        sfs.mySocket.Off("uservariable_inroom");
        sfs.mySocket.Off("changepos_user");
        sfs.mySocket.Off("user_ineedupostion");
        sfs.mySocket.Off("user_key_room");
        sfs.mySocket.Off("user_playandclosejoint");
        sfs.mySocket.Off("user_kickroom");
    }
    private void OnDestroy()
    {
        if (WaitingRoomUI.Instance != null)
        {
            WaitingRoomUI.Instance.SetLog("Ondestroy socket io controller");
            EventDispatcher.Instance.RemoveListener(EventID.InitListRoom, _OnInitListRoomEventRef);
            EventDispatcher.Instance.RemoveListener(EventID.LeaveRoom, _OnLeaveRoomEventRef);
            EventDispatcher.Instance.RemoveListener(EventID.StartBattle, _SendStartBattleRequestEventRef);
            EventDispatcher.Instance.RemoveListener(EventID.OnSendMessage, _sendMessageEventRef);
            EventDispatcher.Instance.RemoveListener(EventID.OnDisconnectSocketIO, _ClearAllListenSocketIOEventRef);
            EventDispatcher.Instance.RemoveListener(EventID.CreateTimeoutConfirmLoadData, _OnCreateTimeoutLoadingData);
            EventDispatcher.Instance.RemoveListener(EventID.ReconnectBattleScene, _OnReconnectBattleScene);
        }
    }

    public void LogInWaitingUI(string str)
    {
        if (WaitingRoomUI.Instance != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                WaitingRoomUI.Instance.SetLog(str);
            });

        }
    }

    public void LoginBattleUI(string str)
    {
        if (BattleSceneController.Instance != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                BattleSceneController.Instance.adapter.Log("SocketIOController", str);
            });

        }
    }

    private void OnApplicationQuit()
    {
        sfs.mySocket.Disconnect();
    }
}
