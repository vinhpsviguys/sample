

using System.Collections.Generic;


/*
 * Đây là enum list các loại đồ trong game
 */
public enum TypeEquipmentCharacter
{
    None = 0, Weapon, OffhandWeapon, Shield, Head, Torso, Leg, Belt, Gloves, Boots, Ring, Amulet, Gem, AlchemyMaterial, Avatar, Buff, VIPCard, Scroll, HPRecovery, LuckMaterial
}

/*
 * Đây là enum list các loại phân nhóm giáp trong game ( giáp nặng, giáp nhẹ, giáp vải)
 * Mỗi class sẽ thích hợp mặc các loại nhóm giáp riêng
 */
public enum ClassCharacterItem
{
    HeavyArmor, SoftArmor, ClothArmor, None
}

/*
 * Đây là độ hiếm của các item trong game
 */
public enum RarelyItem
{
    Common, Rare, Mystical, Legendary, Unique
}


/*
 * Class này thể hiện một đồ bất kỳ trong game
 */
public class EquipmentItem
{
    internal int idItem;
    internal int idItemInit;
    internal int idTypeEquipment;
    internal TypeEquipmentCharacter typeItem; //99 loại : 2 số đầu
    internal ClassCharacterItem classItem;
    internal int rarelyItem;
    internal int idGroupSetItems;
    internal string nameItem;
    internal int levelRequired;
    internal int levelUpgraded;
    internal int priceItem;

    internal Dictionary<string, object> indexes = new Dictionary<string, object>();

    internal float valueItem;
    internal int numberItem; // dành cho các item có số lượng : nguyên liệu,...

    public EquipmentItem()
    {
        idItem = 0;
        typeItem = TypeEquipmentCharacter.None;
    }

    public EquipmentItem(int _idItem, int _idItemInit, TypeEquipmentCharacter _typeItem, ClassCharacterItem _classItem,int idType, int _idGroupSet, string _name, int _lvlRequired, int _lvlUpgraded, int _priceItem, int rareItem)
    {
        idItem = _idItem;
        idItemInit = _idItemInit;
        typeItem = _typeItem;
        classItem = _classItem;
        idTypeEquipment = idType;
        idGroupSetItems = _idGroupSet;
        nameItem = _name;
        levelRequired = _lvlRequired;
        levelUpgraded = _lvlUpgraded;
        priceItem = _priceItem;
        this.rarelyItem = rareItem;
    }
    public EquipmentItem(EquipmentItem _archetype)
    {
        idItem = _archetype.idItem;
        idItemInit = _archetype.idItemInit;
        typeItem = _archetype.typeItem;
        classItem = _archetype.classItem;
        idTypeEquipment = _archetype.idTypeEquipment;
        rarelyItem = _archetype.rarelyItem;
        idGroupSetItems = _archetype.idGroupSetItems;
        nameItem = _archetype.nameItem;
        levelRequired = _archetype.levelRequired;
        levelUpgraded = _archetype.levelUpgraded;
        priceItem = _archetype.priceItem;
        indexes = new Dictionary<string, object>(_archetype.indexes);
    }

    public EquipmentItem(int id, TypeEquipmentCharacter _type)
    {
        idItem = id;
        typeItem = _type;
        levelRequired = 0;
    }

    public object getValue(string field, object value)
    {
        return (indexes.ContainsKey(field) ? indexes[field] : value);
    }

    public object getValue(string field)
    {
        return (indexes.ContainsKey(field) ? indexes[field] : 0);
    }

    public void setValue(string field, object value)
    {
        if (indexes.ContainsKey(field))
            indexes[field] = value;
        else
            // indexes.Remove(field);
            indexes.Add(field, value);
    }
}

