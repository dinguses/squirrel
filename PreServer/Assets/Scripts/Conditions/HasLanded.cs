using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Checks if the player has landed on some sort of ground, and plays different animations depending on how long they were falling
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Has Landed")]
    public class HasLanded : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            // get ammt of time since jump/leaving ground
            float timeDifference = Time.realtimeSinceStartup - state.timeSinceJump;

            // have to have been ungrounded to start checking
            if (timeDifference > .35f)
            {
                bool result = state.isGrounded;

                // if grounded, then land
                if (result)
                {
                    // If been in air for at least a certain amt of time, do the long land anim
                    if (timeDifference > .65f)
                    {
                        state.anim.CrossFade(state.hashes.Land, .2f);
                    }

                    // If not, quick land
                    else
                    {
                        state.anim.SetBool(state.hashes.QuickLand, true);
                    }

                }

                return result;
            }

            else
            {
                return false;
            }
                
        }
    }
}
