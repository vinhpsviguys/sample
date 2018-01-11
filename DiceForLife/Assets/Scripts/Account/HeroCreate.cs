
using UnityEngine;
[System.Serializable]
public class HeroCreate
{
    public string idih;
    public string iduser;
    public string idid_init;
    public int idclass;
    public string name;
    public string thumb;
    public string fighting;
    public string level;

    public string type;
    public string strength;
    public string intelligence;
    public string vitality;
    public string dexterity;
    public string skilldefault;
    public string timemili;

    public static HeroCreate CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HeroCreate>(jsonString);
    }
}
