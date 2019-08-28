using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class EnemyManager : MonoBehaviour
    {
        public State currentState;

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