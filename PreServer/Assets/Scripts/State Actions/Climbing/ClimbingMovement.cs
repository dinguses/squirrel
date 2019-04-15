using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Climbing Movement")]
    public class ClimbingMovement : StateActions
    {
        public float climbSpeed = 2f;
        public float rotationSpeed = 3f;
        public TransformVariable cameraTransform;
        Vector3 climbRight;
        Vector3 climbUp;
        bool transitioning = false;
        RaycastHit front;
        RaycastHit under;
        public override void OnEnter(StateManager states)
        {
            base.OnEnter(states);
            Vector3 forward = Vector3.Cross(states.climbHit.normal, Vector3.up);
            climbUp = Vector3.Cross(forward, states.climbHit.normal);
            climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
            front = new RaycastHit();
            under = new RaycastHit();
            transitioning = false;
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
            }
            else
            {
                Rotate(states);
                Move(states);
                CheckRaycast(states);
            }
        }

        void CheckRaycast(StateManager states)
        {
            float topFloat = .6f;
            Vector3 topRay = states.transform.position + (states.transform.forward * 1.5f) + (states.transform.up * topFloat);

            Debug.DrawRay(topRay, states.transform.forward * 0.75f, Color.blue);

            if (Physics.Raycast(topRay, states.transform.forward, out front, 0.75f, Layers.ignoreLayersController))
            {
                float angle = Vector3.Angle(front.normal, Vector3.up);
                if (angle >= 70 && angle <= 90 && (front.transform != states.climbHit.transform || front.normal != states.climbHit.normal))
                {
                    states.climbHit = front;
                    transitioning = true;
                    startPos = states.transform.position;
                    targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                    targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                    t = 0;
                    inPos = false;
                    inRot = false;
                    Vector3 forward = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    climbUp = Vector3.Cross(forward, states.climbHit.normal);
                    climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    states.rigid.velocity = Vector3.zero;
                    return;
                }
                else if (angle < 70)
                {
                    states.climbHit = front;
                    states.climbState = StateManager.ClimbState.EXITING;
                    return;
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
            Debug.DrawRay(frontOrigin, dir * 1f, Color.green);
            if (Physics.Raycast(frontOrigin, dir, out under, 1f, Layers.ignoreLayersController))
            {
                float angle = Vector3.Angle(under.normal, Vector3.up);
                if (angle >= 70 && angle <= 90 && (under.transform != states.climbHit.transform || under.normal != states.climbHit.normal))
                {
                    states.climbHit = under;
                    transitioning = true;
                    startPos = states.transform.position;
                    targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
                    targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
                    t = 0;
                    inPos = false;
                    inRot = false;
                    Vector3 forward = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    climbUp = Vector3.Cross(forward, states.climbHit.normal);
                    climbRight = Vector3.Cross(states.climbHit.normal, Vector3.up);
                    states.rigid.velocity = Vector3.zero;
                    return;
                }
                else if(angle < 70)
                {
                    states.climbHit = under;
                    states.climbState = StateManager.ClimbState.EXITING;
                    return;
                }
            }
            Debug.DrawRay(targetPos, states.transform.up * 3f, Color.yellow);
        }

        void Rotate(StateManager states)
        {
            if (cameraTransform.value == null)
                return;

            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;


            //var test = cameraTransform.value;
            //Vector3 cameraStuff = test.right;
            //cameraStuff.Normalize();
            ////Debug.Log(Time.frameCount + " || angle: " + Vector3.Angle(cameraStuff, climbRight));

            //if (Vector3.Angle(cameraStuff, climbRight) > 90)
            //{
            //    h = -h;
            //}

            //float m = Mathf.Abs(h) + Mathf.Abs(v);
            //Debug.Log(Time.frameCount + " || h: " + h + " v: " + v);
            Vector3 targetDir = climbUp * v;
            targetDir += climbRight * h;
            targetDir.Normalize();
            //targetDir.x = 0;

            if (targetDir == Vector3.zero)
                return;

            states.movementVariables.moveDirection = targetDir;

            //targetDir.x = states.mTransform.forward.x;
            //Debug.DrawRay(states.mTransform.position, targetDir, Color.red);
            Quaternion tr = Quaternion.LookRotation(targetDir, states.transform.up);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * states.movementVariables.moveAmount * rotationSpeed);

            states.mTransform.rotation = targetRotation;
        }

        void Move(StateManager states)
        {
            Vector3 testOrigin = states.mTransform.position + (states.mTransform.forward * .75f);
            testOrigin.y += .5f;
            //Debug.DrawRay(origin2, -Vector3.up, Color.red);
            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * climbSpeed;
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
                transitioning = false;
                //Vector3 tp = Vector3.Lerp(startPos, targetPos, 0.99f);
                states.transform.position = targetPos;
            }

            if (!inRot)
            {
                if (states.transform.rotation == targetRot)
                    inRot = true;
                Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, targetRot, t);
                states.mTransform.rotation = targetRotation;
            }

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
        }
    }
}
