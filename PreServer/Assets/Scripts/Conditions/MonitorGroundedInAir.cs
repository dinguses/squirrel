using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// If the colliders return I am grounded, but I the raycast can't detect ground, then I am probably touching a slanted wall, go to the grounded in air state
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Monitor Grounded In Air")]
    public class MonitorGroundedInAir : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            return !state.isColidingInAir;
        }
    }
}
