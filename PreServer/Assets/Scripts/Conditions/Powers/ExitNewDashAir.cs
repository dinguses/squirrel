using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Exit New Dash Air")]
    public class ExitNewDashAir : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            return (!state.newDashActive && !state.isGrounded);
        }
    }
}
