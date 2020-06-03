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
        float timer;
        bool dashActivated;
        PlayerManager states;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            base.OnEnter(states);

            states.dashInAirCounter = 0;
            timer = 0;
            dashActivated = false;

            // Move towards the grind center (In case you got misplaced a bit after 180ing)
            Vector3 grindCenterClosestPoint = GetPoint(states.rigid.position, states.facingPoint, states.behindPoint);
            states.rigid.position = Vector3.Lerp(states.rigid.position, grindCenterClosestPoint, Time.deltaTime * 100f);
        }

        public override void Execute(StateManager states)
        {

        }

        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);

            // TODO: This is old dash, need to upgrade it to new dash
            if (timer <= 0 && states.dashActive && states.CanDash() && !dashActivated)
            {
                states.anim.CrossFade(states.hashes.squ_dash, 0.01f);
                states.anim.SetBool(states.hashes.isDashing, true);

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

                dashActivated = true;
            }

            // If not dashing
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

                // Move Player towards center should they not be on it
                Vector3 grindCenterClosestPoint = GetPoint(states.rigid.position, states.facingPoint, states.behindPoint);
                states.rigid.position = Vector3.Lerp(states.rigid.position, grindCenterClosestPoint, Time.deltaTime * 10f * (states.groundSpeedMult * 2));

                // If you haven't fully adjusted to the grind center initially
                if (!states.doneAdjustingGrind)
                {
                    Debug.Log("cc - " + Vector3.Distance(states.rigid.position, grindCenterClosestPoint));

                    // If position is close enough to grind center, adjusting is done
                    if (Vector3.Distance(states.rigid.position, grindCenterClosestPoint) < .1f)
                    {
                        Debug.Log("Done adjusting!");
                        states.doneAdjustingGrind = true;
                    }
                }
            }

            // Decrement timer 
            timer -= Time.deltaTime;

            // If dash time is over, dash is no longer active, and initial dash bool has been hit
            if (timer < 0 && states.dashActive && dashActivated)
            {
                states.dashActive = false;
                states.rigid.velocity = Vector3.zero;
                states.anim.SetBool(states.hashes.isDashing, false);
                states.lagDashCooldown = 1.0f;
                dashActivated = false;
                states.playerMesh.gameObject.SetActive(true);
            }
        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);

            states.frontCollider.enabled = true;

            if (states.dashActive)
            {
                states.dashActive = false;
                states.lagDashCooldown = 1.0f;
                states.anim.SetBool(states.hashes.isDashing, false);
                states.doneAdjustingGrind = false;
            }
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }
    }
}
