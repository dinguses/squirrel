using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Logic for detecting whether or not player is grounded
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/IsGroundedNonPlacing")]
    public class IsGrounded : StateActions
    {
        public float groundedDis = .8f;
        public float onAirDis = .85f;
        public LayerMask groundLayer;
        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

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
            //Debug.Log(Time.frameCount + " || Front Collider Grounded: " + isGrounded(states.frontCollider));
            // If player is already grounded, check if they should remain
            //if (states.isGrounded)
            //{
            float angle = 0;
            if (Physics.SphereCast(middleOrigin, 0.3f, dir, out middleHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.middleNormal = middleHit.normal;
                //states.middle = middleHit.transform.gameObject;
                angle = Vector3.Angle(middleHit.normal, Vector3.up);
                if (angle >= 70)
                    states.middle = null;
                else
                    states.middle = middleHit.transform.gameObject;
            }
            else
            {
                states.middle = null;
            }

            if (Physics.Raycast(frontOrigin, dir, out frontHit, dis + 0.3f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.frontNormal = frontHit.normal;
                //states.front = frontHit.transform.gameObject;
                angle = Vector3.Angle(frontHit.normal, Vector3.up);
                if (angle >= 70)
                    states.front = null;
                else
                    states.front = frontHit.transform.gameObject;
            }
            else
            {
                states.front = null;
            }

            if (Physics.SphereCast(backOrigin, 0.3f, dir, out backHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.backNormal = backHit.normal;
                //states.back = backHit.transform.gameObject;
                angle = Vector3.Angle(backHit.normal, Vector3.up);
                if (angle >= 70)
                    states.back = null;
                else
                    states.back = backHit.transform.gameObject;
            }
            else
            {
                states.back = null;
            }
            //}

            //// If player is not grounded, see if they should be
            //else
            //{                
            //    // If any of the three rays hit the ground AND player isn't stepping up AND player's y velocity isn't greater than 1.0f
            //    // Velocity check is to make sure player gets fully off the ground for a jump on a sloped surface
            //    if ((Physics.SphereCast(middleOrigin, 0.3f, dir, out middleHit, dis, Layers.ignoreLayersController) ||
            //        Physics.SphereCast(frontOrigin, 0.3f, dir, out frontHit, dis, Layers.ignoreLayersController) ||
            //        Physics.SphereCast(backOrigin, 0.3f, dir, out backHit, dis, Layers.ignoreLayersController)  && !states.stepUpJump && states.rigid.velocity.y <= 1.0f))
            //    {
            //        //Debug.Log("Grounding with - " + states.rigid.velocity.y);
            //        //Debug.Log(middleHit.collider.name);
            //        /*var test = origin.y - .7f - hit.point.y;

            //        if (test > .1)
            //        {
            //            var test2 = states.rigid.velocity;
            //            var test3 = new Vector3(test2.x, test2.y - 2f, test2.z);
            //            states.rigid.velocity = Vector3.Lerp(test2, test3, states.delta * 20);
            //        }*/

            //        //Debug.Log("dist to ground? - " + (hit.distance - .3f));
            //        //if (middleHit.distance > .1f)
            //        //{
            //        //Debug.Log("ehhl!");
            //        //states.mTransform.GetComponent<Rigidbody>().position -= new Vector3(0, (hit.distance - .1f) * 2, 0);
            //        //}


            //        // If the back raycast hits something, but the front and middle aren't, AND the player is grounded, then the player is probably on a slope, but not rotated properly
            //        if (backHit.normal != states.groundNormal && frontHit.normal == Vector3.zero && middleHit.normal == Vector3.zero)
            //        {
            //            //Debug.Log("backHit specific use case!");

            //            // Set the ground normal to be the normal of the backHit
            //            states.groundNormal = backHit.normal;
            //        }


            //        //states.isGrounded = true;
            //    }
            //else
            //{
            //    //states.isGrounded = false;
            //}
            //}
            bool front;
            front = isGrounded(states.frontCollider);
            Debug.Log(Time.frameCount + " || Is Grounded Front is: " + front);
            states.isGrounded = (front);
        }

        //Checks to see if the collider is interacting with anything on the default layer '0'
        //https://www.youtube.com/watch?v=vdOFUFMiPDU
        bool isGrounded(CapsuleCollider col)
        {
            //return Physics.CheckBox(new Vector3(col.bounds.center.x, col.bounds.center.y - (col.bounds.size.y - (col.bounds.size.y * 0.5f)), col.bounds.center.z), new Vector3(col.bounds.size.x * 1.5f, col.bounds.size.y * 0.5f, col.bounds.size.z * 1.5f) * 0.5f, col.transform.rotation, groundLayer);
            return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 1.5f, groundLayer);
        }
    }
}
