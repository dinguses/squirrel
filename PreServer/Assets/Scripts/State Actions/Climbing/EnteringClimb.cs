using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Entering Climb")]
    public class EnteringClimb : StateActions
    {
        bool inPos;
        bool inRot;
        float t;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        float delta;
        bool moveCamera = false;
        public TransformVariable cameraTransform;
        CameraManager camera;
        float cameraAngle = 0;
        float tempAngle = 0;
        PlayerManager states;
        public bool debug = false;
        float angle = 0;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            //Debug.Log("Entering climb");
            if (camera == null && cameraTransform != null)
            {
                camera = cameraTransform.value.GetComponent<CameraManager>();
            }
            base.OnEnter(states);
            states.rigid.useGravity = false;
            startPos = states.transform.position;
            states.pauseSpeedHackTimer = false;
            //startRot = states.transform.rotation;
            targetPos = states.climbHit.point + (states.climbHit.normal * states.climbingVariables.offsetFromWall);
            targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
            t = 0;
            inPos = true;
            inRot = false;
            tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, states.climbHit.normal, Vector3.up);
            if (tempAngle < 90 && tempAngle > -90)
            {
                cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                //moveCamera = true;
                if (camera != null)
                    camera.ignoreInput = true;
            }
            else
            {
                moveCamera = false;
            }
            SafeClimb();
        }

        void SafeClimb()
        {
            
            //This is under the assumption that the climbhit will never be inside a mesh collider
            RaycastHit hit = new RaycastHit();

            Vector3 originalDirection = targetRot * Vector3.forward.normalized;
            Vector3 newDirection = originalDirection;
            bool backBlocked = false;

            //Check to see the position we will end up at is colliding with the length of the squirrel's collider
            //if there is any object in the way, then move the target position backwards
            while (Physics.SphereCast(targetPos + states.climbHit.normal * 0.45f, 0.375f, originalDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                backBlocked = CheckBackBlocked(originalDirection, hit);
                if (backBlocked)
                    break;
            }

            bool climbHit = true;
            if (!backBlocked)
            {
                climbHit = CheckClimbHit(targetPos + (states.climbHit.normal * 0.1f) + (originalDirection * 1.5f), hit);
                while (!climbHit)
                {
                    backBlocked = CheckBackBlocked(originalDirection, hit);
                    if (backBlocked)
                        break;
                    climbHit = CheckClimbHit(targetPos + (states.climbHit.normal * 0.1f) + (originalDirection * 1.5f), hit);
                }
            }

            angle = 0;
            newDirection = originalDirection;
            float angleDirection = 1f;

            //We've moved back as far as we can, if we're still hitting anything, then we'll rotating out of it should fix it
            while (Physics.SphereCast(targetPos + states.climbHit.normal * 0.45f, 0.375f, newDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                CheckAngle(ref angle, ref angleDirection);
                angle += angleDirection;
                newDirection = Quaternion.AngleAxis(angle, states.climbHit.normal) * originalDirection.normalized;
                //if we've done a full rotation and we still can't get out, then break out of the function otherwise we'll be in an endless loop
                //If we ever have to break out, then this is most probably an issue with the level design
                if (angle <= -180)
                    break;
            }

            if (angle > -180f)
            {
                climbHit = CheckClimbHit(targetPos + (states.climbHit.normal * 0.1f) + (newDirection * 1.5f), hit);
                while (!climbHit)
                {

                    CheckAngle(ref angle, ref angleDirection);
                    angle += angleDirection;
                    newDirection = Quaternion.AngleAxis(angle, states.climbHit.normal) * originalDirection.normalized;
                    if (angle <= -180)
                        break;
                    climbHit = CheckClimbHit(targetPos + (states.climbHit.normal * 0.1f) + (newDirection * 1.5f), hit);
                }
            }
            if (angle != 0)
                targetRot = Quaternion.AngleAxis(angle, states.climbHit.normal) * targetRot;
            //Debug.DrawRay(targetPos, states.climbHit.normal * 2f, Color.red);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, targetRot * temp.normalized * 2, Color.yellow);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, temp2 * 2, Color.cyan);
        }

        bool CheckClimbHit(Vector3 origin, RaycastHit hit)
        {
            if (Physics.Raycast(origin, -states.climbHit.normal, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag == "Climb")
                {
                    return true;
                }
            }
            return false;
        }

        void CheckAngle(ref float angle, ref float angleDirection)
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
        }

        bool CheckBackBlocked(Vector3 dir, RaycastHit hit)
        {
            //If there is an object that will block us from moving back, then move back only a little bit then break out of here
            if (Physics.SphereCast(targetPos + states.climbHit.normal * 0.45f, 0.375f, -dir, out hit, 0.2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                //targetPos -= (originalDirection.normalized * (hit.distance * 0.5f));
                return true;
            }
            targetPos -= (dir.normalized * 0.2f);
            return false;
        }

        public override void OnFixed(StateManager sm)
        {
            base.OnFixed(states);

            //if (moveCamera)
            //{
            //    if (camera != null && tempAngle < 180 && tempAngle > -180)
            //    {
            //        camera.AddToYaw((cameraAngle * Time.deltaTime * 4));
            //        tempAngle -= (cameraAngle * Time.deltaTime * 4);
            //    }
            //    else
            //    {
            //        moveCamera = false;
            //        if (camera != null)
            //            camera.ignoreInput = false;
            //    }
            //}
        }

        public override void Execute(StateManager sm)
        {
            
        }
        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);
            delta = Time.deltaTime * 4;
            if (!inPos)
            {
                t += delta;
                Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
                if (t >= 1)
                    tp = targetPos;
                states.transform.position = tp;
            }
            if (Vector3.Distance(states.transform.position, targetPos) <= states.climbingVariables.offsetFromWall)
            {
                if(!debug)
                    inPos = true;
            }
            else
            {
                inPos = false;
            }

            if (!inRot)
            {
                if (states.transform.rotation == targetRot)
                {
                    inRot = true;
                }
                else
                {
                    inRot = false;
                    Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, targetRot, t);
                    if (t >= 1)
                        targetRotation = targetRot;
                    states.mTransform.rotation = targetRotation;
                }
            }

            if(inPos && inRot && !moveCamera)
            {
                states.climbState = PlayerManager.ClimbState.CLIMBING;
            }
            //Debug.DrawRay(targetPos, states.climbHit.normal * 2f, Color.red);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, originalDirection * 2, Color.yellow);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, newDirection * 2, Color.green);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, Quaternion.AngleAxis(angle, states.climbHit.normal) * originalDirection.normalized * 2f, Color.blue);
        }

        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * states.climbingVariables.offsetFromWall;
            return target + offset;
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);
            states.rigid.velocity = Vector3.zero;
            moveCamera = false;
            if (camera != null)
                camera.ignoreInput = false;
        }
    }
}
