using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Exit Dash Air")]
    public class ExitDashAir : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool test = state.dashActive;
            bool test2 = state.isGrounded;
            bool test3 = state.climbState == PlayerManager.ClimbState.NONE;

            if (!state.dashActive && !state.isGrounded && test3)
            {
                Debug.Log("exiting dash in air");
            }

            if (!test && !test2 && test3)
                return true;
            else
                return false;

            //return (!state.dashActive && !state.isGrounded);
        }
    }
}
