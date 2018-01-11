
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    class Constants
    {
        // server indexes
        public const string MIN_PDA_ID = "1";
        public const string MAX_PDA_ID = "2";
        public const string MIN_MDA_ID = "3";
        public const string MAX_MDA_ID = "4";
        public const string CRI_CHA_ID = "5";
        public const string MCA_CHA_ID = "6";
        public const string MIN_PRE_ID = "7";
        public const string MAX_PRE_ID = "8";
        public const string MIN_MRE_ID = "9";
        public const string MAX_MRE_ID = "10";
        public const string MIN_ATR_ID = "11";
        public const string MAX_ATR_ID = "12";
        public const string MIN_BLC_ID = "13";
        public const string MAX_BLC_ID = "14";

        public const string MIN_PRD_ID = "15";
        public const string MAX_PRD_ID = "16";
        public const string MIN_MRD_ID = "17";
        public const string MAX_MRD_ID = "18";
        public const string MIN_PAB_ID = "19";
        public const string MAX_PAB_ID = "20";
        public const string MIN_MAB_ID = "21";
        public const string MAX_MAB_ID = "22";

        public const string MIN_PDE_ID = "23";
        public const string MAX_PDE_ID = "24";
        public const string MIN_MDE_ID = "25";
        public const string MAX_MDE_ID = "26";

        public const string SUB_PDA_ID = "27";
        public const string SUB_MDA_ID = "28";
        public const string SUB_PAB_ID = "29";
        public const string SUB_MAB_ID = "30";

        public const string MIN_PRYR_ID = "31";
        public const string MAX_PRYR_ID = "32";
        public const string HP_ID = "33";
        public const string STR_ID = "35";
        public const string INT_ID = "36";
        public const string VIT_ID = "37";
        public const string SUB_CRID_ID = "38";
        public const string BRSHIE_ID = "39";
        public const string BLESS_ID = "40";
        public const string PROTEC_ID = "41";
        public const string ALL_DRD_ID = "42";
        public const string BRWEAP_ID = "43";
        public const string DODGE_CHA_ID = "44";
        public const string FRZ_DURE_ID = "46";
        public const string POI_DURE_ID = "47";
        public const string ELE_DURE_ID = "48";
        public const string BUR_DURE_ID = "49";

        public const string ICR_GOLD_ID = "50";
        public const string ICR_EXP_ID = "51";
        public const string ICR_SKP_ID = "52";
        public const string LUCK_ID = "53";
        public const string PURE_DA_ID = "54";
        public const string STUN_RES_ID = "55";
        public const string HYP_RES_ID = "56";
        public const string IMP_RES_ID = "57";
        public const string ROT_RES_ID = "58";
        public const string PAIN_RES_ID = "59";
        public const string BLEED_RES_ID = "60";
        public const string CRAZY_RES_ID = "61";
        public const string THIRST_RES_ID = "62";

        public const string DAM_ASSASSIN_ID = "63";
        public const string DAM_PALADIN_ID = "64";
        public const string DAM_ZEALOT_ID = "65";
        public const string DAM_SORCERESS_ID = "66";
        public const string DAM_WIZARD_ID = "67";
        public const string DAM_MARKSMAN_ID = "68";
        public const string DAM_ORG_ID = "69";
        public const string DAM_BARBARIAN_ID = "70";

        public const string MIN_PDA_NA = "Min Physical damage";
        public const string MAX_PDA_NA = "Max Physical damage";
        public const string MIN_MDA_NA = "Min Magical damage";
        public const string MAX_MDA_NA = "Max Magical damage";
        public const string CRI_CHA_NA = "Critical chance";
        public const string MCA_CHA_NA = "Multicast chance";
        public const string MIN_PRE_NA = "Min Physical reinforce";
        public const string MAX_PRE_NA = "Max Physical reinforce";
        public const string MIN_MRE_NA = "Min Magical reinforce";
        public const string MAX_MRE_NA = "Max Magical reinforce";
        public const string MIN_ATR_NA = "Min Attack rate";
        public const string MAX_ATR_NA = "Max Attack rate";
        public const string MIN_BLC_NA = "Min Block chance";
        public const string MAX_BLC_NA = "Max Block chance";

        public const string MIN_PRD_NA = "Min Physical reductions";
        public const string MAX_PRD_NA = "Max Physical reductions";
        public const string MIN_MRD_NA = "Min Magical reduction";
        public const string MAX_MRD_NA = "Max Magical reduction";
        public const string MIN_PAB_NA = "Min Physical absorption";
        public const string MAX_PAB_NA = "Max Physical absorption";
        public const string MIN_MAB_NA = "Min Magical absorption";
        public const string MAX_MAB_NA = "Max Magical absorption";

        public const string MIN_PDE_NA = "Min Physical Def";
        public const string MAX_PDE_NA = "Max Physical Def";
        public const string MIN_MDE_NA = "Min Magical Def";
        public const string MAX_MDE_NA = "Max Magical Def";

        public const string SUB_PDA_NA = "Physical damage";
        public const string SUB_MDA_NA = "Magical damage";
        public const string SUB_PAB_NA = "Physical absorb";
        public const string SUB_MAB_NA = "Magical absorb";

        public const string MIN_PRYR_NA = "Min Parry rate";
        public const string MAX_PRYR_NA = "Max Parry rate";
        public const string HP_NA = "HP";
        public const string STR_NA = "Strength";
        public const string INT_NA = "Intelligence";
        public const string VIT_NA = "Vitality";
        public const string SUB_CRID_NA = "Critical damage";
        public const string BRSHIE_NA = "Breaks Shield";
        public const string BLESS_NA = "Blessing";
        public const string PROTEC_NA = "Protection";
        public const string ALL_DRD_NA = "All damage reduces";
        public const string BRWEAP_NA = "Breaks Weapon";
        public const string DODGE_CHA_NA = "Dodge chance";
        public const string FRZ_DURE_NA = "Freezing Duration Reduced";
        public const string POI_DURE_NA = "Poisoning Duration Reduced";
        public const string ELE_DURE_NA = "Electric shock Duration Reduced";
        public const string BUR_DURE_NA = "Burn Duration Reduced";

        public const string ICR_GOLD_NA = "Increase gold";
        public const string ICR_EXP_NA = "Increase exp";
        public const string ICR_SKP_NA = "Increase skill points";
        public const string LUCK_NA = "Luck";
        public const string PURE_DA_NA = "Pure damage";
        public const string STUN_RES_NA = "Stun resistance";
        public const string HYP_RES_NA = "Hypnotic resistance";
        public const string IMP_RES_NA = "Impotent resistance";
        public const string ROT_RES_NA = "Rot resistance";
        public const string PAIN_RES_NA = "Pain resistance";
        public const string BLEED_RES_NA = "Bleed resistance";
        public const string CRAZY_RES_NA = "Crazy resistance";
        public const string THIRST_RES_NA = "Blood Thirst resistance";

        public const string DAM_ASSASSIN_NA = "damage against Assassin";
        public const string DAM_PALADIN_NA = "damage against Paladin";
        public const string DAM_ZEALOT_NA = "damage against Zealot";
        public const string DAM_SORCERESS_NA = "damage against Sorceress";
        public const string DAM_WIZARD_NA = "damage against Wizard";
        public const string DAM_MARKSMAN_NA = "damage against Marksman";
        public const string DAM_ORG_NA = "damage against Org";
        public const string DAM_BARBARIAN_NA = "damage against Barbarian";



        //EXP,
        //GOLD,
        //EVENTPOINT,
        //DIAMOND,

        //HEALTH,
        //STRENGTH,
        //INTELLIGENCE,
        //DEXTERITY,
        //FOCUS,
        //VITALITY,
        //AGILITY,
        //LUCK,
        //ENDURANCE,
        //BLESSING,
        //PROTECTION,

        //ACCURACY,

        //MIN_PHYS_DAMAGE,
        //MAX_PHYS_DAMAGE,
        //MIN_MAGICAL_DAMAGE,
        //MAX_MAGICAL_DAMAGE,

        //CRITICAL_CHANCE,
        //MULTICAST_CHANCE,

        //MIN_PHYS_REINFORCE,
        //MAX_PHYS_REINFORCE,
        //MIN_MAGICAL_REINFORCE,
        //MAX_MAGICAL_REINFORCE,

        //MIN_ATTACK_RATE,
        //MAX_ATTACK_RATE,

        //MIN_PARRY_RATE,
        //MAX_PARRY_RATE,

        //MIN_BLOCK_CHANCE,
        //MAX_BLOCK_CHANCE,

        //BREAKS_SHIELD,
        //BREAKS_WEAPON,

        //MIN_PHYS_REDUCTION,
        //MAX_PHYS_REDUCTION,
        //MIN_MAGICAL_REDUCTION,
        //MAX_MAGICAL_REDUCTION,

        //MIN_PHYS_ABSORPTION,
        //MAX_PHYS_ABSORPTION,
        //MIN_MAGICAL_ABSORPTION,
        //MAX_MAGICAL_ABSORPTION,

        //MIN_PHYS_DEF,
        //MAX_PHYS_DEF,
        //MIN_MAGICAL_DEF,
        //MAX_MAGICAL_DEF,

        //FREEZING_STATUS_CHANCE,
        //POSIONING_STATUS_CHANCE,
        //ELECTRIC_SHOCK_STATUS_CHANCE,
        //BURN_STATUS_CHANCE,
        //KNOCKBACK_STATUS_CHANCE,
        //IMMOBILIZATION_STATUS_CHANCE,
        //BLIND_STATUS_CHANCE,
        //DEMENTIA_STATUS_CHANCE,
        //DISEASE_STATUS_CHANCE,
        //FEAR_STATUS_CHANCE,
        //SLEEP_STATUS_CHANCE,
        //GLAMOUR_STATUS_CHANCE,
        //PAIN_STATUS_CHANCE,
        //CRAZY_STATUS_CHANCE,
        //ROT_STATUS_CHANCE,
        //HYPNOTIC_STATUS_CHANCE,
        //BLEED_STATUS_CHANCE,
        //BLOOD_THIRST_STATUS_CHANCE,
        //IMPOTENT_STATUS_CHANCE,
        //STUN_STATUS_CHANCE,

        //FREEZING_DURATION_REDUCED,
        //POSIONING_DURATION_REDUCED,
        //ELECTRIC_SHOCK_DUTATION_REDUCED,
        //BURN_DURATION_REDUCED,
        //KNOCKBACK_RESISTANCE,
        //IMMOBILIZATION_RESISTANCE,
        //BLIND_RESISTANCE,
        //DEMENTIA_RESISTANCE,
        //DISEASE_RESISTANCE,
        //FEAR_RESISTANCE,
        //SLEEP_RESISTANCE,
        //GLAMOUR_RESISTANCE,
        //PAIN_RESISTANCE,
        //CRAZY_RESISTANCE,
        //ROT_RESISTANCE,
        //HYPNOTIC_RESISTANCE,
        //BLEED_RESISTANCE,
        //BLOOD_THIRST_RESISTANCE,
        //IMPOTENT_RESISTANCE,
        //STUN_RESISTANCE,

        //INCREASE_GOLD,
        //INCREASE_EXP,
        //INCREASE_SKILLPOINT,

        //PURE_DAMAGE,
        //DAMAGE_AGAIN_ASSASSIN,
        //DAMAGE_AGAIN_PALADIN,
        //DAMAGE_AGAIN_ZEALOT,
        //DAMAGE_AGAIN_SORCERESS,
        //DAMAGE_AGAIN_WIZARD,
        //DAMAGE_AGAIN_MARKSMAN,
        //DAMAGE_AGAIN_ORG,
        //DAMAGE_AGAIN_BARBARIAN,

        //RATE,

        // client lib indexes
        public static string Max_Health = "Max Health";
        public static string Type = "Type"; // 
        public static string Class = "Class";
        public static string Level = PROPERTYTYPE.LEVEL.ToString();
        public static string Health = PROPERTYTYPE.HEALTH.ToString();
        public static string Strength = PROPERTYTYPE.STRENGTH.ToString();
        public static string Intelligence = PROPERTYTYPE.INTELLIGENCE.ToString();
        public static string Dexterity = PROPERTYTYPE.DEXTERITY.ToString();
        public static string Focus = PROPERTYTYPE.FOCUS.ToString();
        public static string Vitality = PROPERTYTYPE.VITALITY.ToString();
        public static string Agility = PROPERTYTYPE.AGILITY.ToString(); // Agility
        public static string Luck = PROPERTYTYPE.LUCK.ToString();
        public static string Endurance = PROPERTYTYPE.ENDURANCE.ToString();
        public static string Blessing = PROPERTYTYPE.BLESSING.ToString();
        public static string Protection = PROPERTYTYPE.PROTECTION.ToString();
        public static string PhysicalDamage = PROPERTYTYPE._PHYS_DAMAGE.ToString();
        public static string PhysicalReinforce = PROPERTYTYPE._PHYS_REINFORCE.ToString();
        public static string PhysicalDefense = PROPERTYTYPE._PHYS_DEF.ToString();
        public static string MagicalDamage = PROPERTYTYPE._MAGICAL_DAMAGE.ToString();
        public static string MagicalReinforce = PROPERTYTYPE._MAGICAL_REINFORCE.ToString();
        public static string MagicalDefense = PROPERTYTYPE._MAGICAL_DEF.ToString();
        public static string CriticalChance = PROPERTYTYPE.CRITICAL_CHANCE.ToString();
        public static string CriticalDamage = "CriticalDamage";
        public static string MulticastChance = PROPERTYTYPE.MULTICAST_CHANCE.ToString();
        public static string BlockingChance = PROPERTYTYPE._BLOCK_CHANCE.ToString();
        public static string DodgeChance = "DodgeChance";
        public static string HealthRecovery = "HealthRecovery";
        public static string Reputation = "Reputation";
        public static string AttackRate = PROPERTYTYPE._ATTACK_RATE.ToString();
        public static string ParryRate = PROPERTYTYPE._PARRY_RATE.ToString();
        public static string PhysicalAbsorbtion = PROPERTYTYPE._PHYS_ABSORPTION.ToString();
        public static string MagicalAbsorbtion = PROPERTYTYPE._MAGICAL_ABSORPTION.ToString();
        public static string PhysicalReduction = PROPERTYTYPE._PHYS_REDUCTION.ToString();
        public static string MagicalReduction = PROPERTYTYPE._MAGICAL_REDUCTION.ToString();
        public static string AbsoluteDamage = PROPERTYTYPE.PURE_DAMAGE.ToString();

        //public const PROPERTYTYPE Max_Health = "Max Health";
        //public const PROPERTYTYPE Type = "Type"; // 
        //public const PROPERTYTYPE Class = "Class";
        //public const PROPERTYTYPE Level = "Level";
        //public const PROPERTYTYPE Health = "Health";
        //public const PROPERTYTYPE Strength = "Strength";
        //public const PROPERTYTYPE Intelligence = "Intelligence";
        //public const PROPERTYTYPE Dexterity = "Dexterity";
        //public const PROPERTYTYPE Focus = "Focus";
        //public const PROPERTYTYPE Vitality = "Vitality";
        //public const PROPERTYTYPE Agility = "Agility"; // Agility
        //public const PROPERTYTYPE Luck = "Luck";
        //public const PROPERTYTYPE Endurance = "Endurance";
        //public const PROPERTYTYPE Blessing = "Blessing";
        //public const PROPERTYTYPE Protection = "Protection";
        //public const PROPERTYTYPE PhysicalDamage = "PhysicalDamage";
        //public const PROPERTYTYPE PhysicalReinforce = "PhysicalReinforce";
        //public const PROPERTYTYPE PhysicalDefense = "PhysicalDefense";
        //public const PROPERTYTYPE MagicalDamage = "MagicalDamage";
        //public const PROPERTYTYPE MagicalReinforce = "MagicalReinforce";
        //public const PROPERTYTYPE MagicalDefense = "MagicalDefense";
        //public const PROPERTYTYPE CriticalChance = "CriticalChance";
        //public const PROPERTYTYPE CriticalDamage = "CriticalDamage";
        //public const PROPERTYTYPE MulticastChance = "MulticastChance";
        //public const PROPERTYTYPE BlockingChance = "BlockingChance";
        //public const PROPERTYTYPE DodgeChance = "DodgeChance";
        //public const PROPERTYTYPE HealthRecovery = "HealthRecovery";
        //public const PROPERTYTYPE Reputation = "Reputation";
        //public const PROPERTYTYPE AttackRate = "AttackRate";
        //public const PROPERTYTYPE ParryRate = "ParryRate";
        //public const PROPERTYTYPE PhysicalAbsorbtion = "PhysicalAbsorbtion";
        //public const PROPERTYTYPE MagicalAbsorbtion = "MagicalAbsorbtion";
        //public const PROPERTYTYPE PhysicalReduction = "PhysicalReduction";
        //public const PROPERTYTYPE MagicalReduction = "MagicalReduction";
        //public const PROPERTYTYPE AbsoluteDamage = "AbsoluteDamage";

        public static int[] HP_PER_TIMES = {
            0,
            15,
            17,
            18,
            20,
            21,
            23,
            24,
            26,
            27,
            29,
            30,
            32,
            33,
            35,
            36,
            38,
            39,
            41,
            42,
            44,
            45,
            47,
            48,
            50,
            51,
            53,
            54,
            56,
            57,
            59,
            60,
            62,
            63,
            65,
            66,
            68,
            69,
            71,
            72,
            74,
            75,
            77,
            78,
            80,
            81,
            83,
            84,
            86,
            87,
            89,
            90,
            92,
            93,
            95,
            96,
            98,
            99,
            101,
            102,
            104,
            105,
            107,
            108,
            110,
            111,
            113,
            114,
            116,
            117,
            119,
            120,
            122,
            123,
            125,
            126,
            128,
            129,
            131,
            132,
            134,
            135,
            137,
            138,
            140,
            141,
            143,
            144,
            146,
            147,
            149,
            150,
            152,
            153,
            155,
            156,
            158,
            159,
            161,
            162,
            164
        };

        public static int[] DAMAGE_PER_TIMES = {
            0,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            14
        };

        public static int[] DEFEND_PER_TIMES = {
            0,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            9,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            12,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            14
        };

        public static int[] RECOVERY_PER_TIMES = {
            0,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            3,
            3,
            3,
            3,
            3,
            3,
            4,
            4,
            4,
            4,
            4,
            4,
            4,
            5,
            5,
            5,
            5,
            5,
            5,
            5,
            6,
            6,
            6,
            6,
            6,
            6,
            7,
            7,
            7,
            7,
            7,
            7,
            7,
            8,
            8,
            8,
            8,
            8,
            8,
            8,
            9,
            9,
            9,
            9,
            9,
            9,
            10,
            10,
            10,
            10,
            10,
            10,
            10,
            11,
            11,
            11,
            11,
            11,
            11,
            11,
            12,
            12,
            12,
            12,
            12,
            12,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            14,
            14,
            14,
            14,
            14,
            14,
            14,
            15,
            15,
            15,
            15,
            15,
            15,
            16,
            16,
            16,
            16,
            16,
            16,
            16
        };

        private static Dictionary<string, PROPERTYTYPE> PROPERTYTYPE_Dict = new Dictionary<string, PROPERTYTYPE>();
        private static Dictionary<string, TypeEquipmentCharacter> TypeEquipmentCharacter_Dict = new Dictionary<string, TypeEquipmentCharacter>();
        private static Dictionary<string, ClassCharacterItem> ClassCharacterItem_Dict = new Dictionary<string, ClassCharacterItem>();
        private static Dictionary<string, RarelyItem> RarelyItem_Dict = new Dictionary<string, RarelyItem>();
        private static bool initFlag = false;

        public static void init() {
            Debug.Log("initFlag" + initFlag);
            if (initFlag) return;
            initFlag = true;
            for (int i = 1; i < HP_PER_TIMES.Length; i++) {
                HP_PER_TIMES[i] += HP_PER_TIMES[i - 1];
                DAMAGE_PER_TIMES[i] += DAMAGE_PER_TIMES[i - 1];
                DEFEND_PER_TIMES[i] += DEFEND_PER_TIMES[i - 1];
                RECOVERY_PER_TIMES[i] += RECOVERY_PER_TIMES[i - 1];
            }

            //foreach (PROPERTYTYPE pro in Enum.GetValues(typeof(PROPERTYTYPE)))
            //{
            //    PROPERTYTYPE_Dict.Add(pro.ToString(), pro);
            //}

            //foreach (TypeEquipmentCharacter pro in Enum.GetValues(typeof(TypeEquipmentCharacter)))
            //{
            //    TypeEquipmentCharacter_Dict.Add(pro.ToString(), pro);
            //}

            //foreach (ClassCharacterItem pro in Enum.GetValues(typeof(ClassCharacterItem)))
            //{
            //    ClassCharacterItem_Dict.Add(pro.ToString(), pro);
            //}

            //foreach (RarelyItem pro in Enum.GetValues(typeof(RarelyItem)))
            //{
            //    RarelyItem_Dict.Add(pro.ToString(), pro);
            //}
            //UnityEngine.Debug.Log("init done");
        }

        public static PROPERTYTYPE getPROPERTYTYPE(string field)
        {
            return (PROPERTYTYPE_Dict.ContainsKey(field) ? PROPERTYTYPE_Dict[field] : PROPERTYTYPE.NONE);
        }

        public static TypeEquipmentCharacter getTypeEquipmentCharacter(string filed)
        {
            return (TypeEquipmentCharacter_Dict.ContainsKey(filed) ? TypeEquipmentCharacter_Dict[filed] : TypeEquipmentCharacter.None);
        }

        public static ClassCharacterItem getClassCharacterItem(string filed)
        {
            return (ClassCharacterItem_Dict.ContainsKey(filed) ? ClassCharacterItem_Dict[filed] : ClassCharacterItem.None);
        }

        public static RarelyItem getRarelyItem(string filed)
        {
            return (RarelyItem_Dict.ContainsKey(filed) ? RarelyItem_Dict[filed] : RarelyItem.Common);
        }
    }


    enum ServerIndexes
    {
        min_physical_damage = 1,  //Dame vật lý Min Max
        max_physical_damage = 2,
        min_magical_damage = 3, //Dame phép Min    Max
        max_magical_damage = 4,


        critical_chance = 5,
        multicast_chance = 6,

        min_physical_reinforce = 7, //Gia tăng vật lý   Min    Max 
        max_physical_reinforce = 8, 
        min_magical_reinforce = 9, //Gia tăng phép Min Max
        max_magical_reinforce = 10,

        min_attack_rate = 11, //Tỉ lệ tấn công  Min    Max - Số
        max_attack_rate = 12,
                
        min_parry_rate = 13, //Tỉ lệ do~    Min    Max  - Số
        max_parry_rate = 14,
            
        min_block_chance = 15, 
        max_block_chance = 16,
                
        min_physical_defense = 17, //Def vật lý        Min Max 
        max_physical_defense = 18,
        min_magical_defense = 19, //Def phép    Min    Max
        max_magical_defense = 20, 
                
        min_physical_reduction = 21, //Reduction vật lý       Min Max 
        max_physical_reduction = 22, 
        min_magical_reduction = 23,  //Reduction phép   Min    Max
        max_magical_reduction = 24,
                
        min_physical_absorption = 25, //absorption vật lý      Min Max 
        max_physical_absorption = 26,
        min_magical_absorption = 27, //Reduction phép   Min    Max
        max_magical_absorption = 28, 
                            

                //các dòng xanh + thêm cho đồ
                
        strength = 29, 
        intelligence = 30, 
        vitality = 31, // Tăng lượng Health của nhân vật và tăng tốc độ hồi máu của nhân vật (1% Health mỗi turn)
        dexterity = 32, // 1 Dexterity tăng Critical damage vật lý thêm 1%
        focus = 33, // 1 Focus tăng 1% khả năng đánh thường thêm 1 lần nữa đối với vũ khí phép
        agility = 34, // 1 Agility tăng tỉ lệ Dodge chance (đòn đánh của vũ khí thuần phép)1%
        luck = 35, //Tăng tỉ lệ nhặt đồ hiếm
        endurance = 36, // Quy định bởi Endurance, 1 Endurance giảm 1% 'All damage reduces' phải nhận
        blessing = 37, // Tăng tỉ lệ cường hóa cấp độ đồ, Blessing tăng 5% tỉ lệ thành công, độ lớn của Blessing là số lần dùng của nó. Ví dụ Blessing + 5 time(s) thì trong 5 lần cường hóa liên tiếp mỗi lần thêm 5% tỉ lệ thành công
        protection = 38, // Loại trừ khả năng bị Reset về 0 hoặc về 5 nếu cường hóa cấp độ thất bại, trừ 1 cấp độ thay vì về 0 hoặc 5. Ví dụ Proctection + 5 time(s) thì trong 5 lần cường hóa thất bại liên tiếp, thay vì bị reset về 0 hoặc 5 thì nó chỉ giảm 1 cấp độ           

        maxhealth = 39,               
        breaks_shield = 40,// Giảm khả năng block của Khiên, 1 Breaks Shield giảm 1% khả năng block của Khiên đối phương (nếu có)
        accuracy = 41, //Giảm khả năng né tránh đòn phép của đồi phương (nếu có) , 1 Accuracy giảm 1% khả năng né tránh của đối phương đối với đòn của vũ khí phép
        breaks_weapon = 42,// Giảm khả năng critical của vũ khí, 1 Breaks Weapon giảm 1% khả năng critical của vũ khí đối phương 
        freezing_chance = 43, 
        poisoning_chance = 44, 
        electric_shock_chance = 45, 
        burn_chance = 46, 
                
                //Các dị trạng có do skill 
                
        knockback_chance = 47, 
        immobilization_chance = 48, 
        blind_chance = 49,
        frostbite_chance = 50, 
        disease_chance = 51, 
        fear_chance = 52, 
        sleep_chance = 53, 
        glamour_chance = 54, 
        pain_chance = 55, 
        crazy_chance = 56, 
        rot_chance = 57, 
        hypnotic_chance, 
        bleed_chance = 59, 
        dull_chance = 60, 
        impotent_chance = 61, 
        stun_chance = 62, 
            
                //Các chống dị trạng có do skill 
                
        freezing_duration_reduce = 63, // de dang static
        poisoning_duration_reduce = 64, 
        electric_shock_duration_reduce = 65,
        burn_duration_reduce = 66,
        knockback_resistance = 67,
        immobilization_resistance = 68,
        blind_resistance = 69,
        frostbite_resistance = 70,
        disease_resistance = 71,
        fear_resistance = 72,
        sleep_resistance = 73,
        glamour_resistance = 74,
        pain_resistance = 75, 
        crazy_resistance = 76, 
        rot_resistance = 77,
        hypnotic_resistance = 78,
        bleed_resistance = 79, 
        dull_resistance = 80,                          
        impotent_resistance = 81, 
        stun_resistance = 82,  

                //Các dòng xanh cộng thêm của Avatar
                                                    
                increase_gold = 83, 
                increase_exp = 84,
                increase_skill_points = 85,
                //'86' => 'Luck', //Tăng khả năng tìm đồ hiếm
                //'87' => 'Blessing, //Tăng khả năng thành công khi cường hóa chỉ số              
                pure_damage = 88, //Khả năng đánh ra Sát thương tuyệt đối xuyên giáp
                
                //Các dòng cộng thêm cho Sách
        damage_against_Assassin = 89,// de dang static
        damage_against_Paladin = 90,// de dang static
        damage_against_Cleric = 91, // de dang static
        damage_against_Sorceress = 92,// de dang static
        damage_against_Wizard = 93, // de dang static
        damage_against_Marksman = 94,// de dang static
        damage_against_Org = 95, // de dang static
        damage_against_Barbarian = 96,// dde dang static
                
                // Các dòng trắng cho sẵn của Avatar
                //'97' => 'Health', // co roi ma 39
        maxhealth_percent = 97,// de dang static
        physical_damage_percent = 98,// de dang dynamic co ten
        magical_damage_percent = 99,// de dang dynamic co ten
        physical_absorption = 100,// de dang dynamic co ten
        magical_absorption = 101,// de dang dynamic co ten
    }
}

