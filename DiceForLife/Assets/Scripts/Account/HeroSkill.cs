using UnityEngine;

public class HeroSkill
{
    public string idhk;
    public string idh;
    public string idcode;
    public string idk;
    public string point;
    public string typewear;
    public string timemili;
    public string name;
    public string description;
    public string thumb;
    public string forclass;

    public static HeroSkill CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HeroSkill>(jsonString);
    }
}
