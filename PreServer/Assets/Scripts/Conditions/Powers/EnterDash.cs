using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Enter Dash")]
    public class EnterDash : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return state.dashActive/* && (state.isGrounded || state.dashInAirCounter == 0)*/;
        }
    }
}
