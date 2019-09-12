using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PreServer
{
    public class EnemyManager : StateManager
    {
        public enum DetectState
        {
            NONE, SUSPICIOUS, DETECTED
        }
        public DetectState state;
        public Transform target;
        public float moveSpeed = 10;
        public float turnSpeed = 5;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public NavMeshAgent agent;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
        }

        /*private void FixedUpdate()
        {
            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.Tick(this);
            }
        }*/
    }
}