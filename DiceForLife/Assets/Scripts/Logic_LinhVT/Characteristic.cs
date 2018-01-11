using System;


namespace CoreLib
{
    public class Characteristic // cac chi so co ban khi nhan vat o tran co cong pofloat
    {
        public enum CharacterType
        {
            MELE = 0, RANGER
        };

        public enum CharacterClass
        {
            Assassin = 0,
            Paladin = 1,
            Zealot = 2,
            Sorceress = 3,
            Wizard = 4,
            Marksman = 5,
            Org = 6,
            Barbarian = 7
        }

        public enum DamageType
        {
            Physic,
            Magic,
            Hybrid
        }


        
        public CharacterType Type = CharacterType.MELE; // 
        public CharacterClass Class = CharacterClass.Assassin;
        public DamageType Damage = DamageType.Physic;

        public float Level = 1;
        public float Max_Health;
        public float Health;
        public float Strength;
        public float Intelligence;
        public float Dexterity;
        public float Focus;
        public float Vitality;
        public float Agility; // Agility
        public float Luck;
        public float Endurance;
        public float Blessing;
        public float Protection;
        public float PhysicalDamage;
        public float PhysicalReinforce;
        public float PhysicalDefense;
        public float MagicalDamage;
        public float MagicalReinforce;
        public float MagicalDefense;
        public float CriticalChance;
        public float CriticalDamage;
        public float MulticastChance;
        public float BlockingChance;
        public float DodgeChance; 
        public float HealthRecovery;
        public float Reputation;
        public float AttackRate;
        public float ParryRate;


        public JSONNode toJSON() {
            JSONNode obj = new JSONObject();
            obj.Add(Constants.Level, new JSONNumber(Level));
            obj.Add(Constants.Type, new JSONString(Type.ToString()));
            obj.Add(Constants.Max_Health, new JSONNumber(Max_Health));
            obj.Add(Constants.Class, new JSONString(Class.ToString()));
            obj.Add(Constants.Strength, new JSONNumber(Strength));
            obj.Add(Constants.Intelligence, new JSONNumber(Intelligence));
            obj.Add(Constants.Dexterity, new JSONNumber(Dexterity));
            obj.Add(Constants.Focus, new JSONNumber(Focus));
            obj.Add(Constants.Vitality, new JSONNumber(Vitality));
            obj.Add(Constants.Agility, new JSONNumber(Agility));
            obj.Add(Constants.Luck, new JSONNumber(Luck));
            obj.Add(Constants.Endurance, new JSONNumber(Endurance));
            obj.Add(Constants.Protection, new JSONNumber(Protection));
            obj.Add(Constants.Blessing, new JSONNumber(Blessing));
                

            obj.Add(Constants.PhysicalDamage, new JSONNumber(PhysicalDamage));
            obj.Add(Constants.PhysicalReinforce, new JSONNumber(PhysicalReinforce));
            obj.Add(Constants.PhysicalDefense, new JSONNumber(PhysicalDefense));
            obj.Add(Constants.MagicalDamage, new JSONNumber(MagicalDefense));
            obj.Add(Constants.MagicalReinforce, new JSONNumber(MagicalReinforce));
            obj.Add(Constants.MagicalDefense, new JSONNumber(MagicalDefense));
            obj.Add(Constants.CriticalChance, new JSONNumber(CriticalChance));
            obj.Add(Constants.MulticastChance, new JSONNumber(MulticastChance));
            obj.Add(Constants.BlockingChance, new JSONNumber(BlockingChance));
            obj.Add(Constants.DodgeChance, new JSONNumber(DodgeChance));
            obj.Add(Constants.HealthRecovery, new JSONNumber(HealthRecovery));
            obj.Add(Constants.Reputation, new JSONNumber(Reputation));
            obj.Add(Constants.AttackRate, new JSONNumber(AttackRate));
            obj.Add(Constants.ParryRate, new JSONNumber(ParryRate));
            return obj;
        }

