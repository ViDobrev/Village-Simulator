using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class FloorData
{
    #region Data
    private string name;
    private int durability;

    private Schematic schematic;

    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public int Durability { get => durability; }

    public Schematic Schematic { get => schematic; }

    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public FloorData(string name, int durability, Schematic schematic, Tile tile)
    {
        this.name = name;
        this.durability = durability;

        this.schematic = schematic;

        this.tile = tile;
    }
    #endregion Methods
}