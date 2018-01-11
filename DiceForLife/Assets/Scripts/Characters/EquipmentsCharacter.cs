using UnityEngine;
using System.Collections.Generic;

public class EquipmentsCharacter
{
    internal EquipmentItem headItem;
    internal EquipmentItem weaponItem;
    internal EquipmentItem shieldItem;
    internal EquipmentItem torsoItem;
    internal EquipmentItem legItem;
    internal EquipmentItem beltItem;
    internal EquipmentItem glovesItem;
    internal EquipmentItem bootsItem;
    internal EquipmentItem amuletItem;
    internal EquipmentItem ringItem;
    internal EquipmentItem buffItem;
    internal EquipmentItem avatarItem;

    private EquipmentItem _tempItem;

    public delegate void ChangeItem();
    public static event ChangeItem ChangeItemEvent;

    internal TypeEquipmentCharacter _lastEquipmentChangeSlot;
    internal EquipmentItem _lastEquipmentChangeItem;

    public EquipmentsCharacter() { }
    public EquipmentsCharacter(List<EquipmentItem> _tempListEquipped)
    {

        headItem = null;
        weaponItem = null;
        shieldItem = null;
        torsoItem = null;
        legItem = null;
        beltItem = null;
        glovesItem = null;
        bootsItem = null;
        amuletItem = null;
        ringItem = null;
        buffItem = null;
        avatarItem = null;
        foreach (EquipmentItem tempItemEquip in _tempListEquipped)
        {
            if (tempItemEquip.typeItem == TypeEquipmentCharacter.Head)
            {
                headItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Weapon && weaponItem == null)
            {
                weaponItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Weapon && weaponItem != null)
            {
                tempItemEquip.typeItem = TypeEquipmentCharacter.OffhandWeapon;
                shieldItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Shield)
            {
                shieldItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.OffhandWeapon)
            {
                shieldItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Torso)
            {
                torsoItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Leg)
            {
                legItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Belt)
            {
                beltItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Gloves)
            {
                glovesItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Boots)
            {
                bootsItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Amulet)
            {
                amuletItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Ring)
            {
                ringItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Avatar)
            {
                avatarItem = tempItemEquip;
            }
            else if (tempItemEquip.typeItem == TypeEquipmentCharacter.Buff)
            {
                buffItem = tempItemEquip;
            }
        }

    }
    internal void EquipItem(TypeEquipmentCharacter slot, EquipmentItem newEquip)
    {
        if (newEquip.typeItem == slot
            || (slot == TypeEquipmentCharacter.OffhandWeapon && newEquip.typeItem == TypeEquipmentCharacter.Weapon)
            || (slot == TypeEquipmentCharacter.Weapon && newEquip.typeItem == TypeEquipmentCharacter.OffhandWeapon)) // Mặc đồ mới
        {
            if (CharacterInfo._instance._baseProperties.Level >= newEquip.levelRequired)
            {
                _tempItem = MappingTypeItemToItem(slot);// = newEquip;
                _tempItem = newEquip;
                _tempItem.typeItem = slot;
                MappingTempItemToItem(slot, _tempItem);
                _lastEquipmentChangeSlot = slot;
                _lastEquipmentChangeItem = newEquip;
                ChangeItemEvent();
            }
        }
    }
    internal bool IsCanEquipItem(EquipmentItem newEquip)
    {
        if (CharacterInfo._instance._baseProperties.Level >= newEquip.levelRequired)
        {
            return true;
        }
        return false;
    }
    internal bool isHaveItem(TypeEquipmentCharacter slot)
    {
        return MappingTypeItemToItem(slot).idItem == 0 ? false : true;
    }
    internal EquipmentItem MappingTypeItemToItem(TypeEquipmentCharacter slot)
    {
        switch (slot)
        {
            case TypeEquipmentCharacter.Head: return headItem;
            case TypeEquipmentCharacter.Weapon: return weaponItem;
            case TypeEquipmentCharacter.Shield: return shieldItem;
            case TypeEquipmentCharacter.OffhandWeapon: return shieldItem;
            case TypeEquipmentCharacter.Torso: return torsoItem;
            case TypeEquipmentCharacter.Leg: return legItem;
            case TypeEquipmentCharacter.Gloves: return glovesItem;
            case TypeEquipmentCharacter.Boots: return bootsItem;
            case TypeEquipmentCharacter.Belt: return beltItem;
            case TypeEquipmentCharacter.Amulet: return amuletItem;
            case TypeEquipmentCharacter.Ring: return ringItem;
            case TypeEquipmentCharacter.Buff: return buffItem;
            case TypeEquipmentCharacter.Avatar: return avatarItem;
        }
        return null;
    }
    internal void MappingTempItemToItem(TypeEquipmentCharacter slot, EquipmentItem _tempItem)
    {
        switch (slot)
        {
            case TypeEquipmentCharacter.Head: headItem = _tempItem; break;
            case TypeEquipmentCharacter.Weapon: weaponItem = _tempItem; break;
            case TypeEquipmentCharacter.Shield: shieldItem = _tempItem; break;
            case TypeEquipmentCharacter.OffhandWeapon: shieldItem = _tempItem; break;
            case TypeEquipmentCharacter.Torso: torsoItem = _tempItem; break;
            case TypeEquipmentCharacter.Leg: legItem = _tempItem; break;
            case TypeEquipmentCharacter.Gloves: glovesItem = _tempItem; break;
            case TypeEquipmentCharacter.Boots: bootsItem = _tempItem; break;
            case TypeEquipmentCharacter.Belt: beltItem = _tempItem; break;
            case TypeEquipmentCharacter.Amulet: amuletItem = _tempItem; break;
            case TypeEquipmentCharacter.Ring: ringItem = _tempItem; break;
            case TypeEquipmentCharacter.Buff: buffItem = _tempItem; break;
            case TypeEquipmentCharacter.Avatar: avatarItem = _tempItem; break;
        }
    }
}
