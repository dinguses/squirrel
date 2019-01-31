using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute();
    }
}
