using UnityEngine;
using System.Collections;

namespace PreServer
{
    public class OnEnable_AssignStateManager : MonoBehaviour
    {
        public StateManagerVariable targetVariable;

        private void OnEnable()
        {
            targetVariable.value = GetComponent<StateManager>();
            Destroy(this);
        }
    }
}
