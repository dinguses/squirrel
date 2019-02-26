using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    /// <summary>
    /// Make gravity increase so they get off the slanted wall faster
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Grounded In Air")]
    public class GroundedInAir : StateActions
    {
        public float gravityAdditive;
        public float downwardsGravity;
        public float movementTime = 10;
        public float slideTime = 0.3f;
        public float movementSpeed;
        float gravity = 0;
        public float groundedDis = .8f;
        public float onAirDis = .85f;
        public LayerMask groundLayer;
        public override void Execute(StateManager states)
        {

        }

        public override void OnEnter(StateManager states)
        {
            gravity = downwardsGravity;
        }
        Vector3 currentVelocity;
        Vector3 targetVelocity;
        public override void OnUpdate(StateManager states)
        {
            currentVelocity = states.rigid.velocity;
            targetVelocity = currentVelocity;
            targetVelocity.y = currentVelocity.y - gravity;
            targetVelocity.z = 0;
            targetVelocity.x = 0;
            states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);
            gravity += gravityAdditive;

            // Setup origin points for three different ground checking vector3s. One in middle of player, one in front, and one in back
            Vector3 middleOrigin = states.mTransform.position;
            Vector3 frontOrigin = states.mTransform.position;
            Vector3 backOrigin = states.mTransform.position;

            middleOrigin += states.mTransform.forward;
            frontOrigin += states.mTransform.forward + states.mTransform.forward / 2;
            //backOrigin += states.mTransform.forward / 2;

            // Origins should be coming from inside of player
            middleOrigin.y += .7f;
            frontOrigin.y += .7f;
            backOrigin.y += .7f;

            // Dir represents the downward direction
            Vector3 dir = -Vector3.up;


            //TODO: testing this

            if (states.isGrounded)
            {
                // If player is on a sloped surface, must account for the normal
                dir.z = dir.z - states.groundNormal.z;
            }
            else
            {
                dir.z = dir.z - states.mTransform.up.z;
                //dir.z = dir.z 
            }

            // Set distance depending on if player grounded or in air
            float dis = (states.isGrounded) ? groundedDis : onAirDis;

            // RaycastHits for each grounding ray
            RaycastHit middleHit = new RaycastHit();
            RaycastHit frontHit = new RaycastHit();
            RaycastHit backHit = new RaycastHit();

            // Draw the rays
            Debug.DrawRay(middleOrigin, dir * dis, Color.green);
            Debug.DrawRay(frontOrigin, dir * dis, Color.yellow);
            Debug.DrawRay(backOrigin, dir * dis, Color.white);

            if (Physics.SphereCast(middleOrigin, 0.3f, dir, out middleHit, dis, Layers.ignoreLayersController))
            {
                states.middleNormal = middleHit.normal;
                states.middle = middleHit.transform.gameObject;
            }
            else
            {
                states.middle = null;
            }

            if (Physics.Raycast(frontOrigin, dir, out frontHit, dis, Layers.ignoreLayersController))
            {
                states.frontNormal = frontHit.normal;
                states.front = frontHit.transform.gameObject;
            }
            else
            {
                states.front = null;
            }

            if (Physics.SphereCast(backOrigin, 0.3f, dir, out backHit, dis, Layers.ignoreLayersController))
            {
                states.backNormal = backHit.normal;
                states.back = backHit.transform.gameObject;
            }
            else
            {
                states.back = null;
            }
            states.isGrounded = (isGrounded(states.frontCollider) || isGrounded(states.backCollider));
        }

        //Checks to see if the collider is interacting with anything on the default layer '0'
        //https://www.youtube.com/watch?v=vdOFUFMiPDU
        bool isGrounded(CapsuleCollider col)
        {
            return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 0.9f, groundLayer);
        }

        public override void OnExit(StateManager states)
        {
            gravity = downwardsGravity;
        }
    }
}

