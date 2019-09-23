using UnityEngine;
using System.Collections;
namespace PreServer
{
    /// <summary>
    /// Rudimentary way to slide the player if they should be sliding
    /// 
    /// Barely works
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Slide Player")]
    public class SlidePlayer : StateActions
    {
        public float gravityAdditive;
        public float downwardsGravity;
        public float movementTime = 10;
        public float slideTime = 0.3f;
        public float movementSpeed;
        float timer = 0;
        float gravity = 0;
        PlayerManager states;

        public override void Execute(StateManager sm)
        {

        }

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;

            timer = 0;
            gravity = downwardsGravity;
            //Debug.Log(Time.frameCount + " || Slide Player On State Enter");
        }
        Vector3 currentVelocity;
        Vector3 targetVelocity;
        public override void OnUpdate(StateManager sm)
        {
            //need to make a rotate on slide action
            currentVelocity = states.rigid.velocity;
            targetVelocity = currentVelocity;
            //currentVelocity.x = 0;
            //This is causing the player move up the high slope, but why?
            targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * movementSpeed * states.slideSpeedMult;
            //float angle = 0;
            //if (states.middle != null)
            //    angle = states.middle.transform.eulerAngles.y;
            //else if (states.front != null)
            //    angle = states.front.transform.eulerAngles.y;
            //else if (states.back != null)
            //    angle = states.back.transform.eulerAngles.y;
            //else
            //    angle = states.mTransform.;
            //targetVelocity.z = (force * Mathf.Cos(angle));
            //targetVelocity.z = (force > 0 ? - Mathf.Abs(targetVelocity.z) : + Mathf.Abs(targetVelocity.z));
            //targetVelocity.x = force;
            //targetVelocity = states.mTransform.TransformDirection(Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime));
            //Debug.Log(Time.frameCount + " || target: " + targetVelocity + " force: " + force + " angle: " + angle);
            if (timer >= slideTime)
            {
                

                targetVelocity -= states.GetSlideDirection() * gravity;

                //Debug.Log(targetVelocity.y);

                states.rigid.velocity = targetVelocity;
                gravity += gravityAdditive;
                //Debug.Log(Time.frameCount + " || velocity: " + states.rigid.velocity + " current: " + currentVelocity + " target: " + targetVelocity);
            }
            else if(!currentVelocity.Equals(targetVelocity))
            {
                targetVelocity.y = currentVelocity.y;
                states.rigid.velocity = targetVelocity;
                //Debug.Log(Time.frameCount + " || velocity: " + states.rigid.velocity + " current: " + currentVelocity + " target: " + targetVelocity);
            }
            timer += states.delta;
        }

        public override void OnExit(StateManager sm)
        {
            timer = 0;
            gravity = downwardsGravity;
            if (states.isGrounded)
            {
                Vector3 slideDirection = Vector3.zero;
                if (GetAngle(states.middle, states.middleNormal))
                {
                    Vector3 forward = Vector3.Cross(states.middleNormal, Vector3.up);
                    slideDirection = Vector3.Cross(forward, states.middleNormal);
                }
                else if (GetAngle(states.front, states.frontNormal))
                {
                    Vector3 forward = Vector3.Cross(states.frontNormal, Vector3.up);
                    slideDirection = Vector3.Cross(forward, states.frontNormal);
                }
                else if (GetAngle(states.back, states.backNormal))
                {
                    Vector3 forward = Vector3.Cross(states.backNormal, Vector3.up);
                    slideDirection = Vector3.Cross(forward, states.backNormal);
                }
                else
                {
                    Vector3 forward = Vector3.Cross(states.BackCastNormal(), Vector3.up);
                    slideDirection = Vector3.Cross(forward, states.BackCastNormal());
                }
                //Only happens on flat ground
                if (slideDirection == Vector3.zero)
                {
                    slideDirection = Vector3.Cross(states.BackCastNormal(), Vector3.up);
                }
                states.rigid.velocity = currentVelocity;
                states.slideMomentum = Vector3.Project(currentVelocity, -slideDirection);
                //states.slideMomentum.y = 0;
                //Debug.DrawLine(states.transform.position + states.transform.up + states.transform.forward, -slideDirection, Color.cyan, 10f);
                Debug.DrawRay(states.transform.position + states.transform.up + states.transform.forward, -slideDirection * 5, Color.red, 5f);
            }
            //if(Vector3.Angle(states.groundNormal, Vector3.up) > 35)
            //{
            //    states.groundNormal = Vector3.zero;
            //}
            //Debug.Log(Time.frameCount + " || Slide Player On State Exit");
        }

        bool GetAngle(GameObject obj, Vector3 normal)
        {
            if (obj == null)
                return false;
            float angle = Vector3.Angle(normal, Vector3.up);
            if (angle < 35)
                return true;
            return false;
        }
    }
}
