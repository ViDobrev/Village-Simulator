using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GetItemAction : Action
{
    #region Data
    private ItemSlot itemToGet;
    private Vector2Int lastPosition;
    #endregion Data

    #region Properties
    protected override Vector2Int EndPoint { get => itemToGet.Item.Position; }
    #endregion Properties


    #region Methods
    public GetItemAction(Agent agent, ItemSlot itemToGet)
    {
        name = "GetItemAction";
        this.agent = agent;
        mustBeInRange = true;
        pathFound = false;
        completed = false;

        this.itemToGet = itemToGet;
        lastPosition = itemToGet.Item.Position;
    }


    protected override bool PrePerform()
    {
        if (itemToGet.Item.IsInsideInventory)
        {
            if (itemToGet.Item.ParentInventory.ParentObject.GetType() == typeof(Structure))
            {
                bool storageIsStillThere = itemToGet.Item.ParentInventory.ParentObject.Location.Structure == itemToGet.Item.ParentInventory.ParentObject;
                if (!storageIsStillThere) return false;
            }
        }

        else return itemToGet.Item.Location.Item == itemToGet;

        return true;
    }
    protected override void PostPerform()
    {
        agent.AI.CompleteAction();
    }
    protected override void Interact()
    {
        agent.TakeItem(itemToGet);

        completed = true;
    }

    public override void ReleaseResources()
    {
        itemToGet.Item.Release();
    }
    #endregion Methods
}