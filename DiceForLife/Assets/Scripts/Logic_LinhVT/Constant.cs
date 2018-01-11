
using System.Collections.Generic;
using CoreLib;
///_<summary>
///_Vinh_write
///_</summary>
public enum PROPERTYTYPE
{
    NONE,
    LEVEL,
    EXP,
    GOLD,
    EVENTPOINT,
    DIAMOND,

    HEALTH,
    STRENGTH,
    INTELLIGENCE,
    DEXTERITY,
    FOCUS,
    VITALITY,
    AGILITY,
    LUCK,
    ENDURANCE,
    BLESSING,
    PROTECTION,

    ACCURACY,

    MIN_PHYS_DAMAGE,
    MAX_PHYS_DAMAGE,
    MIN_MAGICAL_DAMAGE,
    MAX_MAGICAL_DAMAGE,

    CRITICAL_CHANCE,
    MULTICAST_CHANCE,

    MIN_PHYS_REINFORCE,
    MAX_PHYS_REINFORCE,
    MIN_MAGICAL_REINFORCE,
    MAX_MAGICAL_REINFORCE,

    MIN_ATTACK_RATE,
    MAX_ATTACK_RATE,

    MIN_PARRY_RATE,
    MAX_PARRY_RATE,

    MIN_BLOCK_CHANCE,
    MAX_BLOCK_CHANCE,

    BREAKS_SHIELD,
    BREAKS_WEAPON,

    MIN_PHYS_REDUCTION,
    MAX_PHYS_REDUCTION,
    MIN_MAGICAL_REDUCTION,
    MAX_MAGICAL_REDUCTION,

    MIN_PHYS_ABSORPTION,
    MAX_PHYS_ABSORPTION,
    MIN_MAGICAL_ABSORPTION,
    MAX_MAGICAL_ABSORPTION,

    MIN_PHYS_DEF,
    MAX_PHYS_DEF,
    MIN_MAGICAL_DEF,
    MAX_MAGICAL_DEF,

    FREEZING_STATUS_CHANCE,
    POSIONING_STATUS_CHANCE,
    ELECTRIC_SHOCK_STATUS_CHANCE,
    BURN_STATUS_CHANCE,
    KNOCKBACK_STATUS_CHANCE,
    IMMOBILIZATION_STATUS_CHANCE,
    BLIND_STATUS_CHANCE,
    Frostbite_STATUS_CHANCE,
    DISEASE_STATUS_CHANCE,
    FEAR_STATUS_CHANCE,
    SLEEP_STATUS_CHANCE,
    GLAMOUR_STATUS_CHANCE,
    PAIN_STATUS_CHANCE,
    CRAZY_STATUS_CHANCE,
    ROT_STATUS_CHANCE,
    HYPNOTIC_STATUS_CHANCE,
    BLEED_STATUS_CHANCE,
    Dull_STATUS_CHANCE,
    IMPOTENT_STATUS_CHANCE,
    STUN_STATUS_CHANCE,

    FREEZING_DURATION_REDUCED,
    POSIONING_DURATION_REDUCED,
    ELECTRIC_SHOCK_DUTATION_REDUCED,
    BURN_DURATION_REDUCED,
    KNOCKBACK_RESISTANCE,
    IMMOBILIZATION_RESISTANCE,
    BLIND_RESISTANCE,
    Frostbite_RESISTANCE,
    DISEASE_RESISTANCE,
    FEAR_RESISTANCE,
    SLEEP_RESISTANCE,
    GLAMOUR_RESISTANCE,
    PAIN_RESISTANCE,
    CRAZY_RESISTANCE,
    ROT_RESISTANCE,
    HYPNOTIC_RESISTANCE,
    BLEED_RESISTANCE,
    Dull_RESISTANCE,
    IMPOTENT_RESISTANCE,
    STUN_RESISTANCE,

    INCREASE_GOLD,
    INCREASE_EXP,
    INCREASE_SKILLPOINT,

