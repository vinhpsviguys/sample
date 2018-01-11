using System;
using System.Collections;
using System.Collections.Generic;



namespace CoreLib
{
    public class CharacterStatus // khi lai trang thai cua nhan vat khi mac do, cong diem, chiu cac tac dung skill
    {
        
        public int playerID;
        public Character character;

        public Characteristic.CharacterType Type = Characteristic.CharacterType.MELE; // 
        public Characteristic.CharacterClass Class = Characteristic.CharacterClass.Assassin;
        public int Level = 1;
        public float Max_Health;
        public float Health;
        public int Strength;
        public int Intelligence;
        public int Dexterity;
        public int Focus;
        public int Vitality;
        public int Agility; // Agility
        public int Luck;
        public int Endurance;
        public int Blessing;
        public int Protection;

        public int PhysicalDamage;
        public float PhysicalReinforce;
        public float PhysicalReduction;
        public int PhysicalDefense;
        public int MagicalDamage;
        public float MagicalReinforce;
        public int MagicalDefense;
        public float MagicalReduction;
        public float CriticalChance;
        public float CriticalDamage;
        public float MulticastChance;
        public float BlockingChance;
        public float DodgeChance;
        public float HealthRecovery;
        public int Reputation;
        public float AttackRate;
        public float ParryRate;

        public float FreezingDurationReduce;// Reduces Freezing Duration – Giảm thời gian bị Đóng băng
        public float PoisoningDurationReduce;// Reduces Poisoning Duration – Giảm thời gian bị Nhiễm độc
        public float ElectricShockDurationReduce;// Reduces Electric shock Duration – Giảm thời gian bị Sốc
        public float BurnDurationReduce;// Reduces Burn Duration – Giảm thời gian bị Thiêu đốt
        public float DiseaseDurationReduced;// Reduces Disease Duration – Giảm thời gian bị Bệnh tật
        public float FearDurationReduce;// Reduces Fear Duration – Giảm thời gian bị Sợ hãi
        public float KnockBackResistance;// Increase chance to resist Knock back – Tăng khả năng phòng chống Đẩy lùi
        public float ImmobilizationResistance;// Increase chance to resist Immobilization – Tăng khả năng phòng chống Trói chặt
        public float BlindnessResistance;// Increase chance to resist Blindness – Tăng khả năng phòng chống Mù loà
        public float DementiaResistance;// Increase chance to resist Dementia – Tăng khả năng phòng chống Mất trí
        public float SleepResistance;// Increase chance to resist Sleep – Tăng khả năng phòng chống Mộng du
        public float GlamourResistance;// Increase chance to resist Glamour – Tăng khả năng phòng chống Yểm bùa
        //public float PhyicalReinforce;// Increases the damage based on a percentage of Physical damage – Gia tăng thêm Sát thương theo tỉ lệ % Sát thương Vật lý
        //public float MagicalReinforce;// Increases the damage based on a percentage of Magical damage – Gia tăng thêm Sát thương theo tỉ lệ % Sát thương Phép thuật
        public float PhysicalAbsorbtion;// Absorb Physical damage – Hấp thụ 1 phần Sát thương Vật lý
        public float MagicalAbsorbtion;// Absorb Magical damage – Hấp thụ 1 phần Sát thương Phép thuật

        public Dictionary<string, ArrayList> effects = new Dictionary<string, ArrayList>();
        public ArrayList skills = new ArrayList();
        public int actionpoints;

        

