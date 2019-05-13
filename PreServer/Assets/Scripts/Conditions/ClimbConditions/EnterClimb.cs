﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Enter Climb")]
    public class EnterClimb : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return state.climbState == StateManager.ClimbState.ENTERING;
        }
    }
}