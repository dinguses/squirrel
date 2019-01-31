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
        public float downwardsGravity;
        public float movementTime = 10;

        public override void Execute(StateManager states)
        {
            Vector3 currentVelocity = states.rigid.velocity;
            Vector3 targetVelocity = currentVelocity;

            targetVelocity.y = currentVelocity.y -= downwardsGravity;

            //Debug.Log(targetVelocity.y);

            states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);
        }
    }
}
