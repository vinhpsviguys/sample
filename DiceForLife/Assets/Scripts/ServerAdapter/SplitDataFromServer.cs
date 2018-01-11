﻿using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using CoreLib;
using System;

public class SplitDataFromServe
{
    public static MyAccount _myAccount;
    public static ClassCharacterInit[] _classCharacters;
    public static HeroInit[] _heroInits;
    public static HeroCreate[] _heroCreated;
    public static HeroCurrentPlay _heroCurrentPLay;
    public static EquipmentsCharacter _equipmentCurrentHero; // load equipment hero đang mặc
    public static HeroCurrentPlay _heroEnemyPlay; // load chỉ số enemy
    public static EquipmentsCharacter _equipmentCurrentEnemy;// load equipment đối thủ
    public static List<EquipmentItem> _listEquipmentInBag = new List<EquipmentItem>();//các equiptment túi
    public static List<Item> _listItemInShop = new List<Item>();//Item bán trong shop
    public static List<Item> _listItemInBag = new List<Item>();// các item còn lại
    public static List<Item> _listGemInBag = new List<Item>();// các gems
    public static EventInGame _eventInGame;
    public static List<int> _listSuitableEquipment = new List<int>();
    static public MyDictionary<string, NewSkill> skillInit = new MyDictionary<string, NewSkill>();
    static public MyDictionary<string, AbnormalStatus> absInit = new MyDictionary<string, AbnormalStatus>();

    public static List<NewSkill> _heroSkill = new List<NewSkill>();
    public static List<NewSkill> _enemySkill = new List<NewSkill>();
    public static List<AbnormalStatus> _heroAbs = new List<AbnormalStatus>();

    internal static List<Item> _InitItems = new List<Item>();
    internal static List<Item> _InitGems = new List<Item>();

