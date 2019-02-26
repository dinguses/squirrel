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
        int count = 0;
        public override bool CheckCondition(StateManager state)
        {
            count = 0;
            if (state.front)
                count++;

            if (state.middle)
                count++;

            if (state.back)
                count++;

            return !state.isGrounded || (state.isGrounded && count != 0);
        }
    }
}
