/*
 * Đây là class thể hiện các thông tin cơ bản của nhân vật. Nhiều vãi
 */
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public enum ClassCharacter
{
    Barbarian = 0,
    Orc,
    Marksman,
    Wizard,
    Sorceress,
    Cleric,
    Paladin,
    Assassin,
    None
}

public enum RaceCharacter
{
    None=0,
    Human,
    Magician,
    Undead,

}

public enum JobCharacter
{
    None=0,

}



public  class CharacterProperties
{
    public int idHero;
    public string idCodeHero;
    //Hero des
    //Level Proerties
    public int Level;
    public double Exp;
    public double Gold;
    public int EnergyPoint;
    public int HeartPoint;
    public int PvpPoint;
    public double SkillPoint;
    public int AttributePoint;
    public int LevelMap;

    public int EventPoint;
    public int Diamond;
    public int SlotChest;
    // 0 - meele, 1 -  ranger
    public int typeAttack = 0;


    public RaceCharacter _raceCharacter;

    public int _idclassCharacter;
    public ClassCharacter _classCharacter;
    public JobCharacter _jobCharacter;

    public string name;
    public string description="";
    public string typeMonster = "";
   


    public float FightingPower;
    //Base Properties
    public float hp;
    public float Strength;
    public float Intelligence;
    public float Vitality;
    public float Dexterity;
    public float Agility;
    public float Focus;
    public float Luck;
    public float Endurance;
    public float Blessing;
    public float Protection;

  
    public float Accuracy;
   

    //Power point

    //Item or Skill Properties
    public float minPhysicalDamage;
    public float maxPhysicalDamage;

    public float minMagicalDamage;
    public float maxMagicalDamage;


    public float minPhysicalReinforce;
    public float maxPhysicalReinforce;

    public float minMagicalReinforce;
    public float maxMagicalReinforce;

    public float minAttackRate;
    public float maxAttackRate;

    public float minParryRate;
    public float maxParryRate;

    public float minBlockChance;
    public float maxBlockChance;

    public float Breakshield;
    public float BreakWeapon;

    public float minPhysicalRedution;
    public float maxPhysicalRedution;
    public float minMagicalRedution;
    public float maxMagicalRedution;

    public float minPhysicalAbsorption;
    public float maxPhysicalAbsorption;
    public float minMagicalAbsorption;
    public float maxMagicalAbsorption;

    public float minPhysicalDef;
    public float maxPhysicalDef;
    public float minMagicalDef;
    public float maxMagicalDef;

    public float physicalDamageReinPercent;
    public float magicalDamageReinPercent;
    public float physicalAbsorbPercent;
    public float magicalAbsorbPercent;

   


    // Properties from item
    public float FreezeChance;
    public float PoisonChance;
    public float ElectricShockChance;
    public float BurnChance;
    public float KnockBackChance;
    public float ImmobilizationChance;
    public float BlindChance;
    public float DementiaChance;
    public float DiseaseChance;
    public float FearChance;
    public float SleepChance;
    public float GlamourChance;
    public float PainChance;
    public float CrazyChance;
    public float RotChance;
    public float HypnoticChance;
    public float BleedChance;
    public float BloodThirstChance;
    public float ImpotentChance;
    public float StunChance;


    public float FreezeDurationReduced;
    public float PoisonDurationReduce;
    public float ElectricShockDurationReduce;
    public float BurnDurationReduce;
    public float KnockBackResitance;
    public float ImmobilizationResitance;
    public float BlindResitance;
    public float DementiaResitance;
    public float DiseaseResitance;
    public float FearResitance;
    public float SleepResitance;
    public float GlamourResitance;
    public float PainResitance;
    public float CrazyResitance;
    public float RotResitance;
    public float HypnoticResitance;
    public float BleedResitance;
    public float BloodThirstResitance;
    public float ImpotentResitance;
    public float StunResitance;


    public float IncreaseGold;
    public float IncreaseExp;
    public float IncreaseSkillpoint;

