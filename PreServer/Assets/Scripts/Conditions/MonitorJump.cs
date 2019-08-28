using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Checks if the player has jumped
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Monitor Jump")]
    public class MonitorJump : Condition
    {
        public StateActions onTrueAction;

        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool result = state.isJumping;

            // If player is jumping AND is grounded
            if (state.isJumping && (state.isGrounded || state.climbState == PlayerManager.ClimbState.CLIMBING))
            {
                //TODO: should this go here?

                state.isJumping = false;
                state.rigid.useGravity = true;
                state.anim.SetBool(state.hashes.isGrinding, false);

                //GOTO
                //state.frontCollider.enabled = true;
                //state.grindCollider.enabled = false;

                state.exitingGrind = false;
                state.inGrindZone = false;

                // Execute actions (Handle Jump Velocity)
                onTrueAction.Execute(state);
            }

            return result;
        }
    }
}
