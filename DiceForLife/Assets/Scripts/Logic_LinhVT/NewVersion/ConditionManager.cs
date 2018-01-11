



using System;




using System.Collections;
//using UnityEngine;

namespace CoreLib {
    public class ConditionManager
    {
        public const string E_Impotent = "when_e_is_impotent";
        public const string E_Hypnotic = "when_e_is_hypnotic";
        public const string E_Rot = "when_e_is_rot";
        public const string E_Pain = "when_e_is_pain";
        public const string E_Bleed = "when_e_is_bleed";
        public const string E_Crazy = "when_e_is_crazy";
        public const string E_Dull = "when_e_is_dull";
        public const string E_Stun = "when_e_is_stun"; //
        public const string E_Frostbite = "when_e_is_frostbite"; //
        public const string E_Burn = "when_e_is_burn"; //s
        public const string E_Shock = "when_e_is_shock";
        public const string E_Poisoning = "when_e_is_poisoning"; //
        public const string E_Knockback = "when_e_is_knockback";
        public const string E_Immobilization = "when_e_is_immobilization";
        public const string E_Blind = "when_e_is_blind";
        public const string E_Freezing = "when_e_is_freezing"; //
        public const string E_Glamour = "when_e_is_glamour";
        public const string E_Fear = "when_e_is_fear"; //
        public const string E_Disease = "when_e_is_disease";
        public const string E_Sleep = "when_e_is_sleep"; //
        public const string Hit = "when_hit";
        public const string Be_Hit = "when_is_hit";// moi
        public const string E_Abnormal = "when_e_is_abnormal";
        public const string Abnormal = "when_is_abnormal";

        public const string Impotent = "when_is_impotent";
        public const string Hypnotic = "when_is_hypnotic";
        public const string Rot = "when_is_rot";
        public const string Pain = "when_is_pain";
        public const string Bleed = "when_is_bleed";
        public const string Crazy = "when_is_crazy";
        public const string Dull = "when_is_dull";
        public const string Stun = "when_is_stun"; //
        public const string Frostbite = "when_is_frostbite"; //
        public const string Burn = "when_is_burn"; //s
        public const string Shock = "when_is_shock";
        public const string Poisoning = "when_is_poisoning"; //
        public const string Knockback = "when_is_knockback";
        public const string Immobilization = "when_is_immobilization";
        public const string Blind = "when_is_blind";
        public const string Freezing = "when_is_freezing"; //
        public const string Glamour = "when_is_glamour";
        public const string Fear = "when_is_fear"; //
        public const string Disease = "when_is_disease";
        public const string Sleep = "when_is_sleep"; //

        public const string Critical = "when_critical";
        public const string Ranger = "when_ranger";
        public const string Melee = "when_melee";
        //public const string Hp_greater_10percent_maxhp = "when health > 0.1 * maxhealth";
        //public const string Hp_less_21percent_maxhp = "when health < 0.21 * maxhealth";
        public const string Shield_block_1_round = "when_shield_block_in_1_round";

        // health > 0.15 * maxhealth


        MyDictionary<string, Condition> conditions = new MyDictionary<string, Condition>();

        NewCharacterStatus owner;
        NewLogic logic;

        public void reset() {
            conditions.Clear();
        }


        public ConditionManager(NewCharacterStatus char_Status, NewLogic logic) {
            this.owner = char_Status;
            this.logic = logic;
        }

        public string convertToString() {
            string result = "";
            foreach (string condition in conditions.Keys) {
                result += result == "" ? condition + "-" + conditions[condition].getDuration() : "-" + condition + "-" + conditions[condition].getDuration();
            }
            return result;
        }

        public ArrayList parseToDict(string str) {
            Console.WriteLine("parseToDict" + str);
            string[] parts = str.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            ArrayList result = new ArrayList();
            conditions.Clear();
            for (int i = 0; i < parts.Length / 2; i++) {
                conditions.Add(parts[2 * i], new Condition(parts[2 * i], int.Parse(parts[2 * i + 1])));
                result.Add(parts[2 * i]);
            }
            return result;
        }


        public void enableCondition(string condition) {
            conditions.Add(condition , new Condition(condition, -1));
        }

        public void enableCondition(string condition, int round) {
            conditions.Add(condition, new Condition(condition, round));
        }

        public void disableCondition(string condition) {
            if (conditions.ContainsKey(condition)) conditions.Remove(condition);
        }

