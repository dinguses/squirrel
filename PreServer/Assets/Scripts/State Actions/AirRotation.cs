using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    /// <summary>
    /// For rotation in the air
    /// 
    /// Basically the same as the ground rotation, but without the need for ground normal stuff
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Air Rotation")]
    public class AirRotation : StateActions
    {
        public TransformVariable cameraTransform;
        public float speed;

        public override void Execute(StateManager states)
        {
            if (cameraTransform.value == null)
                return;

            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;

            Vector3 targetDir = cameraTransform.value.forward * v;
            targetDir += cameraTransform.value.right * h;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = states.mTransform.forward;

            states.movementVariables.moveDirection = targetDir;

            targetDir.y = states.mTransform.forward.y;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);

            states.mTransform.rotation = targetRotation;
        }
    }
}
