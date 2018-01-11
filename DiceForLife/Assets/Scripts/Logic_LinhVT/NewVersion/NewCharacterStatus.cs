using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using UnityEngine;




namespace CoreLib
{
    public class NewCharacterStatus // sai sot trong viec tinh duration cua NewEffect va xem lai Abnormal
    {

        public CharacterPlayer character;
        //public Character character;
        public MyDictionary<string, NewEffect> effects = new MyDictionary<string, NewEffect>(); // ghi lai nhung trang thai NewEffect minh tu gay ra cho minh
        public MyDictionary<string, NewEffect> op_effects = new MyDictionary<string, NewEffect>(); // ghi lai nhung trang thai NewEffect doi phuong gay ra cho minh

        public MyDictionary<string, ArrayList> atomics = new MyDictionary<string, ArrayList>(); // bang liet ket cac dong cua skill hay abnormal status anh huong den thuoc tinh nay
        public MyDictionary<string, string> deltaFormulas = new MyDictionary<string, string>(); // bang cong thuc tinh cac thuoc tinh
        public MyDictionary<string, object> initIndexes = new MyDictionary<string, object>();// bang tra cuuc gia tri cac thuoc tinh ban dau
        public MyDictionary<string, object> midIndexes = new MyDictionary<string, object>();// bang gia tri trung gian
        public MyDictionary<string, object> curIndexes = new MyDictionary<string, object>();// bang tra cuu gia tri cac thuoc tinh hien tai

        // dieu kien duoc dien ta boi string va huu han <dieu kien, day cac atomic>
        // dieu kien 
        public MyDictionary<string, ArrayList> enableAtomics = new MyDictionary<string, ArrayList>();// dung cho danh sach cac dong co dieu kien
        public MyDictionary<string, ArrayList> enableSkills = new MyDictionary<string, ArrayList>();// dung cho danh sach cac skill co dieu kien
        //

        public int playerID { get; set; }
        public ConditionManager conManager;



        public NewCharacterStatus(CharacterPlayer _character)
        {
            this.character = _character;
            this.playerID = character.playerId;

            loadCharacteristic();
            //Console.WriteLine("loadCharacteristic");
            loadStaticEquipmentIndexes();
            //Console.WriteLine("loadStaticEquipmentIndexes");
            loadStaticSkillIndexes();
            loadAbnormalStatusIndex();
            //Console.WriteLine("loadAbnormalStatusIndex");
            recomputeIndexes();
            //Console.WriteLine("recomputeIndexes");
            loadStaticIndexFromInitToMid();
            //Console.WriteLine("loadStaticIndexFromInitToMid");
            Console.WriteLine("loadStaticIndexFromInitToMid");
        }


        public void loadDynamicIndexToMid()
        {
            Debug.Log(" loadDynamicIndexToMid ");
            midIndexes.Add(Indexes.min_pda_na, 0f);
            midIndexes.Add(Indexes.max_pda_na, 0f);
            midIndexes.Add(Indexes.min_pde_na, 0f);
            midIndexes.Add(Indexes.max_pde_na, 0f);
            for (int i = 0; i < character.equipments.Length; i++)
            {
                Equipment e = character.equipments[i];
                if (e == null) continue;
                foreach (String key in e.dynamicIndexes.Keys)
                {
                    String name = (String)e.staticIndexes["name"];
                    CoreLib.Range<float> pair = (CoreLib.Range<float>)e.dynamicIndexes[key];
                    midIndexes.Add(name + "_min_" + key, pair.Minimum);
                    midIndexes.Add(name + "_max_" + key, pair.Maximum);
                }
            }
            
            for (int i = 0; i < character.equipments.Length; i++)
            {
                Equipment e = character.equipments[i];
                if (e == null) continue;
                String name = (String)e.staticIndexes["name"];
                switch (name)
                {
                    case "weapon":
                    case "weapon2":
                        float value = (float)getMidIndex(Indexes.min_pda_na);

                        value += ((float)getMidIndex(name + "_" + Indexes.min_pda_na)) * (1 + (float)getMidIndex(name + "_" + Indexes.min_pre_na));
                        midIndexes.Add(Indexes.min_pda_na, value);
                        Debug.Log(name+" min_pda_na "+value);

                        value = (float)getMidIndex(Indexes.max_pda_na);
                        value += ((float)getMidIndex(name + "_" + Indexes.max_pda_na)) * (1 + (float)getMidIndex(name + "_" + Indexes.max_pre_na));
                        midIndexes.Add(Indexes.max_pda_na, value);
                        Debug.Log(name + " max_pda_na " + value);

                        value = (float)getMidIndex(Indexes.min_mda_na);
                        value += ((float)getMidIndex(name + "_" + Indexes.min_mda_na)) * (1 + (float)getMidIndex(name + "_" + Indexes.min_mre_na));
                        midIndexes.Add(Indexes.min_mda_na, value);
                        Debug.Log(name + " min_mda_na " + value);

                        value = (float)getMidIndex(Indexes.max_mda_na);
                        value += ((float)getMidIndex(name + "_" + Indexes.max_mda_na)) * (1 + (float)getMidIndex(name + "_" + Indexes.max_mre_na));
                        midIndexes.Add(Indexes.max_mda_na, value);
                        Debug.Log(name + " max_mda_na " + value);

                        break;
                    default:
                        value = (float)getMidIndex(Indexes.min_pde_na);
                        value += (float)getMidIndex(name + "_" + Indexes.min_pde_na) * (1 + (float)getMidIndex(name + "_" + Indexes.min_prd_na));
                        midIndexes.Add(Indexes.min_pde_na, value);

                        value = (float)getMidIndex(Indexes.max_pde_na);
                        value += (float)getMidIndex(name + "_" + Indexes.max_pde_na) * (1 + (float)getMidIndex(name + "_" + Indexes.max_prd_na));
                        midIndexes.Add(Indexes.max_pde_na, value);

                        value = (float)getMidIndex(Indexes.min_mde_na);
                        value += (float)getMidIndex(name + "_" + Indexes.min_mde_na) * (1 + (float)getMidIndex(name + "_" + Indexes.min_mrd_na));
                        midIndexes.Add(Indexes.min_mde_na, value);

                        value = (float)getMidIndex(Indexes.max_mde_na);
                        value += (float)getMidIndex(name + "_" + Indexes.max_mde_na) * (1 + (float)getMidIndex(name + "_" + Indexes.max_mrd_na));
                        midIndexes.Add(Indexes.max_mde_na, value);
                        break;

                }
            }
            {
                float value = (float)getMidIndex(Indexes.min_pde_na);
                value += (float)getMidIndex(Indexes.char_pde_na);
                midIndexes.Add(Indexes.min_pde_na, value);

                value = (float)getMidIndex(Indexes.max_pde_na);
                value += (float)getMidIndex(Indexes.char_pde_na);
                midIndexes.Add(Indexes.max_pde_na, value);

                value = (float)getMidIndex(Indexes.min_mde_na);
                value += (float)getMidIndex(Indexes.char_mde_na);
                midIndexes.Add(Indexes.min_mde_na, value);

                value = (float)getMidIndex(Indexes.max_mde_na);
                value += (float)getMidIndex(Indexes.char_mde_na);
                midIndexes.Add(Indexes.max_mde_na, value);

                value = (float)getMidIndex(Indexes.min_pda_na);
                value += (float)getMidIndex(Indexes.char_pda_na);
                midIndexes.Add(Indexes.min_pda_na, value);

                value = (float)getMidIndex(Indexes.max_pda_na);
                value += (float)getMidIndex(Indexes.char_pda_na);
                midIndexes.Add(Indexes.max_pda_na, value);

                value = (float)getMidIndex(Indexes.min_mda_na);
                value += (float)getMidIndex(Indexes.char_mda_na);
                midIndexes.Add(Indexes.min_mda_na, value);

                value = (float)getMidIndex(Indexes.max_mda_na);
                value += (float)getMidIndex(Indexes.char_mda_na);
                midIndexes.Add(Indexes.max_mda_na, value);
            }
            //if ((float)getMidIndex(Indexes.min_pda_na) == 0) {

            //    midIndexes.Add(Indexes.min_pda_na, getMidIndex(Indexes.char_pda_na));
            //}

            //if ((float)getMidIndex(Indexes.max_pda_na) == 0)
            //{
            //    midIndexes.Add(Indexes.max_pda_na, getMidIndex(Indexes.char_pda_na));
            //}

            //if ((float)getMidIndex(Indexes.min_mda_na) == 0)
            //{
            //    midIndexes.Add(Indexes.min_mda_na, getMidIndex(Indexes.char_mda_na));
            //}

            //if ((float)getMidIndex(Indexes.max_mda_na) == 0)
            //{
            //    midIndexes.Add(Indexes.max_mda_na, getMidIndex(Indexes.char_mda_na));
            //}
        }









