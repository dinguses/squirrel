using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Used to check if the player should be stepping up a small stair
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Check Step Up")]
    public class CheckStepUp : StateActions
    {
        public override void Execute(StateManager states)
        {

            var bottomRay = states.mTransform.position + (states.mTransform.forward * 1.25f) + (Vector3.up * .1f);
            var topRay = states.mTransform.position + (states.mTransform.forward * 1.25f) + (Vector3.up * .6f);
            var topRayLong = states.mTransform.position + (states.mTransform.forward * 2.5f) + (Vector3.up * .6f);

            bool bottomHit;
            bool topHit;
            bool topHitLong;

            if (states.stepUp)
            {
                //Debug.DrawRay(bottomRay, states.mTransform.forward, Color.green);
                //Debug.DrawRay(topRay, states.mTransform.forward, Color.green);
                //Debug.DrawRay(topRayLong, states.mTransform.forward, Color.green);
            }
            else
            {
                //Debug.DrawRay(bottomRay, states.mTransform.forward, Color.blue);
                //Debug.DrawRay(topRay, states.mTransform.forward, Color.blue);
                //Debug.DrawRay(topRayLong, states.mTransform.forward, Color.blue);
            }

            RaycastHit hitBottom = new RaycastHit();
            RaycastHit hitTop = new RaycastHit();
            RaycastHit hitTopLong = new RaycastHit();

            if (Physics.Raycast(bottomRay, states.mTransform.forward, out hitBottom, 1, Layers.ignoreLayersController))
                bottomHit = true;
            else
                bottomHit = false;

            if (Physics.Raycast(topRay, states.mTransform.forward, out hitTop, 1, Layers.ignoreLayersController))
                topHit = true;
            else
                topHit = false;

            if (Physics.Raycast(topRayLong, states.mTransform.forward, out hitTopLong, 1, Layers.ignoreLayersController))
                topHitLong = true;
            else
                topHitLong = false;


            if (bottomHit && !topHit && !topHitLong && states.movementVariables.moveAmount > 0.05f)
            {
                states.stepUp = true;
                states.frontCollider.enabled = false;
                states.frontColliderJump.enabled = true;
            } else
            {
                //states.stepUpDelay = true;
                states.stepUp = false;
                states.frontCollider.enabled = true;
                states.frontColliderJump.enabled = false;
            }
        }
    }
}
