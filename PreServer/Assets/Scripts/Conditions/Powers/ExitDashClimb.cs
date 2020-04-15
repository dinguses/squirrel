using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Exit Dash Climb")]
    public class ExitDashClimb : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;
            return !state.dashActive && state.climbState != PlayerManager.ClimbState.NONE;
        }
    }
}
