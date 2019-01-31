using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Sets anim variables for player movement stuff
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Anim_MovementForward")]
    public class Anim_MovementForward : StateActions
    {
        public StateActions[] stateActions;

        public override void Execute(StateManager states)
        {
            if (stateActions != null)
            {
                for (int i = 0; i < stateActions.Length; i++)
                {
                    stateActions[i].Execute(states);
                }
            }

            //states.anim.SetFloat(states.hashes.vertical, states.movementVariables.moveAmount, 0.2f, states.delta);
            states.anim.SetFloat(states.hashes.speed, states.movementVariables.moveAmount, 0.01f, states.delta);

            if (states.movementVariables.moveAmount == 0)
            {
                states.timeSinceMove = Time.realtimeSinceStartup;
            }

            if (states.movementVariables.moveAmount > .3f)
            {
                states.timeSinceSlow = Time.realtimeSinceStartup;
            }

            float timeDifference = Time.realtimeSinceStartup - states.timeSinceMove;
            float timeDifference2 = Time.realtimeSinceStartup - states.timeSinceSlow;

            if (timeDifference < .2f)
            {
                states.anim.SetFloat(states.hashes.TimeSinceMove, timeDifference, 0.01f, states.delta);
            }

            if (timeDifference2 < .2f)
            {
                states.anim.SetFloat(states.hashes.TimeSinceSlow, timeDifference2, 0.01f, states.delta);
            }

            states.anim.SetBool(states.hashes.UpIdle, states.UpIdle);
        }
    }
}
