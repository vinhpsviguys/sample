using System;
using System.Collections;
using UnityEngine;

namespace CoreLib
{
    public class NewLogic // phan biet tac dung HP tung lan, HP dung ngay va mat (doc lap)
    {
        public const int LIMIT_TURNS = 40;
        //Character attacker;
        //Character defender;
        internal CharacterPlayer _attacker;
        internal CharacterPlayer _defender;
        public NewCharacterStatus attStatus;
        public NewCharacterStatus defStatus;

        ArrayList states = new ArrayList();
		int turns = 0;

		bool turnning = false;
        int secondAttack;
        private bool monitoring = true;
        PcgRandom random = new PcgRandom();

        public NewLogic(CharacterPlayer _attacker, CharacterPlayer _defender)
        {
            this._attacker = _attacker;
            this._defender = _defender;
            attStatus = new NewCharacterStatus(_attacker);
            defStatus = new NewCharacterStatus(_defender);
            secondAttack = _defender.playerId;

            turns = LIMIT_TURNS;
            //Debug.Log("attacker = " + attacker.toJSON().ToString());
            //Debug.Log("defender = " + defender.toJSON().ToString());
            Constants.init();
        }

        //public NewLogic(Character attacker, Character defender)
        //{
        //    //this.attacker = attacker;
        //    //this.defender = defender;
        //    attStatus = new NewCharacterStatus(attacker);
        //    defStatus = new NewCharacterStatus(defender);
        //    attStatus.setConditionManager(new ConditionManager(attStatus, this));
        //    defStatus.setConditionManager(new ConditionManager(defStatus, this));
        //    secondAttack = defender.playerId;

        //    turns = LIMIT_TURNS;
        //    //Debug.Log("attacker = " + attacker.toJSON().ToString());
        //    //Debug.Log("defender = " + defender.toJSON().ToString());
        //    Constants.init();
        //}


        public void setFirstID(int firstID)
        {
            this.secondAttack = 3 - firstID;
        }

		//public static NewCharacterStatus createStatus(Character character)
		//{
		//	return new NewCharacterStatus(character);
		//}

		public void beginTurnWithoutLogic(NewCharacterStatus attacker, NewCharacterStatus defender)
		{
			if (turnning) throw new Exception("endTurn must be called before beginTurn.");
			this.attStatus = attacker;
			this.defStatus = defender;
            this.attStatus.rebuildFormulaOfIndexes(defStatus);
            this.defStatus.rebuildFormulaOfIndexes(attStatus);
            calculateUIIndexes(attStatus);
            calculateUIIndexes(defStatus);
			turnning = true;
		}

		public void endTurnWithoutLogic(NewCharacterStatus attacker, NewCharacterStatus defender)
		{
			if (!turnning) throw new Exception("beginTurn must be called before endTurn.");
			if (attStatus != attacker && defStatus != defender) throw new Exception("Targets of endTurn must be same To beginTurn.");
			turnning = false;
            this.turns--;
        }


        private void calculateUIIndexes(NewCharacterStatus status) {
            status.calculateIndexAndSave(getStatusByPlayerID(3 - status.playerID), Indexes.max_hp_na);
            status.setHP((float)status.getCurrentIndex(Indexes.hp_na));

            foreach (string skill in status.character.newSkillDic.Keys) {
                float aps = Math.Max(1.0f, status.calculateIndex(getStatusByPlayerID(3 - status.playerID), skill + "_aps"));
                status.setIndex(status.curIndexes, skill + "_aps", aps);
                Debug.Log(skill + "_aps"+" "+aps);
            }
        }

		public BeginTurnResult beginTurn(NewCharacterStatus attacker, NewCharacterStatus defender) // code to fix glamour effect
		{
			if (turnning) throw new Exception("endTurn must be called before beginTurn.");
			this.attStatus = attacker;
			this.defStatus = defender;
            this.attStatus.rebuildFormulaOfIndexes(defStatus);
            this.defStatus.rebuildFormulaOfIndexes(attStatus);
            calculateUIIndexes(attStatus);
            calculateUIIndexes(defStatus);



            // max

			turnning = true;
            // xet cac di trang
            // tun thi bo qua moi thu tru hoi mau
            Debug.Log("Player " + attStatus.playerID + " is attacking Player " + defStatus.playerID);


            bool poison = attStatus.op_effects.ContainsKey(NewEffect.Poisoning);
            bool burn = attStatus.op_effects.ContainsKey(NewEffect.Burn);
            bool hypnotic = attStatus.op_effects.ContainsKey(NewEffect.Hypnotic);
            bool stun = attStatus.op_effects.ContainsKey(NewEffect.Stun);
            bool freezing = attStatus.op_effects.ContainsKey(NewEffect.Freezing);
            bool bleed = attStatus.op_effects.ContainsKey(NewEffect.Bleed);
            bool immo = attStatus.op_effects.ContainsKey(NewEffect.Immobilization);
            bool blind = attStatus.op_effects.ContainsKey(NewEffect.Blind);
            bool glamour = attStatus.op_effects.ContainsKey(NewEffect.Glamour);
            bool skill78 = attStatus.op_effects.ContainsKey("Magic Tricks");
            Debug.Log("posin "+poison+"|"+ "burn "+burn +"|"+"hypnotic "+ hypnotic+"|"+"stun "+stun+" "+"|"+"freezing "+freezing+"|"+"bleed "+bleed+"|"+"immo "+immo+"|"+"blind "+blind+"|"+"glamour "+glamour+"|"+"skill78 "+" "+skill78);

            BeginTurnResult result = null;
            bool continued = hypnotic || stun || freezing? false : true;

            // Loai cam hoi mau cua Skill78
            // Lam lai glamour
            // lam lai bleed

            ArrayList states = new ArrayList();

            float Recovery = (float)attStatus.getCurrentIndex(Indexes.hp_recor_na);

            float Health = (float)attStatus.getCurrentIndex(Indexes.hp_na);


            if (monitoring) {
                //Debug.Log("BeginTurn delta Health = " + delta);


                ArrayList HealthBuff_Pluses = caculateDeltaOfIndex_Skill(Indexes.hp_buff_na, true, attStatus, defStatus);
                ArrayList HealthBuff_Subs = caculateDeltaOfIndex_Skill(Indexes.hp_buff_na, false, attStatus, defStatus);

                ArrayList HealthPassive_Pluses = caculateDeltaOfIndex_Skill(Indexes.hp_passive_na, true, attStatus, defStatus);
                ArrayList HealthPassive_Subs = caculateDeltaOfIndex_Skill(Indexes.hp_passive_na, false, attStatus, defStatus);

                if (bleed || skill78)
                {
                    Recovery = 0;
                }

                if (glamour)
                {
                    Recovery = -Recovery;
                }

                if (Recovery != 0) {
                    State state = new State(attStatus.playerID, attStatus.playerID);
                    state.setIdSkill(0);
                    float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + Recovery);
                    state.setHP(attStatus, defStatus);
                    NewEffect effect = new NewEffect(attStatus.playerID, Recovery > 0? "HealthRecovery": "HealthRecovery_Glamour", "", -1, 0);

                    effect.playerID = attStatus.playerID;
                    state.setEffects(new ArrayList() { effect });
                    states.Add(state);
                }

                foreach (ArrayList re in HealthBuff_Pluses) {
                    float r = (float)re[0];
                    string skill = (string)re[1];
                    if (bleed || skill78)
                    {
                        r = 0;
                    }

                    if (glamour)
                    {
                        r = -r;
                    }

                    if (r != 0) {
                        State state = new State(attStatus.playerID, attStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(attStatus.playerID, r > 0 ? "HealthAdd_"+skill : "HealthAdd_Glamour_"+skill, "", -1, 0);

                        effect.playerID = attStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }


                }
                foreach (ArrayList re in HealthPassive_Pluses)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];
                    if (bleed || skill78)
                    {
                        r = 0;
                    }

                    if (glamour)
                    {
                        r = -r;
                    }

                    if (r != 0)
                    {
                        State state = new State(attStatus.playerID, attStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(attStatus.playerID, r > 0 ? "HealthAdd_" + skill : "HealthAdd_Glamour_" + skill, "", -1, 0);

                        effect.playerID = attStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }

                foreach (ArrayList re in HealthBuff_Subs)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];


                    if (r != 0)
                    {
                        State state = new State(attStatus.playerID, attStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(attStatus.playerID, "HealthSub_" + skill, "", -1, 0);

                        effect.playerID = attStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }

                foreach (ArrayList re in HealthPassive_Subs)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];


                    if (r != 0)
                    {
                        State state = new State(attStatus.playerID, attStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(attStatus.playerID, "HealthSub_" + skill, "", -1, 0);

                        effect.playerID = attStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }
                float burnDamage = burn ? (int)attStatus.op_effects[NewEffect.Burn].getIndex("burn_damage", 0) : 0;
                float poisonDamage = poison ? (int)attStatus.op_effects[NewEffect.Poisoning].getIndex("poison_damage", 0) : 0;
                if (burnDamage != 0) {
                    State state = new State(attStatus.playerID, attStatus.playerID);
                    state.setIdSkill(0);
                    float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) - burnDamage);
                    state.setHP(attStatus, defStatus);
                    NewEffect effect = new NewEffect(attStatus.playerID, "Burned", "", -1, 0);

                    effect.playerID = attStatus.playerID;
                    state.setEffects(new ArrayList() { effect });
                    states.Add(state);
                }

