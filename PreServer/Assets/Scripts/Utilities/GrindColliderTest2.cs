using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class GrindColliderTest2 : MonoBehaviour
    {
        public PlayerManager states;

        private void OnTriggerEnter(Collider other)
        {
            if (states.currentState.name == "Grinding")
            {
                Debug.Log("Entered end");

                if (states.movementVariables.moveAmount <= 0.5f)
                {
                    states.mTransform.position = Vector3.MoveTowards(states.mTransform.position, (states.mTransform.position + (states.mTransform.forward*10)), .3f);
                }

                states.BackLeftTest();
            }
        }
    }
}