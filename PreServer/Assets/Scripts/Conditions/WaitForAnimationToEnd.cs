using UnityEngine;
using System.Collections;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Wait for Animation to End")]
    public class WaitForAnimationToEnd : Condition
    {
        public string targetBool = "waitForAnimation";

        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool retVal = (!state.anim.GetBool(targetBool) && state.inGrindZone);

            return retVal;
        }
    }
}
