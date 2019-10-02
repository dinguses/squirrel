using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;
namespace PreServer
{
    /// <summary>
    /// TODO: Detach when climbing off platforms, jump forward on climbs, jump backwards
    /// </summary>
    [CreateAssetMenu(menuName = "Actions/State Actions/Climbing Movement")]
    public class ClimbingMovement : StateActions
    {
        public float climbSpeed = 2f;
        public float rotationSpeed = 3f;
        public TransformVariable cameraTransform;
        Vector3 climbRight;
        Vector3 climbUp;
        Vector3 climbForward;
        bool transitioning = false;
        RaycastHit front;
        RaycastHit under;
        public float dashSpeed = 40f;
        public float dashTime = 0.15f;
        bool dash = false;
        float timer = 0;
        bool dashActivated = false;
        bool moveCamera = false;
        CameraManager camera;
        float cameraAngle = 0;
        float tempAngle = 0;
        PlayerManager states;
        float prevAngle = 0;
        public float minTransferAngle = 30f;
        bool ignoreGravity = false;

        //Might have to do some readjustments in check raycast when transitioning, still buggy on faces with varying normals
        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;

            base.OnEnter(states);
            if(camera == null && cameraTransform != null)
            {
                camera = cameraTransform.value.GetComponent<CameraManager>();
            }

            front = new RaycastHit();
            under = new RaycastHit();
            transitioning = false;
            states.isGrounded = false;
            states.dashInAirCounter = 0;
            dash = false;
            timer = 0;
            dashActivated = false;
            moveCamera = false;
            if (camera != null)
                camera.ignoreInput = false;
            states.rigid.drag = 0;
            states.playerMesh.gameObject.SetActive(true);
            CheckUnder();
        }

