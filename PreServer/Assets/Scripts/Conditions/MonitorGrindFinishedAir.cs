using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Checks if player should no longer be grinding
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Monitor Grind Finished Air")]
    public class MonitorGrindFinishedAir : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool result = false;

            // If player has left grind zone, they're no longer grinding
            if (!state.inGrindZone && !state.isGrounded)
            {
                Debug.Log("Left the grind - air");

                // Time for gravity again
                state.rigid.useGravity = true;

                // Grinding is false
                state.anim.SetBool(state.hashes.isGrinding, false);
                state.doneAdjustingGrind = false;

                state.facingPoint = Vector3.zero;
                state.behindPoint = Vector3.zero;

                state.frontCollider.enabled = true;

                result = true;
            }

            return result;
        }
    }
}