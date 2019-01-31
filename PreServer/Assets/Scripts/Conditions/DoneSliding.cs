using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Very early attempt to detect if the player is finished sliding based on their ground angle
    /// 
    /// Doesn't really work
    /// </summary>

    [CreateAssetMenu(menuName = "Conditions/Done Sliding")]
    public class DoneSliding : Condition
    {
        public float minSlideAngle = 35;

        public override bool CheckCondition(StateManager state)
        {
            bool result = false;

            if (Vector3.Angle(state.groundNormal, Vector3.up) < minSlideAngle)
            {
                result = true;
            }

            return result;
        }
    }
}
