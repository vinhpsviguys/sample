
using UnityEngine;
[System.Serializable]
public class MyAccount
{
    public string idu;
    public string idcode;
    public string username;
    public string password;
    public string deviceid;
    public string name;
    public string email;
    public string avatar;
    public string level;
    public string guide;
    public string rank;
    public string server;
    public string idfb;
    public string os;
    public string idhplayed;
    public string active;
    public string token;
    public string created_at;
    public string updated_at;

    public static MyAccount CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MyAccount>(jsonString);
    }
}
