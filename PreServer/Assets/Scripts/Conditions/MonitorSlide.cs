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
        float frontAngle = 0;
        float backAngle = 0;
        float middleAngle = 0;
        int count = 0;
        public override bool CheckCondition(StateManager state)
        {
            //count = 0;
            ////if (state.front != null)
            ////    frontAngle = Vector3.Angle(state.frontNormal, Vector3.up);
            ////else
            ////    frontAngle = 0;

            ////if (state.back != null)
            ////    backAngle = Vector3.Angle(state.backNormal, Vector3.up);
            ////else
            ////    backAngle = 0;

            //if (state.middle != null)
            //    middleAngle = Vector3.Angle(state.middleNormal, Vector3.up);
            //else
            //    middleAngle = 0;

            ////count += CheckAngle(frontAngle) ? 1 : 0;
            //count += CheckAngle(middleAngle) ? 1 : 0;
            ////count += CheckAngle(backAngle) ? 1 : 0;
            //if (count == 1)
            //{
            //    //Debug.Log("sliding?");
            //    return true;
            //}

            return state.isSliding;
        }

        bool CheckAngle(float angle)
        {
            return angle > minSlideAngle && angle < maxSlideAngle;
        }
    }
}
