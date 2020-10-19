using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class GrindColliderBack : MonoBehaviour
    {
        public PlayerManager states;

        private void OnTriggerExit(Collider other)
        {
            if (other.name == "GrindCollider")
            {
                Debug.Log("i'm the culprit");
                states.BackLeftTest();
            }
        }
    }
}