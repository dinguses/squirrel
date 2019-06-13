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
        Vector3 currentVelocity;
        Vector3 targetVelocity;

        public override void Execute(StateManager states)
        {

        }

        public override void OnEnter(StateManager states)
        {
            states.climbState = StateManager.ClimbState.NONE;
            if(states.isRun)
                states.pauseSpeedHackTimer = true;
        }

        public override void OnUpdate(StateManager states)
        {
            if (states.isColidingInAir)
            {
                if (gravity == 0)
                {
                    gravity = downwardsGravity;
                }

                currentVelocity = states.rigid.velocity;
                targetVelocity = currentVelocity;
                targetVelocity.y = currentVelocity.y - gravity;
                targetVelocity.z = 0;
                targetVelocity.x = 0;
                states.rigid.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * movementTime);
                gravity += gravityAdditive;
            }
            else
            {
                gravity = downwardsGravity;
            }
        }

        public override void OnExit(StateManager states)
        {
            if (!states.isRun)
                states.speedHackRecover = 0.1f;
            if(!states.dashActive)
                states.pauseSpeedHackTimer = false;
        }
    }
}