        public override void Execute(StateManager sm)
        {

        }

        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);

            if (transitioning)
            {
                // Wrap around anim here
                // top is in entering/exiting climb

                //states.anim.SetBool(states.hashes.climbCorner, true);
                

                Transfer(states);
                //Rotate(states);
                Move(states);
            }
            else
            {
                RotateTowardsClimb();
                if (timer <= 0 && states.dashActive && states.CanDash() && !dashActivated)
                {
                    states.anim.CrossFade(states.hashes.squ_dash, 0.01f);
                    states.anim.SetBool(states.hashes.isDashing, true);

                    //Debug.Log("Adding velocity 9");
                    states.rigid.velocity = states.transform.forward * dashSpeed - (under.normal.normalized * dashSpeed * 0.5f);
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
                if (!states.dashActive)
                {
                    Rotate(states);
                    Move(states);
                }
                CheckRaycast(states);
            }
            timer -= Time.deltaTime;
            if (timer < 0 && states.dashActive && dashActivated)
            {
                states.dashActive = false;
                states.rigid.velocity = Vector3.zero;
                states.anim.SetBool(states.hashes.isDashing, false);
                //Debug.Log("Dash over");
                states.lagDashCooldown = 1.0f;
                dashActivated = false;
                states.playerMesh.gameObject.SetActive(true);
            }
            Debug.DrawRay(states.climbHit.point, states.climbHit.normal * 3f, Color.black);
        }

        bool frontHit = false, underHit = false;
        void CheckRaycast(StateManager sm)
        {
            float topFloat = .4f;
            Vector3 frontOrigin = states.transform.position + (states.transform.forward * 1.5f) + (states.transform.up * topFloat);

            Debug.DrawRay(frontOrigin, states.transform.forward * 0.75f, Color.blue);
            //Raycast in front of the squirrel, used to check if we've hit a ceiling, ground, or another climb-able surface
            if (Physics.Raycast(frontOrigin, states.transform.forward, out front, 0.75f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                frontHit = true;
                float angle = Vector3.Angle(front.normal, Vector3.up);
                //front has hit a new climb that means the change is a big one, no need to check the angle
                if (front.transform.tag == "Climb")
                {
                    ignoreGravity = true;
                    prevAngle = Vector3.Angle(front.normal, states.climbHit.normal);
                    states.rigid.velocity = Vector3.zero;
                    //Debug.Log("Front are you fucking things up, do I have to fix you?");
                    if (states.dashActive)
                    {
                        states.dashActive = false;
                        states.lagDashCooldown = 1.0f;
                        dashActivated = false;
                    }
                    tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, front.normal, Vector3.up);
                    if (tempAngle < 90 && tempAngle > -90)
                    {
                        cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                        moveCamera = true;
                        if (camera != null)
                            camera.ignoreInput = true;
                    }
                    states.climbHit = front;
                    startPos = states.transform.position;
                    targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                    targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                    t = 0;
                    inPos = false;
                    inRot = false;

                    transitioning = true;
                    states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                    return;
                }
                else
                {
                    if (angle < 70)
                    {
                        //Debug.Log("Front has detected non-climbable surface, exit the climb");
                        states.rigid.velocity = Vector3.zero;
                        states.climbHit = front;
                        //states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                        states.climbState = PlayerManager.ClimbState.EXITING;
                        return;
                    }
                    else
                    {
                        //Debug.Log("Front has detected a wall that blocks the user and isn't climbable");
                        states.rigid.velocity = Vector3.zero;
                    }
                }
            }
            else
                frontHit = false;


            Vector3 underOrigin = states.transform.position;
            underOrigin += states.transform.forward + states.transform.forward / 2.0f + (states.transform.up * 0.2f);

            // Dir represents the downward direction
            Vector3 dir = -states.transform.forward + (-states.transform.up * 0.5f);

            // Draw the rays
            Debug.DrawRay(underOrigin, dir * 1.6f, Color.green);

            if (Physics.Raycast(underOrigin, dir, out under, 1.6f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                //underside has hit something
                underHit = true;
                float angle = Vector3.Angle(under.normal, Vector3.up);
                //If I hit another climb, check if I should transfer to it
                if(under.transform.tag == "Climb")
                {
                    angle = Vector3.Angle(under.normal, states.climbHit.normal);
                    states.climbHit = under;
                    //if(Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                        //Debug.Log(Mathf.Abs(angle - prevAngle));
                    //if the angle between the new climb and the current one is greater than the transfer amount, then transfer to it
                    if (Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                    {
                        prevAngle = angle;
                        //Debug.Log("Under has detected a new climb to transition to");
                        states.rigid.velocity = Vector3.zero;

                        //Stop dashing
                        if (states.dashActive)
                        {
                            states.dashActive = false;
                            states.lagDashCooldown = 1.0f;
                            dashActivated = false;
                        }

                        //Camera rotations, checking if the camera needs to be repositioned based on where it is relative to the climb
                        tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, under.normal, Vector3.up);
                        if (tempAngle < 90 && tempAngle > -90)
                        {
                            cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                            moveCamera = true;
                            if (camera != null)
                                camera.ignoreInput = true;
                        }

                        ////Update variables for transfer
                        startPos = states.transform.position;
                        targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                        targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                        t = 0;
                        inPos = false;
                        inRot = false;
                        transitioning = true;
                        states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                        return;
                    }

                }
                //I didn't hit a climb but I must do something based on the angle of the hit
                else
                {
                    //detach
                    if (angle < 70)
                    {
                        //Debug.Log("Under has detected non-climbable surface, exit the climb");
                        states.climbHit = under;
                        states.climbState = PlayerManager.ClimbState.EXITING;

                        states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                        states.anim.SetBool(states.hashes.isClimbing, false);
                        states.rigid.velocity = Vector3.zero;

                        return;
                    }
                    ////detach
                    //else if (angle > 90)
                    //{
                    //    Debug.Log("Under has detected a wall that blocks the user and isn't climbable");
                    //    states.climbState = PlayerManager.ClimbState.NONE;
                    //    states.isJumping = true;
                    //    states.anim.SetBool(states.hashes.isClimbing, false);
                    //    return;
                    //}
                    //stop moving
                    else
                    {

                        //Debug.Log("Under has detected a non-climbable surface, stop moving");
                        states.rigid.velocity = Vector3.zero;
                        return;
                    }
                }

            }
            else
            {
                underHit = false;
                states.rigid.velocity = Vector3.zero;
                Debug.Log("Under isn't hitting anything");
                states.climbState = PlayerManager.ClimbState.NONE;
                states.isJumping = true;
                states.anim.SetBool(states.hashes.isClimbing, false);
            }
        }
        Quaternion prevRotation;
        void Rotate(StateManager sm)
        {
            if (cameraTransform.value == null)
                return;
           //keep track fo previous rotation in case we need to revert back to it, in the the case of detecting an unclimbable surface
            prevRotation = states.mTransform.rotation; 
            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;

            //Get the angle between the camera's forward and the climb's up and get it in 360 degrees
            float angle = -Vector3.Angle(climbUp, cameraTransform.value.forward);
            angle = (Vector3.Angle(climbRight, cameraTransform.value.forward) > 90f) ? 360f - angle : angle;

            //rotating climb up and climb right based on camera's position
            Vector3 climbForwardAlt = (Quaternion.AngleAxis(angle, climbUp) * -climbForward);
            Vector3 climbRightAlt = (Quaternion.AngleAxis(angle, climbUp) * -climbRight);
            if (climbForwardAlt == Vector3.zero)
                climbForwardAlt = cameraTransform.value.forward;
            if(climbRightAlt == Vector3.zero)
                climbRightAlt = cameraTransform.value.right;

            Vector3 targetDir = climbForwardAlt * v;
            targetDir += (climbRightAlt * h);
            targetDir.Normalize();

            //Debug purposes to visualize the direction
            Debug.DrawRay(states.transform.position, climbForwardAlt * 3, Color.red);
            Debug.DrawRay(states.transform.position, climbRightAlt * 3, Color.yellow);

            //If there's no input, then don't do anything
            if (targetDir == Vector3.zero)
                return;

            //Apply rotation
            states.movementVariables.moveDirection = targetDir;

            Quaternion tr = Quaternion.LookRotation(targetDir, states.transform.up);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * rotationSpeed);

            states.mTransform.rotation = targetRotation;
        }

        void RotateTowardsClimb()
        {

            Vector3 center = states.transform.position;
            center += states.transform.forward + (states.transform.up * 0.1f);

            // Dir represents the downward direction
            Vector3 dir = -states.transform.up * 0.5f;

            // Draw the rays
            //Debug.DrawRay(center, dir * 5f, Color.red);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(center, dir, out hit, 5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.mTransform.rotation = Quaternion.Slerp(states.mTransform.rotation, Quaternion.FromToRotation(states.transform.up, hit.normal) * states.transform.rotation, t * rotationSpeed * (states.dashActive ? 4f : 1f));
                Vector3 v = Vector3.Cross(hit.normal, Vector3.up);
                climbForward = Vector3.Cross(v, hit.normal);
                climbRight = Vector3.Cross(hit.normal, Vector3.up);
                climbUp = hit.normal;
            }
        }

        void Move(StateManager sm)
        {
            Vector3 testOrigin = states.mTransform.position + (states.mTransform.forward * .75f);
            testOrigin.y += .5f;
            //Debug.DrawRay(origin2, -Vector3.up, Color.red);
            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * climbSpeed * states.climbSpeedMult;
            if(!ignoreGravity)
                targetVelocity -= under.normal.normalized * targetVelocity.magnitude * 0.5f;
            //if (transitioning)
            //    targetVelocity = Quaternion.Inverse(targetRot) * targetVelocity;
            Vector3 currentVelocity = states.rigid.velocity;
            states.targetVelocity = targetVelocity;
            states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * climbSpeed);
        }

        bool inPos;
        bool inRot;
        float t;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float offsetFromWall = 0.3f;
        float delta;
        void Transfer(StateManager sm)
        {
            delta = Time.deltaTime * 4;

            if (inPos && inRot)
            {
                //Debug.Log(Time.frameCount + " || I am in rotation");
                transitioning = false;
                ignoreGravity = false;
                //Vector3 tp = Vector3.Lerp(startPos, targetPos, 0.99f);
                states.transform.position = targetPos;
                CheckUnder();
            }

            if (!inRot)
            {
                if (states.transform.rotation == targetRot)
                {
                    inRot = true;
                    //states.rigid.velocity += states.transform.forward;
                }
                else
                {
                    Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, targetRot, t);
                    if (t >= 1)
                        targetRotation = targetRot;
                    states.mTransform.rotation = targetRotation;
                }
                    //Debug.Log(Time.frameCount + " || still rotating: " + inRot);
            }

            if (!inPos)
            {
                t += delta * (dashActivated ? 4 : states.isRun ? states.climbSpeedMult : 1);
                Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
                if (t >= 1)
                    tp = targetPos;
                states.transform.position = tp;
            }
            if (Vector3.Distance(states.transform.position, targetPos) <= offsetFromWall)
            {
                inPos = true;
            }
            else
            {
                inPos = false;
            }
            //Debug.Log(Time.frameCount + " || inPos = " + inPos + " inRot = " + inRot);
        }

        void CheckUnder()
        {
            Vector3 underOrigin = states.transform.position;
            underOrigin += states.transform.forward + states.transform.forward / 2.1f + (states.transform.up * 0.2f);
            // Dir represents the downward direction
            Vector3 dir = -states.transform.forward + (-states.transform.up * 0.5f);
            if (Physics.Raycast(underOrigin, dir, out under, 1.6f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.climbHit = under;
                prevAngle = 0;
            }
            //prevAngle = Vector3.Angle(under.normal, states.climbHit.normal);
            Vector3 v = Vector3.Cross(states.climbHit.normal, Vector3.up);
            climbForward = Vector3.Cross(v, states.climbHit.normal);
            climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
            climbUp = states.climbHit.normal;
        }

        public override void OnFixed(StateManager sm)
        {
            base.OnFixed(states);

            if (moveCamera)
            {
                if (camera != null && tempAngle < 180 && tempAngle > -180)
                {
                    camera.AddToYaw((cameraAngle * Time.deltaTime * 4));
                    tempAngle -= (cameraAngle * Time.deltaTime * 4);
                }
                else
                {
                    moveCamera = false;
                    if (camera != null)
                        camera.ignoreInput = false;
                }
            }
        }

        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);
            //states.rigid.velocity = Vector3.zero;
            if (states.dashActive)
            {
                states.dashActive = false;
                states.lagDashCooldown = 1.0f;
            }
            moveCamera = false;
            if (camera != null)
                camera.ignoreInput = false;
        }
    }
}