        public void decreaseDuration() {
            ArrayList removes = new ArrayList();
            foreach (string key in conditions.Keys) {
                conditions[key].decreaseDuration();
                if (conditions[key].canRemove()) removes.Add(key);
            }
            foreach(string key in removes) {
                conditions.Remove(key);
            }
        }

        public bool checkCondition(string condition) {
            return conditions.ContainsKey(condition) || valueOfCondition(condition);
        }

        public bool valueOfCondition(string condition) {
            
            NewCharacterStatus op_status = logic.getStatusByPlayerID(3 - owner.playerID);
            switch (condition) {
                case "":
                    return true;
                case Critical:
                    return false;
                case Melee:
                    return owner.character.characteristic.Type == Characteristic.CharacterType.MELE;
                case Ranger:
                    return owner.character.characteristic.Type == Characteristic.CharacterType.RANGER;
                case Be_Hit:
                case Hit:
                    return false; // khong the tinh toan chi co tat bat nen de mac dinh la false
                case Abnormal:
                    return valueOfCondition(Impotent)
                        || valueOfCondition(Hypnotic)
                        || valueOfCondition(Rot)
                        || valueOfCondition(Pain)
                        || valueOfCondition(Bleed)
                        || valueOfCondition(Crazy)
                        || valueOfCondition(Dull)
                        || valueOfCondition(Stun)
                        || valueOfCondition(Frostbite)
                        || valueOfCondition(Burn)
                        || valueOfCondition(Shock)
                        || valueOfCondition(Poisoning)
                        || valueOfCondition(Knockback)
                        || valueOfCondition(Immobilization)
                        || valueOfCondition(Blind)
                        || valueOfCondition(Freezing)
                        || valueOfCondition(Glamour)
                        || valueOfCondition(Fear)
                        || valueOfCondition(Disease)
                        || valueOfCondition(Sleep)
                        ; 

                case Impotent:
                    return owner.op_effects.ContainsKey(NewEffect.Impotent);
                case Hypnotic:
                    return owner.op_effects.ContainsKey(NewEffect.Hypnotic) ;
                case Rot:
                    return owner.op_effects.ContainsKey(NewEffect.Rot) ;
                case Pain:
                    return owner.op_effects.ContainsKey(NewEffect.Pain) ;
                case Bleed:
                    return owner.op_effects.ContainsKey(NewEffect.Bleed) ;
                case Crazy:
                    return owner.op_effects.ContainsKey(NewEffect.Crazy) ;
                case Dull:
                    return owner.op_effects.ContainsKey(NewEffect.Dull) ;
                case Stun:
                    return owner.op_effects.ContainsKey(NewEffect.Stun) ;
                case Frostbite:
                    return owner.op_effects.ContainsKey(NewEffect.Frostbite) ;
                case Burn:
                    return owner.op_effects.ContainsKey(NewEffect.Burn) ;
                case Shock:
                    return owner.op_effects.ContainsKey(NewEffect.Shock) ;
                case Poisoning:
                    return owner.op_effects.ContainsKey(NewEffect.Poisoning) ;
                case Knockback:
                    return owner.op_effects.ContainsKey(NewEffect.Knockback) ;
                case Immobilization:
                    return owner.op_effects.ContainsKey(NewEffect.Immobilization) ;
                case Blind:
                    return owner.op_effects.ContainsKey(NewEffect.Blind) ;
                case Freezing:
                    return owner.op_effects.ContainsKey(NewEffect.Freezing) ;
                case Glamour:
                    return owner.op_effects.ContainsKey(NewEffect.Glamour) ;
                case Fear:
                    return owner.op_effects.ContainsKey(NewEffect.Fear) ;
                case Disease:
                    return owner.op_effects.ContainsKey(NewEffect.Disease) ;
                case Sleep:
                    return owner.op_effects.ContainsKey(NewEffect.Sleep) ;
                case E_Abnormal:
                    return valueOfCondition(E_Impotent)
                        ||valueOfCondition(E_Hypnotic)
                        ||valueOfCondition(E_Rot)
                        ||valueOfCondition(E_Pain)
                        ||valueOfCondition(E_Bleed)
                        ||valueOfCondition(E_Crazy)
                        ||valueOfCondition(E_Dull)
                        ||valueOfCondition(E_Stun)
                        ||valueOfCondition(E_Frostbite)
                        ||valueOfCondition(E_Burn)
                        ||valueOfCondition(E_Shock)
                        ||valueOfCondition(E_Poisoning)
                        ||valueOfCondition(E_Knockback)
                        ||valueOfCondition(E_Immobilization)
                        ||valueOfCondition(E_Blind)
                        ||valueOfCondition(E_Freezing)
                        ||valueOfCondition(E_Glamour)
                        ||valueOfCondition(E_Fear)
                        ||valueOfCondition(E_Disease)
                        ||valueOfCondition(E_Sleep)
                        ;
                case E_Impotent :
                    return op_status.op_effects.ContainsKey(NewEffect.Impotent);
                case E_Hypnotic :
                    return op_status.op_effects.ContainsKey(NewEffect.Hypnotic);
                case E_Rot :
                    return op_status.op_effects.ContainsKey(NewEffect.Rot) ;
                case E_Pain :
                    return op_status.op_effects.ContainsKey(NewEffect.Pain) ;
                case E_Bleed :
                    return op_status.op_effects.ContainsKey(NewEffect.Bleed) ;
                case E_Crazy :
                    return op_status.op_effects.ContainsKey(NewEffect.Crazy) ;
                case E_Dull :
                    return op_status.op_effects.ContainsKey(NewEffect.Dull) ;
                case E_Stun :
                    return op_status.op_effects.ContainsKey(NewEffect.Stun) ;
                case E_Frostbite :
                    return op_status.op_effects.ContainsKey(NewEffect.Frostbite) ;
                case E_Burn :
                    return op_status.op_effects.ContainsKey(NewEffect.Burn) ;
                case E_Shock :
                    return op_status.op_effects.ContainsKey(NewEffect.Shock) ;
                case E_Poisoning :
                    return op_status.op_effects.ContainsKey(NewEffect.Poisoning) ;
                case E_Knockback :
                    return op_status.op_effects.ContainsKey(NewEffect.Knockback) ;
                case E_Immobilization :
                    return op_status.op_effects.ContainsKey(NewEffect.Immobilization);
                case E_Blind :
                    return op_status.op_effects.ContainsKey(NewEffect.Blind);
                case E_Freezing :
                    return op_status.op_effects.ContainsKey(NewEffect.Freezing);
                case E_Glamour :
                    return op_status.op_effects.ContainsKey(NewEffect.Glamour) ;
                case E_Fear :
                    return op_status.op_effects.ContainsKey(NewEffect.Fear) ;
                case E_Disease :
                    return op_status.op_effects.ContainsKey(NewEffect.Disease) ;
                case E_Sleep :
                    return op_status.op_effects.ContainsKey(NewEffect.Sleep);
                //case Hp_greater_10percent_maxhp:
                //    return (float)owner.getCurrentIndex(Indexes.hp_na) > 0.1f * (float)owner.getCurrentIndex(Indexes.maX_hp_na);
                //case Hp_less_21percent_maxhp:
                //return (float)owner.getCurrentIndex(Indexes.hp_na) < 0.21f * (float)owner.getCurrentIndex(Indexes.maX_hp_na);
                case Shield_block_1_round:
                    return false;
            } 
            return calcuateLogicExpression(condition); // dieu kien rong thi bao gio cung dung
        }

