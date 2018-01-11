using System.Collections.Generic;
using UnityEngine;

public class MappingData
{

    public static string ConvertTypeItemToString(TypeEquipmentCharacter _type)
    {
        switch (_type)
        {
            case TypeEquipmentCharacter.Head: return "00";
            case TypeEquipmentCharacter.Weapon: return "01";
            case TypeEquipmentCharacter.Shield: return "02";
            case TypeEquipmentCharacter.Torso: return "03";
            case TypeEquipmentCharacter.Leg: return "04";
            case TypeEquipmentCharacter.Gloves: return "05";
            case TypeEquipmentCharacter.Boots: return "06";
            case TypeEquipmentCharacter.Belt: return "07";
            case TypeEquipmentCharacter.Amulet: return "08";
            case TypeEquipmentCharacter.Ring: return "09";
            case TypeEquipmentCharacter.Buff: return "10";
            case TypeEquipmentCharacter.Avatar: return "11";
            case TypeEquipmentCharacter.Gem: return "12";
            case TypeEquipmentCharacter.AlchemyMaterial: return "13";
            case TypeEquipmentCharacter.LuckMaterial: return "14";
            case TypeEquipmentCharacter.HPRecovery: return "15";
            case TypeEquipmentCharacter.Scroll: return "16";
            case TypeEquipmentCharacter.VIPCard: return "17";
        }
        return "99";
    }
    public static string ConvertTypeItemToString(string _type)
    {
        switch (_type)
        {
            case "Head": return "00";
            case "Weapon": return "01";
            case "Shield": return "02";
            case "Torso": return "03";
            case "Leg": return "04";
            case "Gloves": return "05";
            case "Boots": return "06";
            case "Belt": return "07";
            case "Amulet": return "08";
            case "Ring": return "09";
            case "Buff": return "10";
            case "Avatar": return "11";
            case "Gem": return "12";
            case "AlchemyMaterial": return "13";
            case "LuckMaterial": return "14";
            case "HPRecovery": return "15";
            case "Scroll": return "16";
            case "VIPCard": return "17";
        }
        return "99";
    }
    public static TypeEquipmentCharacter ConvertStringToTypeItem(int type)
    {
        string id = type < 10 ? ("0" + type) : type.ToString();
        switch (id)
        {
            case "00": return TypeEquipmentCharacter.Head;
            case "01": return TypeEquipmentCharacter.Weapon;
            case "02":
                if (CharacterInfo._instance._baseProperties._classCharacter != ClassCharacter.Assassin)
                    return TypeEquipmentCharacter.Shield;
                else
                    return TypeEquipmentCharacter.OffhandWeapon;
            case "03": return TypeEquipmentCharacter.Torso;
            case "04": return TypeEquipmentCharacter.Leg;
            case "05": return TypeEquipmentCharacter.Gloves;
            case "06": return TypeEquipmentCharacter.Boots;
            case "07": return TypeEquipmentCharacter.Belt;
            case "08": return TypeEquipmentCharacter.Amulet;
            case "09": return TypeEquipmentCharacter.Ring;
            case "10": return TypeEquipmentCharacter.Buff;
            case "11": return TypeEquipmentCharacter.Avatar;
            case "12": return TypeEquipmentCharacter.Gem;
            case "13": return TypeEquipmentCharacter.AlchemyMaterial;
            case "14": return TypeEquipmentCharacter.LuckMaterial;
            case "15": return TypeEquipmentCharacter.HPRecovery;
            case "16": return TypeEquipmentCharacter.Scroll;
            case "17": return TypeEquipmentCharacter.VIPCard;
        }
        return TypeEquipmentCharacter.None;
    }

    public static Color32 ConvertRarelyToColor(int _rarely)
    {
        switch (_rarely)
        {
            case 0: return new Color32(4, 255, 57, 255);
            case 1: return new Color32(62, 159, 255, 255);
            case 2: return new Color32(252, 255, 65, 255);
            case 3: return new Color32(188, 0, 255, 255);
            case 4: return Color.red;
            case 5: return new Color32(255, 132, 0, 255);
        }
        return Color.white;
    }

    public static List<TypeEquipmentCharacter> GetListTypeEquiptmentByGroup(GroupEQuipment _group)
    {
        List<TypeEquipmentCharacter> _returnList = new List<TypeEquipmentCharacter>();
        if (_group == GroupEQuipment.Equipment)
        {
            _returnList.Add(TypeEquipmentCharacter.Head);
            _returnList.Add(TypeEquipmentCharacter.Weapon);
            _returnList.Add(TypeEquipmentCharacter.Shield);
            _returnList.Add(TypeEquipmentCharacter.Torso);
            _returnList.Add(TypeEquipmentCharacter.Gloves);
            _returnList.Add(TypeEquipmentCharacter.Belt);
            _returnList.Add(TypeEquipmentCharacter.Boots);
            _returnList.Add(TypeEquipmentCharacter.Leg);
            _returnList.Add(TypeEquipmentCharacter.Ring);
            _returnList.Add(TypeEquipmentCharacter.Amulet);
        }

        return _returnList;
    }


