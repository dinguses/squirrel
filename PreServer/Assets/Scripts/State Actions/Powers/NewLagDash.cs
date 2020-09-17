using UnityEngine;

namespace PreServer
{
    //This system needs to work with slopes, otherwise we got a problem
    [CreateAssetMenu(menuName = "Actions/State Actions/New Lag Dash")]
    public class NewLagDash : StateActions
    {
        bool inPos;
        bool inRot;
        float t;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float offsetFromWall = 0.1f;
        float delta;
        RaycastHit hitInfo = new RaycastHit();
        public float dashSpeed = 40f;
        public float dashTime = 0.15f;
        PlayerManager states;
        float timer, letGoTimer;
        bool overrideWait;
        bool setupDone;
        float slowMoTimer;
        bool slowMoSetup;
        float fixedDeltaTimeHold;
        float timeScaleHOld;
        bool overrideSlowMo;
        GameObject blueSquirrel;
        public float dashBazingo = 0;

        Vector3 dashDistance;
        Vector3 dashStartpoint;

        bool dashDone;

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;
            base.OnEnter(states);

            timer = 0;
            letGoTimer = 0;
            slowMoTimer = 0;
            overrideWait = false;
            setupDone = false;
            slowMoSetup = false;
            fixedDeltaTimeHold = 0;
            timeScaleHOld = 0;
            overrideSlowMo = false;
            dashDone = false;

            /*states.anim.SetBool(states.hashes.isDashing, true);
            var time = Time.realtimeSinceStartup - states.timeSinceJump;

            if ((time < 0.01f || time > .6f) && states.isGrounded)
            {
                Debug.Log("doing a grounded dash");
                states.anim.CrossFade(states.hashes.squ_dash, 0.01f);
                states.anim.SetBool(states.hashes.groundDash, true);
                states.anim.SetLayerWeight(2, 0);
            }
            else
            {
                Debug.Log("doing an air dash");
                states.anim.CrossFade(states.hashes.squ_dash_air, 0.01f);
            }

            states.dashInAirCounter++;
            states.rigid.useGravity = false;
            if (states.isRun)
            {
                dashTime = 0.225f;
                states.speedHackAmount -= 0.2f;
                if (states.speedHackAmount <= 0)
                {
                    states.speedHackAmount = 0;
                    states.runRanOut = true;
                }
            }
            else
            {
                dashTime = 0.15f;
            }
            states.rigid.velocity = states.transform.forward * dashSpeed;
            states.rigid.drag = 0;
            timer = 0;*/
        }

        public override void Execute(StateManager sm)
        {

        }

