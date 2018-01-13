using CoreLib;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private int typeItem;
    private string _tempValue;
    Dictionary<string, object> indexes = new Dictionary<string, object>();

    public Item()
    {
        /*"idht": "26",
        "idh": "25",
        "idcode": "ZLE4QQZ",
        "idit": "19",
        "name": "Special Lucky Marterial II",
        "quantity": "2",
        "sellprice": "1",
        "levelrequired": "1",
        "descripton": "Increases chance of successful reinforcement by 10% (only for Grade - 2 equipments)",
        "price": "150",
        "type": "Fame",
        "typeitem": "4"
        */
    }

    public Item(int idhg, int idig, int quantity, int level, int sellprice, int upLevel)//gem
    {
        setValue("idhg", idhg);
        setValue("idig", idig);

        setValue("quantity", quantity);
        setValue("level", level);

        setValue("sellprice", sellprice);
        setValue("uplevel", upLevel);
        foreach (Item _gem in SplitDataFromServe._InitGems)
        {
            if (idig == int.Parse(_gem.getValue("idig").ToString()))
            {
                setValue("name", _gem.getValue("name").ToString());
                setValue("price", _gem.getValue("price").ToString());
                setValue("description", _gem.getValue("description").ToString());
                setValue("attribute", _gem.getValue("attribute").ToString());


                break;
            }
        }
    }

    public Item(int idht, int idit, string nameI, int quantity, int sellPrice, int levelRequired, string des = "", int price = 0)
    {
        setValue("idht", idht);
        setValue("idit", idit);
        setValue("name", nameI);
        setValue("quantity", quantity);
        setValue("sellprice", sellPrice);
        setValue("levelrequired", levelRequired);
        setValue("descripton", des);
        setValue("price", price);

        if (des.Equals(""))
        {
            foreach (Item _gem in SplitDataFromServe._InitItems)
            {
                if (idit == int.Parse(_gem.getValue("idit").ToString()))
                {
                    setValue("name", _gem.getValue("name").ToString());
                    setValue("descripton", _gem.getValue("descripton").ToString());
                    setValue("price", _gem.getValue("price").ToString());
                    setValue("sellprice", _gem.getValue("sellprice").ToString());
                    break;
                }
            }
        }
        //Debug.Log(getValue("price").ToString());

    }//item
    public Item(JSONNode _node)
    {
        try
        {
            foreach (KeyValuePair<string, JSONNode> kvp in (JSONObject)_node)
            {
                setValue(kvp.Key.ToString(), kvp.Value.ToString().Replace("\"", ""));
            }
        }
        catch (Exception e)
        {
            //Debug.Log(e.ToString());
            //Debug.Log(_node.ToString());
        }
    }
    public Item(Item _archetypeItem)
    {
        indexes = new Dictionary<string, object>(_archetypeItem.indexes);
    }

    internal ITEMTYPE GetTypeItem()
    {
        if (indexes.ContainsKey("idit"))//Item
        {
            typeItem = int.Parse(getValue("idit").ToString());
            if (typeItem >= 1 && typeItem <= 10) return ITEMTYPE.LUCKY_MATERIAL_REINFORCEMENT;
            if (typeItem >= 18 && typeItem <= 27) return ITEMTYPE.SPECIAL_LUCKY_MATERIAL_REINFORCEMENT;
            else if (typeItem == 11) return ITEMTYPE.GROWUP_PET;
        }
        else if (indexes.ContainsKey("idig"))//Gem
        {
            typeItem = int.Parse(getValue("idig").ToString());
            //Debug.Log(typeItem);
            switch (typeItem)
            {
                case 1: return ITEMTYPE.RUNESTONES_STRENGTH;
                case 23: return ITEMTYPE.RUNESTONE_MASTERIALALCHEMY;
                case 25: return ITEMTYPE.RUNESTONE_SPECIALALCHEMY;
            }
        }
        return ITEMTYPE.NONE;
    }

    public void setValue(string field, object value)
    {
        if (indexes.ContainsKey(field))
            indexes[field] = value;
        else
            // indexes.Remove(field);
            indexes.Add(field, value);
        //indexes.Add(field, value);
    }

    internal bool isContains(string field)
    {
        return indexes.ContainsKey(field);
    }

    public object getValue(string field)
    {
        //if (indexes.ContainsKey(field))
        //{
        //    object value = indexes[field];
        //    if (value is Range<int>)
        //    {
        //        return (int)(value);
        //    }
        //    else if (value is Range<float>)
        //    {

        //        return (float)(value);
        //    }
        //    else if (value is float || value is int)
        //    {
        //        return (value is float ? (float)value : (int)value);
        //    }
        //}

        return (indexes.ContainsKey(field) ? indexes[field] : 0);
    }
}
