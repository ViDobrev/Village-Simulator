using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : Object
{
    #region Data
    private EquipmentData equipmentData;

    private MaterialData material;
    private FoodData foodData;

    private int value;
    private List<ItemTag> tags;
    private bool stackable;
    private Quality quality;

    private string creatorName;

    private Agent assignedUser;

    private Inventory parentInventory;
    #endregion Data

    #region Properties
    public string FullName { get => (material != null) && !HasTag(ItemTag.Material) ? ($"{material.Adjective} {name}") : name; }
    public override Vector2Int Position
    {
        get
        {
            if (IsInsideInventory) return ParentInventory.ParentObject.Position;
            else return location.Position;
        }
    }

    public EquipmentData EquipmentData { get => equipmentData; }

    public MaterialData Material { get => material; }
    public FoodData FoodData { get => foodData; }

    //public int Value { get => value; }
    public bool Stackable { get => stackable; }
    public Quality Quality { get => quality; }

    public string CreatorName { get => creatorName; }

    public Agent AssignedUser { get => assignedUser; }
    public bool HasAssignedUsed { get => assignedUser != null; }

    public Inventory ParentInventory { get => parentInventory; }
    public bool IsInsideInventory { get => parentInventory != null; }
    #endregion Properties


    #region Methods
    public Item(string name, EquipmentData equipmentData,MaterialData material, FoodData foodData, Quality quality)
        : base(name, Data.ColourDict[ColourEnum.Magenta], Data.Tiles[name])
    {
        this.equipmentData = equipmentData;

        this.material = material;
        this.foodData = foodData;

        ItemData itemData = Data.Items[name];
        value = (material != null) ? material.Value * itemData.Value : itemData.Value;
        value += ((int)quality * value) / 100;
        tags = itemData.Tags;
        stackable = itemData.Stackable;
        this.quality = quality;

        creatorName = string.Empty;
    }
    public Item(Item originalItem)
        : base(originalItem.Name, originalItem.Colour, originalItem.Tile)
    {// Used for item cloning
        equipmentData = new EquipmentData(originalItem.equipmentData);

        material = originalItem.material;
        foodData = originalItem.foodData;

        value = originalItem.value;
        tags = new List<ItemTag>();
        foreach(ItemTag tag in originalItem.tags) tags.Add(tag);
        stackable = originalItem.stackable;
        quality = originalItem.quality;

        creatorName = string.Empty;
        parentInventory = originalItem.parentInventory;
    }

    public void SignCreator(Agent creatorAgent)
    {
        creatorName = creatorAgent.Name;
    }

    public bool HasTag(ItemTag tag)
    {
        return tags.Contains(tag);
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

    public void SetInventory(Inventory inventory)
    {
        parentInventory = inventory;
    }
    #endregion Methods
}


public class EquipmentData
{
    #region Data
    private List<Attack> attacks;
    private int defence;
    private EquipmentSlot slot;
    private bool twoHanded;
    #endregion Data

    #region Properties
    public List<Attack> Attacks { get => attacks; }
    public int Defence { get => defence; }
    public EquipmentSlot Slot { get => slot; }
    public bool TwoHanded { get => twoHanded; }
    #endregion Properties


    #region Methods
    public EquipmentData(List<Attack> attacks, int defence, EquipmentSlot slot, bool twoHanded)
    {
        this.attacks = attacks;
        this.defence = defence;
        this.slot = slot;
        this.twoHanded = twoHanded;
    }
    public EquipmentData(EquipmentData originalData)
    {// Used for item cloning
        attacks = new List<Attack>();
        foreach (Attack attack in originalData.attacks) attacks.Add(new Attack(attack));
        defence = originalData.defence;
        slot = originalData.slot;
        twoHanded = originalData.twoHanded;
    }
    #endregion Methods
}



public enum ItemTag : byte
{
    Tool_Crafting,
    Tool_Building,
    Tool_Mining,
    Tool_Woodcutting,

    Weapon_Melee,
    Weapon_Range,
    //Ammunition,

    Shield,

    Armour,
    Clothing,

    Material,
    Food
}


public enum Quality : sbyte
{
    Awful = -2,
    Bad,
    Normal,
    WellMade,
    Masterwork
}