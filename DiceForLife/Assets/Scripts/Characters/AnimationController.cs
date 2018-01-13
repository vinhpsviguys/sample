using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using CoreLib;

public class AnimationController : MonoBehaviour {
    [SpineAnimation("Idle")]
    public string idleAnimation= "Idle";

    [SpineAnimation]
    public string attackAnimation= "Attack";

    [SpineAnimation]
    public string moveAnimation= "Move";
    [SpineAnimation]
    public string dieAnimation = "Die";
    [SpineAnimation]
    public string parriAnimation = "Damaged";

    SkeletonAnimator skeletonAnimator;
    Animator skeAnimator;
    void Start()
    {
        skeletonAnimator = GetComponent<SkeletonAnimator>();
        skeAnimator = GetComponent<Animator>();
        //skeletonAnimation.AnimationName = idleAnimation;
        
    }


    public void MeeleAttack(CharacterPlayer atk, CharacterPlayer def, int playerId, int indexState, int health, bool isnormalAtk,NewSkill useSkill )
    {
        Debug.Log("melee atack");
        //skeletonAnimation.AnimationName = moveAnimation;
        skeletonAnimator.Skeleton.FlipX = false;
        Vector3 oldPos = this.transform.position;
        float newPosX;
        if (transform.position.x < def.transform.position.x)
        {
            newPosX = def.transform.position.x - def.GetComponent<BoxCollider2D>().bounds.size.x;
        }
        else
        {
            newPosX = def.transform.position.x + def.GetComponent<BoxCollider2D>().bounds.size.x;
        }
        if (CharacterManager.Instance._meCharacter == atk)
        {
            BattleSceneUI.Instance._effectParentMe.transform.DOMoveX(newPosX, 0.1f);
        }
        if (CharacterManager.Instance._enemyCharacter == atk)
        {
            BattleSceneUI.Instance._effectParentEnemy.transform.DOMoveX(newPosX, 0.1f);
        }
        transform.DOMoveX(newPosX, 0.1f).OnComplete(()=>AttackandBack(atk,def,oldPos, playerId, indexState, health, isnormalAtk, useSkill));
    }
    public void AttackandBack(CharacterPlayer atk, CharacterPlayer deff, Vector3 oldPos, int playerId, int indexState, int health, bool isnormalAtk, NewSkill useSkill)
    {
        
        StartCoroutine(ExecuteAttackandBack(atk,deff,oldPos, playerId, indexState, health, isnormalAtk, useSkill));
    }

