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

        public override void Execute(StateManager states)
        {

        }

        public override void OnEnter(StateManager states)
        {
            timer = 0;
            gravity = downwardsGravity;
            //Debug.Log(Time.frameCount + " || Slide Player On State Enter");
        }
        Vector3 currentVelocity;
        Vector3 targetVelocity;
        public override void OnUpdate(StateManager states)
        {
            //need to make a rotate on slide action
            currentVelocity = states.rigid.velocity;
            targetVelocity = currentVelocity;
            //currentVelocity.x = 0;
            //This is causing the player move up the high slope, but why?
            targetVelocity.x = states.mTransform.forward.x * states.movementVariables.moveAmount * movementSpeed;
            if (timer >= slideTime)
            {
                

                targetVelocity.y = currentVelocity.y - gravity;

                //Debug.Log(targetVelocity.y);

                states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);
                gravity += gravityAdditive;
                //Debug.Log(Time.frameCount + " || velocity: " + states.rigid.velocity + " current: " + currentVelocity + " target: " + targetVelocity);
            }
            else if(!currentVelocity.Equals(targetVelocity))
            {
                states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);
                //Debug.Log(Time.frameCount + " || velocity: " + states.rigid.velocity + " current: " + currentVelocity + " target: " + targetVelocity);
            }
            timer += states.delta;
        }

        public override void OnExit(StateManager states)
        {
            timer = 0;
            gravity = downwardsGravity;
            //Debug.Log(Time.frameCount + " || Slide Player On State Exit");
        }
    }
}
