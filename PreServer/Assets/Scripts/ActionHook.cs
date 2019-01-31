using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Used to run actions without using behaviour editor. Used for the camera currently.
    /// </summary>

    public class ActionHook : MonoBehaviour
    {
        public Action[] fixedUpdateActions;
        public Action[] updateActions;
        //public Action[] lateActions;

        void FixedUpdate()
        {
            if (fixedUpdateActions == null)
                return;

            for (int i = 0; i < fixedUpdateActions.Length; i++)
            {
                fixedUpdateActions[i].Execute();
            }
        }

        void Update()
        {
            if (updateActions == null)
                return;

            for (int i = 0; i < updateActions.Length; i++)
            {
                updateActions[i].Execute();
            }
        }

        /*private void LateUpdate()
        {
            if (lateActions == null)
                return;

            for (int i = 0; i < lateActions.Length; i++)
            {
                lateActions[i].Execute();
            }
        }*/
    }
}
