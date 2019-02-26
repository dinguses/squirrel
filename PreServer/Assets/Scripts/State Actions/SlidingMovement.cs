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

        public override void Execute(StateManager states)
        {
            //float frontY = 0;
            RaycastHit hit;
            RaycastHit hit2;
            Vector3 origin = states.mTransform.position + states.mTransform.forward * frontRayOffset;
            Vector3 origin2 = states.mTransform.position;
            origin.y += .5f;
            origin2.y += .5f;

            Debug.DrawRay(origin, -Vector3.up, Color.red);
            Debug.DrawRay(origin2, -Vector3.up, Color.red);
            //Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * movementSpeed;

            if (Physics.Raycast(origin, -Vector3.up, out hit, 1, Layers.ignoreLayersController))
            {
                //float y = hit.point.y;

                states.groundNormal = hit.normal;



                //frontY = y - states.mTransform.position.y;

                if (Physics.Raycast(origin2, -Vector3.up, out hit2, 1, Layers.ignoreLayersController))
                {
                    if (hit.normal != hit2.normal)
                        states.rotateFast = false;
                    else
                        states.rotateFast = true;

                    //Debug.Log(hit2.point.y);
                }

                //Debug.Log("hit 1 distance - " + hit.distance);
                //Debug.Log("hit 2 distance - " + hit2.distance);
                if (hit.distance > .54f || hit2.distance > .54f)
                {
                    //Debug.Log("Sinking");
                    //targetVelocity.y -= 45f;
                    var test = states.rigid.position;
                    test.y -= .05f;
                   // states.rigid.MovePosition(test);
                }
            }

            Vector3 currentVelocity = states.rigid.velocity;
            Vector3 vector = hit.normal;
            vector.y += 0.5f;
            //Debug.DrawLine(origin, hit.point, Color.green);
            //Debug.DrawRay(origin, states.groundNormal, Color.yellow);
            //Vector3 dir = origin - states.groundNormal;
            //Vector3 forward = Vector3.Cross(hit.normal, Vector3.up);
            //vector = Vector3.Cross(forward, hit.normal);
            //Debug.DrawRay(origin, vector, Color.yellow);

            //Vector3 right = -left;
            //Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
            //Vector3 right = Vector3.Cross(-dir, Vector3.up).normalized;
            //Vector3 right = Vector3.Cross(dir, -Vector3.up).normalized;
            //float moveAmount = states.movementVariables.moveAmount;

            //if (moveAmount > 0.1f)
            //{
            states.rigid.isKinematic = false;
                states.rigid.drag = 0;

                //if (Mathf.Abs(frontY) > 0.02f)
                //{
                //    targetVelocity.y = ((frontY > 0) ? frontY + 0.2f : frontY) * movementSpeed;

                //    if (targetVelocity.y > 0)
                //    {
                //        targetVelocity.y = 0;
                //    }
                //}
            //}

            //states.targetVelocity = targetVelocity;
            //states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);

        }
    }
}
