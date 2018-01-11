
using UnityEngine;
[System.Serializable]
public class HeroCurrentPlay{

    public string idh;
    public string idcode;
    public string idu;
    public string idih;
    public int idclass;
    public string name;
    public string thumb="";
    public string level;
    public string turnbase;
    public string heart;
    public string energy;
    public string exp;
    public string skillpoint;
    public string gold;
    public string diamond;
    public string slotchest;
    public string type;
    public string hp;
    public string point;
    public string levelmap;
    public string strength;
    public string intelligence;
    public string vitality;
    public string dexterity;
    public string focus;
    public string immortality;
    public string luck;
    public string endurance;
    public string blessing;
    public string fighting;
    public string rank;
    public string id_weapon;
    public string id_head;
    public string id_shield;
    public string id_gloves;
    public string id_boots;
    public string id_torso;
    public string id_belt;
    public string id_leg;
    public string id_ring;
    public string id_amulet;
    public string id_buff;
    public string id_avatar;
    public string timemili;

    public static HeroCurrentPlay CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HeroCurrentPlay>(jsonString);
    }
}