    PURE_DAMAGE,
    DAMAGE_AGAIN_ASSASSIN,
    DAMAGE_AGAIN_PALADIN,
    DAMAGE_AGAIN_ZEALOT,
    DAMAGE_AGAIN_SORCERESS,
    DAMAGE_AGAIN_WIZARD,
    DAMAGE_AGAIN_MARKSMAN,
    DAMAGE_AGAIN_ORG,
    DAMAGE_AGAIN_BARBARIAN,
    DEMENTIA_STATUS_CHANCE,
    DEMENTIA_RESISTANCE,
    BLOOD_THIRST_STATUS_CHANCE,
    BLOOD_THIRST_RESISTANCE,

    // chi so dung trong tinh toan
    //Min_Physical_damage,//Dame_vật_lý	Min	Max
    //Max_Physical_damage,
    //Min_Magical_damage,//Dame_phép_Min	Max
    //Max_Magical_damage,

    //Critical_chance,//tỉ_lệ_Crit_	
    //Multicast_chance,//Tỉ_lệ_Multicast_

    //Min_Physical_reinforce,//Gia_tăng_vật_lý	_Min	Max	
    //Max_Physical_reinforce,
    //Min_Magical_reinforce,//Gia_tăng_phép	Min	Max
    //Max_Magical_reinforce,

    //Min_Attack_rate,//Tỉ_lệ_tấn_công	_Min	Max_-_Số
    //Max_Attack_rate,

    //Min_Parry_rate,//Tỉ_lệ_do~	_Min	Max__-_Số
    //Max_Parry_rate,

    //Min_Block_chance,//Tỉ_lệ_Block	Min	Max_-_%
    //Max_Block_chance,

    //Min_Physical_Def,//Def_vật_lý		Min	Max	
    //Max_Physical_Def,
    //Min_Magical_Def,//Def_phép	_Min	Max
    //Max_Magical_Def,

    //Min_Physical_reductions,//Reduction_vật_lý		Min	Max	
    //Max_Physical_reductions,
    //Min_Magical_reduction, //Reduction_phép	_Min	Max
    //Max_Magical_reduction,

    //Min_Physical_absorption,//absorption_vật_lý		Min	Max	
    //Max_Physical_absorption,
    //Min_Magical_absorption,//Reduction_phép	_Min	Max
    //Max_Magical_absorption,


    ////các_dòng_xanh_+_thêm_cho_đồ

    //Strength,
    //Intelligence,
    //Vitality,//_Tăng_lượng_Health_của_nhân_vật_và_tăng_tốc_độ_hồi_máu_của_nhân_vật_(%_Health_mỗi_turn)
    //Dexterity,//__Dexterity_tăng_Critical_damage_vật_lý_thêm_%
    //Focus,//__Focus_tăng_%_khả_năng_đánh_thường_thêm__lần_nữa_đối_với_vũ_khí_phép
    //Agility,//__Agility_tăng_tỉ_lệ_Dodge_chance_(đòn_đánh_của_vũ_khí_thuần_phép)%
    //Luck,//Tăng_tỉ_lệ_nhặt_đồ_hiếm
    //Endurance,//_Quy_định_bởi_Endurance,Endurance_giảm_%_All_damage_reduces_phải_nhận
    //Blessing,//_Tăng_tỉ_lệ_cường_hóa_cấp_độ_đồ,Blessing_tăng_%_tỉ_lệ_thành_công,độ_lớn_của_Blessing_là_số_lần_dùng_của_nó._Ví_dụ_Blessing_+__time(s)_thì_trong__lần_cường_hóa_liên_tiếp_mỗi_lần_thêm_%_tỉ_lệ_thành_công
    //Protection,//_Loại_trừ_khả_năng_bị_Reset_về__hoặc_về__nếu_cường_hóa_cấp_độ_thất_bại,trừ__cấp_độ_thay_vì_về__hoặc_._Ví_dụ_Proctection_+__time(s)_thì_trong__lần_cường_hóa_thất_bại_liên_tiếp,thay_vì_bị_reset_về__hoặc__thì_nó_chỉ_giảm__cấp_độ			
    //Health,
    //Breaks_Shield,//_Giảm_khả_năng_block_của_Khiên,Breaks_Shield_giảm_%_khả_năng_block_của_Khiên_đối_phương_(nếu_có)
    //Accuracy,//Giảm_khả_năng_né_tránh_đòn_phép_của_đồi_phương_(nếu_có)_,Accuracy_giảm_%_khả_năng_né_tránh_của_đối_phương_đối_với_đòn_của_vũ_khí_phép
    //Breaks_Weapon,//_Giảm_khả_năng_critical_của_vũ_khí,Breaks_Weapon_giảm_%_khả_năng_critical_của_vũ_khí_đối_phương_
    //Freezing_Status,
    //Poisoning_Status,
    //Electric_shock_Status,
    //Burn_Status,

