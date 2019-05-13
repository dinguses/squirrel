using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Exit Dash")]
    public class ExitDash : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return !state.dashActive;
        }
    }
}
