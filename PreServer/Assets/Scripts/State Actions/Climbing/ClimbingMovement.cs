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
        bool moveCamera = false;
        CameraManager camera;
        float cameraAngle = 0;
        float tempAngle = 0;
        PlayerManager states;
        float prevAngle = 0;
        public float minTransferAngle = 30f;
        bool ignoreGravity = false;
        public float angle = 0;
        Vector3 newDirection;
        Vector3 originalDirection;
        public bool debug = false;
        Vector3 prevNormal;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;

            base.OnEnter(states);
            if(camera == null)
            {
                camera = Camera.main.transform.parent.GetComponent<CameraManager>();
            }

            front = new RaycastHit();
            under = new RaycastHit();
            transitioning = false;
            states.isGrounded = false;
            states.dashInAirCounter = 0;
            moveCamera = false;
            if (camera != null)
                camera.ignoreInput = false;
            states.rigid.drag = 0;
            states.playerMesh.gameObject.SetActive(true);
            CheckUnder();
            cameraAngle = 0;
            prevNormal = states.climbHit.normal;
        }
        
        void SafeClimb()
        {
            originalDirection = targetRot * Vector3.forward.normalized;
            newDirection = originalDirection;
            //This is under the assumption that the climbhit will never be inside a mesh collider
            RaycastHit hit = new RaycastHit();
            //Check to see the position we will end up at is colliding with the length of the squirrel's collider
            //if there is any object in the way, then move the target position backwards
            while (Physics.Raycast(targetPos + states.climbHit.normal * 0.1f, originalDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                //If there is an object that will block us from moving back, then move back only a little bit then break out of here
                if (Physics.Raycast(targetPos + states.climbHit.normal * 0.1f, -originalDirection, out hit, 0.2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    targetPos -= (originalDirection.normalized * (hit.distance * 0.5f));
                    break;
                }
                targetPos -= (originalDirection.normalized * 0.2f);
            }

            angle = 0;
            newDirection = originalDirection;
            float angleDirection = 1f;

            //We've moved back as far as we can, if we're still hitting anything, then we'll rotating out of it should fix it
            while (Physics.Raycast(targetPos + states.climbHit.normal * 0.1f, newDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                switch (angle)
                {
                    case 0:
                        angleDirection = 1;
                        break;
                    case 45:
                        angleDirection = -1;
                        angle = 0;
                        break;
                    case -45:
                        angleDirection = 1;
                        angle = 45;
                        break;
                    case 90:
                        angleDirection = -1;
                        angle = -45;
                        break;
                    case -90:
                        angleDirection = 1;
                        angle = 90;
                        break;
                    case 135:
                        angleDirection = -1;
                        angle = -90;
                        break;
                    case -135:
                        angleDirection = 1;
                        angle = 135;
                        break;
                    case 180:
                        angleDirection = -1;
                        angle = -135;
                        break;
                }

                angle += angleDirection;
                newDirection = Quaternion.AngleAxis(angle, states.climbHit.normal) * originalDirection.normalized;
                //if we've done a full rotation and we still can't get out, then break out of the function otherwise we'll be in an endless loop
                //If we ever have to break out, then this is most probably an issue with the level design
                if (angle <= -180)
                    break;
            }
            //if (angle == 0)
            //{
            //    //Debug.LogError("Mission failed we'll get 'em next time");
            //}
            //Debug.LogError(angle);
            //angle = angle % 360;
            if (angle != 0)
                targetRot = Quaternion.AngleAxis(angle, states.climbHit.normal) * targetRot;
            //Debug.DrawRay(targetPos, states.climbHit.normal * 2f, Color.red);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, targetRot * temp.normalized * 2, Color.yellow);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, temp2 * 2, Color.cyan);
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
                //Move(states);
                //Debug.DrawRay(targetPos, states.climbHit.normal * 2f, Color.red);
                //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, originalDirection * 2, Color.yellow);
                //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, newDirection * 2, Color.green);
                //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, Quaternion.AngleAxis(angle, states.climbHit.normal) * originalDirection.normalized * 2f, Color.blue);
            }
            else
            {
                RotateTowardsClimb();
                Rotate(states);
                Move(states);
                CheckRaycast(states);
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

                    //SetCameraAngle(states.climbHit.normal, -front.normal);

                    states.climbHit = front;
                    startPos = states.transform.position;
                    targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                    targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                    t = 0;
                    inPos = false;
                    inRot = false;

                    transitioning = true;
                    states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                    SafeClimb();
                    return;
                }
                else
                {
                    if (angle < 70)
                    {
                        //Debug.LogError("Front has detected non-climbable surface, exit the climb");
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
            underOrigin += states.transform.forward + states.transform.forward / 1.7f + (states.transform.up * 0.5f);

            // Dir represents the downward direction
            Vector3 dir = -states.transform.forward + (-states.transform.up);

            // Draw the rays
            Debug.DrawRay(underOrigin, dir * 1.5f, Color.green);
            Debug.DrawRay(underOrigin, Quaternion.AngleAxis(30f, states.climbHit.normal) * dir * 1.5f, Color.red);
            Debug.DrawRay(underOrigin, Quaternion.AngleAxis(-30f, states.climbHit.normal) * dir * 1.5f, Color.blue);
            if (Physics.Raycast(underOrigin, dir, out under, 1.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                //underside has hit something
                underHit = true;
                float angle = Vector3.Angle(under.normal, Vector3.up);
                //If I hit another climb, check if I should transfer to it
                if(under.transform.tag == "Climb")
                {
                    angle = Vector3.Angle(under.normal, states.climbHit.normal);
                    prevNormal = states.climbHit.normal;
                    states.climbHit = under;
                    //if(Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                        //Debug.Log(Mathf.Abs(angle - prevAngle));
                    //if the angle between the new climb and the current one is greater than the transfer amount, then transfer to it
                    if (Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                    {
                        ignoreGravity = true;
                        prevAngle = angle;
                        //Debug.Log("Under has detected a new climb to transition to");
                        states.rigid.velocity = Vector3.zero;

                        ////Update variables for transfer
                        startPos = states.transform.position;
                        targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                        targetRot = Quaternion.FromToRotation(prevNormal, states.climbHit.normal) * states.transform.rotation;
                        t = 0;
                        inPos = false;
                        inRot = false;
                        transitioning = true;
                        states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                        SafeClimb();
                    }
                    SetCameraAngle(prevNormal, under.normal);
                }
                //I didn't hit a climb but I must do something based on the angle of the hit
                else
                {
                    //detach
                    if (angle < 70)
                    {
                        //Debug.LogError("Under has detected non-climbable surface, exit the climb");
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
                //TODO: Refactor later, all transfer climbs do the same code, it will be more efficient to make a function
                if (Physics.Raycast(underOrigin, Quaternion.AngleAxis(30f, states.climbHit.normal) * dir, out under, 1.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    if (under.transform.tag == "Climb")
                    {
                        angle = Vector3.Angle(under.normal, states.climbHit.normal);
                        prevNormal = states.climbHit.normal;
                        states.climbHit = under;
                        //if(Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                        //Debug.Log(Mathf.Abs(angle - prevAngle));
                        //if the angle between the new climb and the current one is greater than the transfer amount, then transfer to it
                        if (Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                        {
                            ignoreGravity = true;
                            prevAngle = angle;
                            //Debug.Log("Under has detected a new climb to transition to");
                            states.rigid.velocity = Vector3.zero;


                            ////Update variables for transfer
                            startPos = states.transform.position;
                            targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                            targetRot = Quaternion.FromToRotation(prevNormal, states.climbHit.normal) * states.transform.rotation;
                            t = 0;
                            inPos = false;
                            inRot = false;
                            transitioning = true;
                            states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                            SafeClimb();
                        }
                        SetCameraAngle(prevNormal, under.normal);
                    }
                }
                else if (Physics.Raycast(underOrigin, Quaternion.AngleAxis(-30f, states.climbHit.normal) * dir, out under, 1.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    if (under.transform.tag == "Climb")
                    {
                        angle = Vector3.Angle(under.normal, states.climbHit.normal);
                        prevNormal = states.climbHit.normal;
                        states.climbHit = under;
                        //if(Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                        //Debug.Log(Mathf.Abs(angle - prevAngle));
                        //if the angle between the new climb and the current one is greater than the transfer amount, then transfer to it
                        if (Mathf.Abs(angle - prevAngle) >= minTransferAngle)
                        {
                            ignoreGravity = true;
                            prevAngle = angle;
                            //Debug.Log("Under has detected a new climb to transition to");
                            states.rigid.velocity = Vector3.zero;

                            //Camera rotations, checking if the camera needs to be repositioned based on where it is relative to the climb
                            //angle = Vector3.SignedAngle(prevNormal, under.normal, Vector3.up);
                            //if (angle != 0 && camera != null)
                            //{
                            //    Debug.LogError("Adding amount: " + angle);
                            //    //camera.debugAmount += angle;
                            //}
                            //tempAngle = Vector3.SignedAngle(prevNormal, under.normal, Vector3.up);
                            //if (tempAngle < 90 && tempAngle > -90)
                            //{
                            //cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                            //cameraAngle = 0;
                            //if (camera != null)
                            //    camera.ignoreInput = true;
                            //}

                            ////Update variables for transfer
                            startPos = states.transform.position;
                            targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                            targetRot = Quaternion.FromToRotation(prevNormal, states.climbHit.normal) * states.transform.rotation;
                            t = 0;
                            inPos = false;
                            inRot = false;
                            transitioning = true;
                            states.anim.CrossFade(states.hashes.squ_climb_corner, 0.2f);
                            SafeClimb();
                        }
                        SetCameraAngle(prevNormal, under.normal);
                    }
                }
                else
                {
                    underHit = false;
                    states.rigid.velocity = Vector3.zero;
                    //Debug.LogError("Under isn't hitting anything");
                    states.climbState = PlayerManager.ClimbState.NONE;
                    states.isJumping = true;
                    states.anim.SetBool(states.hashes.isClimbing, false);
                }

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
            //Debug.DrawRay(states.transform.position, climbForwardAlt * 3, Color.red);
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

        void RotateTowardsClimb()
        {

            Vector3 center = states.transform.position;
            center += states.transform.forward + (states.transform.up * 0.2f);

            // Dir represents the downward direction
            Vector3 dir = -states.transform.up * 0.5f;

            // Draw the rays
            //Debug.DrawRay(center, dir * 5f, Color.red);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(center, dir, out hit, 5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.mTransform.rotation = Quaternion.Slerp(states.mTransform.rotation, Quaternion.FromToRotation(states.transform.up, hit.normal) * states.transform.rotation, Time.unscaledDeltaTime * rotationSpeed);
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
                targetVelocity -= climbUp.normalized * targetVelocity.magnitude * 0.5f;
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
                t += delta * (states.isRun ? states.climbSpeedMult : 1);
                Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
                if (t >= 1)
                    tp = targetPos;
                states.transform.position = tp;
            }
            if (Vector3.Distance(states.transform.position, targetPos) <= offsetFromWall)
            {
                if (!debug)
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
            underOrigin += states.transform.forward + states.transform.forward / 1.7f + (states.transform.up * 0.5f);
            // Dir represents the downward direction
            Vector3 dir = -states.transform.forward + (-states.transform.up);
            if (Physics.Raycast(underOrigin, dir, out under, 1.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
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

            if (camera != null && Mathf.Abs(cameraAngle) > 0)
            {
                float angle = 0;
                if (Mathf.Abs(cameraAngle) >= 1.5f)
                {
                    angle = 1.5f;
                }
                else if (Mathf.Abs(cameraAngle) >= 0.75f)
                {
                    angle = 0.75f;
                }
                else
                {
                    angle = Mathf.Abs(cameraAngle);
                }
                camera.AddToYaw(angle * (cameraAngle > 0 ? 1 : -1));
                cameraAngle -= angle * (cameraAngle > 0 ? 1 : -1);
            }
        }

        void SetCameraAngle(Vector3 prev, Vector3 curr)
        {
            //Camera rotations, checking if the camera needs to be repositioned based on where it is relative to the climb
            angle = Vector3.SignedAngle(prev, curr, Vector3.up);
            if (angle != 0 && camera != null)
            {
                Vector3 cameraForward = camera.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                float a = Vector3.SignedAngle(-cameraForward, curr, Vector3.up);
                //angle += (angle * 0.1f);
                if(Mathf.Abs(a) > 45f)
                {
                    if(Mathf.Abs(a) > Mathf.Abs(cameraAngle))
                        cameraAngle += a;
                    else
                        cameraAngle += angle;
                    Debug.LogError("Adding angle: " + angle + " angle between camera: " + a);
                }
            }
        }

        void LogCameraAngle()
        {
            if(camera != null)
            {
                Vector3 cameraForward = camera.transform.forward;
                cameraForward.y = 0;
                Debug.Log(Vector3.SignedAngle(-cameraForward.normalized, states.climbHit.normal, Vector3.up));
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
            moveCamera = false;
            if (camera != null)
                camera.ignoreInput = false;
        }
    }
}