        public CharacterStatus(Character character) {
            this.character = character;
            this.playerID = character.playerId;
            // strength -> damage va phong thu vat ly
            // intel -> damage va phong thu phep thuat
            // dex - > critical damage
            // focus -> Multicast chance
            // viality -> HP + recovery
            // agility -> Dodge Chance 
           
            // chi so

            loadCharacteristic(character.characteristic);
            this.skills = character.skills;
            putOnEquipments(character.equipments);


            this.PhysicalDamage += Constants.DAMAGE_PER_TIMES[Level - 1] + Constants.DAMAGE_PER_TIMES[(int)character.characteristic.Strength - Level + 1] + Constants.DAMAGE_PER_TIMES[(int)this.Strength - (int)character.characteristic.Strength];// Increases the damage from physical attacks - thể hiện damage vật lý
            this.PhysicalDefense += Constants.DEFEND_PER_TIMES[Level - 1] + Constants.DEFEND_PER_TIMES[(int)character.characteristic.Strength - Level + 1] + Constants.DEFEND_PER_TIMES[(int)this.Strength - (int)character.characteristic.Strength];// Take less damage from opponent's physical attack – Giảm lượng damage vật lý phải nhận
            this.MagicalDamage += Constants.DAMAGE_PER_TIMES[Level - 1] + Constants.DAMAGE_PER_TIMES[(int)character.characteristic.Intelligence - Level + 1] + Constants.DAMAGE_PER_TIMES[(int)this.Intelligence - (int)character.characteristic.Intelligence];// Increases the damage from Magical attacks - thể hiện damage phép thuật
            this.MagicalDefense += Constants.DEFEND_PER_TIMES[Level - 1] + Constants.DEFEND_PER_TIMES[(int)character.characteristic.Intelligence - Level + 1] + Constants.DEFEND_PER_TIMES[(int)this.Intelligence - (int)character.characteristic.Intelligence];// Take less damage from opponent's magical attack – Giảm lượng damage phép thuật phải nhận
            this.Max_Health += 1000 + Constants.HP_PER_TIMES[Level - 1] + Constants.HP_PER_TIMES[(int)character.characteristic.Vitality - Level + 1] + Constants.HP_PER_TIMES[(int)this.Vitality - (int)character.characteristic.Vitality];
            this.Health = this.Max_Health;
            this.CriticalDamage += this.Dexterity * 0.01f;
            this.MulticastChance += this.Focus * 0.01f;
            this.HealthRecovery += this.Vitality * 0.01f * Max_Health;
            this.DodgeChance += this.Agility * 0.01f;
            //this.PhysicalReinforce -= this.Endurance * 0.01f;
            //this.MagicalReinforce -= this.Endurance * 0.01f;

            Console.WriteLine("dodge and block " + DodgeChance + " " + BlockingChance);

            
        }

        private void loadCharacteristic(Characteristic characteristic) {
            this.Level = (int)characteristic.Level;
            this.Class = characteristic.Class;
            this.Type = characteristic.Type;
            this.Class = characteristic.Class;
            
            this.Strength = (int)characteristic.Strength;
            this.Intelligence = (int)characteristic.Intelligence;
            this.Vitality = (int)characteristic.Vitality;

            this.Health = 0;
            this.Max_Health = 0;

            this.Dexterity = (int)characteristic.Dexterity;
            this.Focus = (int)characteristic.Focus;

            this.Agility = (int)characteristic.Agility;
            this.Luck = (int)characteristic.Luck;
            this.Endurance = (int)characteristic.Endurance;
            this.Blessing = (int)characteristic.Blessing;
            this.Protection = (int)characteristic.Protection;


            this.PhysicalDamage = 0;
            this.PhysicalReinforce = characteristic.PhysicalReinforce;
            this.PhysicalDefense = 0;
            this.MagicalDamage = 0;
            this.MagicalReinforce = characteristic.MagicalReinforce;
            this.MagicalDefense = 0;
            this.CriticalChance = characteristic.CriticalChance;
            this.CriticalDamage = 0;
            this.MulticastChance = 0;
            this.BlockingChance = characteristic.BlockingChance;
            this.DodgeChance = 0;
            this.HealthRecovery = characteristic.HealthRecovery;
            this.Reputation = (int)characteristic.Reputation;
            this.AttackRate = characteristic.AttackRate;
            this.ParryRate = characteristic.ParryRate;
            this.PhysicalReduction = 0;
            this.MagicalReduction = 0;
        }

