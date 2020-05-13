using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Checks if player should no longer be grinding
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Monitor Grind Finished")]
    public class MonitorGrindFinished : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool result = false;

            // If player has left grind zone, they're no longer grinding
            if (!state.inGrindZone /*&& state.isGrounded*/)
            {
                Debug.Log("Left the grind");

                // Time for gravity again
                state.rigid.useGravity = true;

                // Grinding is false
                state.anim.SetBool(state.hashes.isGrinding, false);
                state.doneAdjustingGrind = false;

                state.frontCollider.enabled = true;

                result = true;
            }

            return result;
        }
    }
}