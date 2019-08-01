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
            //Too many raycasts
            //Vector3 origin = states.transform.position;
            //origin.y += 0.5f;
            //Vector3 frontLeft, frontRight, backLeft, backRight;
            //RaycastHit frontLeftHit, frontRightHit, backLeftHit, backRightHit;
            //backLeft = origin - (states.transform.right * 0.5f) - (states.transform.forward * 0.05f);
            //backRight = origin + (states.transform.right * 0.5f) - (states.transform.forward * 0.05f);
            //frontLeft = backLeft + (states.transform.forward * 2f);
            //frontRight = backRight + (states.transform.forward * 2f);
            //Debug.DrawRay(frontLeft, -Vector3.up * (states.GetLength() + 0.15f), Color.green);
            //Debug.DrawRay(frontRight, -Vector3.up * (states.GetLength() + 0.15f), Color.green);
            //Debug.DrawRay(backLeft, -Vector3.up * (states.GetLength() + 0.15f), Color.green);
            //Debug.DrawRay(backRight, -Vector3.up * (states.GetLength() + 0.15f), Color.green);

            //bool blHit = Physics.Raycast(backLeft, -Vector3.up, out backLeftHit, (states.GetLength() + 0.5f), Layers.ignoreLayersController);
            //bool brHit = Physics.Raycast(backRight, -Vector3.up, out backRightHit, (states.GetLength() + 0.5f), Layers.ignoreLayersController);
            //bool flHit = Physics.Raycast(frontLeft, -Vector3.up, out frontLeftHit, (states.GetLength() + 0.5f), Layers.ignoreLayersController);
            //bool frHit = Physics.Raycast(frontRight, -Vector3.up, out frontRightHit, (states.GetLength() + 0.5f), Layers.ignoreLayersController);


            //Vector3 front = states.transform.position + (states.transform.forward * 1.9f);
            //front.y += .35f;


            // Get the vectors that connect the raycast hit points

            //Vector3 a = backRightHit.point - backLeftHit.point;
            //Vector3 b = frontRightHit.point - backRightHit.point;
            //Vector3 c = frontLeftHit.point - frontRightHit.point;
            //Vector3 d = backRightHit.point - frontLeftHit.point;

            //Debug.DrawRay(backRightHit.point, Vector3.up);
            //Debug.DrawRay(backLeftHit.point, Vector3.up);
            //Debug.DrawRay(frontLeftHit.point, Vector3.up);
            //Debug.DrawRay(frontRightHit.point, Vector3.up);
            // Get the normal at each corner

            //Vector3 crossBA = Vector3.Cross(b, a);
            //Vector3 crossCB = Vector3.Cross(c, b);
            //Vector3 crossDC = Vector3.Cross(d, c);
            //Vector3 crossAD = Vector3.Cross(a, d);

            // Calculate composite normal
            //if (didFrontHit && didBackHit)
            //{   //ground = (crossBA + crossCB + crossDC + crossAD).normalized;
            //    //   ground = (Vector3.Cross(backRightHit.point - Vector3.up, backLeftHit.point - Vector3.up) +
            //    // Vector3.Cross(backLeftHit.point - Vector3.up, frontLeftHit.point - Vector3.up) +
            //    // Vector3.Cross(frontLeftHit.point - Vector3.up, frontRightHit.point - Vector3.up) +
            //    // Vector3.Cross(frontRightHit.point - Vector3.up, backRightHit.point - Vector3.up)
            //    //).normalized;
            //    ground = (frontHit.normal + states.frontNormal + backHit.normal).normalized;
            //}
            //if (didFrontHit && states.front == null)
            //{
            //    ground = frontHit.normal;
            //}
            if (states.front == null)
            {
                Vector3 origin = states.transform.position;
                origin.y += .35f;

                //RaycastHit frontHit;
                RaycastHit hit;
                //bool didFrontHit;
                bool didHit;
                Vector3 dir = Quaternion.AngleAxis(states.transform.rotation.eulerAngles.y, Vector3.up) * Vector3.forward;
                Debug.DrawRay(origin, -dir * 0.6f, Color.green);
                didHit = Physics.Raycast(origin, -dir, out hit, 0.6f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore);


                //if (didFrontHit)
                //{
                //    float frontAngle = Vector3.Angle(frontHit.normal, Vector3.up);
                //    if (frontAngle >= 70)
                //        didFrontHit = false;
                //}
                if (didHit)
                {
                    float backAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if (backAngle >= 70)
                        didHit = false;
                    else
                        ground = hit.normal;
                }

                if (!didHit)
                {
                    ground = states.groundNormal;
                }
            }
            else
                ground = states.frontNormal;
            //if (states.middle != null)
            //    ground = states.middleNormal;
            //else
            //    middleAngle = rotationConstraint;

            //if (states.front != null)
            //else
            //    frontAngle = rotationConstraint;

            //if (states.back != null)
            //    ground = states.backNormal;
            //else
            //    backAngle = rotationConstraint;

            float angle = Vector3.Angle(ground, Vector3.up);
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
