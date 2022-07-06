using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemData
{
    #region Data
    private string name;
    private int value;
    private List<ItemTag> tags;
    private bool stackable;

    private MaterialData materialData;
    private FoodData foodData;

    private Recipe recipe;
    private List<string> attacks;
    private int defence;
    private bool twoHanded;
    private EquipmentSlot equipmentSlot;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public int Value { get => value; }
    public List<ItemTag> Tags { get => tags; }
    public bool Stackable { get => stackable; }

    public MaterialData MaterialData { get => materialData; }
    public FoodData FoodData { get => foodData; }

    public Recipe Recipe { get => recipe; }
    public List<string> Attacks { get => attacks; }
    public int Defence { get => defence; }
    public bool TwoHanded { get => twoHanded; }
    public EquipmentSlot EquipmentSlot { get => equipmentSlot; }
    #endregion Properties


    #region Methods
    public ItemData(string name, int value, List<ItemTag> tags, bool stackable, List<string> attacks, int defence, bool twoHanded, EquipmentSlot equipmentSlot, Recipe recipe)
    {
        this.name = name;
        this.value = value;
        this.tags = tags;
        this.stackable = stackable;

        this.materialData = null;
        this.foodData = null;

        this.recipe = recipe;
        this.attacks = attacks;
        this.defence = defence;
        this.twoHanded = twoHanded;
        this.equipmentSlot = equipmentSlot;
    }
    public ItemData(string name, bool stackable, MaterialData materialData)
    {
        this.name = name;
        this.value = 0;
        this.tags = new List<ItemTag>() { ItemTag.Material };
        this.stackable = stackable;

        this.materialData = materialData;
        this.foodData = null;

        this.recipe = null;
        this.attacks = null;
        this.defence = 0;
        this.twoHanded = false;
        this.equipmentSlot = EquipmentSlot.Held;
    }
    public ItemData(string name, int value, bool stackable, FoodData foodData)
    {
        this.name = name;
        this.value = value;
        this.tags = new List<ItemTag>() { ItemTag.Food };
        this.stackable = stackable;

        this.materialData = null;
        this.foodData = foodData;

        this.recipe = null;
        this.attacks = null;
        this.defence = 0;
        this.twoHanded = false;
        this.equipmentSlot = EquipmentSlot.Held;
    }

    public bool HasTag(ItemTag tag)
    {
        return tags.Contains(tag);
    }
    #endregion Methods
}