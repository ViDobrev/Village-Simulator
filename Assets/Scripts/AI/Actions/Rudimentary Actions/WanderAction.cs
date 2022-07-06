using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WanderAction : Action
{
    #region Data
    private System.Random rng;
    private int timesToWander;
    #endregion Data


    #region Methods
    public WanderAction(Agent agent)
    {
        name = "WanderAction";
        this.agent = agent;
        mustBeInRange = true;
        pathFound = false;
        completed = false;

        rng = new System.Random(Data.aiRng.Next());
        timesToWander = 5;
    }


    protected override bool PrePerform()
    {
        return true;
    }
    protected override void PostPerform()
    {
        agent.AI.CompleteAction();
    }
    protected override void Interact()
    {
        timesToWander--;
        pathFound = false;

        if (timesToWander < 0) completed = true;
    }

    protected override void CalculatePath()
    {
        for (int i = 0; i < 5; i++)
        {
            int xOffset = rng.Next(-7, 5+1);
            int yOffset = rng.Next(-7, 5+1);

            Vector2Int newPosition = agent.Position + new Vector2Int(xOffset, yOffset);
            MapCell cell = Data.Map.CellAtPosition(newPosition);

            if (!(cell != null && cell.Traversible)) continue;

            path = Pathfinder.FindPath(agent.Position, newPosition);
            if (path == null) continue;
            pathFound = true;
            break;
        }
    }
    #endregion Methods
}