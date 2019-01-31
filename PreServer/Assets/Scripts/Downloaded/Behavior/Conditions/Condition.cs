﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public abstract class Condition : ScriptableObject
    {
		public string description;

        public abstract bool CheckCondition(StateManager state);

    }
}
