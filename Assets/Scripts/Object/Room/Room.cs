using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room
{
    #region Data
    //private string name;
    private Vector2 position; // topleft corner
    private RoomType roomType;

    private bool canBeOwned;
    private Agent owner;
    private List<Structure> structures;
    private List<Floor> floors;
    #endregion Data

    #region Properties
    //public string Name { get => name; }
    public Vector2 Position { get => position; }

    public bool CanBeOwned { get => canBeOwned; }
    public Agent Owner { get => owner; }
    public List<Structure> Structures { get => structures; }
    public List<Floor> Floors { get => floors; }
    #endregion Properties


    #region Methods
    public Room(RoomBlueprint roomBlueprint)
    {
        position = roomBlueprint.Position;
        roomType = roomBlueprint.RoomType;

        canBeOwned = roomType == RoomType.Personal;
        owner = null;
        structures = roomBlueprint.BuiltStructures;
        floors = roomBlueprint.BuiltFloors;
    }

    public bool SetOwner(Agent owner)
    {
        if (canBeOwned)
        {
            this.owner = owner;
            return true;
        }
        return false;
    }
    #endregion Methods
}



public enum RoomType : byte { Personal, Workshop, Storage };