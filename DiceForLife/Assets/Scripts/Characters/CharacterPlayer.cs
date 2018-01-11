using CoreLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public enum TurnState
//{
//    NONE=0,
//    DICE,
//    SELECTSKILL,
//    DOACTION,
//    ENEMYTURN,
//}

public class CharacterPlayer : MonoBehaviour {


    public int playerId;

    public Characteristic characteristic;

    public CharacterProperties _baseProperties;

    public Equipment[] equipments = new Equipment[12];

    public EquipmentsCharacter _myEquipments = new EquipmentsCharacter();

    public MyDictionary<string, NewSkill> newSkillDic = new MyDictionary<string, NewSkill>();
    public MyDictionary<string, NewSkill> newSkillDicOfCharacter = new MyDictionary<string, NewSkill>();
    public MyDictionary<string, AbnormalStatus> abDic = new MyDictionary<string, AbnormalStatus>();


    //public List<Effect> _myEffects = new List<Effect>();
    internal int _actionPoints;

    //internal CharacterInfo _infoCharacter;

    internal List<ActionHandle> _listAction;

    internal bool isReady = false;
    internal bool isDoingNormalAttack = false;
    internal string idUserSocketIO="";
    internal int keyPlayer = 0;

    //public TurnState currentState;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        this.gameObject.tag = "DontDestroyObject";
        _listAction = new List<ActionHandle>();
        Constants.init();

    }
   
   
    public static CharacterPlayer LoadCharacterPlayer()
    {
        CharacterPlayer character = new CharacterPlayer();

        character._baseProperties = new CharacterProperties(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, CharacterInfo._instance._baseProperties._idclassCharacter, CharacterInfo._instance._baseProperties.name, CharacterInfo._instance._baseProperties.Level
          , CharacterInfo._instance._baseProperties.Exp, CharacterInfo._instance._baseProperties.EnergyPoint, CharacterInfo._instance._baseProperties.HeartPoint, CharacterInfo._instance._baseProperties.PvpPoint, CharacterInfo._instance._baseProperties.SkillPoint, CharacterInfo._instance._baseProperties.AttributePoint, CharacterInfo._instance._baseProperties.LevelMap, CharacterInfo._instance._baseProperties.Gold, CharacterInfo._instance._baseProperties.Diamond, CharacterInfo._instance._baseProperties.SlotChest, CharacterInfo._instance._baseProperties.typeAttack, (int)CharacterInfo._instance._baseProperties.hp,
          CharacterInfo._instance._baseProperties.Strength, CharacterInfo._instance._baseProperties.Intelligence, CharacterInfo._instance._baseProperties.Dexterity, CharacterInfo._instance._baseProperties.Focus, CharacterInfo._instance._baseProperties.Luck,
          CharacterInfo._instance._baseProperties.Endurance, CharacterInfo._instance._baseProperties.Blessing, CharacterInfo._instance._baseProperties.FightingPower);

        character._myEquipments = SplitDataFromServe._equipmentCurrentHero;

        for (int i = 0; i < 12; i++)
        {
            Equipment equipment = null;
            switch (i)
            {
                case 0:
                    if (character._myEquipments.weaponItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.weaponItem);
                    }
                    break;
                case 1:
                    if (character._myEquipments.shieldItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.shieldItem);
                    }
                    break;
                case 2:
                    if (character._myEquipments.headItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.headItem);
                    }
                    break;
                case 3:
                    if (character._myEquipments.torsoItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.torsoItem);
                    }
                    break;
                case 4:
                    if (character._myEquipments.legItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.legItem);
                    }
                    break;
                case 5:
                    if (character._myEquipments.beltItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.beltItem);
                    }
                    break;
                case 6:
                    if (character._myEquipments.glovesItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.glovesItem);
                    }
                    break;
                case 7:
                    if (character._myEquipments.bootsItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.bootsItem);
                    }
                    break;
                case 8:
                    if (character._myEquipments.ringItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.ringItem);
                    }
                    break;
                case 9:
                    if (character._myEquipments.amuletItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.amuletItem);
                    }
                    break;
                case 10:
                    if (character._myEquipments.avatarItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.avatarItem);
                    }
                    break;
                case 11:
                    if (character._myEquipments.buffItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.buffItem);
                    }
                    break;
            }
            if (equipment!=null) character.equipments[i] = equipment;
            //character.playerId = int.Parse(SplitDataFromServe._heroCurrentPLay.idh);
            //character.characteristic = new Characteristic((Characteristic.CharacterType)int.Parse(SplitDataFromServe._heroCurrentPLay.type), (Characteristic.CharacterClass)SplitDataFromServe._heroCurrentPLay.idclass,
            //   float.Parse(SplitDataFromServe._heroCurrentPLay.level), float.Parse(SplitDataFromServe._heroCurrentPLay.strength), float.Parse(SplitDataFromServe._heroCurrentPLay.intelligence),
            //   float.Parse(SplitDataFromServe._heroCurrentPLay.dexterity), float.Parse(SplitDataFromServe._heroCurrentPLay.focus), float.Parse(SplitDataFromServe._heroCurrentPLay.vitality), 0,
            //    float.Parse(SplitDataFromServe._heroCurrentPLay.luck), float.Parse(SplitDataFromServe._heroCurrentPLay.endurance), float.Parse(SplitDataFromServe._heroCurrentPLay.blessing));
            character.characteristic = new Characteristic((Characteristic.CharacterType)CharacterInfo._instance._baseProperties.typeAttack, (Characteristic.CharacterClass)CharacterInfo._instance._baseProperties._classCharacter,
              CharacterInfo._instance._baseProperties.Level, CharacterInfo._instance._baseProperties.Strength, CharacterInfo._instance._baseProperties.Intelligence,
              CharacterInfo._instance._baseProperties.Dexterity, CharacterInfo._instance._baseProperties.Focus, CharacterInfo._instance._baseProperties.Vitality,0,
              CharacterInfo._instance._baseProperties.Luck, CharacterInfo._instance._baseProperties.Endurance, CharacterInfo._instance._baseProperties.Blessing);
        }
        

        //List<SkillCharacter> _tempListSkill = CharacterItemInGame.Instance.LoadAllSkillOfClass(_baseProperties._classCharacter);

        //for (int i = 0; i < SplitDataFromServe._heroSkillData.Length; i++)
        //{
        //    if (int.Parse(SplitDataFromServe._heroSkillData[i].typewear) == 1)
        //    {

        //        List<Effect> tempEff = character.CreateSampleEffect();

        //        SkillCharacter tempSkill = new SkillCharacter(int.Parse(SplitDataFromServe._heroSkillData[i].idhk), int.Parse(SplitDataFromServe._heroSkillData[i].idk), (ClassCharacter)int.Parse(SplitDataFromServe._heroSkillData[i].forclass),
        //            SplitDataFromServe._heroSkillData[i].name, SplitDataFromServe._heroSkillData[i].description,1, 5, 0, 0, tempEff, 5);
        //        character._mySkills.Add(tempSkill);
        //        //foreach(SkillCharacter skill in _tempListSkill)
        //        //{
        //        //    if(skill.idSkill == int.Parse(SplitDataFromServe._heroSkillData[i].idk))
        //        //    {
        //        //        _mySkills.Add(skill);
        //        //    }
        //        //}
        //    }
        //}
        character._actionPoints = 0;
        character._listAction = new List<ActionHandle>();
        return character;
    }


    public CharacterPlayer loadDictionaries(MyDictionary<string, NewSkill> skills, MyDictionary<string, AbnormalStatus> abs)
    {
        foreach (string key in skills.Keys)
        {
            //Debug.Log(key);
            newSkillDic.Add(key, skills[key].clone());
        }

        foreach (string key in abs.Keys)
        {
            abDic.Add(key, abs[key].clone());
        }
        return this;
    }

    public CharacterPlayer loadDictionaries(List<NewSkill> heroskills, List<NewSkill> enemyskills, List<AbnormalStatus> abs)
    {
        foreach (NewSkill _tempSkill in heroskills)
        {
            if (!newSkillDic.ContainsKey("Skill" + _tempSkill.data["idInit"].AsInt))
            {
                newSkillDic.Add("Skill" + _tempSkill.data["idInit"].AsInt, _tempSkill.clone());
                newSkillDicOfCharacter.Add("Skill" + _tempSkill.data["idInit"].AsInt, _tempSkill.clone());
            }
        }
       
        foreach (NewSkill _tempSkill in enemyskills)
        {
            if (!newSkillDic.ContainsKey("Skill" + _tempSkill.data["idInit"].AsInt))
            {
                newSkillDic.Add("Skill" + _tempSkill.data["idInit"].AsInt, _tempSkill.clone());
            }
        }

        foreach (AbnormalStatus _tempAbs in abs)
        {
            abDic.Add("AS"+ _tempAbs.data["idInit"].AsInt, _tempAbs.clone());
        }
        return this;
    }

    public static void LoadCharacterPlayer(CharacterPlayer character)
    {
        character._baseProperties = new CharacterProperties(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, CharacterInfo._instance._baseProperties._idclassCharacter, CharacterInfo._instance._baseProperties.name, CharacterInfo._instance._baseProperties.Level
         , CharacterInfo._instance._baseProperties.Exp, CharacterInfo._instance._baseProperties.EnergyPoint, CharacterInfo._instance._baseProperties.HeartPoint, CharacterInfo._instance._baseProperties.PvpPoint, CharacterInfo._instance._baseProperties.SkillPoint, CharacterInfo._instance._baseProperties.AttributePoint, CharacterInfo._instance._baseProperties.LevelMap, CharacterInfo._instance._baseProperties.Gold, CharacterInfo._instance._baseProperties.Diamond, CharacterInfo._instance._baseProperties.SlotChest, CharacterInfo._instance._baseProperties.typeAttack, (int)CharacterInfo._instance._baseProperties.hp,
         CharacterInfo._instance._baseProperties.Strength, CharacterInfo._instance._baseProperties.Intelligence, CharacterInfo._instance._baseProperties.Dexterity, CharacterInfo._instance._baseProperties.Focus, CharacterInfo._instance._baseProperties.Luck,
         CharacterInfo._instance._baseProperties.Endurance, CharacterInfo._instance._baseProperties.Blessing, CharacterInfo._instance._baseProperties.FightingPower);

        character._myEquipments = SplitDataFromServe._equipmentCurrentHero;
        for (int i = 0; i < 12; i++)
        {
            Equipment equipment = null;
            switch (i)
            {
                case 0:
                    if (character._myEquipments.weaponItem != null)
                    {
                        
                        equipment = Equipment.convertToEquipment(character._myEquipments.weaponItem);
                    }
                    break;
                case 1:
                    if (character._myEquipments.shieldItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.shieldItem);
                    }
                    break;
                case 2:
                    if (character._myEquipments.headItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.headItem);
                    }
                    break;
                case 3:
                    if (character._myEquipments.torsoItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.torsoItem);
                    }
                    break;
                case 4:
                    if (character._myEquipments.legItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.legItem);
                    }
                    break;
                case 5:
                    if (character._myEquipments.beltItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.beltItem);
                    }
                    break;
                case 6:
                    if (character._myEquipments.glovesItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.glovesItem);
                    }
                    break;
                case 7:
                    if (character._myEquipments.bootsItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.bootsItem);
                    }
                    break;
                case 8:
                    if (character._myEquipments.ringItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.ringItem);
                    }
                    break;
                case 9:
                    if (character._myEquipments.amuletItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.amuletItem);
                    }
                    break;
                case 10:
                    if (character._myEquipments.avatarItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.avatarItem);
                    }
                    break;
                case 11:
                    if (character._myEquipments.buffItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.buffItem);
                    }
                    break;
            }
            if (equipment != null) character.equipments[i] = equipment;
   
            character.characteristic = new Characteristic((Characteristic.CharacterType)CharacterInfo._instance._baseProperties.typeAttack, (Characteristic.CharacterClass)CharacterInfo._instance._baseProperties._classCharacter,
             CharacterInfo._instance._baseProperties.Level, CharacterInfo._instance._baseProperties.Strength, CharacterInfo._instance._baseProperties.Intelligence,
             CharacterInfo._instance._baseProperties.Dexterity, CharacterInfo._instance._baseProperties.Focus, CharacterInfo._instance._baseProperties.Vitality, 0,
             CharacterInfo._instance._baseProperties.Luck, CharacterInfo._instance._baseProperties.Endurance, CharacterInfo._instance._baseProperties.Blessing);
        }
        character._baseProperties.hp = character.characteristic.Max_Health;
        character._actionPoints = 0;
        character._listAction = new List<ActionHandle>();
        character.idUserSocketIO = CharacterInfo._instance.idUserSocketIO;
    }


    public static void LoadCharacterMonster(CharacterPlayer monster, JSONNode dataMonster)
    {
        monster._baseProperties = new CharacterProperties(-1, "",-1,"", dataMonster["level"]
       , dataMonster["exp"].AsDouble,0,0,0, dataMonster["sp"].AsDouble, 0, 0, dataMonster["gold"], 0, dataMonster["type"].AsInt, dataMonster["hp"].AsInt,
       dataMonster["str"].AsInt, dataMonster["int"].AsInt, dataMonster["vit"].AsInt,0, 0, 0,
       0, 0,0, dataMonster["typemonster"].Value);
        EquipmentItem _weaponItemMonster = new EquipmentItem(-1, -1, TypeEquipmentCharacter.Weapon, ClassCharacterItem.None, -1, -1, "", -1, -1, -1, -1);
        _weaponItemMonster.setValue("1", dataMonster["phy_dmg_min"].AsFloat);
        _weaponItemMonster.setValue("2", dataMonster["phy_dmg_max"].AsFloat);
        _weaponItemMonster.setValue("3", dataMonster["mag_dmg_min"].AsFloat);
        _weaponItemMonster.setValue("4", dataMonster["mag_dmg_max"].AsFloat);
        _weaponItemMonster.setValue("5", dataMonster["crit_chance"].AsFloat);
        _weaponItemMonster.setValue("6", dataMonster["multicast_chance"].AsFloat);
        _weaponItemMonster.setValue("7", dataMonster["phy_rein_min"].AsFloat);
        _weaponItemMonster.setValue("8", dataMonster["phy_rein_max"].AsFloat);
        _weaponItemMonster.setValue("9", dataMonster["mag_rein_min"].AsFloat);
        _weaponItemMonster.setValue("10", dataMonster["mag_rein_max"].AsFloat);
        _weaponItemMonster.setValue("11", dataMonster["atk_rate_min"].AsFloat);
        _weaponItemMonster.setValue("12", dataMonster["atk_rate_max"].AsFloat);
        _weaponItemMonster.setValue("13", dataMonster["parry_rate_min"].AsFloat);
        _weaponItemMonster.setValue("14", dataMonster["parry_rate_max"].AsFloat);
        _weaponItemMonster.setValue("15", dataMonster["block"].AsFloat);
        _weaponItemMonster.setValue("16", dataMonster["block"].AsFloat);
        _weaponItemMonster.setValue("17", dataMonster["phy_def_min"].AsFloat);
        _weaponItemMonster.setValue("18", dataMonster["phy_def_max"].AsFloat);
        _weaponItemMonster.setValue("19", dataMonster["mag_def_min"].AsFloat);
        _weaponItemMonster.setValue("20", dataMonster["mag_def_max"].AsFloat);
        _weaponItemMonster.setValue("21", dataMonster["phy_red_min"].AsFloat);
        _weaponItemMonster.setValue("22", dataMonster["phy_red_max"].AsFloat);
        _weaponItemMonster.setValue("23", dataMonster["mag_red_min"].AsFloat);
        _weaponItemMonster.setValue("24", dataMonster["mag_red_max"].AsFloat);

        Equipment equipment = Equipment.convertToEquipment(_weaponItemMonster);
        monster.equipments[0] = equipment;
        monster.characteristic= new Characteristic((Characteristic.CharacterType)dataMonster["type"].AsInt, Characteristic.CharacterClass.Assassin,
             dataMonster["level"], dataMonster["str"].AsInt, dataMonster["int"].AsInt,
             0, 0, dataMonster["vit"].AsInt, 0,
             0, 0, 0);
        monster._baseProperties.hp = monster.characteristic.Max_Health;
        monster._actionPoints = 0;
        monster._listAction = new List<ActionHandle>();
        monster.idUserSocketIO = "";
    }

    public static CharacterPlayer LoadCharacterEnemy()
    {
        CharacterPlayer character = new CharacterPlayer();

        character._baseProperties = new CharacterProperties(int.Parse(SplitDataFromServe._heroEnemyPlay.idh), SplitDataFromServe._heroEnemyPlay.idcode, SplitDataFromServe._heroEnemyPlay.idclass, SplitDataFromServe._heroEnemyPlay.name, int.Parse(SplitDataFromServe._heroEnemyPlay.level)
            , double.Parse(SplitDataFromServe._heroEnemyPlay.exp), int.Parse(SplitDataFromServe._heroCurrentPLay.energy), int.Parse(SplitDataFromServe._heroCurrentPLay.heart), int.Parse(SplitDataFromServe._heroCurrentPLay.rank), double.Parse(SplitDataFromServe._heroEnemyPlay.skillpoint), int.Parse(SplitDataFromServe._heroEnemyPlay.point), int.Parse(SplitDataFromServe._heroEnemyPlay.levelmap), double.Parse(SplitDataFromServe._heroEnemyPlay.gold), int.Parse(SplitDataFromServe._heroEnemyPlay.diamond), int.Parse(SplitDataFromServe._heroCurrentPLay.slotchest), int.Parse(SplitDataFromServe._heroEnemyPlay.type), int.Parse(SplitDataFromServe._heroEnemyPlay.hp),
            float.Parse(SplitDataFromServe._heroEnemyPlay.strength), float.Parse(SplitDataFromServe._heroEnemyPlay.intelligence), float.Parse(SplitDataFromServe._heroEnemyPlay.dexterity), float.Parse(SplitDataFromServe._heroEnemyPlay.focus), float.Parse(SplitDataFromServe._heroEnemyPlay.luck),
            float.Parse(SplitDataFromServe._heroEnemyPlay.endurance), float.Parse(SplitDataFromServe._heroEnemyPlay.blessing), float.Parse(SplitDataFromServe._heroEnemyPlay.fighting));

        character._myEquipments = SplitDataFromServe._equipmentCurrentEnemy;
        
        character._actionPoints = 0;
        character._listAction = new List<ActionHandle>();
        return character;
    }

    public static void LoadCharacterEnemy(CharacterPlayer character)
    {


        character._baseProperties = new CharacterProperties(int.Parse(SplitDataFromServe._heroEnemyPlay.idh), SplitDataFromServe._heroEnemyPlay.idcode, SplitDataFromServe._heroEnemyPlay.idclass, SplitDataFromServe._heroEnemyPlay.name, int.Parse(SplitDataFromServe._heroEnemyPlay.level)
            , double.Parse(SplitDataFromServe._heroEnemyPlay.exp), int.Parse(SplitDataFromServe._heroCurrentPLay.energy), int.Parse(SplitDataFromServe._heroCurrentPLay.heart), int.Parse(SplitDataFromServe._heroCurrentPLay.rank), double.Parse(SplitDataFromServe._heroEnemyPlay.skillpoint), int.Parse(SplitDataFromServe._heroEnemyPlay.point), int.Parse(SplitDataFromServe._heroEnemyPlay.levelmap), double.Parse(SplitDataFromServe._heroEnemyPlay.gold), int.Parse(SplitDataFromServe._heroEnemyPlay.diamond), int.Parse(SplitDataFromServe._heroEnemyPlay.slotchest), int.Parse(SplitDataFromServe._heroEnemyPlay.type), int.Parse(SplitDataFromServe._heroEnemyPlay.hp),
            float.Parse(SplitDataFromServe._heroEnemyPlay.strength), float.Parse(SplitDataFromServe._heroEnemyPlay.intelligence), float.Parse(SplitDataFromServe._heroEnemyPlay.dexterity), float.Parse(SplitDataFromServe._heroEnemyPlay.focus), float.Parse(SplitDataFromServe._heroEnemyPlay.luck),
            float.Parse(SplitDataFromServe._heroEnemyPlay.endurance), float.Parse(SplitDataFromServe._heroEnemyPlay.blessing), float.Parse(SplitDataFromServe._heroEnemyPlay.fighting));

        character._myEquipments = SplitDataFromServe._equipmentCurrentEnemy;

        for (int i = 0; i < 10; i++)
        {
            Equipment equipment = null;
              switch (i)
            {
                case 0:
                    if (character._myEquipments.weaponItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.weaponItem);
                    }
                    break;
                case 1:
                    if (character._myEquipments.shieldItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.shieldItem);
                    }
                    break;
                case 2:
                    if (character._myEquipments.headItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.headItem);
                    }
                    break;
                case 3:
                    if (character._myEquipments.torsoItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.torsoItem);
                    }
                    break;
                case 4:
                    if (character._myEquipments.legItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.legItem);
                    }
                    break;
                case 5:
                    if (character._myEquipments.beltItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.beltItem);
                    }
                    break;
                case 6:
                    if (character._myEquipments.glovesItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.glovesItem);
                    }
                    break;
                case 7:
                    if (character._myEquipments.bootsItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.bootsItem);
                    }
                    break;
                case 8:
                    if (character._myEquipments.ringItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.ringItem);
                    }
                    break;
                case 9:
                    if (character._myEquipments.amuletItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.amuletItem);
                    }
                    break;
                case 10:
                    if (character._myEquipments.avatarItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.avatarItem);
                    }
                    break;
                case 11:
                    if (character._myEquipments.buffItem != null)
                    {
                        equipment = Equipment.convertToEquipment(character._myEquipments.buffItem);
                    }
                    break;
            }
            if (equipment != null) character.equipments[i] = equipment;
            //character.playerId = int.Parse(SplitDataFromServe._heroEnemyPlay.idh);
            character.characteristic = new Characteristic((Characteristic.CharacterType)int.Parse(SplitDataFromServe._heroEnemyPlay.type), (Characteristic.CharacterClass)SplitDataFromServe._heroEnemyPlay.idclass,
               float.Parse(SplitDataFromServe._heroEnemyPlay.level), float.Parse(SplitDataFromServe._heroEnemyPlay.strength), float.Parse(SplitDataFromServe._heroEnemyPlay.intelligence),
               float.Parse(SplitDataFromServe._heroEnemyPlay.dexterity), float.Parse(SplitDataFromServe._heroEnemyPlay.focus), float.Parse(SplitDataFromServe._heroEnemyPlay.vitality), 0,
                float.Parse(SplitDataFromServe._heroEnemyPlay.luck), float.Parse(SplitDataFromServe._heroEnemyPlay.endurance), float.Parse(SplitDataFromServe._heroEnemyPlay.blessing));
        }

        character._baseProperties.hp = character.characteristic.Max_Health;
        character._actionPoints = 0;
        character._listAction = new List<ActionHandle>();
    }

    
}