        private void putOnEquipments(Equipment[] equipments) {
            for (int i = 0; i < character.equipments.Length; i++)
            {
                Equipment equipment = character.equipments[i];
                if (equipment != null)
                {

                    this.Strength += Convert.ToInt32(equipment.getValue(Constants.Strength));
                    this.Intelligence += Convert.ToInt32(equipment.getValue(Constants.Intelligence));
                    this.Vitality += Convert.ToInt32(equipment.getValue(Constants.Vitality));
                    this.Dexterity += Convert.ToInt32(equipment.getValue(Constants.Dexterity));
                    this.Focus += Convert.ToInt32(equipment.getValue(Constants.Focus));
                    this.Agility += Convert.ToInt32(equipment.getValue(Constants.Agility));
                    this.Luck += Convert.ToInt32(equipment.getValue(Constants.Luck));
                    this.Endurance += Convert.ToInt32(equipment.getValue(Constants.Endurance));
                    this.Blessing += Convert.ToInt32(equipment.getValue(Constants.Blessing));
                    this.Protection += Convert.ToInt32(equipment.getValue(Constants.Protection));

                    this.Max_Health += Convert.ToInt32(equipment.getValue(Constants.Health));
                    this.PhysicalDamage += Convert.ToInt32(equipment.getValue(Constants.PhysicalDamage));
                    this.PhysicalReinforce += Convert.ToSingle(equipment.getValue(Constants.PhysicalReinforce));
                    this.PhysicalDefense += Convert.ToInt32(equipment.getValue(Constants.PhysicalDefense));
                    this.MagicalDamage += Convert.ToInt32(equipment.getValue(Constants.MagicalDamage));
                    this.MagicalReinforce += Convert.ToSingle(equipment.getValue(Constants.MagicalReinforce));
                    this.MagicalDefense += Convert.ToInt32(equipment.getValue(Constants.MagicalDefense));
                    this.CriticalChance += Convert.ToSingle(equipment.getValue(Constants.CriticalChance));
                    this.CriticalDamage += Convert.ToSingle(equipment.getValue(Constants.CriticalDamage));
                    this.MulticastChance += Convert.ToSingle(equipment.getValue(Constants.MulticastChance));
                    this.BlockingChance += Convert.ToSingle(equipment.getValue(Constants.BlockingChance));
                    this.DodgeChance += Convert.ToSingle(equipment.getValue(Constants.DodgeChance));
                    this.HealthRecovery += Convert.ToSingle(equipment.getValue(Constants.HealthRecovery));
                    this.Reputation += Convert.ToInt32(equipment.getValue(Constants.Reputation));
                    this.AttackRate += Convert.ToSingle(equipment.getValue(Constants.AttackRate));
                    this.ParryRate += Convert.ToSingle(equipment.getValue(Constants.ParryRate));
                    this.PhysicalReduction += Convert.ToSingle(equipment.getValue(Constants.PhysicalReduction));
                    this.MagicalReduction += Convert.ToSingle(equipment.getValue(Constants.MagicalReduction));


                    this.FreezingDurationReduce += Convert.ToSingle(equipment.getValue(Constants.FRZ_DURE_NA));
                    this.PoisoningDurationReduce += Convert.ToSingle(equipment.getValue(Constants.POI_DURE_NA));
                    this.ElectricShockDurationReduce += Convert.ToSingle(equipment.getValue(Constants.ELE_DURE_NA));
                    this.BurnDurationReduce += Convert.ToSingle(equipment.getValue(Constants.BUR_DURE_NA));
                    this.DiseaseDurationReduced += Convert.ToSingle(equipment.getValue("DiseaseDurationReduced"));
                    this.FearDurationReduce += Convert.ToSingle(equipment.getValue("FearDurationReduce"));
                    this.KnockBackResistance += Convert.ToSingle(equipment.getValue("KnockBackResistance"));
                    this.ImmobilizationResistance += Convert.ToSingle(equipment.getValue("ImmobilizationResistance"));
                    this.BlindnessResistance += Convert.ToSingle(equipment.getValue("BlindnessResistance"));
                    this.DementiaResistance += Convert.ToSingle(equipment.getValue("DementiaResistance"));
                    this.SleepResistance += Convert.ToSingle(equipment.getValue("SleepResistance"));
                    this.GlamourResistance += Convert.ToSingle(equipment.getValue("GlamourResistance"));

                    this.PhysicalAbsorbtion += Convert.ToSingle(equipment.getValue(Constants.PhysicalReduction));
                    this.MagicalAbsorbtion += Convert.ToSingle(equipment.getValue(Constants.MagicalAbsorbtion));
                }

            }
        }

     

      

      

        public ArrayList getEffects(string name) {
            if (!effects.ContainsKey(name)) {
                ArrayList list = new ArrayList();
                effects.Add(name, list);
            }
            return effects.ContainsKey(name) ? effects[name] : null;
        }

       
    }

    
}
