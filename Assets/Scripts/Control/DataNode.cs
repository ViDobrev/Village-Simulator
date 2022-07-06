using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataNode : MonoBehaviour
{
    #region Data
    private LogicNode logicNode;
    private DisplayNode displayNode;
    private Map map;

    [SerializeField] private int seed;
    [SerializeField] private int mapSizeX;
    [SerializeField] private int mapSizeY;

    [SerializeField] private NoiseData noiseData;

    [SerializeField] private Region[] regions;

    [SerializeField] private TileManager tileManager;
    #endregion Data

    #region Properties
    public DisplayNode DisplayNode { get => displayNode; }
    public int MainSeed { get => seed; }
    public Vector2Int MapSize { get => new Vector2Int(mapSizeX, mapSizeY); }
    public Region[] Regions { get => regions; }
    public NoiseData NoiseData { get => noiseData; }
    #endregion Properties


    #region Methods
    public void Initialize(LogicNode logicNode, DisplayNode displayNode)
    {
        this.logicNode = logicNode;
        this.displayNode = displayNode;

        Data.Initialize(MainSeed, tileManager, this);
        tileManager = null;
    }
    public void SetMap(Map map)
    {
        this.map = map;
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void OnValidate()
    {
        if (noiseData.octaves < 0) noiseData.octaves = 0;
        if (noiseData.scale < 0) noiseData.scale = 0.0001f;
        if (noiseData.lacunarity < 0) noiseData.lacunarity = 1;
    }
    #endregion Behaviour
}


#region Region
[System.Serializable]
public struct Region
{
    public string name;
    [Range(0, 1)]
    public float height;
}
public enum RegionType { Water, Grass, Stone }
#endregion Region