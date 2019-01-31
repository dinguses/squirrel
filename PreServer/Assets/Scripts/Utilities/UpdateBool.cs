using UnityEngine;
using System.Collections;

namespace PreServer
{
    public class UpdateBool : StateMachineBehaviour
    {
        public string targetBool;
        public bool status;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(targetBool, status);
        }
    }
}
