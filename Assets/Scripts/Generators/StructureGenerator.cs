using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class StructureGenerator
{
    #region Methods
    public static Structure GenerateStructure(StructureData structureData, ItemSlot component, Agent creatorAgent)
    {
        if (!structureData.Schematic.Component.CanUseMaterial(component.Item.Material)) return null;
        if (component.Amount < structureData.Schematic.Component.Amount) return null;

        Quality quality = Quality.Normal;
        if (creatorAgent != null) quality = GetQuality(creatorAgent);


        Structure structure = new Structure(structureData, component.Item.Material, quality);
        if (creatorAgent != null) structure.SignCreator(creatorAgent);

        return structure;
    }
    public static Structure GenerateStructure(StructureData structureData, ItemSlot component, Agent creatorAgent, Quality quality)
    {
        if (!structureData.Schematic.Component.CanUseMaterial(component.Item.Material)) return null;
        if (component.Amount < structureData.Schematic.Component.Amount) return null;


        Structure structure = new Structure(structureData, component.Item.Material, quality);
        if (creatorAgent != null) structure.SignCreator(creatorAgent);

        return structure;
    }
    public static Structure GenerateStructure(Blueprint blueprint, Agent creatorAgent)
    {
        if (!blueprint.IsStructureBlueprint) return null;

        StructureData structureData = blueprint.StructureData;
        MaterialData material = blueprint.Component.Material;

        Quality quality = GetQuality(creatorAgent);

        Structure structure = new Structure(structureData, material, quality);
        structure.SignCreator(creatorAgent);

        Data.RemoveBlueprint(blueprint);

        return structure;
    }
    public static Floor GenerateFloor(Blueprint blueprint, Agent creatorAgent)
    {
        if (!blueprint.IsFloorBlueprint) return null;

        FloorData floorData = blueprint.FloorData;
        MaterialData material = blueprint.Component.Material;

        Floor floor = new Floor(floorData, material);
        floor.SignCreator(creatorAgent);

        Data.RemoveBlueprint(blueprint);

        return floor;
    }



    private static Quality GetQuality(Agent creatorAgent)
    {
        int skillLevel = creatorAgent.Skills[SkillsEnum.Building].Level;
        var qualities = System.Enum.GetValues(typeof(Quality));

        int[] qualityDistribution = Skill.QualityDistribution[skillLevel];
        int[] qualityValues = new int[qualities.Length];

        for (int i = 0; i < qualities.Length; i++)
        {
            qualityValues[i] = (int)qualities.GetValue(i);
        }

        return (Quality)Utilities.RandomNumberByPropbability(qualityValues, qualityDistribution);
    }
    #endregion Methods
}