        //public NewCharacterStatus(Character character)
        //{
        //    this.character = _character;
        //    this.playerID = character.playerId;

        //    loadCharacteristic();
        //    //Console.WriteLine("loadCharacteristic");
        //    loadStaticEquipmentIndexes();
        //    //Console.WriteLine("loadStaticEquipmentIndexes");
        //    loadStaticSkillIndexes();
        //    loadAbnormalStatusIndex();
        //    //Console.WriteLine("loadAbnormalStatusIndex");
        //    recomputeIndexes();
        //    //Console.WriteLine("recomputeIndexes");
        //    loadStaticIndexFromInitToMid();
        //    //Console.WriteLine("loadStaticIndexFromInitToMid");
        //}

        //public NewCharacterStatus(Character character)
        //{
        //    this.character = character;
        //    this.playerID = character.playerId;
        //    // indexes of character
        //    loadCharacteristic();
        //    // not range indexes of equipments
        //    loadStaticEquipmentIndexes();
        //    // indexes of skills of character
        //    loadStaticSkillIndexes();
        //    // indexes of abnormalStatus character affects op
        //    loadAbnormalStatusIndex();
        //    // plus all static indexes totally
        //    recomputeIndexes();
        //    // transfer init to mid to prepare to compute
        //    loadStaticIndexFromInitToMid();
        //    //Console.WriteLine("loadStaticIndexFromInitToMid");
        //}


        public NewCharacterStatus setConditionManager(ConditionManager conManager)
        {
            this.conManager = conManager;
            return this;
        }

        //private void loadAbnormalStatusIndexFromCharacterPlayer(CharacterPlayer character)
        //{
        //    MyDictionary<string, AbnormalStatus> abs = character.abDic;
        //    foreach (string status in abs.Keys)
        //    {
        //        initIndexes.Add(abs[status].getName() + "_duration", 2.0f * abs[status].getDuration());
        //    }
        //}

        private void loadStaticSkillIndexes()
        {
            MyDictionary<string, NewSkill> skills = character.newSkillDic;
            foreach (string skill in skills.Keys)
            {
                // level fof skill
                // aps of skill
                initIndexes.Add(skill + "_level", skills[skill].getLevel() * 1.0f);
                initIndexes.Add(skill + "_aps", skills[skill].getActionPoints() * 1.0f);
            }
        }

        private void loadAbnormalStatusIndex()
        {
            MyDictionary<string, AbnormalStatus> abs = character.abDic;
            foreach (string status in abs.Keys)
            {
                initIndexes.Add(abs[status].getName() + "_duration", 2.0f * abs[status].getDuration());
            }
        }

        //private void loadCharacteristicFromCharacterPlayer(CharacterPlayer character)
        //{
        //    Characteristic characteristic = character.characteristic;
        //    initIndexes.Add(Indexes.level_na, characteristic.Level);
        //    initIndexes.Add(Indexes.class_na, characteristic.Class);
        //    initIndexes.Add(Indexes.type_na, characteristic.Type);
        //    initIndexes.Add(Indexes.str_na, characteristic.Strength);
        //    initIndexes.Add(Indexes.int_na, characteristic.Intelligence);
        //    initIndexes.Add(Indexes.vit_na, characteristic.Vitality);
        //    initIndexes.Add(Indexes.hp_na, 0f);
        //    initIndexes.Add(Indexes.maX_hp_na, 0f);
        //    initIndexes.Add(Indexes.dex_na, characteristic.Dexterity);
        //    initIndexes.Add(Indexes.foc_na, characteristic.Focus);

        //    initIndexes.Add(Indexes.agi_na, characteristic.Agility);
        //    initIndexes.Add(Indexes.lucK_na, characteristic.Luck);
        //    initIndexes.Add(Indexes.endura_na, characteristic.Endurance);
        //    initIndexes.Add(Indexes.bLess_na, characteristic.Blessing);
        //    initIndexes.Add(Indexes.prOtec_na, characteristic.Protection);

        //    initIndexes.Add(Indexes.pda_na, 0f);
        //    initIndexes.Add(Indexes.mda_na, 0f);
        //    initIndexes.Add(Indexes.pde_na, 0f);
        //    initIndexes.Add(Indexes.mde_na, 0f);
        //    initIndexes.Add(Indexes.cri_cha_na, 0f);
        //    initIndexes.Add(Indexes.crid_da_na, 0f);
        //    initIndexes.Add(Indexes.mul_ca_cha_na, 0f);
        //    initIndexes.Add(Indexes.block_cha_na, 0f);
        //    initIndexes.Add(Indexes.dOdGe_cha_na, 0f);
        //    initIndexes.Add(Indexes.hp_recor_na, 0f);
        //    //initIndexes.Add(Indexes.);
        //}


        private void loadCharacteristic()
        {
            Characteristic characteristic = character.characteristic;
            initIndexes.Add("player_" + Indexes.level_na, characteristic.Level);
            initIndexes.Add(Indexes.class_na, characteristic.Class);
            initIndexes.Add(Indexes.type_na, characteristic.Type);
            initIndexes.Add(Indexes.str_na, characteristic.Strength);
            initIndexes.Add(Indexes.int_na, characteristic.Intelligence);
            initIndexes.Add(Indexes.vit_na, characteristic.Vitality);
            initIndexes.Add(Indexes.hp_na, 0f);
            initIndexes.Add(Indexes.max_hp_na, 0f);
            initIndexes.Add(Indexes.dex_na, characteristic.Dexterity);
            initIndexes.Add(Indexes.foc_na, characteristic.Focus);

            initIndexes.Add(Indexes.agi_na, characteristic.Agility);
            initIndexes.Add(Indexes.lucK_na, characteristic.Luck);
            initIndexes.Add(Indexes.endura_na, characteristic.Endurance);
            initIndexes.Add(Indexes.bLess_na, characteristic.Blessing);
            initIndexes.Add(Indexes.prOtec_na, characteristic.Protection);

            initIndexes.Add(Indexes.pda_na, 0f);
            initIndexes.Add(Indexes.mda_na, 0f);
            initIndexes.Add(Indexes.pde_na, 0f);
            initIndexes.Add(Indexes.mde_na, 0f);
            initIndexes.Add(Indexes.cri_cha_na, 0f);
            initIndexes.Add(Indexes.crid_da_na, 0f);
            initIndexes.Add(Indexes.mul_ca_cha_na, 0f);
            initIndexes.Add(Indexes.block_cha_na, 0f);
            initIndexes.Add(Indexes.dOdGe_cha_na, 0f);
            initIndexes.Add(Indexes.hp_recor_na, 0f);
            //initIndexes.Add(Indexes.);
        }

