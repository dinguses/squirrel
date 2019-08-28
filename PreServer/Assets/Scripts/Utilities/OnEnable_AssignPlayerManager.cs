using UnityEngine;
using System.Collections;

namespace PreServer
{
    public class OnEnable_AssignPlayerManager : MonoBehaviour
    {
        public PlayerManagerVariable targetVariable;

        private void OnEnable()
        {
            targetVariable.value = GetComponent<PlayerManager>();
            Destroy(this);
        }
    }
}
