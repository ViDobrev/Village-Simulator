using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class InteractionHandler
{
    #region Methods
    public static void DropItems(List<ItemSlot> itemsToDrop, Vector2Int position)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        int droppedItems = 0;
        // Attempt to drop item at given location
        if (DropItem(itemsToDrop[droppedItems], position)) droppedItems++;

        int direction = 0;
        int steps = 1;
        int iter = 0;
        //Spiral pattern
        while (droppedItems != itemsToDrop.Count)
        {
            for (int i = 0; i < steps; i++)
            {
                position += directions[direction];
                if (DropItem(itemsToDrop[droppedItems], position)) droppedItems++;
                if (droppedItems == itemsToDrop.Count) break;
            }

            direction++;
            if (direction > directions.Length - 1) direction = 0;

            iter++;
            if (iter == 2)
            {
                steps++;
                iter = 0;
            }
        }
    }

    private static bool DropItem(ItemSlot itemToDrop, Vector2Int position)
    {
        MapCell cell = Data.Map.CellAtPosition(position);

        if (cell.AddItem(itemToDrop)) return true;

        return false;
    }
    #endregion Methods
}