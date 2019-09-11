using UnityEngine;
using System.Collections;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Conditions/Wait for Animation to End Grounded")]
    public class WaitForAnimationToEndGrounded : Condition
    {
        public string targetBool = "waitForAnimation";

        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            //TODO: Fix this, this won't work long-term
            bool retVal = (!state.anim.GetBool(targetBool) && !state.inGrindZone);
            return retVal;
        }
    }
}