    public static void ReadDetailDataHeroEnemyPlay(string data)
    {
        var N = SimpleJSON.JSON.Parse(data);
        _heroEnemyPlay = JsonUtility.FromJson<HeroCurrentPlay>(N["hero"].ToString());

        int numberItemEquip = N["equipped"].Count;
        List<int> listIdWearEquipped = new List<int>();
        for (int i = 0; i < N["wear_equipped"].Count; i++)
        {
            if (N["wear_equipped"][i].AsInt != 0)
            {
                listIdWearEquipped.Add(N["wear_equipped"][i].AsInt);
            }
        }
        List<EquipmentItem> _listEquippedInHero = new List<EquipmentItem>();
        for (int i = 0; i < numberItemEquip; i++)
        {
            if (listIdWearEquipped.Contains(N["equipped"][i]["ide"].AsInt))
            {
                int tempIdItem = N["equipped"][i]["ide"].AsInt;
                int tempIdInitItem = N["equipped"][i]["idie"].AsInt;
                string tempnameItem = N["equipped"][i]["name"].Value;
                int templevelRequired = N["equipped"][i]["levelrequired"].AsInt;
                int templevelUpgrade = N["equipped"][i]["levelupgraded"].AsInt;
                int tempidSet = N["equipped"][i]["idsuit"].AsInt;

                int temTypeEquipped = N["equipped"][i]["typequipped"].AsInt;
                int temChildType = N["equipped"][i]["childtype"].AsInt;

                //TypeEquipmentCharacter tempType = GetSlotWearEquipped(data, tempIdItem, _heroEnemyPlay.idclass);

                int priceItem = N["equipped"][i]["sellprice"].AsInt;
                int rareItem = N["equipped"][i]["rateitem"].AsInt;

                int typeEquipped = N["equipped"][i]["typequipped"].AsInt;
                int typeChild = N["equipped"][i]["childtype"].AsInt;
                ClassCharacterItem _classCharacterItem = ClassCharacterItem.None;
                switch (typeEquipped)
                {
                    case 12: _classCharacterItem = ClassCharacterItem.HeavyArmor; break;
                    case 13: _classCharacterItem = ClassCharacterItem.SoftArmor; break;
                    case 14: _classCharacterItem = ClassCharacterItem.ClothArmor; break;
                    default: _classCharacterItem = ClassCharacterItem.None; break;
                }

                TypeEquipmentCharacter tempType = MappingData.ConvertIdItemToType(typeEquipped, typeChild, _heroCurrentPLay.idclass, listIdWearEquipped.Contains(tempIdItem));

                if (N["wear_equipped"]["ide_weapon"].AsInt == tempIdItem)
                {
                    tempType = TypeEquipmentCharacter.Weapon;
                }
                else if (N["wear_equipped"]["ide_shield"].AsInt == tempIdItem)
                {
                    if (_heroCurrentPLay.idclass == 1 || _heroCurrentPLay.idclass == 5 || _heroCurrentPLay.idclass == 7)
                    {
                        tempType = TypeEquipmentCharacter.OffhandWeapon;
                    }
                    else tempType = TypeEquipmentCharacter.Shield;
                }



                EquipmentItem item = new EquipmentItem(tempIdItem, tempIdInitItem, tempType, _classCharacterItem, typeEquipped, tempidSet, tempnameItem, templevelRequired, templevelUpgrade, priceItem, rareItem);
                item.setValue("1", N["equipped"][i]["mindamage"].AsFloat);
                item.setValue("2", N["equipped"][i]["maxdamage"].AsFloat);
                item.setValue("3", N["equipped"][i]["minmagic"].AsFloat);
                item.setValue("4", N["equipped"][i]["maxmagic"].AsFloat);
                item.setValue("5", N["equipped"][i]["critmax"].AsFloat);
                item.setValue("6", N["equipped"][i]["multicastmax"].AsFloat);
                item.setValue("7", N["equipped"][i]["min_incrdamage"].AsFloat);
                item.setValue("8", N["equipped"][i]["max_incrdamage"].AsFloat);
                item.setValue("9", N["equipped"][i]["min_incrmagic"].AsFloat);
                item.setValue("10", N["equipped"][i]["max_incrmagic"].AsFloat);
                item.setValue("11", N["equipped"][i]["min_rate"].AsFloat);
                item.setValue("12", N["equipped"][i]["max_rate"].AsFloat);
                item.setValue("13", N["equipped"][i]["minparryrate"].AsFloat);
                item.setValue("14", N["equipped"][i]["maxparryrate"].AsFloat);
                item.setValue("15", N["equipped"][i]["min_block"].AsFloat);
                item.setValue("16", N["equipped"][i]["max_block"].AsFloat);
                item.setValue("17", N["equipped"][i]["minphydef"].AsFloat);
                item.setValue("18", N["equipped"][i]["maxphydef"].AsFloat);
                item.setValue("19", N["equipped"][i]["minmagicdef"].AsFloat);
                item.setValue("20", N["equipped"][i]["maxmagicdef"].AsFloat);
                item.setValue("21", N["equipped"][i]["minphyreduction"].AsFloat);
                item.setValue("22", N["equipped"][i]["maxphyreduction"].AsFloat);
                item.setValue("23", N["equipped"][i]["minmagicreduction"].AsFloat);
                item.setValue("24", N["equipped"][i]["maxmagicreduction"].AsFloat);
                item.setValue("25", N["equipped"][i]["minphyabsorb"].AsFloat);
                item.setValue("26", N["equipped"][i]["maxphyabsorb"].AsFloat);
                item.setValue("27", N["equipped"][i]["minmagicabsorb"].AsFloat);
                item.setValue("28", N["equipped"][i]["maxmagicabsorb"].AsFloat);

                item.setValue("listidproperty", N["equipped"][i]["maxmagicabsorb"].Value.ToString());

                _listEquippedInHero.Add(item);
            }
        }

        _equipmentCurrentEnemy = new EquipmentsCharacter(_listEquippedInHero);
    }

