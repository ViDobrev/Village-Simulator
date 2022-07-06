using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Corpse : Object
{
    #region Data
    private Inventory inventory;
    #endregion Data

    #region Properties
    public Inventory Inventory { get => inventory; }
    #endregion Properties


    #region Methods
    public Corpse(Agent agent)
        : base(agent.Name + " corpse", Data.ColourDict[ColourEnum.Magenta], agent.Tile)
    {
        if (agent.Intelligent) name = $"{agent.Name}'s corpse";

        inventory = new Inventory(20);
        inventory.SetParentObject(this);
        if (agent.Intelligent)
        {
            foreach(ItemSlot slot in agent.Inventory.StoredItems)
                inventory.PlaceItem(slot);

            foreach(Item item in agent.Equipment.Slots.Values)
                if (item != null)
                    inventory.PlaceItem(item);
        }
        else
        {
            Item meat = ItemGenerator.GenerateItem(Data.Items["Meat"]);
            inventory.PlaceItem(meat, agent.Species.MeatAmount);
            Item leather = ItemGenerator.GenerateItem(Data.Items["Leather"]);
            inventory.PlaceItem(leather, agent.Species.LeatherAmount);
        }
    }
    #endregion Methods
}