        private bool isOperator(string op)
        {
            switch (op)
            {
                case "&&":
                case "||":
                case "|":
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "==":
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                case "!":
                    return true;
            }
            return false;
        }

        private float getPrecedence(string operators)
        {
            switch (operators)
            {
                case "(":
                case ")":
                    return 1;
                case "&&":
                    return 1.4f;
                case "||":
                    return 1.2f;
                case "|":
                    return 1.6f;
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "==":
                    return 1.8f;
                case "+":
                    return 2;
                case "-":
                    return 2;
                case "*":
                case "/":
                    return 4;
                case "^":
                    return 5;
                case "!":
                    return 6;
            }
            return 0;
        }

        private bool isHigherOrEqual(string op1, string op2)
        {
            return getPrecedence(op1) >= getPrecedence(op2);
        }

        private object getOperand(string e)
        {// loai bo cac truong hop la bieu thuc (
            NewCharacterStatus vs = logic.getStatusByPlayerID(3 - owner.playerID);
            try
            {// co the la so
                float digit = Convert.ToSingle(e);
                return digit;
            }
            catch (Exception ex)
            {
                try {// co the la boolean

				
                   bool digit = Convert.ToBoolean(e);

				   
                    return digit;
                } catch (Exception ex1) { // la bieu thuc hoac menh de
                    string[] parts = e.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts[0] == "e")// chi so cua enemy
                    {
                        bool isClause = (parts[1] == "when");

                        if (isClause) {
                            return null;// truong hop nay khong bao gio xay ra
                        } else {
                            String index = "";
                            for (int i = 1; i < parts.Length; i++)
                                index += i == 1 ? parts[i] : "_" + parts[i];
                            float digit = (float)vs.getMidIndex(index);
                            return digit;
                        }


                    }
                    else
                    {
                        bool isClause = (parts[0] == "when");
                        if (isClause)
                        {
                            return valueOfCondition(e);
                        }
                        else { 
                            float digit = (float)owner.getMidIndex(e);
                            return digit;
                        }

                    }
                }


            }

        }