                if (poisonDamage != 0)
                {
                    State state = new State(attStatus.playerID, attStatus.playerID);
                    state.setIdSkill(0);
                    float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) - poisonDamage);
                    state.setHP(attStatus, defStatus);
                    NewEffect effect = new NewEffect(attStatus.playerID, "Poisoned", "", -1, 0);

                    effect.playerID = attStatus.playerID;
                    state.setEffects(new ArrayList() { effect });
                    states.Add(state);
                }


            } else {
                float HealthBuff_Plus = (float)caculateDeltaOfIndex(Indexes.hp_buff_na, true, attStatus, defStatus);
                float HealthBuff_Sub = (float)caculateDeltaOfIndex(Indexes.hp_buff_na, false, attStatus, defStatus);

                float HealthPassive_Plus = (float)caculateDeltaOfIndex(Indexes.hp_passive_na, true, attStatus, defStatus);
                float HealthPassive_Sub = (float)caculateDeltaOfIndex(Indexes.hp_passive_na, false, attStatus, defStatus);


                if (bleed || skill78)
                {
                    Recovery = 0;
                    HealthBuff_Plus = 0;
                    HealthPassive_Plus = 0;
                }

                if (glamour)
                {
                    Recovery = -Recovery;
                    HealthBuff_Plus = -HealthBuff_Plus;
                    HealthPassive_Plus = -HealthPassive_Plus;
                }

                float delta = Recovery + HealthBuff_Sub + HealthBuff_Plus + HealthPassive_Sub + HealthPassive_Plus;

                delta -= burn ? (int)attStatus.op_effects[NewEffect.Burn].getIndex("burn_damage", 0) : 0;
                delta -= poison ? (int)attStatus.op_effects[NewEffect.Poisoning].getIndex("poison_damage", 0) : 0;

                if (delta != 0)
                {
                    State state = new State(attStatus.playerID, attStatus.playerID);
                    state.setIdSkill(0);
                    //state.healthAtt = Health + delta;
                    //state.healthDef = (float)defStatus.getCurrentIndex(Indexes.hp_na);
                    float hp_fix = attStatus.setHP(Health + delta);
                    //attStatus.setIndex(attStatus.midIndexes, Indexes.hp_na, hp_fix);
                    //attStatus.setIndex(attStatus.curIndexes, Indexes.hp_na, Health + delta);

                    state.setHP(attStatus, defStatus);

                    NewEffect effect = new NewEffect(attStatus.playerID, "HealthChanges", "", -1, 0);
                    effect.playerID = attStatus.playerID;
                    //ArrayList effects = new ArrayList();
                    //effects.Add(effect);
                    state.setEffects(new ArrayList() { effect });
                    if (hp_fix != Health)
                        states.Add(state);
                }
            }


            result = new BeginTurnResult(attacker.playerID, continued);
			result.states = states;
			return result;
		}


        private MyDictionary<string, string> checkAbnormalStatusHappen(ArrayList saveEffects) {
            MyDictionary<string, string> happenedStatus = new MyDictionary<string, string>();

            float chance = defStatus.calculateIndex(attStatus, Indexes.decrease_all_abnormal_1round__chance_na);
            float p = (float)random.GetDouble();
            bool dec_1_round = p < chance ? true : false;
            foreach (string status in attStatus.character.abDic.Keys)
            {// luu y danh gia
                if (isHappend(status))
                {

                    Debug.Log("Generate abnoraml Status "+ status);
                    NewEffect effect = attStatus.character.abDic[status].affect(attStatus, defStatus);
                    //Debug.Log(status + " effect with def Count" + defStatus.atomics.Keys.Count);
                    effect.duration = (int)calculateIndex(attStatus, defStatus, attStatus.character.abDic[status].getName() + "_duration");
                    switch (effect.name)
                    {


                        case NewEffect.Freezing:
                            effect.duration -= (int)Math.Max(0, calculateIndex(defStatus, attStatus, Indexes.frz_dure_na));
                            break;
                        case NewEffect.Poisoning:
                            //isPoisoning = true;
                            effect.duration -= (int)Math.Max(0, calculateIndex(defStatus, attStatus, Indexes.pOi_dure_na));
                            break;
                        case NewEffect.Shock:

                            effect.duration -= (int)Math.Max(0, calculateIndex(defStatus, attStatus, Indexes.eLe_dure_na));
                            break;
                        case NewEffect.Burn:
                            //isBurn = true;
                            effect.duration -= (int)Math.Max(0, calculateIndex(defStatus, attStatus, Indexes.bur_dure_na));
                            break;
                    }
                    if (dec_1_round) {
                        effect.duration -= 2;
                    }

                    if (effect.duration > 0) {
                        saveEffects.Add(effect);
                        happenedStatus.Add(effect.name, effect.name);
                    } else {
                        // loai bo ngay do duration = 0;
                        if (effect.playerID == attStatus.playerID) {
                            attStatus.removeEffect(effect.name, effect.originID);

                        } else if (effect.playerID == defStatus.playerID) {
                            defStatus.removeEffect(effect.name, effect.originID);
                        } else if (effect.playerID == 3) {
                            attStatus.removeEffect(effect.name, effect.originID);
                            defStatus.removeEffect(effect.name, effect.originID);
                        }

                    }


                }
            }

            // tinh lai cac chi so va cong thuc
            if (happenedStatus.Keys.Count > 0) {
                Debug.Log("Recompute All Indexes Due to New Abnormal Status Happening");
                attStatus.recomputeIndexesBeforeActive(defStatus);
                defStatus.recomputeIndexesBeforeActive(attStatus);
            }

            return happenedStatus;
        }

        private void oneAttack(ArrayList savedEffects, int idSkill, ArrayList states) {
            Debug.Log("oneAttack "+idSkill);
            State state = null;


            MyDictionary<string, string> occuredAbs = checkAbnormalStatusHappen(savedEffects);
            // stacks
            float stacks = (float)attStatus.getMidIndex(Indexes.stacks_na);
            attStatus.setIndex(attStatus.midIndexes, Indexes.stacks_na, stacks + 1.0f);
            float HealthActive_Plus = 0;
            float HealthActive_Sub = 0;

            attStatus.preProcessBeforeDamaging(defStatus);
            defStatus.preProcessBeforeDamaging(attStatus);

            bool isBurn = occuredAbs.ContainsKey(NewEffect.Burn);
            bool isPoisoning = occuredAbs.ContainsKey(NewEffect.Poisoning);// xuat hien trang thai burn va poison trong luot danh nay can tinh damage burn va poison
            bool bleed = attStatus.op_effects.ContainsKey(NewEffect.Bleed);
            bool glamour = attStatus.op_effects.ContainsKey(NewEffect.Glamour);
            bool op_sleep = defStatus.op_effects.ContainsKey(NewEffect.Sleep);
            bool skill78 = attStatus.op_effects.ContainsKey("Magic Tricks");
            bool op_bleed = defStatus.op_effects.ContainsKey(NewEffect.Bleed);
            bool op_glamour = defStatus.op_effects.ContainsKey(NewEffect.Glamour);
            bool op_skill78 = defStatus.op_effects.ContainsKey("Magic Tricks");

            bool isDodge = random.GetDouble() < Math.Max((float)defStatus.calculateIndex(attStatus, Indexes.dOdGe_cha_na) - (float)attStatus.calculateIndex(defStatus, Indexes.accuracy_na), 0)
                && attStatus.character.characteristic.Damage == Characteristic.DamageType.Magic;
            bool isBlock = (random.GetDouble() < Math.Max((float)defStatus.calculateIndex(attStatus, Indexes.block_cha_na) - (float)attStatus.calculateIndex(defStatus, Indexes.brshie_na), 0))
                && attStatus.character.characteristic.Damage != Characteristic.DamageType.Magic;
            isDodge = op_sleep ? false : isDodge;
            isBlock = op_sleep ? false : isBlock;

            //isBlock = turns == LIMIT_TURNS ? true : false;
            if (isBlock || isDodge)
            {
                // loai bo het cac effect dinh trong luot danh nay truoc do
                foreach (NewEffect effect in savedEffects)
                {
                    if (effect.playerID == 3)
                    {
                        attStatus.removeEffect(effect.name, effect.originID);
                        defStatus.removeEffect(effect.name, effect.originID);
                    }
                    else if (effect.playerID == attStatus.playerID) {
                        attStatus.removeEffect(effect.name, effect.originID);
                        
                    } else if (effect.playerID == defStatus.playerID) {
                        defStatus.removeEffect(effect.name, effect.originID);
                    } 
                
                }
                savedEffects.Clear();



                if (isBlock)
                {
                    NewEffect effect = new NewEffect(defStatus.playerID, "Blocking", "", -1, 0);
                    effect.playerID = defStatus.playerID;
                    savedEffects.Add(effect);

                    defStatus.enableCondition(ConditionManager.Shield_block_1_round, 2);
                }


                if (isDodge)
                {
                    NewEffect effect = new NewEffect(defStatus.playerID, "Dodge", "", -1, 0);
                    effect.playerID = defStatus.playerID;
                    savedEffects.Add(effect);
                }
                // tao 
                if (monitoring) {
                    // danh hut
                    state = new State(attStatus.playerID, defStatus.playerID);
                    state.setIdSkill(idSkill);
                    state.setEffects(savedEffects);
                    state.setHP(attStatus, defStatus);

                    states.Add(state);
                    // skill active anh huong toi defStatus
                    ArrayList HealthActive_Pluses_Opp = caculateDeltaOfIndex_Skill(Indexes.hp_active_na, true, defStatus, attStatus);
                    ArrayList HealthActive_Subs_Opp = caculateDeltaOfIndex_Skill(Indexes.hp_active_na, false, defStatus, attStatus);

                    foreach (ArrayList re in HealthActive_Subs_Opp)
                    {
                        float r = (float)re[0];
                        string skill = (string)re[1];
                        if (r != 0)
                        {
                            state = new State(defStatus.playerID, defStatus.playerID);
                            state.setIdSkill(0);
                            float hp_fix = defStatus.setHP((float)defStatus.getCurrentIndex(Indexes.hp_na) + r);
                            state.setHP(attStatus, defStatus);
                            NewEffect effect = new NewEffect(defStatus.playerID, "HealthSub_" + skill, "", -1, 0);

                            effect.playerID = defStatus.playerID;
                            state.setEffects(new ArrayList() { effect });
                            states.Add(state);
                        }
                    }

                    foreach (ArrayList re in HealthActive_Pluses_Opp)
                    {
                        float r = (float)re[0];
                        string skill = (string)re[1];

                        if (op_bleed || op_skill78)
                        {
                            r = 0;

                        }

                        if (op_glamour)
                        {
                            r = -r;
                        }

                        if (r != 0)
                        {
                            state = new State(defStatus.playerID, defStatus.playerID);
                            state.setIdSkill(0);
                            float hp_fix = defStatus.setHP((float)defStatus.getCurrentIndex(Indexes.hp_na) + r);
                            state.setHP(attStatus, defStatus);
                            NewEffect effect = new NewEffect(defStatus.playerID, r > 0 ? "HealthAdd_" + skill : "HealthAdd_Glamour_" + skill, "", -1, 0);

                            effect.playerID = defStatus.playerID;
                            state.setEffects(new ArrayList() { effect });
                            states.Add(state);
                        }
                    }

                    defStatus.decreaseIndex(Indexes.hp_active_na);
                    defStatus.rebuildFomulaOfIndex(Indexes.hp_active_na);
                } else {
                    HealthActive_Plus = (float)caculateDeltaOfIndex(Indexes.hp_active_na, true, defStatus, attStatus);
                    HealthActive_Sub = (float)caculateDeltaOfIndex(Indexes.hp_active_na, false, defStatus, attStatus);

                    HealthActive_Plus = op_bleed || op_skill78 ? 0 : HealthActive_Plus;
                    HealthActive_Plus = op_glamour ? -HealthActive_Plus : HealthActive_Plus;


                    defStatus.decreaseIndex(Indexes.hp_active_na); // khong van de gi boi khi do def chua su dung cac skill active
                    defStatus.rebuildFomulaOfIndex(Indexes.hp_active_na);
                    defStatus.setHP((float)defStatus.getCurrentIndex(Indexes.hp_na) + HealthActive_Plus + HealthActive_Sub);

                    state = new State(attStatus.playerID, defStatus.playerID);
                    state.setIdSkill(idSkill);
                    state.setEffects(savedEffects);
                    state.setHP(attStatus, defStatus);

                    states.Add(state);
                }





                defStatus.disableCondition(ConditionManager.Be_Hit);
                isBurn = false;
                isPoisoning = false;
                Debug.Log("Be Dodge or Block+++++++++++++++++++++++++++++++++");
                return;
            } else {
                defStatus.enableCondition(ConditionManager.Be_Hit);
                float hits = (float)attStatus.getMidIndex(Indexes.successful_hits_na);

                attStatus.setIndex(attStatus.midIndexes, Indexes.successful_hits_na, hits + 1.0f);
            }




            ArrayList damages = attStatus.damageWith(defStatus);
            bool isCritical = (bool)damages[3];
            bool isDoubleMagic = (bool)damages[4];


            float d1 = isBlock ? 0 : (float)damages[0];
            float d2 = isDodge ? 0 : (float)damages[1];
            float d3 = (float)damages[2];

            //Debug.Log("ToTal Damage = " + attStatus.getCurrentIndex("total_damage"));



            float total_damage = calculateIndex(attStatus, defStatus, "total_damage");
            if (idSkill == 60 || idSkill == 80) {
                total_damage -= d1 + d2;
                d1 = 0;
                d2 = 0;
            }



            //Debug.Log("ToTal Damage = "+total_damage);

            //Debug.Log("burn =  "+isBurn+ " poison = "+isPoisoning);

            if (isBurn)
            {
                NewEffect effect = defStatus.op_effects[NewEffect.Burn];
                int damage = (int)Math.Round(calculateIndex(attStatus, defStatus, "burn_damage"));
                effect.setIndex("burn_damage", damage);
            }

            if (isPoisoning)
            {
                NewEffect effect = defStatus.op_effects[NewEffect.Poisoning];
                int damage = (int)Math.Round(calculateIndex(attStatus, defStatus, "poison_damage"));
                effect.setIndex("poison_damage", damage);
            }

            if (isCritical)
            {
                NewEffect effect = new NewEffect(attStatus.playerID, "Critical", "", -1, 0);
                effect.playerID = defStatus.playerID;
                savedEffects.Add(effect);
                attStatus.disableCondition(ConditionManager.Critical);

                float critical_hits = (float)attStatus.getMidIndex(Indexes.critical_hits_na);
                attStatus.setIndex(attStatus.midIndexes, Indexes.critical_hits_na, critical_hits + 1.0f);
            }

            if (isDoubleMagic) {
                NewEffect effect = new NewEffect(attStatus.playerID, "DoubleMagic", "", -1, 0);
                effect.playerID = defStatus.playerID;
                savedEffects.Add(effect);
            }



            int d = Math.Max((int)Math.Round(total_damage), 1);
            if (monitoring) {
                Debug.Log("attack monitoring+++++++++++++++++++++++++++++++++");

                state = new State(attStatus.playerID, defStatus.playerID);
                state.setIdSkill(idSkill);

                ArrayList HealthActive_Pluses = caculateDeltaOfIndex_Skill(Indexes.hp_active_na, true, attStatus, defStatus);
                ArrayList HealthActive_Subs = caculateDeltaOfIndex_Skill(Indexes.hp_active_na, false, attStatus, defStatus);

                foreach (ArrayList re in HealthActive_Subs)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];
                    if (r != 0)
                    {
                        state = new State(attStatus.playerID, attStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(attStatus.playerID, "HealthSub_" + skill, "", -1, 0);

                        effect.playerID = attStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }

                foreach (ArrayList re in HealthActive_Pluses)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];
                    if (bleed || skill78)
                    {
                        r = 0;

                    }

                    if (glamour)
                    {
                        r = -r;
                    }

                    if (r != 0)
                    {
                        state = new State(attStatus.playerID, attStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = attStatus.setHP((float)attStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(attStatus.playerID, r > 0? "HealthAdd_" + skill : "HealthAdd_Glamour_" + skill, "", -1, 0);

                        effect.playerID = attStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }
                attStatus.decreaseIndex(Indexes.hp_active_na);
                attStatus.rebuildFomulaOfIndex(Indexes.hp_active_na);

                // attatus subs->attack->Pluses

                float healthDef = defStatus.setHP((float)defStatus.getCurrentIndex(Indexes.hp_na) - d);
                state = new State(attStatus.playerID, defStatus.playerID);
                state.setIdSkill(idSkill);
                state.setHP(attStatus, defStatus);
                state.damage = d;
                state.setEffects(savedEffects);
                states.Add(state);
                // defstatus attack->subs->pluses

                ArrayList HealthActive_Pluses_Opp = caculateDeltaOfIndex_Skill(Indexes.hp_active_na, true, defStatus, attStatus);
                ArrayList HealthActive_Subs_Opp = caculateDeltaOfIndex_Skill(Indexes.hp_active_na, false, defStatus, attStatus);

                foreach (ArrayList re in HealthActive_Subs_Opp)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];
                    if (r != 0)
                    {
                        state = new State(defStatus.playerID, defStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = defStatus.setHP((float)defStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(defStatus.playerID, "HealthSub_" + skill, "", -1, 0);

                        effect.playerID = defStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }

                foreach (ArrayList re in HealthActive_Pluses_Opp)
                {
                    float r = (float)re[0];
                    string skill = (string)re[1];

                    if (op_bleed || op_skill78)
                    {
                        r = 0;

                    }

                    if (op_glamour)
                    {
                        r = -r;
                    }

                    if (r != 0)
                    {
                        state = new State(defStatus.playerID, defStatus.playerID);
                        state.setIdSkill(0);
                        float hp_fix = defStatus.setHP((float)defStatus.getCurrentIndex(Indexes.hp_na) + r);
                        state.setHP(attStatus, defStatus);
                        NewEffect effect = new NewEffect(defStatus.playerID, r > 0 ?"HealthAdd_" + skill : "HealthAdd_Glamour_" + skill, "", -1, 0);

                        effect.playerID = defStatus.playerID;
                        state.setEffects(new ArrayList() { effect });
                        states.Add(state);
                    }
                }

                defStatus.decreaseIndex(Indexes.hp_active_na);
                defStatus.rebuildFomulaOfIndex(Indexes.hp_active_na);

            } else {
                state = new State(attStatus.playerID, defStatus.playerID);
                state.setIdSkill(idSkill);
                //Debug.Log("Finish Check HP " + (float)attStatus.getCurrentIndex(Indexes.hp_na) + " " + ((float)defStatus.getCurrentIndex(Indexes.hp_na)) + " " + attStatus.playerID + " " + defStatus.playerID);
                // hp active for att
                HealthActive_Plus = (float)caculateDeltaOfIndex(Indexes.hp_active_na, true, attStatus, defStatus);
                HealthActive_Sub = (float)caculateDeltaOfIndex(Indexes.hp_active_na, false, attStatus, defStatus);

                HealthActive_Plus = bleed || skill78 ? 0 : HealthActive_Plus;
                HealthActive_Plus = glamour ? -HealthActive_Plus : HealthActive_Plus;

                float healthAtt = (float)attStatus.getCurrentIndex(Indexes.hp_na) + HealthActive_Plus + HealthActive_Sub;
                // trong skill active co chua hoi mau
                attStatus.decreaseIndex(Indexes.hp_active_na);
                attStatus.rebuildFomulaOfIndex(Indexes.hp_active_na);
                // chi dung 1 lan nen loai bo va tinh toan ngay

                // hp_active for def
                HealthActive_Plus = (float)caculateDeltaOfIndex(Indexes.hp_active_na, true, defStatus, attStatus);
                HealthActive_Sub = (float)caculateDeltaOfIndex(Indexes.hp_active_na, false, defStatus, attStatus);

                HealthActive_Plus = op_bleed || op_skill78 ? 0 : HealthActive_Plus;
                HealthActive_Plus = op_glamour ? -HealthActive_Plus : HealthActive_Plus;

                defStatus.decreaseIndex(Indexes.hp_active_na);
                defStatus.rebuildFomulaOfIndex(Indexes.hp_active_na);

                float healthDef = (float)defStatus.getCurrentIndex(Indexes.hp_na) - d + HealthActive_Plus + HealthActive_Sub;


                healthAtt = attStatus.setHP(healthAtt);
                healthDef = defStatus.setHP(healthDef);
                state.setHP(attStatus, defStatus);
                //Debug.Log("Finish Damage " + healthAtt + " " + healthDef + " " + attStatus.playerID + " " + defStatus.playerID);
                // tinh lai cac chi so Abrnomalstatus
                state.damage = d;
                state.setEffects(savedEffects);
                states.Add(state);
            }

            // 1 lan danh thuong
            if (isEndOfCombat()) return;

            bool isReturn = random.GetDouble() < (float)defStatus.calculateIndexAndSave(attStatus, Indexes.return_chance);// calculate Index all return damage index and save
            bool isReturnPhysical = random.GetDouble() < (float)defStatus.calculateIndexAndSave(attStatus,Indexes.physical_return_chance);
            bool isReturnMagical = random.GetDouble() < (float)defStatus.calculateIndexAndSave(attStatus,Indexes.magical_return_chance);
            float returnDamage = 0;
            if (isReturn)
            {
                returnDamage += d1 * ((float)defStatus.calculateIndexAndSave(attStatus,Indexes.physical_return_ratio) + (float)defStatus.calculateIndexAndSave(attStatus,Indexes.return_ratio));
                returnDamage += d2 * ((float)defStatus.calculateIndexAndSave(attStatus,Indexes.magical_return_ratio) + (float)defStatus.calculateIndexAndSave(attStatus,Indexes.return_ratio));
                returnDamage += d3 * (float)defStatus.calculateIndexAndSave(attStatus,Indexes.return_ratio);
            }
            else
            {
                if (isReturnMagical)
                {
                    returnDamage += d2 * (float)defStatus.calculateIndexAndSave(attStatus,Indexes.magical_return_ratio);
                }

                if (isReturnPhysical)
                {
                    returnDamage += d1 * (float)defStatus.calculateIndexAndSave(attStatus,Indexes.physical_return_ratio);
                }
            }

            returnDamage = (int)Math.Round(returnDamage);

            if (returnDamage > 0)
            {
                state = new State(defStatus.playerID, attStatus.playerID);
                state.setIdSkill(0);
                //state.damage = d;
                float healthAtt = (float)defStatus.getCurrentIndex(Indexes.hp_na);
                Debug.Log("mau att = "+attStatus.getCurrentIndex(Indexes.hp_na)+" "+" is returned "+returnDamage);
                float healthDef = Math.Max(0, (float)attStatus.getCurrentIndex(Indexes.hp_na) - returnDamage);
                attStatus.setHP(healthDef);
                state.setHP(attStatus, defStatus);

                ArrayList effs = new ArrayList();
                NewEffect effect = new NewEffect(defStatus.playerID, "ReturnDamage", "", 0, 0);
                effect.playerID = attStatus.playerID;
                effs.Add(effect);
                state.setEffects(effs);
                states.Add(state);
                if (isEndOfCombat()) return;
            }
        }

        public ArrayList activeBranch(ArrayList effects, int idSkill)
        {
            Debug.Log("++++++++++++++++++++activeBranch{0,3}+++++++++++++++++++"+idSkill);

            attStatus.setIndex(attStatus.midIndexes, Indexes.hp_active_na, 0f);
            attStatus.setIndex(attStatus.midIndexes, Indexes.total_damage_na, 0f);
            attStatus.setIndex(attStatus.midIndexes, Indexes.pure_da_na, 0f);
            // total_damage reset

            ArrayList states = new ArrayList();
            int successive_attacks = (int)calculateIndex(attStatus,defStatus, Indexes.successive_attacks);
            // kiem tra xem co effects lien quan den trong skill nay khong passive + buff
            successive_attacks = Math.Max(1, successive_attacks);
            attStatus.enableCondition(ConditionManager.Hit);
            // tong hop cong thuc
            // them vao cac bien so khong co trong cac cong thuc


            for (int i = 0; i < successive_attacks; i++) {
                
                // tinh den su xuat hien cua cac abnomarl
                // tinh lai cac chi so
                // tinh damage
                // xet dodege
                // xet block
                //sinh state // neu dau tien thi kem cac effect va cac abrnomaml
                // xet co bi phan damage khong sinh phan damage
                // need Code hits
                // enable shield block in 1 round
                ArrayList effs = new ArrayList();

                float hits  = (float)attStatus.getMidIndex(Indexes.hits_na);
                attStatus.setIndex(attStatus.midIndexes, Indexes.hits_na, hits + 1);

                if (i == 0) {
                    effs.AddRange(effects);
                }
                oneAttack(effs, idSkill, states);
                if (isEndOfCombat()) break;


                bool isMulticast = random.GetDouble() < (float)attStatus.calculateIndex(defStatus, Indexes.mul_ca_cha_na);
                if (isMulticast && i == successive_attacks - 1)
                {// them 1 lan danh nua nhung chi co hieu ung Multicast
                    Debug.Log("Multicast");


                    NewEffect effect = new NewEffect(attStatus.playerID, "Multicast", "", 0, 0);
                    effect.playerID = attStatus.playerID;
                    effs = new ArrayList();
                    effs.Add(effect);
                    oneAttack(effs, idSkill, states);

                    if (isEndOfCombat()) break;
                }


                // tinh dmage


            }

            attStatus.conManager.disableCondition(ConditionManager.Hit);
            defStatus.disableCondition(ConditionManager.Be_Hit);
            attStatus.setIndex(attStatus.midIndexes, Indexes.hits_na, 0f);
            attStatus.setIndex(attStatus.midIndexes, Indexes.successful_hits_na, 0f);

            return states;
        }



        public bool isHappend(string status) {
            float chance = 0;
            NewEffect effect = null;
            switch (attStatus.character.abDic[status].getName()) {

				case NewEffect.Impotent:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.imp_cha_na) 
                        - calculateIndex(defStatus,attStatus, Indexes.imp_res_na));
                    break;
        		case NewEffect.Hypnotic:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.hyp_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.hYp_res_na));
                    break;
        		case NewEffect.Rot:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.rot_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.rOt_res_na));
                    break;
        		case NewEffect.Pain:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.pain_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.pain_res_na));
                    break;
        		case NewEffect.Bleed:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.bleed_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.bLeed_res_na));
                    break;
        		case NewEffect.Crazy:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.crazy_cha_na) 
                        - calculateIndex(defStatus,attStatus, Indexes.craZY_res_na));
                    break;
        		case NewEffect.Dull:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.dull_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.dull_res_na));
                    break;
        		case NewEffect.Stun:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.stun_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.stun_res_na));
                    break;
        		case NewEffect.Frostbite:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.fro_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.fro_res_na));
                    break;
        		case NewEffect.Burn:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.bur_cha_na));
                    break;
        		case NewEffect.Shock:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.eLe_cha_na));
                    break;
        		case NewEffect.Poisoning:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.poi_cha_na));
                    break;
        		case NewEffect.Knockback:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.kno_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.kno_res_na));
                    break;
        		case NewEffect.Immobilization:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.imm_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.imm_res_na));
                    break;
        		case NewEffect.Blind:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.blind_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.blind_res_na));
                    break;
        		case NewEffect.Freezing:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.frz_cha_na));
                    break;
        		case NewEffect.Glamour:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.gla_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.gla_res_na));
                    break;
        		case NewEffect.Fear:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.fear_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.fear_res_na));
                    break;
        		case NewEffect.Disease:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.dis_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.dis_res_na));
                    break;
        		case NewEffect.Sleep:
                    chance = Math.Max(0, calculateIndex(attStatus, defStatus, Indexes.sleep_cha_na) 
                        - calculateIndex(defStatus, attStatus, Indexes.sleep_res_na));
                    break;
                              
            }

            float p = (float)random.GetDouble();

            //Debug.Log("chance "+ attStatus.character.abDic[status].getName() + " "+chance+" p = "+p);

            return p < chance;
        }


        public void addPassiveSkills(ArrayList skills, NewCharacterStatus target)
        {
            
            NewCharacterStatus vs = getStatusByPlayerID(3 - target.playerID);
            foreach (int idSkill in skills)
            {
                Debug.Log("adddPassive skill "+idSkill+" vao targetID " +" "+target.playerID);

                NewSkill skill = target.character.newSkillDic.ContainsKey("Skill" + idSkill) ? target.character.newSkillDic["Skill" + idSkill] : null;
                ArrayList effects = skill.affect(target, vs);

                foreach (NewEffect effect in effects)
                {
                    foreach (AtomicEffect atomic in effect.atomicEffects)
                    {
                        if (atomic.enemy)
                        {// tac dung vao def
                            vs.rebuildFomulaOfIndex(atomic.index);
                        }
                        else
                        {// tac dung vao att
                            target.rebuildFomulaOfIndex(atomic.index);
                        }
                    }
                }

            }

            string str = "";

            foreach (string key in attStatus.effects.Keys)
            {
                str += "|" + key;
            }

            Debug.Log(str);

            str = "";

            foreach (string key in attStatus.op_effects.Keys)
            {
                str += "|" + key;
            }
            Debug.Log(str);

            str = "";

            foreach (string key in defStatus.effects.Keys)
            {
                str += "|" + key;
            }
            Debug.Log(str);

            str = "";

            foreach (string key in defStatus.op_effects.Keys) {
                str += "|" + key;
            }
            Debug.Log(str);
        }


        public ArrayList buffBranch(ArrayList effects, int idSkill) // almost done
        {

            Console.WriteLine("++++++++++++++++++++buffBranch{0,3}+++++++++++++++++++", idSkill);


            ArrayList states = new ArrayList();
            // neu trong so nhung effect cua skill co HP hoac lech thi dung ngay
            State state = null;
            state = new State(attStatus.playerID, defStatus.playerID);
            state.setIdSkill(idSkill);

            float delta_Att = 0;
            float delta_Def = 0;

            attStatus.recomputeIndexesBeforeActive(defStatus);// khong dung

            defStatus.recomputeIndexesBeforeActive(attStatus);// khong dung

            bool attHpBuff = false;
            bool defHpBuff = false;

            foreach (NewEffect effect in effects)
            {
                ArrayList atomics = effect.atomicEffects;

                foreach (AtomicEffect atomic in atomics)
                {
                    if (atomic.index == Indexes.hp_buff_na)
                    {// skill tac dung den health
                        // cong ngay cho me lan nay
                        if (atomic.enemy) defHpBuff = true;
                        else attHpBuff = true;
                        //atomic.decDuration();
                    }

                    //if (atomic.enemy) defStatus.rebuildFomulaOfIndex(atomic.index);
                    //else attStatus.rebuildFomulaOfIndex(atomic.index);
                }
            }
            if (attHpBuff) {
                delta_Att = calculateIndex(attStatus, defStatus, Indexes.hp_buff_na);
                attStatus.decreaseIndex(Indexes.hp_buff_na);
            }
            if (defHpBuff) {
                delta_Def = calculateIndex(defStatus, attStatus, Indexes.hp_buff_na);
                defStatus.decreaseIndex(Indexes.hp_buff_na);
            }

            // xet dong remove_all_abnormal_chance

            bool bleed = attStatus.op_effects.ContainsKey(NewEffect.Bleed);
            bool glamour = attStatus.op_effects.ContainsKey(NewEffect.Glamour);
            bool skill78 = attStatus.op_effects.ContainsKey("Magic Tricks");
            //float result = 0;

            float hp_Att = (float)attStatus.getCurrentIndex(Indexes.hp_na);
            float hp_Def = (float)defStatus.getCurrentIndex(Indexes.hp_na);


            delta_Att = bleed || skill78 ? delta_Att > 0 ? 0 : delta_Att : delta_Att;
            delta_Att = glamour ? delta_Att > 0 ? -delta_Att : delta_Att : delta_Att;

            hp_Att += delta_Att;
            hp_Def += delta_Def;
            // Update HP 2 ben o day
            //Debug.Log("buff " + hp_Att + " " + hp_Def+" "+delta_Att+" "+delta_Def);
            attStatus.setHP(hp_Att);
            defStatus.setHP(hp_Def);
            state.setHP(attStatus, defStatus);

            bool remove_All_Abs = random.GetDouble() < attStatus.calculateIndex(defStatus, Indexes.remove_all_abnormal_chance_na);
            if (remove_All_Abs) {
                NewEffect effect = new NewEffect(attStatus.playerID, "RemoveAllAbnormal","", -1, 0);
                effects.Add(effect);
                effect.playerID = attStatus.playerID;

                // remove all abnormal op cast to me
                foreach (string status in attStatus.character.abDic.Keys)
                {// luu y danh gia
                    AbnormalStatus ab = attStatus.character.abDic[status];
                    if (attStatus.op_effects.ContainsKey(ab.getName())) {
                        attStatus.removeEffect(ab.getName(), 3 - attStatus.playerID);
                    }
                }
            }


			if (state != null)
			{
			    state.setEffects(effects);
				states.Add(state);
			}
            return states;
        }

		public ArrayList inTurn(ArrayList actionHandles)
		{
            //Debug.Log("1");
            attStatus.loadDynamicIndexToMid(defStatus);
           // Debug.Log("2");
            defStatus.loadDynamicIndexToMid(attStatus);
           // Debug.Log("3");
            //foreach (string index in attStatus.midIndexes.Keys)
            //{
            //    Debug.Log(index + " = " + attStatus.midIndexes[index]);
            //}
            //Debug.Log("4");


            states.Clear();
           // Debug.Log("5");

            //foreach (String index in attStatus.deltaFormulas.Keys)
            //{
            //    Debug.Log("|" + index + "|" + attStatus.deltaFormulas[index]);
            //}

            bool isActive = false;
            foreach (ActionHandle action in actionHandles)
			{
				//action.idSkill = 14;

                Debug.Log("++++++++++++++++++++++++++NEWSKILL+++++++++++++++++");

                NewSkill skill = attStatus.character.newSkillDic.ContainsKey("Skill" + action.idSkill) ? attStatus.character.newSkillDic["Skill" + action.idSkill] : null;
				ArrayList effects = new ArrayList();
                // null logic
                //Debug.Log("use skill "+ skill.data.ToString());
                
                effects = skill.affect(attStatus, defStatus);
                foreach (NewEffect effect in effects) {
                    foreach (AtomicEffect atomic in effect.atomicEffects) {
                        if (atomic.enemy) {// tac dung vao def
                            defStatus.rebuildFomulaOfIndex(atomic.index);
                        } else {// tac dung vao att
                            attStatus.rebuildFomulaOfIndex(atomic.index);
                        }
                    }
                }

                //Debug.Log("effect skill " + effects.Count);

                ArrayList subStates = null;

                if (skill.getType() == SkillType.Active) {
                    subStates = activeBranch(effects, action.idSkill);
                    isActive = true;
                    foreach (NewEffect effect in effects)
                    {
                        effect.decDurationOfAtomics();
                        foreach (AtomicEffect atomic in effect.atomicEffects)
                        {
                            if (atomic.enemy)
                            {// tac dung vao def
                                defStatus.rebuildFomulaOfIndex(atomic.index);
                            }
                            else
                            {// tac dung vao att
                                attStatus.rebuildFomulaOfIndex(atomic.index);
                            }
                        }
                    }


					
                } else {
                    subStates = buffBranch(effects, action.idSkill);
                }
                //Debug.Log(subStates + " = " + subStates.Count);
                if (subStates != null)
                states.AddRange(subStates);
                //foreach (State state in subStates)
                //{
                //    states.Add(state);
                //}

                if (isEndOfCombat()) break;
			}



            if (!isActive && attStatus.canAttack())
            {
                if (isEndOfCombat()) return states;
                Debug.Log("++++++++++++++++++++++++++NormalAttack+++++++++++++++++");
                ArrayList subStates = activeBranch(new ArrayList(), 0);
                foreach (State state in subStates)
                {
                    states.Add(state);
                }
            }
            // reset stacks
            attStatus.setIndex(attStatus.midIndexes, Indexes.stacks_na, 0f);
            attStatus.setIndex(attStatus.midIndexes, Indexes.critical_hits_na, 0f);
            return states;
		}

		public EndTurnResult endTurn(NewCharacterStatus attacker, NewCharacterStatus defender) //-1 0 1 2
		{
			if (!turnning) throw new Exception("beginTurn must be called before endTurn.");
			if (attStatus != attacker && defStatus != defender) throw new Exception("Targets of endTurn must be same To beginTurn.");

			turnning = false;
			// giam coolDown cacSkill


			//Dictionary<string, ArrayList> effects = attacker.effects; // tai sao trong effects tac dong vao nhan vat lai co effect tac dong vao doi phuong
			ArrayList releasedState = new ArrayList();

            // check effects cast from attacker to attacker 

            attStatus.conManager.decreaseDuration();
            defStatus.conManager.decreaseDuration();
            
            foreach (String name in attStatus.effects.Keys)
            {
                NewEffect effect = attStatus.effects[name];
                //effect.duration = effect.duration > 0 ? effect.duration - 1 : effect.duration;
                effect.decDuration();
                if (effect.duration == 0) {
                    releasedState.Add(effect);
                }
            }

            // check effects cast from defender to attacker 
            foreach (String name in attStatus.op_effects.Keys) {
                NewEffect effect = attStatus.op_effects[name];
                effect.decDuration();
                if (effect.duration == 0)
                {
                    releasedState.Add(effect);
                }
            }

            //foreach (NewEffect effect in releasedState)
            //{
            //    attStatus.effects.Remove(effect.name);
            //}

            // check effects cast from defender to defender 
            foreach (String name in defStatus.effects.Keys)
            {
                NewEffect effect = defStatus.effects[name];
                //if (effect.playerID < 3)
                //effect.duration = effect.duration > 0 ? effect.duration - 1 : effect.duration;
                if (effect.playerID < 3)
                effect.decDuration();
                if (effect.duration == 0)
                {
                    
                    if (effect.playerID < 3)
                    releasedState.Add(effect);
                }

            }


            // check effects cast from attacker to defender 
            foreach (String name in defStatus.op_effects.Keys) {
                NewEffect effect = defStatus.op_effects[name];
                if (effect.playerID < 3)
                    effect.decDuration();
                if (effect.duration == 0)
                {
                    // loai bo chinh effect
                    if (effect.playerID < 3)
                        releasedState.Add(effect);
                }
            }



            foreach (NewEffect effect in releasedState)
            {
                NewCharacterStatus from = getStatusByPlayerID(effect.originID);
                NewCharacterStatus op_from = getStatusByPlayerID(3 - effect.originID);

                if (effect.playerID == 3 ) {
                    attStatus.removeEffect(effect.name, effect.originID);
                    defStatus.removeEffect(effect.name, effect.originID);
                } else {
                    // cast to one target
                    NewCharacterStatus target = getStatusByPlayerID(effect.playerID);
                    target.removeEffect(effect.name, effect.originID);
                }

                //if (effect.playerID == defStatus.playerID || effect.playerID == 3)
                    //defStatus.effects.Remove(effect.name);
            }



            this.turns--;
            float Health_1 = (float)attStatus.getCurrentIndex(Indexes.hp_na);
            float Health_2 = (float)defStatus.getCurrentIndex(Indexes.hp_na);
            //Debug.Log("EndTurn " + Health_1 + " " + Health_2);
            return new EndTurnResult((Health_1 <= 0 ? defStatus.playerID : (Health_2 <= 0 ? attStatus.playerID : (turns == 0 ? secondAttack: -1))), releasedState);

		}

        public bool isEndOfCombat() {
            float Health_1 = (float)attStatus.getCurrentIndex(Indexes.hp_na);
            float Health_2 = (float)defStatus.getCurrentIndex(Indexes.hp_na);
            return Health_1 <= 0 || Health_2 <= 0;
        }

		public ArrayList getCurrentStates()
		{
			return states;
		}

        public void fixTurn(int turn)
        {
            if (attStatus.playerID != turn)
            {
                NewCharacterStatus temp = attStatus;
                attStatus = defStatus;
                defStatus = temp;
            }
        }

		public NewCharacterStatus getStatusByPlayerID(int id)
		{
			//Debug.Log(attStatus.playerID + " " + defStatus.playerID);
			return (attStatus != null && attStatus.playerID == id) ? attStatus : (defStatus != null && defStatus.playerID == id ? defStatus : null);
		}

        public NewCharacterStatus getStatusByTurn()
        {
            //Debug.Log(attStatus.playerID + " " + defStatus.playerID);
            return attStatus;
        }

        public int getRemainingTurns() {
            return turns;
        }

        private float calculateIndex(NewCharacterStatus att, NewCharacterStatus def, string index, MyDictionary <string, string> replaceDice = null) {
            string fomulas = index + (att.deltaFormulas.ContainsKey(index) && att.deltaFormulas[index] != "" ? " + "+ att.deltaFormulas[index] : "");
            //Debug.Log("calculateIndex " + index + " formulas = " + fomulas); // de log Ishappen Stun
            try {
                float digit = Convert.ToSingle(fomulas);
                Debug.Log("calculateIndex index " + index +" fomulas = "+fomulas  + " value = " + digit);
                return digit;
            } catch {
                // khong phai la digit
                return calculateExpression(att, def, fomulas, replaceDice);
            }

            return 0;
        }

        private float calculateExpression(NewCharacterStatus att, NewCharacterStatus def, string fomulas, MyDictionary<string, string> replaceDice = null)
		{
			try
			{
				float digit = Convert.ToSingle(fomulas);
                Debug.Log("calculateExpression " + " formulas = " + fomulas + " value = " + digit);
				return digit;
			}
			catch
			{
                // khong phai la digit
                float digit = att.calcuateExpression(fomulas, def, replaceDice);
                Debug.Log("calculateExpression " +" formulas = " + fomulas+" value = "+digit);
                return digit;
			}

			return 0;
		}

        private float caculateDeltaOfIndex(string index, bool positive, NewCharacterStatus att, NewCharacterStatus def) {
            float result = 0;
            ArrayList atomics = attStatus.atomics.ContainsKey(index) ? attStatus.atomics[index] : null;
            if (atomics == null) return 0;
            foreach (AtomicEffect atomic in atomics) {
                if (!atomic.isActive()) continue;
                float r = calculateExpression(att, def, atomic.delta);
                bool similar = (r >= 0) == positive;
                if (similar) result += r;
            }
            return result;
        }

        private ArrayList caculateDeltaOfIndex_Skill(string index, bool positive, NewCharacterStatus att, NewCharacterStatus def) {
            ArrayList results = new ArrayList();
            Debug.Log("caculateDeltaOfIndex_Skill atomic" + att.playerID+" "+att.atomics.ContainsKey(index)+" "+index);
            ArrayList atomics = att.atomics.ContainsKey(index) ? att.atomics[index] : null;
            if (atomics == null) return results;
            foreach (AtomicEffect atomic in atomics)
            {
                Debug.Log("caculateDeltaOfIndex_Skill atomic" + atomic.delta + " " + atomic.index + " " + atomic.condition + " " + atomic.enabled);
                if (!atomic.isActive()) continue;

                float r = calculateExpression(att, def, atomic.delta);
                bool similar = (r >= 0) == positive;
                if (similar) {
                    ArrayList result = new ArrayList();
                    result.Add(r);
                    result.Add(atomic.parent.nick); 
                    results.Add(result);
                }
            }

            return results;
        }
    }
}
