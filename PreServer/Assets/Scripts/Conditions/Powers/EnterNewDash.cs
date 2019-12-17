using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Enter New Dash")]
    public class EnterNewDash : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            if (state.newDashActive)
                state.timeSinceNewDash = Time.realtimeSinceStartup;

            return state.newDashActive/* && (state.isGrounded || state.dashInAirCounter == 0)*/;
        }
    }
}
