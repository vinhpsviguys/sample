using System;
using System.Collections.Generic;



namespace CoreLib
{
    public class Equipment
    {

        Random random = new Random();
        public MyDictionary<string, object> dynamicIndexes = new MyDictionary<string, object>();
        public MyDictionary<string, object> staticIndexes = new MyDictionary<string, object>();

        public JSONNode toJSON()
        {
            // name : JSONString
            // properties: JSONArray[JSONObject{name : JSONString, type: JSONString}]
            // ten chi so : JSONObject[min: JSONNumber, max: JSonNumber]

            JSONNode objet = new JSONObject();
            JSONArray properties = new JSONArray();// danh sach cac dong trong trang bi



            foreach (string key in dynamicIndexes.Keys)
            {

                object value = dynamicIndexes[key];


                if (value is Range<int>)
                {
                    JSONObject node = new JSONObject();
                    node.Add("name", key);
                    Range<int> num = (Range<int>)value;
                    JSONObject element = new JSONObject();
                    element.Add("min", new JSONNumber(num.Minimum));
                    element.Add("max", new JSONNumber(num.Maximum));
                    objet.Add(key, element);
                    node.Add("type", "range int");
                    properties.Add(node);
                }
                else if (value is Range<float>)
                {
                    JSONObject node = new JSONObject();
                    node.Add("name", key);
                    JSONObject element = new JSONObject();
                    Range<float> num = (Range<float>)value;
                    element.Add("min", new JSONNumber(num.Minimum));
                    element.Add("max", new JSONNumber(num.Maximum));
                    objet.Add(key, element);
                    node.Add("type", "range float");
                    properties.Add(node);
                }

            }
            objet.Add("dynamicProperties", properties);

            properties = new JSONArray();
            foreach (string key in staticIndexes.Keys)
            {
                object value = staticIndexes[key];


                if (value is string)
                {
                    JSONObject node = new JSONObject();
                    node.Add("name", key);
                    objet.Add(key, new JSONString((string)value));
                    node.Add("type", "string");
                    properties.Add(node);
                }
                else if (value is int)
                {
                    JSONObject node = new JSONObject();
                    node.Add("name", key);
                    objet.Add(key, new JSONNumber((int)value));
                    node.Add("type", "int");
                    properties.Add(node);
                }
                else if (value is float)
                {
                    JSONObject node = new JSONObject();
                    node.Add("name", key);
                    objet.Add(key, new JSONNumber((float)value));
                    node.Add("type", "float");
                    properties.Add(node);
                }
                else
                {
                    if (value is TypeEquipmentCharacter)
                    {
                        JSONObject node = new JSONObject();
                        node.Add("name", key);
                        objet.Add(key, new JSONString(value.ToString()));
                        node.Add("type", "enum TypeEquipmentCharacter");
                        properties.Add(node);
                    }
                    else if (value is ClassCharacterItem)
                    {
                        JSONObject node = new JSONObject();
                        node.Add("name", key);
                        objet.Add(key, new JSONString(value.ToString()));
                        node.Add("type", "enum ClassCharacterItem");
                        properties.Add(node);
                    }
                    else if (value is RarelyItem)
                    {
                        JSONObject node = new JSONObject();
                        node.Add("name", key);
                        objet.Add(key, new JSONString(value.ToString()));
                        node.Add("type", "enum RarelyItem");
                        properties.Add(node);
                    }
                }

            }

            objet.Add("staticProperties", properties);

            return objet;
        }

