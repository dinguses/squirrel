using UnityEngine;
using System.Collections;

namespace PreServer
{
    // Sets the waitForAnimation bool in the StateManager's anim

    [CreateAssetMenu (menuName = "Actions/State Actions/Set Wait for Animation")]
    public class SetWaitForAnimation : StateActions
    {
        public bool status;

        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            states.anim.SetBool(states.hashes.waitForAnimation, status);
        }
    }
}