        //private void loadStaticEquipmentIndexesFromCharacterPlayer(CharacterPlayer _character)
        //{
        //    Equipment[] equipments = _character.equipments;
        //    foreach (Equipment e in equipments)
        //    {
        //        if (e == null) continue;
        //        MyDictionary<string, object> staticIndexes = e.staticIndexes;
        //        foreach (string key in staticIndexes.Keys)
        //        {
        //            object value = getIndex(staticIndexes, key, 0);
        //            if (value is int)
        //            {
        //                int total = (int)getIndex(staticIndexes, key, 0);
        //                total += (int)value;
        //                initIndexes.Add(key, total);
        //            }
        //            else if (value is float)
        //            {
        //                float total = (float)getIndex(staticIndexes, key, 0);
        //                total += (float)value;
        //                initIndexes.Add(key, total);
        //            }
        //        }
        //    }

        //    // nap cac chi so tinh vu khi vao
        //}

        // weapong 2 neu la static thi ok 
        private void loadStaticEquipmentIndexes()
        {
            Equipment[] equipments = character.equipments;
            foreach (Equipment e in equipments)
            {
                if (e == null) continue;
                MyDictionary<string, object> staticIndexes = e.staticIndexes;
                foreach (string key in staticIndexes.Keys)
                {
                    object value = getIndex(staticIndexes, key, 0);

                    if (key == "physical_absorption" || key == "magical_absorption" || key == "physical_damage_percent" || key == "magical_damage_percent") {// doi voi avatar
                        string name = staticIndexes["name"] as string;
                        initIndexes.Add(name+"_"+key, staticIndexes[key]);
                    } 
                    if (value is int)
                    {
                        int total = (int)getIndex(initIndexes, key, 0);
                        total += (int)value;
                        initIndexes.Add(key, total);
                    }
                    else if (value is float)
                    {
                        float total = (float)getIndex(initIndexes, key, 0f);
                        total += (float)value;
                        initIndexes.Add(key, total);
                    }
                    Debug.Log(key+":"+staticIndexes[key]);

                }
            }

            // nap cac chi so tinh vu khi vao
        }

        private void loadStaticIndexFromInitToMid()
        {
            // xu ly rieng truong hop maxhealth
            float maxhealth_percent = (float)getIndex(initIndexes, Indexes.max_hp_per_na, 0f);
            if (maxhealth_percent > 0) {// chi so max_percent cua do
                AtomicEffect atomic = new AtomicEffect(null, maxhealth_percent+" * "+Indexes.max_hp_na, Indexes.max_hp_na, false, "");
            }
            //
            foreach (string key in initIndexes.Keys)
            {
                midIndexes.Add(key, initIndexes[key]);
            }
        }