        public static Equipment parseJSON(string aJSON)
        {
            JSONNode obj = JSON.Parse(aJSON);
            Equipment equipment = new Equipment();
            //equipment.dynamicIndexes.Add("name", obj["name"].Value);
            JSONArray properties = (JSONArray)obj["staticProperties"];
            foreach (JSONNode node in properties)
            {
                string property = node["name"].Value;
                string type = node["type"].Value;
                JSONNode infos = obj[property];
                switch (type)
                {
                    case "string":
                        equipment.staticIndexes.Add(property, infos.Value);
                        break;
                    case "int":
                        equipment.staticIndexes.Add(property, infos.AsInt);
                        break;
                    case "float":
                        equipment.staticIndexes.Add(property, infos.AsFloat);
                        break;
                    case "enum TypeEquipmentCharacter":
                        equipment.staticIndexes.Add(property, Utilities.ParseEnum<TypeEquipmentCharacter>(infos.Value));
                        break;
                    case "enum ClassCharacterItem":
                        equipment.staticIndexes.Add(property, Utilities.ParseEnum<ClassCharacterItem>(infos.Value));
                        break;
                    case "enum RarelyItem":
                        equipment.staticIndexes.Add(property, Utilities.ParseEnum<RarelyItem>(infos.Value));
                        break;
                }
            }

            properties = (JSONArray)obj["dynamicProperties"];
            foreach (JSONNode node in properties)
            {
                string property = node["name"].Value;
                string type = node["type"].Value;
                JSONNode infos = obj[property];
                switch (type)
                {
                    case "range int":
                        equipment.dynamicIndexes.Add(property, new Range<int>(Convert.ToInt32(infos["min"].Value), Convert.ToInt32(infos["max"].Value)));
                        break;
                    case "range float":
                        equipment.dynamicIndexes.Add(property, new Range<float>(Convert.ToSingle(infos["min"].Value), Convert.ToSingle(infos["max"].Value)));
                        break;
                }
            }

            return equipment;
        }


        public float getPhysicalDamage(float attackRate, float parryRate)
        {


            float ratio = 0;
            if (attackRate == 0)
            {
                ratio = Math.Max(-1, Math.Min(1, attackRate - parryRate));
            }
            else ratio = (attackRate - parryRate) / attackRate;
            if (dynamicIndexes.ContainsKey(Indexes.pda_na))
            {
                object value = dynamicIndexes[Indexes.pda_na];
                float max = 0;
                float min = 0;
                if (value is Range<int>)
                {
                    Range<int> num = (Range<int>)value;
                    max = num.Maximum;
                    min = num.Minimum;
                }
                else if (value is Range<float>)
                {
                    Range<float> num = (Range<float>)value;
                    max = num.Maximum;
                    min = num.Minimum;
                }

                if (ratio < 0)
                {
                    max = (max - min) * ratio + max;
                }
                else
                {
                    min = (max - min) * ratio + min;
                }

                return (float)(random.NextDouble() * (max - min) + min);
            }

            return 0;
        }

        public float getMagicalDamage(float attackRate, float parryRate)
        {
            float ratio = 0;
            if (attackRate == 0)
            {
                ratio = Math.Max(-1, Math.Min(1, attackRate - parryRate));
            }
            else ratio = (attackRate - parryRate) / attackRate;
            if (dynamicIndexes.ContainsKey(Indexes.mda_na))
            {
                object value = dynamicIndexes[Indexes.mda_na];
                float max = 0;
                float min = 0;
                if (value is Range<int>)
                {
                    Range<int> num = (Range<int>)value;
                    max = num.Maximum;
                    min = num.Minimum;
                }
                else if (value is Range<float>)
                {
                    Range<float> num = (Range<float>)value;
                    max = num.Maximum;
                    min = num.Minimum;
                }

                if (ratio < 0)
                {
                    max = (max - min) * ratio + max;
                }
                else
                {
                    min = (max - min) * ratio + min;
                }

                return (float)(random.NextDouble() * (max - min) + min);
            }

            return 0;
        }




        public static Equipment[] createEquipments()
        {
            Random random = new Random();
            Equipment[] equiptments = new Equipment[12];
            for (int i = 0; i < 10; i++)
            {
                if (random.Next(2) == 0 && i > 0) continue;
                Equipment equipment = null;
                switch (i)
                {
                    case 0:
                        equipment = createWeapon();
                        break;
                    case 1:
                        equipment = createShield();
                        break;
                    case 2:
                        equipment = createHelmet();
                        break;
                    case 3:
                        equipment = createCape();
                        break;
                    case 4:
                        equipment = createPants();
                        break;
                    case 5:
                        equipment = createBelt();
                        break;
                    case 6:
                        equipment = createGloves();
                        break;
                    case 7:
                        equipment = createBoots();
                        break;
                    case 8:
                        equipment = createRing();
                        break;
                    case 9:
                        equipment = createNecklace();
                        break;
                }
                equiptments[i] = equipment;
            }
            return equiptments;
        }

