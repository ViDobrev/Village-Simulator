using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AI
{
    #region Data
    private Agent agent;

    private List<Action> actions;
    private Stack<Action> actionPlan;
    #endregion Data

    #region Properties
    public bool HasActionPlan { get => actionPlan.Count > 0; }
    public Action CurrentAction { get => HasActionPlan ? actionPlan.Peek() : null; }
    #endregion Properties


    #region Methods
    public AI(Agent agent)
    {
        this.agent = agent;

        actions = Data.GetActions(agent.Intelligent);
        actionPlan = new Stack<Action>();
    }

    public void CreateActionPlan()
    {
        // Create a list of all states ordered by priority
        List<State> allStates = GetAllStates();

        // Clear previous actionPlam
        ReleaseResources();

        // Find new achievable action
        bool done = false;
        for (int i = 0; i < allStates.Count; i++)
        {
            string priorityState = allStates[i].Name;

            for (int j = 0; j < actions.Count; j++)
            {
                if (actions[j].HasEffect(priorityState, StateEffect.Remove))
                {
                    if (actions[j].CheckRequirements(agent) && actions[j].IsAchievable())
                    {
                        actionPlan.Push(actions[j].Initialize());
                        done = true;
                        break;
                    }
                }
            }

            if (done) break;
        }

        // If no action was achievable push the default action (WanderAction)
        if (!HasActionPlan)
            actionPlan.Push(new WanderAction(agent));
    }
    private List<State> GetAllStates()
    {
        List<State> allStates = new List<State>();

        foreach(State state in agent.States.States.Values)
            allStates.Add(state);
        if (agent.Village != null)
        {
            foreach (State state in agent.Village.States.States.Values)
                allStates.Add(state);
        }
        foreach (State state in Data.Map.States.States.Values)
            allStates.Add(state);

        return (from state in allStates
                orderby state.Priority descending
                select state).ToList();
    }

    public Action GetAction()
    {
        return actionPlan.Peek();
    }

    public void CompleteAction()
    {
        Action action = actionPlan.Pop();
        action.ReleaseResources();
    }
    public void ReleaseResources()
    {
        while (actionPlan.Count > 0)
        {
            Action action = actionPlan.Pop();
            action.ReleaseResources();
        }
    }

    public void PushAction(Action action)
    {
        actionPlan.Push(action);
    }
    #endregion Methods
}