    public float PureDamage;
    public float damageAgainsAssassin;
    public float damageAgainsPaladin;
    public float damageAgainsZealot;
    public float damageAgainsSorceress;
    public float damageAgainWizard;
    public float damageAgainMarksman;
    public float damageAgainOrg;
    public float damageAgainBarbarian;

    // LinhVT - properties
    public int PhysicalDamage;
    public float PhysicalReinforce;
    public int PhysicalDefense;
    public int MagicalDamage;
    public float MagicalReinforce;
    public int MagicalDefense;
    public float CriticalChance;
    public float CriticalDamage;
    public float MulticastChance;
    public float BlockingChance;
    public float DodgeChance;
    public float HealthRecovery;
    public int Reputation;
    public float AttackRate;
    public float ParryRate;

    public CharacterProperties()
    {

    }
    
    public CharacterProperties(int _idH,string _idCodeH, int _idClassH, string _nameH, int _levelH, double _expH,int _energyPoint, int _hearthPoint, int _pvpPoint, double _skillPoint, int _atributtePoint, int _levelmap, double _goldH, int _diamondH,int _slotchestH, int _typeH, int _hpH, float _strengthH, float _intelH, float _vitH, float _focusH, float _luckH, float _enduranceH, float _blessingH, float _fightingH, string _typeMonster="")
    {
        idHero = _idH;
        idCodeHero = _idCodeH;
        _idclassCharacter = _idClassH;
        _classCharacter = ConvertIdClassToClassName(_idClassH);
        name = _nameH;
        Level = _levelH;
        Exp = _expH;
        EnergyPoint = _energyPoint;
        HeartPoint = _hearthPoint;
        PvpPoint = _pvpPoint;
        SkillPoint = _skillPoint;
        AttributePoint = _atributtePoint;
        LevelMap = _levelmap;
        Gold = _goldH;
        Diamond = _diamondH;
        SlotChest = _slotchestH;
        typeAttack = _typeH;
        hp = _hpH;
        Strength = _strengthH;
        Intelligence = _intelH;
        Vitality = _vitH;
        Focus = _focusH;
        Luck = _luckH;
        Endurance = _enduranceH;
        Blessing = _blessingH;
        FightingPower = _fightingH;
        typeMonster = _typeMonster;
        
    }

    


   public void AddBonusItem(EquipmentItem _item)
    {
        minPhysicalDamage += float.Parse(_item.getValue("1").ToString());
        maxPhysicalDamage += float.Parse(_item.getValue("2").ToString());
        minMagicalDamage += float.Parse(_item.getValue("3").ToString());
        maxMagicalDamage += float.Parse(_item.getValue("4").ToString());
        CriticalChance += float.Parse(_item.getValue("5").ToString());
        MulticastChance += float.Parse(_item.getValue("6").ToString());
        minPhysicalReinforce += float.Parse(_item.getValue("7").ToString());
        maxPhysicalReinforce += float.Parse(_item.getValue("8").ToString());
        minMagicalReinforce += float.Parse(_item.getValue("9").ToString());
        maxMagicalReinforce += float.Parse(_item.getValue("10").ToString());
        minAttackRate += float.Parse(_item.getValue("11").ToString());
        maxAttackRate += float.Parse(_item.getValue("12").ToString());
        minParryRate += float.Parse(_item.getValue("13").ToString());
        maxParryRate += float.Parse(_item.getValue("14").ToString());
        minBlockChance += float.Parse(_item.getValue("15").ToString());
        maxBlockChance += float.Parse(_item.getValue("16").ToString());
        minPhysicalDef += float.Parse(_item.getValue("17").ToString());
        maxPhysicalDef += float.Parse(_item.getValue("18").ToString());
        minMagicalDef += float.Parse(_item.getValue("19").ToString());
        maxMagicalDef += float.Parse(_item.getValue("20").ToString());
        minPhysicalRedution += float.Parse(_item.getValue("21").ToString());
        maxPhysicalRedution += float.Parse(_item.getValue("22").ToString());
        minPhysicalAbsorption += float.Parse(_item.getValue("23").ToString());
        maxPhysicalAbsorption += float.Parse(_item.getValue("24").ToString());
        minMagicalAbsorption += float.Parse(_item.getValue("27").ToString());
        maxMagicalAbsorption += float.Parse(_item.getValue("28").ToString());
        try
        {
            var N = SimpleJSON.JSON.Parse(_item.getValue("listidproperty").ToString());

            for (int i = 0; i < N.Count; i++)
            {
                foreach (KeyValuePair<string, JSONNode> _temp in N[i].AsObject)
                {
                    AddBonusProperties(int.Parse(_temp.Key.ToString()), float.Parse(_temp.Value.ToString()));
                }
            }
        } catch(Exception e)
        {

        }
    }

