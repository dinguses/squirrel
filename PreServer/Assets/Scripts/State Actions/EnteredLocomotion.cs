using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Sets anim variables for player movement stuff
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Entered Locomotion")]
    public class EnteredLocomotion : StateActions
    {
        public override void Execute(StateManager states)
        {
            Debug.Log("Locomotion entered");
        }
    }
}
