using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Exited Climb")]
    public class ExitedClimb : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            return state.climbState == PlayerManager.ClimbState.NONE;
        }
    }
}