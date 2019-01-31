using UnityEngine;
using System.Collections;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Variables/StateManager")]
    public class StateManagerVariable : ScriptableObject
    {
        public StateManager value;

        public void Set(StateManager s)
        {
            value = s;
        }
    }
}