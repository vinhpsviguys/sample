using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO;

public class SocketInitConnection : MonoBehaviour {

    //Socket Reference
    private static SocketManager socketManagerRef;
    private static Socket mySocket;

    private static string namePlayer = "";
    private static string idUser = "";
    private static string nameRoom = "";
    private static string idRoom = "";
    private static string gameSpace = "";

    public static SocketManager Connection
    {
        get
        {
            return socketManagerRef;
        }
        set
        {
            socketManagerRef = value;
        }
    }

    public static Socket getSocketCurrent
    {
        get{ return mySocket; }
        set { mySocket = value; }
    }

    public static string namePlayerCurrent
    {
        get { return namePlayer; }
        set { namePlayer = value; }
    }

    public static string idUserCurrent
    {
        get { return idUser; }
        set { idUser = value; }
    }


    public static string nameRoomCurrent
    {
        get { return nameRoom; }
        set { nameRoom = value; }
    }

    public static string idRoomCurrent
    {
        get { return idRoom; }
        set { idRoom = value; }
    }

    public static string gameSpaceCurrent
    {
        get { return gameSpace; }
        set { gameSpace = value; }
    }

    public static bool IsInitialized
    {
        get
        {
            return (socketManagerRef != null);
        }
    }
}
