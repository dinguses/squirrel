using UnityEngine;

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
                state.anim.SetBool(state.hashes.waitForAnimation, false);
                //state.inGrindZone = false;

                state.facingPoint = Vector3.zero;
                state.behindPoint = Vector3.zero;
                state.comingBackFrom180 = false;

                // Execute actions (Handle Jump Velocity)
                onTrueAction.Execute(state);
            }

            return result;
        }
    }
}
