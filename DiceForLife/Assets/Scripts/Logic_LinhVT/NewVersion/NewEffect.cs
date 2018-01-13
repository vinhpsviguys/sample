using System;
using System.Collections;
using UnityEngine;
//using UnityEngine;

namespace CoreLib
{
	class AtomicEffect
	{
        public NewEffect parent;
		public string delta;
		public string index;
		public bool enemy;
		public string condition;
		public bool enabled = true;
		public int duration; // dung de trong truong hop cac atomicnay cung co duration

        ArrayList atomicChildren = new ArrayList();

		public AtomicEffect(NewEffect parent, string delta, string index, bool enemy, string condition, int duration = -1)
		{
			this.parent = parent;
			this.delta = delta;
			this.index = index;
			this.enemy = enemy;
			this.condition = condition;
      
			this.duration = duration; // la round nen phai * 2
			if (condition != "") enabled = false;
            Console.WriteLine("create atomic with condition"+ condition+" "+enabled);
		}


		public bool isActive()
		{
			return duration != 0 && (condition == "" ? true : enabled);
		}

		public void decDuration()
		{
            if (duration > 0)
			this.duration = Math.Max(0, duration - 1);
		}

	}

	public class NewEffect // Skill + Abnormal Status
	{
		public const string Impotent = "impotent";
		public const string Hypnotic = "hypnotic";
		public const string Rot = "rot";
		public const string Pain = "pain";
		public const string Bleed = "bleed";
		public const string Crazy = "crazy";
		public const string Dull = "dull";
		public const string Stun = "stun"; //
		public const string Frostbite = "frostbite"; //
		public const string Burn = "burn"; //
		public const string Shock = "shock";
		public const string Poisoning = "poisoning"; //
		public const string Knockback = "knockback";
		public const string Immobilization = "immobilization";
		public const string Blind = "blind";
		public const string Freezing = "freezing"; //
		public const string Glamour = "glamour";
		public const string Fear = "fear"; //
		public const string Disease = "disease";
		public const string Sleep = "sleep"; //
        // Loai cam hoi mau cua Skill78
        // Lam lai glamour
        // lam lai bleed
        // HealthChanges
        // HealthRecovery
        //HealthRecovery_Glamour
        // HealthAdd
        // HealthAdd_Glamour
        // HealthSub


		public ArrayList atomicEffects = new ArrayList(); // link den cac noi

        public int originID { get; set; }

        public int playerID { get; set; } // 3 la tac dung voi ca 2 nguoi
		public string name;
		int level;
		public int duration { get; set; }
		public string condition;

		private MyDictionary<string, object> indexes = new MyDictionary<string, object>();
        public string nick;

        public NewEffect(int originID, string name,string nick = "", int level = 0, int duration = -1, string condition = "")
		{
            this.originID = originID;
			this.name = name;
            this.nick = nick;
			this.level = level;
           
			this.duration = duration;// do thuc chat truyen vao la round
			this.condition = condition;
		}


        public void parseAtomics(NewCharacterStatus me, NewCharacterStatus you) {
            for (int i = 0; i < atomicEffects.Count; i++) {
                
                ArrayList atomics = parseAtomic((AtomicEffect)atomicEffects[i], me, you);
                if (atomics != null) {// parse ra
                    AtomicEffect removeA = (AtomicEffect)atomicEffects[i]; 
                    atomicEffects.RemoveAt(i);
                    i--;
                    // remove Atomics
                    // remove condition
                    NewCharacterStatus target = removeA.enemy ? you : me;
                    target.atomics[removeA.index].Remove(removeA);
                    if (removeA.condition != "") 
                        target.enableAtomics[removeA.condition].Remove(removeA);
                    // add Atomics
                    // Add condition
                    foreach (AtomicEffect atomic in atomics) {
                        target = atomic.enemy ? you : me;
                        ArrayList list = target.atomics.ContainsKey(atomic.index) ? target.atomics[atomic.index] : new ArrayList();
                        list.Add(atomic);
                        target.atomics.Add(atomic.index, list);
                        if (atomic.condition != "") {
                            list = target.enableAtomics.ContainsKey(atomic.condition) ? target.enableAtomics[atomic.condition] : new ArrayList();
                            list.Add(atomic);
                            target.enableAtomics.Add(atomic.condition, list);
                        }

                    }

                    atomicEffects.AddRange(atomics);
                    //foreach (AtomicEffect atomic in atomics) {
                    //    Console.WriteLine("parse  to Atomic Skill "+atomic.index+" "+atomic.delta+" "+atomic.condition);
                    //}
                }

            }

            //foreach (string key in me.enableAtomics.Keys) {
            //    Console.WriteLine("me "+key); 
            //}

            //foreach (string key in you.enableAtomics.Keys)
            //{
            //    Console.WriteLine("you "+key); 
            //}


        }

