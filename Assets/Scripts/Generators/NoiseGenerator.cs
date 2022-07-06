using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoiseGenerator
{
    #region Data
    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int( 0,  1),     //N
        new Vector2Int( 1,  1),     //NE
        new Vector2Int( 1,  0),     //E
        new Vector2Int( 1, -1),     //SE
        new Vector2Int( 0, -1),     //S
        new Vector2Int(-1, -1),     //SW
        new Vector2Int(-1,  0),     //W
        new Vector2Int(-1,  1),     //NW
    };
    #endregion Data


    #region Methods
    public float[,] GenerateNoiseMap(NoiseData noiseData, int sizeX, int sizeY, NoiseMode noiseMode = NoiseMode.Normal)
    {
        float persistance;
        if (noiseMode == NoiseMode.Normal) persistance = noiseData.persistance;
        else if (noiseMode == NoiseMode.NaturalStructures) persistance = noiseData.naturalStructureDensity;
        else persistance = noiseData.faunaDensity;

        if (noiseData.scale <= 0) noiseData.scale = 0.0001f;

        float halfWidth = sizeX / 2f;
        float halfHeight = sizeY / 2f;

        Vector2[] octaveOffsets = new Vector2[noiseData.octaves];

        for (int i = 0; i < noiseData.octaves; i++)
        {
            float offsetX = Data.mapRng.Next(-100_000, 100_000);
            float offsetY = Data.mapRng.Next(-100_000, 100_000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float[,] noiseMap = new float[sizeX, sizeY];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < noiseData.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseData.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseData.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= noiseData.lacunarity;
                }
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);

        return noiseMap;
    }

    public List<Vector2Int> GenerateNaturalStrucutresPositions(NoiseData noiseData, int sizeX, int sizeY)
    {
        float[,] naturalStructuresNoise = GenerateNoiseMap(noiseData, sizeX, sizeY, NoiseMode.NaturalStructures);

        List<Vector2Int> localMaximas = FindLocalMaximas(naturalStructuresNoise);

        return localMaximas;
    }
    public List<Vector2Int> GenerateInitialFaunaPositions(NoiseData noiseData, int sizeX, int sizeY)
    {
        float[,] naturalStructuresNoise = GenerateNoiseMap(noiseData, sizeX, sizeY, NoiseMode.Fauna);

        List<Vector2Int> localMaximas = FindLocalMaximas(naturalStructuresNoise);

        return localMaximas;
    }


    private List<Vector2Int> FindLocalMaximas(float[,] noiseMap)
    {
        List<Vector2Int> localMaximas = new List<Vector2Int>();
        for (int y = 1; y < noiseMap.GetLength(1) - 1; y++)
        {
            for (int x = 1; x < noiseMap.GetLength(0) - 1; x++)
            {
                float currentNoiseValue = noiseMap[x, y];
                if (CheckNeighbours(noiseMap, x, y, (neighbourhoodNoiseValue) => neighbourhoodNoiseValue > currentNoiseValue))
                    localMaximas.Add(new Vector2Int(x, y));
            }
        }

        return localMaximas;
    }

    private List<Vector2Int> FindLocalMinimas(float[,] noiseMap)
    {
        List<Vector2Int> localMinimas = new List<Vector2Int>();
        for (int y = 1; y < noiseMap.GetLength(1) - 1; y++)
        {
            for (int x = 1; x < noiseMap.GetLength(0) - 1; x++)
            {
                float currentNoiseValue = noiseMap[x, y];
                if (CheckNeighbours(noiseMap, x, y, (neighbourhoodNoiseValue) => neighbourhoodNoiseValue < currentNoiseValue))
                    localMinimas.Add(new Vector2Int(x, y));
            }
        }

        return localMinimas;
    }


    private bool CheckNeighbours(float[,] noiseMap, int x, int y, Func<float, bool> failCondition)
    {
        Vector2Int pos = new Vector2Int(x, y);
        Vector2Int newPos;

        foreach (var dir in directions)
        {
            newPos = pos + dir;

            // Out of bounds exception handler
            if (newPos.x < 0 || newPos.x >= noiseMap.GetLength(0) || newPos.y < 0 || newPos.y >= noiseMap.GetLength(1))
                continue;

            if (failCondition(noiseMap[newPos.x, newPos.y]))
                return false;
        }

        return true;
    }
    #endregion Methods
}


#region Noise
[System.Serializable]
public struct NoiseData
{
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    [Range(0, 2)]
    public float naturalStructureDensity;
    [Range(0, 1)]
    public float faunaDensity;
}
public enum NoiseMode { Normal, NaturalStructures, Fauna }
#endregion Noise