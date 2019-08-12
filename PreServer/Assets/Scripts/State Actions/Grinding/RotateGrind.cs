using UnityEngine;
using System.Collections;
using SO;

namespace PreServer
{
    /// <summary>
    /// Detects if the player should rotate
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Rotate Grind")]
    public class RotateGrind : StateActions
    {
        public TransformVariable cameraTransform;
        public float speed;


        public override void Execute(StateManager states)
        {
            // The front of the player
            Vector3 reusable = (states.mTransform.position + (states.mTransform.forward));

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

            // TODO: This seems uneccesary HMM
            //states.movementVariables.moveDirection = targetDir;

            targetDir.y = states.mTransform.forward.y;

            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * 10.5f;

            // Test1 and Test2 are to slightly decrease sensitivity of the 180 rotation
            //TODO: Cleanup vairables

            var test1 = Mathf.Abs(states.movementVariables.horizontal);
            var test2 = Mathf.Abs(states.movementVariables.vertical);
             
            if (Vector3.Angle(targetDir, states.mTransform.forward) > 90 && (test1 >= .02 || test2 >= .02) && states.rotateBool /*&& states.anim.GetCurrentAnimatorClipInfo(1)[0].clip.name != "squ_grind_180"*/)
            {
                //Debug.Log(states.anim.GetCurrentAnimatorClipInfo(1)[0].clip.name);
                //Debug.Log("GOTTA 180");
                states.rotateBool = false;

                states.testINT = 0;

                var holdFacing = states.facingPoint;
                var holdFacingPair = states.facingPointPair;
                states.facingPoint = states.behindPoint;
                states.facingPointPair = states.behindPointPair;
                states.behindPoint = holdFacing;
                states.behindPointPair = holdFacingPair;

                states.angleTest = 0;          

                states.anim.SetBool(states.hashes.waitForAnimation, true);

                states.anim.CrossFade(states.hashes.squ_grind_180, 0.01f);

                states.rigid.velocity = new Vector3 (0,0,0);
            }

            Quaternion tr = Quaternion.LookRotation(targetDir, states.groundNormal);

            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * speed);
        }
    }
}