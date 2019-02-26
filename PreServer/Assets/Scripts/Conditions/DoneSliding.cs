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
        float angle = 0;
        public override bool CheckCondition(StateManager state)
        {
            //bool result = false;
            Vector3 frontOrigin = state.mTransform.position;
            frontOrigin += state.mTransform.forward + state.mTransform.forward / 2;
            frontOrigin.y += .7f;
            RaycastHit frontHit = new RaycastHit();
            Vector3 dir = -Vector3.up;
            dir.z = dir.z - state.groundNormal.z;
            float dis = .8f;
            Physics.SphereCast(frontOrigin, 0.3f, dir, out frontHit, dis, Layers.ignoreLayersController);

            angle = Vector3.Angle(frontHit.normal, Vector3.up);
            if (angle <= minSlideAngle || angle >= maxSlideAngle)
            {
                return true;
            }

            return false;
        }
    }
}
