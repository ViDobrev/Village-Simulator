using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomBlueprint
{
    #region Statics
    private static Dictionary<RoomType, string[][]> roomStructureNames;
    private static Dictionary<RoomType, Vector2Int> roomSizes;
    #endregion Statics

    #region Data
    private Vector2Int size;
    private Vector2Int position; // Topleft corner

    private RoomType roomType;

    private List<Blueprint> blueprints;
    private List<Structure> builtStructures;
    private List<Floor> builtFloors;
    #endregion Data

    #region Properties
    public Vector2Int Size { get => size; }
    public Vector2Int Position { get => position; }

    public RoomType RoomType { get => roomType; }

    public List<Blueprint> Blueprints { get => blueprints; }
    public List<Structure> BuiltStructures { get => builtStructures; }
    public List<Floor> BuiltFloors { get => builtFloors; }


    public static Dictionary<RoomType, string[][]> RoomStructureNames { get => roomStructureNames; }
    public static Dictionary<RoomType, Vector2Int> RoomSizes { get => roomSizes;}
    #endregion Properties


    #region Methods
    public RoomBlueprint(RoomType roomType, Vector2Int position)
    {
        size = roomSizes[roomType];
        this.position = position;

        this.roomType = roomType;

        blueprints = new List<Blueprint>();
        builtStructures = new List<Structure>();

        // Create all the blueprints for the structures/floors
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Blueprint blueprint = null;
                string structureName = roomStructureNames[roomType][x][y];
                if (Data.Structures.ContainsKey(structureName))
                    blueprint = new Blueprint(Data.Structures[structureName], position + new Vector2Int(x, y), this);
                else if (Data.Floors.ContainsKey(structureName))
                    blueprint = new Blueprint(Data.Floors[structureName], position + new Vector2Int(x, y), this);
                else
                    Debug.LogError($"RoomBlueprint: Constructor: blueprint for structure {structureName} could not be found in Data.");

                blueprints.Add(blueprint);
                Data.AddBlueprint(blueprint);
            }
        }
    }

    public Room GetRoom()
    {
        return new Room(this);
    }

    public void AddBuiltStructure(Structure structure)
    {
        builtStructures.Add(structure);
    }
    public void AddBuiltFloor(Floor floor)
    {
        builtFloors.Add(floor);
    }
    public void RemoveBlueprint(Blueprint blueprint)
    {
        blueprints.Remove(blueprint);
    }

    public static void LoadRoomData(Dictionary<RoomType, string[][]> _roomStructureNames, Dictionary<RoomType, Vector2Int> _roomSizes)
    {
        roomStructureNames = _roomStructureNames;
        roomSizes = _roomSizes;
    }
    #endregion Methods
}