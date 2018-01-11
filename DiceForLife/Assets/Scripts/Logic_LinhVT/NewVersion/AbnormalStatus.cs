using System;
using System.Collections;

namespace CoreLib
{
    public class AbnormalStatus
    {

        public JSONNode data;

        public AbnormalStatus(JSONNode data)
        {
            this.data = data;
            
        }

        public string getName() {
            return data["name"].Value;
        }

        public int getDuration() {
            return Convert.ToInt32(data["duration"].Value);
        }



        public NewEffect affect(NewCharacterStatus attStatus, NewCharacterStatus defStatus) {// viet abnoraml status ngay
            //Console.WriteLine("affect Abnomral " + data.ToString());
            string name = data["name"].Value;
            int level = data["level"] == null? 0 : Convert.ToInt32(data["level"].Value);
            int duration = data["duration"] == null ? -1 : Convert.ToInt32(data["duration"].Value);
            string condition = "";
            string nick = data["nick"].Value;
            int playerID = -1;
            NewEffect effect = new NewEffect(attStatus.playerID, name,nick, level, 2 * duration, condition);
            bool me = false;
            bool you = false;

            JSONArray lines = data["lines"] != null ? (JSONArray)data["lines"] : null;
            if (lines != null)
            {
                foreach (JSONObject line in lines)
                {
                    // them duration
                    string delta = line["delta"].Value;
                    string index = line["index"].Value;
                    string con = line["condition"] == null ? "" : line["condition"].Value;
                    bool enemy = line["enemy"] == null ? false : line["enemy"].AsBool;
                    duration = line["duration"] == null ? -1 : Convert.ToInt32(line["duration"].Value);
                    AtomicEffect atomic = new AtomicEffect(effect, delta, index, enemy, con, duration);
                    NewCharacterStatus targetStatus = enemy? defStatus: attStatus;
                    Console.WriteLine("name:"+name+" delta:"+delta+" index:"+index+" enemy:"+enemy);
                    if (enemy) you = true;
                    else me = true;

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

                if (me && you) {
                    playerID = 3;
                    effect.playerID = playerID;
                    attStatus.replaceEffect(name, effect);
                    defStatus.replaceEffect(name, effect);
                } else if (me) { 
                    playerID = attStatus.playerID;
                    effect.playerID = playerID;
                    attStatus.replaceEffect(name, effect);
                } else if (you) {
                    playerID = defStatus.playerID;
                    effect.playerID = playerID;
                    defStatus.replaceEffect(name, effect);
                }


            }
            else {
                playerID = defStatus.playerID;
                effect.playerID = defStatus.playerID;
                defStatus.replaceEffect(name, effect);

            }
            

            //NewEffect clone = effect.clone();

            //ArrayList result = new ArrayList();
            //result.Add(effect);
            return effect;
        }

		public void addField(string field, object value)
		{
			if (value is string)
			{
				data.Add(field, new JSONString((string)value));
			}
			else if (value is int) data.Add(field, new JSONNumber((int)value));
			else if (value is float) data.Add(field, new JSONNumber((float)value));
			else if (value is bool) data.Add(field, new JSONBool((bool)value));
		}

        public AbnormalStatus clone()
        {
            JSONNode newData = JSON.Parse(data.ToString());
            return new AbnormalStatus(newData);
        }
    }
}