        private ArrayList parseAtomic(AtomicEffect atomic, NewCharacterStatus me, NewCharacterStatus you) {
            switch (atomic.index) {
                case "allaps":
                    MyDictionary<string, NewSkill> skills = me.character.newSkillDic;
                    ArrayList list = new ArrayList();
                    foreach (string skill in skills.Keys) {
                        string index = atomic.index;
                        string delta = atomic.delta;
                        index = index.Replace("allaps", skill+"_aps");
                        delta = delta.Replace("allaps", skill+"_aps");
                        Debug.Log(atomic.index+" "+atomic.index+" "+atomic.delta + " "+index+" "+delta);
                        AtomicEffect newAtomic = new AtomicEffect((NewEffect)atomic.parent, delta, index, atomic.enemy, atomic.condition, atomic.duration);
                        list.Add(newAtomic);
                    }
                    return list;
                default:
                    return null;
            }
        }


		public NewEffect clone()
		{
            return new NewEffect(originID, name, nick, level, duration, condition);
		}

//		byte[] bytes = { 130, 200, 234, 23 }; // A byte array contains non-ASCII (or non-readable) characters

//		string s1 = Encoding.UTF8.GetString(bytes); // ���
//		byte[] decBytes1 = Encoding.UTF8.GetBytes(s1);  // decBytes1.Length == 10 !!
//														// decBytes1 not same as bytes
//														// Using UTF-8 or other Encoding object will get similar results

//		string s2 = BitConverter.ToString(bytes);   // 82-C8-EA-17
//		String[] tempAry = s2.Split('-');
//		byte[] decBytes2 = new byte[tempAry.Length];
//for (int i = 0; i<tempAry.Length; i++)
//    decBytes2[i] = Convert.ToByte(tempAry[i], 16);
//// decBytes2 same as bytes

//string s3 = Convert.ToBase64String(bytes);  // gsjqFw==
		//byte[] decByte3 = Convert.FromBase64String(s3);
		//// decByte3 same as bytes

		//string s4 = HttpServerUtility.UrlTokenEncode(bytes);    // gsjqFw2
		//byte[] decBytes4 = HttpServerUtility.UrlTokenDecode(s4);
		// decBytes4 same as bytes

        public void decDurationOfAtomics() {
            foreach (AtomicEffect atomic in atomicEffects) {
                if (atomic.isActive())
                    atomic.decDuration();
            }
        }

		public String convertToString() {
            // nick 2 byte
            // playerID 1 byte
            // level 1 sbyte
            // duration 1 sbyte
            // truyen duration cua cac atomic toi da 3 byte
            // truyen cac chi so them vao //loai chi so 1 byte + tuy chinh do loai chi so
            //ArrayList bytes = new ArrayList();
            byte[] bytes = Utilities.convertToByteArr(ConvertTypeToNumber());
            //foreach (byte by in bytes)
            //{
            //    bytes.Add(by);
            //}

            byte originID = (byte)this.originID;

            bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(originID));

            byte playerID = (byte)this.playerID;

            bytes = Utilities.Concat<byte>( bytes,  Utilities.convertToByteArr(playerID));
            

