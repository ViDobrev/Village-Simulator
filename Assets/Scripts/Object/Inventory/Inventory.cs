using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Inventory
{
    #region Data
    private Object parentObject;
    private List<ItemSlot> storedItems;
    private int itemSlots;
    #endregion Data

    #region Properties
    public Object ParentObject { get => parentObject; }
    public int ItemSlots { get => itemSlots; }
    public int EmptySlots { get => itemSlots - storedItems.Count; }
    public bool IsEmpty { get => storedItems.Count == 0; }
    public bool IsFull { get => storedItems.Count == itemSlots; }

    public List<ItemSlot> StoredItems { get => storedItems;}
    #endregion Properties


    #region Methods
    public Inventory(int itemSlots)
    {
        storedItems = new List<ItemSlot>(itemSlots);
        this.itemSlots = itemSlots;
    }
    public void SetParentObject(Object parentObject)
    {
        this.parentObject = parentObject;
    }

    public bool PlaceItem(ItemSlot itemSlot)
    {
        if (EmptySlots > 0)
        {
            if (itemSlot.Item.Stackable)
            {
                for (int i = 0; i < storedItems.Count; i++)
                {
                    if (storedItems[i].Item.FullName == itemSlot.Item.FullName)
                    {
                        storedItems[i].Amount += itemSlot.Amount;
                        return true;
                    }
                }
            }

            storedItems.Add(itemSlot);
            itemSlot.Item.SetInventory(this);
            return true;
        }

        return false;
    }
    public bool PlaceItem(Item item)
    {
        if (EmptySlots > 0)
        {
            if (item.Stackable)
            {
                for (int i = 0; i < storedItems.Count; i++)
                {
                    if (storedItems[i].Item.FullName == item.FullName)
                    {
                        storedItems[i].Amount++;
                        return true;
                    }
                }
            }

            storedItems.Add(new ItemSlot(item, 1));
            item.SetInventory(this);
            return true;
        }

        return false;
    }
    public bool PlaceItem(Item item, int amount)
    {
        if (EmptySlots > 0)
        {
            if (item.Stackable)
            {
                for (int i = 0; i < storedItems.Count; i++)
                {
                    if (storedItems[i].Item.FullName == item.FullName)
                    {
                        storedItems[i].Amount += amount;
                        return true;
                    }
                }
            }

            storedItems.Add(new ItemSlot(item, amount));
            item.SetInventory(this);
            return true;
        }

        return false;
    }
    public ItemSlot RemoveItem(ItemSlot itemSlot)
    {
        for (int i = 0; i < storedItems.Count; i++)
        {
            if (storedItems[i].Item == itemSlot.Item)
            {
                ItemSlot returnSlot;

                if (storedItems[i].Item.Stackable && storedItems[i].Amount >= itemSlot.Amount)
                {
                    int leftoverAmount = storedItems[i].Amount - itemSlot.Amount;
                    if (leftoverAmount > 0)
                    {
                        storedItems[i].Amount = leftoverAmount;
                        Item item = ItemGenerator.CloneItem(itemSlot.Item);
                        item.SetInventory(null);
                        return new ItemSlot(item, itemSlot.Amount);
                    }

                    // no leftover => entire slot is taken
                    returnSlot = storedItems[i];
                    storedItems.RemoveAt(i);
                    returnSlot.Item.SetInventory(null);
                    return returnSlot;
                }

                else if (!storedItems[i].Item.Stackable)
                {
                    returnSlot = storedItems[i];
                    storedItems.RemoveAt(i);
                    returnSlot.Item.SetInventory(null);
                    return returnSlot;
                }
            }
        }

        Debug.LogError("Item not found in inventory.");
        return null;
    }
    
    public bool SwapItem(ref ItemSlot itemOutsideInventory, ref ItemSlot itemInsideInventory)
    {
        for (int i = 0; i < storedItems.Count; i++)
        {
            if (storedItems[i] == itemInsideInventory)
            {
                storedItems[i] = itemOutsideInventory;
                itemOutsideInventory = itemInsideInventory;

                storedItems[i].Item.SetInventory(this);
                itemOutsideInventory.Item.SetInventory(null);

                return true;
            }
        }

        return false;
    }
    #endregion Methods
}


public class ItemSlot
{
    #region Data
    private Item item;
    private int amount;
    #endregion Data

    #region Properties
    public Item Item { get => item; set => item = value; }
    public int Amount { get => amount; set => amount = value; }

    public string Name { get => item.Name; }
    public Color Colour { get => item.Colour; }
    public Tile Tile { get => item.Tile; }
    #endregion Properties


    #region Methods
    public ItemSlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    #endregion Methods
}