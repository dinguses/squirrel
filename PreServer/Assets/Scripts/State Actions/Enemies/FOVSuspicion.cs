using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Enemy Actions/FOVSuspicion")]
    public class FOVSuspicion : StateActions
    {
        public float radius = 3;
        public float angle = 20;
        //public float heightMultiplier = 1.5f;
        bool isInFOV = false;
        EnemyManager states;

        public override void OnEnter(StateManager sm)
        {
            CheckState(sm);
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
            isInFOV = inFOV(states.transform, PlayerManager.ptr.transform, angle, radius);
            //Debug.DrawLine(states.transform.position, PlayerManager.ptr.transform.position, isInFOV ? Color.green : Color.red);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(angle, states.transform.up) * states.transform.forward * radius, Color.cyan);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(-angle, states.transform.up) * states.transform.forward * radius, Color.cyan);
        }

        public bool inFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
        {
            Vector3 directionBetween = (target.position - checkingObject.position).normalized;
            directionBetween.y = 0;
            RaycastHit hit;
            if (Physics.Raycast(checkingObject.position, (target.position + target.forward + target.up * 0.5f - checkingObject.position).normalized, out hit, maxRadius))
            {
                if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Controller")
                {
                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= maxAngle && states.state != EnemyManager.DetectState.DETECTED)
                    {
                        states.target = hit.transform;
                        states.state = EnemyManager.DetectState.SUSPICIOUS;
                        return true;
                    }
                }
            }
            return false;
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(sm);
        }
    }
}
