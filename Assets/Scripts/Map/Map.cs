using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map
{
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    #region Data
    private int sizeX, sizeY;
    private MapCell[,] terrainMap;

    private StatesHolder states;
    #endregion Data

    #region Properties
    public MapCell[,] TerrainMap { get => terrainMap; }

    public StatesHolder States { get => states; }
    #endregion Properties


    #region Methods
    public Map(Vector2Int size, MapCell[,] terrainMap)
    {
        sizeX = size.x;
        sizeY = size.y;

        states = new StatesHolder();

        this.terrainMap = terrainMap;
    }

    public MapCell CellAtPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= sizeX || position.y < 0 || position.y >= sizeY)
            return null;

        return terrainMap[position.x, position.y];
    }


    public void MakeNeighbouringCellsVisible(MapCell centreCell)
    {
        if (!centreCell.Visible) return;

        foreach(Vector2Int direction in directions)
        {
            CellAtPosition(centreCell.Position + direction).SetVisible(true);
        }
    }
    public void UpdateNeighbouringCellsVisibility(MapCell initiatorCell)
    {
        foreach (Vector2Int direction in directions)
        {
            MapCell nextCell = CellAtPosition(initiatorCell.Position + direction);
            if (nextCell == null) continue;

            bool visible = false;
            foreach(Vector2Int _direction in directions)
            {
                MapCell checkCell = CellAtPosition(nextCell.Position + _direction);
                if (checkCell != null && checkCell.Visible)
                {
                    visible = !checkCell.HasStructure || (checkCell.Structure.Name != Data.Mountain);
                    if (visible) break;
                }
            }

            nextCell.SetVisible(visible);
        }
    }
    #endregion Methods
}