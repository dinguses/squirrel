using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Rotates the player to match the ground normal of whatever ground they're on
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Rotate Based On Ground")]
    public class RotateBasedOnGround : StateActions
    {
        public float rotSpeed = 8;
        public float rotationConstraint = 70;
        Vector3 ground;
        float frontAngle = 0;
        float middleAngle = 0;
        float backAngle = 0;
        float groundAngle = 0;
        public override void Execute(StateManager states)
        {
            //if (states.middle != null)
            //    ground = states.middleNormal;
            //else
            //    middleAngle = rotationConstraint;

            //if (states.front != null)
            ground = states.frontNormal;
            //else
            //    frontAngle = rotationConstraint;

            //if (states.back != null)
            //    ground = states.backNormal;
            //else
            //    backAngle = rotationConstraint;

            float angle = Vector3.Angle(states.frontNormal, Vector3.up);
            //Debug.Log("angle 1 - " + angle);
            //Get the smallest angle and that's what we're rotating towards
            //if (middleAngle < backAngle && middleAngle < angle && middleAngle < frontAngle)
            //{
            //    ground = states.middleNormal;
            //    angle = middleAngle;
            //}
            //else if(frontAngle < middleAngle && frontAngle < backAngle && frontAngle < angle)
            //{
            //    ground = states.frontNormal;
            //    angle = frontAngle;
            //}
            //else if (backAngle < middleAngle && backAngle < frontAngle && backAngle < angle)
            //{
            //    ground = states.backNormal;
            //    angle = backAngle;
            //}
            //else
            //{
            //    ground = states.groundNormal;
            //}

            float angle2 = Vector3.Angle(states.mTransform.up, Vector3.up);
            //Debug.Log("angle 2 - " + angle2);


            // QUATERNION WAY

            float amount = 6;

            var test = states.GetComponent<Animation>();

            if (states.rotateFast && !states.anim.GetBool(states.hashes.isLanding))
            {
                bool quickLand = states.anim.GetBool(states.hashes.QuickLand);

                if (!quickLand)
                    amount = 30;
                else
                    amount = 15;
            }

            if (angle < rotationConstraint)
            {
                Quaternion tr = Quaternion.FromToRotation(states.mTransform.up, ground) * states.mTransform.rotation;
                Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * amount);
                states.mTransform.rotation = targetRotation;
            }

            if (Mathf.Abs((angle2 - angle)) < 1.0f)
            {
                states.anim.SetBool(states.hashes.QuickLand, false);
            }

            // NON-Quaternion way

            /*Vector3 newAxis = Vector3.Cross(states.mTransform.up, states.groundNormal);
            float angle3 = Vector3.Angle(states.mTransform.up, states.groundNormal);
            states.mTransform.RotateAround(states.mTransform.position, newAxis, angle3);*/
        }
    }
}
