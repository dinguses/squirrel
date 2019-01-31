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

        public override void Execute(StateManager states)
        {

            float angle = Vector3.Angle(states.groundNormal, Vector3.up);
            //Debug.Log("angle 1 - " + angle);

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

            if (angle < 35)
            {
                Quaternion tr = Quaternion.FromToRotation(states.mTransform.up, states.groundNormal) * states.mTransform.rotation;
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
