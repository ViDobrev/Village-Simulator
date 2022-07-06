using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Action
{
    #region Data
    protected string name;
    protected Agent agent;
    protected bool mustBeInRange;
    //protected int range;

    protected bool pathFound;
    protected Queue<Vector2Int> path;

    protected bool completed;

    protected List<string> requirements;
    protected Dictionary<string, StateEffect> effects;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public Queue<Vector2Int> Path { get => path; }
    public bool IsCompleted { get => completed; }
    protected virtual Vector2Int EndPoint { get => Vector2Int.zero; }
    #endregion Properties


    #region Methods
    public virtual void LoadRequirementsAndEffect()
    {

    }
    public virtual Action Initialize()
    {
        return null;
    }
    public virtual void Reset()
    {

    }
    public virtual void ReleaseResources()
    {

    }

    public bool CheckRequirements(Agent agent)
    {
        foreach (string requiredState in requirements)
        {
            if (agent.States.HasState(requiredState)) continue;
            if (agent.States.HasGoal(requiredState)) continue;
            if (agent.Village != null && agent.Village.States.HasState(requiredState)) continue;
            if (Data.Map.States.HasState(requiredState)) continue;

            return false;
        }
        return true;
    }
    public virtual bool IsAchievable()
    {
        return true;
    }


    public bool Run()
    {// Returns false if a problem occured
        if (!PrePerform()) return false;
        Perform();
        if (completed) PostPerform();
        return true;
    }
    protected abstract bool PrePerform();
    protected void Perform()
    {
        if (mustBeInRange)
        {
            if (!pathFound) CalculatePath();

            else
            {
                if (path.Count == 0) Interact();
                else
                {
                    if (agent.Move(path.Peek())) path.Dequeue();
                }
            }
        }
    }
    protected abstract void PostPerform();

    protected abstract void Interact();
    protected virtual void CalculatePath()
    {
        path = Pathfinder.FindPath(agent.Position, EndPoint);
        if (path == null) return;
        pathFound = true;
    }

    public bool HasEffect(string stateName, StateEffect effect)
    {
        if (!effects.ContainsKey(stateName)) return false;

        return effects[stateName] == effect;
    }
    #endregion Methods
}

public enum StateEffect: byte { Add, Remove}