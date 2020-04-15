using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{

    [CreateAssetMenu(menuName = "Actions/State Actions/Smart Lag Dash")]
    public class SmartLagDash : StateActions
    {
        //Default Dash Variables
        public float defaultDist = 10f; //distance the dash normally goes
        public float runDist = 15f; //distance the dash goes when speed hack is activated
        public float time = 0.15f; //time it takes for normal dash to end
        public float endMomentum = 20f;//amount of extra momentum applied to the player when the dash ends
        public Vector3 velocityMult = Vector3.one;//velocity mult that will be applied to the player's velocity at the start of the dash
        float distance = 5f; //distance the current dash will go, dash will go shorter if a wall is hit
        float totalTime = 0.15f; //time is takes for the dash to end, this value is altered to maintain a consitent speed across every distance
        float t = 0; //timer - used for debugging purposes now
        PlayerManager player; //reference to the player
        List<Path> path; //path the dash travels on
        int pathIndex = 0; //index of the current path the squirrel will travel on

        //Slow mo Variables
        public float slowMoDuration = 2f; //total time the slow mo effect is applied
        public float slowMoSpeedUpDelay = 1f; //time the speed up occurs
        float prevTimeScale = 0; //used to restore timescale
        float prevFixedDeltaTime = 0; //used to restore fixed delta time
        float slowMoTimer = 0; //times how long the slow mo affect should go
        bool buttonHeld = false; //used to check if the button is still being held down
        float heldDownTimer = 0;
        GameObject blueSquirrel; //the blue (ghosted) squirrel
        bool timeScaleSet = false; //has the timescale been set
        float bsTimer = 0;
        int bsPathIndex = 0;

        //Rotation Variables
        public float rotationSpeed = 16f;//speed the player rotates at
        public float rotationCutoff = 15f;//stops the player from rotating past this amount
        Vector3 prevRotation; //used to check against current rotation to know if the path should be recalculated
        Quaternion startRotation;
        Quaternion minRotation;
        Quaternion maxRotation;
        SmartDashDebugger debugger;
        public override void Execute(StateManager sm)
        {

        }

        //Set variables, reset others, and generate the path
        public override void OnEnter(StateManager states)
        {
            player = (PlayerManager)states; //Set the player
            player.dashInAirCounter++; //Increment the in air counter (NOTE: THIS COUNTER IS NOT USED BUT CAN BE USED LATER IF WE WANT TO LIMIT IN AIR DASHES)

            if (path == null)
                path = new List<Path>();
            t = 0;

            if (debugger == null)
                debugger = FindObjectOfType<SmartDashDebugger>();
            prevFixedDeltaTime = Time.fixedDeltaTime;
            prevTimeScale = Time.timeScale;

            //check if the button is currently being held, since it is a trigger instead of a button, I didn't want to make it too sensitive
            buttonHeld = true/*Input.GetAxisRaw("DashCont") == 1f || Input.GetButton("Dash")*/;

            //If the button isn't held, then set 
            if (!buttonHeld)
                player.lagDashCooldown = 1f;

            timeScaleSet = false;
            slowMoTimer = 0;

            prevRotation = player.transform.forward;
            startRotation = player.transform.rotation;
            Vector3 dir = player.transform.forward;
            if (!player.isGrounded)
                dir.y = 0;
            minRotation = Quaternion.AngleAxis(-rotationCutoff, player.transform.up) * startRotation;
            maxRotation = Quaternion.AngleAxis(rotationCutoff, player.transform.up) * startRotation;

            if (player.climbState != PlayerManager.ClimbState.CLIMBING)
                player.rigid.velocity = Vector3.Scale(velocityMult, player.rigid.velocity);
            else
                player.rigid.velocity = Vector3.zero;

            heldDownTimer = 0;

            base.OnEnter(states);
            GeneratePath(states, true);
        }

        //Will generate a full path for the player to traverse, currently only takes the start and end positions
        void GeneratePath(StateManager states, bool setTime)
        {
            if (setTime)
            {
                //Check if speed hack is active and set variables based on status
                if (((PlayerManager)states).isRun)
                {
                    distance = runDist;
                    totalTime = (runDist / defaultDist) * time; //to get a consistent speed, the time will be larger for longer distances
                }
                else
                {
                    distance = defaultDist;
                    totalTime = time;
                }
            }
            //Get the current direction of the player and ignore vertical direction if in the air
            Vector3 dir = player.transform.forward;
            if (!player.isGrounded && player.climbState == PlayerManager.ClimbState.NONE)
                dir.y = 0;
            Vector3 pos = player.transform.position;
            if (player.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                CheckUnder();
                //Debug.DrawRay(player.transform.position, player.climbHit.normal * 5f, Color.black, 50f);
                if (player.climbHit.transform != null)
                {
                    states.transform.rotation = Quaternion.FromToRotation(player.transform.up, player.climbHit.normal) * player.transform.rotation;
                    dir = states.transform.rotation  * Vector3.forward.normalized;
                    pos = player.climbHit.point;
                }
                CalculatePathClimb(distance, player.climbHit.transform == null ? (player.transform.up.normalized * 0.25f) : (player.climbHit.normal.normalized * 0.25f), dir, pos, totalTime);
            }
            else
                CalculatePath(distance, (player.transform.up * 0.25f), dir, pos, totalTime);
        }

        /// <summary>
        /// Recursive function used to generate a path based on distance
        /// </summary>
        /// <param name="remainingDistance">distance remaining, if this is 0, then the recursive loop breaks</param>
        /// <param name="up">current up direction of the ground, based on previous raycast hit</param>
        /// <param name="dir">current direction of the path, based on previous raycast hit</param>
        /// <param name="start">start position</param>
        /// <param name="remainingTime">time remaining to keep a consistent speed across all paths and make the sum of all paths = total dash time</param>
        void CalculatePath(float remainingDistance, Vector3 up, Vector3 dir, Vector3 start, float remainingTime)
        {
            //Add a new path object
            path.Add(new Path());

            RaycastHit triggerInfo = new RaycastHit();
            Vector3 target;
            //Check if the path will hit any object including triggers
            if (Physics.Raycast(start + up, dir, out triggerInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Collide))
            {
                //Debug.LogError(/*Time.frameCount + */"Smart Lag Dash, triggerInfo.transform.tag: " + triggerInfo.transform.tag);
                //Trigger is hit, check if it is a grind
                if (triggerInfo.transform.tag == "Grind")
                {
                    target = triggerInfo.point - up;
                    path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                    remainingDistance = 0;
                    remainingTime = 0;
                }
                else
                {
                    RaycastHit hitInfo = new RaycastHit();
                    //If a grind has not been hit, then check again ignoring triggers this time
                    //Check if a wall is hit on the path, if it is then modify the distance and time based on the wall hit
                    if (Physics.Raycast(start + up, dir, out hitInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    {
                        float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
                        //if the player hit either a surface that it cannot traverse, a climb, or is in the air
                        if (angle >= 90 || hitInfo.transform.tag == "Climb" || !player.isGrounded)
                        {
                            //move the target point back the length of the squirrel and end the dash
                            target = hitInfo.point - (dir * 2f) - up;
                            path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                            remainingDistance = 0;
                            remainingTime = 0;
                        }
                        else
                        {
                            //end the current path at the hit point and set up the information for the new path
                            target = hitInfo.point - up; //might subtract dir * (length of squirrel) later
                            path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;

                            up = hitInfo.normal * 0.25f;
                            dir = (Quaternion.FromToRotation(player.transform.up, hitInfo.normal) * player.transform.rotation) * Vector3.forward.normalized;

                            //subtract the distance and time from the respective remainings
                            remainingDistance -= hitInfo.distance;
                            remainingTime -= path[path.Count - 1].time;
                        }
                    }
                    else
                    {
                        //If a wall isn't hit, then use up the rest of the distance because ain't nothing gonna stop us baby!!!!
                        path[path.Count - 1].time = Mathf.Abs(remainingTime);
                        target = start + dir * Mathf.Abs(remainingDistance);
                        remainingDistance = 0;
                        remainingTime = 0;
                    }
                }
            }
            else
            {
                //Nothing was hit, use up the rest of the distance because ain't nothing gonna stop us baby!!!!
                path[path.Count - 1].time = Mathf.Abs(remainingTime);
                target = start + dir * Mathf.Abs(remainingDistance);
                remainingDistance = 0;
                remainingTime = 0;
            }
            //Create a new lerper using the start and end positions established
            path[path.Count - 1].lerper = new VectorLerper(start, target);
            //Debug.LogError(start + " " + target);

            //break out of the loop if for some reason the start and end positions are the same (Happened before hasn't been triggered since)
            //Just keep an eye out for this one
            if (path[path.Count - 1].lerper.startVal == path[path.Count - 1].lerper.endVal)
            {
                Debug.LogError("Aborting Smart Dash Pathing Loop: Start and End positions in are the same");
                return;
            }

            //If the distance isn't 0, then keep this party train going
            if (remainingDistance > 0)
            {
                //Debug.LogError(remainingDistance);
                CalculatePath(remainingDistance, up, dir, path[path.Count - 1].lerper.endVal, remainingTime);
            }
        }

        /// <summary>
        /// Recursive function used to generate a path based on distance
        /// </summary>
        /// <param name="remainingDistance">distance remaining, if this is 0, then the recursive loop breaks</param>
        /// <param name="up">current up direction of the ground, based on previous raycast hit</param>
        /// <param name="dir">current direction of the path, based on previous raycast hit</param>
        /// <param name="start">start position</param>
        /// <param name="remainingTime">time remaining to keep a consistent speed across all paths and make the sum of all paths = total dash time</param>
        void CalculatePathClimb(float remainingDistance, Vector3 up, Vector3 dir, Vector3 start, float remainingTime)
        {
            //Add a new path object
            path.Add(new Path());

            //Check under and a straight shot
            RaycastHit triggerInfo = new RaycastHit();
            Vector3 target = Vector3.zero;
            Vector3 tempPos = start;
            RaycastHit underInfo = CheckUnder(remainingDistance, start, up, dir, out tempPos);
            if (Physics.Raycast(start + up, dir, out triggerInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Collide))
            {
                if(underInfo.transform != null)
                {
                    //Under has been hit, figure out which one is closer
                    if (Vector3.Distance(start, GetPoint(underInfo.point, start, tempPos - up)) < triggerInfo.distance)
                    {
                        CheckClimbHit(underInfo, start, tempPos, ref target, ref dir, ref up, ref remainingTime, ref remainingDistance);
                    }
                    else
                    {
                        CheckClimbTrigger(triggerInfo, underInfo, start, tempPos, ref target, ref dir, ref up, ref remainingTime, ref remainingDistance);
                    }
                }
                else
                {
                    CheckClimbTrigger(triggerInfo, underInfo, start, tempPos, ref target, ref dir, ref up, ref remainingTime, ref remainingDistance);
                }
            }
            else
            {
                if(underInfo.transform != null)
                {
                    CheckClimbHit(underInfo, start, tempPos, ref target, ref dir, ref up, ref remainingTime, ref remainingDistance);
                }
                else
                {
                    //If a wall isn't hit, then use up the rest of the distance because ain't nothing gonna stop us baby!!!!
                    path[path.Count - 1].time = Mathf.Abs(remainingTime);
                    target = start + dir * Mathf.Abs(remainingDistance);
                    remainingDistance = 0;
                    remainingTime = 0;
                }
            }

            //Create a new lerper using the start and end positions established
            path[path.Count - 1].lerper = new VectorLerper(start, target);
            //Debug.LogError(start + " " + target);

            //break out of the loop if for some reason the start and end positions are the same (Happened before hasn't been triggered since)
            //Just keep an eye out for this one
            if (path[path.Count - 1].lerper.startVal == path[path.Count - 1].lerper.endVal)
            {
                Debug.LogError("Aborting Smart Dash Pathing Loop: Start and End positions in are the same");
                return;
            }

            //If the distance isn't 0, then keep this party train going
            if (remainingDistance > 0)
            {
                //Debug.LogError(remainingDistance);
                CalculatePathClimb(remainingDistance, up, dir, path[path.Count - 1].lerper.endVal, remainingTime);
            }
            else if(blueSquirrel != null)
            {
                blueSquirrel.transform.rotation = Quaternion.FromToRotation(blueSquirrel.transform.up, up.normalized) * blueSquirrel.transform.rotation;
            }
        }

        private void CheckClimbTrigger(RaycastHit triggerInfo, RaycastHit climbHit, Vector3 start, Vector3 tempPos, ref Vector3 target, ref Vector3 dir, ref Vector3 up, ref float remainingTime, ref float remainingDistance)
        {
            if (triggerInfo.transform.tag == "Grind")
            {
                target = triggerInfo.point - up;
                path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                remainingDistance = 0;
                remainingTime = 0;
            }
            else
            {
                RaycastHit hitInfo = new RaycastHit();
                //If a grind has not been hit, then check again ignoring triggers this time
                //Check if a wall is hit on the path, if it is then modify the distance and time based on the wall hit
                if (Physics.Raycast(start + up, dir, out hitInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    float angle = Vector3.Angle(up, hitInfo.normal);
                    //if the player hit either a surface that it cannot traverse, a climb, or is in the air
                    if (angle >= 90)
                    {
                        //move the target point back the length of the squirrel and end the dash
                        target = hitInfo.point - (dir * 2f) - up;
                        path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                        remainingDistance = 0;
                        remainingTime = 0;
                    }
                    else
                    {
                        //end the current path at the hit point and set up the information for the new path
                        target = hitInfo.point - up; //might subtract dir * (length of squirrel) later
                        path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;

                        up = hitInfo.normal * 0.25f;
                        dir = (Quaternion.FromToRotation(player.transform.up, hitInfo.normal) * player.transform.rotation) * Vector3.forward.normalized;

                        //subtract the distance and time from the respective remainings
                        remainingDistance -= hitInfo.distance;
                        remainingTime -= path[path.Count - 1].time;
                    }
                }
                else
                {
                    if (climbHit.transform == null)
                    {
                        //If a wall isn't hit, then use up the rest of the distance because ain't nothing gonna stop us baby!!!!
                        path[path.Count - 1].time = Mathf.Abs(remainingTime);
                        target = start + dir * Mathf.Abs(remainingDistance);
                        remainingDistance = 0;
                        remainingTime = 0;
                    }
                    else
                    {
                        CheckClimbHit(climbHit, start, tempPos, ref target, ref dir, ref up, ref remainingTime, ref remainingDistance);
                    }
                }
            }
        }

        private void CheckClimbHit(RaycastHit climbHit, Vector3 start, Vector3 tempPos, ref Vector3 target, ref Vector3 dir, ref Vector3 up, ref float remainingTime, ref float remainingDistance)
        {
            if (climbHit.normal == up.normalized && climbHit.transform.tag == "Climb")
            {
                path[path.Count - 1].time = Mathf.Abs(remainingTime);
                target = start + dir * Mathf.Abs(remainingDistance);
                remainingDistance = 0;
                remainingTime = 0;
            }
            else
            {
                if (climbHit.transform.tag == "Climb")
                {
                    //end the current path at the hit point and set up the information for the new path
                    target = GetPoint(climbHit.point, start, tempPos - up); //might subtract dir * (length of squirrel) later
                    path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;

                    up = climbHit.normal * 0.25f;
                    dir = (Quaternion.FromToRotation(player.transform.up, climbHit.normal) * player.transform.rotation) * Vector3.forward.normalized;

                    //subtract the distance and time from the respective remainings
                    remainingDistance -= Vector3.Distance(start, target);
                    remainingTime -= path[path.Count - 1].time;
                }
                else
                {
                    //move the target point back the length of the squirrel and end the dash
                    target = GetPoint(climbHit.point, start, tempPos - up) - dir;
                    path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                    remainingDistance = 0;
                    remainingTime = 0;
                }
            }
        }

        RaycastHit CheckUnder(float distance, Vector3 start, Vector3 up, Vector3 dir, out Vector3 temp)
        {
            //Nothing in that direction will hit us now check if there is anything underneath
            float tempRemaining = distance;
            temp = start + up;
            RaycastHit underInfo = new RaycastHit();
            //Vector3 newDir = /*(-dir * 0.15f)*/ - up;
            while (tempRemaining > 0)
            {
                if (tempRemaining >= 0.1f)
                {
                    temp += dir.normalized * 0.1f;
                    tempRemaining -= 0.1f;
                }
                else
                {
                    temp += dir.normalized * tempRemaining;
                    tempRemaining = 0;
                }
                //Debug.LogError(tempPos + " " + dir.normalized);
                Debug.DrawRay(temp, -up.normalized * 0.5f, Color.red);
                if (Physics.Raycast(temp, -up, out underInfo, 0.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    if (underInfo.normal.normalized != up.normalized || underInfo.transform.tag != "Climb")
                    {
                        return underInfo;
                    }
                }
                else
                {
                    if(Physics.Raycast(temp - up * 2f, -dir, out underInfo, 0.15f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    {
                        if (underInfo.normal.normalized != up.normalized || underInfo.transform.tag != "Climb")
                        {
                            return underInfo;
                        }
                    }
                    //else if (Physics.Raycast(temp - up * 0.5f, -up, out underInfo, 0.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    //{
                    //    if (underInfo.normal.normalized != up.normalized || underInfo.transform.tag != "Climb")
                    //    {
                    //        return underInfo;
                    //    }
                    //}
                }
            }
            return underInfo;
        }

        void CheckUnder()
        {
            Vector3 underOrigin = player.transform.position;
            underOrigin += player.transform.forward + player.transform.forward / 1.7f + (player.transform.up * 0.5f);
            // Dir represents the downward direction
            Vector3 dir = -player.transform.forward + (-player.transform.up);
            RaycastHit under = new RaycastHit();
            if (Physics.Raycast(underOrigin, dir, out under, 1.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                player.climbHit = under;
            }
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }

        public override void OnUpdate(StateManager states)
        {
            base.OnUpdate(states);
            if(heldDownTimer <= .15f && buttonHeld)
            {
                heldDownTimer += Time.unscaledDeltaTime;
                if (Input.GetAxisRaw("DashCont") != 1f && !Input.GetButton("Dash"))
                {
                    buttonHeld = false;
                }
            }
            else
            {
                if (buttonHeld)
                {
                    if (!timeScaleSet)
                    {
                        Time.timeScale = 0.05f;
                        timeScaleSet = true;
                    }
                    else
                    {
                        if (slowMoTimer >= slowMoSpeedUpDelay)
                        {
                            Time.timeScale = Mathf.Lerp(0.05f, prevTimeScale, (slowMoTimer - slowMoSpeedUpDelay) / (slowMoDuration - slowMoSpeedUpDelay));
                        }
                    }
                    slowMoTimer += Time.unscaledDeltaTime;
                    Time.fixedDeltaTime = Time.timeScale * 0.0167f;
                    player.lagDashCooldown = slowMoTimer / slowMoDuration;
                    if (player.lagDashCooldown >= 1f)
                        player.lagDashCooldown = 1f;
                    if ((Input.GetAxisRaw("DashCont") != 1f && !Input.GetButton("Dash")) || player.lagDashCooldown >= 1f)
                    {
                        buttonHeld = false;
                        timeScaleSet = false;
                        player.lagDashCooldown = 1f;
                        ResetPath();
                    }
                    else if (!blueSquirrel)
                    {
                        blueSquirrel = Instantiate(Resources.Load<GameObject>("BlueSquirrel"), path[path.Count - 1].lerper.endVal, player.transform.rotation);
                        //blueSquirrel.transform.parent = player.transform;
                    }

                    if (player.transform.position != path[0].lerper.startVal || player.transform.forward != prevRotation)
                    {
                        while (path.Count > 0)
                            path.RemoveAt(0);
                        GeneratePath(player, false);
                        if (blueSquirrel)
                            blueSquirrel.transform.position = path[path.Count - 1].lerper.endVal;
                    }
                    Rotate(player);
                    //if(blueSquirrel)
                    //    MoveGhost();
                }
                else
                {
                    if (!timeScaleSet)
                    {
                        Time.timeScale = prevTimeScale;
                        Time.fixedDeltaTime = prevFixedDeltaTime;
                        timeScaleSet = true;
                        player.rigid.useGravity = false;
                        player.rigid.velocity = Vector3.zero;
                        player.rigid.drag = 0;
                    }
                    if (blueSquirrel)
                    {
                        Destroy(blueSquirrel);
                        blueSquirrel = null;
                    }
                    Move(states);
                }
                if (pathIndex >= path.Count/*t >= totalTime*/)
                    player.dashActive = false;
            }
            DrawPath();
        }

        void DrawPath()
        {
            for(int i = 0; i < path.Count; ++i)
            {
                Debug.DrawLine(path[i].lerper.startVal, path[i].lerper.endVal, Color.black);
            }
        }

        //Moves the player
        void Move(StateManager states)
        {
            t += Time.deltaTime;
            //If there there is more of the path then continue
            if (pathIndex < path.Count)
            {
                //Update the time in the current path and get the position it calculated
                path[pathIndex].Update(Time.deltaTime);
                states.transform.position = path[pathIndex].lerper.GetValue();

                //Debug.LogError("Path time: " + path[pathIndex].time + " time passed: " + t + " lerpVal: " + path[pathIndex].lerper.GetLerpVal());

                //If the current path is complete, then move onto the next one
                if (path[pathIndex].lerper.done)
                {
                    //Debug.LogError(path[pathIndex].time + " " + path[pathIndex].GetRemainder());

                    ++pathIndex;
                    //Add any remaining time from the previous path into the current so we don't go an extra frame or 2 over
                    if (pathIndex < path.Count)
                        path[pathIndex].lerper.Update(path[pathIndex - 1].GetRemainder() / path[pathIndex].time);
                }
            }
        }

        void Rotate(StateManager states)
        {
            prevRotation = player.transform.forward;
            var cam = Camera.main.transform;

            float h = player.movementVariables.horizontal;
            float v = player.movementVariables.vertical;

            Vector3 targetDir = cam.forward * v;
            targetDir += cam.right * h;

            targetDir.Normalize();
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = player.transform.forward;

            player.movementVariables.moveDirection = targetDir;

            Quaternion tr = Quaternion.LookRotation(targetDir, player.groundNormal);
            if(Quaternion.Angle(tr, startRotation) > rotationCutoff)
            {
                if (Quaternion.Angle(tr, minRotation) <= Quaternion.Angle(tr, maxRotation))
                    tr = minRotation;
                else
                    tr = maxRotation;
            }
            Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, Time.unscaledDeltaTime * player.movementVariables.moveAmount * rotationSpeed);
            
            player.transform.rotation = targetRotation;
            if (blueSquirrel != null)
                blueSquirrel.transform.rotation = targetRotation;
        }

        //Moves the player
        void MoveGhost()
        {
            if (bsPathIndex >= path.Count)
            {
                bsPathIndex = 0;
                bsTimer = 0;
            }
            bsTimer += Time.unscaledDeltaTime * 0.25f;

            //Update the time in the current path and get the position it calculated
            path[bsPathIndex].Update(bsTimer);
            blueSquirrel.transform.position = path[bsPathIndex].lerper.GetValue();

            //Debug.LogError("Path time: " + path[pathIndex].time + " time passed: " + t + " lerpVal: " + path[pathIndex].lerper.GetLerpVal());

            //If the current path is complete, then move onto the next one
            if (path[bsPathIndex].lerper.done)
            {
                //Debug.LogError(path[pathIndex].time + " " + path[pathIndex].GetRemainder());

                ++bsPathIndex;
                //Add any remaining time from the previous path into the current so we don't go an extra frame or 2 over
                if (bsPathIndex < path.Count)
                    path[bsPathIndex].lerper.Update(path[bsPathIndex - 1].GetRemainder() / path[bsPathIndex].time);
                else
                {
                    bsPathIndex = 0;
                    ResetPath();
                }
                bsTimer = 0;
            }

        }

        void ResetPath()
        {
            for(int i = 0; i < path.Count; ++i)
            {
                path[i].Reset();
            }
        }

        public override void OnExit(StateManager states)
        {
            int pathsCompleted = 0;
            for (int i = 0; i < path.Count; ++i)
            {
                pathsCompleted += path[i].lerper.done ? 1 : 0;
            }
            Debug.Log("Completed " + pathsCompleted + " path(s) in " + t + " seconds, total time: " + totalTime + " slowMoTimer: " + slowMoTimer);
            base.OnExit(states);
            while (path.Count > 0)
                path.RemoveAt(0);
            pathIndex = 0;
            player.lagDashCooldown = 1.0f;
            if (player.climbState == PlayerManager.ClimbState.NONE)
            {
                player.rigid.velocity = states.transform.forward * endMomentum;
                player.rigid.useGravity = true;
            }
        }
    }

    public class Path
    {
        public VectorLerper lerper;
        public float time;
        float t = 0;
        float remainder;
        public void Update(float amount)
        {
            remainder = (amount / time) + lerper.GetLerpVal();
            if (remainder >= 1)
                remainder = (remainder - 1f) * time;
            lerper.Update(amount / time);
        }

        public float GetRemainder()
        {
            return remainder;
        }

        public void Reset()
        {
            remainder = 0;
            lerper.Reset();
        }
    }

    public class VectorLerper
    {
        Vector3 value = Vector3.zero;
        Vector3 _startVal = Vector3.zero;
        public Vector3 startVal
        {
            get { return _startVal; }
        }
        Vector3 _endVal = Vector3.zero;
        public Vector3 endVal
        {
            get { return _endVal; }
            set
            {
                lerpVal = 0;
                _startVal = _endVal;
                _endVal = value;
            }
        }

        float lerpVal = 0;
        public float GetLerpVal() { return lerpVal; }
        public bool done { get { return value == endVal; } }
        public VectorLerper(Vector3 s, Vector3 e)
        {
            _startVal = s;
            value = s;
            _endVal = e;
        }

        public void Reset(Vector3 s, Vector3 e)
        {
            _startVal = s;
            _endVal = e;
        }

        public void Reset()
        {
            lerpVal = 0;
            value = startVal;
        }

        public void Update(float amount)
        {
            if (value != endVal)
            {
                lerpVal += amount;
                lerpVal = Mathf.Clamp(lerpVal, 0, 1);
                value = Vector3.Lerp(startVal, endVal, lerpVal);
            }
        }

        public Vector3 GetValue()
        {
            return value;
        }
    }

    public class IntLerper
    {
        int value = 0;
        int startVal = 0;
        int _endVal = 0;
        public int endVal
        {
            get { return _endVal; }
            set
            {
                lerpVal = 0;
                startVal = _endVal;
                _endVal = value;
            }
        }

        float lerpVal = 0;
        public bool done { get { return value == endVal; } }
        public IntLerper(int s, int e)
        {
            startVal = s;
            value = s;
            endVal = e;
        }

        public void Reset(int s, int e)
        {
            startVal = s;
            endVal = e;
        }

        public void Update(float amount)
        {
            if (value != endVal)
            {
                lerpVal += amount;
                lerpVal = Mathf.Clamp(lerpVal, 0, 1);
                value = Mathf.RoundToInt(Mathf.Lerp(startVal, endVal, lerpVal));
            }
        }

        public int GetValue()
        {
            return value;
        }
    }


}
