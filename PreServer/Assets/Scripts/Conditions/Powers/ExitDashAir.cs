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

            if (!state.dashActive && !state.isGrounded)
            {
                Debug.Log("exiting dash in air");
            }

            if (!test && !test2)
                return true;
            else
                return false;

            //return (!state.dashActive && !state.isGrounded);
        }
    }
}
