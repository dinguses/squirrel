using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    //This system needs to work with slopes, otherwise we got a problem
    [CreateAssetMenu(menuName = "Actions/State Actions/Lag Dash")]
    public class LagDash : StateActions
    {
        bool inPos;
        bool inRot;
        float t;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float offsetFromWall = 0.1f;
        float delta;
        RaycastHit hitInfo = new RaycastHit();
        public float dashSpeed = 40f;
        public float dashTime = 0.15f;
        public override void OnEnter(StateManager states)
        {
            base.OnEnter(states);
            states.dashInAirCounter++;
            states.rigid.useGravity = false;
            states.rigid.velocity = states.transform.forward * dashSpeed;
            //Debug.Log("LagDash Setting Velocity to: " + states.rigid.velocity);
            states.rigid.drag = 0;
            timer = 0;
            //startPos = states.transform.position;
            //t = 0;
            //inPos = false;
            //inRot = false;
            //bool hit = Physics.BoxCast(states.transform.position + (states.transform.forward * 2f),
            //    new Vector3(states.frontCollider.bounds.size.x * 0.49f, states.frontCollider.bounds.size.y * 0.49f, 2.5f),
            //    states.transform.forward, out hitInfo, states.transform.rotation, 5f, Layers.ignoreLayersController);
            //if (hit)
            //{
            //    targetPos = hitInfo.point - (states.transform.forward * 2f);
            //    float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            //    if (angle < 70)
            //    {
            //        targetRot = Quaternion.FromToRotation(states.transform.up, hitInfo.normal) * states.transform.rotation;
            //    }
            //    else
            //    {
            //        inRot = true;
            //    }
            //}
            //else
            //{
            //    targetPos = states.transform.position + (states.transform.forward * 7f);
            //    inRot = true;
            //}
        }

        public override void Execute(StateManager states)
        {

        }
        float timer = 0;
        public override void OnUpdate(StateManager states)
        {
            base.OnUpdate(states);
            if(timer > dashTime)
            {
                states.dashActive = false;
            }
            timer += Time.deltaTime;
            //Debug.Log("LagDash Update: " + states.rigid.velocity);
            //Transfer(states);
            //Debug.DrawRay(targetPos, Vector3.up * ff, Color.yellow);
        }

        void CheckRaycast(StateManager states)
        {
            
        }
        
        void Transfer(StateManager states)
        {
            delta = Time.deltaTime * 4;

            if (inPos && inRot)
            {
                states.dashActive = false;
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

        public override void OnExit(StateManager states)
        {
            base.OnExit(states);
            states.rigid.useGravity = true;
            states.rigid.velocity = Vector3.zero;
            timer = 0;
            states.lagDashCooldown = 1.0f;
        }
    }
}
