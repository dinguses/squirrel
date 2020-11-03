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
        float distanceFactor = 0;
        float totalTime = 0.15f; //time is takes for the dash to end, this value is altered to maintain a consitent speed across every distance
        float t = 0; //timer - used for debugging purposes now
        PlayerManager player; //reference to the player
        List<Path> path; //path the dash travels on
        int pathIndex = 0; //index of the current path the squirrel will travel on
        bool fullDistanceUsed = false;

        //Slow mo Variables
        public float slowMoDuration = 2f; //total time the slow mo effect is applied
        public float slowMoSpeedUpDelay = 1f; //time the speed up occurs
        float prevTimeScale = 0; //used to restore timescale
        float prevFixedDeltaTime = 0; //used to restore fixed delta time
        float slowMoTimer = 0; //times how long the slow mo affect should go
        bool buttonHeld = false; //used to check if the button is still being held down
        float heldDownTimer = 0;
        bool timeScaleSet = false; //has the timescale been set
        bool pathChanged = false;


        //Rotation Variables
        public float rotationSpeed = 16f;//speed the player rotates at
        public float rotationCutoff = 15f;//stops the player from rotating past this amount
        public float dashRotationSpeed = 16f;//speed the player rotates at
        Quaternion prevRotation; //used to check against current rotation to know if the path should be recalculated
        Quaternion startRotation;
        Quaternion minRotation;
        Quaternion maxRotation;
        SmartDashDebugger debugger;

        //Ghost Squirrel Variables
        GameObject blueSquirrel; //the blue (ghosted) squirrel
        GameObject bsTrail;
        TrailRenderer trail;
        LineRenderer line;
        int bsPathIndex = 0;
        float currentBSPathTime = 0;
        bool showGhost = false;
        float ghostTimer = 0;
        public float flickerTime = 0.175f;
        public float ghostSpeed = 0.3f;
        public GhostDisplayMode gdm;
        public enum GhostDisplayMode { On, Flicker, Max }
        SkinnedMeshRenderer bsRend;
        float rendTime = 0;

        public override void Execute(StateManager sm)
        {

        }

        //Set variables, reset others, and generate the path
        public override void OnEnter(StateManager states)
        {
            player = (PlayerManager)states; //Set the player
            player.dashInAirCounter++; //Increment the in air counter (NOTE: THIS COUNTER IS NOT USED BUT CAN BE USED LATER IF WE WANT TO LIMIT IN AIR DASHES)
            if (camera == null)
            {
                camera = Camera.main.transform.parent.GetComponent<CameraManager>();
            }

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

            prevRotation = player.transform.rotation;
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
            bsPathIndex = 0;
            rendTime = 0;
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
            fullDistanceUsed = false;
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
                    dir = states.transform.rotation * Vector3.forward.normalized;
                    //pos = player.climbHit.point;
                    pos = GetPoint(player.transform.position, player.climbHit.point, player.climbHit.point - (dir * distance));
                    player.transform.position = pos;
                }
                //Debug.DrawRay(pos, dir, Color.red, 5f);

                CalculatePathClimb(distance, player.climbHit.transform == null ? (player.transform.up.normalized * 0.45f) : (player.climbHit.normal.normalized * 0.45f), dir, pos, totalTime, states.transform.rotation);
            }
            else
                CalculatePath(distance, (player.transform.up * 0.45f), dir, pos, totalTime, states.transform.rotation);

            if (fullDistanceUsed || player.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                //Apparently safety net always needs to be active for a climb or just always
                //That's so annoying!
                SafetyNet();
                fullDistanceUsed = false;
            }

            distanceFactor = 0;
            for (int i = 0; i < path.Count; ++i)
            {
                distanceFactor += Vector3.Distance(path[i].lerper.startVal, path[i].lerper.endVal);
            }
            distanceFactor /= distance;
            if (path.Count > 0)
            {
                ((PlayerManager)states).path = path;
                ((PlayerManager)states).drawPath = true;
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
        void CalculatePath(float remainingDistance, Vector3 up, Vector3 dir, Vector3 start, float remainingTime, Quaternion startRot)
        {
            //Add a new path object
            path.Add(new Path());
            RaycastHit triggerInfo = new RaycastHit();
            Vector3 target;
            path[path.Count - 1].slerper = new QuaternionSlerper(startRot, Quaternion.FromToRotation(player.transform.up, up.normalized) * player.transform.rotation);
            path[path.Count - 1].up = up.normalized;
            //Check if the path will hit any object including triggers
            //CHANGED TO SPHERECAST
            //IMPORTANT NOTE, ONLY CHECKING MIDDLE AND NOT BOTTOM, BOTTOM OF SQUIRREL IS WHAT MATTERS
            if (Physics.SphereCast(start + up, 0.375f, dir, out triggerInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Collide))
            {
                ConvertHitInfo(ref triggerInfo, start + up, dir, up.normalized);
                //Debug.LogError(/*Time.frameCount + */"Smart Lag Dash, triggerInfo.transform.tag: " + triggerInfo.transform.tag);
                //Trigger is hit, check if it is a grind
                if (triggerInfo.transform.tag == "Grind")
                {
                    target = triggerInfo.point - (up.normalized * 0.375f);
                    path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                    remainingDistance = 0;
                    remainingTime = 0;
                }
                else
                {
                    RaycastHit hitInfo = new RaycastHit();
                    //If a grind has not been hit, then check again ignoring triggers this time
                    //Check if a wall is hit on the path, if it is then modify the distance and time based on the wall hit
                    //CHANGED TO SPHERECAST
                    if (Physics.SphereCast(start + up, 0.375f, dir, out hitInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    {
                        ConvertHitInfo(ref hitInfo, start + up, dir, up.normalized);
                        float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
                        //Debug.Log(angle);
                        //if the player hit either a surface that it cannot traverse, a climb, or is in the air
                        if (angle >= 46 || hitInfo.transform.tag == "Climb" || !player.isGrounded)
                        {
                            //move the target point back the length of the squirrel and end the dash
                            target = hitInfo.point - (dir * 2f) - (up.normalized * 0.375f);
                            path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                            remainingDistance = 0;
                            remainingTime = 0;
                        }
                        else
                        {
                            //end the current path at the hit point and set up the information for the new path
                            target = hitInfo.point - (up.normalized * 0.375f); //might subtract dir * (length of squirrel) later
                            path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;

                            up = hitInfo.normal * 0.45f;
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
                        fullDistanceUsed = true;
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
                fullDistanceUsed = true;
            }
            //Create a new lerper using the start and end positions established
            if (path.Count == 0)
                path[path.Count - 1].lerper = new VectorLerper(start + (up.normalized * 0.075f), target);
            else
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
                CalculatePath(remainingDistance, up, dir, path[path.Count - 1].lerper.endVal, remainingTime, path[path.Count - 1].slerper.endVal);
            }
        }

        void ConvertHitInfo(ref RaycastHit hit, Vector3 start, Vector3 dir, Vector3 up)
        {
            //Debug.DrawLine(start, hit.point, Color.green, 5f);
            RaycastHit secondHit = new RaycastHit();
            if (Physics.Linecast(start, hit.point + dir * 0.1f - up * 0.025f, out secondHit, Layers.ignoreLayersController, QueryTriggerInteraction.Collide))
            {
                hit.normal = secondHit.normal;
                //Debug.LogError("Changing normals");
            }
            //Debug.DrawRay(hit.point, hit.normal * 5f, Color.green, 5f);
            Vector3 hitDir = hit.point - start;
            //Debug.DrawRay(player.transform.position, dir * 5f, Color.green, 5f);
            if (hitDir != dir)
            {
                dir = Vector3.Project(hitDir, dir);
                hit.point = start + dir;
                //Debug.LogError("Changing dir");
            }
            //Debug.DrawRay(player.transform.position, dir * 5f, Color.cyan, 5f);
            //Debug.DrawRay(hit.point, hit.normal * 5f, Color.cyan, 5f);
        }

        /// <summary>
        /// Recursive function used to generate a path based on distance
        /// </summary>
        /// <param name="remainingDistance">distance remaining, if this is 0, then the recursive loop breaks</param>
        /// <param name="up">current up direction of the ground, based on previous raycast hit</param>
        /// <param name="dir">current direction of the path, based on previous raycast hit</param>
        /// <param name="start">start position</param>
        /// <param name="remainingTime">time remaining to keep a consistent speed across all paths and make the sum of all paths = total dash time</param>
        void CalculatePathClimb(float remainingDistance, Vector3 up, Vector3 dir, Vector3 start, float remainingTime, Quaternion startRot)
        {
            //Add a new path object
            path.Add(new Path());
            path[path.Count - 1].slerper = new QuaternionSlerper(startRot, Quaternion.FromToRotation(player.transform.up, up.normalized) * player.transform.rotation);
            path[path.Count - 1].up = up.normalized;

            //Check under and a straight shot
            RaycastHit triggerInfo = new RaycastHit();
            Vector3 target = Vector3.zero;
            Vector3 tempPos = start;
            RaycastHit underInfo = CheckUnder(remainingDistance, start, up, dir, out tempPos);
            if (Physics.SphereCast(start + up, 0.375f, dir, out triggerInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Collide))
            {
                ConvertHitInfo(ref triggerInfo, start + up, dir, up.normalized);
                if (underInfo.transform != null)
                {
                    //Under has been hit, figure out which one is closer
                    if (Vector3.Distance(start, GetPoint(underInfo.point, start, tempPos - (up.normalized * 0.25f))) < triggerInfo.distance)
                    {
                        //Debug.LogError("CLIMB HIT CLOSER || CLIMB HIT: " + Vector3.Distance(start, GetPoint(underInfo.point, start, tempPos - (up.normalized * 0.25f))) + " || TRIGGER HIT: " + triggerInfo.distance);
                        //Debug.DrawRay(tempPos - (up.normalized * 0.25f), underInfo.normal, Color.yellow,50);
                        //Debug.DrawRay(triggerInfo.point, triggerInfo.normal, Color.red, 50);
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
                if (underInfo.transform != null)
                {
                    CheckClimbHit(underInfo, start, tempPos, ref target, ref dir, ref up, ref remainingTime, ref remainingDistance);
                }
                else
                {
                    //If a wall isn't hit, then use up the rest of the distance because ain't nothing gonna stop us baby!!!!
                    //path[path.Count - 1].time = Mathf.Abs(remainingTime);
                    target = tempPos - (up.normalized * 0.25f);//start + dir * Mathf.Abs(remainingDistance);
                    path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
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
                CalculatePathClimb(remainingDistance, up, dir, path[path.Count - 1].lerper.endVal, remainingTime, path[path.Count - 1].slerper.endVal);
            }
        }

        private void CheckClimbTrigger(RaycastHit triggerInfo, RaycastHit climbHit, Vector3 start, Vector3 tempPos, ref Vector3 target, ref Vector3 dir, ref Vector3 up, ref float remainingTime, ref float remainingDistance)
        {
            if (triggerInfo.transform.tag == "Grind")
            {
                target = triggerInfo.point - (up.normalized * 0.375f);
                path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                remainingDistance = 0;
                remainingTime = 0;
            }
            else
            {
                RaycastHit hitInfo = new RaycastHit();
                //If a grind has not been hit, then check again ignoring triggers this time
                //Check if a wall is hit on the path, if it is then modify the distance and time based on the wall hit
                if (Physics.SphereCast(start + up, 0.375f, dir, out hitInfo, remainingDistance, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    ConvertHitInfo(ref hitInfo, start + up, dir, up.normalized);
                    float angle = Vector3.Angle(up, hitInfo.normal);
                    //if the player hit either a surface that it cannot traverse, a climb, or is in the air
                    if (angle >= 90 && hitInfo.transform.tag != "Climb" || hitInfo.transform.tag != "Climb")
                    {
                        //move the target point back the length of the squirrel and end the dash
                        target = hitInfo.point - (dir * 2f) - (up.normalized * 0.375f);
                        path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                        remainingDistance = 0;
                        remainingTime = 0;
                    }
                    else
                    {
                        //end the current path at the hit point and set up the information for the new path
                        target = hitInfo.point - (up.normalized * 0.375f); //might subtract dir * (length of squirrel) later
                        path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;

                        up = hitInfo.normal * 0.45f;
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
                        //path[path.Count - 1].time = Mathf.Abs(remainingTime);
                        target = tempPos - (up.normalized * 0.25f)/* - (dir * 1.75f)*/;//start + dir * Mathf.Abs(remainingDistance);
                        path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
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
                //path[path.Count - 1].time = Mathf.Abs(remainingTime);
                //target = start + dir * Mathf.Abs(remainingDistance);
                target = tempPos - (up.normalized * 0.25f)/* - (dir * 1.75f)*/;//start + dir * Mathf.Abs(remainingDistance);
                path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;
                remainingDistance = 0;
                remainingTime = 0;
            }
            else
            {
                if (climbHit.transform.tag == "Climb")
                {
                    //end the current path at the hit point and set up the information for the new path
                    target = GetPoint(climbHit.point, start, tempPos - (up.normalized * 0.25f)); //might subtract dir * (length of squirrel) later
                    path[path.Count - 1].time = (Vector3.Distance(start, target) / distance) * totalTime;

                    up = climbHit.normal * 0.45f;
                    dir = (Quaternion.FromToRotation(player.transform.up, climbHit.normal) * player.transform.rotation) * Vector3.forward.normalized;

                    //subtract the distance and time from the respective remainings
                    remainingDistance -= Vector3.Distance(start, target);
                    remainingTime -= path[path.Count - 1].time;
                }
                else
                {
                    //move the target point back the length of the squirrel and end the dash
                    target = GetPoint(climbHit.point, start, tempPos - (up.normalized * 0.25f))/* - (dir * 1.75f)*/;
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
            temp = start + (up.normalized * 0.25f);
            RaycastHit underInfo = new RaycastHit();
            RaycastHit prevUnderInfo;
            //Vector3 newDir = /*(-dir * 0.15f)*/ - up;
            while (tempRemaining > 0)
            {
                prevUnderInfo = underInfo;
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
                    if (Physics.Raycast(temp - (up.normalized * 0.25f) * 2f, -dir, out underInfo, 0.15f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    {
                        if (underInfo.normal.normalized != up.normalized || underInfo.transform.tag != "Climb")
                        {
                            return underInfo;
                        }
                    }
                    else
                    {
                        return prevUnderInfo;
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
                if(under.transform.tag == "Climb")
                    player.climbHit = under;
            }
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }

        //Once the path has been fully calculated figure out if the squirrel's face will hit anything just to make sure we don't go into any walls
        void SafetyNet()
        {
            if (path.Count == 0)
            {
                Debug.LogError("ERROR: Dash pathing length is zero");
                return;
            }
            if (player.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                SafeClimb();
            }
            else
            {
                bool safe = false;
                Vector3 dir = Vector3.zero;
                RaycastHit hit = new RaycastHit();
                float dist = 0;
                do
                {
                    dir = path[path.Count - 1].lerper.endVal - path[path.Count - 1].lerper.startVal;
                    //CHANGED TO SPHERECAST
                    safe = !Physics.SphereCast(path[path.Count - 1].lerper.endVal + (path[path.Count - 1].up * 0.45f), 0.375f, dir, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore);
                    if (!safe)
                    {
                        dist = Vector3.Distance(path[path.Count - 1].lerper.endVal, path[path.Count - 1].lerper.startVal);
                        if (dist < 2f)
                        {
                            path.RemoveAt(path.Count - 1);
                            continue;
                        }
                        else
                        {
                            path[path.Count - 1].lerper.Reset(path[path.Count - 1].lerper.startVal, path[path.Count - 1].lerper.endVal - (dir.normalized * (2f - hit.distance)));
                            break;
                        }
                    }
                } while (!safe && path.Count > 0);
            }            
        }

        void SafeClimb()
        {

            //This is under the assumption that the climbhit will never be inside a mesh collider
            RaycastHit hit = new RaycastHit();
            Vector3 targetPos = path[path.Count - 1].lerper.endVal;
            Vector3 originalDirection = (path[path.Count - 1].lerper.endVal - path[path.Count - 1].lerper.startVal).normalized;

            bool backBlocked = false;

            //Check to see the position we will end up at is colliding with the length of the squirrel's collider
            //if there is any object in the way, then move the target position backwards
            while (Physics.SphereCast(targetPos + path[path.Count - 1].up * 0.4f, 0.375f, originalDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                backBlocked = CheckBackBlocked(originalDirection, hit, false, ref targetPos);
                if (backBlocked)
                    break;
            }

            bool climbHit = true;
            if (!backBlocked)
            {
                climbHit = CheckClimbHit(targetPos + (path[path.Count - 1].up * 0.1f) + (originalDirection * 1.5f), hit);
                while (!climbHit)
                {
                    backBlocked = CheckBackBlocked(originalDirection, hit, true, ref targetPos);
                    if (backBlocked)
                        break;
                    climbHit = CheckClimbHit(targetPos + (path[path.Count - 1].up * 0.1f) + (originalDirection * 1.5f), hit);
                }
            }

            if (targetPos != path[path.Count - 1].lerper.endVal)
            {
                path[path.Count - 1].lerper.Reset(path[path.Count - 1].lerper.startVal, targetPos);
            }

            float angle = 0;
            Vector3 newDirection = originalDirection;
            float angleDirection = 1f;

            //We've moved back as far as we can, if we're still hitting anything, then we'll rotating out of it should fix it
            while (Physics.SphereCast(targetPos + path[path.Count - 1].up * 0.4f, 0.375f, newDirection, out hit, 2f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                CheckAngle(ref angle, ref angleDirection);
                angle += angleDirection;
                newDirection = Quaternion.AngleAxis(angle, path[path.Count - 1].up) * originalDirection.normalized;
                //if we've done a full rotation and we still can't get out, then break out of the function otherwise we'll be in an endless loop
                //If we ever have to break out, then this is most probably an issue with the level design
                if (angle <= -180)
                    break;
            }

            if (angle > -180f)
            {
                climbHit = CheckClimbHit(targetPos + (path[path.Count - 1].up * 0.1f) + (newDirection * 1.5f), hit);
                while (!climbHit)
                {

                    CheckAngle(ref angle, ref angleDirection);
                    angle += angleDirection;
                    newDirection = Quaternion.AngleAxis(angle, path[path.Count - 1].up) * originalDirection.normalized;
                    if (angle <= -180)
                        break;
                    climbHit = CheckClimbHit(targetPos + (path[path.Count - 1].up * 0.1f) + (newDirection * 1.5f), hit);
                }
            }

            if (angle != 0)
            {
                path[path.Count - 1].slerper.Reset(path[path.Count - 1].slerper.startVal, Quaternion.AngleAxis(angle, path[path.Count - 1].up) * path[path.Count - 1].slerper.endVal);
                originalDirection = path[path.Count - 1].slerper.endVal * Vector3.forward.normalized;
            }


            //Debug.DrawRay(targetPos, states.climbHit.normal * 2f, Color.red);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, targetRot * temp.normalized * 2, Color.yellow);
            //Debug.DrawRay(targetPos + states.climbHit.normal * 0.25f, temp2 * 2, Color.cyan);
        }

        bool CheckClimbHit(Vector3 origin, RaycastHit hit)
        {
            if (Physics.Raycast(origin, -path[path.Count - 1].up, out hit, 0.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag == "Climb")
                {
                    return true;
                }
            }
            return false;
        }

        void CheckAngle(ref float angle, ref float angleDirection)
        {
            switch (angle)
            {
                case 0:
                    angleDirection = 1;
                    break;
                case 45:
                    angleDirection = -1;
                    angle = 0;
                    break;
                case -45:
                    angleDirection = 1;
                    angle = 45;
                    break;
                case 90:
                    angleDirection = -1;
                    angle = -45;
                    break;
                case -90:
                    angleDirection = 1;
                    angle = 90;
                    break;
                case 135:
                    angleDirection = -1;
                    angle = -90;
                    break;
                case -135:
                    angleDirection = 1;
                    angle = 135;
                    break;
                case 180:
                    angleDirection = -1;
                    angle = -135;
                    break;
            }
        }

        bool CheckBackBlocked(Vector3 dir, RaycastHit hit, bool safetyCheck, ref Vector3 targetPos)
        {
            //If there is an object that will block us from moving back, then move back only a little bit then break out of here
            if (Physics.SphereCast(targetPos + path[path.Count - 1].up * 0.4f, 0.375f, -dir, out hit, 0.1f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                //targetPos -= (originalDirection.normalized * (hit.distance * 0.5f));
                return true;
            }
            if (safetyCheck)
            {
                if (!CheckClimbHit(targetPos + path[path.Count - 1].up * 0.1f - (dir.normalized * 0.1f), hit))
                    return true;
                if (Physics.CheckSphere(targetPos + path[path.Count - 1].up * 0.4f, 0.375f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                    return true;
            }
            targetPos -= (dir.normalized * 0.1f);
            return false;
        }

        public override void OnUpdate(StateManager states)
        {
            base.OnUpdate(states);
            if (heldDownTimer <= .15f && buttonHeld)
            {
                heldDownTimer += Time.unscaledDeltaTime;
                if (Input.GetAxisRaw("DashCont") != 1f && !Input.GetButton("Dash"))
                {
                    buttonHeld = false;
                }
                RotateBasedOnGround(player);
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
                        blueSquirrel = Instantiate(Resources.Load<GameObject>("BlueSquirrel"), path[path.Count - 1].lerper.endVal, path[path.Count - 1].slerper.endVal);
                        bsTrail = Instantiate(Resources.Load<GameObject>("DashTrail"), path[path.Count - 1].lerper.endVal, player.transform.rotation);
                        line = bsTrail.GetComponent<LineRenderer>();
                        trail = bsTrail.GetComponentInChildren<TrailRenderer>();
                        line.positionCount = path.Count + 1;
                        for (int i = 0; i < line.positionCount; ++i)
                        {
                            line.SetPosition(i, path[0].lerper.startVal + path[0].up * 0.25f);
                        }
                        bsRend = blueSquirrel.GetComponentInChildren<SkinnedMeshRenderer>();
                        //blueSquirrel.transform.parent = player.transform;
                    }


                    if (player.transform.position != path[0].lerper.startVal || player.transform.rotation != prevRotation)
                    {
                        if (blueSquirrel)
                            GetGhostTime();
                        while (path.Count > 0)
                            path.RemoveAt(0);
                        GeneratePath(player, false);
                        pathChanged = true;

                        if (blueSquirrel)
                        {
                            line.positionCount = path.Count + 1;
                            for (int i = 0; i < line.positionCount; ++i)
                            {
                                line.SetPosition(i, path[0].lerper.startVal + path[0].up * 0.25f);
                            }
                            blueSquirrel.transform.position = path[path.Count - 1].lerper.endVal;
                            blueSquirrel.transform.rotation = path[path.Count - 1].slerper.endVal;
                            bsTrail.transform.rotation = player.transform.rotation;
                        }
                    }
                    Rotate(player);
                    RotateBasedOnGround(player);
                    if (blueSquirrel)
                    {
                        line.material.mainTextureOffset = new Vector2((line.material.mainTextureOffset.x + Time.unscaledDeltaTime * 8), line.material.mainTextureOffset.y);
                        MoveGhost();
                    }
                    if (bsRend != null)
                    {
                        rendTime += Time.unscaledDeltaTime;
                        //if (rendTime >= 1f)
                        //    rendTime = 0;
                        bsRend.material.SetFloat("_UnscaledTime", rendTime);
                    }
                }
                else
                {
                    if (path.Count == 0)
                    {
                        GeneratePath(player, false);
                    }
                    if (!timeScaleSet)
                    {
                        Time.timeScale = prevTimeScale;
                        Time.fixedDeltaTime = prevFixedDeltaTime;
                        timeScaleSet = true;
                        player.rigid.useGravity = false;
                        player.rigid.velocity = Vector3.zero;
                        player.rigid.drag = 0;
                        if (player.climbState == PlayerManager.ClimbState.CLIMBING && path.Count > 1)
                            SetCameraAngle(states.transform.up, path[path.Count - 1].up);
                    }
                    if (!player.anim.GetBool(player.hashes.isDashing))
                    {
                        player.anim.SetBool(player.hashes.isDashing, true);
                        if (player.isGrounded || player.climbState == PlayerManager.ClimbState.CLIMBING)
                        {
                            Debug.Log("doing a grounded dash");
                            player.anim.CrossFade(player.hashes.squ_dash, 0.01f);
                        }
                        else
                        {
                            Debug.Log("doing an air dash");
                            player.anim.CrossFade(player.hashes.squ_dash_air, 0.01f);
                        }
                    }
                    if (blueSquirrel)
                    {
                        Destroy(bsTrail);
                        Destroy(blueSquirrel);
                        blueSquirrel = null;
                        bsTrail = null;
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
            for (int i = 0; i < path.Count; ++i)
            {
                Debug.DrawLine(path[i].lerper.startVal/* + (path[i].up * 0.45f)*/, path[i].lerper.endVal/* + (path[i].up * 0.45f)*/, Color.black);
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
                states.transform.rotation = path[pathIndex].slerper.GetValue();
                //Debug.LogError("Path time: " + path[pathIndex].time + " time passed: " + t + " lerpVal: " + path[pathIndex].lerper.GetLerpVal());

                //If the current path is complete, then move onto the next one
                //using a loop so I can keep passing the time over if the next path is complete
                while (path[pathIndex].lerper.done)
                {
                    states.transform.position = path[pathIndex].lerper.endVal;
                    states.transform.rotation = path[pathIndex].slerper.endVal;
                    ++pathIndex;
                    //Add any remaining time from the previous path into the current so we don't go an extra frame or 2 over
                    if (pathIndex < path.Count)
                    {
                        path[pathIndex].Update(path[pathIndex - 1].GetRemainder());
                    }
                    else
                        break;
                }
            }
        }

        Vector3 climbRight;
        Vector3 climbUp;
        Vector3 climbForward;
        void Rotate(StateManager states)
        {
            prevRotation = player.transform.rotation;
            Quaternion tr = prevRotation;
            Vector3 targetDir = Vector3.zero;
            if (player.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                float h = player.movementVariables.horizontal;
                float v = player.movementVariables.vertical;

                //rotating climb up and climb right based on camera's position
                targetDir = climbForward * v;
                targetDir += (climbRight * h);
                targetDir.Normalize();

                if (targetDir == Vector3.zero)
                    return;

                //Apply rotation
                player.movementVariables.moveDirection = targetDir;
                //rotationSpeed = Mathf.Lerp(20, 6, (states.rigid.velocity.magnitude / 6f));
                tr = Quaternion.LookRotation(targetDir, states.transform.up);
            }
            else
            {
                var cam = Camera.main.transform;

                float h = player.movementVariables.horizontal;
                float v = player.movementVariables.vertical;

                targetDir = cam.forward * v;
                targetDir += cam.right * h;

                targetDir.Normalize();
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = player.transform.forward;

                player.movementVariables.moveDirection = targetDir;

                tr = Quaternion.LookRotation(targetDir, player.groundNormal);
            }
            if (targetDir == Vector3.zero)
            {
                Debug.Log("TargetDir is zero!");
                return;
            }

            if (Quaternion.Angle(tr, startRotation) > rotationCutoff)
            {
                if (Quaternion.Angle(tr, minRotation) <= Quaternion.Angle(tr, maxRotation))
                    tr = minRotation;
                else
                    tr = maxRotation;
            }
            Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, Time.unscaledDeltaTime * player.movementVariables.moveAmount * rotationSpeed);

            if (player.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                if(CheckClimbRotation(targetRotation))
                    player.transform.rotation = targetRotation;
                else
                    player.transform.rotation = prevRotation;
            }
            else
            {
                player.transform.rotation = targetRotation;
            }

            //if (blueSquirrel != null)
            //    blueSquirrel.transform.rotation = targetRotation;
        }

        bool CheckClimbRotation(Quaternion targetRotation)
        {
            Vector3 underPos = player.transform.position + player.transform.forward + (-player.transform.up * 1.5f);
            RaycastHit hit = new RaycastHit();

            Vector3 underOrigin = player.transform.position;
            underOrigin += (targetRotation * (Vector3.forward * 1.25f)) + (player.transform.up * 0.5f);

            if (Physics.Linecast(underOrigin, underPos, out hit, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag == "Climb")
                {
                    return true;
                }
            }
            return false;
        }

        void RotateBasedOnGround(StateManager states)
        {
            if (player.climbState == PlayerManager.ClimbState.CLIMBING)
            {
                Vector3 center = states.transform.position;
                center += states.transform.forward + (states.transform.up * 0.2f);

                // Dir represents the downward direction
                Vector3 dir = -states.transform.up * 0.5f;

                // Draw the rays
                //Debug.DrawRay(center, dir * 5f, Color.red);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(center, dir, out hit, 5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform.tag == "Climb")
                    {
                        states.transform.rotation = Quaternion.Slerp(states.transform.rotation, Quaternion.FromToRotation(states.transform.up, hit.normal) * states.transform.rotation, Time.unscaledDeltaTime * rotationSpeed);
                        Vector3 v = Vector3.Cross(hit.normal, Vector3.up);
                        climbForward = Vector3.Cross(v, hit.normal);
                        climbRight = Vector3.Cross(hit.normal, Vector3.up);
                        climbUp = hit.normal;
                    }
                }
            }
            else
            {
                Vector3 rotationNormal = ((PlayerManager)states).GetRotationNormal();
                states.transform.rotation = Quaternion.Slerp(states.transform.rotation, Quaternion.FromToRotation(states.transform.up, rotationNormal) * states.transform.rotation, Time.unscaledDeltaTime * rotationSpeed);
            }

        }

        void GetGhostTime()
        {
            currentBSPathTime = 0;
            for (int i = 0; i < path.Count; ++i)
            {
                if (path[i].lerper.done)
                {
                    currentBSPathTime += path[i].time;
                }
                else
                {
                    currentBSPathTime += (path[i].time * path[i].lerper.GetLerpVal());
                }
            }
            //Debug.LogError(currentBSPathTime);
            //bsTimer = currentBSPathTime;
        }

        //Moves the ghost
        void MoveGhost()
        {
            //trail.time = totalTime * Time.timeScale;
            if (bsPathIndex >= path.Count)
            {
                bsPathIndex = 0;
                //bsTimer = 0;
            }

            if (pathChanged)
            {
                bsPathIndex = 0;
                //currentBSPathTime += Time.unscaledDeltaTime * 0.4f;
                while (currentBSPathTime > 0 && bsPathIndex < path.Count)
                {
                    if (currentBSPathTime > path[bsPathIndex].time)
                    {
                        path[bsPathIndex].Update(path[bsPathIndex].time);
                        currentBSPathTime -= path[bsPathIndex].time;
                        bsPathIndex++;
                    }
                    else
                    {
                        path[bsPathIndex].Update(currentBSPathTime);
                        currentBSPathTime = 0;
                    }
                }
                if (bsPathIndex >= path.Count)
                {
                    bsPathIndex = 0;
                    ResetPath();
                    bsTrail.transform.position = path[bsPathIndex].lerper.startVal + path[bsPathIndex].up * 0.25f;
                    //trail.Clear();
                    for (int i = 0; i < line.positionCount; ++i)
                    {
                        line.SetPosition(i, path[bsPathIndex].lerper.startVal + path[bsPathIndex].up * 0.25f);
                    }
                }
                else
                {
                    for (int i = 0; i <= bsPathIndex; ++i)
                    {
                        line.SetPosition(i, path[i].lerper.startVal + path[i].up * 0.25f);
                    }
                }
            }
            pathChanged = false;
            path[bsPathIndex].Update(Time.unscaledDeltaTime * (ghostSpeed * distanceFactor));

            bsTrail.transform.position = path[bsPathIndex].lerper.GetValue() + path[bsPathIndex].up * 0.25f;
            //bsTrail.transform.rotation = path[bsPathIndex].slerper.GetValue();
            for (int i = bsPathIndex; i < path.Count; ++i)
            {
                line.SetPosition(i + 1, path[bsPathIndex].lerper.GetValue() + path[bsPathIndex].up * 0.25f);
            }
            if (distanceFactor * distance <= 2f)
            {
                showGhost = false;
                blueSquirrel.gameObject.SetActive(showGhost);
                bsTrail.gameObject.SetActive(showGhost);
            }
            else
            {
                switch (gdm)
                {
                    case GhostDisplayMode.On:
                        if (!showGhost)
                        {
                            showGhost = true;
                            blueSquirrel.SetActive(showGhost);
                            bsTrail.gameObject.SetActive(showGhost);
                        }
                        break;
                    case GhostDisplayMode.Flicker:
                        if (ghostTimer >= flickerTime)
                        {
                            showGhost = !showGhost;
                            blueSquirrel.SetActive(showGhost);
                            bsTrail.gameObject.SetActive(showGhost);
                            ghostTimer = 0;
                        }
                        ghostTimer += Time.unscaledDeltaTime;
                        break;
                }

            }

            //If the current path is complete, then move onto the next one
            if (path[bsPathIndex].lerper.done)
            {
                //Debug.LogError(path[pathIndex].time + " " + path[pathIndex].GetRemainder());
                bsTrail.transform.rotation = path[bsPathIndex].slerper.endVal;
                ++bsPathIndex;
                //Add any remaining time from the previous path into the current so we don't go an extra frame or 2 over
                if (bsPathIndex < path.Count)
                {
                    path[bsPathIndex].lerper.Update(path[bsPathIndex - 1].GetRemainder() / path[bsPathIndex].time);
                }
                else
                {
                    bsPathIndex = 0;
                    ResetPath();
                    bsTrail.transform.position = path[bsPathIndex].lerper.startVal + path[bsPathIndex].up * 0.25f;
                    //trail.Clear();
                    for (int i = 0; i < line.positionCount; ++i)
                    {
                        line.SetPosition(i, path[bsPathIndex].lerper.startVal + path[bsPathIndex].up * 0.25f);
                    }
                }
            }

        }

        void ResetPath()
        {
            for (int i = 0; i < path.Count; ++i)
            {
                path[i].Reset();
            }
        }

        CameraManager camera;
        //float prevCameraAmount = 0;
        //FloatLerper camLerper;
        public override void OnLateUpdate(StateManager states)
        {
            base.OnLateUpdate(states);

            //if (camera != null && camLerper != null && !camLerper.done)
            //{
            //    prevCameraAmount = camLerper.GetValue();
            //    camLerper.Update(t / totalTime);
            //    camera.AddToYaw(camLerper.GetValue() - prevCameraAmount);
            //}
        }

        void SetCameraAngle(Vector3 prev, Vector3 curr)
        {
            //prevCameraAmount = 0;
            //totalTime = 0;
            //for(int i = 0; i < path.Count; ++i)
            //{
            //    totalTime += path[i].time;
            //}

            //Camera rotations, checking if the camera needs to be repositioned based on where it is relative to the climb
            float angle = Vector3.SignedAngle(prev, curr, Vector3.up);
            if (angle != 0 && camera != null)
            {
                Vector3 cameraForward = camera.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                float a = Vector3.SignedAngle(-cameraForward, curr, Vector3.up);
                //angle += (angle * 0.1f);
                if (Mathf.Abs(a) > 45f)
                {
                    //if (Mathf.Abs(a) > Mathf.Abs(angle))
                    //{
                    camera.AddCamAdjustment(new CameraAdjustment(new Vector2(0, a), 3f));
                    //camLerper = new FloatLerper(0, a);
                    //}
                    //else
                    //{
                    //    camera.AddCamAdjustment(new CameraAdjustment(new Vector2(0, angle), 3f));
                    //    //camLerper = new FloatLerper(0, angle);
                    //}
                    //Debug.LogError("Adding angle: " + angle + " angle between camera: " + a);
                }
            }
        }

        void LogCameraAngle()
        {
            if (camera != null)
            {
                Vector3 cameraForward = camera.transform.forward;
                cameraForward.y = 0;
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
            player.anim.SetBool(player.hashes.isDashing, false);
            base.OnExit(states);

            pathIndex = 0;
            player.lagDashCooldown = 1.0f;
            if (player.climbState == PlayerManager.ClimbState.NONE && path.Count > 0)
            {
                if (Vector3.Distance(path[path.Count - 1].lerper.startVal, path[path.Count - 1].lerper.endVal) > 0.2f)
                {
                    Vector3 additionalMomentum = (path[path.Count - 1].lerper.endVal - path[path.Count - 1].lerper.startVal).normalized * endMomentum;
                    //additionalMomentum.y *= 0.5f;
                    player.rigid.velocity = additionalMomentum;
                    player.rigid.useGravity = true;
                }
            }
            else
            {
                player.rigid.velocity = Vector3.zero;
            }
            if (player.isGrounded || player.climbState != PlayerManager.ClimbState.NONE)
                player.pauseSpeedHackTimer = false;
            while (path.Count > 0)
                path.RemoveAt(0);
        }
    }

    public class Path
    {
        public VectorLerper lerper;
        public QuaternionSlerper slerper;
        public float time;
        public Vector3 up;
        float remainder;

        public void Update(float amount)
        {
            remainder = (amount / time) + lerper.GetLerpVal();
            if (remainder >= 1)
                remainder = (remainder * time) - time;
            lerper.Update(amount / time);
            slerper.Update(amount / (time * 0.25f));
        }

        public float GetRemainder()
        {
            return remainder;
        }

        public void Reset()
        {
            remainder = 0;
            lerper.Reset();
            slerper.Reset();
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

    public class QuaternionSlerper
    {
        Quaternion value = Quaternion.identity;
        Quaternion _startVal = Quaternion.identity;
        public Quaternion startVal
        {
            get { return _startVal; }
        }
        Quaternion _endVal = Quaternion.identity;
        public Quaternion endVal
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
        public QuaternionSlerper(Quaternion s, Quaternion e)
        {
            _startVal = s;
            value = s;
            _endVal = e;
        }

        public void Reset(Quaternion s, Quaternion e)
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
                value = Quaternion.Slerp(startVal, endVal, lerpVal);
            }
        }

        public Quaternion GetValue()
        {
            return value;
        }
    }

    public class FloatLerper
    {
        float value = 0;
        float startVal = 0;
        float _endVal = 0;
        public float endVal
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
        public FloatLerper(float s, float e)
        {
            startVal = s;
            value = s;
            endVal = e;
        }

        public void Reset(float s, float e)
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
                value = Mathf.Lerp(startVal, endVal, lerpVal);
            }
        }

        public float GetValue()
        {
            return value;
        }
    }

}