        public static Characteristic parseJSON(string aJSON) {
            Console.WriteLine("parse Characteristic " + aJSON);
            JSONNode obj = JSON.Parse(aJSON);
            Characteristic characteristic = new Characteristic();
            switch (obj[Constants.Type].Value) {
                case "MELE":
                    characteristic.Type = CharacterType.MELE;
                    break;
                case "RANGER":
                    characteristic.Type = CharacterType.RANGER;
                    break;
                default:
                    characteristic.Type = CharacterType.MELE;
                    break;
            }
            switch (obj[Constants.Class].Value) {
                case "Assassin":
                    characteristic.Class = CharacterClass.Assassin;
                    break;
                case "Paladin":
                    characteristic.Class = CharacterClass.Paladin;
                    break;
                case "Zealot":
                    characteristic.Class = CharacterClass.Zealot;
                    break;
                case "Sorceress":
                    characteristic.Class = CharacterClass.Sorceress;
                    break;
                case "Wizard":
                    characteristic.Class = CharacterClass.Wizard;
                    break;
                case "Marksman":
                    characteristic.Class = CharacterClass.Marksman;
                    break;
                case "Org":
                    characteristic.Class = CharacterClass.Org;
                    break;
                case "Barbarian":
                    characteristic.Class = CharacterClass.Barbarian;
                    break;
                default:
                    characteristic.Class = CharacterClass.Assassin;
                    break;
            }

            switch (characteristic.Class)
            {
                case CharacterClass.Assassin:
                case CharacterClass.Paladin:
                    characteristic.Damage = DamageType.Physic;
                    break;
                case CharacterClass.Wizard:
                    characteristic.Damage = DamageType.Magic;
                    break;
                default:
                    characteristic.Damage = DamageType.Magic;
                    break;
            }

            //characteristic.Level = Convert.Tofloat32(obj[Constants.Level].Value);
            //characteristic.Strength = Convert.Tofloat32(obj[Constants.Strength].Value);
            //characteristic.floatelligence = Convert.Tofloat32(obj[Constants.floatelligence].Value);
            //characteristic.Dexterity = Convert.Tofloat32(obj[Constants.Dexterity].Value);
            //characteristic.Focus = Convert.Tofloat32(obj[Constants.Focus].Value);
            //characteristic.Vitality = Convert.Tofloat32(obj[Constants.Vitality].Value);
            //characteristic.Agility = Convert.Tofloat32(obj[Constants.Agility].Value);
            //characteristic.Luck = Convert.Tofloat32(obj[Constants.Luck].Value);
            //characteristic.Endurance = Convert.Tofloat32(obj[Constants.Endurance].Value);
            //characteristic.Protection = Convert.Tofloat32(obj[Constants.Protection].Value);
            //characteristic.Blessing = Convert.Tofloat32(obj[Constants.Blessing].Value);

            //characteristic.Max_Health = 100 + Constants.HP_PER_TIMES[characteristic.Level - 1] + Constants.HP_PER_TIMES[characteristic.Vitality - characteristic.Level + 1];
            //characteristic.Health = characteristic.Max_Health;

            //characteristic.PhysicalDamage = 0;// Increases the damage from physical attacks - thể hiện damage vật lý
            //characteristic.PhysicalReinforce = 0;// Increases the damage from physical attacks based on a percentage of physical damage – Tăng thêm damage vật lý theo tỉ lệ phần trăm của damage vật lý
            //characteristic.PhysicalDefense = 0;// Take less damage from opponent's physical attack – Giảm lượng damage vật lý phải nhận
            //characteristic.MagicalDamage = 0;// Increases the damage from Magical attacks - thể hiện damage phép thuật
            //characteristic.MagicalReinforce = 0;// Increases the damage from magical attacks based on a percentage of magical damage - Tăng thêm damage phép theo tỉ lệ phần trăm của damage phép
            //characteristic.MagicalDefense = 0;// Take less damage from opponent's magical attack – Giảm lượng damage phép thuật phải nhận
            //characteristic.CriticalChance = 0;// Increases the chance to perform a critical attack - thể hiện tỉ lệ ra đòn chí mạng
            //characteristic.CriticalDamage = 0.01f * characteristic.Dexterity;// Damage of the critical attacks – thể hiện lượng Sát thương cộng thêm khi người chơi ra đòn Chí mạng(mặc định tăng thêm 100% Sát thương vật lý)
            //characteristic.MulticastChance = 0.01f * characteristic.Focus;// Increases the chance to perform a double casting spells - thể hiện tỉ lệ đánh 2 lần 1 phép bất kỳ
            //characteristic.BlockingChance = 0;// Increases the chance to block attacks with a shield - thể hiện tỉ lệ ngăn chặn hoàn toàn 1 đòn tấn công vật lý bất kỳ
            //characteristic.DodgeChance = 0.01f * characteristic.Agility; //Increases the chance to dodge attacks - thể hiện tỉ lệ tránh né hoàn toàn 1 đòn tấn công vật lý hoặc phép thuật bất kỳ
            //characteristic.HealthRecovery = 0.01f * characteristic.Vitality * characteristic.Max_Health;// Restores health on the target unit over time – Tự động hồi 1 lượng HP nhất định sau mỗi turn
            //characteristic.Reputation = 0;// (Fame): Số danh tiếng người chơi đang có(Cộng sau mỗi lần hoàn thành nhiệm vụ)
            //characteristic.AttackRate = 0;// Increases the chance to do maximum damage – Tăng tỉ lệ ra đòn max damage
            //characteristic.ParryRate = 0;// Increases the chance to take minimum damage – Tăng tỉ lệ nhận đòn min damage


            return characteristic;
        }