    ////Các_dị_trạng_có_do_skill_

    //Knockback_status,
    //Immobilization_status,
    //Blind_status,
    //Dementia_status,
    //Disease_Status,
    //Fear_Status,
    //Sleep_Status,
    //Glamour_Status,
    //Pain_Status,
    //Crazy_status,
    //Rot_Status,
    //Hypnotic_Status,
    //Bleed_Status,
    //Blood_Thirst_Status,
    //Impotent_Status,
    //Stun_Status,

    ////Các_chống_dị_trạng_có_do_skill_

    //Freezing_Duration_Reduced,
    //Poisoning_Duration_Reduced,
    //Electric_shock_Duration_Reduced,
    //Burn_Duration_Reduced,
    //Knockback_resistance,
    //Immobilization_resistance,
    //Blind_resistance,
    //Dementia_resistance,
    //Disease_resistance,
    //Fear_resistance,
    //Sleep_resistance,
    //Glamour_resistance,
    //Pain_resistance,
    //Crazy_resistance,
    //Rot_resistance,
    //Hypnotic_resistance,
    //Bleed_resistance,
    //Blood_Thirst_resistance,
    //Impotent_resistance,
    //Stun_resistance,

    ///

    //Increase_gold,
    //Increase_exp,
    //Increase_skill_points,
    //Luck,//Tăng_khả_năng_tìm_đồ_hiếm
    //Blessing,//Tăng_khả_năng_thành_công_khi_cường_hóa_chỉ_số				
    //Pure_damage,//Khả_năng_đánh_ra_Sát_thương_tuyệt_đối_xuyên_giáp

    //Các_dòng_cộng_thêm_cho_Sách
    //damage_against_Assassin,
    //damage_against_Paladin,
    //damage_against_Zealot,
    //damage_against_Sorceress,
    //damage_against_Wizard,
    //damage_against_Marksman,
    //damage_against_Org,
    //damage_against_Barbarian,

    //_Các_dòng_trắng_cho_sẵn_của_Avatar
    //Health,//_co_roi_ma_
    Physical_damage,
    Magical_damamge,
    Physical_absoption,
    Magical_absorption,
    //
    Poisoning_round,
    Maximum_HP,
    Physical_defense,
    Total_damage,
    Magical_defense,
    Poison_damage,
    weapon_damage,
    Action_Point,
    Remove_All_Status_Chance,
    Damage_return_chance,
    Physical_Damage_return_ratio,
    Magical_Damage_return_ratio,
    Shield_physical_defense,
    Shield_magical_defense,
    //Pure_damage,
    Decrease_All_Status_Round_Chance,
    CureChance, // kha nang xoa bo  di trang bat ky
    Crazy_Round,
    Damage_return_ratio,
    Healing_spells_effect, // hoi them  luong % phep do hoi mau do
    All_Status_Round,

    Successive_attacks,



    //ket thuc chi so tinh toan

    RATE,
    _PHYS_DAMAGE,
    _MAGICAL_DAMAGE,
    _PHYS_REINFORCE,
    _MAGICAL_REINFORCE,
    _ATTACK_RATE,
    _PARRY_RATE,
    _BLOCK_CHANCE,
    _PHYS_REDUCTION,
    _MAGICAL_REDUCTION,
    _PHYS_ABSORPTION,
    _MAGICAL_ABSORPTION,
    _PHYS_DEF,
    _MAGICAL_DEF,
}