        public void loadDynamicIndexToMid(NewCharacterStatus vs)
        {// when in inTurn
            for (int i = 0; i < character.equipments.Length; i++)
            {
                Equipment e = character.equipments[i];
                if (e == null) continue;
                foreach (String key in e.dynamicIndexes.Keys)
                {
                    String name = (String)e.staticIndexes["name"];
                    switch (name)
                    {
                        case "weapon":
                        case "weapon2":
                            switch (key)
                            {
                                case "physical_damage":
                                    // truong hop rieng

                                    break;
                                case "magical_damage":
                                    // truong hop rieng
                                    break;
                                default:
                                    midIndexes.Add(name + "_" + key, e.getValue(key, 0));
                                    break;
                            }
                            break;
                        default:
                            midIndexes.Add(name + "_" + key, e.getValue(key, 0));
                            break;
                    }
                }

                foreach (String key in e.dynamicIndexes.Keys)
                {
                    String name = (String)e.staticIndexes["name"];
                    switch (name)
                    {
                        case "weapon":
                        case "weapon2":
                            switch (key)
                            {
                                case "physical_damage":
                                    float attack_rate = (float)getMidIndex("attack_rate");
                                    float parry_rate = (float)vs.getMidIndex("parry_rate");
                                    midIndexes.Add(name + "_" + key, e.getPhysicalDamage(attack_rate, parry_rate));
                                    break;
                                case "magical_damage":
                                    // truong hop rieng
                                    attack_rate = (float)getMidIndex("attack_rate");
                                    parry_rate = (float)vs.getMidIndex("parry_rate");
                                    midIndexes.Add(name + "_" + key, e.getMagicalDamage(attack_rate, parry_rate));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }

            }

            midIndexes.Add("weapon_damage", (float)getMidIndex("weapon_physical_damage") + (float)getMidIndex("weapon_magical_damage") + (float)getMidIndex("weapon_pure_damage") +
                           (float)getMidIndex("weapon2_physical_damage") + (float)getMidIndex("weapon2_magical_damage") + (float)getMidIndex("weapon2_pure_damage"));
        }




        //public void recomputeIndexesFromCharacterPlayer(CharacterPlayer character)
        //{
        //    float Level = character.characteristic.Level;

        //    float Strength = (float)getIndex(initIndexes, Indexes.str_na, 0f);
        //    float Intelligence = (float)getIndex(initIndexes, Indexes.int_na, 0f);
        //    float Dexterity = (float)getIndex(initIndexes, Indexes.dex_na, 0f);
        //    float Focus = (float)getIndex(initIndexes, Indexes.foc_na, 0f);
        //    float Vitality = (float)getIndex(initIndexes, Indexes.vit_na, 0f);
        //    float Agility = (float)getIndex(initIndexes, Indexes.agi_na, 0f);

        //    float PhysicalDamage = Constants.DAMAGE_PER_TIMES[(int)Level - 1] + Constants.DAMAGE_PER_TIMES[(int)character.characteristic.Strength - (int)Level + 1] + Constants.DAMAGE_PER_TIMES[(int)Strength - (int)character.characteristic.Strength];// Increases the damage from physical attacks - thể hiện damage vật lý
        //    PhysicalDamage += (float)getIndex(initIndexes, Indexes.pda_na, 0f);
        //    initIndexes.Add(Indexes.pda_na, PhysicalDamage);

        //    float PhysicalDefense = Constants.DEFEND_PER_TIMES[(int)Level - 1] + Constants.DEFEND_PER_TIMES[(int)character.characteristic.Strength - (int)Level + 1] + Constants.DEFEND_PER_TIMES[(int)Strength - (int)character.characteristic.Strength];// Take less damage from opponent's physical attack – Giảm lượng damage vật lý phải nhận
        //    PhysicalDefense += (float)getIndex(initIndexes, Indexes.pde_na, 0f);
        //    initIndexes.Add(Indexes.pde_na, PhysicalDefense);

        //    float MagicalDamage = Constants.DAMAGE_PER_TIMES[(int)Level - 1] + Constants.DAMAGE_PER_TIMES[(int)character.characteristic.Intelligence - (int)Level + 1] + Constants.DAMAGE_PER_TIMES[(int)Intelligence - (int)character.characteristic.Intelligence];// Increases the damage from Magical attacks - thể hiện damage phép thuật
        //    MagicalDamage += (float)getIndex(initIndexes, Indexes.mda_na, 0f);
        //    initIndexes.Add(Indexes.mda_na, MagicalDamage);

        //    float MagicalDefense = Constants.DEFEND_PER_TIMES[(int)Level - 1] + Constants.DEFEND_PER_TIMES[(int)character.characteristic.Intelligence - (int)Level + 1] + Constants.DEFEND_PER_TIMES[(int)Intelligence - (int)character.characteristic.Intelligence];// Take less damage from opponent's magical attack – Giảm lượng damage phép thuật phải nhận
        //    MagicalDefense += (float)getIndex(initIndexes, Indexes.mde_na, 0f);
        //    initIndexes.Add(Indexes.mde_na, MagicalDefense);

        //    float Max_Health = 1000 + Constants.HP_PER_TIMES[(int)Level - 1] + Constants.HP_PER_TIMES[(int)character.characteristic.Vitality - (int)Level + 1] + Constants.HP_PER_TIMES[(int)Vitality - (int)character.characteristic.Vitality];
        //    Max_Health += (float)getIndex(initIndexes, Indexes.maX_hp_na, 0f);
        //    initIndexes.Add(Indexes.maX_hp_na, Max_Health);

        //    float Health = Max_Health;
        //    initIndexes.Add(Indexes.hp_na, Health);

        //    float CriticalDamage = Dexterity * 0.01f;
        //    CriticalDamage += (float)getIndex(initIndexes, Indexes.crid_da_na, 0f);
        //    initIndexes.Add(Indexes.crid_da_na, CriticalDamage);

        //    float MulticastChance = Focus * 0.01f;
        //    MulticastChance += (float)getIndex(initIndexes, Indexes.mul_ca_cha_na, 0f);
        //    initIndexes.Add(Indexes.mul_ca_cha_na, MulticastChance);

        //    float HealthRecovery = Vitality * 0.01f * Max_Health;
        //    HealthRecovery += (float)getIndex(initIndexes, Indexes.hp_recor_na, 0f);
        //    initIndexes.Add(Indexes.hp_recor_na, HealthRecovery);

        //    float DodgeChance = Agility * 0.01f;
        //    DodgeChance += (float)getIndex(initIndexes, Indexes.dOdGe_cha_na, 0f);
        //    initIndexes.Add(Indexes.dOdGe_cha_na, DodgeChance);

        //    //         foreach (string index in initIndexes.Keys)
        //    //{
        //    //            Console.WriteLine("initIndexes  " + index + " = " + initIndexes[index]);
        //    //}
        //}


        public void recomputeIndexes()
        {
            float Level = character.characteristic.Level;

            Console.WriteLine("recomputeIndexes " + Level + " " + Constants.DAMAGE_PER_TIMES[(int)Level - 1] + " " + Constants.DEFEND_PER_TIMES[(int)Level - 1]);

            float Strength = (float)getIndex(initIndexes, Indexes.str_na, 0f);
            float Intelligence = (float)getIndex(initIndexes, Indexes.int_na, 0f);
            float Dexterity = (float)getIndex(initIndexes, Indexes.dex_na, 0f);
            float Focus = (float)getIndex(initIndexes, Indexes.foc_na, 0f);
            float Vitality = (float)getIndex(initIndexes, Indexes.vit_na, 0f);
            float Agility = (float)getIndex(initIndexes, Indexes.agi_na, 0f);

            float PhysicalDamage = Constants.DAMAGE_PER_TIMES[(int)Level - 1] + Constants.DAMAGE_PER_TIMES[(int)character.characteristic.Strength - (int)Level + 1] + Constants.DAMAGE_PER_TIMES[(int)Strength - (int)character.characteristic.Strength];// Increases the damage from physical attacks - thể hiện damage vật lý
            PhysicalDamage += (float)getIndex(initIndexes, Indexes.pda_na, 0f);
            initIndexes.Remove(Indexes.pda_na);
            initIndexes.Add(Indexes.char_pda_na, PhysicalDamage);

            float PhysicalDefense = Constants.DEFEND_PER_TIMES[(int)Level - 1] + Constants.DEFEND_PER_TIMES[(int)character.characteristic.Strength - (int)Level + 1] + Constants.DEFEND_PER_TIMES[(int)Strength - (int)character.characteristic.Strength];// Take less damage from opponent's physical attack – Giảm lượng damage vật lý phải nhận
            PhysicalDefense += (float)getIndex(initIndexes, Indexes.pde_na, 0f);
            initIndexes.Remove(Indexes.pde_na);
            initIndexes.Add(Indexes.char_pde_na, PhysicalDefense);

            float MagicalDamage = Constants.DAMAGE_PER_TIMES[(int)Level - 1] + Constants.DAMAGE_PER_TIMES[(int)character.characteristic.Intelligence - (int)Level + 1] + Constants.DAMAGE_PER_TIMES[(int)Intelligence - (int)character.characteristic.Intelligence];// Increases the damage from Magical attacks - thể hiện damage phép thuật
            MagicalDamage += (float)getIndex(initIndexes, Indexes.mda_na, 0f);
            initIndexes.Remove(Indexes.mda_na);
            initIndexes.Add(Indexes.char_mda_na, MagicalDamage);

            float MagicalDefense = Constants.DEFEND_PER_TIMES[(int)Level - 1] + Constants.DEFEND_PER_TIMES[(int)character.characteristic.Intelligence - (int)Level + 1] + Constants.DEFEND_PER_TIMES[(int)Intelligence - (int)character.characteristic.Intelligence];// Take less damage from opponent's magical attack – Giảm lượng damage phép thuật phải nhận
            MagicalDefense += (float)getIndex(initIndexes, Indexes.mde_na, 0f);
            initIndexes.Remove(Indexes.mde_na);
            initIndexes.Add(Indexes.char_mde_na, MagicalDefense);

            float Max_Health = 1000 + Constants.HP_PER_TIMES[(int)Level - 1] + Constants.HP_PER_TIMES[(int)character.characteristic.Vitality - (int)Level + 1] + Constants.HP_PER_TIMES[(int)Vitality - (int)character.characteristic.Vitality];

            Max_Health += (float)getIndex(initIndexes, Indexes.max_hp_na, 0f);
            initIndexes.Add(Indexes.max_hp_na, Max_Health);
            //UnityEngine.Console.WriteLine("max health " + Max_Health +" level charater: "+ character.characteristic.Level+" vitality char: "+ character.characteristic.Vitality+" total vit: "+ Vitality);

            float Health = Max_Health;
            initIndexes.Add(Indexes.hp_na, Health);



            float CriticalDamage = 1 + Dexterity * 0.01f;


            CriticalDamage += (float)getIndex(initIndexes, Indexes.crid_da_na, 0f);
            initIndexes.Add(Indexes.crid_da_na, CriticalDamage);

            float MulticastChance = Focus * 0.01f;
            MulticastChance += (float)getIndex(initIndexes, Indexes.mul_ca_cha_na, 0f);
            initIndexes.Add(Indexes.mul_ca_cha_na, MulticastChance);

            float HealthRecovery = Constants.RECOVERY_PER_TIMES[(int)Level - 1] + Constants.RECOVERY_PER_TIMES[(int)character.characteristic.Vitality - (int)Level + 1] + Constants.RECOVERY_PER_TIMES[(int)Vitality - (int)character.characteristic.Vitality];//Vitality * 0.01f * Max_Health;

            HealthRecovery += (float)getIndex(initIndexes, Indexes.hp_recor_na, 0f);
            initIndexes.Add(Indexes.hp_recor_na, HealthRecovery);

            float DodgeChance = Agility * 0.01f;
            DodgeChance += (float)getIndex(initIndexes, Indexes.dOdGe_cha_na, 0f);
            initIndexes.Add(Indexes.dOdGe_cha_na, DodgeChance);

            //         foreach (string index in initIndexes.Keys)
            //{
            //            Console.WriteLine("initIndexes  " + index + " = " + initIndexes[index]);
            //}
        }

        public void preProcessBeforeDamaging(NewCharacterStatus vs)
        {// chi la tinh tong hop thoi
         //public const string echar_pda_na = "echar_physical_damage";
         //public const string echar_mda_na = "echar_magical_damage";
         //public const string echar_pde_na = "echar_physical_defense";
         //public const string echar_mde_na = "echar_magical_defense";
            Console.WriteLine("+++++++++++++++++++++++++preProcessBeforeDamaging id " + playerID);
            float echar_pda = (float)getCurrentIndex(Indexes.char_pda_na);
            float echar_mda = (float)getCurrentIndex(Indexes.char_mda_na);
            echar_pda += ((float)calculateIndex(vs, "weapon_" + Indexes.pda_na)) * (1 + (float)getCurrentIndex("weapon_" + Indexes.pre_na));
            echar_mda += ((float)calculateIndex(vs, "weapon_" + Indexes.mda_na)) * (1 + (float)getCurrentIndex("weapon_" + Indexes.mre_na));
            echar_pda += ((float)calculateIndex(vs, "weapon2_" + Indexes.pda_na)) * (1 + (float)getCurrentIndex("weapon2_" + Indexes.pre_na));
            echar_mda += ((float)calculateIndex(vs, "weapon2_" + Indexes.mda_na)) * (1 + (float)getCurrentIndex("weapon2_" + Indexes.mre_na));
            float PT = (float)getCurrentIndex(Indexes.char_pde_na);
            foreach (Equipment e in character.equipments)
            {
                if (e == null) continue;
                string name = (string)e.staticIndexes["name"];
                PT += (float)calculateIndex(vs, name + "_" + Indexes.pde_na) * (1 + (float)getCurrentIndex(name + "_" + Indexes.prd_na));
            }
            float echar_pde = PT;
            PT = (float)getCurrentIndex(Indexes.char_mde_na);
            foreach (Equipment e in character.equipments)
            {
                if (e == null) continue;
                string name = (string)e.staticIndexes["name"];
                PT += (float)calculateIndex(vs, name + "_" + Indexes.mde_na) * (1 + (float)getCurrentIndex(name + "_" + Indexes.mrd_na));
            }
            float echar_mde = PT;
            midIndexes.Add(Indexes.echar_pda_na, echar_pda);
            midIndexes.Add(Indexes.echar_mda_na, echar_mda);
            midIndexes.Add(Indexes.echar_pde_na, echar_pde);
            midIndexes.Add(Indexes.echar_mde_na, echar_mde);
        }

        public ArrayList damageWith(NewCharacterStatus vs)
        {
            //∑ Sát thương vật lý vũ khí tổng = [((VK + ∑NVAtk) * (100 % + % GT) * ∑% CM + ∑SKactive – ∑PT] 
            //*(100 % + ∑% SKpassive) 
            //*(100 % - % HTts) 
            //* (100 % - ∑% HTskill) 
            //*(100 % - % ENarmor) 
            //* (100 % + ∑% Avatar) 
            //*(100 % + % Sách)
            System.Random random = new System.Random();
            float Critical_Chance = (float)calculateIndexAndSave(vs, Indexes.cri_cha_na);
            float CM = random.NextDouble() < Critical_Chance ? 1 + (float)calculateIndexAndSave(vs, Indexes.crid_da_na) : 1;
            if (CM > 1)
            {
                enableCondition(ConditionManager.Critical);
                CM = 1 + (float)calculateIndexAndSave(vs, Indexes.crid_da_na);
            }


            float VK_NVATK_GT = calculateIndexAndSave(vs, Indexes.echar_pda_na);

            float SKactive = (float)calculateIndexAndSave(vs, Indexes.pda_na) - (float)vs.calculateIndexAndSave(this, Indexes.pde_na);


            float PT = vs.calculateIndexAndSave(this, Indexes.echar_pde_na);

            float SKpassive = (float)calculateIndexAndSave(vs, Indexes.pda_per_na) - (float)vs.calculateIndexAndSave(this, Indexes.pde_per_na);
            float HTts = (float)vs.getCurrentIndex("ring_physical_absorption") + (float)vs.getCurrentIndex("necklace_physical_absorption") + (float)vs.getCurrentIndex("avatar_physical_absorption");
            float HTskill = (float)vs.calculateIndexAndSave(this, Indexes.pab_na);
            float ENarmor = (float)vs.getCurrentIndex(Indexes.endura_na);
            float Avatar = (float)getCurrentIndex("avatar_physical_damage_percent");
            float Sach = (float)getCurrentIndex("damage_against_"+vs.character.characteristic.Class.ToString());
            Console.WriteLine(" Physical VK " + (float)getCurrentIndex("weapon_physical_damage") + " ");
            Console.WriteLine(" Physical VK2 " + (float)getCurrentIndex("weapon2_physical_damage") + " ");

            Console.WriteLine(" Physical Critical Chance " + Critical_Chance + " ");

            Console.WriteLine(" Physical NVATK " + (float)getCurrentIndex(Indexes.char_pda_na) + " ");
            Console.WriteLine(" Physical GT " + (float)getCurrentIndex("weapon_" + Indexes.pre_na) + " ");
            Console.WriteLine(" Physical GT2 " + (float)getCurrentIndex("weapon2_" + Indexes.pre_na) + " ");
            Console.WriteLine(" Physical VK_NVATK_GT " + VK_NVATK_GT + " ");

            Console.WriteLine(" Physical SKactive " + SKactive + " ");
            Console.WriteLine(" Physical CM " + CM + " ");
            Console.WriteLine(" Physical PT " + PT + " ");
            Console.WriteLine(" Physical SKpassive " + SKpassive + " ");
            Console.WriteLine(" Physical HTts " + HTts + " ");
            Console.WriteLine(" Physical HTskill " + HTskill + " ");
            Console.WriteLine(" Physical ENarmor " + ENarmor + " ");
            Console.WriteLine(" Physical Avatar " + Avatar + " ");
            Console.WriteLine(" Physical Sach " + Sach + " ");

            float Physical_Damage = (VK_NVATK_GT * CM + SKactive - PT) * (1 + SKpassive) * (1 - HTts) * (1 - HTskill) * (1 - ENarmor) * (1 + Avatar) * (1 + Sach);
            Physical_Damage = Math.Max(0, Physical_Damage);
            Console.WriteLine(" Physical Damage " + Physical_Damage + " ");
            midIndexes.Add(Indexes.total_physical_damage_na, Physical_Damage);
            Physical_Damage = calculateIndexAndSave(vs, Indexes.total_physical_damage_na);
            Console.WriteLine(" Total Physical Damage " + Physical_Damage + " ");

            VK_NVATK_GT = calculateIndexAndSave(vs, Indexes.echar_mda_na);
            SKactive = (float)calculateIndexAndSave(vs, Indexes.mda_na) - (float)vs.calculateIndexAndSave(this, Indexes.mde_na);

            PT = vs.calculateIndexAndSave(this, Indexes.echar_mde_na);
            SKpassive = (float)calculateIndexAndSave(vs, Indexes.mda_per_na) - (float)vs.calculateIndexAndSave(this, Indexes.mde_per_na);
            HTts = (float)vs.getCurrentIndex("ring_magical_absorption") + (float)vs.getCurrentIndex("necklace_magical_absorption")+ (float)vs.getCurrentIndex("avatar_magical_absorption");;
            HTskill = (float)vs.calculateIndexAndSave(this, "magical_absorption");
            ENarmor = (float)vs.getCurrentIndex(Indexes.endura_na);
            Avatar = (float)getCurrentIndex("avatar_magical_damage_percent");
            Sach = (float)getCurrentIndex("damage_against_" + vs.character.characteristic.Class.ToString());

            Console.WriteLine(" Magical VK " + (float)getCurrentIndex("weapon_magical_damage") + " ");
            Console.WriteLine(" Magical VK2 " + (float)getCurrentIndex("weapon2_magical_damage") + " ");
            Console.WriteLine(" Magical NVATK " + (float)getCurrentIndex(Indexes.char_mda_na) + " ");
            Console.WriteLine(" Magical GT " + (float)getCurrentIndex("weapon_" + Indexes.mre_na) + " ");
            Console.WriteLine(" Magical GT2 " + (float)getCurrentIndex("weapon2_" + Indexes.mre_na) + " ");

            Console.WriteLine(" Magical VK_NVATK_GT " + VK_NVATK_GT + " ");

            Console.WriteLine(" Magical SKactive " + SKactive + " ");
            Console.WriteLine(" Magical PT " + PT + " ");
            Console.WriteLine(" Magical SKpassive " + SKpassive + " ");
            Console.WriteLine(" Magical HTts " + HTts + " ");
            Console.WriteLine(" Magical HTskill " + HTskill + " ");
            Console.WriteLine(" Magical ENarmor " + ENarmor + " ");
            Console.WriteLine(" Magical Avatar " + Avatar + " ");
            Console.WriteLine(" Magical Sach " + Sach + " ");


            float Magical_Damage = (VK_NVATK_GT + SKactive - PT) * (1 + SKpassive) * (1 - HTts) * (1 - HTskill) * (1 - ENarmor) * (1 + Avatar) * (1 + Sach);
            Magical_Damage = Math.Max(0, Magical_Damage);


            Console.WriteLine(" Magical Damage " + Magical_Damage + " ");
            //setIndex(midIndexes, "pure_damage", 0f);

            midIndexes.Add(Indexes.total_magical_damage_na, Magical_Damage);
            Magical_Damage = calculateIndexAndSave(vs, Indexes.total_magical_damage_na);
            float double_magical_damage_chance = calculateIndexAndSave(vs, Indexes.double_magical_damage_chance_na);
            bool double_magical_damage = random.NextDouble() < double_magical_damage_chance;
            Magical_Damage = double_magical_damage ? Magical_Damage * 2 : Magical_Damage;

            Console.WriteLine(" Total Magical Damage " + Magical_Damage + " double damage " + double_magical_damage);



            float Pure_Damage = (float)calculateIndexAndSave(vs, Indexes.pure_da_na);

            setIndex(midIndexes, "pure_damage", Pure_Damage);


            Console.WriteLine(" Pure Damage " + Pure_Damage + " ");

            setIndex(midIndexes, "total_damage", Physical_Damage + Magical_Damage + Pure_Damage);
            setIndex(curIndexes, "total_damage", Physical_Damage + Magical_Damage + Pure_Damage);
            Console.WriteLine("All Damage" + midIndexes["total_damage"]);

            logMyDictinary("curIndexes ", curIndexes);


            return new ArrayList() { Physical_Damage, Magical_Damage, Pure_Damage, CM > 1, double_magical_damage };
        }

        public void enableCondition(string condition, int duration = -1)
        {// sua CT chi de tinh damage roi moi tinh
            // add Condition
            conManager.enableCondition(condition, duration);

            if (enableAtomics.ContainsKey(condition))
            {
                ArrayList changedIndexes = new ArrayList();
                ArrayList atomics = enableAtomics[condition];
                foreach (AtomicEffect atomic in atomics)
                {
                    atomic.enabled = true;
                    changedIndexes.Add(atomic.index);
                }

                foreach (string index in changedIndexes)
                {
                    String formula = "";
                    ArrayList ats = this.atomics[index];
                    int count = -1;
                    for (int i = 0; i < ats.Count; i++)
                    {
                        AtomicEffect atomic = (AtomicEffect)ats[i];
                        if (atomic.isActive())
                        {
                            count++;
                            formula += count == 0 ? atomic.delta : " + " + atomic.delta;
                        }
                    }
                    Console.WriteLine("player " + playerID + " enable Condition:" + condition + " recompute " + index + "|" + deltaFormulas[index] + "|" + formula + "|");

                    // ghi lai cong thuc
                    deltaFormulas.Add(index, formula);
                }
            }
        }

        public void disableCondition(string condition)
        {
            // remove Condition
            conManager.disableCondition(condition);

            if (enableAtomics.ContainsKey(condition))
            {
                ArrayList changedIndexes = new ArrayList();
                ArrayList atomics = enableAtomics[condition];
                foreach (AtomicEffect atomic in atomics)
                {
                    atomic.enabled = false;
                    changedIndexes.Add(atomic.index);
                }

                foreach (string index in changedIndexes)
                {
                    String formula = "";
                    ArrayList ats = this.atomics[index];
                    int count = -1;
                    for (int i = 0; i < ats.Count; i++)
                    {
                        AtomicEffect atomic = (AtomicEffect)ats[i];
                        if (atomic.isActive())
                        {
                            count++;
                            formula += count == 0 ? atomic.delta : " + " + atomic.delta;
                        }
                    }
                    Console.WriteLine("player " + playerID + " disable Condition:" + condition + " recompute " + index + "|" + deltaFormulas[index] + "|" + formula + "|");

                    // ghi lai cong thuc
                    deltaFormulas.Add(index, formula);
                }
            }

        }

        public void rebuildFomulaOfIndex(string index)
        {
            if (!atomics.ContainsKey(index)) return;

            String formula = "";
            ArrayList ats = this.atomics[index];
            int count = -1;
            for (int i = 0; i < ats.Count; i++)
            {
                AtomicEffect atomic = (AtomicEffect)ats[i];
                atomic.enabled = checkCondition(atomic.condition);
                if (atomic.isActive())
                {
                    count++;
                    formula += count == 0 ? atomic.delta : " + " + atomic.delta;
                }
            }
            //Console.WriteLine("ghi lai|" + index + "|" + deltaFormulas[index]+"|"+formula+"|");

            // ghi lai cong thuc
            deltaFormulas.Add(index, formula);
            Console.WriteLine(index + " deltaFormula =" + formula);
        }


        public bool checkCondition(String condition)
        {
            return conManager.checkCondition(condition);
        }


        public void rebuildFormulaOfIndexes(NewCharacterStatus vs)
        {// reset toan bo dieu kien va tinh lai
            //reset toan bo flag cua cac atomic co dieu kien
            foreach (string index in atomics.Keys)
            {
                ArrayList ats = atomics[index];
                foreach (AtomicEffect atomic in ats)
                    if (atomic.condition != "") atomic.enabled = false;
            }

            foreach (String condition in enableAtomics.Keys)
            {
                bool result = checkCondition(condition);
                ArrayList atomics = enableAtomics[condition];
                Console.WriteLine("rebuildFormulaOfIndexes condition " + condition + " = " + result);
                foreach (AtomicEffect atomic in atomics)
                {
                    atomic.enabled = result;
                }
            }
            // tinh lai string cong thuc
            foreach (String index in atomics.Keys)
            {
                String formula = "";
                ArrayList ats = atomics[index];
                int count = -1;
                for (int i = 0; i < ats.Count; i++)
                {
                    AtomicEffect atomic = (AtomicEffect)ats[i];
                    if (atomic.isActive())
                    {
                        count++;
                        formula += count == 0 ? atomic.delta : " + " + atomic.delta;
                    }
                }

                // ghi lai cong thuc
                deltaFormulas.Add(index, formula);
            }

        }

        public void replaceEffect(string name, NewEffect effect)
        {
            MyDictionary<string, NewEffect> effects = effect.originID == playerID ? this.effects : this.op_effects;
            removeEffect(name, effects);
            effects.Add(name, effect);
        }

        public void removeEffect(string name, int castingPlayerID)
        {
            MyDictionary<string, NewEffect> effects = castingPlayerID == playerID ? this.effects : this.op_effects;
            if (!effects.ContainsKey(name)) return;
            NewEffect effect = effects[name];

            // loai bo o atomic
            Console.WriteLine(playerID + " removeEffect " + name);

            foreach (AtomicEffect atomic in effect.atomicEffects)
            {
                if (!atomics.ContainsKey(atomic.index)) continue;
                ArrayList atomicLine = atomics[atomic.index];
                atomicLine.Remove(atomic);

                if (atomic.condition != "")
                {
                    if (enableAtomics.ContainsKey(atomic.condition))
                    {
                        ArrayList enableLine = enableAtomics[atomic.condition];
                        enableLine.Remove(atomic);
                    }

                }
            }

            if (effect.condition != "")
            {
                Console.WriteLine(effect.condition + " playerID =  " + effect.playerID + " origin =  " + effect.originID + " in newCharacterStatus by " + playerID);
                if (enableSkills.ContainsKey(effect.condition))
                {
                    ArrayList enableLine = enableSkills[effect.condition];
                    enableLine.Remove(effect);
                }

            }
            // loai bo o enableAtomics
            // loai bo o enableSkills

            // loai bo chinh effect
            effects.Remove(name);

        }

        public void removeEffect(string name, MyDictionary<string, NewEffect> effects)
        {
            if (!effects.ContainsKey(name)) return;
            NewEffect effect = effects[name];

            // loai bo o atomic
            Console.WriteLine(playerID + " removeEffect " + name);

            foreach (AtomicEffect atomic in effect.atomicEffects)
            {
                if (!atomics.ContainsKey(atomic.index)) continue;
                ArrayList atomicLine = atomics[atomic.index];
                atomicLine.Remove(atomic);

                if (atomic.condition != "")
                {
                    ArrayList enableLine = enableAtomics[atomic.condition];
                    enableLine.Remove(atomic);
                }
            }

            if (effect.condition != "")
            {
                ArrayList enableLine = enableSkills[effect.condition];
                enableLine.Remove(effect);
            }
            // loai bo o enableAtomics
            // loai bo o enableSkills

            // loai bo chinh effect
            effects.Remove(name);

        }

        public void recomputeIndexesBeforeActive(NewCharacterStatus vs)
        {
            //foreach (String index in deltaFormulas.Keys)
            //{
            //   Console.WriteLine("|" + index + "|" + deltaFormulas[index]);
            //}
            // tinh lai dieu kien
            Console.WriteLine(playerID + "recomputeIndexesBeforeActive index+++++++++++++++++++++++++++++++");
            foreach (String condition in enableAtomics.Keys)
            {
                bool result = checkCondition(condition);
                ArrayList atomics = enableAtomics[condition];
                foreach (AtomicEffect atomic in atomics)
                {
                    atomic.enabled = result;
                }
            }
            // tinh lai string cong thuc

            foreach (String index in atomics.Keys)
            {
                String formula = "";
                ArrayList ats = atomics[index];
                int count = -1;
                for (int i = 0; i < ats.Count; i++)
                {
                    AtomicEffect atomic = (AtomicEffect)ats[i];
                    if (atomic.isActive())
                    {
                        count++;
                        formula += count == 0 ? atomic.delta : " + " + atomic.delta;
                    }
                }
                //Console.WriteLine("ghi lai|" + index + "|" + deltaFormulas[index]+"|"+formula+"|");

                // ghi lai cong thuc
                deltaFormulas.Add(index, formula);


                //Console.WriteLine(playerID+"recomputeIndexesBeforeActive index" + index + " " + formula);
                //Console.WriteLine("ghi lai ket qua|" + index + "|" + deltaFormulas[index] + "|" + formula+"|");


            }


            // tinh lai theo cong thuc cac chi so thuc su thay doi
            foreach (String index in deltaFormulas.Keys)
            {
                //Console.WriteLine("|" + index + "|"+ deltaFormulas[index]);
                String formula = index + (deltaFormulas[index] != "" ? " + " + deltaFormulas[index] : ""); // luong truoc deltaFormulas la rong
                //Console.WriteLine(formula);
                float result = calcuateExpression(formula, vs);
                curIndexes.Add(index, result);
            }

            Console.WriteLine(playerID + "recomputeIndexesBeforeActive index+++++++++++++++++++++++++++++++");

        }

        private bool isOperator(string op)
        {
            switch (op)
            {
                case "+":

                case "-":


                case "*":
                case "/":
                    return true;
            }
            return false;
        }

        private int getPrecedence(string operators)
        {
            switch (operators)
            {
                case "+":
                    return 1;
                case "-":
                    return 2;

                case "*":
                case "/":
                    return 3;
            }
            return 0;
        }

        private bool isHigherOrEqual(string op1, string op2)
        {
            return getPrecedence(op1) >= getPrecedence(op2);
        }

        public float calculateIndexAndSave(NewCharacterStatus def, string index)
        {
            string fomulas = index + (deltaFormulas.ContainsKey(index) && deltaFormulas[index] != "" ? " + " + deltaFormulas[index] : "");
            Console.WriteLine("calculateIndex " + index + " formulas = " + fomulas); // de log Ishappen Stun
            try
            {
                float digit = Convert.ToSingle(fomulas);
                Console.WriteLine("save value = " + digit);
                curIndexes.Add(index, digit);
                return digit;
            }
            catch
            {
                // khong phai la digit
                float digit = calcuateExpression(fomulas, def);
                Console.WriteLine("save value = " + digit);
                curIndexes.Add(index, digit);
                return digit;
            }
        }

        public float calculateIndex(NewCharacterStatus def, string index)
        {
            string fomulas = index + (deltaFormulas.ContainsKey(index) && deltaFormulas[index] != "" ? " + " + deltaFormulas[index] : "");
            if (!index.Contains("aps"))
                Console.WriteLine("calculateIndex " + index + " formulas = " + fomulas); // de log Ishappen Stun
            try
            {
                float digit = Convert.ToSingle(fomulas);
                if (!index.Contains("aps"))
                    Console.WriteLine("value = " + digit);
                return digit;
            }
            catch
            {
                // khong phai la digit
                float digit = calcuateExpression(fomulas, def);
                if (!index.Contains("aps"))
                    Console.WriteLine("value = " + digit);
                return digit;
            }
        }

        public float calcuateExpression(string formulas, NewCharacterStatus vs, MyDictionary<string, string> replaceDict = null)
        {
            // chia chuoi thanh cac 
            string[] elements = formulas.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            ArrayList opstack = new ArrayList();
            ArrayList postfixEx = new ArrayList();
            for (int i = 0; i < elements.Length; i++)
            {
                string e = elements[i];
                switch (e)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
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

                        float operand = getOperand(e, vs, replaceDict);
                        postfixEx.Add(operand);
                        break;
                }
            }

            while (opstack.Count > 0)
            {
                postfixEx.Add(opstack[opstack.Count - 1]);
                opstack.RemoveAt(opstack.Count - 1);
            }

            //for (int i = 0; i < postfixEx.Count; i++) 
            //    Console.Write(postfixEx[i]+" ");
            //Console.WriteLine();
            // tinh
            for (int i = 0; i < postfixEx.Count; i++)
            {
                if (postfixEx[i] is float)
                {
                    opstack.Add(postfixEx[i]);
                }
                else
                {
                    float y = (float)opstack[opstack.Count - 1];
                    opstack.RemoveAt(opstack.Count - 1);
                    float x = (float)opstack[opstack.Count - 1];
                    opstack.RemoveAt(opstack.Count - 1);

                    switch ((string)postfixEx[i])
                    {
                        case "+":
                            opstack.Add(x + y);
                            break;
                        case "-":
                            opstack.Add(x - y);
                            break;
                        case "*":
                            opstack.Add(x * y);
                            break;
                        case "/":
                            opstack.Add(x / y);
                            break;
                        default:
                            Console.WriteLine("toan tu khong biet " + postfixEx[i]);
                            break;
                    }
                }
            }

            return opstack.Count > 0 ? (float)opstack[opstack.Count - 1] : 0f;
        }

        private float getOperand(string e, NewCharacterStatus vs, MyDictionary<string, string> replaceDict = null)
        {// loai bo cac truong hop la bieu thuc (
            try
            {
                float digit = Convert.ToSingle(e);
                return digit;
            }
            catch (Exception ex)
            {
                string[] parts = e.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts[0] == "e")// chi so cua enemy
                {
                    String index = "";
                    for (int i = 1; i < parts.Length; i++)
                        index += i == 1 ? parts[i] : "_" + parts[i];
                    index = (replaceDict == null || !replaceDict.ContainsKey(index)) ? index : replaceDict[index];

                    float digit = (float)vs.getMidIndex(index);
                    return digit;

                }
                else
                {
                    e = (replaceDict == null || !replaceDict.ContainsKey(e)) ? e : replaceDict[e];
                    float digit = (float)getMidIndex(e);
                    return digit;
                }
            }

        }

