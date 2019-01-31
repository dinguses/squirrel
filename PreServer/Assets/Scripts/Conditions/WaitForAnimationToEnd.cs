using UnityEngine;
using System.Collections;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Wait for Animation to End")]
    public class WaitForAnimationToEnd : Condition
    {
        public string targetBool = "waitForAnimation";

        public override bool CheckCondition(StateManager state)
        {
            bool retVal = !state.anim.GetBool(targetBool);
            return retVal;
        }
    }
}
