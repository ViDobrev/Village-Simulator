using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemBlueprint
{
    #region Data
    private ItemData itemData;
    private float workDone;
    private List<ItemSlot> itemsToUse;
    private Item mainIngredient;
    #endregion Data

    #region Properties
    public ItemData ItemData { get => itemData; }
    public float WorkDone { get => workDone; }
    public List<ItemSlot> ItemsToUse { get => itemsToUse; }
    public Item MainIngredient { get => mainIngredient; }

    public bool Completed { get => workDone >= itemData.Recipe.WorkRequired; }
    #endregion Properties


    #region Methods
    public ItemBlueprint(ItemData itemData)
    {
        this.itemData = itemData;
        workDone = 0f;
    }

    public bool TryGiveItems(List<ItemSlot> itemsToUse)
    {
        List<Ingredient> ingredients = itemData.Recipe.Ingredients;
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (!ingredients[i].CanUseMaterial(itemsToUse[i].Item.Material)) return false;
            if (ingredients[i].Main) mainIngredient = itemsToUse[i].Item;
        }

        this.itemsToUse = itemsToUse;
        return true;
    }

    public void Work(float increment)
    {
        workDone += increment;
    }
    #endregion Methods
}