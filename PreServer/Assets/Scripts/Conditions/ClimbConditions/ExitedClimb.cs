using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Exited Climb")]
    public class ExitedClimb : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return state.climbState == StateManager.ClimbState.NONE;
        }
    }
}