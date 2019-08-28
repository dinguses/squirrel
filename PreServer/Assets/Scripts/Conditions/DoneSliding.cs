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
        public float maxSlideAngle = 70;
        float frontAngle = 0;
        float backAngle = 0;
        float middleAngle = 0;
        int count = 0;

        bool result = false;

        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            //count = 0;
            ////bool result = false;
            ////Vector3 frontOrigin = state.mTransform.position;
            ////frontOrigin += state.mTransform.forward + state.mTransform.forward / 2;
            ////frontOrigin.y += .7f;
            ////RaycastHit frontHit = new RaycastHit();
            ////Vector3 dir = -Vector3.up;
            ////dir.z = dir.z - state.groundNormal.z;
            ////float dis = .8f;
            ////Physics.SphereCast(frontOrigin, 0.3f, dir, out frontHit, dis, Layers.ignoreLayersController);
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
            //    return true;
            //}

            result = state.isSliding;

            return !state.isSliding;
        }

        /*bool CheckAngle(float angle)
        {
            return angle <= minSlideAngle || angle >= maxSlideAngle;
        }*/
    }
}
