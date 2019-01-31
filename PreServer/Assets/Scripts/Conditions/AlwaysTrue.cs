using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Always returns true
    /// 
    /// Used for debugging in Behavior Editor
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Always True")]
    public class AlwaysTrue : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return true;
        }
    }
}
