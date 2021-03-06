﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// For player movement in the air
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Air Movement")]
    public class AirMovement : StateActions
    {
        public float movementSpeed;
        public float movementTime = 10;
        public float upwardsGravity;
        public float downwardsGravity;
        public bool shortHop = false;
        public bool letGo = false;
        public float extraGrav;
        public float extraGravShort = 4.5f;
        public float extaGravFull = 3.5f;
        public float rotationConstraint = 70;

        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            Vector3 currentVelocity = states.rigid.velocity;
            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * movementSpeed * states.airSpeedMult;

            extraGrav = upwardsGravity;

            // Is jump being pressed?
            if (Input.GetKeyUp("joystick button 3") || Input.GetKeyUp("joystick button 0") || Input.GetKeyUp("space"))
            {
                // was <= 10, noticed it was broken after Camera
                // Needs to be at least a minimum jump
                if (currentVelocity.y <= 12)
                {
                    // Did they let go before short hop window? If so, do short hop. If not, they're letting go after short hop window
                    if (currentVelocity.y >= 7.5f)
                    {
                        shortHop = true;
                    }
                    else
                    {
                        letGo = true;
                    }
                }
            }

            if (shortHop && currentVelocity.y < 7.5f)
            {
                extraGrav = extraGravShort;
                shortHop = false;
            }

            if (letGo)
            {
                extraGrav = extaGravFull;
                letGo = false;
            }

            if (currentVelocity.y > 0)
            {
                targetVelocity.y = currentVelocity.y -= extraGrav;
            }
            else
            {
                targetVelocity.y = currentVelocity.y -= downwardsGravity;
            }




            float groundedDis = 2.9f;

            Vector3 origin = states.mTransform.position;
            origin += states.mTransform.forward;

            origin.y += .7f;

            Vector3 dir = -Vector3.up;

            //TODO: testing this

            if (states.isGrounded)
            {
                // If player is on a sloped surface, must account for the normal
                dir.z = dir.z - states.groundNormal.z;
            }
            else
            {
                dir.z = dir.z - states.mTransform.up.z;
                //dir.z = dir.z 
            }

            float dis = groundedDis;
            RaycastHit hit;

            //Debug.DrawRay(origin, dir * dis, Color.blue);

            //// If player is directly above a slope
            //if (Physics.Raycast(origin, dir, out hit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            //{
            //    // Get player's angle
            Vector3 rotationNormal = states.GetRotationNormal(2.25f);
            float angle = Vector3.Angle(rotationNormal, Vector3.up);
            //    //Debug.Log(angle);
            //    //Debug.DrawRay(hit.point, hit.normal, Color.red, 3.0f);
            //    // when right above slope, pre-rotate towards that slope's angle
            if (angle <= 85)
            {
                Quaternion tr = Quaternion.FromToRotation(states.mTransform.up, rotationNormal) * states.mTransform.rotation;
                Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * 8f);
                states.mTransform.rotation = targetRotation;
            }
            //    else if (angle > 35 && angle < 70)
            //    {
            //        Quaternion tr = Quaternion.FromToRotation(states.mTransform.up, hit.normal) * states.mTransform.rotation;
            //        Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * 14.75f);
            //        states.mTransform.rotation = targetRotation;
            //    }
            //}
            //// If player is not directly above slope
            //else
            //{
            //    // Slowly rotate back towards 0 degrees

            //    Quaternion tr = Quaternion.FromToRotation(states.mTransform.up, Vector3.up) * states.mTransform.rotation;
            //    Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * 1.75f);
            //    states.mTransform.rotation = targetRotation;
            //}

            //TODO: properly implement step up logic for air movement
            if (states.stepUp)
            {
                bool positive = false;

                var test = states.rigid.position;

                //var test = states.mTransform.forward.z;
                if (test.z > 0)
                    positive = true;
                //Debug.Log(states.mTransform.forward.z);

                /*if (positive)
                     test.z += .2f;
                 else
                     test.z -= -.2f;*/
                test.y = test.y + .1f;

                //TODO
                //I turned this off
                //states.rigid.position = Vector3.Lerp(states.rigid.position, test + (states.mTransform.forward / 8), states.delta * 10);



                //states.rigid.velocity = new Vector3(states.rigid.velocity.x, 0, states.rigid.velocity.z);
                //targetVelocity.y += 1f;
                //states.rigid.AddRelativeForce(0, 10f, 0);
                //targetVelocity.y += 1;
                //holdYVel = targetVelocity.y;
            }



            // Apply velocity to player
            states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity + states.slideMomentum, states.delta * movementTime);
        }
    }
}