    public static TypeEquipmentCharacter ConvertIdItemToType(int typeEquipped, int typeChild, int idClass, bool isEquipped)
    {
        TypeEquipmentCharacter tempType = TypeEquipmentCharacter.None;
        switch (typeEquipped)
        {
            case 1:
                if (isEquipped)
                {
                    if (idClass == 1 || idClass == 5 || idClass == 7)
                    {
                        tempType = TypeEquipmentCharacter.OffhandWeapon;
                    }
                    else tempType = TypeEquipmentCharacter.Shield;
                }
                else tempType = TypeEquipmentCharacter.Shield;
                break;
            case 2: tempType = TypeEquipmentCharacter.Ring; break;
            case 3: tempType = TypeEquipmentCharacter.Amulet; break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11: tempType = TypeEquipmentCharacter.Weapon; break;
            case 12:
            case 13:
            case 14:
                switch (typeChild)
                {
                    case 1: tempType = TypeEquipmentCharacter.Head; break;
                    case 2: tempType = TypeEquipmentCharacter.Torso; break;
                    case 3: tempType = TypeEquipmentCharacter.Leg; break;
                    case 4: tempType = TypeEquipmentCharacter.Belt; break;
                    case 5: tempType = TypeEquipmentCharacter.Gloves; break;
                    case 6: tempType = TypeEquipmentCharacter.Boots; break;
                }
                break;
        }
        return tempType;
    }
    public static TypeEquipmentCharacter ConvertIdSlotToTypeEquipment(int idSlot, ClassCharacter classHero)
    {
        switch (idSlot)
        {
            case 0: return TypeEquipmentCharacter.Head;
            case 1: return TypeEquipmentCharacter.Weapon;
            case 2:
                if (classHero == ClassCharacter.Assassin || classHero == ClassCharacter.Wizard || classHero == ClassCharacter.Orc)
                    return TypeEquipmentCharacter.OffhandWeapon;

                if (classHero == ClassCharacter.Paladin || classHero == ClassCharacter.Cleric)
                    return TypeEquipmentCharacter.Shield;

                if (classHero == ClassCharacter.Sorceress || classHero == ClassCharacter.Marksman || classHero == ClassCharacter.Barbarian)
                    return TypeEquipmentCharacter.None;

                return TypeEquipmentCharacter.Shield;

            case 3: return TypeEquipmentCharacter.Torso;
            case 4: return TypeEquipmentCharacter.Leg;
            case 5: return TypeEquipmentCharacter.Gloves;
            case 6: return TypeEquipmentCharacter.Boots;
            case 7: return TypeEquipmentCharacter.Belt;
            case 8: return TypeEquipmentCharacter.Amulet;
            case 9: return TypeEquipmentCharacter.Ring;
            case 10: return TypeEquipmentCharacter.Buff;
            case 11: return TypeEquipmentCharacter.Avatar;
        }
        return TypeEquipmentCharacter.None;
    }

    public static ClassCharacter GetSuitableClassForWeapon(int idWeapon)
    {
        switch (idWeapon)
        {
            case 4: return ClassCharacter.Assassin;
            case 5: return ClassCharacter.Wizard;
            case 6: return ClassCharacter.Orc;
            case 7: return ClassCharacter.Barbarian;
            case 8: return ClassCharacter.Sorceress;
            case 9: return ClassCharacter.Marksman;
            case 10: return ClassCharacter.Cleric;
            case 11: return ClassCharacter.Paladin;
        }
        return ClassCharacter.None;
    }

    public static int ConvertIdBuffToAttribute(int id)
    {
        int _idRate = 0;
        switch (id)
        {
            case 1: _idRate = 89; break;
            case 2: _idRate = 90; break;
            case 3: _idRate = 91; break;
            case 4: _idRate = 92; break;
            case 5: _idRate = 93; break;
            case 6: _idRate = 94; break;
            case 7: _idRate = 95; break;
            case 8: _idRate = 96; break;
            case 9: _idRate = 67; break;
            case 10: _idRate = 68; break;
            case 11: _idRate = 69; break;
            case 12: _idRate = 70; break;
            case 13: _idRate = 71; break;
            case 14: _idRate = 72; break;
            case 15: _idRate = 73; break;
            case 16: _idRate = 74; break;
            case 17: _idRate = 82; break;
            case 18: _idRate = 78; break;
            case 19: _idRate = 81; break;
            case 20: _idRate = 77; break;
            case 21: _idRate = 75; break;
            case 22: _idRate = 79; break;
            case 23: _idRate = 76; break;
            case 24: _idRate = 80; break;
        }
        return _idRate;
    }
}
