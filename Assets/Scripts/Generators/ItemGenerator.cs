using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public static class ItemGenerator
{
    #region Methods
    public static Item GenerateItem(ItemBlueprint blueprint, Agent creatorAgent)
    {
        Quality quality = Quality.Normal;
        if (creatorAgent != null) quality = GetQuality(creatorAgent);

        List<Attack> attacks = null;
        if (blueprint.ItemData.Attacks != null)
        {
            attacks = new List<Attack>();
            foreach (string damageString in blueprint.ItemData.Attacks)
            {
                attacks.Add(new Attack(damageString, (int)quality));
            }
        }
        int defence = blueprint.ItemData.Defence * blueprint.MainIngredient.Material.ArmourModifier;
        EquipmentData equipmentData = new EquipmentData(attacks, defence, blueprint.ItemData.EquipmentSlot, blueprint.ItemData.TwoHanded);


        Item item = new Item(blueprint.ItemData.Name, equipmentData, blueprint.MainIngredient.Material, null, quality);
        if (creatorAgent != null) item.SignCreator(creatorAgent);

        return item;
    }
    public static Item GenerateItem(ItemData itemData, List<ItemSlot> ingredients, Agent creatorAgent)
    {
        List<Ingredient> recipeIngredients = itemData.Recipe.Ingredients;
        Item mainIngredient = null;

        bool recipeFulfilled = true;
        for (int i = 0; i < recipeIngredients.Count; i++)
        {
            if (!recipeIngredients[i].CanUseMaterialType(ingredients[i].Item.Material.Type))
            {
                recipeFulfilled = false;
                break;
            }
            if (recipeIngredients[i].Amount < ingredients[i].Amount)
            {
                recipeFulfilled = false;
                break;
            }
            if (recipeIngredients[i].Main) mainIngredient = ingredients[i].Item;
        }
        if (!recipeFulfilled) return null;

        Quality quality = Quality.Normal;
        if (creatorAgent != null) quality = GetQuality(creatorAgent);

        List<Attack> attacks = null;
        if (itemData.Attacks != null)
        {
            attacks = new List<Attack>();
            foreach (string damageString in itemData.Attacks)
            {
                attacks.Add(new Attack(damageString, (int)quality));
            }
        }
        int defence = itemData.Defence * mainIngredient.Material.ArmourModifier;
        EquipmentData equipmentData = new EquipmentData(attacks, defence, itemData.EquipmentSlot, itemData.TwoHanded);


        Item item = new Item(itemData.Name, equipmentData, mainIngredient.Material, null, quality);
        if (creatorAgent != null) item.SignCreator(creatorAgent);

        return item;
    }
    public static Item GenerateItem(ItemData itemData, List<ItemSlot> ingredients, Agent creatorAgent, Quality quality)
    {
        List<Ingredient> recipeIngredients = itemData.Recipe.Ingredients;
        Item mainIngredient = null;

        bool recipeFulfilled = true;
        for (int i = 0; i < recipeIngredients.Count; i++)
        {
            if (!recipeIngredients[i].CanUseMaterialType(ingredients[i].Item.Material.Type))
            {
                recipeFulfilled = false;
                break;
            }
            if (recipeIngredients[i].Main) mainIngredient = ingredients[i].Item;
        }
        if (!recipeFulfilled) return null;

        List<Attack> attacks = new List<Attack>();
        foreach (string damageString in itemData.Attacks)
        {
            attacks.Add(new Attack(damageString, (int)quality));
        }

        int defence = itemData.Defence * mainIngredient.Material.ArmourModifier;
        EquipmentData equipmentData = new EquipmentData(attacks, defence, itemData.EquipmentSlot, itemData.TwoHanded);


        Item item = new Item(itemData.Name, equipmentData, mainIngredient.Material, null, quality);
        if (creatorAgent != null) item.SignCreator(creatorAgent);

        return item;
    }
    public static Item GenerateItem(ItemData itemData)
    {
        if (itemData.HasTag(ItemTag.Material))
        {
            return new Item(itemData.Name, null, itemData.MaterialData, null, Quality.Normal);
        }
        if (itemData.HasTag(ItemTag.Food))
        {
            return new Item(itemData.Name, null, null, itemData.FoodData, Quality.Normal);
        }


        Debug.LogError($"ItemGenerator: Item ({itemData.Name}) could not be generated.");
        return null;
    }

    public static Item CloneItem(Item itemToCopy)
    {
        return new Item(itemToCopy);
    }


    private static Quality GetQuality(Agent creatorAgent)
    {
        int skillLevel = creatorAgent.Skills[SkillsEnum.Crafting].Level;
        var qualities = Enum.GetValues(typeof(Quality));

        int[] qualityDistribution = Skill.QualityDistribution[skillLevel];
        int[] qualityValues = new int[qualities.Length];

        for (int i = 0; i < qualities.Length; i++)
        {
            qualityValues[i] = (int)qualities.GetValue(i);
        }

        return (Quality)Utilities.RandomNumberByPropbability(qualityValues, qualityDistribution);
    }
    #endregion Methods
}