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
        public float offsetFromWall = 0.3f;
        float delta;
        PlayerManager states;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            base.OnEnter(states);
            states.rigid.velocity = Vector3.zero;
            states.rigid.useGravity = false;
            startPos = states.transform.position;
            targetPos = states.climbHit.point + (states.climbHit.normal * offsetFromWall);
            targetRot = Quaternion.FromToRotation(states.transform.up, states.climbHit.normal) * states.transform.rotation;
            t = 0;
            inPos = false;
            inRot = false;
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
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);
            states.rigid.velocity = Vector3.zero;
            states.rigid.useGravity = true;
            states.anim.SetBool(states.hashes.isClimbing, false);
        }
    }
}
