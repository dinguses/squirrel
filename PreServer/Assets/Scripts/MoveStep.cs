using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// MoveStep is an NPC action that moves them to a NPC_Point.
    /// </summary>
    public class MoveStep : NPCStep
    {
        public Vector3 destination;
        public string pointName;
    }
}