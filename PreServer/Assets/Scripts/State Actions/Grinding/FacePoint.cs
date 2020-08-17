using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Face the most forward point on the grind
    /// </summary>

    [CreateAssetMenu (menuName = "Actions/State Actions/Face Point")]
    public class FacePoint : StateActions
    {
        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            // The front of the squirrel
            var reusable = (states.mTransform.position);

            // Rotate towards the facing point
            var _direction = (states.facingPoint - reusable).normalized;
            var _lookRotation = Quaternion.LookRotation(_direction);

            // Set the rotation

            states.mTransform.rotation = Quaternion.Slerp(states.mTransform.rotation, _lookRotation, states.delta * 9.0f * states.groundSpeedMult);
        }
    }
}
