using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Movement for when player is sliding.
    /// 
    /// Currently does nothing because the player's velocity never gets changed
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Sliding Movement")]
    public class SlidingMovement : StateActions
    {
        public float frontRayOffset = .5f;
        public float movementSpeed;
        public float movementTime = 10;

        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            //float frontY = 0;
            //RaycastHit hit;
            //RaycastHit hit2;
            //Vector3 origin = states.mTransform.position + states.mTransform.forward * frontRayOffset;
            //Vector3 origin2 = states.mTransform.position;
            //origin.y += .5f;
            //origin2.y += .5f;

            //Debug.DrawRay(origin, -Vector3.up, Color.red);
            //Debug.DrawRay(origin2, -Vector3.up, Color.red);

            //if (Physics.Raycast(origin, -Vector3.up, out hit, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            //{
            //    states.groundNormal = hit.normal;

            //    if (Physics.Raycast(origin2, -Vector3.up, out hit2, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            //    {
            //        if (hit.normal != hit2.normal)
            //            states.rotateFast = false;
            //        else
            //            states.rotateFast = true;

            //    }
            //    if (hit.distance > .54f || hit2.distance > .54f)
            //    {
            //        var test = states.rigid.position;
            //        test.y -= .05f;
            //    }
            //}

            //Vector3 currentVelocity = states.rigid.velocity;
            //Vector3 vector = hit.normal;
            //vector.y += 0.5f;

            states.rigid.isKinematic = false;
            states.rigid.drag = 0;
        }
    }
}
