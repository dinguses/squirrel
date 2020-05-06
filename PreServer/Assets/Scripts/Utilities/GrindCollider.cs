using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class GrindCollider : MonoBehaviour
    {
        public PlayerManager states;

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "GrindCollider")
            {
                Debug.Log("hit too?");
                states.NextPoint();
            }
        }
    }
}