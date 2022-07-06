using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CraftAction : Action
{
    #region Data
    private Structure workstation;
    private string itemNameToCraft;
    private ItemBlueprint blueprint;
    private ItemSlot craftedItem;

    private Item tool;

    private List<ItemSlot> itemsToUse;
    #endregion Data

    #region Properties
    protected override Vector2Int EndPoint { get => workstation.Position; }
    #endregion Properties


    #region Methods
    public CraftAction()
    {
        name = "CraftAction";
        mustBeInRange = true;
        pathFound = false;
        completed = false;

        itemsToUse = new List<ItemSlot>();
    }
    private CraftAction(Agent agent, Structure workstation, Item tool, string itemNameToCraft, List<ItemSlot> itemsToUse)
    {
        name = "CraftAction";
        this.agent = agent;
        mustBeInRange = true;
        pathFound = false;
        completed = false;

        this.workstation = workstation;
        this.itemNameToCraft = itemNameToCraft;
        this.tool = tool;
        this.itemsToUse = itemsToUse;


        this.workstation.AssignUser(agent);
        this.tool.AssignUser(agent);
        foreach(ItemSlot item in itemsToUse)
        {
            item.Item.AssignUser(agent);
        }
    }
    public override void LoadRequirementsAndEffect()
    {
        requirements = new List<string>();
        requirements.Add("work");
        requirements.Add("villageNeedsItem");

        effects = new Dictionary<string, StateEffect>();
        effects["villageNeedsItem"] = StateEffect.Remove;
    }
    public override void Reset()
    {
        itemNameToCraft = null;
        workstation = null;
        tool = null;
        itemsToUse = new List<ItemSlot>();
    }
    public override Action Initialize()
    {
        return new CraftAction(agent, workstation, tool, itemNameToCraft, itemsToUse);
    }

    public override bool IsAchievable()
    {
        Reset();
        if (agent.Job != Job.Crafter) return false;

        // Get a list of all items
        List<List<ItemSlot>> fullListOfItems = Utilities.GetAllItems(agent);

        // Find required tool
        System.Func<ItemSlot, bool> condition = (itemSlot) => itemSlot.Item.HasTag(ItemTag.Tool_Crafting);
        ItemSlot slot = Utilities.FindItemByCondition(fullListOfItems, condition);
        if (slot == null)
        {
            agent.Village.AddNeededItem("Hammer");
            return false;
        }
        tool = slot.Item;

        // Find required items
        itemsToUse = new List<ItemSlot>();
        foreach(string neededItem in agent.Village.NeededItems)
        {
            List<Ingredient> ingredientsRequired = Data.Items[neededItem].Recipe.Ingredients;

            bool ingredientsFound = true;
            foreach (Ingredient ingredient in ingredientsRequired)
            {
                condition = (itemSlot) => itemSlot.Item.HasTag(ItemTag.Material) && (itemSlot.Amount >= ingredient.Amount) && ingredient.CanUseMaterial(itemSlot.Item.Material);
                ItemSlot item = Utilities.FindItemByCondition(fullListOfItems, condition);

                if (item == null)
                {
                    foreach (MaterialType materialType in ingredient.MaterialTypes)
                    {
                        agent.Village.AddNeededResource(materialType);
                    }
                    ingredientsFound = false;
                    break;
                }
                itemsToUse.Add(new ItemSlot(item.Item, ingredient.Amount));
            }
            if (ingredientsFound)
            {
                itemNameToCraft = neededItem;
                return true;
            }
        }

        return false;
    }

    protected override bool PrePerform()
    {
        /*foreach (ItemSlot slot in itemsToUse)
        {
            if (slot.Item.Container != workstation)
            {
                //Action action = new StoreItemAction(agent, slot, );
            }
        }*/

        return true;
    }
    protected override void PostPerform()
    {
        /*agent.AI.CompleteAction();

        agent.Village.RemoveNeededItem(itemNameToCraft);

        Structure availableStorage = agent.Village.GetAvailableStorage();
        if (availableStorage != null)
        {
            StoreItemAction action = new StoreItemAction(agent, craftedItem, true, availableStorage, null);
            
            agent.AI.PushAction(action);
        }*/
    }

    protected override void Interact()
    {
        if (blueprint == null)
        {
            blueprint = new ItemBlueprint(Data.Items[itemNameToCraft]);
            blueprint.TryGiveItems(itemsToUse);
        }

        float workIncrement = Time.deltaTime * agent.Skills[SkillsEnum.Crafting].Level / 2;
        blueprint.Work(workIncrement);

        if (blueprint.Completed)
        {

            foreach (ItemSlot itemSlot in itemsToUse)
            {
                workstation.Inventory.RemoveItem(itemSlot);
            }

            Item item = ItemGenerator.GenerateItem(blueprint, agent);
            int itemAmount = blueprint.ItemData.Recipe.Yield;

            craftedItem = new ItemSlot(item, itemAmount);

            //workstation.Inventory.PlaceItem(item, itemAmount);
            agent.Inventory.PlaceItem(craftedItem);

            blueprint = null;
            completed = true;
        }
    }

    public override void ReleaseResources()
    {
        if (completed) agent.Village.AddNeededItem(itemNameToCraft);
        workstation.Release();
    }
    #endregion Methods
}