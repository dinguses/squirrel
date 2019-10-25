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
        public float dashSpeed = 40f;
        public float dashTime = 0.15f;
        bool dash = false;
        float timer = 0;
        bool dashActivated = false;
        PlayerManager states;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            base.OnEnter(states);

            states.dashInAirCounter = 0;
            dash = false;
            timer = 0;
            dashActivated = false;
        }

        public override void Execute(StateManager states)
        {

        }

        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);

            if (timer <= 0 && states.dashActive && states.CanDash() && !dashActivated)
            {
                states.anim.CrossFade(states.hashes.squ_dash, 0.01f);
                states.anim.SetBool(states.hashes.isDashing, true);

                //Debug.Log("Adding velocity 9");
                states.rigid.velocity = states.transform.forward * dashSpeed;
                if (states.isRun)
                {
                    timer = 0.225f;
                    states.speedHackAmount -= 0.2f;
                    if (states.speedHackAmount <= 0)
                    {
                        states.speedHackAmount = 0;
                        states.runRanOut = true;
                    }
                }
                else
                {
                    timer = 0.15f;
                }

                states.anim.SetLayerWeight(1, 0);
                dashActivated = true;
            }

            if (!states.dashActive)
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
                if (states.grindCenter != null)
                {
                    // Move Player towards center should they not be on it
                    Vector3 grindCenterClosestPoint = states.grindCenter.ClosestPoint(states.mTransform.position);

                    states.rigid.position = Vector3.Lerp(states.rigid.position, grindCenterClosestPoint, Time.deltaTime * 10 * (states.groundSpeedMult * 2));
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

            timer -= Time.deltaTime;
            if (timer < 0 && states.dashActive && dashActivated)
            {
                states.dashActive = false;
                states.rigid.velocity = Vector3.zero;
                states.anim.SetBool(states.hashes.isDashing, false);
                states.anim.SetLayerWeight(1, 1);
                //Debug.Log("Dash over");
                states.lagDashCooldown = 1.0f;
                dashActivated = false;
                states.playerMesh.gameObject.SetActive(true);
            }
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);
            //states.rigid.velocity = Vector3.zero;
            if (states.dashActive)
            {
                states.dashActive = false;
                states.lagDashCooldown = 1.0f;
                states.anim.SetBool(states.hashes.isDashing, false);
                states.anim.SetLayerWeight(1, 1);
            }
        }
    }
}
