using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipItemReward
{
    public string idh;
    public string idcode;
    public string idie;
    public string type;
    public string listidproperty;
    public string timemili;
    public string idit;

    public static EquipItemReward CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<EquipItemReward>(jsonString);
    }
}