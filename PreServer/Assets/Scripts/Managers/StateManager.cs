using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

namespace PreServer
{
    public class StateManager : MonoBehaviour
    {
        public State currentState;

        public virtual void FixedUpdateParent()
        {

        }

        public virtual void UpdateParent()
        {

        }

        public virtual void LateUpdateParent()
        {

        }

        private void FixedUpdate()
        {
            FixedUpdateParent();

            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }

        private void Update()
        {
            UpdateParent();

            if (currentState != null)
            {
                currentState.Tick(this);
            }
        }

        private void LateUpdate()
        {
            LateUpdateParent();

            if (currentState != null)
            {
                currentState.LateTick(this);
            }
        }
    }
}