        public static Characteristic createCharacteristic() {
            Random random = new Random();
            Characteristic characteristic = new Characteristic();
            characteristic.Type = CharacterType.MELE;
            characteristic.Class = CharacterClass.Assassin;
            characteristic.Level = 1;
            
            characteristic.Strength = characteristic.Level - 1;
            characteristic.Intelligence = characteristic.Level - 1;
            characteristic.Dexterity = 0;
            characteristic.Focus = 0;
            characteristic.Vitality = characteristic.Level - 1;
            characteristic.Agility = 0;
            characteristic.Luck = 0;
            characteristic.Endurance = 0;
            characteristic.Blessing = 0;

            
            characteristic.Max_Health = 1000 + Constants.HP_PER_TIMES[(int)characteristic.Level - 1] + Constants.HP_PER_TIMES[(int)characteristic.Vitality - (int)characteristic.Level + 1];
            characteristic.Health = characteristic.Max_Health ;
            //Physical Damage +1 %,
            //Physical Defense +0,1 %
            //Magical Damage + 1 %
            //Magical Defense + 0,1 %
            //Critical damage + 1 %
            //Multicast Chance + 1 %
            //Health + 10 %, (tra bảng HP)
            //Health Recovery +1 %
            //Dodge Chance + 1 %
            //Luck + 1
            //Damage reduces 1 %
            //Blessing + 1


            characteristic.PhysicalDamage = Constants.DAMAGE_PER_TIMES[(int)characteristic.Level - 1] + Constants.DAMAGE_PER_TIMES[(int)characteristic.Strength - (int)characteristic.Level + 1];// Increases the damage from physical attacks - thể hiện damage vật lý
            characteristic.PhysicalReinforce = 0;// Increases the damage from physical attacks based on a percentage of physical damage – Tăng thêm damage vật lý theo tỉ lệ phần trăm của damage vật lý
            characteristic.PhysicalDefense = Constants.DEFEND_PER_TIMES[(int)characteristic.Level - 1] + Constants.DEFEND_PER_TIMES[(int)characteristic.Strength - (int)characteristic.Level + 1];// Take less damage from opponent's physical attack – Giảm lượng damage vật lý phải nhận
            characteristic.MagicalDamage = Constants.DAMAGE_PER_TIMES[(int)characteristic.Level - 1] + Constants.DAMAGE_PER_TIMES[(int)characteristic.Intelligence - (int)characteristic.Level + 1];// Increases the damage from Magical attacks - thể hiện damage phép thuật
            characteristic.MagicalReinforce = 0;// Increases the damage from magical attacks based on a percentage of magical damage - Tăng thêm damage phép theo tỉ lệ phần trăm của damage phép
            characteristic.MagicalDefense = Constants.DEFEND_PER_TIMES[(int)characteristic.Level - 1] + Constants.DEFEND_PER_TIMES[(int)characteristic.Intelligence - (int)characteristic.Level + 1];// Take less damage from opponent's magical attack – Giảm lượng damage phép thuật phải nhận
            characteristic.CriticalChance = 0;// Increases the chance to perform a critical attack - thể hiện tỉ lệ ra đòn chí mạng
            characteristic.CriticalDamage = 0.01f * characteristic.Dexterity;// Damage of the critical attacks – thể hiện lượng Sát thương cộng thêm khi người chơi ra đòn Chí mạng(mặc định tăng thêm 100% Sát thương vật lý)
            characteristic.MulticastChance = 0.01f * characteristic.Focus;// Increases the chance to perform a double casting spells - thể hiện tỉ lệ đánh 2 lần 1 phép bất kỳ
            characteristic.BlockingChance = 0;// Increases the chance to block attacks with a shield - thể hiện tỉ lệ ngăn chặn hoàn toàn 1 đòn tấn công vật lý bất kỳ
            characteristic.DodgeChance = 0.01f * characteristic.Agility; //Increases the chance to dodge attacks - thể hiện tỉ lệ tránh né hoàn toàn 1 đòn tấn công vật lý hoặc phép thuật bất kỳ
            characteristic.HealthRecovery = 0.01f * characteristic.Vitality * characteristic.Max_Health;// Restores health on the target unit over time – Tự động hồi 1 lượng HP nhất định sau mỗi turn
            characteristic.Reputation = 0;// (Fame): Số danh tiếng người chơi đang có(Cộng sau mỗi lần hoàn thành nhiệm vụ)
            characteristic.AttackRate = 0;// Increases the chance to do maximum damage – Tăng tỉ lệ ra đòn max damage
            characteristic.ParryRate = 0;// Increases the chance to take minimum damage – Tăng tỉ lệ nhận đòn min damage
            return characteristic;
        }


        public Characteristic()
        {

        }