        public void decreaseIndex(string index)
        {
            if (atomics.ContainsKey(index))
            {
                ArrayList ats = atomics[index];
                foreach (AtomicEffect atomic in ats)
                {
                    if (conManager.checkCondition(atomic.condition))
                    {
                        atomic.decDuration();
                        Console.WriteLine("decDuration " + atomic.index + " " + atomic.delta + " " + atomic.condition + " " + atomic.duration);
                    }

                }


                //midIndexes.Remove(index);
                //initIndexes.Remove(index);
                //curIndexes.Remove(index);
                //deltaFormulas.Remove(index);
            }
        }

        //public void recomputeChanged()


        public void setIndex(MyDictionary<string, object> dict, string field, object values)
        {
            dict.Add(field, values);
        }

        public object getIndex(MyDictionary<string, object> dict, string field, object defaultValue)
        {
            if (dict.ContainsKey(field))
            {
                return dict[field];
            }
            return defaultValue;
        }

        public object getCurrentIndex(string index)
        {// co the khong co se tinh
            if (!curIndexes.ContainsKey(index))
            {
                //Console.WriteLine("Not found Index " + index + " in curIndexes");
                return getMidIndex(index);
                //throw new Exception("Not found Index " + index);
            }
            return curIndexes.ContainsKey(index) ? curIndexes[index] : null;
        }

