using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Enemy Actions/CheckTarget")]
    public class CheckTarget : StateActions
    {
        public float radius = 3;
        public float suspicionAngle = 45;
        public float detectAngle = 20;
        public float timeToWait = 3f;
        public float turnAngle = 30;
        float timer;
        //public float heightMultiplier = 1.5f;
        bool isInFOV = false;
        EnemyManager states;
        Vector3 targetPos;
        public override void OnEnter(StateManager sm)
        {
            CheckState(sm);
            timer = timeToWait;
            targetPos = states.target.position;
            states.agent.SetDestination(targetPos);
            base.OnEnter(sm);
        }

        public override void Execute(StateManager sm)
        {

        }

        void CheckState(StateManager sm)
        {
            if (states == null)
                states = (EnemyManager)sm;
        }

        public override void OnFixed(StateManager sm)
        {
            CheckState(sm);
            base.OnFixed(sm);
            inFOV(states.transform, states.target, radius);
            if (timer <= 0)
            {
                states.rigid.velocity = Vector3.zero;
                states.state = EnemyManager.DetectState.NONE;
            }
            if(states.agent.remainingDistance < 1f)
            {
                timer -= Time.fixedDeltaTime;
            }
            //Debug.DrawLine(states.transform.position, PlayerManager.ptr.transform.position, isInFOV ? Color.green : Color.red);
            Debug.DrawRay(states.transform.position, (PlayerManager.ptr.transform.position + PlayerManager.ptr.transform.forward + PlayerManager.ptr.transform.up * 0.5f - states.transform.position).normalized * (radius < Vector3.Distance(PlayerManager.ptr.transform.position + PlayerManager.ptr.transform.forward + PlayerManager.ptr.transform.up * 0.5f, states.transform.position) ? radius : Vector3.Distance(PlayerManager.ptr.transform.position + PlayerManager.ptr.transform.forward + PlayerManager.ptr.transform.up * 0.5f, states.transform.position)), states.state == EnemyManager.DetectState.SUSPICIOUS ? Color.yellow : (states.state == EnemyManager.DetectState.DETECTED ? Color.green : Color.red));
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(detectAngle, states.transform.up) * states.transform.forward * radius, Color.blue);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(-detectAngle, states.transform.up) * states.transform.forward * radius, Color.blue);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(suspicionAngle, states.transform.up) * states.transform.forward * radius, Color.cyan);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(-suspicionAngle, states.transform.up) * states.transform.forward * radius, Color.cyan);
            Debug.DrawRay(states.transform.position, states.transform.forward * radius, Color.black);
        }

        public bool inFOV(Transform checkingObject, Transform target, float maxRadius)
        {
            Vector3 directionBetween = (target.position - checkingObject.position).normalized;
            directionBetween.y = 0;
            RaycastHit hit;
            if (Physics.Raycast(checkingObject.position, (target.position + target.forward + target.up * 0.5f - checkingObject.position).normalized, out hit, maxRadius))
            {
                if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Controller")
                {
                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= suspicionAngle)
                    {
                        if (angle <= detectAngle)
                        {
                            states.state = EnemyManager.DetectState.DETECTED;
                            states.target = hit.transform;
                        }
                        else
                        {
                            timer = timeToWait;
                            targetPos = hit.transform.position;
                            states.agent.SetDestination(targetPos);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public override void OnExit(StateManager sm)
        {
            if(states.state == EnemyManager.DetectState.NONE)
                states.target = null;
            states.agent.ResetPath();
            base.OnExit(sm);
        }
    }
}
