using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Check if the player should be sliding.
    /// 
    /// Very rudimentary, only kind of works
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Monitor Slide")]
    public class MonitorSlide : Condition
    {
        public float minSlideAngle = 35;
        
        public override bool CheckCondition(StateManager state)
        {
            bool result = false;

            if (Vector3.Angle(state.backupGroundNormal, Vector3.up) > minSlideAngle && state.backupGroundNormal == state.middleNormal)
            {
                //Debug.Log("sliding?");
                result = true;
            }

            return result;
        }
    }
}
