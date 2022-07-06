using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class Data
{
    #region Data
    private static DataNode dataNode;

    private static Map map;
    private static Village village;
    private static Tilemap tilemap;
    //private static Tile wall_16x16_tile;

    private static List<Agent> agentsHolder;
    private static List<Agent> animalsHolder;
    private static List<Plant> plantsHolder;
    private static List<Corpse> corpsesHolder;
    private static List<ItemSlot> itemsOnGround;
    private static List<Blueprint> blueprintsHolder;
    private static List<RoomBlueprint> roomBlueprintsHolder;

    public static System.Random mapRng;
    public static System.Random agentRng;
    public static System.Random diceRng;
    public static System.Random mathsRng;
    public static System.Random aiRng;

    private static Dictionary<ColourEnum, Color> colourDict = new Dictionary<ColourEnum, Color>()
    {
        {ColourEnum.White,      Color.white     },
        {ColourEnum.Green,      Color.green     },
        {ColourEnum.Yellow,     Color.yellow    },
        {ColourEnum.Blue,       Color.blue      },
        {ColourEnum.Magenta,    Color.magenta   },
        {ColourEnum.Red,        Color.red       },
        {ColourEnum.Grey,       Color.grey      },
        {ColourEnum.Brown,      new Color(0.59f, 0.29f, 0)}
    };

    private static Dictionary<string, MyTerrain> terrains;
    private static Dictionary<string, MaterialData> materials;
    private static Dictionary<string, FoodData> foods;
    private static Dictionary<string, StructureData> structures;
    private static Dictionary<string, FloorData> floors;
    private static Dictionary<string, ItemData> items;
    private static Dictionary<string, Species> species;
    private static Dictionary<string, PlantSpecies> plantSpecies;
    private static Dictionary<string, State> states;

    private static Dictionary<string, Tile> tiles;

    #endregion Data

    #region Properties
    public static Map Map { get => map; }
    public static Village Village { get => village; }

    public static List<Agent> Agents { get => agentsHolder; }
    public static List<Agent> Animals { get => animalsHolder; }
    public static List<Plant> Plants { get => plantsHolder; }
    public static List<Corpse> Corpses { get => corpsesHolder; }
    public static List<ItemSlot> ItemsOnGround { get => itemsOnGround; }
    public static List<Blueprint> Blueprints { get => blueprintsHolder; }
    public static List<RoomBlueprint> RoomBlueprints { get => roomBlueprintsHolder; }

    public static string PathToXmlDataBase { get => Application.dataPath + @"\Database\XML Database\"; }
    public static string Unknown { get => "Unknown"; }
    public static string Mountain { get => "Mountain"; }

    public static int VillagerInventorySlots { get => 5; }

    public static Dictionary<ColourEnum, Color> ColourDict { get => colourDict; }

    public static Dictionary<string, MyTerrain> Terrains { get => terrains; }
    public static Dictionary<string, MaterialData> Materials { get => materials; }
    public static Dictionary<string, FoodData> Foods { get => foods; }
    public static Dictionary<string, StructureData> Structures { get => structures; }
    public static Dictionary<string, FloorData> Floors { get => floors; }
    public static Dictionary<string, ItemData> Items { get => items; }
    public static Dictionary<string, Species> Species { get => species; }
    public static Dictionary<string, PlantSpecies> PlantSpecies { get => plantSpecies;}
    public static Dictionary<string, State> States { get => states; }

    public static Dictionary<string, Tile> Tiles { get => tiles; }
    #endregion Properties


    #region Methods
    public static void Initialize(int mainSeed, TileManager tileManager, DataNode _dataNode)
    {
        dataNode = _dataNode;

        System.Random initialRng = new System.Random(mainSeed);
        mapRng = new System.Random(initialRng.Next());
        agentRng = new System.Random(initialRng.Next());
        diceRng = new System.Random(initialRng.Next());
        mathsRng = new System.Random(initialRng.Next());
        aiRng = new System.Random();

        agentsHolder = new List<Agent>();
        animalsHolder = new List<Agent>();
        plantsHolder = new List<Plant>();
        corpsesHolder = new List<Corpse>();
        itemsOnGround = new List<ItemSlot>();
        blueprintsHolder = new List<Blueprint>();
        roomBlueprintsHolder = new List<RoomBlueprint>();

        LoadAssets(tileManager);
    }
    public static void SetMap(Map _map)
    {
        map = _map;
    }
    public static void SetVillage(Village _village)
    {
        village = _village;
    }
    public static void SetTilemap(Tilemap _tilemap)
    {
        tilemap = _tilemap;
    }
    public static void LoadAssets(TileManager tileManager)
    {
        LoadTiles(tileManager);
        LoadTerrains();
        LoadMaterials();
        LoadFoods();
        LoadItems();
        LoadStructures();
        LoadFloors();
        LoadSpecies();
        LoadPlantSpecies();
        LoadStates();
        LoadRooms();
    }


    private static void LoadTiles(TileManager tileManager)
    {
        tiles = new Dictionary<string, Tile>();

        foreach (var tilePackArray in tileManager.tilePacks)
        {
            foreach (var tilePack in tilePackArray.tilePacks)
            {
                tiles.Add(tilePack.name, tilePack.tile);
            }
        }
    }
    private static void LoadTerrains()
    {
        terrains = new Dictionary<string, MyTerrain>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Terrains.xml");

        foreach (var terrain in xmlString.Descendants("Terrain"))
        {
            string name = terrain.Descendants("Name").First().Value;
            bool traversible = terrain.Descendants("Traversible").First().Value == "True";
            if (!Enum.TryParse(terrain.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Terrain ({name}). Colour could not be parsed.");
            }

            terrains.Add(name, new MyTerrain(name, traversible, tiles[name], colourDict[colour]));
        }
    }
    private static void LoadMaterials()
    {
        materials = new Dictionary<string, MaterialData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Materials.xml");

        foreach (var material in xmlString.Descendants("Material"))
        {
            string name = material.Descendants("Name").First().Value;
            if (!Enum.TryParse(material.Descendants("Type").First().Value, out MaterialType type))
            {
                Debug.LogError($"Material ({name}). Type could not be parsed.");
            }
            string adjective = material.Descendants("Adjective").First().Value;
            if (!int.TryParse(material.Descendants("Toughness").First().Value, out int toughnesss))
            {
                Debug.LogError($"Material ({name}). Toughness could not be parsed.");
            }
            if (!int.TryParse(material.Descendants("Value").First().Value, out int value))
            {
                Debug.LogError($"Material ({name}). Value could not be parsed.");
            }
            if (!int.TryParse(material.Descendants("DamageModifier").First().Value, out int damageModifier))
            {
                Debug.LogError($"Material ({name}). DamageModifier could not be parsed.");
            }
            if (!int.TryParse(material.Descendants("ArmourModifier").First().Value, out int armourModifier))
            {
                Debug.LogError($"Material ({name}). ArmourModifier could not be parsed.");
            }
            if (!Enum.TryParse(material.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Material ({name}). Colour could not be parsed.");
            }

            materials.Add(name, new MaterialData(name, type, adjective, toughnesss, value, damageModifier, armourModifier, colour));
        }
    }
    private static void LoadFoods()
    {
        foods = new Dictionary<string, FoodData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Foods.xml");

        foreach (var food in xmlString.Descendants("Food"))
        {
            string name = food.Descendants("Name").First().Value;
            if (!Enum.TryParse(food.Descendants("Type").First().Value, out FoodType type))
            {
                Debug.LogError($"Food ({name}). Type could not be parsed.");
            }
            if (!int.TryParse(food.Descendants("Nutrition").First().Value, out int nutrition))
            {
                Debug.LogError($"Food ({name}). Nutrition could not be parsed.");
            }

            foods.Add(name, new FoodData(nutrition, type));
        }
    }
    private static void LoadItems()
    {// TODO THIS IS AWFUL MUST FIX
        items = new Dictionary<string, ItemData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Items.xml");

        foreach (var item in xmlString.Descendants("Item"))
        {
            string name = item.Descendants("Name").First().Value;
            bool stackable = item.Descendants("Stackable").First().Value == "True";

            // All data for a material item is acquired. If the item is a material load it now and continue onto next item
            if (materials.ContainsKey(name))
            {
                items.Add(name, new ItemData(name, stackable, materials[name]));
                continue;
            }

            if (!int.TryParse(item.Descendants("Value").First().Value, out int value))
            {
                Debug.LogError($"Item ({name}). Value could not be parsed.");
            }

            // All data for a food item is acquired. If the item is food load it now and continue onto next item
            if (foods.ContainsKey(name))
            {
                items.Add(name, new ItemData(name, value, stackable, foods[name]));
                continue;
            }

            List<ItemTag> tags = new List<ItemTag>();
            foreach (var tagNode in item.Descendants("Tag"))
            {
                if (!Enum.TryParse(tagNode.Value, out ItemTag tag))
                {
                    Debug.LogError($"Item ({name}). Tag could not be parsed.");
                }
                else tags.Add(tag);
            }


            List<string> attacks = null;
            var attackNodes = item.Descendants("Attack").ToList();
            if (attackNodes.Count > 0)
            {
                attacks = new List<string>();
                foreach (var attack in attackNodes)
                {
                    attacks.Add(attack.Value);
                }
            }

            int defence = 0;
            var defenceNodes = item.Descendants("Defence").ToList();
            if (defenceNodes.Count > 0)
            {
                if (!int.TryParse(defenceNodes.First().Value, out defence))
                {
                    Debug.LogError($"Item ({name}). Defence could not be parsed.");
                }
            }

            bool twoHanded = false;
            if (item.Descendants("TwoHanded").ToList().Count > 0)
                twoHanded = item.Descendants("TwoHanded").First().Value == "True";

            if (!Enum.TryParse(item.Descendants("EquipmentSlot").First().Value, out EquipmentSlot equipmentSlot))
            {
                Debug.LogError($"Item ({name}). EquipmentSlot could not be parsed.");
            }


            // Load item recipe
            var recipeNode = item.Descendants("Recipe").First();
            List<Ingredient> recipeIngredients = new List<Ingredient>();
            foreach (var ingredientNode in recipeNode.Descendants("Ingredient"))
            {
                List<MaterialType> materialTypes = new List<MaterialType>();
                foreach (string _materialType in ingredientNode.Value.Split(' '))
                {
                    if (!Enum.TryParse(_materialType, out MaterialType materialType))
                    {
                        Debug.LogError($"Item ({name}). Recipe: MaterialType({_materialType}) could not be parsed.");
                    }
                    materialTypes.Add(materialType);
                }

                if (!int.TryParse(ingredientNode.Attribute("Amount").Value, out int amount))
                {
                    Debug.LogError($"Item ({name}). Recipe: Amount could not be parsed.");
                }
                bool main = ingredientNode.Attribute("Main").Value == "True";

                recipeIngredients.Add(new Ingredient(materialTypes, amount, main));
            }
            if (!int.TryParse(recipeNode.Descendants("Yield").First().Value, out int yield))
            {
                Debug.LogError($"Item ({name}). Recipe: Yield could not be parsed.");
            }
            if (!int.TryParse(recipeNode.Descendants("Work").First().Value, out int work))
            {
                Debug.LogError($"Item ({name}). Recipe: WorkRequired could not be parsed.");
            }


            Recipe recipe = new Recipe(recipeIngredients, yield, work);

            // Load the item data
            items.Add(name, new ItemData(name, value, tags, stackable, attacks, defence, twoHanded, equipmentSlot, recipe));
        }
    }
    private static void LoadStructures()
    {// TODO THIS IS AN AWFUL FUNCTION MUST FIX
        structures = new Dictionary<string, StructureData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Structures.xml");

        foreach (var structure in xmlString.Descendants("Structure"))
        {
            string name = structure.Descendants("Name").First().Value;
            StructureTag tag = GetXmlValue<StructureTag>(structure, "Tag", "Structure", name);
            int durability = GetXmlValue<int>(structure, "Durability", "Structure", name);
            int value = GetXmlValue<int>(structure, "Value", "Structure", name);
            int inventorySlots = GetXmlValue<int>(structure, "InventorySlots", "Structure", name);


            // Load structure schematic
            var componentNode = structure.Descendants("Component").First();
            List<MaterialType> materialTypes = new List<MaterialType>();
            foreach (string _materialType in componentNode.Value.Split(' '))
            {
                if (!Enum.TryParse(_materialType, out MaterialType materialType))
                {
                    Debug.LogError($"Item ({name}). Recipe: MaterialType({_materialType}) could not be parsed.");
                }
                materialTypes.Add(materialType);
            }
            if (!int.TryParse(componentNode.Attribute("Amount").Value, out int amount))
            {
                Debug.LogError($"Item ({name}). Recipe: Amount could not be parsed.");
            }

            Component schematicComponent = new Component(materialTypes, amount);
            if (!int.TryParse(componentNode.Attribute("Work").Value, out int workRequired))
            {
                Debug.LogError($"Item ({name}). Recipe: WorkRequired could not be parsed.");
            }


            Schematic schematic = new Schematic(schematicComponent, workRequired);

            // Load the structure data
            structures.Add(name, new StructureData(name, tag, durability, value, inventorySlots, schematic, tiles[name]));
        }
    }
    private static void LoadFloors()
    {
        floors = new Dictionary<string, FloorData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Structures.xml");

        foreach (var structure in xmlString.Descendants("Structure"))
        {
            string name = structure.Descendants("Name").First().Value;
            int durability = GetXmlValue<int>(structure, "Durability", "Structure", name);


            // Load structure schematic
            var componentNode = structure.Descendants("Component").First();
            List<MaterialType> materialTypes = new List<MaterialType>();
            foreach (string _materialType in componentNode.Value.Split(' '))
            {
                if (!Enum.TryParse(_materialType, out MaterialType materialType))
                {
                    Debug.LogError($"Item ({name}). Recipe: MaterialType({_materialType}) could not be parsed.");
                }
                materialTypes.Add(materialType);
            }
            if (!int.TryParse(componentNode.Attribute("Amount").Value, out int amount))
            {
                Debug.LogError($"Item ({name}). Recipe: Amount could not be parsed.");
            }

            Component schematicComponent = new Component(materialTypes, amount);
            if (!int.TryParse(componentNode.Attribute("Work").Value, out int workRequired))
            {
                Debug.LogError($"Item ({name}). Recipe: WorkRequired could not be parsed.");
            }


            Schematic schematic = new Schematic(schematicComponent, workRequired);

            // Load the floor data
            floors.Add(name, new FloorData(name, durability, schematic, tiles[name]));
        }
    }
    private static void LoadSpecies()
    {
        species = new Dictionary<string, Species>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Species.xml");

        foreach (var speciesNode in xmlString.Descendants("Species"))
        {
            string name = speciesNode.Descendants("Name").First().Value;
            bool intelligent = speciesNode.Descendants("Intelligent").First().Value == "True";
            if (!int.TryParse(speciesNode.Descendants("Health").First().Value, out int health))
            {
                Debug.LogError($"Species ({name}). Health could not be parsed.");
            }
            if (!float.TryParse(speciesNode.Descendants("Energy").First().Value, out float energy))
            {
                Debug.LogError($"Species ({name}). Energy could not be parsed.");
            }
            if (!float.TryParse(speciesNode.Descendants("Speed").First().Value, out float speed))
            {
                Debug.LogError($"Species ({name}). Speed could not be parsed.");
            }
            if (!float.TryParse(speciesNode.Descendants("HungerRate").First().Value, out float hungerRate))
            {
                Debug.LogError($"Species ({name}). HungerRate could not be parsed.");
            }

            string unarmedAttackString = speciesNode.Descendants("UnarmedAttack").First().Value;
            Attack unarmedAttack = new Attack(unarmedAttackString, 0);

            List<MovementType> movementTypes = new List<MovementType>();
            foreach (var movementTypeNode in speciesNode.Descendants("MovementType"))
            {
                if (!Enum.TryParse(movementTypeNode.Value, out MovementType movementType))
                {
                    Debug.LogError($"Species ({name}). MovementType could not be parsed.");
                }
                else movementTypes.Add(movementType);
            }

            if (!int.TryParse(speciesNode.Descendants("MeatAmount").First().Value, out int meatAmount))
            {
                Debug.LogError($"Species ({name}). MeatAmount could not be parsed.");
            }
            if (!int.TryParse(speciesNode.Descendants("LeatherAmount").First().Value, out int leatherAmount))
            {
                Debug.LogError($"Species ({name}). LeatherAmount could not be parsed.");
            }

            if (!Enum.TryParse(speciesNode.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Species ({name}). Colour could not be parsed.");
            }

            species.Add(name, new Species(name, intelligent, health, energy, speed, hungerRate, unarmedAttack, movementTypes, meatAmount, leatherAmount, colourDict[colour], tiles[name]));
        }
    }
    private static void LoadPlantSpecies()
    {
        plantSpecies = new Dictionary<string, PlantSpecies>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Plant Species.xml");

        foreach (var speciesNode in xmlString.Descendants("Species"))
        {
            string name = speciesNode.Descendants("Name").First().Value;
            if (!int.TryParse(speciesNode.Descendants("Health").First().Value, out int health))
            {
                Debug.LogError($"Plant Species ({name}). Health could not be parsed.");
            }
            if (!float.TryParse(speciesNode.Descendants("GrowingRate").First().Value, out float growingRate))
            {
                Debug.LogError($"Plant Species ({name}). GrowingRate could not be parsed.");
            }
            ItemData resource = items[speciesNode.Descendants("Resource").First().Value];
            if (!Enum.TryParse(speciesNode.Descendants("GatherMethod").First().Value, out GatherMethod gatherMethod))
            {
                Debug.LogError($"Plant Species ({name}). GatherMethod could not be parsed.");
            }

            Dictionary<PlantStage, int> yieldByStage = new Dictionary<PlantStage, int>();
            foreach (var yieldNode in speciesNode.Descendants("Yield"))
            {
                if (!Enum.TryParse(yieldNode.Attribute("Stage").Value, out PlantStage plantStageName))
                {
                    Debug.LogError($"Plant Species ({name}). LifeCycle. Name could not be parsed.");
                }
                if (!int.TryParse(yieldNode.Value, out int _yield))
                {
                    Debug.LogError($"Plant Species ({name}). LifeCycle. Yield could not be parsed.");
                }

                yieldByStage.Add(plantStageName, _yield);
            }

            if (!Enum.TryParse(speciesNode.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Plant Species ({name}). Colour could not be parsed.");
            }

            plantSpecies.Add(name, new PlantSpecies(name, health, growingRate, resource, gatherMethod, yieldByStage, colourDict[colour], tiles[name]));
        }
    }
    private static void LoadStates()
    {
        states = new Dictionary<string, State>();

        var xmlString = XElement.Load(PathToXmlDataBase + "States.xml");

        foreach (var state in xmlString.Descendants("State"))
        {
            string name = state.Value;
            if (!float.TryParse(state.Attribute("Priority").Value, out float priority))
            {
                Debug.LogError($"State ({name}). Priority could not be parsed.");
            }

            states.Add(name, new State(name, priority));
        }
    }
    private static void LoadRooms()
    {
        Dictionary<RoomType, string[][]> roomBlueprints = new Dictionary<RoomType, string[][]>();
        Dictionary<RoomType, Vector2Int> roomSizes = new Dictionary<RoomType, Vector2Int>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Rooms.xml");

        foreach (var room in xmlString.Descendants("Room"))
        {
            if (!Enum.TryParse(room.Descendants("Type").First().Value, out RoomType type))
            {
                string roomTypeName = room.Descendants("Type").First().Value;
                Debug.LogError($"Room ({roomTypeName}). Type could not be parsed.");
            }

            string sizeString = room.Descendants("Size").First().Value;
            if (!int.TryParse(sizeString.Split('x')[0], out int sizeX))
            {
                string roomTypeName = room.Descendants("Type").First().Value;
                Debug.LogError($"Room ({roomTypeName}). SizeX could not be parsed.");
            }
            if (!int.TryParse(sizeString.Split('x')[1], out int sizeY))
            {
                string roomTypeName = room.Descendants("Type").First().Value;
                Debug.LogError($"Room ({roomTypeName}). SizeY could not be parsed.");
            }
            Vector2Int roomSize = new Vector2Int(sizeX, sizeY);

            string[][] blueprints = new string[sizeX][];
            int i = 0;
            foreach (var blueprintsNode in room.Descendants("Structures"))
            {
                blueprints[i] = blueprintsNode.Value.Split(' ');
                i++;
            }

            roomBlueprints.Add(type, blueprints);
            roomSizes.Add(type, roomSize);
        }

        RoomBlueprint.LoadRoomData(roomBlueprints, roomSizes);
    }


    private static T GetXmlValue<T>(XElement mainNode, string valueName, string objectType, string objectName)
    {
        var targetNode = mainNode.Descendants(valueName).FirstOrDefault();
        if (targetNode == null) return default(T);

        try
        {
            T returnValue;
            if (typeof(T).GetTypeInfo().IsEnum)
                returnValue = (T)Enum.Parse(typeof(T), targetNode.Value);
            else
                returnValue = (T)Convert.ChangeType(targetNode.Value, typeof(T));
            return returnValue;
        }
        catch
        {
            Console.WriteLine($"{objectType} ({objectName}). {valueName} could not be parsed.");
            return default(T);
        }
    }



    public static void AddAgent(Agent agent)
    {
        if (agent.Intelligent)
            agentsHolder.Add(agent);
        else
            animalsHolder.Add(agent);
    }
    public static void AddPlant(Plant plant)
    {
        plantsHolder.Add(plant);
    }
    public static void AddCorpse(Corpse corpse)
    {
        corpsesHolder.Add(corpse);
    }
    public static void AddItemOnGround(ItemSlot item)
    {
        itemsOnGround.Add(item);
    }
    public static void AddBlueprint(Blueprint blueprint)
    {
        blueprintsHolder.Add(blueprint);
    }
    public static void AddRoomBlueprint(RoomBlueprint roomBlueprint)
    {
        roomBlueprintsHolder.Add(roomBlueprint);

        foreach (Blueprint blueprint in roomBlueprint.Blueprints)
        {
            AddBlueprint(blueprint);
        }
    }

    public static void RemoveAgent(Agent agent)
    {
        if (agent.Intelligent)
            agentsHolder.Remove(agent);
        else
            animalsHolder.Remove(agent);

        village.RemoveAgent(agent);
    }
    public static void RemovePlant(Plant plant)
    {
        plantsHolder.Remove(plant);
    }
    public static void RemoveCorpse(Corpse corpse)
    {
        corpsesHolder.Remove(corpse);
    }
    public static void RemoveItemOnGround(ItemSlot item)
    {
        itemsOnGround.Remove(item);
    }
    public static void RemoveBlueprint(Blueprint blueprint)
    {
        blueprintsHolder.Remove(blueprint);

        if (blueprint.RoomBlueprint != null)
        {
            RoomBlueprint roomBlueprint = blueprint.RoomBlueprint;
            if (blueprint.IsStructureBlueprint) roomBlueprint.AddBuiltStructure(blueprint.BuiltStructure);
            else roomBlueprint.AddBuiltFloor(blueprint.BuiltFloor);

            roomBlueprint.RemoveBlueprint(blueprint);
            if (roomBlueprint.Blueprints.Count == 0)
            {
                Room room = roomBlueprint.GetRoom();
                village.AddRoom(room);
            }
        }
    }
    public static void RemoveRoomBlueprint(RoomBlueprint roomBlueprint)
    {
        roomBlueprintsHolder.Remove(roomBlueprint);
    }


    public static List<Action> GetActions(bool intelligent)
    {
        List<Action> actions = new List<Action>();


        return actions;
    }


    public static Tile GetTile(MapCell cell)
    {
        if (!cell.Visible) return tiles["Unknown"];

        Tile tile = cell.Tile;
        tile.color = cell.Colour;

        return tile;
    }

    public static void UpdateTile(MapCell cell)
    {
        if (tilemap == null) return;

        Vector3Int position = new Vector3Int(cell.Position.x, cell.Position.y, 0);
        Tile tile = GetTile(cell);
        tilemap.SetTile(position, tile);
    }
    #endregion Methods
}



public enum ColourEnum: byte { White, Green, Yellow, Blue, Magenta, Red, Grey, Brown }