    public void RemoveBonusItem(EquipmentItem _item)
    {
        minPhysicalDamage -= float.Parse(_item.getValue("1").ToString());
        maxPhysicalDamage -= float.Parse(_item.getValue("2").ToString());
        minMagicalDamage -= float.Parse(_item.getValue("3").ToString());
        maxMagicalDamage -= float.Parse(_item.getValue("4").ToString());
        CriticalChance -= float.Parse(_item.getValue("5").ToString());
        MulticastChance -= float.Parse(_item.getValue("6").ToString());
        minPhysicalReinforce -= float.Parse(_item.getValue("7").ToString());
        maxPhysicalReinforce -= float.Parse(_item.getValue("8").ToString());
        minMagicalReinforce -= float.Parse(_item.getValue("9").ToString());
        maxMagicalReinforce -= float.Parse(_item.getValue("10").ToString());
        minAttackRate -= float.Parse(_item.getValue("11").ToString());
        maxAttackRate -= float.Parse(_item.getValue("12").ToString());
        minParryRate -= float.Parse(_item.getValue("13").ToString());
        maxParryRate -= float.Parse(_item.getValue("14").ToString());
        minBlockChance -= float.Parse(_item.getValue("15").ToString());
        maxBlockChance -= float.Parse(_item.getValue("16").ToString());
        minPhysicalDef -= float.Parse(_item.getValue("17").ToString());
        maxPhysicalDef -= float.Parse(_item.getValue("18").ToString());
        minMagicalDef -= float.Parse(_item.getValue("19").ToString());
        maxMagicalDef -= float.Parse(_item.getValue("20").ToString());
        minPhysicalRedution -= float.Parse(_item.getValue("21").ToString());
        maxPhysicalRedution -= float.Parse(_item.getValue("22").ToString());
        minPhysicalAbsorption -= float.Parse(_item.getValue("23").ToString());
        maxPhysicalAbsorption -= float.Parse(_item.getValue("24").ToString());
        minMagicalAbsorption -= float.Parse(_item.getValue("27").ToString());
        maxMagicalAbsorption -= float.Parse(_item.getValue("28").ToString());

        try
        {
            var N = SimpleJSON.JSON.Parse(_item.getValue("listidproperty").ToString());

            for (int i = 0; i < N.Count; i++)
            {
                foreach (KeyValuePair<string, JSONNode> _temp in N[i].AsObject)
                {
                    SubBonusProperties(int.Parse(_temp.Key.ToString()), float.Parse(_temp.Value.ToString()));
                }
            }
        }
        catch (Exception e)
        {
            
        }
    }

