
using UnityEngine;
[System.Serializable]
public class ClassCharacterInit
{
    public int idcl;
    public string name;
    public string weapon;
    public string typeattack;
    public string damage;
    public string magic;
    public string twohand;
    public string thumb;

    public static ClassCharacterInit CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ClassCharacterInit>(jsonString);
    }
}
