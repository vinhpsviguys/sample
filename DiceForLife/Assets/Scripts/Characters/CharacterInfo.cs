
using CoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public static CharacterInfo _instance;
    internal CharacterProperties _baseProperties;
    internal EquipmentsCharacter _myEquipments = new EquipmentsCharacter();
    internal List<SkillCharacter> _mySkills = new List<SkillCharacter>();

    internal int _currentHP;
    internal int _skillPoints;
    internal int _statPoints;
    internal int _actionPoints;
    internal float _damaged;

    internal bool isReady = false;
    internal bool isDoingNormalAttack = false;
    internal string idUserSocketIO = "";
    internal int keyPlayer = 0;

    void Awake()
    {
        _instance = this;
        LoadCharacterProperties();
    }

    private void Start()
    {
        //LoadCharacterProperties();
    }

    public void LoadCharacterProperties()
    {
            _baseProperties = new CharacterProperties(int.Parse(SplitDataFromServe._heroCurrentPLay.idh),
            SplitDataFromServe._heroCurrentPLay.idcode, 
            SplitDataFromServe._heroCurrentPLay.idclass, 
            SplitDataFromServe._heroCurrentPLay.name,
            int.Parse(SplitDataFromServe._heroCurrentPLay.level),
            double.Parse(SplitDataFromServe._heroCurrentPLay.exp), 
            0, 0, 0, 
            double.Parse(SplitDataFromServe._heroCurrentPLay.skillpoint), 
            int.Parse(SplitDataFromServe._heroCurrentPLay.point), 
            int.Parse(SplitDataFromServe._heroCurrentPLay.levelmap) ,
            double.Parse(SplitDataFromServe._heroCurrentPLay.gold), 
            int.Parse(SplitDataFromServe._heroCurrentPLay.diamond),
            int.Parse(SplitDataFromServe._heroCurrentPLay.slotchest),
            int.Parse(SplitDataFromServe._heroCurrentPLay.type),
            int.Parse(SplitDataFromServe._heroCurrentPLay.hp),
            float.Parse(SplitDataFromServe._heroCurrentPLay.strength),
            float.Parse(SplitDataFromServe._heroCurrentPLay.intelligence), 
            float.Parse(SplitDataFromServe._heroCurrentPLay.vitality),
            float.Parse(SplitDataFromServe._heroCurrentPLay.focus), 
            float.Parse(SplitDataFromServe._heroCurrentPLay.luck),
            float.Parse(SplitDataFromServe._heroCurrentPLay.endurance), 
            float.Parse(SplitDataFromServe._heroCurrentPLay.blessing), 
            float.Parse(SplitDataFromServe._heroCurrentPLay.fighting));

        _myEquipments = SplitDataFromServe._equipmentCurrentHero;
        _currentHP = (int)_baseProperties.hp;
        UpdatePropertiesFromEquipment(_myEquipments, _baseProperties);
        _skillPoints = 0;
        _statPoints = 0;
        _actionPoints = 0;
        _damaged = 0;
        MainMenuUI._instance.UpdateTextValue();

    }
    public void UpdatePropertiesFromEquipment(EquipmentsCharacter _tempEquipment, CharacterProperties _tempProperties)
    {
        if (_tempEquipment.amuletItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.amuletItem);
        }
        if (_tempEquipment.beltItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.beltItem);
        }
        if (_tempEquipment.bootsItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.bootsItem);
        }
        if (_tempEquipment.glovesItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.glovesItem);
        }
        if (_tempEquipment.headItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.headItem);
        }
        if (_tempEquipment.legItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.legItem);
        }
        if (_tempEquipment.ringItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.ringItem);
        }
        if (_tempEquipment.shieldItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.shieldItem);
        }
        if (_tempEquipment.torsoItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.torsoItem);
        }
        if (_tempEquipment.weaponItem != null)
        {
            _tempProperties.AddBonusItem(_tempEquipment.weaponItem);
        }
    }

    //public void LoadCharacterProperties(int id)
    //{
    //    _baseProperties = CharacterItemInGame.Instance.LoadDataCharacter(id);
    //    _currentHP = (int)_baseProperties.hp;
    //    _skillPoints = 0;
    //    _statPoints = 0;
    //    _actionPoints = 0;
    //    _damaged = 0;
    //    if (_mySkills.Count==0)
    //    {
    //        _mySkills = DemoCreateSkill();
    //    }
    //}
    ///// <summary>
    ///// Create a new Character
    ///// </summary>
    //public void InitNewCharacterProperties(int _id, int _level, int _exp, RaceCharacter race, ClassCharacter _class, JobCharacter _job, string _name, string _des, int physicDmg, int physicDef, int magicDmg, int magicDef, int _hp, int _range)
    //{
    //    _baseProperties = new CharacterProperties();
    //    _baseProperties.StartNewCharacter(_id,_level,_exp,race,_class,_job,_name,_des,physicDmg,physicDef,magicDmg,magicDef,_hp,_range);
    //    //_mySkills = DemoCreateSkill();
    //    //_myEffects = new List<EffectBuffCharacter>();
    //    //_myEffects.Add(new EffectBuffCharacter(1, EffectBuffType.Root,1, "Troi", "Troi doi thu", 0f, 1));
    //    //_myEffects.Add(new EffectBuffCharacter(2, EffectBuffType.Burning,1, "Dot chay", "Dot chay doi thu", 150f, 3));
    //    _myEffects = null;
    //    _currentHP = _hp;
    //    _skillPoints = 0;
    //    _statPoints = 0;
    //    _actionPoints = 0;
    //    _damaged = 0;
    // }


    //public List<SkillCharacter> DemoCreateSkill()
    //{
    //    List<SkillCharacter> _tempList = CharacterItemInGame.Instance.LoadAllSkillOfHero(_baseProperties._classCharacter);
    //    List<SkillCharacter> _listSkill = new List<SkillCharacter>();
    //    int i= 1;
    //    while (i < 5)
    //    {
    //        int tempInt = Random.Range(0, _tempList.Count);
    //        _listSkill.Add(_tempList[tempInt]);
    //        _tempList.RemoveAt(tempInt);
    //        i++;
    //    }
    //    return _listSkill;
    //}

    public void LoadEquipmentSet()
    {

    }

    //public List<SkillCharacter> CreateARandomListSkill()
    //{
    //    List<SkillCharacter> randomList = new List<SkillCharacter>();

    //    SkillCharacter skill1= new SkillCharacter(1,_baseProperties._classCharacter,"Freeze","Dong ban doi phuong",10,300f,EffectBuffCharacter.)
    //}

    IEnumerator LoadData()
    {
        yield return null;
    }
}
