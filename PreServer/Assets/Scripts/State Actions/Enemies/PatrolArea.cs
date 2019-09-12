using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Enemy Actions/Patrol Area")]
    public class PatrolArea : StateActions
    {
        public Vector3 min;
        public Vector3 max;
        public float angle = 30;
        public float dist = 1f;
        EnemyManager states;
        Vector3 targetPos;
        NavMeshPath path;
        RaycastHit hit;
        public override void OnEnter(StateManager sm)
        {
            CheckState(sm);
            GenerateTarget();
            base.OnEnter(sm);
        }

        public override void Execute(StateManager sm)
        {

        }

        void CheckState(StateManager sm)
        {
            if (states == null)
                states = (EnemyManager)sm;
            if (path == null)
                path = new NavMeshPath();
        }

        public override void OnFixed(StateManager sm)
        {
            CheckState(sm);
            base.OnFixed(sm);
            if (targetPos == Vector3.zero || states.agent.remainingDistance <= dist)
                GenerateTarget();
            MoveToTarget();
            //Debug.DrawLine(states.transform.position, PlayerManager.ptr.transform.position, isInFOV ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(min.x, states.transform.position.y, min.z), new Vector3(max.x, states.transform.position.y, min.z), Color.black);
            Debug.DrawLine(new Vector3(max.x, states.transform.position.y, min.z), new Vector3(max.x, states.transform.position.y, max.z), Color.black);
            Debug.DrawLine(new Vector3(max.x, states.transform.position.y, max.z), new Vector3(min.x, states.transform.position.y, max.z), Color.black);
            Debug.DrawLine(new Vector3(min.x, states.transform.position.y, max.z), new Vector3(min.x, states.transform.position.y, min.z), Color.black);
            Debug.DrawRay(targetPos, Vector3.up, Color.green);
        }

        void MoveToTarget()
        {
            //targetPos.y = states.transform.position.y;
            //Quaternion temp = states.transform.rotation;
            //states.transform.LookAt(targetPos);
            //states.transform.rotation = Quaternion.Lerp(temp, states.transform.rotation, Time.deltaTime * states.turnSpeed);
            //Vector3 targetVel = states.transform.forward * states.moveSpeed;
            //targetVel.y = states.rigid.velocity.y;
            //states.rigid.velocity = targetVel;
            //states.agent.CalculatePath(targetPos, path);
        }
        
        void GenerateTarget()
        {
            targetPos = new Vector3(Random.Range(min.x, max.x), states.transform.position.y, Random.Range(min.z, max.z));
            states.agent.SetDestination(targetPos);
        }

        public override void OnExit(StateManager sm)
        {
            states.agent.ResetPath();
            base.OnExit(sm);
        }
    }
}