        public float setHP(float hp)
        {
            float maxHp = (float)getCurrentIndex(Indexes.max_hp_na);
            float hp_fix = Math.Min(hp, maxHp);
            setIndex(midIndexes, Indexes.hp_na, hp_fix);
            setIndex(curIndexes, Indexes.hp_na, hp_fix);
            return hp_fix;
        }

        public object getMidIndex(string index)
        {
            if (!midIndexes.ContainsKey(index))
            {
                //Console.WriteLine("Not found Index " + index + " in midIndexes");
                return 0f;
            }
            return midIndexes.ContainsKey(index) ? midIndexes[index] : null;
        }

        public object getInitIndex(string index)
        {
            if (!initIndexes.ContainsKey(index)) throw new Exception("Not found Index " + index + " in initIndex");
            return initIndexes.ContainsKey(index) ? initIndexes[index] : null;
        }

        public ArrayList updateTurn()
        {

            return null;
        }

        private void logMyDictinary(string tag, MyDictionary<string, object> dic)
        {
            //        Console.WriteLine("+++++++++++++++++++++++++++++++++++++++begin "+tag);
            //foreach (string index in dic.Keys)
            //{
            //            Console.WriteLine(tag+" : " + index + " = " + dic[index]);
            //}
            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++end "+tag);
        }

