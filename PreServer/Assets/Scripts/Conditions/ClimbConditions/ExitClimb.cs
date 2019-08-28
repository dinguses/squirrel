using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{ 
    [CreateAssetMenu(menuName = "Conditions/Exit Climb")]
    public class ExitClimb : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            return state.climbState == PlayerManager.ClimbState.EXITING;
        }
    }
}