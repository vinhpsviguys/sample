using System;
using System.Collections;
using System.Collections.Generic;


namespace CoreLib
{
    public enum SkillType
    {
        Active,
        Buff,
        Passive
    }

    public class NewSkill
    {
        //Immobilization + Melee
        //Blind + Ranger;

        public JSONNode data;
        // can mot truy xuat tu NewSkill den cac dong atomicEffect
        private int cooldown;
        private int coolDownLimit;
        private int idSkill;
        private string description;
        private int round;
        private int level;

        public NewSkill(JSONNode node)
        {
            this.data = node;
            coolDownLimit = 2 * (data["cooldown"] == null ? 0 : data["cooldown"].AsInt); // gap 2 lan len do tinh theo turn
            cooldown = 0;
            idSkill = data["idInit"].AsInt;
            description = data["description"].Value;
            round = data["duration"] == null ? -1 : data["duration"].AsInt;
            level = data["level"].AsInt;
        }

        public int getActionPoints() {
            return data["aps"].AsInt;
        }

        public int getActionPoints(NewLogic logic) {

            int id = getID();
            NewCharacterStatus player1 = logic.getStatusByPlayerID(1);
            NewCharacterStatus player2 = logic.getStatusByPlayerID(2);
            NewCharacterStatus target = player1.character.newSkillDic.ContainsKey("Skill" + id) && player1.character.newSkillDic["Skill" + id] == this ? player1 : null;
            if (target == null) target = player2.character.newSkillDic.ContainsKey("Skill" + id) && player2.character.newSkillDic["Skill" + id] == this ? player2 : null;
            if (target == null) return getActionPoints();
            else return (int)target.getCurrentIndex("Skill" + id + "_aps");

        }

        public ArrayList affect(NewCharacterStatus attStatus, NewCharacterStatus defStatus)
        {
            bool me = false;
            bool you = false;
            string name = data["name"].Value;
            string nick = data["nick"].Value;
            int level = Convert.ToInt32(data["level"].Value);
            int duration = data["duration"] == null? -1 :  Convert.ToInt32(data["duration"].Value);

            string condition = data["condition"] == null ? "" : data["condition"].Value;
            NewEffect effect = new NewEffect(attStatus.playerID, name, nick, level, 2 * duration, condition); // vi tinh theo turn
            if (condition != "")
            {
                ArrayList enableSkills = attStatus.enableSkills.ContainsKey(condition) ? attStatus.enableSkills[condition] : new ArrayList();
                enableSkills.Add(effect);
                attStatus.enableSkills.Add(condition, enableSkills);
            }

            JSONArray lines = (JSONArray)data["lines"];
            foreach (JSONObject line in lines)
            {
                string delta = line["delta"].Value;
                string index = line["index"].Value;
                string con = line["condition"] == null ? "" : line["condition"].Value;
                bool enemy = line["enemy"] == null ? false : Convert.ToBoolean(line["enemy"].Value);
                duration = line["duration"] == null ? -1 : Convert.ToInt32(line["duration"].Value);
                AtomicEffect atomic = new AtomicEffect(effect, delta, index, enemy, con, duration);
                NewCharacterStatus targetStatus = null;
                if (enemy) {
                    targetStatus = defStatus;
                    // them effect vao danh sach effect
                    // them danh sach atmoic
                    // them danh sach fomula
                    you = true;
                } else {
                    targetStatus = attStatus;
                    me = true;
                }

                //targetStatus.replaceEffect(name, effect);

                ArrayList atomics = targetStatus.atomics.ContainsKey(index) ? targetStatus.atomics[index] : new ArrayList();
                atomics.Add(atomic);
                targetStatus.atomics.Add(index, atomics);

                effect.atomicEffects.Add(atomic);
                // tinh lai cong thuc
                //ArrayList formulas = defStatus.formulas.ContainsKey(index) ? defStatus.formulas[index] : new ArrayList();
                //string formulas = !targetStatus.deltaFormulas.ContainsKey(index) ? "" : targetStatus.deltaFormulas[index] + " + ";
                //formulas += delta;
                //targetStatus.deltaFormulas.Add(index, formulas);

                if (con != "")
                {
                    // them vao danh sach dieu kien
                    ArrayList enableAtomics = targetStatus.enableAtomics.ContainsKey(con) ? targetStatus.enableAtomics[con] : new ArrayList();
                    enableAtomics.Add(atomic);
                    targetStatus.enableAtomics.Add(con, enableAtomics);
                }

            }
            int playerID = 0;

            if (me && you)
            {
                playerID = attStatus.playerID + defStatus.playerID;
            }
            else if (me) playerID = attStatus.playerID;
            else if (you) playerID = defStatus.playerID;


            //NewEffect clone = effect.clone();
            effect.playerID = playerID;

            if (me) attStatus.replaceEffect(name, effect);
            if (you) defStatus.replaceEffect(name, effect);
            // parse Special Atomics
            effect.parseAtomics(attStatus, defStatus);


            ArrayList result = new ArrayList();
            result.Add(effect);

            return result;
        }


        public SkillType getType()
        {
            switch (data["type"].Value)
            {
                case "passive":
                    return SkillType.Passive;
                case "buff":
                    return SkillType.Buff;
                case "active":
                    return SkillType.Active;
                default: return SkillType.Active;
            }
        }

        private string getCondition() {
            return data["condition"] == null ? "" : data["condition"].Value;
        }

        public void addField(string field, object value) {
            if (value is string)
            {
                data.Add(field, new JSONString((string)value));
            }
            else if (value is int) data.Add(field, new JSONNumber((int)value));
            else if (value is float) data.Add(field, new JSONNumber((float)value));
            else if (value is bool) data.Add(field, new JSONBool((bool)value));
        }

        public NewSkill clone()
        {
            JSONNode newData = JSON.Parse(data.ToString());
            return new NewSkill(newData);
        }

        public bool canUse() {
            return cooldown == 0;
        }

        public void use() {
            cooldown = coolDownLimit;
        }

        public void decreaseCoolDown() {
            cooldown = Math.Max(cooldown - 1, 0);
        }

        public int getCoolDown() {
            return (int)Math.Round((1.0f * cooldown) / 2);
        }

        public int getCoolDownLimit() {
            return (int)Math.Round((1.0f * coolDownLimit) / 2);
        }

        public int getRound() {
            return round;
        }

        public int getCoolDownByTurn() {
            return cooldown;
        }

        public void resetCoolDown() {
            cooldown = 0;
        }

        public int getID() {
            return idSkill;
        }

        public void setCoolDown(int cooldown) {
            this.cooldown = cooldown;
        }

        public string getDescription() {
            return description;
        }
        public int getLevel() {
            return level;
        }

        public string getName() {
            return data["name"].Value;
        }

        public string getNick() {
            return data["nick"].Value;
        }

        public bool canCastSkill(NewLogic logic) {
            int id = getID();
            NewCharacterStatus player1 = logic.getStatusByPlayerID(1);
            NewCharacterStatus player2 = logic.getStatusByPlayerID(2);
            NewCharacterStatus target = player1.character.newSkillDic.ContainsKey("Skill" + id) && player1.character.newSkillDic["Skill" + id] == this ? player1 : null;
            if (target == null) target = player2.character.newSkillDic.ContainsKey("Skill" + id) && player2.character.newSkillDic["Skill" + id] == this ? player2 : null;
            if (target == null) return true;
            
            bool canAttack = target.canAttack();
            return  (getType() == SkillType.Buff && target.checkCondition(getCondition()))
                || (getType() == SkillType.Active && target.checkCondition(getCondition()) && canAttack);
        }


        

    }


}
