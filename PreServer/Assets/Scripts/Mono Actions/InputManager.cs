using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/Mono Actions/Input Manager")]
    public class InputManager : Action
    {
        public FloatVariable horizontal;
        public FloatVariable vertical;
        public BoolVariable jump;
        public BoolVariable restart;
        public BoolVariable run;
        public FloatVariable runAxis;
        public BoolVariable zoom;
        public BoolVariable dash;
        public FloatVariable dashAxis;

        public StateManagerVariable playerStates;
        public ActionBatch inputUpdateBatch;

        public override void Execute()
        {
            inputUpdateBatch.Execute();

            if (playerStates.value != null)
            {
                playerStates.value.movementVariables.horizontal = horizontal.value;
                playerStates.value.movementVariables.vertical = vertical.value;

                float moveAmount = 0f;
                float moveAmountClamped = Mathf.Clamp01(Mathf.Abs(horizontal.value) + Mathf.Abs(vertical.value));

                if (moveAmountClamped > .1f)
                    moveAmount = moveAmountClamped;

                playerStates.value.movementVariables.moveAmount = moveAmount;

                playerStates.value.isJumping = jump.value;
                playerStates.value.isRestart = restart.value;
                if (playerStates.value.CanRun())
                {
                    if (!playerStates.value.isRun && (playerStates.value.currentState.name == "On Air" || playerStates.value.currentState.name == "LagDash"))
                        playerStates.value.isRun = false;
                    else
                        playerStates.value.isRun = (playerStates.value.isRun ? (run.value || runAxis.value > 0) : ((run.value || runAxis.value > 0)/* && playerStates.value.speedHackAmount > 0.25f*/));
                }
                else
                    playerStates.value.isRun = false;

                if (!playerStates.value.dashActive && playerStates.value.CanDash())
                    playerStates.value.dashActive = (dash.value || dashAxis.value > 0);
            }
        }
    }
}
