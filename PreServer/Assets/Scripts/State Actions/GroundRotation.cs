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

            targetDir2.y = -states.mTransform.forward.y;

           // Debug.Log("targetDir2 - " + targetDir2);


            Quaternion tr = Quaternion.LookRotation(targetDir, states.groundNormal);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);

            // Rotate 180?
            var testAngle = Vector3.Angle(targetDir2, states.mTransform.forward);
            var testAngleSide = Vector3.Angle(targetDir2, states.mTransform.right);

            if (testAngle > 120 && testAngle < 240)
            {
                states.testINT = 0;

                Debug.Log("test angle - " + testAngle);
                Debug.Log("test angle side - " + testAngleSide);

                // If the player should be rotating to the left
                if (testAngleSide > 90)
                {
                    //states.anim.SetBool(states.hashes.mirror180, true);
                    states.anim.CrossFade(states.hashes.squ_ground_180_mirror, 0.01f);
                }
                else
                {
                    states.anim.CrossFade(states.hashes.squ_ground_180, 0.01f);
                }

                states.anim.SetBool(states.hashes.waitForAnimation, true);
                states.rigid.velocity = new Vector3(0, 0, 0);

                states.storedTargetDir = targetDir;
            }


            //Debug.Log("Ground Rotation target DIR: " + targetDir);

            states.mTransform.rotation = targetRotation;
        }
    }
}
