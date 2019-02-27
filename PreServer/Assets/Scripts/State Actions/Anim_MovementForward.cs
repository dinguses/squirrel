using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Sets anim variables for player movement stuff
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Anim_MovementForward")]
    public class Anim_MovementForward : StateActions
    {
        public StateActions[] stateActions;

        public override void Execute(StateManager states)
        {

        }
    }
}
