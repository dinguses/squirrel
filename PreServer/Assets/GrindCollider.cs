using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class GrindCollider : MonoBehaviour
    {
        public StateManager sm;

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "GrindCollider")
            {
                sm.NextPoint();
            }
        }
    }
}