public enum ITEMTYPE
{
    NONE,
    RECOVERY_HEAlTH_NUMBER,
    LUCKY_MATERIAL_REINFORCEMENT,
    CIVILIAN,
    SOLDIER,
    BASIC_EXPERIENCE_SCROLL,
    BASIC_SKILL_SCROLL,
    FOOD_PET,
    GROWUP_PET,
    BRONZE_COIN,
    SILVER_COIN,
    GOLDEN_COIN,
    SPECIAL_LUCKY_MATERIAL_REINFORCEMENT,
    EXPERIENCE_SCROLL,
    SKILL_SCROLL,
    STRENGTH_SCROLL,
    INTELLIGENCE_SCROLL,
    HEATH_SCROLL,
    PHYSICAL_REDUCTION_SCROLL,
    MAGICAL_REDUCTION_SCROLL,
    SPELL_EFFECT_REDUCTION_SCROLL,
    RECOVERY_HEAlTH_PERCENT,
    ONE_DAY_PROTECTION_SCROLL,

    SPECIAL_EXPERIENCE_SCROLL,
    SPECIAL_SKILL_SCROLL,
    PRACTICE_SCROLL,
    SPECIAL_PRACTICE_SCROLL,
    SPECIAL_FOOD_PET,
    ONE_DAY_SILVER_TIME_PREMIUM,
    ONE_DAY_GOLDEN_TIME_PREMIUM,
    SEVEN_DAY_SILVER_TIME_PREMIUM,
    SEVEN_DAY_GOLDEN_TIME_PREMIUM,
    SEVEN_DAY_EXP_SKILL_POINT_BOOTSTER,
    SEVEN_DAY_GOLDEN_TIME_PREMIUN,
    SKILL_POINT_RECALL,
    RUNESTONE_MASTERIALALCHEMY = 23,
    RUNESTONE_SPECIALALCHEMY = 25,
    RUNESTONE_ACCURACY = 77,
    RUNESTONE_IMMUNITY = 78,
    RUNESTONE_AIR = 79,
    RUNESTONE_WEALTH = 80,
    RUNESTONE_EXPERIENCE = 81,
    RUNESTONE_CRAFTSMANSHIP = 82,
    RUNESTONE_BLESSING = 83,
    RUNESTONE_LUCK = 84,
    RUNESTONE_PENETRATION = 85,
    RUNESTONE_REMOVAL = 86,
    RUNESTONE_ETERNITY = 87,
    RUNESTONES_STRENGTH = 88,
    RUNESTONES_INTELLIGENCE = 89,
    RUNESTONES_VITALITY = 90,
    RUNESTONES_DEXTERITY = 91,
    RUNESTONES_FORCUS = 92,
    RUNESTONE_AGILITY = 93,
    RUNESTONE_ENDURANCE = 94,
    RUNESTONE_STAMINA = 95,
    RUNESTONE_SOPHISTICATION = 96,
    RUNESTONE_IRONSHIELD = 97,
    RUNESTONE_MELT = 98,
    RUNESTONE_PROTECTION = 99
}