        public bool calcuateLogicExpression(string formulas)
        {
            // chia chuoi thanh cac 
            //NewCharacterStatus vs = logic.getStatusByPlayerID(3 - owner.playerID);
            string[] elements = formulas.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine("do dai " + elements.Length);
            foreach (string e in elements)
                Console.Write(e + " ");
            Console.WriteLine();
            ArrayList opstack = new ArrayList();
            ArrayList postfixEx = new ArrayList();
            for (int i = 0; i < elements.Length; i++)
            {
                string e = elements[i];
                switch (e)
                {
                    case "&&":
                    case "||":
                    case "|":
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                    case "==":
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "^":
                    case "!":
                        while (opstack.Count > 0 && isOperator((string)opstack[opstack.Count - 1]) && isHigherOrEqual((string)opstack[opstack.Count - 1], e))
                        {
                            postfixEx.Add(opstack[opstack.Count - 1]);
                            opstack.RemoveAt(opstack.Count - 1);
                        }
                        opstack.Add(e);
                        break;
                    case "(":
                        opstack.Add(e);
                        break;
                    case ")":// lay ra tinh het phep tinh ra cho den khi gap (
                        while ((string)opstack[opstack.Count - 1] != "(" && opstack.Count > 0 && isOperator((string)opstack[opstack.Count - 1]))
                        {
                            string op = (string)opstack[opstack.Count - 1];
                            postfixEx.Add(op);
                            opstack.RemoveAt(opstack.Count - 1);
                        }
                        if (opstack.Count > 0)
                            opstack.RemoveAt(opstack.Count - 1);
                        break;
                    default:
                        object operand = getOperand(e);
                        postfixEx.Add(operand);

                        break;
                }
            }

            while (opstack.Count > 0)
            {
                postfixEx.Add(opstack[opstack.Count - 1]);
                opstack.RemoveAt(opstack.Count - 1);
            }

            foreach (object e in postfixEx)
                Console.Write(e + " ");
            Console.WriteLine();

            // tinh
            for (int i = 0; i < postfixEx.Count; i++)
            {
                if (postfixEx[i] is float || postfixEx[i] is bool)
                {
                    opstack.Add(postfixEx[i]);
                }
                else
                {
                    switch ((string)postfixEx[i])
                    {
                        case "&&":
                            bool yB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            bool xB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(xB && yB);
                            break;
                        case "||":
                            yB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            xB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(xB || yB);
                            break;
                        case "|":
                            yB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(!yB);
                            break;
                        case "!":
                            float y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            float x = 0;
                            //opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(factorial((int)y));
                            break;
                        case ">":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x > y);
                            break;
                        case "<":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x < y);
                            break;
                        case ">=":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x >= y);
                            break;
                        case "<=":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x <= y);
                            break;
                        case "==":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x == y);
                            break;

                        case "^":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add((float)Math.Pow(x, y));
                            break;

                        case "+":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x + y);
                            break;
                        case "-":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x - y);
                            break;
                        case "*":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x * y);
                            break;
                        case "/":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x / y);
                            break;
                        default:
                            Console.WriteLine("toan tu khong biet " + postfixEx[i]);
                            break;
                    }


                }
            }

            Console.WriteLine("fouma = " + formulas+" value = "+(opstack.Count == 0 ? false : (bool)opstack[0]));

			
            return opstack.Count == 0 ? false : (bool)opstack[0];
        }

        public static float factorial(int n)
        {
            float result = 1;
            for (int i = 1; i < n + 1; i++)
            {
                result *= i;
            }
            return result;
        }

    }

    class Condition {
        string condition;
        int duration;
        int skillID;
        int lineID;

        public Condition (string condition, int duration, int skillID = -1, int lineID = -1) {
            this.condition = condition;
            this.duration = duration;
        }

        public int getDuration() {
            return duration;
        }

        public void decreaseDuration() {
            if (duration > 0) duration--;
        }

        public bool canRemove() {
            return duration == 0;
        }

    }
}

