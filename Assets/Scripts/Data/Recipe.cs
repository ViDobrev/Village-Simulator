using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Recipe
{
    #region Data
    //private string name;
    private List<Ingredient> ingredients;
    private int yield;
    private int work;
    #endregion Data

    #region Properties
    //public string Name { get => name; }
    public List<Ingredient> Ingredients { get => ingredients; }
    public Ingredient MainIngredient
    {
        get
        {
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.Main) return ingredient;
            }
            return null;
        }
    }
    public int Yield { get => yield; }
    public int WorkRequired { get => work; }
    #endregion Properties


    #region Methods
    public Recipe(List<Ingredient> ingredients, int yield, int work)
    {
        this.ingredients = ingredients;
        this.yield = yield;
        this.work = work;
    }
    #endregion Methods
}


public class Ingredient
{
    #region Data
    private List<MaterialType> materialTypes;
    private int amount;
    private bool main;
    #endregion Data

    #region Properties
    public List<MaterialType> MaterialTypes { get => materialTypes; }
    public int Amount { get => amount; }
    public bool Main { get => main; }
    #endregion Properties


    #region Methods
    public Ingredient(List<MaterialType> materialTypes, int amount, bool main)
    {
        this.materialTypes = materialTypes;
        this.amount = amount;
        this.main = main;
    }


    public bool CanUseMaterialType(MaterialType type)
    {// Returns true if given materialType can be used for this ingredient
        if (materialTypes.Contains(type)) return true;
        return false;
    }
    public bool CanUseMaterial(MaterialData material)
    {// Returns true if a given material can be used for this ingredient
        if (materialTypes.Contains(material.Type)) return true;
        return false;
    }
    #endregion Methods
}