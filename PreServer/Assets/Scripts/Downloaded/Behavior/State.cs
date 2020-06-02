using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
    	public StateActions[] onFixed;
        public StateActions[] onLateUpdate;
        public StateActions[] onUpdate;
        public StateActions[] onEnter;
        public StateActions[] onExit;
        public StateActions[] actions;

        [SerializeField]
        public string stateName;

        public int idCount;
		[SerializeField]
        public List<Transition> transitions = new List<Transition>();

        public void OnEnter(StateManager states)
        {
            ExecuteActions(states, onEnter);
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                {
                    //Debug.Log(Time.frameCount + " || Entering States: " + actions[i].name);
                    actions[i].OnEnter(states);
                }
            }
        }
	
		public void FixedTick(StateManager states)
		{
            ExecuteActions(states, onFixed);
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                    actions[i].OnFixed(states);
            }
        }

        public void Tick(StateManager states)
        {
            ExecuteActions(states, onUpdate);
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                    actions[i].OnUpdate(states);
            }
            CheckTransitions(states);
        }

        public void LateTick(StateManager states)
        {
            ExecuteActions(states, onLateUpdate);
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                    actions[i].OnLateUpdate(states);
            }
            CheckTransitions(states);
        }

        public void OnExit(StateManager states)
        {
            ExecuteActions(states, onExit);
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                {
                    //Debug.Log(Time.frameCount + " || Leaving States: " + actions[i].name);
                    actions[i].OnExit(states);
                }
            }
        }

        public void CheckTransitions(StateManager states)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].disable)
                    continue;

                if(transitions[i].condition.CheckCondition(states))
                {
                    if (transitions[i].targetState != null)
                    {
                        states.currentState = transitions[i].targetState;
                        OnExit(states);
                        states.currentState.OnEnter(states);
                    }
                    return;
                }
            }
        }
        
        public void ExecuteActions(StateManager states, StateActions[] l)
        {
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i] != null)
                    l[i].Execute(states);
            }
        }

        public Transition AddTransition()
        {
            Transition retVal = new Transition();
            transitions.Add(retVal);
            retVal.id = idCount;
            idCount++;
            return retVal;
        }

        public Transition GetTransition(int id)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].id == id)
                    return transitions[i];
            }

            return null;
        }

		public void RemoveTransition(int id)
		{
			Transition t = GetTransition(id);
			if (t != null)
				transitions.Remove(t);
		}

    }
}
