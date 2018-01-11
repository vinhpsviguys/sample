using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP;
using BestHTTP.SocketIO;
using SimpleJSON;
using System.Linq;

public class LibSocketIO : MonoBehaviour {
    //Instance
    //public static LibSocketIO instance;

    //Set Namespace For Current Player
    public string namespaceForCurrentPlayer = "";

    //SocketManager Reference
    public SocketManager socketManagerRef;

    //Socket Reference
    public Socket mySocket;

    //Options
    SocketOptions options;

    #region PRIVATE_VARS
    public bool checkEnterRoom = false;
    public String idRoom = "";
    public String nameRoom = "";
    public String namePlayer = "";
    public String iduser = "";
    //=== Emit Events ===//

    //Leave Room After Quite The Game
    void OnApplicationQuit()
    {
        mySocket.Emit("exitconnect");
        mySocket.Off();
        DisconnectMySocket();
    }

    #endregion

    #region PUBLIC_METHODS
    //init connect server
    public void Connect()
    {
        TimeSpan miliSecForReconnect = TimeSpan.FromMilliseconds(1000);

        options = new SocketOptions();
        options.ReconnectionAttempts = 3;
        options.AutoConnect = true;
        options.ReconnectionDelay = miliSecForReconnect;

        //Server URI
        socketManagerRef = new SocketManager(new Uri("http://45.32.106.62:2017/socket.io/"), options);
        //socketManagerRef = new SocketManager(new Uri("http://127.0.0.1:3000/socket.io/"), options);

    }
    //set name space return new my socket
    public Socket SetNamespaceSocket(string socketNamespace)
    {
        namespaceForCurrentPlayer = socketNamespace;
        mySocket = socketManagerRef.GetSocket(namespaceForCurrentPlayer);
        
        Debug.Log("ket noi namespace " + socketNamespace);
        return mySocket;
    }

    //// LOGIN ////
    //set name user + spacegame
    public void OnLogin(String namePlayerUser, String spaceGame)
    {
        //SetNameAndConnetServer();
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("name", namePlayerUser);
        gameData.Add("gamespace", spaceGame);

        mySocket.Emit("set_name", OnSendEmitDataToServerCallBack, gameData);
        namePlayer = namePlayerUser;
        Debug.Log("gui name");
    }
    private void SetNameAndConnetServer()
    {
        mySocket.Once("name_set", OnNameSet);
    }
    void OnNameSet(Socket socket, Packet packet, params object[] args)
    {
        var data = args[0] as Dictionary<string, object>;
        var username = data["name"] as string;
        iduser = data["iduser"] as string;
        Debug.Log("Da xet name: " + username);
    }

    //// ROOM : JOINT AND CREATE ////
    //send create name room
    public void CreateRoomInName(RoomSetting settings)
    {
        nameRoom = settings.nameRoom;
        //String maxUsers = settings.maxUsers.ToString();
        int maxUsers = settings.maxUsers;

        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("name", nameRoom);
        gameData.Add("maxuser", maxUsers);
        gameData.Add("owncreate", namePlayer);
        mySocket.Emit("create_room", gameData);
    }

    //send joint name room
    public void JointRoomInName(string nameRoom, string idRoom)
    {
      
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("name", nameRoom);
        gameData.Add("idroom", idRoom);
        gameData.Add("owncreate", namePlayer);
        mySocket.Emit("joint_room", gameData);
        Debug.Log("joint room " + nameRoom + " id room " + idRoom + " nameplayer " + namePlayer);
    }

    //listen user enter room
    public void OffLitenUserEnterRoom()
    {
        mySocket.Off("user_entered");
        mySocket.Off("local_entered");
        mySocket.Off("user_disconnect");
    }
    public void OnLitenUserEnterRoom()
    {
        GetPlayerEnterRoom();
        GetLocalEnterRoom();
        GetPlayerDisconnect();
    }
    public void GetPlayerEnterRoom()
    {
        mySocket.On("user_entered", OnEnterRoom);
    }
    void OnEnterRoom(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("User Enter : ");
        var data = args[0] as Dictionary<string, object>;
        var username = data["name"] as string;
        Debug.Log("User Enter Room: " + username);
    }

    public void GetLocalEnterRoom()
    {
        mySocket.On("local_entered", OnLocalEnterRoom);
    }
    void OnLocalEnterRoom(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Ban Da vao Room Local Enter Room");
        var data = args[0] as Dictionary<string, object>;
        idRoom = data["idroom"] as string;
        nameRoom = data["nameroom"] as string;

        Debug.Log("Room id: " + idRoom);
        checkEnterRoom = true;
    }