        private static Equipment createWeapon()
        {
            Equipment equipment = new Equipment();
            // kiem dai
            //Physical damage, Magical damage, Critical chance, Attack rate, Physical Reinforce, Magical Reinforce

            //13.7    14.9    35.4    38.5    19.2 % 25.8 % 22.5 % 19.2 % 25.8 % 22.5 % 25.0    28.0
            equipment.staticIndexes.Add("name", "weapon");
            equipment.setValue(Indexes.pda_na, new CoreLib.Range<float>(70.8f, 85.9f));
            equipment.setValue(Indexes.mda_na, new CoreLib.Range<float>(70.8f, 85.9f));
            equipment.staticIndexes.Add(Indexes.cri_cha_na, 0.02f);

            equipment.setValue(Indexes.atr_na, new CoreLib.Range<float>(19.3f, 28.9f));

            equipment.setValue(Indexes.pre_na, new CoreLib.Range<float>(10.00f / 100, 15.00f / 100));
            equipment.setValue(Indexes.mre_na, new CoreLib.Range<float>(0, 0));
            return equipment;
        }

        private static Equipment createShield()
        {
            Equipment equipment = new Equipment();

            equipment.staticIndexes.Add("name", "shield");
            //Physical defense, Magical defense, Block chance, Physical Reinforce, Magical Reinforce
            //13.7    14.9    35.4    38.5    19.2 % 25.8 % 22.5 % 19.2 % 25.8 % 22.5 % 25.0    28.0
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(3.7f, 4.4f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.block_cha_na, new CoreLib.Range<float>(0.01f, 0.02f));

            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.58f / 100, 2.36f / 100));

            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.58f / 100, 2.36f / 100));
            return equipment;
        }

        private static Equipment createHelmet() // mu
        {
            Equipment equipment = new Equipment();
            equipment.staticIndexes.Add("name", "helmet");
            // Physical defense, Magical defense, Physical Reinforce, Magical Reinforce
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(3.2f, 3.9f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.38f / 100, 2.07f / 100));
            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.38f / 100, 2.07f / 100));
            equipment.setValue(Indexes.pryr_na, new CoreLib.Range<float>(4.2f, 6.3f));
            return equipment;
        }

        private static Equipment createCape()// ao
        {
            Equipment equipment = new Equipment();
            equipment.staticIndexes.Add("name", "cape");
            // Physical defense, Magical defense, Physical Reinforce, Magical Reinforce
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(3.5f, 4.2f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.50f / 100, 2.25f / 100));
            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.50f / 100, 2.25f / 100));
            equipment.setValue(Indexes.pryr_na, new CoreLib.Range<float>(4.6f, 6.9f));
            return equipment;
        }

        private static Equipment createPants()// quan
        {
            Equipment equipment = new Equipment();
            equipment.staticIndexes.Add("name", "pants");
            //Physical defense, Magical defense, Physical Reinforce, Magical Reinforce
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(3.4f, 4.0f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.44f / 100, 2.16f / 100));
            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.44f / 100, 2.16f / 100));
            equipment.setValue(Indexes.pryr_na, new CoreLib.Range<float>(4.4f, 6.6f));
            return equipment;
        }

        private static Equipment createBelt()
        {
            Equipment equipment = new Equipment();
            equipment.staticIndexes.Add("name", "belt");
            //Physical defense, Magical defense, Physical Reinforce, Magical Reinforce
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(3.1f, 3.7f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.32f / 100, 1.98f / 100));
            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.32f / 100, 1.98f / 100));
            equipment.setValue(Indexes.pryr_na, new CoreLib.Range<float>(4.0f, 6.1f));
            return equipment;
        }

        private static Equipment createGloves()
        {
            //Physical defense, Magical defense, Physical Reinforce, Magical Reinforce
            Equipment equipment = new Equipment();
            equipment.staticIndexes.Add("name", "gloves");
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(2.9f, 3.5f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.26f / 100, 1.89f / 100));
            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.26f / 100, 1.89f / 100));
            equipment.setValue(Indexes.pryr_na, new CoreLib.Range<float>(3.9f, 5.8f));
            return equipment;
        }

        private static Equipment createBoots()
        {
            Equipment equipment = new Equipment();
            //Physical defense, Magical defense, Physical Reinforce, Magical Reinforce
            equipment.staticIndexes.Add("name", "boots");
            equipment.setValue(Indexes.pde_na, new CoreLib.Range<float>(2.8f, 3.4f));
            equipment.setValue(Indexes.mde_na, new CoreLib.Range<float>(0, 0));
            equipment.setValue(Indexes.prd_na, new CoreLib.Range<float>(1.20f / 100, 1.80f / 100));
            equipment.setValue(Indexes.mrd_na, new CoreLib.Range<float>(1.20f / 100, 1.80f / 100));
            equipment.setValue(Indexes.pryr_na, new CoreLib.Range<float>(3.7f, 5.5f));
            return equipment;
        }

        private static Equipment createRing()
        {
            Equipment equipment = new Equipment();
            ///Physiscs absorption, Magic absorption
            equipment.staticIndexes.Add("name", "ring");
            equipment.setValue(Indexes.pab_na, new CoreLib.Range<float>(0.60f / 100, 0.90f / 100));
            equipment.setValue(Indexes.mab_na, new CoreLib.Range<float>(0.69f / 100, 1.04f / 100));
            return equipment;
        }

        private static Equipment createNecklace()
        {
            Equipment equipment = new Equipment();
            ///Physiscs absorption, Magic absorption
            ///
            equipment.staticIndexes.Add("name", "necklace");
            equipment.setValue(Indexes.pab_na, new CoreLib.Range<float>(0.60f / 100, 0.90f / 100));
            equipment.setValue(Indexes.mab_na, new CoreLib.Range<float>(0.69f / 100, 1.04f / 100));
            return equipment;
        }

        public static EquipmentItem convertToEquipmentItem(Equipment equipment)
        {
            EquipmentItem item = new EquipmentItem();
            //item.attributeItems = new List<PropertiesBonus>();
            foreach (string key in equipment.dynamicIndexes.Keys)
            {
                object value = equipment.dynamicIndexes[key];
                Range<float> range = (Range<float>)value;
                float min = range.Minimum;
                String index = "min_" + key;
                string id = (int)Utilities.ParseEnum<ServerIndexes>(index) + "";
                item.indexes.Add(index, min + "");
                float max = range.Maximum;
                index = "max_" + key;
                id = (int)Utilities.ParseEnum<ServerIndexes>(index) + "";
                item.indexes.Add(index, max + "");
            }


            foreach (String key in equipment.staticIndexes.Keys)
            {

                switch (key)
                {
                    case "idItem":
                        item.idItem = (int)equipment.staticIndexes["idItem"];
                        break;
                    case "idItemInit":
                        item.idItemInit = (int)equipment.staticIndexes["idItemInit"];
                        break;
                    case "type":
                        item.typeItem = (TypeEquipmentCharacter)equipment.staticIndexes["type"];// TypeEquipmentCharacter.None);
                        break;
                    case "class":

                        item.classItem = (ClassCharacterItem)equipment.staticIndexes["class"];
                        break;
                    case "rarely":
                        item.rarelyItem = (int)equipment.staticIndexes["rarely"];
                        break;
                    case "idGroup":
                        item.idGroupSetItems = (int)equipment.staticIndexes["idGroup"];
                        break;
                    case "nameItem":
                        item.nameItem = (string)equipment.staticIndexes["nameItem"];
                        break;
                    case "levelRequired":

                        item.levelRequired = (int)equipment.staticIndexes["levelRequired"];
                        break;
                    case "levelUpgraded":
                        item.levelUpgraded = (int)equipment.staticIndexes["levelUpgraded"];
                        break;
                    case "valueItem":
                        item.valueItem = (float)equipment.staticIndexes["valueItem"];
                        break;
                    case "numberItem":
                        item.numberItem = (int)equipment.staticIndexes["numberItem"];

                        break;
                    case "name":
                        //item.indexes.Add(key, equipment.staticIndexes["name"]);
                        break;
                    default:
                        ServerIndexes index = Utilities.ParseEnum<ServerIndexes>(key);
                        if (index == null) continue;
                        string id = "" + (int)index;

                        string v = (float)equipment.staticIndexes[key] + "";

                        item.indexes.Add(id, v);
                        break;
                }
            }



            switch ((string)equipment.staticIndexes["name"])
            {


                case "necklace":
                    item.typeItem = TypeEquipmentCharacter.Amulet;
                    break;
                case "avatar":
                    item.typeItem = TypeEquipmentCharacter.Avatar;
                    break;
                case "belt":
                    item.typeItem = TypeEquipmentCharacter.Belt;
                    break;
                case "boots":
                    item.typeItem = TypeEquipmentCharacter.Boots;
                    break;

                case "gloves":
                    item.typeItem = TypeEquipmentCharacter.Gloves;
                    break;
                case "helmet":
                    item.typeItem = TypeEquipmentCharacter.Head;
                    break;

                case "pants":
                    //pants
                    item.typeItem = TypeEquipmentCharacter.Leg;
                    break;


                case "ring":
                    item.typeItem = TypeEquipmentCharacter.Ring;
                    break;


                case "shield":
                    item.typeItem = TypeEquipmentCharacter.Shield;
                    break;
                case "cape":
                    //cape
                    item.typeItem = TypeEquipmentCharacter.Torso;
                    break;

                case "weapon":
                    item.typeItem = TypeEquipmentCharacter.Weapon;
                    break;
            }


            return item;
        }



        public static Equipment convertToEquipment(EquipmentItem Item)
        {
            Equipment equipment = new Equipment();
            equipment.staticIndexes.Add("idItem", Item.idItem);
            equipment.staticIndexes.Add("idItemInit", Item.idItemInit);
            equipment.staticIndexes.Add("type", Item.typeItem);
            equipment.staticIndexes.Add("class", Item.classItem);
            equipment.staticIndexes.Add("rarely", Item.rarelyItem);
            equipment.staticIndexes.Add("idGroup", Item.idGroupSetItems);
            equipment.staticIndexes.Add("nameItem", Item.nameItem);
            equipment.staticIndexes.Add("levelRequired", Item.levelRequired);
            equipment.staticIndexes.Add("levelUpgraded", Item.levelUpgraded);
            equipment.staticIndexes.Add("valueItem", Item.valueItem);
            equipment.staticIndexes.Add("numberItem", Item.numberItem);

            switch (Item.typeItem)
            {

                case TypeEquipmentCharacter.Gem:
                    break;
                case TypeEquipmentCharacter.AlchemyMaterial:
                    break;
                case TypeEquipmentCharacter.Amulet:
                    equipment.staticIndexes.Add("name", "necklace");
                    break;
                case TypeEquipmentCharacter.Avatar:
                    equipment.staticIndexes.Add("name", "avatar");
                    break;
                case TypeEquipmentCharacter.Belt:
                    equipment.staticIndexes.Add("name", "belt");
                    break;
                case TypeEquipmentCharacter.Boots:
                    equipment.staticIndexes.Add("name", "boots");
                    break;
                case TypeEquipmentCharacter.Buff:
                    equipment.staticIndexes.Add("name", "book");
                    break;
                case TypeEquipmentCharacter.Gloves:
                    equipment.staticIndexes.Add("name", "gloves");
                    break;
                case TypeEquipmentCharacter.Head:
                    equipment.staticIndexes.Add("name", "helmet");
                    break;
                case TypeEquipmentCharacter.HPRecovery:
                    break;
                case TypeEquipmentCharacter.Leg:
                    //pants
                    equipment.staticIndexes.Add("name", "pants");
                    break;
                case TypeEquipmentCharacter.LuckMaterial:
                    break;
                case TypeEquipmentCharacter.Ring:
                    equipment.staticIndexes.Add("name", "ring");
                    break;
                case TypeEquipmentCharacter.Scroll:
                    break;
                case TypeEquipmentCharacter.Shield:
                    equipment.staticIndexes.Add("name", "shield");
                    break;
                case TypeEquipmentCharacter.Torso:
                    //cape
                    equipment.staticIndexes.Add("name", "cape");
                    break;
                case TypeEquipmentCharacter.VIPCard:
                    break;
                case TypeEquipmentCharacter.Weapon:
                    equipment.staticIndexes.Add("name", "weapon");
                    break;
                case TypeEquipmentCharacter.OffhandWeapon:
                    equipment.staticIndexes.Add("name", "weapon2");
                    break;
            }

            foreach (string key in Item.indexes.Keys)
            {
                if (key != "listidproperty")
                {
                    ServerIndexes index = Utilities.ParseEnum<ServerIndexes>(key);
                    String name = index.ToString();
                    string[] parts = name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    String prefix = parts[0];
                    if (prefix == "min")
                    {// chi so min cua dynamic
                        String property = "";
                        for (int i = 1; i < parts.Length; i++)
                            property += i == 1 ? parts[i] : "_" + parts[i];
                        Range<float> ranger = equipment.dynamicIndexes.ContainsKey(property) ? (Range<float>)equipment.dynamicIndexes[property] : new Range<float>();
                        equipment.dynamicIndexes.Add(property, ranger);
                        ranger.Minimum = Utilities.convertStringToFloat(String.Format("{0:0.00}", Item.indexes[key]));
                    }
                    else if (prefix == "max")
                    {// chi so max cuat dynamic
                        String property = "";
                        for (int i = 1; i < parts.Length; i++)
                            property += i == 1 ? parts[i] : "_" + parts[i];
                        Range<float> ranger = equipment.dynamicIndexes.ContainsKey(property) ? (Range<float>)equipment.dynamicIndexes[property] : new Range<float>();
                        equipment.dynamicIndexes.Add(property, ranger);
                        ranger.Maximum = Utilities.convertStringToFloat(String.Format("{0:0.00}", Item.indexes[key]));
                    }
                    else
                    {// chi so static
                        String property = name;
                        equipment.staticIndexes.Add(property, Utilities.convertStringToFloat(String.Format("{0:0.00}", Item.indexes[key])));
                    }
                }
            }

            return equipment;
        }

        public void setValue(string field, object value)
        {
            dynamicIndexes.Add(field, value);
        }

        public object getValue(string field)
        {
            if (dynamicIndexes.ContainsKey(field))
            {
                object value = dynamicIndexes[field];
                if (value is Range<int>)
                {
                    Range<int> num = (Range<int>)value;
                    return random.Next(num.Minimum, num.Maximum);
                }
                else if (value is Range<float>)
                {
                    Range<float> num = (Range<float>)value;
                    return (float)(random.NextDouble() * (num.Maximum - num.Minimum) + num.Minimum);
                }
                else if (value is float || value is int)
                {
                    return (value is float ? (float)value : (int)value);
                }
            }

            return (dynamicIndexes.ContainsKey(field) ? dynamicIndexes[field] : 0);
        }

        public object getValue(string field, object dvalue)
        {
            if (dynamicIndexes.ContainsKey(field))
            {
                object value = dynamicIndexes[field];
                if (value is Range<int>)
                {
                    Range<int> num = (Range<int>)value;
                    return random.Next(num.Minimum, num.Maximum);
                }
                else if (value is Range<float>)
                {
                    Range<float> num = (Range<float>)value;
                    return (float)(random.NextDouble() * (num.Maximum - num.Minimum) + num.Minimum);
                }
                else if (value is float || value is int)
                {
                    return (value is float ? (float)value : (int)value);
                }
            }

            return (dynamicIndexes.ContainsKey(field) ? dynamicIndexes[field] : dvalue);
        }

        public object getOriginalValue(string field, object dvalue)
        {
            return (dynamicIndexes.ContainsKey(field) ? dynamicIndexes[field] : dvalue);
        }


        public byte[] convertToByteArr()
        {
            // cac kieu la ushort
            // dynamic truoc
            //string prefix = staticIndexes["name"] as string;
            //byte[] bytes = null;
            //foreach (string key in dynamicIndexes.Keys) { 
            //    Range
            //}

            return null;
        }


    }
}