using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Exiting Climb")]
    public class ExitingClimb : StateActions
    {
        bool inPos;
        bool inRot;
        float t;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        float delta;
        PlayerManager states;
        float angle = 0;
        Vector3 newDirection;
        Vector3 originalDirection;
        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            base.OnEnter(states);
            states.rigid.velocity = Vector3.zero;
            states.rigid.useGravity = false;
            startPos = states.transform.position;
            targetPos = states.climbHit.point + (states.climbHit.normal * states.climbingVariables.offsetFromWall);
            targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
            t = 0;
            inPos = false;
            inRot = false;
            SafeClimb();
        }

        void SafeClimb()
        {

            //This is under the assumption that the climbhit will never be inside a mesh collider
            RaycastHit hit = new RaycastHit();

            Vector3 originalDirection = targetRot * Vector3.forward.normalized;
            Vector3 newDirection = originalDirection;

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

            if (angle != 0)
            {
                targetRot = Quaternion.AngleAxis(angle, states.climbHit.normal) * targetRot;
                originalDirection = targetRot * Vector3.forward.normalized;
            }
            bool backBlocked = false;
            //Check to see the position we will end up at is colliding with the length of the squirrel's collider
            //if there is any object in the way, then move the target position backwards
            while (Physics.SphereCast(targetPos + states.climbHit.normal * 0.45f, 0.375f, originalDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                backBlocked = CheckBackBlocked(originalDirection, hit);
                if (backBlocked)
                    break;
            }
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

        public override void Execute(StateManager sm)
        {

        }

        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);
            delta = Time.deltaTime * 4 * states.climbingVariables.transferSpeedMult;
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
                else
                {
                    inRot = false;
                    Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, targetRot, t);
                    if (t >= 1)
                        targetRotation = targetRot;
                    states.mTransform.rotation = targetRotation;
                }
            }

            if (inPos && inRot)
            {
                states.climbState = PlayerManager.ClimbState.NONE;
            }
            //Debug.Log(Time.frameCount + " || inPos = " + inPos + " inRot = " + inRot);
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
            ((PlayerManager)states).UpdateGroundNormals();
            states.rigid.velocity = Vector3.zero;
            states.rigid.useGravity = true;
            states.anim.SetBool(states.hashes.isClimbing, false);
        }
    }
}