        public override void OnUpdate(StateManager sm)
        {
            base.OnUpdate(states);

            letGoTimer += Time.deltaTime;

            if (letGoTimer <= .15f && !overrideWait)
            {
                if (Input.GetKeyUp("joystick button 5"))
                {
                    SetupDash();
                    overrideWait = true;
                }
            }
            else
            {
                if (!setupDone)
                {
                    if (!slowMoSetup)
                    {
                        fixedDeltaTimeHold = Time.fixedDeltaTime;
                        timeScaleHOld = Time.timeScale;
                        Time.timeScale = 0.05f;
                        Time.fixedDeltaTime = Time.timeScale * 0.0167f;
                        overrideSlowMo = false;
                        slowMoSetup = true;

                        blueSquirrel = Instantiate(Resources.Load<GameObject>("BlueSquirrel"), states.mTransform.position + (states.mTransform.forward * 6.5f), states.mTransform.rotation);
                        blueSquirrel.transform.parent = states.mTransform.transform;
                    }

                    slowMoTimer += (Time.deltaTime * 5);

                    Vector3 targetVelocity = blueSquirrel.transform.forward * states.movementVariables.vertical * 100.5f;
                    Vector3 targetVelocity2 = blueSquirrel.transform.right * states.movementVariables.horizontal * 100.5f;
                    //Vector3 currentVelocity = blueSquirrel.GetComponent<Rigidbody>().velocity;
                    //blueSquirrel.GetComponent<Rigidbody>().velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * 10.5f);
                    // blueSquirrel.GetComponent<Rigidbody>().velocity = Vector3.Lerp(currentVelocity, targetVelocity2, states.delta * 10.5f);

                    if (Input.GetKeyUp("joystick button 5"))
                    {
                        overrideSlowMo = true;
                    }

                    Debug.Log(slowMoTimer);
                    states.lagDashCooldown = slowMoTimer;
                    if (slowMoTimer >= 1 || overrideSlowMo)
                    {


                        var newDashEnd = blueSquirrel.transform.position;

                        //blueSquirrel.transform.parent = null;

                        Time.timeScale = timeScaleHOld;
                        Time.fixedDeltaTime = fixedDeltaTimeHold;
                        SetupDash();

                        dashDistance = blueSquirrel.transform.position;

                        Destroy(blueSquirrel.gameObject);
                    }
                }



                if (setupDone)
                {
                    timer += Time.deltaTime;

                    states.mTransform.position = Vector3.Lerp(states.mTransform.position, dashDistance, Time.deltaTime * dashBazingo);
                }
            }

            /*if (timer > dashTime || states.climbState != PlayerManager.ClimbState.NONE)
            {
                Debug.Log("dash done?");
                states.newDashActive = false;
            }*/

            if (Vector3.Distance(states.mTransform.position, dashDistance) <= 1.0f)
            {
                Debug.Log("Dash is done");
                states.newDashActive = false;
            }


            /*if(timer > dashTime || states.climbState != PlayerManager.ClimbState.NONE)
            {
                states.dashActive = false;
            }
            timer += Time.deltaTime;
            //Debug.Log("LagDash Update: " + states.rigid.velocity);
            //Transfer(states);
            //Debug.DrawRay(targetPos, Vector3.up * ff, Color.yellow);*/
        }

        void SetupDash()
        {
            //dashDistance = states.mTransform.position + (states.mTransform.forward * 6.5f);
            dashStartpoint = states.mTransform.position;

            states.anim.SetBool(states.hashes.isDashing, true);
            var time = Time.realtimeSinceStartup - states.timeSinceJump;

            if ((time < 0.01f || time > .6f) && states.isGrounded)
            {
                Debug.Log("doing a grounded dash");
                states.anim.CrossFade(states.hashes.squ_dash, 0.01f);
            }
            else
            {
                Debug.Log("doing an air dash");
                states.anim.CrossFade(states.hashes.squ_dash_air, 0.01f);
            }

            states.dashInAirCounter++;
            states.rigid.useGravity = false;
            if (states.isRun)
            {
                dashTime = 0.225f;
                states.speedHackAmount -= 0.2f;
                if (states.speedHackAmount <= 0)
                {
                    states.speedHackAmount = 0;
                    states.runRanOut = true;
                }
            }
            else
            {
                dashTime = 0.15f;
            }

            //states.rigid.velocity = states.transform.forward * dashSpeed;

            states.rigid.drag = 0;

            setupDone = true;
        }

        void CheckRaycast(StateManager sm)
        {

        }

        public override void OnExit(StateManager sm)
        {
            base.OnExit(states);

            states.rigid.useGravity = true;
            //states.rigid.velocity = Vector3.zero;
            states.rigid.velocity = states.rigid.velocity / 2;
            timer = 0;
            states.lagDashCooldown = 1.0f;
            states.speedHackRecover = 0.1f;
            states.anim.SetBool(states.hashes.isDashing, false);
            states.timeSinceJump = Time.realtimeSinceStartup;
            //states.anim.SetLayerWeight(2, 1);

            if (states.isGrounded)
                states.pauseSpeedHackTimer = false;
            //states.anim.CrossFade(states.hashes.sq, 0.2f);
        }
    }
}
