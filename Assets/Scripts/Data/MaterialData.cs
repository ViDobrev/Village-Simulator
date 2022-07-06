using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialData
{
    #region Data
    private string name;
    private MaterialType type;
    private string adjective;
    private int toughness;
    private int value;
    private int damageModifier;
    private int armourModifier;
    private Color colour;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public MaterialType Type { get => type; }
    public string Adjective { get => adjective; }
    public int Toughness { get => toughness; }
    public int Value { get => value; }
    public int DamageModifier { get => damageModifier; }
    public int ArmourModifier { get => armourModifier; }
    public Color Colour { get => colour; }
    #endregion Properties


    #region Methods
    public MaterialData(string name, MaterialType type, string adjective, int toughness, int value, int damageModifier, int armourModifier, ColourEnum colour)
    {
        this.name = name;
        this.type = type;
        this.adjective = adjective;
        this.toughness = toughness;
        this.value = value;
        this.damageModifier = damageModifier;
        this.armourModifier = armourModifier;
        this.colour = Data.ColourDict[colour];
    }
    #endregion Methods
}

public enum MaterialType : byte
{
    Wood,
    Stone,
    Metal,
    Leather,
    Fabric,
    Glass,
    Thread
}