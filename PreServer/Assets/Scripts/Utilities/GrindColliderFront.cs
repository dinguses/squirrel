using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class GrindColliderFront : MonoBehaviour
    {
        public PlayerManager states;

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "GrindColliderFront")
            {
                Debug.Log("Entered point");
                states.NextPoint();
            }
        }
    }
}