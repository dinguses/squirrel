using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Used only with AnimatorHook
    /// </summary>

    public abstract class AnimAction : ScriptableObject
    {
        public abstract void Execute(AnimatorData d);
    }
}
