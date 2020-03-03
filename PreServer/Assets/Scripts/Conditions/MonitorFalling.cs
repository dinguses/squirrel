using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Checks if the player is falling
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Monitor Falling")]
    public class MonitorFalling : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool result = false;

            // If player is not grounded AND is not jumping AND is currently in the Locomotion state, then they must be falling
            if (!state.isGrounded && !state.isJumping && state.currentState.stateName == "Locomotion")
            {
                result = true;

                // start counting how long they've been ungrounded
                state.timeSinceJump = Time.realtimeSinceStartup;
                state.anim.SetBool(state.hashes.waitForAnimation, false);
            }

            return result;
        }
    }
}
