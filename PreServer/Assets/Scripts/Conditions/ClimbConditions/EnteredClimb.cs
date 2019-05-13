using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Entered Climb")]
    public class EnteredClimb : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return state.climbState == StateManager.ClimbState.CLIMBING;
        }
    }
}