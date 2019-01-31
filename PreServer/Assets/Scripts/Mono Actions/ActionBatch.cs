using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Used to execute actions outside of the behaviour editor
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/Mono Actions/Action Batch")]
    public class ActionBatch : Action
    {
        public Action[] actions;

        public override void Execute()
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Execute();
            }
        }
    }
}