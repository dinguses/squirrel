using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class GrindColliderTest : MonoBehaviour
    {
        public PlayerManager states;

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "GrindCollider" || other.name == "GrindColliderFront")
            {
                Debug.Log("entering w/ test");
                states.inJoint = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name == "GrindCollider" || other.name == "GrindColliderFront")
            {
                Debug.Log("exiting w/ test");
                states.inJoint = false;
            }
        }
    }
}