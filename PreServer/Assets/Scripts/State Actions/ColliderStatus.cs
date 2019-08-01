using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Used to activate/deactivate the player's colliders
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Collider Status")]
    public class ColliderStatus : StateActions
    {
        public bool status;

        public override void Execute(StateManager states)
        {
            states.frontCollider.enabled = status;
        }
    }
}
