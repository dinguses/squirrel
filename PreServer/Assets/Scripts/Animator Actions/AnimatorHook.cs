using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// I think this was mostly for IK, probably unnecessary
    /// </summary>

    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        public AnimatorData data;

        public AnimAction[] animActions;

        private void OnAnimatorIK(int layerIndex)
        {
            for (int i = 0; i < animActions.Length; i++)
            {
                animActions[i].Execute(data);
            }
        }
    }
}
