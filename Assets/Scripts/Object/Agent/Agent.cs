using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : Object
{

    #region Data
    private Gender gender;
    private Species species;

    private int healthMax, healthCurrent;
    private float energyMax, energyCurrent;
    private float hunger;

    private List<MovementType> movementTypes;

    // Only for intelligent agents
    private Equipment equipment;
    private SkillsManager skills;

    // AI
    private Job job;
    private Village village;
    private StatesHolder states;
    private AI ai;

    private const float MAX_HUNGER = 100f;
    #endregion Data

    #region Properties
    public Species Species { get => species; }

    public List<MovementType> MovementTypes { get => movementTypes; }

    public Equipment Equipment { get => equipment; }
    public Inventory Inventory { get => equipment.Inventory; }
    public SkillsManager Skills { get => skills; }

    public bool Intelligent { get => species.Intelligent; }
    public Job Job { get => job; }
    public Village Village { get => village; }
    public StatesHolder States { get => states; }
    public AI AI { get => ai; }

    #endregion Properties


    #region Methods
    public Agent(string name, Gender gender, Species species, Equipment equipment, SkillsManager skills)
        : base(name, species.Colour, species.Tile)
    {
        this.gender = gender;
        this.species = species;

        healthMax = healthCurrent = species.Health;
        energyMax = energyCurrent = species.Energy;

        hunger = MAX_HUNGER;

        movementTypes = species.MovementTypes;

        this.equipment = equipment;
        if (equipment != null) equipment.SetAgent(this);
        this.skills = skills;

        job = Job.None;
        states = new StatesHolder(species.Intelligent);
        ai = new AI(this);
    }


    public bool Move(Vector2Int direction)
    {// Agent moves 1 cell in the given direction. Returns true if move was successful
        MapCell initialLocation = location;
        bool hasMoved = Data.Map.CellAtPosition(location.Position + direction).AddAgent(this);
        
        if (hasMoved) initialLocation.RemoveAgent(this);
        return hasMoved;
    }

    public void Heal(int healAmount)
    {
        healthCurrent += healAmount;
        if (healthCurrent > healthMax) healthCurrent = healthMax;
    }
    public void TakeDamage(int damage)
    {// Agent takes damage and checks if it is fatal
        healthCurrent -= damage;

        if (CheckFatality())
            Die();
    }
    private bool CheckFatality()
    {// Agent checks if its health is below zero, i.e if agent has to die
        if (healthCurrent <= 0)
            return true;

        return false;
    }
    private void Die()
    {
        AI.ReleaseResources();
        location.RemoveAgent(this);
        Data.RemoveAgent(this);
        Debug.Log(name + " died!");

        if (village != null)
        {
            foreach (Room room in village.Rooms)
            {
                if (room.Owner == this) room.SetOwner(null);
            }
        }

        MapCell corpseLocation = Utilities.GetClosestCellByCondition(location, (cell) => !cell.HasCorpse && cell.Traversible);
        if (corpseLocation == null) return;

        Corpse corpse = new Corpse(this);
        corpseLocation.AddCorpse(corpse);

    }

    public void AssignJob(Job job)
    {
        this.job = job;
    }
    public void AssignVillage(Village village)
    {
        this.village = village;
    }

    public void Act()
    {
        if (!AI.HasActionPlan)
            AI.CreateActionPlan();

        Action currentAction = AI.GetAction();
        currentAction.Run();

        //if (currentAction.IsCompleted)
        //    AI.CompleteAction();
    }
    public void TakeItem(ItemSlot itemSlot)
    {
        if (itemSlot == location.Item)
        {
            location.RemoveItem();
            Inventory.PlaceItem(itemSlot);
            return;
        }
        location.Structure.Inventory.RemoveItem(itemSlot);
        Inventory.PlaceItem(itemSlot);
    }
    #endregion Methods
}


public enum Gender : byte { Male, Female }

public enum Job : byte { None, Builder, Crafter, Miner, Botanist, Hunter }