﻿using UnityEngine;
using System.Collections;

namespace PreServer
{
    [System.Serializable]
    public class MovementVariables
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public Vector3 moveDirection;
    }
}
