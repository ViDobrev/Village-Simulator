using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Blueprint
{
    #region Data
    private StructureData structureData;
    private FloorData floorData;
    private float workDone;
    private Item component;

    private Vector2Int location;
    private RoomBlueprint roomBlueprint;

    private Structure builtStructure;
    private Floor builtFloor;

    private Agent assignedAgent;
    #endregion Data

    #region Properties
    public string Name { get => structureData.Name; }
    public StructureData StructureData { get => structureData; }
    public FloorData FloorData { get => floorData; }
    public float WorkDone { get => workDone; }
    public Item Component { get => component; }
    public bool ComponentDelivered { get => component != null; }

    public bool IsStructureBlueprint { get => structureData != null; }
    public bool IsFloorBlueprint { get => floorData != null; }

    public Vector2Int Location { get => location; }
    public RoomBlueprint RoomBlueprint { get => roomBlueprint; }

    public Structure BuiltStructure { get => builtStructure; }
    public Floor BuiltFloor { get => builtFloor; }

    public Agent AssignedAgent { get => assignedAgent; }

    public bool Completed { get => workDone >= structureData.Schematic.WorkRequired; }
    #endregion Properties


    #region Methods
    public Blueprint(StructureData structureData, Vector2Int location, RoomBlueprint roomBlueprint)
    {
        this.structureData = structureData;
        workDone = 0f;

        this.location = location;
        this.roomBlueprint = roomBlueprint;
    }
    public Blueprint(FloorData floorData, Vector2Int location, RoomBlueprint roomBlueprint)
    {
        this.floorData = floorData;
        workDone = 0f;

        this.location = location;
        this.roomBlueprint = roomBlueprint;
    }

    public bool TryGiveComponent(Item component)
    {
        if (ComponentDelivered) return false;
        if (component == null) return false;
        if (component.Material == null) return false;

        if (!structureData.Schematic.Component.CanUseMaterial(component.Material)) return false;

        this.component = component;
        return true;
    }
    public bool AttachedFinishedStructure(Structure builtStructure)
    {// Returns true if the blueprint is for a structure
        if (!IsStructureBlueprint) return false;

        this.builtStructure = builtStructure;
        return true;
    }
    public bool AttachedFinishedFloor(Floor builtFloor)
    {// Returns true if the blueprint is for a floor
        if (!IsFloorBlueprint) return false;

        this.builtFloor = builtFloor;
        return true;
    }

    public void Work(float increment)
    {
        workDone += increment;
    }

    public bool Assign(Agent agent)
    {
        if (assignedAgent != null) return false;

        assignedAgent = agent;
        return true;
    }
    public void Release()
    {
        assignedAgent = null;
    }
    #endregion Methods
}