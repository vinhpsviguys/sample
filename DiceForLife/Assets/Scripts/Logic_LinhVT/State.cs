
using System;
using System.Collections;
using CoreLib;


namespace CoreLib
{
    class State
    {
        public int id = 0;
        public int attacker;
        public int defender;

        public float healthPlayer2;
        public float healthPlayer1;// float Số HP sau cùng khi bị kết thúc Step này(Đã được tính toán sau khi chịu các dị trạng)
        public float damage; //Số damage tổng cộng mà Target nhận trong Step này
        //public int recovery; // Số lượng hồi máu tổng cộng trong step này
        public ArrayList effects = new ArrayList();
        //effects Array of JSonObject { name<String>, id<int>, turns<int>
        //    }
        //    Name là tên dị trạng,
        //ID là mã số di trạng,
        //Turns là số turn còn lại của dị trạng 1..20
        public int actionpoints;



        public State(int attacker, int defender) {
            this.attacker = attacker;
            this.defender = defender;
        }

        public void setIdSkill(int id)
        {
            this.id = id;
        }

        public void setEffects(ArrayList effects) {
            this.effects = effects;
        }

        public byte[] convertToByteArr() {
            byte[] bytes = Utilities.convertToByteArr((ushort)id);
            bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((byte)attacker));
            bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((byte)defender));
            bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((int)healthPlayer1));
            bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr((int)healthPlayer2));
            foreach (NewEffect effect in effects)
            {
                bytes = Utilities.Concat<byte>(bytes, effect.convertToByteArr());
            }
            return bytes;
        }

        public static State parseBytes(byte[] bytes) {
            Console.WriteLine("parseBytes" + (bytes != null ? ""+bytes.Length: "null"));
            byte[] part = Utilities.SubArray<byte>(bytes, 0, sizeof(ushort) + 2 * sizeof(byte) + 2 * sizeof(int));
            int idSkill = BitConverter.ToUInt16(Utilities.SubArray<byte>(part, 0, sizeof(ushort)), 0);
            int attacker = bytes[0 + sizeof(ushort)];
            int defender = bytes[1 + sizeof(ushort)];
            int healthAtt = BitConverter.ToInt32(Utilities.SubArray<byte>(part, sizeof(ushort) + 2, sizeof(int)), 0);
            int healthDef = BitConverter.ToInt32(Utilities.SubArray<byte>(part, sizeof(ushort) + 2 + sizeof(int), sizeof(int)), 0);

            State state = new State(attacker, defender);
            state.healthPlayer1 = healthAtt;
            state.healthPlayer2 = healthDef;
            state.id = idSkill;

            int byte_shift = sizeof(ushort) + 2 * sizeof(byte) + 2 * sizeof(int);
            while (byte_shift < bytes.Length) {
                int length = bytes[byte_shift];
                part = Utilities.SubArray<byte>(bytes, byte_shift, length + 1);
                NewEffect effect = NewEffect.parseBytes(part);
                state.effects.Add(effect);
                byte_shift += 1 + length;
            }
            return state;                     
        }

        public void setHP(NewCharacterStatus one, NewCharacterStatus two) {
            healthPlayer1 = one.playerID == 1 ? (float)one.getMidIndex(Indexes.hp_na) : (float)two.getMidIndex(Indexes.hp_na);
            healthPlayer2 = one.playerID == 2 ? (float)one.getMidIndex(Indexes.hp_na) : (float)two.getMidIndex(Indexes.hp_na);
        }

        public JSONNode toJSON() {
            JSONNode obj = new JSONObject();
            obj.Add("idSkill", id);
            obj.Add("attacker", attacker);
            obj.Add("defender", defender);
            obj.Add("hpPlayer1", healthPlayer1);
            obj.Add("hpPlayer2", healthPlayer2);
            obj.Add("damage", damage);
            JSONArray arrays = new JSONArray();
            foreach (NewEffect effect in effects) {
                arrays.Add(effect.toJSON());
            }
            obj.Add("effects", arrays);
            return obj;
        }

    }
}
