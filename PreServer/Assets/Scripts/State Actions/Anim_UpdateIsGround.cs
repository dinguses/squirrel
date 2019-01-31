using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Updates animator's isGrounded
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Anim_UpdateIsGrounded")]
    public class Anim_UpdateIsGround : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.anim.SetBool(states.hashes.isGrounded, states.isGrounded);

            float timeDifference = Time.realtimeSinceStartup - states.timeSinceJump;

            states.anim.SetFloat(states.hashes.TimeSinceGrounded, timeDifference);
        }
    }
}
