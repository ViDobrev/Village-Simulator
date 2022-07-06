using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class StructureData
{
    #region Data
    private string name;
    private StructureTag tag;
    private int durability;
    private int value;
    private int inventorySlots;

    private Schematic schematic;

    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public StructureTag Tag { get => tag; }
    public int Durability { get => durability; }
    public int Value { get => value; }
    public int InventorySlots { get => inventorySlots; }

    public Schematic Schematic { get => schematic; }

    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public StructureData(string name, StructureTag tag, int durability, int value, int inventorySlots, Schematic schematic, Tile tile)
    {
        this.name = name;
        this.tag = tag;
        this.durability = durability;
        this.value = value;
        this.inventorySlots = inventorySlots;

        this.schematic = schematic;

        this.tile = tile;
    }
    #endregion Methods
}