    public static void ReadDetailDataHeroCurrentPlay(string data)
    {
        //Debug.Log(data);
        var N = SimpleJSON.JSON.Parse(data);
        _heroCurrentPLay = JsonUtility.FromJson<HeroCurrentPlay>(N["hero"].ToString());

        int numberItemEquip = N["equipped"].Count;
        List<int> listIdWearEquipped = new List<int>();
        for (int i = 0; i < N["wear_equipped"].Count; i++)
        {
            if (N["wear_equipped"][i].AsInt != 0)
            {
                listIdWearEquipped.Add(N["wear_equipped"][i].AsInt);
            }
        }
        List<EquipmentItem> _listEquippedInHero = new List<EquipmentItem>();
        for (int i = 0; i < numberItemEquip; i++)
        {
            int tempIdItem = N["equipped"][i]["ide"].AsInt;
            int tempIdInitItem = N["equipped"][i]["idie"].AsInt;
            string tempnameItem = N["equipped"][i]["name"].Value;
            int templevelRequired = N["equipped"][i]["levelrequired"].AsInt;
            int templevelUpgrade = N["equipped"][i]["levelupgraded"].AsInt;
            int tempidSet = N["equipped"][i]["idsuit"].AsInt;
            int temTypeEquipped = N["equipped"][i]["typequipped"].AsInt;
            int temChildType = N["equipped"][i]["childtype"].AsInt;

            int typeEquipped = N["equipped"][i]["typequipped"].AsInt;
            int typeChild = N["equipped"][i]["childtype"].AsInt;
            ClassCharacterItem _classCharacterItem = ClassCharacterItem.None;
            switch (typeEquipped)
            {
                case 12: _classCharacterItem = ClassCharacterItem.HeavyArmor; break;
                case 13: _classCharacterItem = ClassCharacterItem.SoftArmor; break;
                case 14: _classCharacterItem = ClassCharacterItem.ClothArmor; break;
                default: _classCharacterItem = ClassCharacterItem.None; break;
            }

            TypeEquipmentCharacter tempType = MappingData.ConvertIdItemToType(typeEquipped, typeChild, _heroCurrentPLay.idclass, listIdWearEquipped.Contains(tempIdItem));

            if (N["wear_equipped"]["ide_weapon"].AsInt == tempIdItem)
            {
                tempType = TypeEquipmentCharacter.Weapon;
            }
            else if (N["wear_equipped"]["ide_shield"].AsInt == tempIdItem)
            {
                if (_heroCurrentPLay.idclass == 1 || _heroCurrentPLay.idclass == 5 || _heroCurrentPLay.idclass == 7)
                {
                    tempType = TypeEquipmentCharacter.OffhandWeapon;
                }
                else tempType = TypeEquipmentCharacter.Shield;
            }

            int priceItem = N["equipped"][i]["sellprice"].AsInt;
            int rare = N["equipped"][i]["rateitem"].AsInt;

            EquipmentItem item = new EquipmentItem(tempIdItem, tempIdInitItem, tempType, _classCharacterItem, typeEquipped, tempidSet, tempnameItem, templevelRequired, templevelUpgrade, priceItem, rare);
            item.setValue("1", N["equipped"][i]["mindamage"].AsFloat);
            item.setValue("2", N["equipped"][i]["maxdamage"].AsFloat);
            item.setValue("3", N["equipped"][i]["minmagic"].AsFloat);
            item.setValue("4", N["equipped"][i]["maxmagic"].AsFloat);
            item.setValue("5", N["equipped"][i]["critmax"].AsFloat);
            item.setValue("6", N["equipped"][i]["multicastmax"].AsFloat);
            item.setValue("7", N["equipped"][i]["min_incrdamage"].AsFloat);
            item.setValue("8", N["equipped"][i]["max_incrdamage"].AsFloat);
            item.setValue("9", N["equipped"][i]["min_incrmagic"].AsFloat);
            item.setValue("10", N["equipped"][i]["max_incrmagic"].AsFloat);
            item.setValue("11", N["equipped"][i]["min_rate"].AsFloat);
            item.setValue("12", N["equipped"][i]["max_rate"].AsFloat);
            item.setValue("13", N["equipped"][i]["minparryrate"].AsFloat);
            item.setValue("14", N["equipped"][i]["maxparryrate"].AsFloat);
            item.setValue("15", N["equipped"][i]["min_block"].AsFloat);
            item.setValue("16", N["equipped"][i]["max_block"].AsFloat);
            item.setValue("17", N["equipped"][i]["minphydef"].AsFloat);
            item.setValue("18", N["equipped"][i]["maxphydef"].AsFloat);
            item.setValue("19", N["equipped"][i]["minmagicdef"].AsFloat);
            item.setValue("20", N["equipped"][i]["maxmagicdef"].AsFloat);
            item.setValue("21", N["equipped"][i]["minphyreduction"].AsFloat);
            item.setValue("22", N["equipped"][i]["maxphyreduction"].AsFloat);
            item.setValue("23", N["equipped"][i]["minmagicreduction"].AsFloat);
            item.setValue("24", N["equipped"][i]["maxmagicreduction"].AsFloat);
            item.setValue("25", N["equipped"][i]["minphyabsorb"].AsFloat);
            item.setValue("26", N["equipped"][i]["maxphyabsorb"].AsFloat);
            item.setValue("27", N["equipped"][i]["minmagicabsorb"].AsFloat);
            item.setValue("28", N["equipped"][i]["maxmagicabsorb"].AsFloat);

            string _valueProperty = N["equipped"][i]["listidproperty"].Value;
            item.setValue("listidproperty", _valueProperty);

            if (_valueProperty.Equals("null") || _valueProperty.Equals("[]") || string.IsNullOrEmpty(_valueProperty))
            { }
            else
            {
                try
                {
                    var M = CoreLib.JSON.Parse(_valueProperty);
                    foreach (KeyValuePair<string, CoreLib.JSONNode> _temp in M.AsObject)
                    {
                        //Debug.Log(_temp.Key.ToString());
                        item.setValue(_temp.Key.ToString(), float.Parse(_temp.Value.AsFloat.ToString()));
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log(tempIdItem + " " + _valueProperty);
                }
            }

            //Debug.Log(item.typeItem);
            if (listIdWearEquipped.Contains(tempIdItem))
            {
                _listEquippedInHero.Add(item);
            }
            else _listEquipmentInBag.Add(item);
        }

        int numberAvatar = N["avatar"].Count;
        for (int i = 0; i < numberAvatar; i++)
        {
            int tempIdItem = N["avatar"][i]["idha"].AsInt;

            //int tempIdInitItem = N["avatar"][i]["idia"].AsInt;       
            string tempnameItem = N["avatar"][i]["name"].Value.ToString();
            int templevelRequired = 1;
            int templevelUpgrade = int.Parse(N["avatar"][i]["level"].Value.ToString());
            int tempidSet = 0;
            int priceItem = 0;
            int rare = 0;
            EquipmentItem item = new EquipmentItem(tempIdItem, 1, TypeEquipmentCharacter.Avatar, ClassCharacterItem.None, 0, tempidSet, tempnameItem, templevelRequired, templevelUpgrade, priceItem, rare);

            //Thêm các dòng trắng
            item.setValue("97", float.Parse(N["avatar"][i]["hp"].Value.ToString()));
            item.setValue("98", float.Parse(N["avatar"][i]["physical_damage"].Value.ToString()));
            item.setValue("99", float.Parse(N["avatar"][i]["magical_damage"].Value.ToString()));
            item.setValue("100", float.Parse(N["avatar"][i]["physical_absorption"].Value.ToString()));
            item.setValue("101", float.Parse(N["avatar"][i]["magical_absorption"].Value.ToString()));
            item.setValue("999", N["avatar"][i]["maxlist"].AsInt);
            item.setValue("listidproperty", N["avatar"][i]["listidproperty"].Value.ToString());
            try
            {
                var M = SimpleJSON.JSON.Parse(N["avatar"][i]["listidproperty"].Value.ToString());
                foreach (KeyValuePair<string, SimpleJSON.JSONNode> _temp in M.AsObject)
                {
                    //Debug.Log(_temp.Key.ToString());
                    item.setValue(_temp.Key.ToString(), float.Parse(_temp.Value.AsFloat.ToString()));
                }
            }
            catch (Exception e)
            {

            }
            //check trong túi hay người
            if (listIdWearEquipped.Contains(tempIdItem))
            {
                _listEquippedInHero.Add(item);
            }
            else _listEquipmentInBag.Add(item);
        }

        int numberBuff = N["buff"].Count;
        for (int i = 0; i < numberBuff; i++)
        {
            int tempIdItem = N["buff"][i]["idbf"].AsInt;
            int tempIdInit = N["buff"][i]["idibf"].AsInt;
            string tempnameItem = N["buff"][i]["name"].Value.ToString();
            int templevelRequired = 1;
            int templevelUpgrade = int.Parse(N["buff"][i]["level"].Value.ToString());
            int tempidSet = 0;
            //TypeEquipmentCharacter tempType = TypeEquipmentCharacter.Buff;
            int priceItem = 0;
            int rare = 0;
            EquipmentItem item = new EquipmentItem(tempIdItem, tempIdInit, TypeEquipmentCharacter.Buff, ClassCharacterItem.None, 0, tempidSet, tempnameItem, templevelRequired, templevelUpgrade, priceItem, rare);

            float _rate = N["buff"][i]["rate"].AsFloat;
            int _idRate = MappingData.ConvertIdBuffToAttribute(tempIdInit);
            item.setValue(_idRate.ToString(), _rate);
            //check trong túi hay người
            if (listIdWearEquipped.Contains(tempIdItem))
            {
                _listEquippedInHero.Add(item);
            }
            else _listEquipmentInBag.Add(item);
        }
        _equipmentCurrentHero = new EquipmentsCharacter(_listEquippedInHero);
    }

    public static EquipmentItem CreateEquipmentItem(string data)
    {
        //Debug.Log(data);
        var N = SimpleJSON.JSON.Parse(data);
        int tempIdItem = N["ide"].AsInt;
        int tempIdInitItem = N["idie"].AsInt;
        string tempnameItem = N["name"].Value.ToString();
        int templevelRequired = N["levelrequired"].AsInt;
        int templevelUpgrade = N["levelupgraded"].AsInt;
        int tempidSet = N["idsuit"].AsInt;

        int typeEquipped = N["typequipped"].AsInt;
        int typeChild = N["childtype"].AsInt;
        ClassCharacterItem _classCharacterItem = ClassCharacterItem.None;
        switch (typeEquipped)
        {
            case 12: _classCharacterItem = ClassCharacterItem.HeavyArmor; break;
            case 13: _classCharacterItem = ClassCharacterItem.SoftArmor; break;
            case 14: _classCharacterItem = ClassCharacterItem.ClothArmor; break;
            default: _classCharacterItem = ClassCharacterItem.None; break;
        }
        TypeEquipmentCharacter tempType = MappingData.ConvertIdItemToType(typeEquipped, typeChild, _heroCurrentPLay.idclass, false);


        int priceItem = N["sellprice"].AsInt;
        int rare = N["rateitem"].AsInt;

        EquipmentItem item = new EquipmentItem(tempIdItem, tempIdInitItem, tempType, _classCharacterItem, typeEquipped, tempidSet, tempnameItem, templevelRequired, templevelUpgrade, priceItem, rare);

        item.setValue("1", N["mindamage"].AsFloat);
        item.setValue("2", N["maxdamage"].AsFloat);
        item.setValue("3", N["minmagic"].AsFloat);
        item.setValue("4", N["maxmagic"].AsFloat);
        item.setValue("5", N["critmax"].AsFloat);
        item.setValue("6", N["multicastmax"].AsFloat);
        item.setValue("7", N["min_incrdamage"].AsFloat);
        item.setValue("8", N["max_incrdamage"].AsFloat);
        item.setValue("9", N["min_incrmagic"].AsFloat);
        item.setValue("10", N["max_incrmagic"].AsFloat);
        item.setValue("11", N["min_rate"].AsFloat);
        item.setValue("12", N["max_rate"].AsFloat);
        item.setValue("13", N["minparryrate"].AsFloat);
        item.setValue("14", N["maxparryrate"].AsFloat);
        item.setValue("15", N["min_block"].AsFloat);
        item.setValue("16", N["max_block"].AsFloat);
        item.setValue("17", N["minphydef"].AsFloat);
        item.setValue("18", N["maxphydef"].AsFloat);
        item.setValue("19", N["minmagicdef"].AsFloat);
        item.setValue("20", N["maxmagicdef"].AsFloat);
        item.setValue("21", N["minphyreduction"].AsFloat);
        item.setValue("22", N["maxphyreduction"].AsFloat);
        item.setValue("23", N["minmagicreduction"].AsFloat);
        item.setValue("24", N["maxmagicreduction"].AsFloat);
        item.setValue("25", N["minphyabsorb"].AsFloat);
        item.setValue("26", N["maxphyabsorb"].AsFloat);
        item.setValue("27", N["minmagicabsorb"].AsFloat);
        item.setValue("28", N["maxmagicabsorb"].AsFloat);

        item.setValue("listidproperty", N["listidproperty"].Value.ToString());
        return item;
    }

    public static void ReadItemInitData(string data)
    {
        var N = CoreLib.JSON.Parse(data);

        var _dataItemInit = N["item"];
        int numberItemInit = _dataItemInit.Count;
        for (int i = 0; i < numberItemInit; i++)
        {
            _InitItems.Add(new Item(_dataItemInit[i]));
        }
        var _dataGemInit = N["gem"];
        int numberGemInit = _dataGemInit.Count;
        for (int i = 0; i < numberGemInit; i++)
        {
            _InitGems.Add(new Item(_dataGemInit[i]));
        }
    }
    public static void ReadShopInitData(string data)
    {
        var N = CoreLib.JSON.Parse(data);
        int numberItemInShop = N.Count;
        for (int i = 0; i < numberItemInShop; i++)
        {
            _listItemInShop.Add(new Item(N[i]));
        }
    }

    public static void ReadItemInBagData(string data)
    {
        var N = CoreLib.JSON.Parse(data);
        int numberItem = N["item"].Count;
        for (int i = 0; i < numberItem; i++)
        {
            Item _tempItem = new Item(N["item"][i]);
            _listItemInBag.Add(_tempItem);
        }

        int numberGem = N["gem"].Count;
        for (int i = 0; i < numberGem; i++)
        {
            Item _tempItem = new Item(N["gem"][i]);
            _listGemInBag.Add(_tempItem);
        }

        string _weaponCharacter = string.Empty;
        for (int i = 0; i < _classCharacters.Length; i++)
        {
            if (N["hero"]["idclass"].AsInt == _classCharacters[i].idcl)
            {
                _weaponCharacter = _classCharacters[i].weapon;
                break;
            }
        }
        string[] _splitData = _weaponCharacter.Split(',');
        for (int i = 0; i < _splitData.Length; i++)
        {
            _listSuitableEquipment.Add(int.Parse(_splitData[i]));
        }
        _listSuitableEquipment.Add(0);
    }

    public static void ReadSkillHeroData(string data)
    {
        //Debug.Log(data);
        _heroSkill.Clear();
        var N = SimpleJSON.JSON.Parse(data);
        int numberSkillHero = N.Count;
        int index = 0;
        for (int i = 0; i < numberSkillHero; i++)
        {

            foreach (NewSkill _tempSkill in skillInit.Values)
            {

                if (N[i]["idk"].AsInt == _tempSkill.data["idInit"].AsInt)
                {
                    _tempSkill.addField("idhk", N[i]["idhk"].AsInt);
                    _tempSkill.addField("idk", N[i]["idk"].AsInt);
                    _tempSkill.addField("typewear", N[i]["typewear"].AsInt);
                    _tempSkill.addField("level", N[i]["level"].AsInt);
                    _heroSkill.Add(_tempSkill);

                }
            }
        }
    }

    public static void ReadEnemySkillData(string data)
    {
        _enemySkill.Clear();
        var N = SimpleJSON.JSON.Parse(data);
        int numberSkillHero = N.Count;
        for (int i = 0; i < numberSkillHero; i++)
        {
            foreach (NewSkill _tempSkill in skillInit.Values)
            {

                if (N[i]["idk"].AsInt == _tempSkill.data["idInit"].AsInt && !_enemySkill.Contains(_tempSkill))
                {
                    _tempSkill.addField("level", N[i]["level"].AsInt);
                    _enemySkill.Add(_tempSkill);

                }
            }
        }
    }

    public static NewSkill getSkill(int idSkill)
    {
        foreach (NewSkill _tempSkill in SplitDataFromServe.skillInit.Values)
        {

            if (idSkill == _tempSkill.data["idInit"].AsInt)
            {
                return _tempSkill;

            }
        }
        return null;
    }

    public static void ReadLoginData(string data)
    {
        //Debug.Log(data);
        var N = SimpleJSON.JSON.Parse(data);
        _myAccount = JsonUtility.FromJson<MyAccount>(N["account"].ToString());
        int numberHeroCreate = N["hero"].Count;
        _heroCreated = new HeroCreate[numberHeroCreate];
        for (int i = 0; i < numberHeroCreate; i++)
        {
            _heroCreated[i] = JsonUtility.FromJson<HeroCreate>(N["hero"][i].ToString());
        }
    }

    public static void ReadInitData(string data)
    {
        var N = SimpleJSON.JSON.Parse(data);

        int numberClass = N["class"].Count;
        _classCharacters = new ClassCharacterInit[numberClass];
        for (int i = 0; i < numberClass; i++)
        {
            _classCharacters[i] = JsonUtility.FromJson<ClassCharacterInit>(N["class"][i].ToString());
        }
        int numberHero = N["hero"].Count;
        _heroInits = new HeroInit[numberHero];
        for (int i = 0; i < numberHero; i++)
        {
            _heroInits[i] = JsonUtility.FromJson<HeroInit>(N["hero"][i].ToString());
        }
        _eventInGame = new EventInGame(N["event"]["event_lucky"].Value);
    }

    public static string GetNameClass(int id)
    {
        foreach (var _class in _classCharacters)
        {
            if (_class.idcl == id) return _class.name;
        }
        return "Unknow";
    }

    public static void SubItemInBag(int indexItem, int number)
    {
        if (indexItem > _listItemInBag.Count) Debug.LogError("Out of range!");
        else
        {
            int _curentQtt = int.Parse(_listItemInBag[indexItem].getValue("quantity").ToString());
            if (_curentQtt < number) Debug.LogError("Out of number!");
            else
            {
                if (_curentQtt == number) _listItemInBag.RemoveAt(indexItem);
                else _listItemInBag[indexItem].setValue("quantity", _curentQtt - number);
            }
        }
    }

}