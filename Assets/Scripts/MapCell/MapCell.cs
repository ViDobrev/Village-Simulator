using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapCell
{
    #region Data
    private Vector2Int position;
    private bool visible;

    private MyTerrain terrain;
    private Floor floor;
    private ItemSlot item;
    private Plant plant;
    private Structure structure;
    private Corpse corpse;
    private Agent agent;
    private Agent passingAgent;
    #endregion Data

    #region Properties
    public Vector2Int Position { get => position; }
    public bool Visible { get => visible; }
    public bool Traversible { get => HasStructure ? !structure.Traversible : terrain.Traversible; }
    public bool CanBeMovedTo { get => Traversible && !HasAgent; }

    public bool HasItem { get => item != null; }
    public bool HasPlant { get => plant != null; }
    public bool HasStructure { get => structure != null; }
    public bool HasFloor { get => floor != null; }
    public bool HasCorpse { get => corpse != null; }
    public bool HasAgent { get => agent != null; }
    public bool IsFree
    {
        get
        {
            if (HasItem || HasPlant || HasStructure || HasCorpse || HasAgent || !Traversible) return false;
            return true;
        }
    }


    public MyTerrain Terrain { get => terrain; }
    public Floor Floor { get => floor; }
    public ItemSlot Item { get => item; }
    public Plant Plant { get => plant; }
    public Structure Structure { get => structure; }
    public Corpse Corpse { get => corpse; }
    public Agent Agent { get => agent; }
    public Agent PassingAgent { get => passingAgent; }

    public string UppermostName
    {
        get => visible      ?
               HasAgent     ? agent.Name     :
               HasStructure ? structure.Name :
               HasPlant     ? plant.Name     :
               HasCorpse    ? corpse.Name    :
               HasItem      ? item.Name      :
               HasFloor     ? floor.Name     :
               terrain.Name : Data.Unknown;
    }
    public Tile Tile
    {
        get => HasAgent     ? agent.Tile     :
               HasStructure ? structure.Tile :
               HasPlant     ? plant.Tile     :
               HasCorpse    ? corpse.Tile    :
               HasItem      ? item.Tile      :
               HasFloor     ? floor.Tile     :
               terrain.Tile;
    }
    public Color Colour
    {
        get => HasAgent     ? agent.Colour     :
               HasStructure ? structure.Colour :
               HasPlant     ? plant.Colour     :
               HasCorpse    ? corpse.Colour    :
               HasItem      ? item.Colour      :
               HasFloor     ? floor.Colour     :
               terrain.Colour;
    }
    #endregion Properties


    #region Methods
    public MapCell(Vector2Int position, MyTerrain terrain)
    {
        this.position = position;
        this.terrain = terrain;

        visible = true;
    }
    
    public bool AddItem(ItemSlot item)
    {// Returns true if item was attached successfully, otherwise returns false
        if (!HasStructure && !HasPlant && Traversible)
        {
            this.item = item;
            Data.AddItemOnGround(item);
            item.Item.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddPlant(Plant plant)
    {// Returns true if plant was attached successfully, otherwise returns false
        if (!HasPlant && !HasStructure && Traversible)
        {
            this.plant = plant;
            this.plant.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddStructure(Structure structure)
    {// Returns true if structure was attached successfully, otherwise returns false
        if (!HasStructure && !HasPlant && Traversible)
        {
            this.structure = structure;
            this.structure.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddFloor(Floor floor)
    {// Returns true if floor was attached successfully, otherwise returns false
        if (!HasStructure && !HasPlant && HasFloor && Traversible)
        {
            this.floor = floor;
            this.floor.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddCorpse(Corpse corpse)
    {// Returns true if corpse was attached successfully, otherwise returns false
        if (!HasCorpse && Traversible)
        {
            this.corpse = corpse;
            corpse.SetLocation(this);
            Data.AddCorpse(corpse);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddAgent(Agent agent)
    {// Returns true if agent was attached successfully, otherwise returns false
        if (!HasAgent && Traversible)
        {
            this.agent = agent;
            agent.SetLocation(this);
            UpdateTile();
            return true;
        }
        if (HasAgent && Traversible && passingAgent == null)
        {
            if (agent.AI.CurrentAction.Path.Count > 1)
            {
                passingAgent = agent;
                agent.SetLocation(this);
                return true;
            }
        }
        return false;
    }


    public void RemoveItem()
    {
        Data.RemoveItemOnGround(item);
        item = null;
        UpdateTile();
    }
    public void RemovePlant()
    {
        plant = null;
        UpdateTile();
    }
    public void RemoveStructure()
    {
        structure = null;

        UpdateTile();
        if (visible && Data.Map != null) Data.Map.MakeNeighbouringCellsVisible(this);
    }
    public void RemoveFloor()
    {
        floor = null;
        UpdateTile();
    }
    public void RemoveCorpse()
    {
        Data.RemoveCorpse(corpse);
        corpse = null;
        UpdateTile();
    }
    public void RemoveAgent(Agent agent)
    {
        if (this.agent == agent) this.agent = null;
        else passingAgent = null;
        UpdateTile();
    }


    public void SetVisible(bool visible)
    {
        if (this.visible != visible)
        {
            this.visible = visible;

            if (Data.Map != null)
                Data.Map.UpdateNeighbouringCellsVisibility(this);

            UpdateTile();
        }
    }
    private void UpdateTile()
    {
        Data.UpdateTile(this);
    }
    #endregion Methods
}