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
        public float maxSlideAngle = 70;
        float angle = 0;
        public override bool CheckCondition(StateManager state)
        {
            bool result = false;
            angle = Vector3.Angle(state.backupGroundNormal, Vector3.up);
            if (angle > minSlideAngle && angle < maxSlideAngle/* && state.backupGroundNormal == state.middleNormal*/)
            {
                //Debug.Log("sliding?");
                result = true;
            }

            return result;
        }
    }
}
