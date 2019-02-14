using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    /// <summary>
    /// Rotation for Player on ground
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Slide Rotation")]
    public class SlideRotation : StateActions
    {
        public TransformVariable cameraTransform;
        public float speed;
        public float constraint = 60f;
        Quaternion start;

        public override void OnEnter(StateManager states)
        {
            start = states.mTransform.rotation;
        }
        public override void OnUpdate(StateManager states)
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
            //Figure out the angle between the 2 quaternions, if it is outside the constraint or there is no input, slowly rotate the user back to start
            float angle = Quaternion.Angle(start, targetRotation);
            Debug.Log(angle);
            if (angle <= constraint && angle >= -constraint)
                states.mTransform.rotation = targetRotation;
        }

        public override void Execute(StateManager states)
        {
            //if (cameraTransform.value == null)
            //    return;

            //var test = cameraTransform.value;
            ////test.forward = states.mTransform.forward;

            //float h = states.movementVariables.horizontal;
            //float v = states.movementVariables.vertical;

            //Vector3 targetDir = test.forward * v;
            //targetDir += test.right * h;
            //targetDir.Normalize();
            //targetDir.y = 0;

            //if (targetDir == Vector3.zero)
            //    targetDir = states.mTransform.forward;

            //states.movementVariables.moveDirection = targetDir;

            //targetDir.y = states.mTransform.forward.y;

            //Quaternion tr = Quaternion.LookRotation(targetDir, states.groundNormal);
            //Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);
            //float angle = Quaternion.Angle(start, targetRotation);
            //Debug.Log(angle);
            //if (angle <= constraint && angle >= -constraint)
            //    states.mTransform.rotation = targetRotation;
        }
    }
}

