using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator
{
    #region Methods
    public Map GenerateMap(DataNode dataNode)
    {
        Vector2Int mapSize = dataNode.MapSize;
        float minMountainHeight = GetMinMountainHeight(dataNode.Regions);


        float[,] noiseMap;
        List<Vector2Int> naturalStructuresPositions;
        List<Vector2Int> faunaPositions;
        MapCell[,] terrainMap;


        //Request Noise
        NoiseGenerator noiseGenerator = new NoiseGenerator();
        noiseMap = noiseGenerator.GenerateNoiseMap(dataNode.NoiseData, mapSize.x, mapSize.y);
        naturalStructuresPositions = noiseGenerator.GenerateNaturalStrucutresPositions(dataNode.NoiseData, mapSize.x, mapSize.y);
        faunaPositions = noiseGenerator.GenerateInitialFaunaPositions(dataNode.NoiseData, mapSize.x, mapSize.y);

        //Turn the noiseMap into an array of map cells
        terrainMap = GenerateTerrainArray(noiseMap, mapSize.x, mapSize.y, dataNode.Regions);
        GenerateMountains(terrainMap, noiseMap, minMountainHeight);
        GenerateNaturalStructures(terrainMap, naturalStructuresPositions);
        GenerateFauna(terrainMap, faunaPositions);

        return new Map(mapSize, terrainMap);
    }

    private float GetMinMountainHeight(Region[] regions)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (regions[i].name == Data.Mountain)
                return regions[i].height;
        }
        return 0f;
    }

    private MapCell[,] GenerateTerrainArray(float[,] worldNoiseMap, int mapSizeX, int mapSizeY, Region[] regions)
    {
        MapCell[,] worldTerrainArray = new MapCell[mapSizeX, mapSizeY];

        for (int y = 0; y < mapSizeY; y++)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                foreach (var region in regions)
                {
                    if (worldNoiseMap[x, y] <= region.height)
                    {
                        worldTerrainArray[x, y] = new MapCell(new Vector2Int(x, y), Data.Terrains[region.name]);
                        break;
                    }
                }
            }
        }
        return worldTerrainArray;
    }

    private void GenerateMountains(MapCell[,] terrainMap, float[,] noiseMap, float minMountainHeight)
    {
        int mapSizeX = terrainMap.GetLength(0);
        int mapSizeY = terrainMap.GetLength(1);

        HashSet<Vector2Int> mountainPositions = new HashSet<Vector2Int>();
        //Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        Vector2Int[] directions = {Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        for (int y = 0; y < mapSizeY; y++)
        {
            for (int x = 0; x < mapSizeX; x++)
            {
                if (noiseMap[x, y] >= minMountainHeight)
                    mountainPositions.Add(new Vector2Int(x, y));
            }
        }

        foreach (var position in mountainPositions)
        {
            bool visible = false;

            foreach (var direction in directions)
            {
                var neighbour = direction + position;

                if (neighbour.x < 0 || neighbour.x >= noiseMap.GetLength(0) || neighbour.y < 0 || neighbour.y >= noiseMap.GetLength(1)) continue;

                if (!mountainPositions.Contains(neighbour))
                {
                    visible = true;
                    break;
                }
            }
            terrainMap[position.x, position.y].SetVisible(visible);

            int stoneAmount = Data.Structures["Mountain"].Schematic.Component.Amount;
            ItemSlot mountainRock = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Stone"]), stoneAmount);
            Structure mountain = StructureGenerator.GenerateStructure(Data.Structures["Mountain"], mountainRock, null);
            terrainMap[position.x, position.y].AddStructure(mountain);
        }
    }

    private void GenerateNaturalStructures(MapCell[,] terrainMap, List<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            MapCell cell = terrainMap[position.x, position.y];

            if (cell.Terrain.Name == "Grass Terrain")
            {
                Plant plant;
                int rng = Data.mapRng.Next() % 2;

                if (rng == 0)
                {
                    plant = PlantGenerator.GeneratePlant(Data.PlantSpecies["Tree"]);
                }
                else
                {
                    plant = PlantGenerator.GeneratePlant(Data.PlantSpecies["Berry Bush"]);
                }

                Data.AddPlant(plant);
                cell.AddPlant(plant);
            }
            else if (cell.Terrain.Name == "Stone Terrain")
            {
                int stoneAmount = Data.Structures["Rock"].Schematic.Component.Amount;
                ItemSlot stone = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Stone"]), stoneAmount);
                Structure rock = StructureGenerator.GenerateStructure(Data.Structures["Rock"], stone, null);
                    
                cell.AddStructure(rock);
            }
        }
    }

    private void GenerateFauna(MapCell[,] terrainMap, List<Vector2Int> positions)
    {
        // Rabbit, Deer, Wolf
        int[] faunaSpeciesPointers = { 0, 1, 2 };
        int[] faunaDistribution = { 5, 3, 2 };

        foreach (Vector2Int position in positions)
        {
            MapCell cell = terrainMap[position.x, position.y];
            if (!cell.Traversible) continue;

            int rng = Utilities.RandomNumberByPropbability(faunaSpeciesPointers, faunaDistribution);

            if (rng == 0)
            {
                Agent rabbit = AgentGenerator.GenerateAgent(Data.Species["Rabbit"], null);

                Data.AddAgent(rabbit);
                cell.AddAgent(rabbit);
            } // Rabbit
            else if (rng == 1)
            {
                Agent deer = AgentGenerator.GenerateAgent(Data.Species["Deer"], null);

                Data.AddAgent(deer);
                cell.AddAgent(deer);
            } // Deer
            else if (rng == 2)
            {
                Agent wolf = AgentGenerator.GenerateAgent(Data.Species["Wolf"], null);

                Data.AddAgent(wolf);
                cell.AddAgent(wolf);
            } // Wolf
        }
    }
    #endregion Methods
}