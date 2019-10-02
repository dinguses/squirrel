﻿using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Executing a 180  turn on the grind
    /// </summary>

    [CreateAssetMenu (menuName = "Actions/State Actions/Grind 180")]
    public class Grind180 : StateActions
    {
        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            if (states.anim.GetBool(states.hashes.waitForAnimation))
            {
                Debug.Log(states.testINT);
                states.testINT++;

                Vector3 eulerAngles = states.mTransform.rotation.eulerAngles;

                float rotateAmount = states.anim.GetFloat(states.hashes.rotate_test);
                float currentHold = rotateAmount;

                rotateAmount = rotateAmount - states.held180RotationAmt;

                states.mTransform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + rotateAmount, eulerAngles.z);
                states.held180RotationAmt = currentHold;
            }

        }
    }
}
