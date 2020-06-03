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
        PlayerManager states;
        Vector3 slideRight;
        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            Vector3 middleOrigin = states.transform.position + states.transform.forward + (states.transform.up * 0.1f);
            RaycastHit middleHit = new RaycastHit();
            if (Physics.Raycast(middleOrigin, -states.transform.up, out middleHit, 0.3f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                slideRight = Vector3.Cross(middleHit.normal, Vector3.up);
                Vector3 slideDirection = Vector3.Cross(slideRight, middleHit.normal);
                float forwardDirAngle = Vector3.Angle(slideDirection, states.transform.forward);
                float backwardDirAngle = Vector3.Angle(-slideDirection, states.transform.forward);
                if(forwardDirAngle <= backwardDirAngle)
                {
                    start = Quaternion.LookRotation(slideDirection, states.mTransform.up);
                }
                else
                {
                    start = Quaternion.LookRotation(-slideDirection, states.mTransform.up);
                }
            }
            else
                start = states.mTransform.rotation;
        }
        public override void OnUpdate(StateManager sm)
        {
            if (cameraTransform.value == null)
                return;

            var test = cameraTransform.value;
            //test.forward = states.mTransform.forward;

            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;

            Vector3 targetDir = test.forward * v;
            targetDir += test.right * h;
            targetDir.y = 0;
            targetDir.Normalize();

            if (targetDir == Vector3.zero)
                targetDir = states.mTransform.forward;

            states.movementVariables.moveDirection = targetDir;

            targetDir.y = states.mTransform.forward.y;

            Quaternion tr = Quaternion.LookRotation(targetDir, states.groundNormal);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);
            ////Figure out the angle between the 2 quaternions, if it is outside the constraint or there is no input, slowly rotate the user back to start
            float angle = Quaternion.Angle(start, targetRotation);
            ////Debug.Log(angle);
            if (angle <= constraint && angle >= -constraint)
                states.mTransform.rotation = targetRotation;
            Debug.DrawRay(states.transform.position, slideRight * 2f, Color.yellow);
            //if(h > 0)
            //{
            //    Debug.LogError("Sliding Right?");
            //    states.rigid.AddForce(slideRight * h, ForceMode.Impulse);
            //}
            //else if(h < 0)
            //{
            //    Debug.LogError("Sliding Left?");
            //    states.rigid.AddForce(slideRight * h, ForceMode.Impulse);
            //}
        }

        public override void Execute(StateManager sm)
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

