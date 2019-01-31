using UnityEngine;
using System.Collections;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Test")]
    public class Test : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.exitingGrind = false;
        }
    }
}