    public void GetPlayerDisconnect()
    {
        mySocket.On("user_disconnect", OnUserDisconnect);
    }
    void OnUserDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("User Disconnect : ");
        var data = args[0] as Dictionary<string, object>;
        var username = data["name"] as string;
        Debug.Log("User Disconnect Room: " + username);
    }

    //Send Leave Room Data
    public void LeaveRoom()
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("name", namePlayer);
        gameData.Add("idroom", idRoom);
        gameData.Add("nameroom", nameRoom);
        mySocket.Emit("leaveuser", gameData);
        Debug.Log("leaveRoom " + nameRoom);
        WaitingRoomUI.Instance.SetLog("leaveRoom " + nameRoom);
        //leave var reset
        checkEnterRoom = false;
        idRoom = "";
        nameRoom = "";
    }
    // On listen get user leave room
    public void OnLitenUserLeaveRoom()
    {
        mySocket.On("user_leave", OnLeaveRoomData);
    }
    private void OnLeaveRoomData(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Leave Room");
    }

    //On listen List Room
    public void OnListenListRoom()
    {
        
        mySocket.On("rooms_list", OnListRoom);
    }    
    void OnListRoom(Socket socket, Packet packet, params object[] args)
    {
       
        var dataOjb = args[0] as Dictionary<string, object>;
        List<object> data = dataOjb["listroom"] as List<object>;
        foreach (object ojbRoom in data)
        {
            var bb = ojbRoom as Dictionary<string, object>;
            Debug.Log(bb["name"]);
            Debug.Log(bb["id"]);
            Debug.Log(bb["own"]);
        }
    }
    //Function client
    public void InitListRoomsGame() // gui cai nay lan dau de nhan list room, dat no sau onlisten list room
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("namespace", namespaceForCurrentPlayer);
        mySocket.Emit("get_rooms", gameData);
        Debug.Log("gui yeu cau list room");
    }

    // 

    #endregion

    ///// REMOVE LISTEN    
    public void ExitConnect(BestHTTP.SocketIO.Events.SocketIOAckCallback ExitConnectCallBack)
    {
        mySocket.Emit("exitconnect", ExitConnectCallBack);
    }

    public void SetNamespaceForChat(string socketNamespace)
    {
        namespaceForCurrentPlayer = socketNamespace;
        mySocket = socketManagerRef.GetSocket(namespaceForCurrentPlayer);
        Debug.Log("ket noi chat" + socketNamespace);
        //Set All Events, When Join The New Room
        //Connect
        mySocket.On("connect", OnConnect);
        //Disconnect
        mySocket.On("disconnect", OnDisconnect);
    }

    public void SetNamespaceForSocket(string socketNamespace)
    {
        namespaceForCurrentPlayer = socketNamespace;
        mySocket = socketManagerRef.GetSocket(namespaceForCurrentPlayer);
        Debug.Log("ket noi " + socketNamespace);
    }

    //Send User Action Data
    public void SendGameActionDataToServer(float objXPos, float objYPos, string senderDeviceUniqueId)
    {
        Dictionary<string, object> gameActionData = new Dictionary<string, object>();
        gameActionData.Add("objXPos", objXPos);
        gameActionData.Add("objYPos", objYPos);
        gameActionData.Add("senderDeviceUniqueId", senderDeviceUniqueId);

        mySocket.Emit("action", OnSendEmitDataToServerCallBack, gameActionData);
    }

// FUNCTION
    //Disconnect My Socket
    public void DisconnectMySocket()
    {
        mySocket.Disconnect();
    }

    
    public void ListClientInRoom()
    {
        Dictionary<string, object> gameData = new Dictionary<string, object>();
        gameData.Add("name", namePlayer);
        gameData.Add("idroom", idRoom);
        gameData.Add("nameroom", nameRoom);
        mySocket.Emit("client:room", gameData);
    }




// VIDU _RAC
    //listen Event
    private void listenOnClientFromServer()
    {
        SetNameAndConnetServer();
        //GetListRoom();
        GetLocalEnterRoom();
        GetPlayerEnterRoom();
    }
    private void SetAllEvents()
    {

        //Get UserAction Data From Server
        mySocket.On("action", OnGetActionData);

        //Leave Room
        //mySocket.On("leave", OnLeaveRoomData);

        //Connect
        mySocket.On("connect", OnConnect);

        //Re-Connect
        mySocket.On("reconnect", OnReConnect);

        //Re-Connecting
        mySocket.On("reconnecting", OnReConnecting);

        //Re-Connect Attempt
        mySocket.On("reconnect_attempt", OnReConnectAttempt);

        //Re-Connect Attempt
        mySocket.On("reconnect_failed", OnReConnectFailed);

        //Disconnect
        mySocket.On("disconnect", OnDisconnect);

        mySocket.On(SocketIOEventTypes.Error, OnError);


    }

    //=== On Event's Methods ===//

    private void OnConnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Connect...");
    }

    public void OnReConnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Re-Connect...");
    }

    public void OnReConnecting(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Re-Connecting...");
    }

    public void OnReConnectAttempt(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Re-Connect Attempt...");
    }

    public void OnReConnectFailed(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Re-ConnectFailed...");
    }

    //Get User Action Data
    public void OnGetActionData(Socket socket, Packet packet, params object[] args)
    {
        var res = JSON.Parse(packet.ToString());
    }
    //Disconnect From Room
    public void OnDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Disconnect...");
    }
    //=== Emit Event's Methods ===//
    private void OnSendEmitDataToServerCallBack(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Send Packet Data : " + packet.ToString());
    }

    public void OnError(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("vao loi Exception!");
        Error error = args[0] as Error;

        switch (error.Code)
        {
            case SocketIOErrors.User:
                Debug.Log("Exception in an event handler!");
                break;
            case SocketIOErrors.Internal:
                Debug.Log("Internal error!");
                break;
            default:
                Debug.Log("Server error!");
                break;
        }

        Debug.Log(error.ToString());
    }

}

public class RoomSetting{

    public int maxUsers { get; set; }
    public string nameRoom { get; set; }

}
