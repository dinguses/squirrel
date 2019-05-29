using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PreServer
{
    public class MsgStep : NPCStep
    {
        public string msg;
        public List<int> waitForPals;
    }
}