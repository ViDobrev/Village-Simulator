using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Floor : Object
{
    #region Data
    private int durabilityMax, durabilityCurrent;
    //private float movementRequiredToTraverse;

    private MaterialData material;

    private string creatorName;
    #endregion Data

    #region Properties
    public string FullName { get => $"{material.Name} {name}"; }
    //public float MovementRequiredToTraverse { get => movementRequiredToTraverse; }

    public MaterialData Material { get => material; }

    public string CreatorName { get => creatorName; }
    #endregion Properties


    #region Methods
    public Floor(FloorData data, MaterialData material)
        : base(data.Name, material.Colour, data.Tile)
    {
        durabilityMax = durabilityCurrent = data.Durability * material.Toughness;
        this.material = material;
    }

    public void SignCreator(Agent creatorAgent)
    {
        creatorName = creatorAgent.Name;
    }

    public void Repair(int durability)
    {
        durabilityCurrent += durability;
        if (durabilityCurrent > durabilityMax) durabilityCurrent = durabilityMax;
    }
    public void TakeDamage(int damage)
    {
        durabilityCurrent -= damage;

        if (CheckFatality())
            Destroy();
    }
    private bool CheckFatality()
    {
        if (durabilityCurrent <= 0)
            return true;

        return false;
    }
    private void Destroy()
    {
        location.RemoveFloor();

        List<ItemSlot> itemsToDrop = new List<ItemSlot>();
        itemsToDrop.Add(new ItemSlot(ItemGenerator.GenerateItem(Data.Items[material.Name]), 1));

        InteractionHandler.DropItems(itemsToDrop, location.Position);
    }
    #endregion Methods
}