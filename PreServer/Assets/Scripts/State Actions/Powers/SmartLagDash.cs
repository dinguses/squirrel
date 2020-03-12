using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{

    [CreateAssetMenu(menuName = "Actions/State Actions/Smart Lag Dash")]
    public class SmartLagDash : StateActions
    {
        public float defaultDist = 10f; //distance the dash normally goes
        public float runDist = 15f; //distance the dash goes when speed hack is activated
        public float time = 0.15f; //time it takes for normal dash to end
        float distance = 5f; //distance the current dash will go, dash will go shorter if a wall is hit
        float totalTime = 0.15f; //time is takes for the dash to end, this value is altered to maintain a consitent speed across every distance
        Vector3 startPos; //starting position of the player
        Vector3 targetPos; //end position of the player
        float t = 0; //timer

        public override void Execute(StateManager sm)
        {

        }

        public override void OnEnter(StateManager states)
        {
            base.OnEnter(states);
            GeneratePath(states);
            t = 0;
        }

        //Will generate a full path for the player to traverse, currently only takes the start and end positions
        void GeneratePath(StateManager states)
        {
            //make a temporary float that will store the totalTime
            float tempTime = 0;
            //Check if speed hack is active and set variables based on status
            if (((PlayerManager)states).isRun)
            {
                distance = runDist;
                tempTime = (runDist / defaultDist) * time; //to get a consistent speed, the time will be larger for longer distances
            }
            else
            {
                distance = defaultDist;
                tempTime = time;
            }
            startPos = states.transform.position;

            //Check if a wall is hit on the path, if it is then modify the distance and time based on the wall hit
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(startPos + (states.transform.up * 0.25f), states.transform.forward, out hitInfo, distance, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                targetPos = hitInfo.point - (states.transform.forward * 2f);
                totalTime = (Vector3.Distance(startPos, targetPos) / distance) * tempTime;
            }
            else
            {
                targetPos = startPos + states.transform.forward * distance;
                totalTime = tempTime;
            }
        }

        public override void OnUpdate(StateManager states)
        {
            base.OnUpdate(states);
            if (t >= totalTime)
                ((PlayerManager)states).dashActive = false;
            Move(states);
        }

        //Moves the player
        void Move(StateManager states)
        {
            t += Time.deltaTime;
            states.transform.position = Vector3.Lerp(startPos, targetPos, t/totalTime);
        }

        public override void OnExit(StateManager states)
        {
            base.OnExit(states);
            ((PlayerManager)states).lagDashCooldown = 1.0f;
        }
    }
}