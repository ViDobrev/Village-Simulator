using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment
{
    #region Data
    private Dictionary<EquipmentSlot, Item> slots = new Dictionary<EquipmentSlot, Item>()
    {
        {EquipmentSlot.Head,    null},
        {EquipmentSlot.Torso,   null},
        {EquipmentSlot.Back,    null},
        {EquipmentSlot.Hands,   null},
        {EquipmentSlot.Waist,   null},
        {EquipmentSlot.Legs,    null},
        {EquipmentSlot.Feet,    null},

        {EquipmentSlot.Held,    null},
        {EquipmentSlot.Shield,  null}
    };

    private Inventory inventory;
    //private Agent agent;
    #endregion Data

    #region Properties
    public Dictionary<EquipmentSlot, Item> Slots { get => slots; }
    public Item this[EquipmentSlot key]
    {
        get => slots[key];
        //set => slots[key] = value;
    }

    public Inventory Inventory { get => inventory; }
    #endregion Properties


    #region Methods
    public Equipment(int inventorySlots)
    {
        inventory = new Inventory(inventorySlots);
    }
    public Equipment(int inventorySlots, List<Item> items)
    {
        inventory = new Inventory(inventorySlots);

        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            EquipItem(ref item);
            if (item != null)
                inventory.PlaceItem(item);
        }
    }
    public void SetAgent(Agent agent)
    {
        //this.agent = agent;
        inventory.SetParentObject(agent);
    }

    public bool EquipItem(ref Item item)
    {// Returns true if the given item was equipped, false if it could not be
        if (item.EquipmentData != null)
        {
            Item itemToEquip = item;
            item = slots[item.EquipmentData.Slot];
            slots[itemToEquip.EquipmentData.Slot] = itemToEquip;
            return true;
        }
        return false;
    }
    #endregion Methods
}



public enum EquipmentSlot : byte
{
    Head,
    Torso,
    Back,
    Hands,
    Waist,
    Legs,
    Feet,

    Held,
    Shield
}