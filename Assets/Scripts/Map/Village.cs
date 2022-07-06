using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Village
{
    #region Data
    private List<Agent> villagers;
    private List<Agent> animals;
    private List<Room> rooms;
    private List<Structure> outdoorsStructures;

    private StatesHolder states;

    private List<string> neededItems;
    private List<MaterialType> neededResources;
    private List<RoomType> neededRooms;
    #endregion Data

    #region Properties
    public List<Agent> Villagers { get => villagers; }
    public List<Agent> Animals { get => animals; }
    public List<Room> Rooms { get => rooms; }
    public List<Structure> OutdoorsStructures { get => outdoorsStructures; }

    public StatesHolder States { get => states; }

    public List<string> NeededItems { get => neededItems; }
    public List<MaterialType> NeededResources { get => neededResources; }
    public List<RoomType> NeededRooms { get => neededRooms; }
    #endregion Properties


    #region Methods
    public Village()
    {
        villagers = new List<Agent>();
        animals = new List<Agent>();
        rooms = new List<Room>();
        outdoorsStructures = new List<Structure>(); 

        states = new StatesHolder();

        neededItems = new List<string>();
        neededResources = new List<MaterialType>();
        neededRooms = new List<RoomType>();
    }

    public Structure GetAvailableStorage(Agent agent)
    {
        foreach(Structure structure in outdoorsStructures)
        {
            if (structure.HasInventory && structure.Inventory.EmptySlots > 0) return structure;
        }
        foreach (Room room in rooms)
        {
            if (room.Owner == agent)
            {
                foreach (Structure structure in room.Structures)
                {
                    if (structure.HasInventory && structure.Inventory.EmptySlots > 0) return structure;
                }
            }
        }

        return null;
    }

    public void AddAgent(Agent agent)
    {
        if (agent.Intelligent) villagers.Add(agent);
        else animals.Add(agent);

        agent.AssignVillage(this);
    }
    public void AddRoom(Room room)
    {
        rooms.Add(room);
    }
    public void AddOutdoorsStructure(Structure structure)
    {
        outdoorsStructures.Add(structure);
    }


    public void RemoveAgent(Agent agent)
    {
        if (agent.Intelligent) villagers.Remove(agent);
        else animals.Remove(agent);
    }
    public void RemoveRoom(Room room)
    {
        rooms.Remove(room);
    }
    public void RemoveOutdoorsStructure(Structure structure)
    {
        outdoorsStructures.Remove(structure);
    }


    public void AddNeededItem(string itemName)
    {
        neededItems.Add(itemName);
        if (!states.HasState("villageNeedsItem")) states.AddState(Data.States["villageNeedsItem"]);
    }
    public void AddNeededResource(MaterialType resource)
    {
        neededResources.Add(resource);
    }
    public void AddNeededRoom(RoomType roomType)
    {
        neededRooms.Add(roomType);
    }

    public void RemoveNeededItem(string itemName)
    {
        neededItems.Remove(itemName);
        if (neededItems.Count == 0) states.RemoveState("villageNeedsItem");
    }
    public void RemoveNeededResource(MaterialType resource)
    {
        neededResources.Remove(resource);
    }
    public void RemoveNeededRoom(RoomType roomType)
    {
        neededRooms.Remove(roomType);
    }
    #endregion Methods
}