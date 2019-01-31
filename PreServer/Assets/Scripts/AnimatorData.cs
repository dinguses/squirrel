using UnityEngine;
using System.Collections;

namespace PreServer
{
    public class AnimatorData
    {
        public Animator anim;

        public AnimatorData(Animator anim)
        {
            this.anim = anim;

            AnimatorHook aHook = anim.GetComponent<AnimatorHook>();
            if (aHook == null)
                aHook = anim.gameObject.AddComponent<AnimatorHook>();
            aHook.data = this;
        }
    }
}
