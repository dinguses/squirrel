using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Sets the StateManager's 'followMeOnFixedUpdate' variable
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/FollowMeStatus")]
    public class FollowMeAtFixedAction : StateActions
    {
        public bool status;

        public override void Execute(StateManager states)
        {
            states.followMeOnFixedUpdate = status;
        }
    }
}