    public IEnumerator ExecuteAttackandBack(CharacterPlayer atk, CharacterPlayer def,Vector3 oldPos, int playerId,int indexState, int health, bool isnormalAtk, NewSkill useSkill)
    {
        skeAnimator.SetBool("attack", true);
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length/2f-0.01f);
        if (isnormalAtk)
        {
            if (CharacterManager.Instance._meCharacter.playerId == playerId)
            {
                GameObject slash = EffectManager.Instance.CreateEffect("Effect/Slash", this.transform);
                slash.transform.position = new Vector3(atk.transform.position.x + atk.transform.GetComponent<BoxCollider2D>().bounds.size.x / 2, atk.transform.position.y + atk.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, atk.transform.position.z);
            }
            else if (CharacterManager.Instance._meCharacter.playerId != playerId)
            {
                GameObject slash = EffectManager.Instance.CreateEffect("Effect/Slash", this.transform);
                slash.transform.localScale = new Vector3(-1, 1,1);
                slash.transform.position = new Vector3(atk.transform.position.x - atk.transform.GetComponent<BoxCollider2D>().bounds.size.x / 2, atk.transform.position.y + atk.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, atk.transform.position.z);
            }
        }
        else
        {
            if (useSkill != null)
            {
                if (useSkill.data["type"].Value.ToString() == "passive")
                {

                    GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                    effSkill.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + this.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, this.transform.position.z);
                } else if (useSkill.data["type"].Value.ToString() != "passive")
                {
                   
                        GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/"+ useSkill.data["prefEffect"].Value.ToString(), this.transform);
                        effSkill.transform.position = new Vector3(def.transform.position.x, def.transform.position.y + def.transform.GetComponent<BoxCollider2D>().bounds.size.y / 2, def.transform.position.z);

                }
            }
        }
        SendDamage(playerId.ToString()+","+indexState.ToString()+","+health.ToString() + "," + "senddamge");
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length/2f);
        skeAnimator.SetBool("attack", false);
        transform.DOMoveX(oldPos.x, 0.25f);
        if (CharacterManager.Instance._meCharacter == atk)
        {
            BattleSceneUI.Instance._effectParentMe.transform.DOMoveX(oldPos.x, 0.1f);
        }
        if (CharacterManager.Instance._enemyCharacter == atk)
        {
            BattleSceneUI.Instance._effectParentEnemy.transform.DOMoveX(oldPos.x, 0.1f);
        }

    }


    public void SendDamage(string data)
    {
        this.PostEvent(EventID.OnCharacterUpdateUIState, data);
    }

    public void RangeAttack(CharacterPlayer atk, CharacterPlayer def, int playerId, int indexState, int health, bool isnormalAtk, NewSkill useSkill)
    {
        Transform hostPos = this.transform;
        Transform newPosX;
        newPosX = def.transform;
        StartCoroutine(ExecuteRangeAttack(atk,def, hostPos, newPosX, playerId, indexState,health, isnormalAtk,useSkill));
    }

    public IEnumerator ExecuteRangeAttack(CharacterPlayer _atk, CharacterPlayer _target, Transform hostPos, Transform newPos, int playerId, int indexState, int health, bool isnormalAtk, NewSkill useSkill)
    {
        skeAnimator.SetBool("attack", true);
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length / 2f-0.01f);

        if (isnormalAtk)
        {

            GameObject bullet = EffectManager.Instance.CreateEffect("Prefabs/RangeAttack", hostPos);
            bullet.transform.position = new Vector3(hostPos.position.x + hostPos.GetComponent<BoxCollider2D>().bounds.size.x / 2, hostPos.position.y + hostPos.GetComponent<BoxCollider2D>().bounds.size.y / 2, hostPos.position.z);
            EffectManager.Instance.EffectMoving(bullet,newPos.position.x, 0.5f);
  
        }
        else
        {
            if (useSkill != null)
            {
                if (useSkill.data["type"].Value.ToString() == "passive")
                {

                }
                else if (useSkill.data["type"].Value.ToString() != "passive")
                {
                   
                        GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                        effSkill.transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y + _target.GetComponent<BoxCollider2D>().bounds.size.y / 2, _target.transform.position.z);
                }
            }
        }


      
        SendDamage(playerId.ToString() + "," + indexState.ToString() + "," + health.ToString() + "," + "senddamge");
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length / 2f);
        skeAnimator.SetBool("attack", false);
    }

    public void UseSkill(CharacterPlayer meCharacter, CharacterPlayer enemyCharacter, int playerId, int indexState, int health, NewSkill useSkill)
    {
        StartCoroutine(ExecuteSkill(meCharacter, enemyCharacter, playerId, indexState, health,useSkill));
    }
    public IEnumerator ExecuteSkill(CharacterPlayer meCharacter, CharacterPlayer enemyCharacter, int playerId, int indexState, int health, NewSkill useSkill)
    {
        skeAnimator.SetBool("attack", true);
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length / 2f-0.01f);
        if (meCharacter != enemyCharacter)
        {
            if (meCharacter.playerId == playerId)
            {
                if (useSkill != null)
                {
                    GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                    if (useSkill.data["typeDisplayEffect"].Value == "me")
                    {
                        effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                    }
                    else if (useSkill.data["typeDisplayEffect"].Value == "enemy")
                    {
                        effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                        effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    }
                    else if (useSkill.data["typeDisplayEffect"].Value == "move")
                    {
                        effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + meCharacter.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                        EffectManager.Instance.EffectMoving(effSkill, enemyCharacter.transform.position.x, 0.5f);
                    }
                    else if (useSkill.data["typeDisplayEffect"].Value == "center")
                    {
                        effSkill.transform.position = Vector3.zero;
                    }
                    else
                    {
                        effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + enemyCharacter.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                        effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    }
                }
            }
            else if (enemyCharacter.playerId == playerId)
            {
                Debug.Log("name skill " + useSkill.data["name"].Value.ToString());
                Debug.Log("name eff " + useSkill.data["prefEffect"].Value.ToString());
                GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                if (useSkill.data["typeDisplayEffect"].Value == "me")
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "enemy")
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "move")
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + enemyCharacter.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    EffectManager.Instance.EffectMoving(effSkill, meCharacter.transform.position.x, 0.5f);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "center")
                {
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    effSkill.transform.position = Vector3.zero;
                }
                else
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + meCharacter.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                }
            }
            SendDamage(playerId.ToString() + "," + indexState.ToString() + "," + health.ToString()+ "," +"senddamge");
        } else
        {
            if (useSkill != null)
            {
                GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                if (useSkill.data["typeDisplayEffect"].Value == "me")
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "enemy")
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "move")
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + meCharacter.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                    EffectManager.Instance.EffectMoving(effSkill, enemyCharacter.transform.position.x, 0.5f);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "center")
                {
                    effSkill.transform.position = Vector3.zero;
                }
                else
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + enemyCharacter.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                }
            }
            SendDamage(playerId.ToString() + "," + indexState.ToString() + "," + health.ToString() + "," + "dontsend");
        }
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length / 2f);
        skeAnimator.SetBool("attack", false);

    }

   

    public void Donothing(CharacterPlayer meCharacter, CharacterPlayer enemyCharacter, int playerId, int indexState, int health, NewSkill useSkill)
    {
        StartCoroutine(ExecuteDonothing(meCharacter, enemyCharacter, playerId, indexState, health, useSkill));
    }

    public IEnumerator ExecuteDonothing(CharacterPlayer meCharacter, CharacterPlayer enemyCharacter, int playerId, int indexState, int health, NewSkill useSkill)
    {
        yield return new WaitForSeconds(0.5f);
        if (meCharacter != enemyCharacter)
        {
            if (meCharacter.playerId == playerId)
            {
                if (useSkill != null)
                {
                    GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                    if (useSkill.data["typeDisplayEffect"].Value == "me")
                    {
                        effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                    }
                    else if (useSkill.data["typeDisplayEffect"].Value == "enemy")
                    {
                        effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                        effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    }
                    else if (useSkill.data["typeDisplayEffect"].Value == "move")
                    {
                        effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + meCharacter.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                        EffectManager.Instance.EffectMoving(effSkill, enemyCharacter.transform.position.x, 0.5f);
                    }
                    else if (useSkill.data["typeDisplayEffect"].Value == "center")
                    {
                        effSkill.transform.position = Vector3.zero;
                    }
                    else
                    {
                        effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + enemyCharacter.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                        effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    }
                }
            }
            else if (enemyCharacter.playerId == playerId)
            {
                Debug.Log("name skill " + useSkill.data["name"].Value.ToString());
                Debug.Log("name eff " + useSkill.data["prefEffect"].Value.ToString());
                GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                if (useSkill.data["typeDisplayEffect"].Value == "me")
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "enemy")
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "move")
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + enemyCharacter.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    EffectManager.Instance.EffectMoving(effSkill, meCharacter.transform.position.x, 0.5f);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "center")
                {
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                    effSkill.transform.position = Vector3.zero;
                }
                else
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + meCharacter.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                }
            }
            SendDamage(playerId.ToString() + "," + indexState.ToString() + "," + health.ToString() + "," + "senddamge");
        }
        else
        {
            if (useSkill != null)
            {
                GameObject effSkill = EffectManager.Instance.CreateEffect("Effect/" + useSkill.data["prefEffect"].Value.ToString(), this.transform);
                if (useSkill.data["typeDisplayEffect"].Value == "me")
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "enemy")
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + effSkill.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "move")
                {
                    effSkill.transform.position = new Vector3(meCharacter.transform.position.x, meCharacter.transform.position.y + meCharacter.GetComponent<BoxCollider2D>().size.y / 2, meCharacter.transform.position.z);
                    EffectManager.Instance.EffectMoving(effSkill, enemyCharacter.transform.position.x, 0.5f);
                }
                else if (useSkill.data["typeDisplayEffect"].Value == "center")
                {
                    effSkill.transform.position = Vector3.zero;
                }
                else
                {
                    effSkill.transform.position = new Vector3(enemyCharacter.transform.position.x, enemyCharacter.transform.position.y + enemyCharacter.GetComponent<BoxCollider2D>().size.y / 2, enemyCharacter.transform.position.z);
                    effSkill.transform.localScale = new Vector3(effSkill.transform.localScale.x * -1, effSkill.transform.localScale.y, effSkill.transform.localScale.z);
                }
            }
            SendDamage(playerId.ToString() + "," + indexState.ToString() + "," + health.ToString() + "," + "dontsend");
        }
    }

    public void GetDamage(int playerId, Transform target)
    {
        StartCoroutine(ExecuteGetDamage(playerId, target));
    }

    public IEnumerator ExecuteGetDamage(int playerId, Transform _target)
    {
       
        GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Hit2", _target);
        damagedEffect.transform.position = new Vector3(_target.position.x, _target.position.y + _target.GetComponent<BoxCollider2D>().bounds.size.y / 2, _target.position.z);
        skeAnimator.SetBool("damaged", true);
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length-0.01f);
        skeAnimator.SetBool("damaged", false);
    }


    public void Die(int _playerId, Transform _target)
    {
        StartCoroutine(ExecuteDie(_playerId,_target));
    }
    IEnumerator ExecuteDie(int _playerId, Transform _target)
    {
        GameObject damagedEffect = EffectManager.Instance.CreateEffect("Effect/Hit2", _target);
        damagedEffect.transform.position = new Vector3(_target.position.x, _target.position.y + _target.GetComponent<BoxCollider2D>().bounds.size.y / 2, _target.position.z);
        Debug.Log("die");
        skeAnimator.SetBool("die", true);
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(skeAnimator.GetCurrentAnimatorStateInfo(0).length-0.01f);
        //this.PostEvent(EventID.OnBattleEnd);
    }
   
}