    void AddBonusProperties(int id, float value)
    {
        switch (id)
        {
            case 1:
                minPhysicalDamage += value;
                break;
            case 2:
                maxPhysicalDamage += value;
                break;
            case 3:
                minMagicalDamage += value;
                break;
            case 4:
                maxMagicalDamage += value;
                break;
            case 5:
                CriticalChance += value;
                break;
            case 6:
                MulticastChance += value;
                break;
            case 7:
                minPhysicalReinforce += value;
                break;
            case 8:
                maxPhysicalReinforce += value;
                break;
            case 9:
                minMagicalReinforce += value;
                break;
            case 10:
                maxMagicalReinforce += value;
                break;
            case 11:
                minAttackRate += value;
                break;
            case 12:
                maxAttackRate += value;
                break;
            case 13:
                minParryRate += value;
                break;
            case 14:
                maxParryRate += value;
                break;
            case 15:
                minBlockChance += value;
                break;
            case 16:
                maxBlockChance += value;
                break;
            case 17:
                minPhysicalDef += value;
                break;
            case 18:
                maxPhysicalDef += value;
                break;
            case 19:
                minMagicalDef += value;
                break;
            case 20:
                maxMagicalDef += value;
                break;
            case 21:
                minPhysicalRedution += value;
                break;
            case 22:
                maxPhysicalRedution += value;
                break;
            case 23:
                minMagicalRedution += value;
                break;
            case 24:
                maxMagicalRedution += value;
                break;
            case 25:
                minPhysicalAbsorption += value;
                break;
            case 26:
                maxPhysicalAbsorption += value;
                break;
            case 27:
                minMagicalAbsorption += value;
                break;
            case 28:
                maxMagicalAbsorption += value;
                break;
            case 29:
                Strength += value;
                break;
            case 30:
                Intelligence += value;
                break;
            case 31:
                Vitality += value;
                break;
            case 32:
                Dexterity += value;
                break;
            case 33:
                Focus += value;
                break;
            case 34:
                Agility+=value;
                break;
            case 35:
                Luck += value;
                break;
            case 36:
                Endurance += value;
                break;
            case 37:
                Blessing += value;
                break;
            case 38:
                Protection += value;
                break;
            case 39:
                hp += value;
                break;
            case 40:
                Breakshield += value;
                break;
            case 41:
                Accuracy += value;
                break;
            case 42:
                BreakWeapon += value;
                break;
            case 43:
                FreezeChance += value;
                break;
            case 44:
                PoisonChance += value;
                break;
            case 45:
                ElectricShockChance += value;
                break;
            case 46:
                BurnChance += value;
                break;
            case 47:
                KnockBackChance += value;
                break;
            case 48:
                ImmobilizationChance += value;
                break;
            case 49:
                BlindChance += value;
                break;
            case 50:
                DementiaChance += value;
                break;
            case 51:
                DiseaseChance += value;
                break;
            case 52:
                FearChance += value;
                break;
            case 53:
                SleepChance += value;
                break;
            case 54:
                GlamourChance += value;
                break;
            case 55:
                PainChance += value;
                break;
            case 56:
                CrazyChance += value;
                break;
            case 57:
                RotChance += value;
                break;
            case 58:
                HypnoticChance += value;
                break;
            case 59:
                BleedChance += value;
                break;
            case 60:
                BloodThirstChance += value;
                break;
            case 61:
                ImpotentChance += value;
                break;
            case 62:
                StunChance += value;
                break;
            case 63:
                FreezeDurationReduced += value;
                break;
            case 64:
                PoisonDurationReduce += value;
                break;
            case 65:
                ElectricShockDurationReduce += value;
                break;
            case 66:
                BurnDurationReduce += value;
                break;
            case 67:
                KnockBackResitance += value;
                break;
            case 68:
                ImmobilizationResitance += value;
                break;
            case 69:
                BlindResitance += value;
                break;
            case 70:
                DementiaResitance += value;
                break;
            case 71:
                DiseaseResitance += value;
                break;
            case 72:
                FearResitance += value;
                break;
            case 73:
                SleepResitance += value;
                break;
            case 74:
                GlamourResitance += value;
                break;
            case 75:
                PainResitance += value;
                break;
            case 76:
                CrazyResitance += value;
                break;
            case 77:
                RotResitance += value;
                break;
            case 78:
                HypnoticResitance += value;
                break;
            case 79:
                BleedResitance += value;
                break;
            case 80:
                ImpotentResitance += value;
                break;
            case 81:
                StunResitance += value;
                break;
            case 82:
                IncreaseGold += value;
                break;
            case 83:
                IncreaseExp += value;
                break;
            case 84:
                IncreaseSkillpoint += value;
                break;
            case 85:
                Luck += value;
                break;
            case 86:
                Blessing += value;
                break;
            case 87:
                PureDamage += value;
                break;
            case 88:
                damageAgainsAssassin += value;
                break;
            case 89:
                damageAgainsPaladin += value;
                break;
            case 90:
                damageAgainsZealot += value;
                break;
            case 91:
                damageAgainsSorceress += value;
                break;
            case 92:
                damageAgainWizard += value;
                break;
            case 93:
                damageAgainWizard += value;
                break;
            case 94:
                damageAgainMarksman += value;
                break;
            case 95:
                damageAgainOrg += value;
                break;
            case 96:
                damageAgainBarbarian += value;
                break;
            case 97:
                hp += value;
                break;
            case 98:
                minPhysicalDamage += value;
                maxPhysicalDamage += value;
                break;
            case 99:
                minMagicalDamage += value;
                maxMagicalDamage += value;
                break;
            case 100:
                minPhysicalAbsorption += value;
                maxPhysicalAbsorption += value;
                break;
            case 101:
                maxMagicalAbsorption += value;
                minMagicalAbsorption += value;
                break;
        }
    }

