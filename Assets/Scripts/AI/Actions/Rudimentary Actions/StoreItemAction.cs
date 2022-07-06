using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StoreItemAction : Action
{
    #region Data
    private ItemSlot itemToStore;

    private Structure storage;
    private MapCell cellToDropItem;
    #endregion Data

    #region Properties
    protected override Vector2Int EndPoint
    {
        get
        {
            if (storage != null) return storage.Position;
            else return cellToDropItem.Position;
        }
    }
    #endregion Properties


    #region Methods
    public StoreItemAction(Agent agent, ItemSlot itemToStore, Structure storage, MapCell cellToDropItem)
    {
        name = "StoreItemAction";
        this.agent = agent;
        mustBeInRange = true;
        pathFound = false;
        completed = false;

        this.itemToStore = itemToStore;
        this.storage = storage;
        this.cellToDropItem = cellToDropItem;

        if (storage == null && cellToDropItem == null)
        {
            storage = agent.Village.GetAvailableStorage(agent);
            if (storage == null)
            {
                agent.Village.AddNeededRoom(RoomType.Storage);

                System.Func<MapCell, bool> condition = (cell) => cell.Traversible && !cell.HasStructure && !cell.HasPlant;
                cellToDropItem = Utilities.GetClosestCellByCondition(agent.Location, condition);

                if (cellToDropItem == null)
                {
                    agent.Village.States.AddState(Data.States["villageIsLittered"]);
                    mustBeInRange = false; // Basically just a Perform() skip
                    completed = true;
                }
            }
        }
    }

    protected override bool PrePerform()
    {
        if (itemToStore.Item.ParentInventory.ParentObject != agent)
        {
            GetItemAction action = new GetItemAction(agent, itemToStore);
        }

        if (storage == null && cellToDropItem == null)
        {
            storage = agent.Village.GetAvailableStorage(agent);
            if (storage == null)
            {
                agent.Village.AddNeededRoom(RoomType.Storage);

                System.Func<MapCell, bool> condition = (cell) => cell.Traversible && !cell.HasStructure && !cell.HasPlant;
                cellToDropItem = Utilities.GetClosestCellByCondition(agent.Location, condition);

                if (cellToDropItem == null)
                {
                    agent.Village.States.AddState(Data.States["villageIsLittered"]);
                    mustBeInRange = false; // Basically just a Perform() skip
                    completed = true;
                }
            }
        }

        if (storage != null)
        {
            if (storage.Location.Structure != storage || storage.Inventory.IsFull)
            {
                storage = null;
                return false;
            }
        }
        else
        {
            if (cellToDropItem.HasItem || cellToDropItem.HasStructure || cellToDropItem.HasPlant)
            {
                cellToDropItem = null;
                return false;
            }
        }

        return true;
    }
    protected override void PostPerform()
    {
        agent.AI.CompleteAction();
        // Empty, not needed
    }
    protected override void Interact()
    {
        agent.Inventory.RemoveItem(itemToStore);

        if (storage != null)
        {
            storage.Inventory.PlaceItem(itemToStore);
        }
        else
        {
            cellToDropItem.AddItem(itemToStore);
        }

        completed = true;
    }

    public override void ReleaseResources()
    {
        itemToStore.Item.Release();
    }
    #endregion Methods
}