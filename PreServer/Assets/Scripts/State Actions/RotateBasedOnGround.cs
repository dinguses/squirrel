﻿using System.Collections;
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
        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            ground = states.GetRotationNormal();
            //Vector3 origin = states.transform.position + (states.transform.up * 0.2f) + (states.transform.forward * 1.75f);
            //RaycastHit hit = new RaycastHit();
            //Vector3 dir = SlidePlayer.ProjectVectorOnPlane(Vector3.up, states.transform.forward);
            //Debug.DrawRay(origin, dir * 0.5f, Color.blue);
            ////Raycast in front of the squirrel, used to check if we've hit a ceiling, ground, or another climb-able surface
            //if (Physics.SphereCast(origin, 0.2f, states.transform.forward, out hit, 0.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            //{
            //    ground = (hit.normal + ground).normalized;
            //    Debug.DrawRay(hit.point, hit.normal, Color.black);
            //}
            ////If the front raycast hasn't hit anything then do another raycast 
            ////Otherwise just use the front normal
            //if (states.front == null)
            //{
            //    //This raycast goes out the back and points diagonally behnd the player
            //    //If this hits something then use that as the ground unless it hit a wall
            //    Vector3 origin = states.transform.position;
            //    origin += states.transform.up * 0.35f;
            //    RaycastHit hit;
            //    bool didHit;
            //    Vector3 dir = -states.transform.forward - states.transform.up;
            //    Debug.DrawRay(origin, dir * 0.6f, Color.green);
            //    didHit = Physics.Raycast(origin, dir, out hit, 0.6f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore);

            //    if (didHit)
            //    {
            //        float backAngle = Vector3.Angle(hit.normal, Vector3.up);
            //        if (backAngle >= 70)
            //            didHit = false;
            //        else
            //            ground = hit.normal;
            //    }

            //    if (!didHit)
            //    {
            //        ground = states.groundNormal;
            //    }
            //}
            //else
            //    ground = states.frontNormal;

            float angle = Vector3.Angle(ground, Vector3.up);

            float angle2 = Vector3.Angle(states.mTransform.up, Vector3.up);
            //Debug.Log("angle 2 - " + angle2);


            // QUATERNION WAY

            float amount = 15;

            var test = states.GetComponent<Animation>();

            if (states.rotateFast && !states.anim.GetBool(states.hashes.isLanding))
            {
                bool quickLand = states.anim.GetBool(states.hashes.QuickLand);

                if (!quickLand)
                    amount = 30;
                //else
                //    amount = 15;
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