            if (ConvertTypeToNumber() < 1000)
            {
                sbyte level = (sbyte)this.level;
                bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(level));
            }
            if (ConvertTypeToNumber() < 2000)
            {
                sbyte duration = (sbyte)this.duration;
                bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(duration));
                
            }
            

            foreach (AtomicEffect atomic in this.atomicEffects)
            {
                sbyte duration = (sbyte)this.duration;
                bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(duration));
                
            }
            foreach (String key in indexes.Keys)
            {
                bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(indexes[key]));
                
            }

			byte[] length = Utilities.convertToByteArr((byte)bytes.Length);
			bytes = Utilities.Concat<byte>(length, bytes);
            
            return Convert.ToBase64String(bytes);
        }

        public byte[] convertToByteArr() {
            byte[] bytes = Utilities.convertToByteArr(ConvertTypeToNumber());
            //foreach (byte by in bytes)
            //{
            //    bytes.Add(by);
            //}

            Console.WriteLine("type convert byte arr"+bytes.Length);

            byte originID = (byte)this.originID;

            bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(originID));

            byte playerID = (byte)this.playerID;

			bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(playerID));
            Console.WriteLine("playerID convert byte arr" + bytes.Length);

            if (ConvertTypeToNumber() < 1000)
            {
				sbyte level = (sbyte)this.level;
				bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(level));
                Console.WriteLine("level convert byte arr" + bytes.Length);
            }
            if (ConvertTypeToNumber() < 2000)
            {
				sbyte duration = (sbyte)this.duration;
				bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(duration));
                Console.WriteLine("duration convert byte arr" + bytes.Length);

            }


            foreach (AtomicEffect atomic in this.atomicEffects)
            {
				sbyte duration = (sbyte)this.duration;
				bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(duration));
                Console.WriteLine("atomic duration convert byte arr" + bytes.Length);

            }
            foreach (String key in indexes.Keys)
            {
                bytes = Utilities.Concat<byte>(bytes, Utilities.convertToByteArr(indexes[key]));
                Console.WriteLine("bonus indexes byte arr" + bytes.Length);

            }

            byte[] length = Utilities.convertToByteArr(((byte)bytes.Length));
            bytes = Utilities.Concat<byte>(length, bytes);
            Console.WriteLine("Convert effect "+toJSON().ToString()+" to "+bytes.Length+ " bytes");

            return bytes;
        }

        public static NewEffect parseBytes(byte[] bytes) {
            bytes = Utilities.SubArray<byte>(bytes, 1, bytes.Length - 1); // loai bo byte file do dai day byte
            Console.WriteLine("bytes in NewEffect"+bytes.Length);
			NewEffect effect = null;
			int byte_shift = 0;
			int sub = 0;
			byte[] part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(ushort));
			ushort type = BitConverter.ToUInt16(part, 0);
			byte_shift += sizeof(ushort);


			string name = "";
			string nick = "";
            part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
            byte_shift += sizeof(byte);
            int originID = part[0];

            part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
			byte_shift += sizeof(byte);
            int playerID = part[0];
			if (type < 1000) // SKill
			{
				NewSkill skill = Adapter.skills["Skill" + type];
				nick = "Skill" + type;
				name = skill.data["name"].Value;
				// doc tiep
				part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
				byte_shift += sizeof(byte);
                int level = (sbyte)part[0];
				// duration
				part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
				byte_shift += sizeof(byte);
				int duration = (sbyte)part[0];
				// doc tiep cac atomic

				string condition = skill.data["condition"] == null ? "" : skill.data["condition"].Value;
				effect = new NewEffect(originID, name, nick, level, duration, condition);

				JSONNode data = skill.data;

				JSONArray lines = (JSONArray)data["lines"];

				foreach (JSONObject line in lines)
				{

					part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
					byte_shift += sizeof(byte);
					duration = (sbyte)part[0];

					string delta = line["delta"].Value;
					string index = line["index"].Value;
					string con = line["condition"] == null ? "" : line["condition"].Value;
					bool enemy = line["enemy"] == null ? false : Convert.ToBoolean(line["enemy"].Value);
					AtomicEffect atomic = new AtomicEffect(effect, delta, index, enemy, con, duration);
					effect.atomicEffects.Add(atomic);
				}

				effect.playerID = playerID;
			}
			else if (type < 2000) // AS
			{
				sub = type - 1000;
                Console.WriteLine("parse Type AS"+sub);
                //Debug.Log("Find AS thu " + sub + " " + Adapter.abs.ContainsKey("AS" + sub));
                AbnormalStatus ab = Adapter.abs["AS" + sub];
				nick = "AS" + sub;
				name = ab.data["name"].Value;

				part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
				byte_shift += sizeof(byte);
				int duration = (sbyte)part[0];
				effect = new NewEffect(originID, name, nick, -1, duration);
				JSONNode data = ab.data;

				JSONArray lines = data["line"] != null ? (JSONArray)data["lines"] : null;
				if (lines != null)
				{
					foreach (JSONObject line in lines)
					{
						part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
						byte_shift += sizeof(byte);
						duration = (sbyte)part[0];
						string delta = line["delta"].Value;
						string index = line["index"].Value;
						string con = line["condition"] == null ? "" : line["condition"].Value;
						bool enemy = true;
						AtomicEffect atomic = new AtomicEffect(effect, delta, index, enemy, con, duration);
						effect.atomicEffects.Add(atomic);
					}
				}
				switch (name)
				{
					case Burn:
						part = Utilities.SubArray<byte>(bytes, byte_shift, bytes.Length - byte_shift);
						effect.indexes.Add("burn_damage", BitConverter.ToInt32(part, 0));
						break;
					case Poisoning:
						part = Utilities.SubArray<byte>(bytes, byte_shift, bytes.Length - byte_shift);
						effect.indexes.Add("poison_damage", BitConverter.ToInt32(part, 0));
						break;
				}

				effect.playerID = playerID;
			}
			else
			{
				sub = type - 2000;

                // HealthChanges
        // HealthRecovery
        //HealthRecovery_Glamour
        // HealthAdd
        // HealthAdd_Glamour
        // HealthSub

				switch (sub)
				{
					case 1:
                        effect = new NewEffect(originID, "HealthChanges");
						break;
					case 2:
						effect = new NewEffect(originID, "Blocking");

						break;
					case 3:
						effect = new NewEffect(originID, "Dodge");

						break;
					case 4:
						effect = new NewEffect(originID, "Critical");

						break;
					case 5:
						effect = new NewEffect(originID, "ReturnDamage");
						break;
                    case 6:
                        effect = new NewEffect(originID, "Multicast");
                        break;
                    case 7:
                        effect = new NewEffect(originID, "RemoveAllAbnormal");
                        break;
                    case 8:
                        effect = new NewEffect(originID, "DoubleMagic");
                        break;
                    case 9:
                        effect = new NewEffect(originID, "HealthRecovery");
                        break;
                    case 10:
                        effect = new NewEffect(originID, "HealthRecovery_Glamour");
                        break;
                    default:
                        if (sub < 4000) {// HealthAdd_Skill
                            effect = new NewEffect(originID, "HealthAdd_Skill"+(sub - 3000));
                        } else if (sub < 5000) {// HealthAdd_Glamour_Skill
                            effect = new NewEffect(originID, "HealthAdd_Glamour_Skill" + (sub - 4000));
                        } else if (sub < 6000) {
                            effect = new NewEffect(originID, "HealthSub_Skill" + (sub - 5000));
                        }
                        break;
				}
				effect.playerID = playerID;
			}


			return effect;
        }

        public NewEffect parseString(string base64string) // Convert ra NewEffect chua ket noi voi cac Status
        {
            NewEffect effect = null;
            byte[] bytes = Convert.FromBase64String(base64string);
			bytes = Utilities.SubArray<byte>(bytes, 1, bytes.Length - 1); // loai bo byte file do dai day byte

			
			int byte_shift = 0;
			int sub = 0;
			byte[] part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(ushort));
			ushort type = BitConverter.ToUInt16(part, 0);
			byte_shift += sizeof(ushort);

            part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
            byte_shift += sizeof(byte);
            int originID = part[0];

            string name = "";
			string nick = "";
			part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
			byte_shift += sizeof(byte);
			int playerID = part[0];
			if (type < 1000) // SKill
			{
				NewSkill skill = Adapter.skills["Skill" + type];
				nick = "Skill" + type;
				name = skill.data["name"].Value;
				// doc tiep
				part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
				byte_shift += sizeof(byte);
				int level = (sbyte)part[0];
				// duration
				part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
				byte_shift += sizeof(byte);
				int duration = (sbyte)part[0];
				// doc tiep cac atomic

				string condition = skill.data["condition"] == null ? "" : skill.data["condition"].Value;
				effect = new NewEffect(originID, name, nick, level, duration, condition);

				JSONNode data = skill.data;

				JSONArray lines = (JSONArray)data["lines"];

				foreach (JSONObject line in lines)
				{

					part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
					byte_shift += sizeof(byte);
					duration = (sbyte)part[0];

					string delta = line["delta"].Value;
					string index = line["index"].Value;
					string con = line["condition"] == null ? "" : line["condition"].Value;
					bool enemy = line["enemy"] == null ? false : Convert.ToBoolean(line["enemy"].Value);
					AtomicEffect atomic = new AtomicEffect(effect, delta, index, enemy, con, duration);
					effect.atomicEffects.Add(atomic);
				}

				effect.playerID = playerID;
			}
			else if (type < 2000) // AS
			{
				sub = type - 1000;
				AbnormalStatus ab = Adapter.abs["AS" + type];
				nick = "AS" + type;
				name = ab.data["name"].Value;

				part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
				byte_shift += sizeof(byte);
				int duration = (sbyte)part[0];
				effect = new NewEffect(originID, name, nick, -1, duration);
				JSONNode data = ab.data;

				JSONArray lines = data["line"] != null ? (JSONArray)data["lines"] : null;
				if (lines != null)
				{
					foreach (JSONObject line in lines)
					{
						part = Utilities.SubArray<byte>(bytes, byte_shift, sizeof(byte));
						byte_shift += sizeof(byte);
						duration = (sbyte)part[0];
						string delta = line["delta"].Value;
						string index = line["index"].Value;
						string con = line["condition"] == null ? "" : line["condition"].Value;
						bool enemy = true;
						AtomicEffect atomic = new AtomicEffect(effect, delta, index, enemy, con, duration);
						effect.atomicEffects.Add(atomic);
					}
				}
				switch (name)
				{
					case Burn:
						part = Utilities.SubArray<byte>(bytes, byte_shift, bytes.Length - byte_shift);
						effect.indexes.Add("burn_damage", BitConverter.ToInt32(part, 0));
						break;
					case Poisoning:
						part = Utilities.SubArray<byte>(bytes, byte_shift, bytes.Length - byte_shift);
						effect.indexes.Add("poison_damage", BitConverter.ToInt32(part, 0));
						break;
				}

				effect.playerID = playerID;
			}
			else
			{
				sub = type - 2000;
				switch (sub)
				{
					case 1:
						effect = new NewEffect(originID,"HealthChanges");
						break;
					case 2:
						effect = new NewEffect(originID, "Blocking");

						break;
					case 3:
						effect = new NewEffect(originID, "Dodge");

						break;
					case 4:
						effect = new NewEffect(originID, "Critical");

						break;
					case 5:
						effect = new NewEffect(originID, "ReturnDamage");
						break;
                    case 6:
                        effect = new NewEffect(originID, "Multicast");
                        break;
                    case 7:
                        effect = new NewEffect(originID, "RemoveAllAbnormal");
                        break;
                    case 8:
                        effect = new NewEffect(originID, "DoubleMagic");
                        break;
                    case 9:
                        effect = new NewEffect(originID, "HealthRecovery");
                        break;
                    case 10:
                        effect = new NewEffect(originID, "HealthRecovery_Glamour");
                        break;
                    default:
                        if (sub < 4000)
                        {// HealthAdd_Skill
                            effect = new NewEffect(originID, "HealthAdd_Skill" + (sub - 3000));
                        }
                        else if (sub < 5000)
                        {// HealthAdd_Glamour_Skill
                            effect = new NewEffect(originID, "HealthAdd_Glamour_Skill" + (sub - 4000));
                        }
                        else if (sub < 6000)
                        {
                            effect = new NewEffect(originID, "HealthSub_Skill" + (sub - 5000));
                        }
                        break;

                }
				effect.playerID = playerID;
			}


            return effect;
        }

        public void decDuration() {
            if (duration > 0)
                duration = Math.Max(duration - 1, 0);
        }

        public ushort ConvertTypeToNumber() {
            if (nick.Contains("Skill")) {
                string[] parts = nick.Split(new string[] { "Skill"}, StringSplitOptions.RemoveEmptyEntries);
                return Convert.ToUInt16(parts[0]);
            } else if (nick.Contains("AS")) {
                string[] parts = nick.Split(new string[] { "AS" }, StringSplitOptions.RemoveEmptyEntries);
                ushort sub = 1000;
                return (ushort)(sub + Convert.ToUInt16(parts[0]));
            } else {
                ushort sub = 2000;
                switch (name) {
                    case "HealthChanges":
                        sub+=1;
                        break;
                    case "Blocking":
                        sub+=2;
                        break;
                    case "Dodge":
                        sub+=3;
                        break;
                    case "Critical":
                        sub+=4;
                        break;
                    case "ReturnDamage":
                        sub+=5;
                        break;
                    case "Multicast":
                        sub += 6;
                        break;
                    case "RemoveAllAbnormal":
                        sub += 7;
                        break;
                    case "DoubleMagic":
                        sub += 8;
                        break;
                    case "HealthRecovery":
                        sub += 9;
                        break;
                    case "HealthRecovery_Glamour":
                        sub += 10;
                        break;
                    default:
                        if (name.Contains("HealthAdd_Skill")) {
                            string[] indexes = name.Split(new string[] {"HealthAdd_Skill"}, StringSplitOptions.RemoveEmptyEntries);
                            sub = (ushort)(3000 + int.Parse(indexes[0]));
                        } else if (name.Contains("HealthAdd_Glamour_Skill")) {
                            string[] indexes = name.Split(new string[] { "HealthAdd_Glamour_Skill" }, StringSplitOptions.RemoveEmptyEntries);
                            sub = (ushort)(4000 + int.Parse(indexes[0]));
                        } else if (name.Contains("HealthSub_Skill")) {
                            string[] indexes = name.Split(new string[] { "HealthSub_Skill" }, StringSplitOptions.RemoveEmptyEntries);
                            sub = (ushort)(5000 + int.Parse(indexes[0]));
                        }
                        break;
                }
                return sub;
            }
        }

		public JSONNode toJSON()
		{
			JSONObject obj = new JSONObject();
			obj.Add("name", new JSONString(name));
			obj.Add("level", new JSONNumber(level));
			obj.Add("duration", new JSONNumber(duration));
			obj.Add("condition", new JSONString(condition));
			obj.Add("playerID", new JSONNumber(playerID));
			foreach (string index in indexes.Keys)
			{
				object v = indexes[index];
				if (v is int)
				{
					obj.Add(index, new JSONNumber((int)v));
				}
				else if (v is float)
				{
					obj.Add(index, new JSONNumber((float)v));
				}
				else if (v is string)
				{
					obj.Add(index, new JSONString((string)v));
				}
			}
			return obj;
		}

		public void setIndex(string name, object v)
		{
			indexes.Add(name, v);
		}

		public object getIndex(string name, object v)
		{
			return indexes.ContainsKey(name) ? indexes[name] : v;
		}



		// SKill 2 byte = 1 bit phan biet + 4 bit level + 3 bit duration + 8 bit danh so skill
		// Abnormal 2 byte = 1 bit phan biet + 5 bit so thu tu status + 3 bit duration

	}


}
