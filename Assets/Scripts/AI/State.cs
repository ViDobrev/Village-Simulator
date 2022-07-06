using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class State
{
    #region Data
    private string name;
    //private string value;
    private float priority;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    //public string Value { get => value; }
    public float Priority { get => priority; }
    #endregion Properties


    #region Methods
    public State(string name,/* string value,*/ float priority)
    {
        this.name = name;
        //this.value = value;
        this.priority = priority;
    }

    public void SetPriority(float priority)
    {
        this.priority = priority;
    }
    #endregion Methods
}



public class StatesHolder
{
    #region Data
    private Dictionary<string, State> states;
    private HashSet<string> goals;
    #endregion Data

    #region Properties
    public Dictionary<string, State> States { get => states; }

    public State this[string key]
    {
        get => states[key];
        set => states[key] = value;
    }
    #endregion Properties


    #region Methods
    public StatesHolder(bool intelligent)
    {
        states = new Dictionary<string, State>();
        goals = new HashSet<string>();

        goals.Add("stayAlive");
        if (intelligent) goals.Add("work");
    }
    public StatesHolder()
    {
        states = new Dictionary<string, State>();
    }

    public void AddState(State state)
    {
        states[state.Name] = state;
    }
    public void AddState(string stateName, float priority)
    {
        states[stateName] = new State(stateName, priority);
    }

    public void RemoveState(string stateName)
    {
        if (states.ContainsKey(stateName))
            states.Remove(stateName);
    }

    public bool HasState(string stateName)
    {
        return states.ContainsKey(stateName);
    }
    public bool HasGoal(string goalName)
    {
        return goals.Contains(goalName);
    }
    #endregion Methods
}