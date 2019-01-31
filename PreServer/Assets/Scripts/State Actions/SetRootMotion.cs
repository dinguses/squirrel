using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Sets root motion for StateManager's anim
    /// </summary>

    [CreateAssetMenu (menuName = "Actions/State Actions/Set Root Motion")]
    public class SetRootMotion : StateActions
    {
        public bool status;

        public override void Execute(StateManager states)
        {
            states.anim.applyRootMotion = status;
        }
    }
}
