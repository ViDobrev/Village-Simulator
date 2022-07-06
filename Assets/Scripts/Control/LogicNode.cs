using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LogicNode : MonoBehaviour
{
    #region Data
    private DataNode dataNode;
    private GUINode guiNode;
    private DisplayNode displayNode;

    private Map map;
    private Village village;

    private float updateTime = 0.25f;
    private float timeElapsed = 0f;
    private bool paused = false;
    #endregion Data


    #region Methods
    private void Initialize()
    {
        dataNode = FindObjectOfType<DataNode>();
        guiNode = FindObjectOfType<GUINode>();
        displayNode = FindObjectOfType<DisplayNode>();

        dataNode.Initialize(this, displayNode);
        guiNode.Initialize(this, dataNode, displayNode);
        displayNode.Initialize(this, dataNode);
    }
    private void SetMap()
    {
        dataNode.SetMap(map);
        guiNode.SetMap(map);
        displayNode.SetMap(map);

        Pathfinder.SetMap(map);
        Data.SetMap(map);
    }

    private void StartSimulation()
    {
        GenerateMap();
        CreateVillage();

        Vector2Int embarkPosition = FindEmbackPosition();
        GenerateInitialVillagers(embarkPosition);
        TogglePause();
    }

    private void GenerateMap()
    {
        MapGenerator mapGenerator = new MapGenerator();
        map = mapGenerator.GenerateMap(dataNode);
        SetMap();
        displayNode.DisplayMap();
    }
    private void CreateVillage()
    {
        village = new Village();
        Data.SetVillage(village);
    }

    private Vector2Int FindEmbackPosition()
    {
        int mapSizeX = map.TerrainMap.GetLength(0);
        int mapSizeY = map.TerrainMap.GetLength(1);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        Vector2Int[] neighbourCells =
        {
            Vector2Int.up,          // N
            new Vector2Int( 1,  1), // NE
            Vector2Int.right,       // E
            new Vector2Int( 1, -1), // SE
            Vector2Int.down,        // S
            new Vector2Int(-1, -1), // SW
            Vector2Int.left,        // W
            new Vector2Int(-1,  1)  // NW
        };

        Vector2Int centre = new Vector2Int(mapSizeX / 2, mapSizeY / 2);
        if (CheckNeighbourCells(neighbourCells, centre)) return centre;

        bool done = false;

        int steps = 1;
        int direction = 0;
        int iter = 0;

        while (!done)
        {
            try
            {
                for (int i = 0; i < steps; i++)
                {
                    centre += directions[direction];
                    if (CheckNeighbourCells(neighbourCells, centre)) return centre;
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
            catch
            {
                break;
            }
        }

        Debug.LogError("Could not find suitable embark position.");
        return new Vector2Int(mapSizeX / 2, mapSizeY / 2);
    }
    private bool CheckNeighbourCells(Vector2Int[] neighbourCells, Vector2Int centrePosition)
    {
        MapCell cell = map.CellAtPosition(centrePosition);
        if (!cell.IsFree) return false;

        for (int i = 0; i < neighbourCells.Length; i++)
        {
            cell = map.CellAtPosition(centrePosition + neighbourCells[i]);
            if (!cell.IsFree) return false;
        }

        return true;
    }

    private void GenerateInitialVillagers(Vector2Int embarkPosition)
    {// TODO I DON'T LIKE THIS FUNCTION - FIX
        Vector2Int[] directions =
        {
            Vector2Int.left,
            new Vector2Int(-1, 1),
            Vector2Int.up,
            new Vector2Int(1, 1),
            Vector2Int.right
        };

        // Generate the initial chest
        int woodAmount = Data.Structures["Chest"].Schematic.Component.Amount;
        ItemSlot woodForChest = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Wood"]), woodAmount);
        Structure chest = StructureGenerator.GenerateStructure(Data.Structures["Chest"], woodForChest, null);
        MapCell cell = map.CellAtPosition(embarkPosition);
        cell.AddStructure(chest);
        village.AddOutdoorsStructure(chest);

        // Generate 2xPickaxe, 2xHammer, 1xAxe to place in the chest
        string[] itemNames = { "Pickaxe", "Pickaxe", "Hammer", "Hammer", "Axe" };

        // Generate the stone for the tools
        int amountForPickaxe = Data.Items["Pickaxe"].Recipe.MainIngredient.Amount;
        int amountForHammer = Data.Items["Hammer"].Recipe.MainIngredient.Amount;
        int amountForAxe = Data.Items["Axe"].Recipe.MainIngredient.Amount;

        ItemSlot stoneForPickaxe = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Stone"]), amountForPickaxe);
        ItemSlot stoneForHammer = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Stone"]), amountForHammer);
        ItemSlot stoneForAxe = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Stone"]), amountForAxe);

        Dictionary<string, List<ItemSlot>> stoneForTools = new Dictionary<string, List<ItemSlot>>();
        stoneForTools["Pickaxe"] = new List<ItemSlot>() { stoneForPickaxe };
        stoneForTools["Hammer"] = new List<ItemSlot>() { stoneForHammer };
        stoneForTools["Axe"] = new List<ItemSlot>() { stoneForAxe };

        // Generate the tools and place them in the chest
        for (int i = 0; i < itemNames.Length; i++)
        {
            Item item = ItemGenerator.GenerateItem(Data.Items[itemNames[i]], stoneForTools[itemNames[i]], null);
            chest.Inventory.PlaceItem(item);
        }


        // Generate the initial agents
        // Generate the cloth for their initial clothing
        int amountForTunic = Data.Items["Tunic"].Recipe.MainIngredient.Amount;
        int amountForTrousers = Data.Items["Trousers"].Recipe.MainIngredient.Amount;
        int amountForBoots = Data.Items["Boots"].Recipe.MainIngredient.Amount;

        ItemSlot clothForTunic = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Cloth"]), amountForTunic);
        ItemSlot clothForTrousers = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Cloth"]), amountForTrousers);
        ItemSlot clothForBoots = new ItemSlot(ItemGenerator.GenerateItem(Data.Items["Cloth"]), amountForBoots);

        Dictionary<string, List<ItemSlot>> clothForClothing = new Dictionary<string, List<ItemSlot>>();
        clothForClothing["Tunic"] = new List<ItemSlot>() { clothForTunic };
        clothForClothing["Trousers"] = new List<ItemSlot>() { clothForTrousers };
        clothForClothing["Boots"] = new List<ItemSlot>() { clothForBoots };

        // Generate the agents
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int agentPosition = embarkPosition + directions[i];
            cell = map.CellAtPosition(agentPosition);

            Item tunic = ItemGenerator.GenerateItem(Data.Items["Tunic"], clothForClothing["Tunic"], null);
            Item trousers = ItemGenerator.GenerateItem(Data.Items["Trousers"], clothForClothing["Trousers"], null);
            Item boots = ItemGenerator.GenerateItem(Data.Items["Boots"], clothForClothing["Boots"], null);

            List<Item> itemsForAgent = new List<Item>() { tunic, trousers, boots };

            Agent agent = AgentGenerator.GenerateAgent(Data.Species["Human"], itemsForAgent);

            Data.AddAgent(agent);
            Data.Village.AddAgent(agent);
            cell.AddAgent(agent);
        }
    }

    public void TogglePause()
    {
        paused = !paused;
        if (paused) Debug.Log("Paused");
        else Debug.Log("Unpaused");
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
        Initialize();
        StartSimulation();
    }

    void Update()
    {
        if (paused) return;        

        timeElapsed += Time.deltaTime;
        if (timeElapsed < updateTime) return;
        timeElapsed = 0f;

        //Random Event

        foreach (Agent agent in Data.Agents)
        {
            agent.Act();
        }
        foreach(Agent animal in Data.Animals)
        {
            animal.Act();
        }
        foreach(Plant plant in Data.Plants)
        {
            plant.Grow();
        }
    }
    #endregion Behaviour
}