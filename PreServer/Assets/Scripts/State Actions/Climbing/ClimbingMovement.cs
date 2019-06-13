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
        public override void OnEnter(StateManager states)
        {
            base.OnEnter(states);
            if(camera == null && cameraTransform != null)
            {
                camera = cameraTransform.value.GetComponent<CameraManager>();
            }
            Vector3 v = Vector3.Cross(states.climbHit.normal, Vector3.up);
            climbForward = Vector3.Cross(v, states.climbHit.normal);
            climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
            climbUp = states.climbHit.normal;
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
        }

        public override void Execute(StateManager states)
        {

        }

        public override void OnUpdate(StateManager states)
        {
            base.OnUpdate(states);

            if (transitioning)
            {
                Transfer(states);
                //Rotate(states);
                Move(states);
            }
            else
            {
                if (timer <= 0 && states.dashActive && states.CanDash() && !dashActivated)
                {
                    states.anim.CrossFade(states.hashes.squ_dash, 0.01f);

                    //Debug.Log("Adding velocity 9");
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
                //Debug.Log("Dash over");
                states.lagDashCooldown = 1.0f;
                dashActivated = false;
            }
            Debug.DrawRay(states.climbHit.point, states.climbHit.normal * 3f, Color.yellow);
        }

        void CheckRaycast(StateManager states)
        {
            bool frontHit = false, underHit = false;
            float topFloat = .6f;
            Vector3 topRay = states.transform.position + (states.transform.forward * 1.5f) + (states.transform.up * topFloat);

            Debug.DrawRay(topRay, states.transform.forward * 0.75f, Color.blue);
            //Raycast in front of the squirrel, used to check if we've hit a ceiling, ground, or another climb-able surface
            if (Physics.Raycast(topRay, states.transform.forward, out front, 0.75f, Layers.ignoreLayersController))
            {
                frontHit = true;
                float angle = Vector3.Angle(front.normal, Vector3.up);
                //can climb if the angle is between 70 and 90, and it is a different surface, might need to adjust
                if (angle >= 70 && angle <= 90 && front.transform.tag == "Climb" && (front.transform != states.climbHit.transform || front.normal != states.climbHit.normal))
                {
                    angle = Vector3.Angle(front.normal, states.climbHit.normal);
                    bool angleOver = angle > 45;
                    if (angleOver)
                    {
                        states.rigid.velocity = Vector3.zero;
                        moveCamera = true;
                        if (camera != null)
                            camera.ignoreInput = true;
                        //camera angle is the amount the camera needs to move, tempAngle is the starting point
                        tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, front.normal, Vector3.up);
                        cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                        if (states.dashActive)
                        {
                            states.dashActive = false;
                            states.lagDashCooldown = 1.0f;
                            dashActivated = false;
                        }
                    }
                    else
                    {
                        tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, front.normal, Vector3.up);
                        if (tempAngle < 90 && tempAngle > -90)
                        {
                            cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                            moveCamera = true;
                            if (camera != null)
                                camera.ignoreInput = true;
                        }
                    }
                    states.climbHit = front;
                    startPos = states.transform.position;
                    targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                    targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                    t = 0;
                    inPos = false;
                    inRot = false;
                    Vector3 v = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    climbForward = Vector3.Cross(v, states.climbHit.normal);
                    climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    climbUp = states.climbHit.normal;
                    transitioning = true;
                    return;
                }
                else if (angle < 70)
                {
                    states.climbHit = front;
                    states.climbState = StateManager.ClimbState.EXITING;
                    return;
                }
                else if (angle > 90 || front.transform.tag != "Climb")
                {
                    states.rigid.velocity = Vector3.zero;
                    //states.mTransform.rotation = prevRotation;
                }
            }

            // Setup origin points for three different ground checking vector3s. One in middle of player, one in front, and one in back
            Vector3 frontOrigin = states.transform.position;
            frontOrigin += states.transform.forward + states.transform.forward / 2;
            //backOrigin += states.mTransform.forward / 2;

            // Origins should be coming from inside of player

            // Dir represents the downward direction
            Vector3 dir = -states.transform.forward + (-states.transform.up * 0.5f);

            // Draw the rays
            Debug.DrawRay(frontOrigin, dir * 1.5f, Color.green);
            if (Physics.Raycast(frontOrigin, dir, out under, 1.5f, Layers.ignoreLayersController))
            {
                underHit = true;
                float angle = Vector3.Angle(under.normal, Vector3.up);
                if (angle >= 70 && angle <= 90 && under.transform.tag == "Climb" && (under.transform != states.climbHit.transform || under.normal != states.climbHit.normal))
                {
                    angle = Vector3.Angle(under.normal, states.climbHit.normal);
                    bool angleOver = angle > 45;
                    if (angleOver)
                    {
                        states.rigid.velocity = Vector3.zero;
                        moveCamera = true;
                        if (camera != null)
                            camera.ignoreInput = true;
                        //camera angle is the amount the camera needs to move, tempAngle is the starting point
                        tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, under.normal, Vector3.up);
                        cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                        if (states.dashActive)
                        {
                            states.dashActive = false;
                            states.lagDashCooldown = 1.0f;
                            dashActivated = false;
                        }
                    }
                    else
                    {
                        tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, under.normal, Vector3.up);
                        if (tempAngle < 90 && tempAngle > -90)
                        {
                            cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                            moveCamera = true;
                            if (camera != null)
                                camera.ignoreInput = true;
                        }
                    }
                    states.climbHit = under;
                    startPos = states.transform.position;
                    targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                    targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                    t = 0;
                    inPos = false;
                    inRot = false;
                    Vector3 v = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    climbForward = Vector3.Cross(v, states.climbHit.normal);
                    climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    climbUp = states.climbHit.normal;
                    transitioning = true;
                    return;
                }
                else if (angle < 70)
                {
                    states.climbHit = under;
                    states.climbState = StateManager.ClimbState.EXITING;

                    states.anim.SetBool(states.hashes.isClimbing, false);

                    return;
                }
                else if (angle > 90)
                {
                    //states.rigid.velocity = Vector3.zero;
                    //states.mTransform.rotation = prevRotation;
                    states.climbState = StateManager.ClimbState.NONE;
                    states.isJumping = true;
                    return;
                }
                else if (under.transform.tag != "Climb")
                {
                    states.rigid.velocity = Vector3.zero;
                    states.mTransform.rotation = prevRotation;
                    return;
                }
            }
            else
            {
                //states.rigid.velocity = Vector3.zero;
                //states.mTransform.rotation = prevRotation;
                states.climbState = StateManager.ClimbState.NONE;
                states.isJumping = true;
            }
            if (!underHit && !frontHit)
            {
                states.climbState = StateManager.ClimbState.NONE;
                states.isJumping = true;
            }
        }
        Quaternion prevRotation;
        void Rotate(StateManager states)
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

            Vector3 targetDir = climbForwardAlt * v;
            targetDir += (climbRightAlt * h);
            targetDir.Normalize();

            //Debug purposes to visualize the direction
            //Debug.DrawRay(states.transform.position, climbForwardAlt * 3, Color.blue);
            //Debug.DrawRay(states.transform.position, climbRightAlt * 3, Color.yellow);

            //If there's no input, then don't do anything
            if (targetDir == Vector3.zero)
                return;

            //Apply rotation
            states.movementVariables.moveDirection = targetDir;

            Quaternion tr = Quaternion.LookRotation(targetDir, states.transform.up);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * rotationSpeed);

            states.mTransform.rotation = targetRotation;
        }

        void Move(StateManager states)
        {
            Vector3 testOrigin = states.mTransform.position + (states.mTransform.forward * .75f);
            testOrigin.y += .5f;
            //Debug.DrawRay(origin2, -Vector3.up, Color.red);
            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * climbSpeed * states.climbSpeedMult;
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
        void Transfer(StateManager states)
        {
            delta = Time.deltaTime * 4;

            if (inPos && inRot)
            {
                //Debug.Log(Time.frameCount + " || I am in rotation");
                transitioning = false;
                //Vector3 tp = Vector3.Lerp(startPos, targetPos, 0.99f);
                states.transform.position = targetPos;
            }

            if (!inRot)
            {
                if (states.transform.rotation == targetRot)
                {
                    inRot = true;
                    states.rigid.velocity += states.transform.forward;
                }
                Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, targetRot, t);
                states.mTransform.rotation = targetRotation;
                //Debug.Log(Time.frameCount + " || still rotating: " + inRot);
            }

            if (!inPos)
            {
                t += delta * (dashActivated ? 4 : states.isRun ? states.climbSpeedMult : 1);
                Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
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

        public override void OnFixed(StateManager states)
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

        public override void OnExit(StateManager states)
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
