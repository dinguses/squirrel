﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Enemy Actions/FollowTarget")]
    public class FollowTarget : StateActions
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
            isInFOV = inFOV(states.transform, states.target, angle, radius);
            if (isInFOV)
            {
                Quaternion temp = states.transform.rotation;
                states.transform.LookAt(new Vector3(states.target.position.x, states.transform.position.y, states.target.position.z));
                states.transform.rotation = Quaternion.Lerp(temp, states.transform.rotation, Time.deltaTime * states.turnSpeed);
                states.rigid.velocity = states.transform.forward * states.moveSpeed;
            }
            else
            {
                states.rigid.velocity = Vector3.zero;
                states.state = EnemyManager.DetectState.NONE;
            }
            //Debug.DrawLine(states.transform.position, PlayerManager.ptr.transform.position, isInFOV ? Color.green : Color.red);
            Debug.DrawRay(states.transform.position, (PlayerManager.ptr.transform.position + PlayerManager.ptr.transform.forward + PlayerManager.ptr.transform.up * 0.5f - states.transform.position).normalized * (radius < Vector3.Distance(PlayerManager.ptr.transform.position + PlayerManager.ptr.transform.forward + PlayerManager.ptr.transform.up * 0.5f, states.transform.position) ? radius : Vector3.Distance(PlayerManager.ptr.transform.position + PlayerManager.ptr.transform.forward + PlayerManager.ptr.transform.up * 0.5f, states.transform.position)), isInFOV ? Color.green : Color.red);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(angle, states.transform.up) * states.transform.forward * radius, Color.blue);
            Debug.DrawRay(states.transform.position, Quaternion.AngleAxis(-angle, states.transform.up) * states.transform.forward * radius, Color.blue);
            Debug.DrawRay(states.transform.position, states.transform.forward * radius, Color.black);
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

                    if (angle <= maxAngle)
                    {
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
            base.OnExit(sm);
        }
    }
}