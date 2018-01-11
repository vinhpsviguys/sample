
using System.Collections.Generic;
using CoreLib;

/*
 * Đây là class thể hiện tất cẩ các skill trong game
 */
public class SkillCharacter
{
    public int idSkill;
    public int idInitSkill;
    public int _classCharacter;
    public string nameSkill;
    public string descriptionSkill;
    public int levelSkill;
    public float damageSkill;
    public float physicalDamage;
    public float magicalDamage;
    public float pureDamage;
    public int actionPointRequired;

    /// <summary>
    /// Bonus
    /// </summary>
    public bool requireTarget;
    public bool canCastOnSelf;

    //public SkillCharacter(int _idSkill, ClassCharacter _classChar, string name, string des, int level, float damage, List<EffectBuffCharacter> effect, int actionPoint)
    //{
    //    idSkill = _idSkill;
    //    _classCharacter = (int)_classChar;
    //    nameSkill = name;
    //    descriptionSkill = des;
    //    levelSkill = level;
    //    damageSkill = damage;
    //    //effectSkill = effect;
    //    actionPointRequired = actionPoint;

    //}
    public SkillCharacter(int _idSkill, int _idInitSkill, ClassCharacter _classChar, string name, string des, int level, int _tempPhysicDamage, int _tempMagicDame, int _tempPureDame, int actionPoint)
    {
        idSkill = _idSkill;
        idInitSkill = _idInitSkill;
        _classCharacter = (int)_classChar;
        nameSkill = name;
        descriptionSkill = des;
        levelSkill = level;
        physicalDamage = _tempPhysicDamage;
        magicalDamage = _tempMagicDame;
        pureDamage = _tempPureDame;
        actionPointRequired = actionPoint;

    }


    public SkillCharacter(int _idSkill, int _classChar, string name, string des, int level, float damage, int actionPoint)
    {
        idSkill = _idSkill;
        _classCharacter = (int)_classChar;
        nameSkill = name;
        descriptionSkill = des;
        levelSkill = level;
        damageSkill = damage;
        //effectSkill = effect;
        actionPointRequired = actionPoint;

    }

    public void SetData(SkillCharacter _skill)
    {

        idSkill = _skill.idSkill;
        _classCharacter = _skill._classCharacter;
        nameSkill = _skill.nameSkill;
        descriptionSkill = _skill.descriptionSkill;
        levelSkill = _skill.levelSkill;
        damageSkill = _skill.damageSkill;
        //effectSkill = _skill.effectSkill;
        actionPointRequired = _skill.actionPointRequired;
    }
}