        public byte[] convertToByteArrOfPlayerID(int playerID)
        {
            byte[] bytes = null;
            foreach (string key in effects.Keys)
            {
                NewEffect effect = effects[key];
                if (effect.playerID == playerID)
                    bytes = bytes == null ? effect.convertToByteArr() : Utilities.Concat<byte>(bytes, effect.convertToByteArr());

            }
            return bytes;
        }

        public ArrayList getEffectsOfPlayerID(int playerID)
        {
            ArrayList list = new ArrayList();
            foreach (string key in effects.Keys)
            {
                NewEffect effect = effects[key];
                if (effect.playerID == playerID)
                    list.Add(effect);

            }
            return list;
        }

        public ArrayList getOp_EffectsOfPlayerID(int playerID)
        {
            ArrayList list = new ArrayList();
            foreach (string key in op_effects.Keys)
            {
                NewEffect effect = op_effects[key];
                if (effect.playerID == playerID)
                    list.Add(effect);

            }
            return list;
        }

        public bool canAttack()
        {
            //Immobilization + Melee
            //Blind + Ranger;
            bool immobilization = effects.ContainsKey(NewEffect.Immobilization);
            bool blind = effects.ContainsKey(NewEffect.Blind);
            return !((immobilization && character.characteristic.Type == Characteristic.CharacterType.MELE) ||
                (blind && character.characteristic.Type == Characteristic.CharacterType.RANGER));
        }

        public bool haveAbnomarlStatusByName(string name)
        {
            return op_effects.ContainsKey(name);
        }

        public void clear()
        {
            effects.Clear();
            op_effects.Clear();
            atomics.Clear();
            deltaFormulas.Clear();
            enableSkills.Clear();
            enableAtomics.Clear();
        }

    }

}