public enum PROPERTIESNAME
{
    Min_Physical_damage = 1,
    Max_Physical_damage = 2,
    Min_Magical_damage = 3,
    Max_Magical_damage = 4,
    Critical_chance = 5,
    Multicast_chance = 6,
    Min_Physical_reinforce = 7,
    Max_Physical_reinforce = 8,
    Min_Magical_reinforce = 9,
    Max_Magical_reinforce = 10,
    Min_Attack_rate = 11,
    Max_Attack_rate = 12,
    Min_Parry_rate = 13,
    Max_Parry_rate = 14,
    Block_chance = 16,
    Min_Physical_Def = 17,
    Max_Physical_Def = 18,
    Min_Magical_Def = 19,
    Max_Magical_Def = 20,
    Min_Physical_reductions = 21,
    Max_Physical_reductions = 22,
    Min_Magical_reduction = 23,
    Max_Magical_reduction = 24,
    Min_Physical_absorption = 25,
    Max_Physical_absorption = 26,
    Min_Magical_absorption = 27,
    Max_Magical_absorption = 28,
    Strength = 29,
    Intelligence = 30,
    Vitality = 31,
    Dexterity = 32,
    Focus = 33,
    Agility = 34,
    Luck = 35,
    Endurance = 36,
    Blessing = 37,
    Protection = 38,
    Health = 39,
    Breaks_Shield = 40,
    Accuracy = 41,
    Breaks_Weapon = 42,
    Freezing_Status = 43,
    Poisoning_Status = 44,
    Electric_shock_Status = 45,
    Burn_Status = 46,
    Knockback_status = 47,
    Immobilization_status = 48,
    Blind_status = 49,
    Frostbite_status = 50,
    Disease_Status = 51,
    Fear_Status = 52,
    Sleep_Status = 53,
    Glamour_Status = 54,
    Pain_Status = 55,
    Crazy_status = 56,
    Rot_Status = 57,
    Hypnotic_Status = 58,
    Bleed_Status = 59,
    Dull_Status = 60,
    Impotent_Status = 61,
    Stun_Status = 62,
    Freezing_Duration_Reduced = 63,
    Poisoning_Duration_Reduced = 64,
    Electric_shock_Duration_Reduced = 65,
    Burn_Duration_Reduced = 66,
    Knockback_resistance = 67,
    Immobilization_resistance = 68,
    Blind_resistance = 69,
    Frostbite_resistance = 70,
    Disease_resistance = 71,
    Fear_resistance = 72,
    Sleep_resistance = 73,
    Glamour_resistance = 74,
    Pain_resistance = 75,
    Crazy_resistance = 76,
    Rot_resistance = 77,
    Hypnotic_resistance = 78,
    Bleed_resistance = 79,
    Blood_Thirst_resistance = 80,
    Impotent_resistance = 81,
    Stun_resistance = 82,
    Increase_gold = 83,
    Increase_exp = 84,
    Increase_skill_points = 85,
    Pure_damage = 88,
    damage_against_Assassin = 89,
    damage_against_Paladin = 90,
    damage_against_Zealot = 91,
    damage_against_Sorceress = 92,
    damage_against_Wizard = 93,
    damage_against_Marksman = 94,
    damage_against_Org = 95,
    damage_against_Barbarian = 96,
    Physical_damage = 98,
    Magical_damamge = 99,
    Physical_absoption = 100,
    Magical_absorption = 101,

}


public class Constant
{

    public static string urlRequest = "http://45.32.106.62";
    public static string USER_NAME = "username";
    public static string PASSWORD = "password";
    public static string IDHERO_CURRENTPLAY = "ideherocurrent";
    public static int ID_ERROR = -1;
    public static string VERSION = "version";
    public static string AllINIT = "allinit";
    public static string EQUIPPEDINIT = "equippedinit";
    public static string SHOPINIT = "shopinit";

    public static string[] PROPERTIES =
    {
            "Min Physical damage",
             "Max Physical damage",
             "Min Magical damage",
             "Max Magical damage",
             "Critical chance",
             "Multicast chance",
             "Min Physical reinforce",
             "Max Physical reinforce",
             "Min Magical reinforce",
             "Max Magical reinforce",
             "Min Attack rate",
             "Max Attack rate",
             "Min Parry rate",
             "Max Parry rate",
             "Block chance",
             "Min Physical Def",
             "Max Physical Def",
             "Min Magical Def",
             "Max Magical Def",
             "Min Physical reductions",
             "Max Physical reductions",
             "Min Magical reduction",
             "Max Magical reduction",
             "Min Physical absorption",
             "Max Physical absorption",
             "Min Magical absorption",
             "Max Magical absorption",
             "Strength",
             "Intelligence",
             "Vitality",
             "Dexterity",
             "Focus",
             "Agility",
             "Luck",
             "Endurance",
             "Blessing",
             "Protection",
             "Health",
             "Breaks Shield",
             "Accuracy",
             "Breaks Weapon",
             "Freezing Status",
             "Poisoning Status",
             "Electric shock Status",
             "Burn Status",
             "Knockback status",
             "Immobilization status",
             "Blind status",
             "Frostbite status",
             "Disease Status",
             "Fear Status",
             "Sleep Status",
             "Glamour Status",
             "Pain Status",
             "Crazy status",
             "Rot Status",
             "Hypnotic Status",
             "Bleed Status",
             "Dull Status",
             "Impotent Status",
             "Stun Status",
             "Freezing Duration Reduced",
             "Poisoning Duration Reduced",
             "Electric shock Duration Reduced",
             "Burn Duration Reduced",
             "Knockback resistance",
             "Immobilization resistance",
             "Blind resistance",
             "Frostbite resistance",
             "Disease resistance",
             "Fear resistance",
             "Sleep resistance",
             "Glamour resistance",
             "Pain resistance",
             "Crazy resistance",
             "Rot resistance",
             "Hypnotic resistance",
             "Bleed resistance",
             "Blood Thirst resistance",
             "Impotent resistance",
             "Stun resistance",
             "Increase gold",
             "Increase exp",
             "Increase skill points",
             "Pure damage",
             "damage against Assassin",
             "damage against Paladin",
             "damage against Zealot",
             "damage against Sorceress",
             "damage against Wizard",
             "damage against Marksman",
             "damage against Org",
             "damage against Barbarian",
             "Physical damage",
             "Magical damamge",
             "Physical absoption",
             "Magical absorption"
    };

