using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Player's movement
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Movement Forward With Angle")]
    public class MovementForward : StateActions
    {
        public float frontRayOffset = .5f;
        public float movementSpeed;
        public float movementTime = 10;

        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            float frontY = 0;
            RaycastHit hit;
            RaycastHit hit2;
            Vector3 origin = states.mTransform.position + (states.mTransform.forward * frontRayOffset);
            Vector3 origin2 = states.mTransform.position;
            origin.y += .5f;
            origin2.y += .5f;

            Vector3 testOrigin = states.mTransform.position + (states.mTransform.forward * .75f);
            testOrigin.y += .5f;
            RaycastHit testHit;

            //Debug.DrawRay(origin2, -Vector3.up, Color.red);
            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * movementSpeed * states.groundSpeedMult;

            Vector3 dir = -Vector3.up;

            // If player is on a sloped surface, must account for the normal
            dir.z = dir.z - states.groundNormal.z;

            //Debug.DrawRay(origin, dir, Color.red);
            //Debug.DrawRay(origin2, dir, Color.cyan);

            // Raycast from first origin point
            if (Physics.Raycast(origin, dir, out hit, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                // Store y position of hit point
                float y = hit.point.y;

                // Set StateManager ground normal to be normal of hit
                if (Vector3.Angle(hit.normal, Vector3.up) <= 35)
                {
                    states.groundNormal = hit.normal;
                    states.backupGroundNormal = hit.normal;
                }
                else
                {
                    states.backupGroundNormal = hit.normal;
                }

                if (Physics.Raycast(testOrigin, dir, out testHit, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {

                }

                // Set frontY to hit point y pos - player's y pos
                //WHY?
                frontY = y - states.mTransform.position.y;

                // Second Raycast from second origin point
                if (Physics.Raycast(origin2, dir, out hit2, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    // If front hit is on a different ground than second hit, most likeley moving between two different grounds
                    // If so, shouldn't rotate quickly
                    if (hit.normal != hit2.normal)
                        states.rotateFast = false;
                    else
                        states.rotateFast = true;
                }

                // Trying to address the problem of player "landing" too high up, and then slowly sinking into the slope

                //Debug.Log("hit 1 distance - " + hit.distance);
                //Debug.Log("hit 2 distance - " + hit2.distance);

                // If either of the two raycast hits are too far down
                if (hit.distance > .54f || hit2.distance > .54f)
                {
                    //targetVelocity.y -= 45f;

                    // Set player's position slighly down and then move towards it
                    var test = states.rigid.position;
                    test.y -= .05f;
                    states.rigid.MovePosition(test);
                }
            }

            Vector3 currentVelocity = states.rigid.velocity;

            // If grounded
            if (states.isGrounded)
            {
                // Get move amount from movement variables
                float moveAmount = states.movementVariables.moveAmount;

                // If at least 0.1f movement
                if (moveAmount > 0.1f)
                {
                    // Set drag to 0 while moving
                    states.rigid.drag = 0;

                    // If player is on a slope
                    if (Mathf.Abs(frontY) > 0.02f)
                    {
                        //targetVelocity.y = ((frontY > 0) ? 1 : -1) * movementSpeed;
                        //targetVelocity.y = frontY * movementSpeed;

                        // Set y for target vel
                        // If frontY is positive (upward slope) then add .2f to the velocity
                        // If frontY is negative (downward slope) then don't do anything
                        //targetVelocity.y = ((frontY > 0) ? frontY + 0.2f : frontY) * movementSpeed;

                        //// If targetVelocity.y is greater than 0, then set it to 0
                        //// TODO: These two sections seem to cancel each other out
                        //// This cancels out the extra jump you could get off of slopes
                        //if (targetVelocity.y > 0)
                        //{
                        //    targetVelocity.y = 0;
                        //}
                    }
                }

                // If not moving at least 0.1f
                else
                {
                    // Get abs of frontY
                    float abs = Mathf.Abs(frontY);

                    // If player is on any slope
                    if (abs > 0.02f && states.slideMomentum.magnitude < 0.25f)
                    {
                        //states.rigid.isKinematic = false;

                        // Stop moving and increase drag so that player doesn't slide while idling
                        // TODO: check what lower drag values work instead of 4000. Probably is too high.
                        //targetVelocity.y = 0;
                        states.rigid.drag = 4000;
                    }
                }
            }
            else
            {
                states.rigid.isKinematic = false;
                states.rigid.drag = 0;
                targetVelocity.y = currentVelocity.y;
            }

            #region Step up stuff

            // If step up is true
            // Right now, moves y position up slightly
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
                test.y = test.y + .25f;
                states.rigid.position = Vector3.Lerp(states.rigid.position, test + (states.mTransform.forward / 4), states.delta * 10);
                //states.rigid.velocity = new Vector3(states.rigid.velocity.x, 0, states.rigid.velocity.z);
                //targetVelocity.y += 1f;
                //states.rigid.AddRelativeForce(0, 10f, 0);
                //targetVelocity.y += 1;
                //holdYVel = targetVelocity.y;
            }

            // Step up Jump testing
            if (states.stepUpJump)
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
                     test.z -= -.1f;*/
                test.y = test.y + .1f;
                //states.rigid.position = Vector3.Lerp(states.rigid.position, test, states.delta * 20);
                //targetVelocity.y += 1f;
                //states.rigid.AddRelativeForce(0, 10f, 0);
                //targetVelocity.y += 1;
                //holdYVel = targetVelocity.y;
            }
            #endregion

            //(Just for moving quickly in Debug - NOT how it will be done later in game)
            //if (states.isRun)
            //{
            //    targetVelocity = targetVelocity * 5;
            //}


            states.targetVelocity = targetVelocity;
            states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity + states.slideMomentum, states.delta * movementTime);
            //Debug.Log(states.rigid.velocity);
        }
    }
}
