using UnityEngine;
[System.Serializable]

public class HeroInit {

    public string idih;
    public int idclass;
    public string name;
    public string thumb;
    public string description;
    public string type;
    public string fighting;

    public string strength;
    public string intelligence;
    public string dexterity;
    public string vilality;
    public string skilldefault;

    public static HeroInit CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HeroInit>(jsonString);
    }
}