    void SubBonusProperties(int id, float value)
    {
        switch (id)
        {
            case 1:
                minPhysicalDamage -= value;
                break;
            case 2:
                maxPhysicalDamage -= value;
                break;
            case 3:
                minMagicalDamage -= value;
                break;
            case 4:
                maxMagicalDamage -= value;
                break;
            case 5:
                CriticalChance -= value;
                break;
            case 6:
                MulticastChance -= value;
                break;
            case 7:
                minPhysicalReinforce -= value;
                break;
            case 8:
                maxPhysicalReinforce -= value;
                break;
            case 9:
                minMagicalReinforce -= value;
                break;
            case 10:
                maxMagicalReinforce -= value;
                break;
            case 11:
                minAttackRate -= value;
                break;
            case 12:
                maxAttackRate -= value;
                break;
            case 13:
                minParryRate -= value;
                break;
            case 14:
                maxParryRate -= value;
                break;
            case 15:
                minBlockChance -= value;
                break;
            case 16:
                maxBlockChance -= value;
                break;
            case 17:
                minPhysicalDef -= value;
                break;
            case 18:
                maxPhysicalDef -= value;
                break;
            case 19:
                minMagicalDef -= value;
                break;
            case 20:
                maxMagicalDef -= value;
                break;
            case 21:
                minPhysicalRedution -= value;
                break;
            case 22:
                maxPhysicalRedution -= value;
                break;
            case 23:
                minMagicalRedution -= value;
                break;
            case 24:
                maxMagicalRedution -= value;
                break;
            case 25:
                minPhysicalAbsorption -= value;
                break;
            case 26:
                maxPhysicalAbsorption -= value;
                break;
            case 27:
                minMagicalAbsorption -= value;
                break;
            case 28:
                maxMagicalAbsorption -= value;
                break;
            case 29:
                Strength -= value;
                break;
            case 30:
                Intelligence -= value;
                break;
            case 31:
                Vitality -= value;
                break;
            case 32:
                Dexterity -= value;
                break;
            case 33:
                Focus -= value;
                break;
            case 34:
                Agility -= value;
                break;
            case 35:
                Luck -= value;
                break;
            case 36:
                Endurance += value;
                break;
            case 37:
                Blessing -= value;
                break;
            case 38:
                Protection -= value;
                break;
            case 39:
                hp -= value;
                break;
            case 40:
                Breakshield -= value;
                break;
            case 41:
                Accuracy -= value;
                break;
            case 42:
                BreakWeapon -= value;
                break;
            case 43:
                FreezeChance -= value;
                break;
            case 44:
                PoisonChance -= value;
                break;
            case 45:
                ElectricShockChance -= value;
                break;
            case 46:
                BurnChance -= value;
                break;
            case 47:
                KnockBackChance -= value;
                break;
            case 48:
                ImmobilizationChance -= value;
                break;
            case 49:
                BlindChance -= value;
                break;
            case 50:
                DementiaChance -= value;
                break;
            case 51:
                DiseaseChance -= value;
                break;
            case 52:
                FearChance -= value;
                break;
            case 53:
                SleepChance -= value;
                break;
            case 54:
                GlamourChance -= value;
                break;
            case 55:
                PainChance -= value;
                break;
            case 56:
                CrazyChance -= value;
                break;
            case 57:
                RotChance -= value;
                break;
            case 58:
                HypnoticChance -= value;
                break;
            case 59:
                BleedChance -= value;
                break;
            case 60:
                BloodThirstChance -= value;
                break;
            case 61:
                ImpotentChance -= value;
                break;
            case 62:
                StunChance -= value;
                break;
            case 63:
                FreezeDurationReduced -= value;
                break;
            case 64:
                PoisonDurationReduce -= value;
                break;
            case 65:
                ElectricShockDurationReduce -= value;
                break;
            case 66:
                BurnDurationReduce -= value;
                break;
            case 67:
                KnockBackResitance -= value;
                break;
            case 68:
                ImmobilizationResitance -= value;
                break;
            case 69:
                BlindResitance -= value;
                break;
            case 70:
                DementiaResitance -= value;
                break;
            case 71:
                DiseaseResitance -= value;
                break;
            case 72:
                FearResitance -= value;
                break;
            case 73:
                SleepResitance -= value;
                break;
            case 74:
                GlamourResitance -= value;
                break;
            case 75:
                PainResitance -= value;
                break;
            case 76:
                CrazyResitance -= value;
                break;
            case 77:
                RotResitance -= value;
                break;
            case 78:
                HypnoticResitance -= value;
                break;
            case 79:
                BleedResitance -= value;
                break;
            case 80:
                ImpotentResitance -= value;
                break;
            case 81:
                StunResitance -= value;
                break;
            case 82:
                IncreaseGold -= value;
                break;
            case 83:
                IncreaseExp -= value;
                break;
            case 84:
                IncreaseSkillpoint -= value;
                break;
            case 85:
                Luck -= value;
                break;
            case 86:
                Blessing -= value;
                break;
            case 87:
                PureDamage -= value;
                break;
            case 88:
                damageAgainsAssassin -= value;
                break;
            case 89:
                damageAgainsPaladin -= value;
                break;
            case 90:
                damageAgainsZealot -= value;
                break;
            case 91:
                damageAgainsSorceress -= value;
                break;
            case 92:
                damageAgainWizard -= value;
                break;
            case 93:
                damageAgainWizard -= value;
                break;
            case 94:
                damageAgainMarksman -= value;
                break;
            case 95:
                damageAgainOrg -= value;
                break;
            case 96:
                damageAgainBarbarian -= value;
                break;
            case 97:
                hp -= value;
                break;
            case 98:
                minPhysicalDamage -= value;
                maxPhysicalDamage -= value;
                break;
            case 99:
                minMagicalDamage -= value;
                maxMagicalDamage -= value;
                break;
            case 100:
                minPhysicalAbsorption -= value;
                maxPhysicalAbsorption -= value;
                break;
            case 101:
                maxMagicalAbsorption -= value;
                minMagicalAbsorption -= value;
                break;
        }
    }


    ClassCharacter ConvertIdClassToClassName(int id)
    {
        ClassCharacter tempClass = ClassCharacter.None;
        switch (id)
        {
            case 1:
                tempClass= ClassCharacter.Assassin;
                break;
            case 2:
                tempClass = ClassCharacter.Paladin;
                break;
            case 3:
                tempClass = ClassCharacter.Cleric;
                break;
            case 4:
                tempClass = ClassCharacter.Sorceress;
                break;
            case 5:
                tempClass = ClassCharacter.Wizard;
                break;
            case 6:
                tempClass = ClassCharacter.Marksman;
                break;
            case 7:
                return ClassCharacter.Orc;
                break;
            case 8:
                tempClass = ClassCharacter.Barbarian;
                break;
        }
        return tempClass;
    }
}
