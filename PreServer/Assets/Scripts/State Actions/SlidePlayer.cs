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

        public override void OnUpdate(StateManager states)
        {
            if (timer >= slideTime)
            {
                Vector3 currentVelocity = states.rigid.velocity;
                Vector3 targetVelocity = currentVelocity;

                targetVelocity.y = currentVelocity.y - gravity;

                //Debug.Log(targetVelocity.y);

                states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);
                gravity += gravityAdditive;
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
