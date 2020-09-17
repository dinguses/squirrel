using UnityEngine;

namespace PreServer
{
    public class GrindColliderFront : MonoBehaviour
    {
        public PlayerManager states;

        private void OnTriggerEnter(Collider other)
        {
            if (states.currentState.name == "Grinding" && (other.name == "GrindColliderFront" || other.name == "GrindCollider"))
            {
                Debug.Log(Time.frameCount + " - Entered end");
                //states.NextPoint();

                if (states.movementVariables.moveAmount <= 0.5f)
                {
                    states.mTransform.position = Vector3.MoveTowards(states.mTransform.position, (states.mTransform.position + states.mTransform.forward + states.mTransform.forward), .5f);
                }

                states.BackLeftTest();
            }
        }
    }
}