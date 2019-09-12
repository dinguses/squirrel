using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Enemy/Exit Suspicion")]
    public class ExitSuspicionState : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            EnemyManager state = (EnemyManager)sm;
            return state.state == EnemyManager.DetectState.NONE;
        }
    }
}
