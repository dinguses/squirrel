using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Moves the player while grinding
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Grind Movement")]
    public class GrindMovement : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.rigid.drag = 0;

            states.rotateBool = true;

            // Get target velocity from player's move amount and current velocity           
            Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * 10.5f * states.groundSpeedMult;
            Vector3 currentVelocity = states.rigid.velocity;

            // Set velocity
            states.targetVelocity = targetVelocity;
            states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * 10.5f);

            // If there is a current grind center
            if (!states.exitingGrind)
            {
                // Move Player towards center should they not be on it
                Vector3 grindCenterClosestPoint = states.grindCenter.ClosestPoint(states.mTransform.position);

                states.rigid.position = Vector3.Lerp(states.rigid.position, grindCenterClosestPoint, Time.deltaTime * 100);
                //states.rigid.MovePosition(grindCenterClosestPoint);


                //Debug.Log(Vector3.Distance(states.rigid.position, grindCenterClosestPoint));
                /*
                int mult = 10;

                if (Vector3.Distance(states.rigid.position, grindCenterClosestPoint) < .2f && states.inJoint)
                    mult = 3;

                states.rigid.position = Vector3.Lerp(states.rigid.position, grindCenterClosestPoint, Time.deltaTime * mult);*/
                //states.rigid.MovePosition(grindCenterClosestPoint);
            }
        }
    }
}
