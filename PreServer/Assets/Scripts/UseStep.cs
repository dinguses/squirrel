using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// UseStep is an NPC action that lets the NPC interact with an object in-game, like a bench
    /// </summary>
    public class UseStep : NPCStep
    {
        public string objToUse;
    }
}