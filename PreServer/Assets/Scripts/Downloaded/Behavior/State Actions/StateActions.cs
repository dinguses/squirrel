using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public abstract class StateActions : ScriptableObject
    {
        public abstract void Execute(StateManager states);
        public virtual void OnUpdate(StateManager states) { }
        public virtual void OnLateUpdate(StateManager states) { }
        public virtual void OnFixed(StateManager states) { }
        public virtual void OnEnter(StateManager states) { }
        public virtual void OnExit(StateManager states) { }
    }
}
