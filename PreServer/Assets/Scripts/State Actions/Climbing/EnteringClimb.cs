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
        public float offsetFromWall = 0.3f;
        float delta;
        bool moveCamera = false;
        public TransformVariable cameraTransform;
        CameraManager camera;
        float cameraAngle = 0;
        float tempAngle = 0;
        public override void OnEnter(StateManager states)
        {
            if (camera == null && cameraTransform != null)
            {
                camera = cameraTransform.value.GetComponent<CameraManager>();
            }
            base.OnEnter(states);
            states.rigid.useGravity = false;
            startPos = states.transform.position;
            targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
            targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
            t = 0;
            inPos = false;
            inRot = false;
            tempAngle = Vector3.SignedAngle(cameraTransform.value.forward, states.climbHit.normal, Vector3.up);
            if (tempAngle < 90 && tempAngle > -90)
            {
                cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                moveCamera = true;
                if (camera != null)
                    camera.ignoreInput = true;
            }
            else
            {
                moveCamera = false;
            }
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

        public override void Execute(StateManager states)
        {
            
        }

        public override void OnUpdate(StateManager states)
        {
            base.OnUpdate(states);
            delta = Time.deltaTime * 4;
            if (!inPos)
            {
                t += delta;
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

            if (!inRot)
            {
                if (states.transform.rotation == targetRot)
                    inRot = true;
                Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, targetRot, t);
                states.mTransform.rotation = targetRotation;
            }

            if(inPos && inRot && !moveCamera)
            {
                states.climbState = StateManager.ClimbState.CLIMBING;
            }
            //Debug.Log(Time.frameCount + " || inPos = " + inPos + " inRot = " + inRot);
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
            states.rigid.velocity = Vector3.zero;
            moveCamera = false;
            if (camera != null)
                camera.ignoreInput = false;
        }
    }
}
