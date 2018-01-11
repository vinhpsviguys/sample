
using System.Collections.Generic;
using UnityEngine;

public enum EffectBuffType
{
    None=0,
    Stun,
    Silent,
    Root,
    Healing,
    Burning,
    IncreasePhysicDamage,
    IncreaseMageDamage,
    
}



/*
 * Đây là class thể hiện các hiệu ứng tác động lên player trong game
 */
public class EffectBuffCharacter
{
    public int idEffect;
    public EffectBuffType effectBuff;
    /// <summary>
    /// 0 - buff, 1- debuff
    /// </summary>
    public int source;
    public string nameEffect;
    public string descriptionEffect;
    public float valueEffect;
    public int turnDuration;

    public EffectBuffCharacter(int id, EffectBuffType effect, int _source, string name, string des, float value, int turn)
    {

        idEffect = id;
        effectBuff = effect;
        source = _source;
        nameEffect = name;
        descriptionEffect = des;
        valueEffect = value;
        turnDuration = turn;
    }

    public void SetDataEffect(EffectBuffCharacter _effect)
    {
        idEffect = _effect.idEffect;
        effectBuff = _effect.effectBuff;
        nameEffect = _effect.nameEffect;
        source = _effect.source;
        descriptionEffect = _effect.descriptionEffect;
        valueEffect = _effect.valueEffect;
        turnDuration = _effect.turnDuration;

    }
}