    public static Dictionary<int, string> propertiesDict = new Dictionary<int, string>();

    public static void init()
    {

        foreach (string _tempProperties in PROPERTIES)
        {
            string _replacePro = _tempProperties.Replace(' ', '_');
            if(!propertiesDict.ContainsKey((int)Utilities.ParseEnum<PROPERTIESNAME>(_replacePro)))
            propertiesDict.Add((int)Utilities.ParseEnum<PROPERTIESNAME>(_replacePro), _tempProperties);

        }
    }

    public static string getPropertiesName(int index)
    {
        return (propertiesDict.ContainsKey(index) ? propertiesDict[index] : "");
    }
    public static string getPropertiesName(string index)
    {
        return (propertiesDict.ContainsKey(int.Parse(index)) ? propertiesDict[int.Parse(index)] : "");
    }

    public static string displayTypeProperty(int id, float value)
    {
        string tempStr = "";
        if (id == 32) tempStr = (value * 100).ToString() + "%";
        else if (id == 33 || id == 40 || id == 41 || id == 36 || id == 42 || id == 34 || id == 39) tempStr = (value * 100).ToString() + "%";
        else if (id == 37 || id == 38) tempStr = ((int)value).ToString();
        else if (id == 63 || id == 64 || id == 65 || id == 66) tempStr = (value * 100).ToString() + "%";
        else if (id == 29 || id == 30 || id == 31) tempStr = ((int)value).ToString();
        else tempStr = value.ToString();
        return tempStr;
    }

    public static int[] UPGRADE_REINFORCE_SUCCESSFULRATE = new int[50]
    {
        100,90,81,73,66,59,53,48,43,39,36,33,30,28,26,23,22,20,18,17,16,15,14,13,12,12,11,10,10,9,9,8,8,8,7,7,7,7,6,6,6,6,6,6,5,5,5,5,5,5
    };

    public static int[] UPGRADE_REINFORCE_SUCCESSFULRATE_AVATAR = new int[10] { 90, 80, 70, 60, 50, 40, 30, 20, 10, 5 };
    public static int[] UPGRADE_REINFORCE_SUCCESSFULRATE_BOOK = new int[10] { 90, 80, 70, 60, 50, 40, 30, 20, 10, 5 };

    public static int[] UPGRADE_COMBINE_PRICE = new int[10] { 0, 100, 200, 400, 800, 1600, 3200, 6400, 12800, 25600 };

    public static int[] UPGRADE_SKILLPOINT_REQUIRE = new int[125]
    {
       0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,16,18,20,22,24,27,30,33,37,41,46,51,57,63,70,78,87,97,108,120,133,148,164,182,202,224,
       249,276,306,340,377,418,464,515,572,635,705,783,869,965,1071,1189,1320,1465,1626,1805,2004,2224,2469,2741,3043,3378,3750,4163,4621,5129,5693,6319,7014,7786,8642,9593,10648,
       11819,13119,14562,16164,17942,19916,22107,24539,27238,30234,33560,37252,41350,45899,50948,56552,62773,69678,77343,85851,95295,105777,117412,130372,144663,160576,178239,197845,219608,
       243765,270579,300343,333381,370053,410759,455942,506096,561767,623561,692153,768290,852802,946610,1050737,1166318,1294613,1437020
    };
}


