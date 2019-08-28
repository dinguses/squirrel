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
            if (!state.inGrindZone)
            {
                // Time for gravity again
                state.rigid.useGravity = true;

                // Grinding is false
                state.anim.SetBool(state.hashes.isGrinding, false);
                state.exitingGrind = false;

                // re-enable the front collider
                //GOTO
                //state.frontCollider.enabled = true;
                //state.grindCollider.enabled = false;

                result = true;
            }

            return result;
        }
    }
}