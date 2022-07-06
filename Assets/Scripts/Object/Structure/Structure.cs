using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Structure : Object
{
    #region Data
    private StructureTag tag;
    private int durabilityMax, durabilityCurrent;
    private int value;
    private Quality quality;

    private MaterialData material;

    private Inventory inventory;

    private string creatorName;

    private Agent assignedUser;
    #endregion Data

    #region Properties
    public string FullName { get => $"{material.Adjective} {name}"; }
    public bool Traversible { get => tag == StructureTag.Wall || tag == StructureTag.Mountain; }
    //public int Value { get => value; }
    public Quality Quality { get => quality; }

    public MaterialData Material { get => material; }

    public bool HasInventory { get => inventory != null; }
    public Inventory Inventory { get => inventory; }

    public string CreatorName { get => creatorName; }

    public Agent AssignedUser { get => assignedUser; }
    public bool HasAssignedUsed { get => assignedUser != null; }
    #endregion Properties


    #region Methods
    public Structure(StructureData data, MaterialData material, Quality quality)
        : base(data.Name, material.Colour, data.Tile)
    {
        tag = data.Tag;
        durabilityMax = durabilityCurrent = data.Durability * material.Toughness;
        this.quality = quality;
        value = data.Value * material.Value;
        value += ((int)quality * value) / 100;

        this.material = material;

        inventory = data.InventorySlots > 0 ? new Inventory(data.InventorySlots) : null;
        if (inventory != null) inventory.SetParentObject(this);

        creatorName = string.Empty;
    }


    public void SignCreator(Agent creatorAgent)
    {
        creatorName = creatorAgent.Name;
    }

    public void Repair(int durability)
    {
        durabilityCurrent += durability;
        if (durabilityCurrent > durabilityMax) durabilityCurrent = durabilityMax;
    }
    public void TakeDamage(int damage)
    {
        durabilityCurrent -= damage;

        if (CheckFatality())
            Destroy();
    }
    private bool CheckFatality()
    {
        if (durabilityCurrent <= 0)
            return true;

        return false;
    }
    private void Destroy()
    {
        location.RemoveStructure();

        List<ItemSlot> itemsToDrop = new List<ItemSlot>();
        int amountOfMaterialToDrop = Data.Structures[name].Schematic.Component.Amount - 1;
        if (amountOfMaterialToDrop < 1) amountOfMaterialToDrop = 1;
        itemsToDrop.Add(new ItemSlot(ItemGenerator.GenerateItem(Data.Items[material.Name]), amountOfMaterialToDrop));

        if (HasInventory)
        {
            foreach(ItemSlot inventoryItemSlot in inventory.StoredItems)
            {
                itemsToDrop.Add(inventoryItemSlot);
            }
        }

        InteractionHandler.DropItems(itemsToDrop, location.Position);
    }

    public bool AssignUser(Agent user)
    {
        if (HasAssignedUsed && assignedUser != user) return false;

        assignedUser = user;
        return true;
    }
    public void Release()
    {
        assignedUser = null;
    }
    #endregion Methods
}



public enum StructureTag : byte
{
    Terrain,
    Floor,
    Workstation,
    Storage,
    Furniture,
    Door,
    Wall,
    Mountain,

    Rock
}