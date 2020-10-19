using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Moves the player while grinding
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Grind Movement")]
    public class GrindMovement : StateActions
    {
        public float dashSpeed = 40f;
        public float dashTime = 0.15f;
        float timer;
        bool dashActivated;
        PlayerManager states;
        Vector3 grindCenterClosestPoint;
        bool adjusting = false;
        Vector3 optimalPoint;
        Vector3 behindVector;
        Vector3 reusable;
        public RaycastHit climbHit;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            base.OnEnter(states);

            states.dashInAirCounter = 0;
            timer = 0;
            dashActivated = false;
            inPos = false;
            inRot = false;
            adjusting = false;
            reusable = states.mTransform.position;
            states.grindDoneAdjusting = false;
            states.rotateDelayTest = 0;

            if (!states.comingBackFrom180)
            {
                Debug.Log("initial adjustment");
                grindSetup();
            }
            else
            {
                Debug.Log("back from a 180");
                adjusting = false;
                states.comingBackFrom180 = false;
                states.grindDoneAdjusting = true;
            }
        }

        public void grindSetup()
        {
            behindVector = -(states.facingPoint - states.behindPoint).normalized;
            Vector3 testFront = states.mTransform.position + (states.mTransform.forward * 2);

            Vector3 frontClosest = GetPoint(testFront, states.facingPoint, states.behindPoint);
            Vector3 backClosest = GetPoint(states.mTransform.position, states.facingPoint, states.behindPoint);

            if (Vector3.Distance(frontClosest, testFront) < Vector3.Distance(backClosest, states.mTransform.position))
            {
                backClosest = frontClosest + (behindVector * 2f);
            }
            else
            {
                frontClosest = backClosest - (behindVector * 2f);
            }

            //Vector3 frontClosest = GetPoint(testFront, states.facingPoint, states.behindPoint);

            RaycastHit hit = new RaycastHit();

            optimalPoint = backClosest;

            Vector3 projectedVector = SlidePlayer.ProjectVectorOnPlane(-behindVector, Vector3.up);

            if (Physics.Raycast(frontClosest + (projectedVector * 0.2f), behindVector, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Front to back hit");
                optimalPoint = backClosest - (behindVector * (2f - hit.distance));
            }
            else if (Physics.Raycast(backClosest + (projectedVector * 0.2f), -behindVector, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("back to front hit");
                optimalPoint = backClosest + (-behindVector * (2f - hit.distance));
            }

            //var end0 = states.grindEnds[0];

            /*if (Physics.Raycast(testFront, optimalPoint, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hit.collider.tag);
            }*/

            //if (states.facingPoint == states.grindPoints[0])
            //{
                //Debug.Log("optimalPoint - " + Vector3.Distance(optimalPoint, states.facingPoint));
                //Debug.Log("end0 - " + Vector3.Distance(end0, states.facingPoint));

                //if (Vector3.Distance(optimalPoint, states.facingPoint) < Vector3.Distance(end0, states.facingPoint))
                //{
                    //states.LeaveGrindOverride();

                    /*var holdFacing = states.facingPoint;
                    var holdFacingPair = states.facingPointPair;
                    states.facingPoint = states.behindPoint;
                    states.facingPointPair = states.behindPointPair;
                    states.behindPoint = holdFacing;
                    states.behindPointPair = holdFacingPair;*/


                    //states.LeftUndergroundGrind();


                   // Debug.Log("fruit swapping");
                //}

                //Debug.Log("ya facing the end");
            //}

            /*Debug.Log("opt = " + optimalPoint + " AND grind end = " + states.grindEnds[0]);
            Debug.Log("something's amiss - " + (states.grindEnds[0].y - optimalPoint.y));

            if (optimalPoint.y < states.grindEnds[0].y)
            {
                OnExit(states);
            }*/

            //states.frontCollider.enabled = false;
            adjusting = true;
        }

        public override void Execute(StateManager states)
        {

        }

        public void FacePoint()
        {
            // TODO this needs to get called before RotateGrind, which can't happen while RotateGrind is it's own script
            // REMEMBER: GrindMovement gets called after the Update scripts in a state

            var reusable = (states.mTransform.position);
            var _direction = (states.facingPoint - reusable).normalized;
            var _lookRotation = Quaternion.LookRotation(_direction);
            //states.mTransform.rotation = Quaternion.Slerp(states.mTransform.rotation, _lookRotation, states.delta * 10.0f * states.groundSpeedMult);
        }

        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);

            //var end0 = states.grindEnds[0];
            Debug.DrawLine(states.mTransform.position, optimalPoint, Color.yellow);
            //Debug.Log("optimalPoint - " + Vector3.Distance(optimalPoint, states.facingPoint));
            //Debug.Log("end0 - " + Vector3.Distance(end0, states.facingPoint));

            FacePoint();

            if (adjusting)
            {
                Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * 10.5f;
                Vector3 currentVelocity = states.rigid.velocity;

                // Set velocity
                states.targetVelocity = targetVelocity;
                states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * 50.5f);

                Adjust(states);
            }
            else
            {

                if (states.rotateDelayTest < 10)
                    states.rotateDelayTest++;

                //TODO: This where the squirrel should SLERP to the initial grind spot, pos + rot
                // Lock off movement like in ClimbingMovement (OnUpdate + Transfer())

                // TODO: This is old dash, need to upgrade it to new dash
                if (timer <= 0 && states.dashActive && states.CanDash() && !dashActivated)
                {
                    states.anim.CrossFade(states.hashes.squ_dash, 0.01f);
                    states.anim.SetBool(states.hashes.isDashing, true);

                    states.rigid.velocity = states.transform.forward * dashSpeed;
                    if (states.isRun)
                    {
                        timer = 0.225f;
                        states.speedHackAmount -= 0.2f;
                        if (states.speedHackAmount <= 0)
                        {
                            states.speedHackAmount = 0;
                            states.runRanOut = true;
                        }
                    }
                    else
                    {
                        timer = 0.15f;
                    }

                    dashActivated = true;
                }

                // If not dashing
                if (!states.dashActive)
                {
                    //states.mTransform.rotation = Quaternion.LookRotation((states.facingPoint - states.behindPoint).normalized);
                    var testQuat = Quaternion.Euler((states.facingPoint - states.behindPoint).normalized);
                    states.mTransform.rotation = Quaternion.RotateTowards(states.mTransform.rotation, testQuat, Time.deltaTime * 9f);

                    // FACE POINT MOVED HERE
                    // Rotate towards the facing point
                    /*var _direction = (states.facingPoint - reusable).normalized;
                    var _lookRotation = Quaternion.LookRotation(_direction);

                    // Set the rotation
                    states.mTransform.rotation = Quaternion.Slerp(states.mTransform.rotation, _lookRotation * testQuat, states.delta * 10.0f * states.groundSpeedMult);*/


                    states.rigid.drag = 0;
                    states.rotateBool = true;

                    // Get target velocity from player's move amount and current velocity           
                    Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * 10.5f * states.groundSpeedMult;
                    Vector3 currentVelocity = states.rigid.velocity;

                    // Set velocity
                    states.targetVelocity = targetVelocity;
                    states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * 10.5f);

                    // Move Player towards center should they not be on it

                    Vector3 testMiddle = states.mTransform.position + states.mTransform.forward;
                    Vector3 testFront = states.mTransform.position + (states.mTransform.forward * 2);

                    grindCenterClosestPoint = GetPoint(testFront, states.facingPoint, states.behindPoint);

                    ////TODO: This busted 180s on corners, fix it
                    if (states.movementVariables.moveAmount > 0.1)
                    {
                        //grindCenterClosestPoint = GetPoint(testMiddle, states.facingPoint, states.behindPoint);
                    }

                    states.rigid.position = Vector3.Lerp(states.rigid.position, (grindCenterClosestPoint - (states.mTransform.forward * 2)), Time.deltaTime * 20f);

                    //if (Vector3.Distance(states.rigid.position, (grindCenterClosestPoint - states.mTransform.forward)) < .1f)
                    //{
                    //    //Debug.Log("Done adjusting!");
                    //    states.doneAdjustingGrind = true;
                    //    states.frontCollider.enabled = true;
                    //}
                    //else
                    //{
                    //    /*states.frontCollider.enabled = false;
                    //    states.rigid.position = Vector3.Lerp(states.rigid.position, (grindCenterClosestPoint - states.mTransform.forward), Time.deltaTime * 20f);*/
                    //}

                    Vector3 facingVector = (states.facingPoint - states.behindPoint).normalized;


                    Vector3 topRay = states.mTransform.position + (states.mTransform.forward * 0.9f) + (states.mTransform.up * 0.5f);
                    if (Physics.SphereCast(topRay, 0.3f, states.mTransform.forward, out climbHit, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    {
                        Debug.Log("leaving because of facecast");

                        if (climbHit.collider.tag != "grindMeat")
                        {
                            states.LeaveGrindOverrideNoMovement();
                        }
                    }

                    #region OLD
                    //Debug.DrawRay(states.mTransform.position, Vector3.Cross(facingVector, Vector3.right), Color.magenta);
                    //Debug.DrawRay(states.mTransform.position, SlidePlayer.ProjectVectorOnPlane(facingVector, Vector3.up), Color.yellow);

                    //states.rigid.position = Vector3.Lerp(states.rigid.position, grindCenterClosestPoint, Time.deltaTime * 10f * (states.groundSpeedMult * 2));
                    /*states.rigid.position = Vector3.Lerp(states.rigid.position, (grindCenterClosestPoint - states.mTransform.forward), Time.deltaTime * 20f);


                    Debug.DrawRay(testMiddle, facingVector, Color.blue);

                    Debug.DrawRay(grindCenterClosestPoint, Vector3.up, Color.green);

                    // If you haven't fully adjusted to the grind center initially
                    if (!states.doneAdjustingGrind || Vector3.Distance(states.rigid.position, (grindCenterClosestPoint - states.mTransform.forward)) > .1f)
                    {
                        Debug.Log("cc - " + Vector3.Distance(states.rigid.position, (grindCenterClosestPoint - states.mTransform.forward)));

                        // If position is close enough to grind center, adjusting is done
                        if (Vector3.Distance(states.rigid.position, (grindCenterClosestPoint - states.mTransform.forward)) < .1f)
                        {
                            Debug.Log("Done adjusting!");
                            states.doneAdjustingGrind = true;
                            states.frontCollider.enabled = true;
                        }
                    }
                    else
                    {
                        //states.frontCollider.enabled = true;
                    }*/
                    #endregion

                }

                // Decrement timer 
                timer -= Time.deltaTime;

                // If dash time is over, dash is no longer active, and initial dash bool has been hit
                if (timer < 0 && states.dashActive && dashActivated)
                {
                    states.dashActive = false;
                    states.rigid.velocity = Vector3.zero;
                    states.anim.SetBool(states.hashes.isDashing, false);
                    states.lagDashCooldown = 1.0f;
                    dashActivated = false;
                    states.playerMesh.gameObject.SetActive(true);
                }
            }

        }

        bool inPos;
        bool inRot;

        void Adjust(StateManager sm)
        {


            //TODO: For jitter
            // DO this instead of rotatebasedongrind
            // Make sure that doesn't break 180 slope rotation
            // Has to get called before 180, or if 180 happens this 
            // Move GrindRotation + RotateBasedOnGrind to GrindMovement

            //Debug.Log("adjusting...");


            if (Vector3.Distance(states.transform.position, optimalPoint) <= 0.75/* || (Vector3.Distance(states.transform.position, states.facingPoint) < Vector3.Distance(optimalPoint, states.facingPoint))*/)
            {
                Debug.Log("DONE ADJUSTING!@");
                states.doneAdjustingGrind = true;
                states.frontCollider.enabled = true;
                inPos = true;
            }

            if (!inPos)
            {
                float slow = (states.movementVariables.moveAmount > .5f ? 1f : .5f);
                states.mTransform.position = Vector3.Lerp(states.mTransform.position, optimalPoint, Time.deltaTime * 15 * slow * (states.groundSpeedMult * ((states.groundSpeedMult > 1f) ? 2 : 1)));
                //states.mTransform.rotation = Quaternion.LookRotation(-behindVector);

                var testQuat2 = Quaternion.Euler(-behindVector);
                states.mTransform.rotation = Quaternion.RotateTowards(states.mTransform.rotation, testQuat2, Time.deltaTime * 7f * slow * (states.groundSpeedMult * ((states.groundSpeedMult > 1f) ? 2 : 1)));
            }
            else
            {
                adjusting = false;
                states.grindDoneAdjusting = true;
            }
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);

            states.frontCollider.enabled = true;

            states.grindTimer = 0f;

            if (states.dashActive)
            {
                states.dashActive = false;
                states.lagDashCooldown = 1.0f;
                states.anim.SetBool(states.hashes.isDashing, false);
                states.doneAdjustingGrind = false;
            }
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }
    }
}
