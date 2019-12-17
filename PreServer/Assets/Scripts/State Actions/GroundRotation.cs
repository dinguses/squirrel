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

        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            if (cameraTransform.value == null)
                return;

            var test = cameraTransform.value;
            //test.forward = states.mTransform.forward;

            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;

            Vector3 targetDir = test.forward * v;
            targetDir += test.right * h;

            var targetDir2 = targetDir;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = states.mTransform.forward;

            states.movementVariables.moveDirection = targetDir;

            var targetDir3 = targetDir;
            targetDir3.y = -states.mTransform.forward.y;

            targetDir2.y = -states.mTransform.forward.y;

           // Debug.Log("targetDir2 - " + targetDir2);


            Quaternion tr = Quaternion.LookRotation(targetDir, states.groundNormal);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);

            //Rotate 180 ?
            var testAngle = Vector3.Angle(targetDir3, states.mTransform.forward);
            var testAngleSide = Vector3.Angle(targetDir2, states.mTransform.right);

            // Rotate 180 - disabled to test rotation
            if (testAngle > 160 && testAngle < 200)
            {
                states.testINT = 0;

                //Debug.Log("test angle - " + testAngle);
                //Debug.Log("test angle side - " + testAngleSide);

                // If the player should be rotating to the left
                //if (testAngleSide > 90)
                //{
                //    //states.anim.SetBool(states.hashes.mirror180, true);
                //    states.anim.CrossFade(states.hashes.squ_ground_180_mirror, 0.01f);
                //}
                //else
                //{
                    states.anim.CrossFade(states.hashes.squ_ground_180, 0.01f);
                //}

                states.anim.SetBool(states.hashes.waitForAnimation, true);
                states.rigid.velocity = Vector3.zero;

                //states.storedTargetDir = targetDir;
                states.storedTargetDir = targetDir3;
            }


            //float direction = 0;
            //float speedOut = 0;
            //StickToWorldSpace(states.transform, Camera.main.transform, states, ref direction, ref speedOut);
            //Debug.Log("Ground Rotation target DIR: " + targetDir);
            //targetRotation = Quaternion.AngleAxis(direction, states.transform.up);
            //states.mTransform.forward = targetRotation * states.transform.forward;
            states.mTransform.rotation = targetRotation;
        }

        public void StickToWorldSpace(Transform root, Transform camera, PlayerManager states, ref float directionOut, ref float speedOut)
        {
            Vector3 rootDirection = root.forward;
            Vector3 stickDirection = new Vector3(states.movementVariables.horizontal, 0, states.movementVariables.vertical);

            speedOut = stickDirection.sqrMagnitude;

            //Get Camera Rotation
            Vector3 cameraDirection = camera.forward;
            cameraDirection.y = 0;
            Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

            //Convert input in worldspace coordinates
            Vector3 moveDirection = referentialShift * stickDirection;
            Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

            Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
            Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
            Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

            float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
            angleRootToMove /= 180f;
            directionOut = angleRootToMove * speed;
        }
    }
}
