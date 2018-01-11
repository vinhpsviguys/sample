using System;
using System.Collections;
using System.Collections.Generic;

namespace CoreLib
{
    public class Character // Dac ta cac thong so tinh cua character trong 1 tran dau khong co avatar + sach, chi co skill chu dong
    {
        public int playerId;
        public Characteristic characteristic;
        public Equipment[] equipments = new Equipment[12];
        public ArrayList skills = new ArrayList(); // cac skill va thu co the dung
        public int startPoints = 0;
        //public MyDictionary<string, Skill> skillDic = new MyDictionary<string, Skill>();
        public MyDictionary<string, NewSkill> newSkillDic = new MyDictionary<string, NewSkill>();
        public MyDictionary<string, AbnormalStatus> abDic = new MyDictionary<string, AbnormalStatus>();

        public static Character createCharacter(int playerID) {
            Character character = new Character();
            character.playerId = playerID;
            character.characteristic = Characteristic.createCharacteristic();
            character.equipments = Equipment.createEquipments();
            //character.skills = Skill.createSkills();
            //for (int i = 0; i < Skill.MAX_SKILLS; i++)
            //{
            //    character.skillDic.Add(""+i,Skill.createSkill(i));
            //}
            return character;
        }

        public Character loadDictionaries(MyDictionary<string, NewSkill> skills, MyDictionary<string, AbnormalStatus> abs)
        {
            newSkillDic.Clear();
            abDic.Clear();
            foreach (string key in skills.Keys)
            {
                newSkillDic.Add(key, skills[key].clone());
            }

            foreach(string key in abs.Keys)
            {
                abDic.Add(key, abs[key].clone());
            }
            return this;
        }

        //public static Character parseJSON(string aJSON) {
        //    JSONNode obj = JSON.Parse(aJSON);
        //    Character character = new Character();
        //    character.playerId = Convert.ToInt32(obj["playerId"].Value);
        //    character.characteristic = Characteristic.parseJSON(obj["characteristic"].ToString());
        //    character.startPoints = Convert.ToInt32(obj["startPoints"].Value);
        //    JSONArray array = (JSONArray)JSON.Parse(obj["equipments"].ToString());
        //    int i = -1;
        //    foreach (JSONNode node in array) {
        //        i++;
        //        if (node.Tag != JSONNodeType.NullValue) {
        //            character.equipments[i] = Equipment.parseJSON(node.ToString());
        //        }
        //    }
        //    // code tiep skill
        //    JSONArray array2 = (JSONArray)JSON.Parse(obj["skills"].ToString());
        //    foreach (JSONNode node in array2)
        //    {
        //        Skill skill = Skill.parseJSON(node.ToString());
        //        character.skills.Add(skill);
        //    }

        //    return character;
        //}



        //public JSONNode toJSON() {// output
        //    JSONNode obj = new JSONObject();
        //    obj.Add("playerId", new JSONNumber(playerId));
        //    obj.Add("characteristic", characteristic.toJSON());
        //    JSONArray arr = new JSONArray();
        //    for (int i = 0; i < 10; i++) {
        //        arr.Add(equipments[i] != null ? equipments[i].toJSON() : new JSONNull());
        //    }
        //    obj.Add("equipments", arr);
        //    arr = new JSONArray();
        //    foreach (Skill skill in skills) {
        //        arr.Add(skill.toJSON());
        //    }
        //    obj.Add("skills", arr);
        //    obj.Add("startPoints", startPoints);

        //    return obj;
        //}

        //public void saveToFile(string path) {
        //    toJSON().SaveToFile(path);
        //}

        public JSONNode fetchFromFile(string path)
        {
            return JSONNode.LoadFromFile(path);
        }
        // For Vinh
        //public SkillCharacter convertToSkillCharacter(Skill skill)
        //{
        //    SkillCharacter skillChar = new SkillCharacter(Convert.ToInt32(skill.getValue("id")), (int)characteristic.Class, Convert.ToString(skill.getValue("name")), Convert.ToString(skill.getValue("description", "Unknow")), Convert.ToInt32(skill.getValue("level", 1)), Convert.ToInt32(skill.getValue("dmamage",  0)), Convert.ToInt32(skill.getValue("actionpoints", 0)));
        //    return skillChar;
        //}

        //public static Skill convertToSkill(SkillCharacter skillChar)
        //{
        //    Random random = new Random();
        //    Skill skill = new Skill();
        //    skill.setValue("id", skillChar.idSkill);
        //    skill.setValue("level", skillChar.levelSkill);
        //    skill.setValue("name", skillChar.nameSkill);
        //    skill.setValue("damagetype", random.Next(1, 2));
        //    skill.setValue("PhysicalDamage", skillChar.physicalDamage);
        //    skill.setValue("MagicalDamage", skillChar.magicalDamage);
        //    skill.setValue("PureDamage", skillChar.pureDamage);
        //    skill.setValue("actionpoints", skillChar.actionPointRequired);
        //    ArrayList array = new ArrayList();
        //    switch (skillChar.idSkill)
        //    {
        //        case 0: // chem thuong
                    
        //            break;
        //        case 1: //slience
        //            Effect effect = Effect.createSample(Effect.SILENCE_ANY);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 2:// Stun
        //            effect = Effect.createSample(Effect.STUN);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 3: // tang damage vat ly
        //            effect = Effect.createSample(Effect.PDA_RATIO_INC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 4: // giam damage vat ly
        //            effect = Effect.createSample(Effect.PDA_RATIO_DEC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 5: // tang damage phep thuat
        //            effect = Effect.createSample(Effect.MDA_RATIO_INC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 6: // giam damage phep thuat
        //            effect = Effect.createSample(Effect.MDA_RATIO_DEC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 7: // khong duoc dung skill vat ly
        //            effect = Effect.createSample(Effect.PSK_SILENCE);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 8: // khong duoc dung skill phep thuat
        //            effect = Effect.createSample(Effect.MSK_SILENCE);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 9:// tang phong thu vat ly
        //            effect = Effect.createSample(Effect.PDE_RATIO_INC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 10: // giam phong thu vat ly
        //            effect = Effect.createSample(Effect.PDE_RATIO_DEC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 11: // tang phong thu phep thuat
        //            effect = Effect.createSample(Effect.MDE_RATIO_INC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //        case 12: // giam phong thu phep thuat
        //            effect = Effect.createSample(Effect.MDE_RATIO_DEC);
        //            array.Add(effect);
        //            effect.setValue("enemy", skillChar.requireTarget);
        //            break;
        //    }
        //    skill.setValue("effects", array);
        //    return skill;
        //}

        public byte[] convertToByteArr() {
            byte[] bytes = characteristic.convertToByteArr();
            bytes.Concat<byte>(Utilities.convertToByteArr((ushort)startPoints));
            return bytes;
        }


    }
}
