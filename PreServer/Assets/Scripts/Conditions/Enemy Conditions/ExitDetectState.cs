using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Enemy/Exit Detect")]
    public class ExitDetectState : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            EnemyManager state = (EnemyManager)sm;
            return state.state == EnemyManager.DetectState.NONE;
        }
    }
}