        public Characteristic(CharacterType _typeChar, CharacterClass _classChar, float _lvl, float _str, float _intel, float _dex, float _focus, float _vit, float _agi, float _luck, float _endurance, float _blessing)
        {
            Type = _typeChar;
            Class = _classChar;
            Level = _lvl;

            Strength = _str ;
            Intelligence = _intel;
            Dexterity = _dex;
            Focus = _focus;
            Vitality = _vit;
            Agility = _agi;
            Luck = _luck;
            Endurance = _endurance;
            Blessing = _blessing;

            switch (_classChar) {
                case CharacterClass.Assassin:
                case CharacterClass.Paladin:
                    Damage = DamageType.Physic;
                    break;
                case CharacterClass.Wizard:
                    Damage = DamageType.Magic;
                    break;
                default:
                    Damage = DamageType.Magic;
                    break;
            }

            Max_Health = 1000 + Constants.HP_PER_TIMES[(int)this.Level - 1] + Constants.HP_PER_TIMES[(int)this.Vitality - (int)this.Level + 1];
            Health = Max_Health;
            //Physical Damage +1 %,
            //Physical Defense +0,1 %
            //Magical Damage + 1 %
            //Magical Defense + 0,1 %
            //Critical damage + 1 %
            //Multicast Chance + 1 %
            //Health + 10 %, (tra bảng HP)
            //Health Recovery +1 %
            //Dodge Chance + 1 %
            //Luck + 1
            //Damage reduces 1 %
            //Blessing + 1


            PhysicalDamage = Constants.DAMAGE_PER_TIMES[(int)Level - 1] + Constants.DAMAGE_PER_TIMES[(int)Strength - (int)Level + 1];// Increases the damage from physical attacks - thể hiện damage vật lý
            PhysicalReinforce = 0;// Increases the damage from physical attacks based on a percentage of physical damage – Tăng thêm damage vật lý theo tỉ lệ phần trăm của damage vật lý
            PhysicalDefense = Constants.DEFEND_PER_TIMES[(int)Level - 1] + Constants.DEFEND_PER_TIMES[(int)Strength - (int)Level + 1];// Take less damage from opponent's physical attack – Giảm lượng damage vật lý phải nhận
            MagicalDamage = Constants.DAMAGE_PER_TIMES[(int)Level - 1] + Constants.DAMAGE_PER_TIMES[(int)Intelligence - (int)Level + 1];// Increases the damage from Magical attacks - thể hiện damage phép thuật
            MagicalReinforce = 0;// Increases the damage from magical attacks based on a percentage of magical damage - Tăng thêm damage phép theo tỉ lệ phần trăm của damage phép
            MagicalDefense = Constants.DEFEND_PER_TIMES[(int)Level - 1] + Constants.DEFEND_PER_TIMES[(int)Intelligence - (int)Level + 1];// Take less damage from opponent's magical attack – Giảm lượng damage phép thuật phải nhận
            CriticalChance = 0;// Increases the chance to perform a critical attack - thể hiện tỉ lệ ra đòn chí mạng
            CriticalDamage = 0.01f * Dexterity;// Damage of the critical attacks – thể hiện lượng Sát thương cộng thêm khi người chơi ra đòn Chí mạng(mặc định tăng thêm 100% Sát thương vật lý)
            MulticastChance = 0.01f * Focus;// Increases the chance to perform a double casting spells - thể hiện tỉ lệ đánh 2 lần 1 phép bất kỳ
            BlockingChance = 0;// Increases the chance to block attacks with a shield - thể hiện tỉ lệ ngăn chặn hoàn toàn 1 đòn tấn công vật lý bất kỳ
            DodgeChance = 0.01f * Agility; //Increases the chance to dodge attacks - thể hiện tỉ lệ tránh né hoàn toàn 1 đòn tấn công vật lý hoặc phép thuật bất kỳ
            HealthRecovery = 0.01f * Vitality * Max_Health;// Restores health on the target unit over time – Tự động hồi 1 lượng HP nhất định sau mỗi turn
            Reputation = 0;// (Fame): Số danh tiếng người chơi đang có(Cộng sau mỗi lần hoàn thành nhiệm vụ)
            AttackRate = 0;// Increases the chance to do maximum damage – Tăng tỉ lệ ra đòn max damage
            ParryRate = 0;// Increases the chance to take minimum damage – Tăng tỉ lệ nhận đòn min damage
        }



        public byte[] convertToByteArr() {
            byte[] bytes = Utilities.convertToByteArr((byte)Type);
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((byte)Class));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((byte)Level));

            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Strength));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Intelligence));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Dexterity));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Focus));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Vitality));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Agility));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Luck));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Endurance));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Blessing));
            bytes = bytes.Concat<byte>(Utilities.convertToByteArr((ushort)Protection));
            
            return bytes;
        }
    }

}
