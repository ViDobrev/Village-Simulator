using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Schematic
{
    #region Data
    private Component component;
    private int workRequired;
    #endregion Data

    #region Properties
    public Component Component { get => component; }
    public int WorkRequired { get => workRequired; }
    #endregion Properties


    #region Methods
    public Schematic(Component component, int workRequired)
    {
        this.component = component;
        this.workRequired = workRequired;
    }
    #endregion Methods
}


public class Component
{
    #region Data
    private List<MaterialType> materialTypes;
    private int amount;
    #endregion Data

    #region Properties
    public List<MaterialType> MaterialTypes { get => materialTypes; }
    public int Amount { get => amount; }
    #endregion Properties


    #region Methods
    public Component(List<MaterialType> materialTypes, int amount)
    {
        this.materialTypes = materialTypes;
        this.amount = amount;
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