using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    /// <summary>
    /// Rotation for Player on ground
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Rotate Based on Cam Orientation")]
    public class GroundRotation : StateActions
    {
        public TransformVariable cameraTransform;
        public float speed;

        public override void Execute(StateManager states)
        {
            if (cameraTransform.value == null)
                return;

            var test = cameraTransform.value;
            //test.forward = states.mTransform.forward;

            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;

            Vector3 targetDir = test.forward * v;
            targetDir += test.right * h;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = states.mTransform.forward;

            states.movementVariables.moveDirection = targetDir;

            targetDir.y = states.mTransform.forward.y;

            Quaternion tr = Quaternion.LookRotation(targetDir, states.groundNormal);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);

            states.mTransform.rotation = targetRotation;
        }
    }
}
