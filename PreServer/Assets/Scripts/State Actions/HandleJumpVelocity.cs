using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Executes a player's jump. Executes from the MonitorJump Condition
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Handle Jump Animation")]
    public class HandleJumpVelocity : StateActions
    {
        public float jumpSpeed;

        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            states.rigid.drag = 0;
            //TODO: May need to revert to old (commented out) system based on feedback
            Vector3 currentVelocity = /*states.transform.InverseTransformDirection(*/states.rigid.velocity/*)*/;
            //Vector3 currentVelocity = new Vector3(0, 0, 0);

            states.timeSinceJump = Time.realtimeSinceStartup;
            states.isGrounded = false;

            states.anim.CrossFade(states.hashes.jump, 0.2f);

            if (currentVelocity.y < -.3f)
            {
                currentVelocity.y = -.3f;
            }

            if (currentVelocity.y > 0)
            {
                currentVelocity.y = 0;
            }

            //currentVelocity += jumpSpeed * Vector3.up;
            //var test = states.mTransform.up;

            /*Debug.Log(test.y);
            if (test.y < 1)
            {
                test.y = 1;
            }*/

            //Debug.Log(currentVelocity.z);
            //currentVelocity.z = 0;

            //TODO: Counteracts velocity when you're on a slope 
            //var grunk = Vector3.Angle(states.mTransform.up, Vector3.up);

            //if (grunk >= 5 && grunk < 10)
            //{
            //    currentVelocity.x = currentVelocity.x / 4;
            //}

            //if (grunk >= 10)
            //{
            //    currentVelocity.x = currentVelocity.x / 8;
            //    currentVelocity.z = currentVelocity.z / 8;
            //}

            //if (grunk >= 25)
            //{
            //    currentVelocity.x = 0;
            //    currentVelocity.z = 0;
            //}
            if (states.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                currentVelocity += jumpSpeed * ((states.transform.up * 2f) + Vector3.up);
                currentVelocity.y = jumpSpeed * Vector3.up.y;
                states.jumpFromClimb = true;
                states.jumpFromClimbTimer = 0;
                states.jumpFromClimbTarget = Quaternion.LookRotation(states.climbHit.normal);
            }
            else
                currentVelocity += jumpSpeed * Vector3.up;

            //if (currentVelocity.y > jumpSpeed)
            //{
            //    //currentVelocity.y = jumpSpeed;
            //}

            Debug.DrawRay(states.mTransform.position, currentVelocity);

            states.rigid.velocity = /*states.transform.TransformDirection(*/currentVelocity/*)*/;

            states.anim.SetBool(states.hashes.isClimbing, false);
        }
    }
}