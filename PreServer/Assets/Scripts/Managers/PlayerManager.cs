﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

namespace PreServer
{
    public class PlayerManager : StateManager
    {       
        public MovementVariables movementVariables;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public Transform mTransform;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public Animator anim;

        public CapsuleCollider frontCollider;
        public CapsuleCollider grindCollider;

        public bool rotateFast;

        public AnimHashes hashes;
        public AnimatorData animData;

        public bool isJumping;
        public bool dashActive;
        public int dashInAirCounter;
        public bool isGrounded;

        public bool newDashActive;
        public float timeSinceNewDash;

        public bool isColidingInAir;

        public bool isRun;
        public bool isRestart;

        [HideInInspector]
        public float timeSinceJump;
        [HideInInspector]
        public float timeSinceMove;
        [HideInInspector]
        public float timeSinceSlow;

        public Text groundAngle;
        public Text playerAngle;

        public Text PlayerVolX;
        public Text PlayerVolY;
        public Text PlayerVolZ;

        public Text PlayerPosX;
        public Text PlayerPosY;
        public Text PlayerPosZ;

        public Vector3 targetVelocity;
        public Text TargetVolX;
        public Text TargetVolY;
        public Text TargetVolZ;

        public Image dashCooldown;
        public Image speedHack;

        public bool UpIdle;
        public int idleRand;
        public int randMax;
        public int idleInc;
        public bool isSliding;

        public bool inGrindZone;
        public Dictionary<int, Vector3> grindPoints;

        public float held180RotationAmt = 0;
        public Vector3 middlePivot;

        public int testINT = 0;
        public bool ending180Early = false;

        public Vector3 facingPoint;
        public Vector3 behindPoint;

        public KeyValuePair<int, Vector3> facingPointPair;
        public KeyValuePair<int, Vector3> behindPointPair;

        public Vector3 groundNormal;
        public Vector3 frontNormal;
        public Vector3 middleNormal;
        public Vector3 backNormal;
        public Vector3 backupGroundNormal;

        //public KeyValuePair<int, BoxCollider> grindCenterPair;
        //public BoxCollider grindCenter;
        public bool inJoint = false;
        public float grindTimer = 0.25f;
        public bool comingBackFrom180;
        public bool grindDoneAdjusting = false;
        public int nextPointTimer = -1;
        public enum GrindType { STD, UNDERGROUND, DBL_UNDERGROUND, CIRCULAR };
        public GrindType grindType;
        public bool onLastGrindSeg = false;
        public Collider heldGrindCollider;

        public bool rotateBool = false;
        public SkinnedMeshRenderer playerMesh;
        public GameObject bonesTEST;
        public Vector3 storedTargetDir;
        public bool dashStarted = false;
        public bool testRotate = false;
        public bool doneAdjustingGrind = false;
        public bool nextPointHit = false;
        public int rotateDelayTest = 0;

        public Camera mainCam;
        public GameObject front;
        public GameObject middle;
        public GameObject back;
        public static PlayerManager ptr;

        public float groundedDis = .8f;
        public float onAirDis = .85f;
        public LayerMask groundLayer;

        public bool stepUp;
        public bool stepUpJump;
        public bool topHitLong = false;
        public bool topHit = false;
        public bool bottomHit = false;

        public float minSlideAngle = 35;
        public float maxSlideAngle = 70;
        float length = 0;
        [HideInInspector]
        public float lagDashCooldown = 0;
        public float speedHackAmount { get; set; }
        public float speedHackRecover = 0;
        float _groundSpeedMult = 1f;
        float _airSpeedMult = 1f;
        float _climbSpeedMult = 1f;
        float _slideSpeedMult = 1f;
        public float groundSpeedMult { get { return _groundSpeedMult; } }
        public float airSpeedMult { get { return _airSpeedMult; } }
        public float climbSpeedMult { get { return _climbSpeedMult; } }
        public float slideSpeedMult { get { return _slideSpeedMult; } }

        public object DebugExtension { get; private set; }

        [HideInInspector]
        public bool jumpFromClimb;
        [HideInInspector]
        public float jumpFromClimbTimer = 0;
        [HideInInspector]
        public Quaternion jumpFromClimbTarget;
        [HideInInspector]
        public Vector3 slideMomentum;

        public int numNuts = 0;
        public Text nutsText;
        public GameObject barrierText;
        public GameObject barrier;
        public bool inSpeedZone = false;
        public bool speedNutGot = false;
        public bool inFinalZone = false;
        public bool restartFinalCrusher = false;
        public bool inChallenge3Room = false;
        public TimeManager tm;
        public GameObject slideObject;
        public TerrainCollider slideTerrain;
        public GameObject midSection;

        public bool ground180Enabled = true;
        private void Start()
        {
            climbHit = new RaycastHit();

            Vector3 frontOrigin = transform.position;
            Vector3 backOrigin = transform.position;

            frontOrigin += transform.forward + transform.forward / 2;
            length = Vector3.Distance(frontOrigin, backOrigin);
            ptr = this;
            mTransform = this.transform;

            groundNormal = Vector3.zero;

            rigid = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            hashes = new AnimHashes();
            animData = new AnimatorData(anim);

            if (currentState != null)
                currentState.OnEnter(this);

            rotateFast = true;
            stepUp = false;
            stepUpJump = false;
            UpIdle = false;
            randMax = 501;
            idleRand = Random.Range(0, randMax);
            idleInc = 0;

            inGrindZone = false;

            grindPoints = new Dictionary<int, Vector3>();

            speedHackAmount = 2f;
        }

        public void DashAnimDone()
        {
            Debug.Log("dash anim done");
        }

        public float GetLength()
        {
            return length;
        }

        public override void FixedUpdateParent()
        {
            if (isRestart)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            delta = Time.fixedDeltaTime;

        }
        public bool runRanOut = false;
        bool prevRunState = false;

        public override void UpdateParent()
        {
            anim.SetFloat(hashes.grindTimer, grindTimer);
            anim.SetBool(hashes.groundedState, (currentState.name == "Locomotion") ? true : false);

            //Debug.Log(mTransform.rotation.eulerAngles);

            if (grindTimer <= 0.25f)
            {
                grindTimer += 0.01f;
            }

            if (nextPointTimer >= 0 && nextPointTimer < 16)
            {
                nextPointTimer++;
            }
            else
            {
                if (grindPoints.Count > 0)
                {
                    if (grindType == GrindType.UNDERGROUND && facingPoint == grindPoints[0] && behindPoint == grindPoints[1])
                    {
                        LeaveGrindOverrideNoMovement();
                    }

                    else if (grindType == GrindType.DBL_UNDERGROUND)
                    {
                        if ((facingPoint == grindPoints[0] && behindPoint == grindPoints[1]) 
                            || (facingPoint == grindPoints[grindPoints.Count - 1] && behindPoint == grindPoints[grindPoints.Count - 2]))
                        {
                            LeaveGrindOverrideNoMovement();
                        }
                    }
                }
            }

            if (nextPointTimer == 15)
            {
                if (grindPoints.Count > 0)
                {
                    if (grindType == GrindType.UNDERGROUND && facingPoint == grindPoints[0])
                    {
                        nextPointTimer = -1;
                        LeaveGrindOverrideNoMovement();
                    }

                    else if (grindType == GrindType.DBL_UNDERGROUND)
                    {
                        if ((facingPoint == grindPoints[0])
                            || (facingPoint == grindPoints[grindPoints.Count - 1]))
                        {
                            nextPointTimer = -1;
                            LeaveGrindOverrideNoMovement();
                        }
                    }
                }
            }

            if (grindPoints.Count > 0)
            {
                if (grindType == GrindType.UNDERGROUND && facingPoint == grindPoints[1] && behindPoint == grindPoints[0])
                {
                    NextPoint();
                }

                else if (grindType == GrindType.DBL_UNDERGROUND)
                {
                    if ((facingPoint == grindPoints[1] && behindPoint == grindPoints[0])
                        || (facingPoint == grindPoints[grindPoints.Count - 2] && behindPoint == grindPoints[grindPoints.Count - 1]))
                    {
                        NextPoint();
                    }
                }
            }

            // Check squirrel distance to forward point
            if (facingPoint != Vector3.zero && currentState.name == "Grinding")
            {
                if (Vector3.Distance(mTransform.position, facingPoint) <= 2.1f)
                {
                    //Debug.Log(Vector3.Distance(mTransform.position, facingPoint) + " - got too close, next point");

                    frontCollider.enabled = false;
                    NextPoint();
                }
            }

            //TODO Better endpoint
            /*if (grindEnds.Count != 0 && currentState.name == "Grinding")
            {
                var squirrelFront = mTransform.position + (mTransform.forward * 2);

                foreach (var grindEnd in grindEnds)
                {
                    //Debug.Log("Dist from end - "+Vector3.Distance(squirrelFront, grindEnd));
                    if (Vector3.Distance(squirrelFront, grindEnd) < 1.25f)
                    {
                        //Debug.Log("Time for leaving");
                        //BackLeftTest();
                    }
                }
            }*/

            delta = Time.deltaTime;
            if (climbState == ClimbState.NONE)
            {
                UpdateGroundNormals();
                SetAnimStates();
                StepUpTest();
                CheckSliding();
                if (Input.GetKeyDown(KeyCode.L))
                {
                    isSliding = !isSliding;
                }
                CheckForClimb();
            }
            else
            {
                SetClimbAnimStates();
            }

            //Reset lagdash when squirrel touches the ground
            if (isGrounded && dashInAirCounter != 0)
                dashInAirCounter = 0;

            //TODO: make this better
            #region upIdle
            if (!UpIdle)
            {
                if (movementVariables.moveAmount == 0)
                {
                    if (idleInc < 250)
                        idleInc++;
                }
                else
                {
                    idleInc = 0;
                }

                if (idleInc >= 250)
                {
                    var test = Random.Range(0, randMax);
                    if (test == idleRand)
                    {
                        UpIdle = true;
                        idleInc = 0;
                    }
                }
            }
            else
            {
                if (movementVariables.moveAmount == 0 && currentState.name == "Locomotion")
                {
                    if (idleInc < 250)
                        idleInc++;
                }
                else
                {
                    idleInc = 0;
                    UpIdle = false;
                }

                if (idleInc >= 250)
                {
                    var test = Random.Range(0, randMax);
                    if (test == idleRand)
                    {
                        UpIdle = false;
                        idleInc = 0;
                    }
                }
            }
            #endregion
            if (lagDashCooldown > 0 && !dashActive)
            {
                lagDashCooldown -= delta;
                if (lagDashCooldown < 0)
                    lagDashCooldown = 0;
            }
            SpeedHackCooldown();
            if (slideMomentum != Vector3.zero)
            {
                slideMomentum = Vector3.Lerp(slideMomentum, Vector3.zero, delta);
                if (slideMomentum.magnitude <= 0.1f)
                    slideMomentum = Vector3.zero;
            }
            #region UI
            //groundAngle.text = "Ground Angle: " + Vector3.Angle(groundNormal, Vector3.up).ToString("F2");
            //playerAngle.text = "Player Angle: " + Vector3.Angle(mTransform.up, Vector3.up).ToString("F2");

            //PlayerVolX.text = "X: " + rigid.velocity.x.ToString("F2");
            //PlayerVolY.text = "Y: " + rigid.velocity.y.ToString("F2");
            //PlayerVolZ.text = "Z: " + rigid.velocity.z.ToString("F2");

            //PlayerPosX.text = "X: " + mTransform.position.x.ToString("F2");
            //PlayerPosY.text = "Y: " + mTransform.position.y.ToString("F2");
            //PlayerPosZ.text = "Z: " + mTransform.position.z.ToString("F2");

            //TargetVolX.text = "X: " + targetVelocity.x.ToString("F2");
            //TargetVolY.text = "Y: " + targetVelocity.y.ToString("F2");
            //TargetVolZ.text = "Z: " + targetVelocity.z.ToString("F2");

            //inGrindZoneText.text = "inGrindZone - " + inGrindZone;

            dashCooldown.fillAmount = 1 - lagDashCooldown;
            speedHack.fillAmount = speedHackAmount / 2f;
            #endregion
            
            //RotateSections();
            
            //TODO: try using climbHit instead of bottomHit
            //if (dashActive && didClimbHit && climbState == ClimbState.NONE)
            //{
            //    Debug.Log("Ending this dash EARLY");
            //    dashActive = false;
            //    playerMesh.gameObject.SetActive(true);
            //}

            if (!dashActive && dashStarted)
                dashStarted = false;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var currentScene = SceneManager.GetActiveScene();

                switch (currentScene.name)
                {
                    //case "TestScene":
                    //    SceneManager.LoadScene("Plaza");
                    //    break;
                    case "Plaza":
                        //SceneManager.LoadScene("DEMO");
                        SceneManager.LoadScene("DEMO 2");
                        break;
                    //case "DEMO":
                    //    SceneManager.LoadScene("DEMO 2");
                    //    break;
                    case "DEMO 2":
                        //SceneManager.LoadScene("TestScene");
                        SceneManager.LoadScene("Plaza");
                        break;
                    default:
                        //SceneManager.LoadScene("DEMO");
                        SceneManager.LoadScene("DEMO 2");
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                mTransform.position = new Vector3(51, -4, -29);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                mTransform.position = new Vector3(-80f, 30f, 32f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                mTransform.position = new Vector3(-286f, 22f, 61f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                mTransform.position = new Vector3(-433f, 30f, 124f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                mTransform.position = new Vector3(-536f, 36f, 155f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                mTransform.position = new Vector3(-534f, 60f, 167);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                mTransform.position = new Vector3(-545f, 60f, 311);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                mTransform.position = new Vector3(133f, 25f, -197);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ground180Enabled = !ground180Enabled;
            }
        }

        void RotateSections()
        {
            Vector3 origin = transform.position + (transform.up * 0.2f) + (transform.forward * 1.75f);
            RaycastHit hit = new RaycastHit();
            Vector3 dir = (transform.forward * 0.2f) - transform.up;
            Debug.DrawRay(origin, dir * 0.5f, Color.blue);
            //Raycast in front of the squirrel, used to check if we've hit a ceiling, ground, or another climb-able surface
            if (Physics.Raycast(origin, dir, out hit, 0.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                Quaternion tr = Quaternion.FromToRotation(midSection.transform.up, hit.normal) * midSection.transform.rotation;
                Quaternion targetRotation = Quaternion.Slerp(midSection.transform.rotation, tr, delta * 15);
                midSection.transform.rotation = targetRotation;
                Debug.Log("Rotating midsection");
            }
        }

        public bool pauseSpeedHackTimer = false;
        void SpeedHackCooldown()
        {
            //If I'm running and the run meter hasn't been depleted
            if (isRun && !runRanOut)
            {
                //if(isGrounded || climbState == ClimbState.CLIMBING)
                //Deplete the meter a little, starting a run will deplete a more of the bar to avoid spamming the run button
                speedHackAmount -= prevRunState ? delta : .075f;
                //if the meter is depleted, then run ran out and you can't activate it until it refills
                if (speedHackAmount <= 0)
                {
                    speedHackAmount = 0;
                    runRanOut = true;
                }
                //update the movement multipliers
                _groundSpeedMult = 2.5f;
                _airSpeedMult = 2.5f;
                _climbSpeedMult = 2f;
                _slideSpeedMult = 2f;
            }
            else if (speedHackAmount < 2f)
            {
                if (pauseSpeedHackTimer && (climbState == ClimbState.CLIMBING || isGrounded))
                    pauseSpeedHackTimer = false;
                if (!pauseSpeedHackTimer)
                    speedHackAmount += runRanOut ? delta * 0.5f : dashActive ? 0 : delta;
                if (speedHackAmount >= 2f)
                {
                    speedHackAmount = 2f;
                    runRanOut = false;
                }
                _groundSpeedMult = 1f;
                if (!isSliding && !dashActive)
                {
                    if (_airSpeedMult > 1f)
                    {
                        _airSpeedMult -= delta * 4f;
                        if (_airSpeedMult < 1f)
                            _airSpeedMult = 1f;
                    }
                }
                else
                {
                    _airSpeedMult = 1f;
                }
                _climbSpeedMult = 1f;
                _slideSpeedMult = 1f;
            }
            else
            {
                if (_airSpeedMult != 1f)
                {
                    _airSpeedMult = 1f;
                }
            }
            if (speedHackRecover > 0)
            {
                speedHackRecover -= delta;
                if (speedHackRecover < 0)
                    speedHackRecover = 0;
            }
            prevRunState = isRun;
        }

        public void StepUpTest()
        {
            var bottomFloat = .1f;
            var topFloat = .6f;
            var topFloatLong = .6f;

            if (!isGrounded)
            {
                topFloat = .9f;
                topFloatLong = .9f;
            }

            var bottomRay = mTransform.position + (mTransform.forward * 1.25f) /*+ (Vector3.up * bottomFloat)*/;
            var topRay = mTransform.position + (mTransform.forward * 1.25f) + (Vector3.up * topFloat);
            var topRayLong = mTransform.position + (mTransform.forward * 2.5f) + (Vector3.up * topFloatLong);

            RaycastHit hitBottom = new RaycastHit();
            RaycastHit hitTop = new RaycastHit();
            RaycastHit hitTopLong = new RaycastHit();

            //if (stepUp)
            //{
            //    Debug.DrawRay(bottomRay, mTransform.forward, Color.green);
            //    Debug.DrawRay(topRay, mTransform.forward, Color.green);
            //    Debug.DrawRay(topRayLong, mTransform.forward, Color.green);
            //}
            //else
            //{
            //    Debug.DrawRay(bottomRay, mTransform.forward, Color.blue);
            //    Debug.DrawRay(topRay, mTransform.forward, Color.blue);
            //    Debug.DrawRay(topRayLong, mTransform.forward, Color.blue);
            //}

            if (Physics.Raycast(bottomRay, mTransform.forward, out hitBottom, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                bottomHit = true;
            else
                bottomHit = false;

            if (Physics.Raycast(topRay, mTransform.forward, out hitTop, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                topHit = true;
            else
                topHit = false;

            if (Physics.Raycast(topRayLong, mTransform.forward, out hitTopLong, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                topHitLong = true;
            else
                topHitLong = false;


            if (bottomHit && !topHit && !topHitLong && movementVariables.moveAmount > 0.05f)
            {
                stepUp = true;
            }
            else
            {
                stepUp = false;
            }
        }

        public void UpdateGroundNormals()
        {
            // Setup origin points for three different ground checking vector3s. One in middle of player, one in front, and one in back
            Vector3 middleOrigin = transform.position + (transform.up * 0.7f);
            Vector3 frontOrigin = transform.position + (transform.up * 0.7f);
            Vector3 backOrigin = transform.position + (transform.forward / 4) + (transform.up * 0.7f);

            middleOrigin += transform.forward;
            frontOrigin += transform.forward + transform.forward / 2;
            //backOrigin += states.mTransform.forward / 2;

            //// Origins should be coming from inside of player
            //middleOrigin.y += .7f;
            //frontOrigin.y += .7f;
            //backOrigin.y += .7f;

            // Dir represents the downward direction
            Vector3 dir = -transform.up;


            //TODO: testing this

            if (isGrounded)
            {
                // If player is on a sloped surface, must account for the normal
                dir.z = dir.z - groundNormal.z;
            }
            else
            {
                dir.z = dir.z - transform.up.z;
                //dir.z = dir.z 
            }

            // Set distance depending on if player grounded or in air
            float dis = (isGrounded) ? groundedDis : onAirDis;

            // RaycastHits for each grounding ray
            RaycastHit frontHit = new RaycastHit();
            RaycastHit middleHit = new RaycastHit();
            RaycastHit backHit = new RaycastHit();

            // Draw the rays
            //Debug.DrawRay(frontOrigin, dir * dis, Color.green);
            //Debug.DrawRay(middleOrigin, dir * dis, Color.yellow);
            //Debug.DrawRay(backOrigin, dir * dis, Color.white);

            // If player is already grounded, check if they should remain
            //if (states.isGrounded)
            //{

            float angle = 0;
            frontSliding = false;
            middleSliding = false;
            backSliding = false;
            if (Physics.Raycast(frontOrigin, dir, out frontHit, dis + 0.3f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                /*if (frontHit.transform.tag == "grindMeat" && !inGrindZone)
                {
                    inGrindZone = true;
                    GenerateGrindPoints(heldGrindCollider);
                }*/

                frontNormal = frontHit.normal;
                angle = Vector3.Angle(frontHit.normal, Vector3.up);
                if (angle >= 70)
                    front = null;
                else
                {
                    front = frontHit.transform.gameObject;
                }
            }
            else
            {
                front = null;
            }

            if (Physics.SphereCast(middleOrigin, 0.3f, dir, out middleHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                middleNormal = middleHit.normal;
                angle = Vector3.Angle(middleHit.normal, Vector3.up);
                if (angle >= 70)
                    middle = null;
                else
                    middle = middleHit.transform.gameObject;
                if(slideObject == null || slideObject != middleHit.transform.gameObject)
                {
                    slideObject = middleHit.transform.gameObject;
                    slideTerrain = slideObject.GetComponent<TerrainCollider>();
                }
            }
            else
            {
                slideObject = null;
                slideTerrain = null;
                middle = null;
            }

            if (Physics.SphereCast(backOrigin, 0.3f, dir, out backHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                if (backHit.transform.tag == "grindMeat")
                {
                    back = null;
                }
                else
                {
                    backNormal = backHit.normal;
                    angle = Vector3.Angle(backHit.normal, Vector3.up);
                    if (angle >= 70)
                        back = null;
                    else
                        back = backHit.transform.gameObject;
                }
            }
            else
            {
                back = null;
            }

            // If the back raycast hits something, but the front and middle aren't, AND the player is grounded, then the player is probably on a slope, but not rotated properly
            //TODO: Make this better ASAP!!!!!
            if (backHit.normal != groundNormal && frontHit.normal == Vector3.zero && middleHit.normal == Vector3.zero && !isGrounded)
            {
                //Debug.Log("backHit specific use case!");

                // Set the ground normal to be the normal of the backHit
                groundNormal = backHit.normal;
            }

            if (front || middle || back)
                isGrounded = true;
            else
                isGrounded = false;

            if (CheckGrounded(frontCollider) && (!front && !middle && !back) && !isGrounded/* && bottomHit*/)
            {
                float timeDifference = Time.realtimeSinceStartup - timeSinceJump;

                // have to have been ungrounded to start checking
                if (timeDifference > .05f)
                {
                    isColidingInAir = true;
                }
            }
            else
            {
                isColidingInAir = false;
            }

            //isGrounded = (CheckGrounded(frontCollider) || CheckGrounded(backCollider));
            //isColidingInAir = (CheckGrounded(frontCollider) || CheckGrounded(backCollider));
        }
        bool didClimbHit = false;
        float climbAngle = 0;

        public Vector3 GetRotationNormal(float dis = 1.5f)
        {
            Vector3 backRight = transform.position + (transform.up * 0.25f) + (transform.right * 0.3f);
            Vector3 backLeft = transform.position + (transform.up * 0.25f) - (transform.right * 0.3f);
            Vector3 middleRight = backRight + (transform.forward);
            Vector3 middleLeft = backLeft + (transform.forward);
            Vector3 frontRight = backRight + (transform.forward * 2f);
            Vector3 frontLeft = backLeft + (transform.forward * 2f);
            RaycastHit backRightHit = new RaycastHit();
            RaycastHit backLeftHit = new RaycastHit();
            RaycastHit middleRightHit = new RaycastHit();
            RaycastHit middleLeftHit = new RaycastHit();
            RaycastHit frontRightHit = new RaycastHit();
            RaycastHit frontLeftHit = new RaycastHit();
            Vector3 dir = -Vector3.up;
            if (!Physics.Raycast(backRight, dir, out backRightHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                backRightHit.normal = Vector3.up;
            }
            if (!Physics.Raycast(backLeft, dir, out backLeftHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                backLeftHit.normal = Vector3.up;
            }
            if (!Physics.Raycast(middleRight, dir, out middleRightHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                middleRightHit.normal = Vector3.up;
            }
            if (!Physics.Raycast(middleLeft, dir, out middleLeftHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                middleLeftHit.normal = Vector3.up;
            }
            if (!Physics.Raycast(frontRight, dir, out frontRightHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                frontRightHit.normal = Vector3.up;
            }
            if (!Physics.Raycast(frontLeft, dir, out frontLeftHit, dis, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                frontLeftHit.normal = Vector3.up;
            }
            //Debug.DrawRay(backRight, dir * dis, Color.green);
            //Debug.DrawRay(backLeft, dir * dis, Color.green);
            //Debug.DrawRay(middleRight, dir * dis, Color.green);
            //Debug.DrawRay(middleLeft, dir * dis, Color.green);
            //Debug.DrawRay(frontRight, dir * dis, Color.green);
            //Debug.DrawRay(frontLeft, dir * dis, Color.green);
            //// Get the vectors that connect the raycast hit points

            //Vector3 a = backRightHit.point - backLeftHit.point;
            //Vector3 b = frontRightHit.point - backRightHit.point;
            //Vector3 c = frontLeftHit.point - frontRightHit.point;
            //Vector3 d = backRightHit.point - frontLeftHit.point;

            //// Get the normal at each corner

            //Vector3 crossBA = Vector3.Cross(b, a);
            //Vector3 crossCB = Vector3.Cross(c, b);
            //Vector3 crossDC = Vector3.Cross(d, c);
            //Vector3 crossAD = Vector3.Cross(a, d);

            //// Calculate composite normal

            //return (crossBA + crossCB + crossDC + crossAD).normalized;
            return (backRightHit.normal + backLeftHit.normal + middleRightHit.normal + middleLeftHit.normal + frontRightHit.normal + frontLeftHit.normal).normalized;
        }

        //Checks to see if the face is hitting a slanted wall
        bool CheckGrounded(CapsuleCollider col)
        {
            //Debug.DrawLine(mTransform.position, mTransform.position + mTransform.forward, Color.yellow);

            //return Physics.CheckBox(new Vector3(col.bounds.center.x, col.bounds.center.y - (col.bounds.size.y - (col.bounds.size.y * 0.5f)), col.bounds.center.z), new Vector3(col.bounds.size.x * 1.5f, col.bounds.size.y * 0.5f, col.bounds.size.z * 1.5f) * 0.5f, col.transform.rotation, groundLayer);
            //return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 0.9f, groundLayer, QueryTriggerInteraction.Ignore);
            Vector3 topRay = mTransform.position + (mTransform.forward * 1.6f) + (mTransform.up * 0.5f);
            if(Physics.SphereCast(topRay, 0.3f, mTransform.forward, out climbHit, .15f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                didClimbHit = true;
                climbAngle = Vector3.Angle(climbHit.normal, Vector3.up);
                if (climbAngle >= 70 && climbAngle < 90)
                {
                    return true;
                }
            }
            else
            {
                didClimbHit = false;
            }
            return false;
        }

        /// <summary>
        /// Updates animator's isGrounded
        /// </summary>
        void SetAnimStates()
        {
            anim.SetBool(hashes.isGrounded, isGrounded);

            float timeDifference = Time.realtimeSinceStartup - timeSinceJump;

            anim.SetFloat(hashes.TimeSinceGrounded, timeDifference);

            //states.anim.SetFloat(states.hashes.vertical, states.movementVariables.moveAmount, 0.2f, states.delta);
            anim.SetFloat(hashes.speed, movementVariables.moveAmount, 0.01f, delta);

            if (movementVariables.moveAmount == 0)
            {
                timeSinceMove = Time.realtimeSinceStartup;
            }

            if (movementVariables.moveAmount > .3f)
            {
                timeSinceSlow = Time.realtimeSinceStartup;
            }

            timeDifference = Time.realtimeSinceStartup - timeSinceMove;
            float timeDifference2 = Time.realtimeSinceStartup - timeSinceSlow;

            if (timeDifference < .2f)
            {
                anim.SetFloat(hashes.TimeSinceMove, timeDifference, 0.01f, delta);
            }

            if (timeDifference2 < .2f)
            {
                anim.SetFloat(hashes.TimeSinceSlow, timeDifference2, 0.01f, delta);
            }

            anim.SetBool(hashes.UpIdle, UpIdle);
            anim.SetBool(hashes.isSliding, isSliding);
        }

        void SetClimbAnimStates()
        {
            anim.SetBool(hashes.isGrounded, true);

            float timeDifference = Time.realtimeSinceStartup - timeSinceJump;

            anim.SetFloat(hashes.TimeSinceGrounded, timeDifference);

            //states.anim.SetFloat(states.hashes.vertical, states.movementVariables.moveAmount, 0.2f, states.delta);
            anim.SetFloat(hashes.speed, movementVariables.moveAmount, 0.01f, delta);

            if (movementVariables.moveAmount == 0)
            {
                timeSinceMove = Time.realtimeSinceStartup;
            }

            if (movementVariables.moveAmount > .3f)
            {
                timeSinceSlow = Time.realtimeSinceStartup;
            }

            timeDifference = Time.realtimeSinceStartup - timeSinceMove;
            float timeDifference2 = Time.realtimeSinceStartup - timeSinceSlow;

            if (timeDifference < .2f)
            {
                anim.SetFloat(hashes.TimeSinceMove, timeDifference, 0.01f, delta);
            }

            if (timeDifference2 < .2f)
            {
                anim.SetFloat(hashes.TimeSinceSlow, timeDifference2, 0.01f, delta);
            }
            anim.SetBool(hashes.UpIdle, false);
        }

        public enum ClimbState { NONE, ENTERING, CLIMBING, EXITING }
        public ClimbState climbState;
        public RaycastHit climbHit;
        Transform prevClimbT;
        void CheckForClimb()
        {
            //Vector3 topRay = mTransform.position + (mTransform.forward * 0.9f) + (mTransform.up * 0.5f);

            //Debug.DrawRay(topRay, mTransform.forward, Color.blue);
            if (didClimbHit && !dashActive/*Physics.SphereCast(topRay, 0.3f, mTransform.forward, out climbHit, 1, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore)*/)
            {
                if (/*climbAngle >= 70 && climbAngle <= 90 && */climbHit.transform.tag == "Climb"/* && (prevClimbT != climbHit.transform || isGrounded)*/)
                {
                    climbState = ClimbState.ENTERING;
                    prevClimbT = climbHit.transform;

                    anim.SetBool(hashes.isClimbing, true);
                }
            }
        }

        bool frontSliding = false;
        bool middleSliding = false;
        bool backSliding = false;
        Vector3 slideDirection;
        void CheckSliding()
        {
            if(slideTerrain != null && middle != null)
            {
                if((double)Vector3.Angle(Vector3.up, middleNormal) > (double)(slideTerrain.slideAngle))
                {
                    isSliding = true;
                }
            }
        }

        Vector3 backCastNormal;
        public Vector3 BackCastNormal()
        {
            return backCastNormal;
        }

        public Vector3 GetSlideDirection()
        {
            return slideDirection;
        }

        void OnTriggerEnter(Collider other)
        {
            // Nut (for playtest)
            if (other.tag == "nut")
            {
                if (other.name == "speedNut")
                {
                    speedNutGot = true;
                }

                Destroy(other.gameObject);
                numNuts++;

                //update canvas text
                nutsText.text = "Nuts Collected: " + numNuts + "/5";

                if (numNuts == 5)
                {
                    // Remove barrier
                    barrierText.gameObject.SetActive(false);
                    barrier.SetActive(false);
                }
            }

            // SpeedZone (for playtest)
            if (other.tag == "speedZone")
            {
                inSpeedZone = true;
            }

            // FinalZone (for playtest)
            if (other.tag == "finalCrusher")
            {
                inFinalZone = true;
            }

            if (other.tag == "challenge3")
            {
                inChallenge3Room = true;
            }

            // For the crushers
            if (other.tag == "hurty")
            {
                if (SceneManager.GetActiveScene().name == "Plaza")
                {
                    mTransform.position = new Vector3(-82.6f, 9.2f, 3.7f);
                }
                else
                {
                    if (inFinalZone || !inChallenge3Room)
                    {
                        mTransform.position = new Vector3(-545f, 60f, 311);
                        restartFinalCrusher = true;
                    }

                    else
                    {
                        mTransform.position = new Vector3(-286f, 22f, 61f);
                    }
                }
            }

            /*if (other.tag == "GrindCenter")
            {
                Debug.Log("in da grind center");

                inGrindZone = true;

                GenerateGrindPoints(other);

                // If it's only a two point grind
                if (grindPoints.Count == 2)
                {
                    var grindMaster = other.gameObject.transform.parent.parent;
                    var centers = grindMaster.GetChild(1);
                    grindCenter = centers.GetChild(0).GetComponent<BoxCollider>();
                }
            }*/

            // Start a grind if you've entered a grind zone and were not already in one
            if (other.tag == "Grind")
            {
                heldGrindCollider = other;
            }


            if (other.tag == "Grind" && !inGrindZone && grindTimer >= 0.15f && currentState.name != "WaitForAnimation")
            {
                Debug.Log(Time.frameCount + " - entered grind collider?");
                inGrindZone = true;
                GenerateGrindPoints(other);
            }

            if (other.tag == "joint")
            {
                inJoint = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            // SpeedZone (for playtest)
            if (other.tag == "speedZone")
            {
                inSpeedZone = false;
            }

            if (other.tag == "challenge3")
            {
                inChallenge3Room = false;
            }

            // TODO: temporarily moved to BackLeftTest()
            /*if (other.tag == "Grind" && !inJoint && !testRotate && doneAdjustingGrind)
            {
                Debug.Log("purge?");

                Debug.Log("isGrounded: " + isGrounded);

                PurgeGrindPoints();
                inGrindZone = false;
                doneAdjustingGrind = false;
            }*/

            if (other.tag == "Grind" && currentState.name != "Grinding" && currentState.name != "WaitForAnimation")
            {
                inGrindZone = false;
                heldGrindCollider = null;
            }

            if (other.tag == "joint")
            {
                inJoint = false;
                nextPointHit = false;
            }
        }

        public void BackLeftTest()
        {
            if (!inJoint && /*!testRotate &&*/ grindDoneAdjusting)
            {
                Debug.Log("purge?");

                Debug.Log("isGrounded: " + isGrounded);

                mTransform.position = Vector3.Lerp(mTransform.position, (mTransform.position + (mTransform.forward)), Time.deltaTime * 2f);
                //Vector3.Lerp(states.rigid.position, test + (states.mTransform.forward / 8), states.delta * 10);

                grindTimer = 0f;

                PurgeGrindPoints();
                inGrindZone = false;
                grindDoneAdjusting = false;
            }
        }

        public void LeftUndergroundGrind()
        {
            Debug.Log("left underground grind");

            var fronttest = (mTransform.position + (mTransform.forward * 2));

            //var testyyy = Vector3.Distance(mTransform.position, grindEnds[2]) - Vector3.Distance(grindEnds[1], grindEnds[2]);

            //mTransform.position = Vector3.Lerp(mTransform.position, grindEnds[2], Time.deltaTime * 10f);

            if (movementVariables.moveAmount <= 0.5f)
            {
                //mTransform.position = Vector3.Lerp(mTransform.position, grindEnds[2], Time.deltaTime * 5f);
            }

            /*var testyyy = Vector3.Distance(mTransform.position, grindEnds[1]);

            if (movementVariables.moveAmount <= 0.5f)
            {
                mTransform.position = Vector3.MoveTowards(mTransform.position, grindEnds[1], testyyy / 6);
            }*/

            grindTimer = 0f;

            PurgeGrindPoints();
            inGrindZone = false;
            grindDoneAdjusting = false;
        }

        public void LeaveGrindOverride()
        {
            mTransform.position = Vector3.Lerp(mTransform.position, (mTransform.position + (mTransform.forward * 4)), Time.deltaTime * 5f);

            grindTimer = 0f;

            PurgeGrindPoints();
            inGrindZone = false;
            grindDoneAdjusting = false;
        }

        public void LeaveGrindOverrideNoMovement()
        {
            grindTimer = 0f;

            PurgeGrindPoints();
            inGrindZone = false;
            grindDoneAdjusting = false;
        }

        /// <summary>
        /// Sets player's mesh to not active - for dash animation
        /// </summary>
        public void DashAnimNotActive()
        {
            if (dashActive)
            {
                playerMesh.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Sets player's mesh to active - for dash animation
        /// </summary>
        public void DashAnimActive()
        {
            playerMesh.gameObject.SetActive(true);
        }

        /// <summary>
        /// Sets dashStarted to true to guarentee dash always at least starts
        /// </summary>
        public void DashSetDashStarted()
        {
            //tm.DoSlowMotion();
            dashStarted = true;
        }

        public void Check180Rotation()
        {
            //Debug.Log(Vector3.Angle(mTransform.forward, storedTargetDir));

            /*if (Vector3.Angle(mTransform.forward, storedTargetDir) < 10)
            {
                Debug.Log("ENDIN THE TURN EARLY");
                anim.SetBool(hashes.waitForAnimation, false);
                ending180Early = true;
                anim.CrossFade(hashes.squ_idle, .2f);
            }*/
        }

        /// <summary>
        /// Moves player forward a bit during dash
        /// </summary>
        public void DashAnimForward()
        {
            if (!bottomHit)
            {
                //mTransform.position += (mTransform.forward / 2);
                //bonesTEST.transform.position += (bonesTEST.transform.position / 2);
            }
        }

        /// <summary>
        /// Moves player back a bit during dash
        /// </summary>
        public void DashAnimBackward()
        {
            if (!bottomHit)
            {
               // mTransform.position -= (mTransform.forward / 2);
                //bonesTEST.transform.position -= (bonesTEST.transform.position / 2);
            }
        }

        /// <summary>
        /// When 180 is done, sets the anim boolean to false
        /// </summary>
        public void Done180()
        {
            testRotate = false;

            anim.SetBool(hashes.waitForAnimation, false);
            rotateDelayTest = 0;
            //anim.SetBool(hashes.mirror180, false);
        }

        /// <summary>
        /// When 180 starts, reset the held 180 amount back to 0
        /// </summary>
        public void Start180()
        {
            Debug.Log("Start 180");
            held180RotationAmt = 0;
        }

        /// <summary>
        /// Logic for moving to next grind segment
        /// </summary>
        public void NextPoint()
        {
            if (grindDoneAdjusting == false)
            {
                return;
            }

            if (grindType != GrindType.CIRCULAR && (facingPointPair.Key == 0 || facingPointPair.Key == (grindPoints.Count - 1)))
            {
                return;
            }

            Debug.Log("Next Point");

            bool point0Adj = false;
            if (grindType == GrindType.UNDERGROUND && facingPoint == grindPoints[1] && behindPoint == grindPoints[0])
            {
                point0Adj = true;
            }

            else if (grindType == GrindType.DBL_UNDERGROUND)
            {
                if ((facingPoint == grindPoints[1] && behindPoint == grindPoints[0]) 
                    || (facingPoint == grindPoints[grindPoints.Count - 2] && behindPoint == grindPoints[grindPoints.Count - 1]))
                {
                    point0Adj = true;
                }
            }

            // TODO simplify this, no need to repeat the same code 8 times
            // If the grind has more than 1 segment, move to next (or previous) point
            if (grindPoints.Count > 2)
            {
                // Circular grind logic
                if (grindType == GrindType.CIRCULAR)
                {
                    // If heading towards point 0
                    if (facingPointPair.Key == 0)
                    {
                        // And you're coming from the last point and heading towards point 1
                        if (behindPointPair.Key == grindPoints.Count - 1)
                        {
                            var newKey = 1;
                            var bungus = grindPoints[newKey];

                            var hold = facingPoint;
                            var holdPair = facingPointPair;

                            facingPoint = bungus;
                            facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                            behindPoint = hold;
                            behindPointPair = holdPair;
                        }
                        // Or coming from point 1 and heading towards last point
                        else
                        {
                            var newKey = grindPoints.Count - 1;
                            var bungus = grindPoints[newKey];

                            var hold = facingPoint;
                            var holdPair = facingPointPair;

                            facingPoint = bungus;
                            facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                            behindPoint = hold;
                            behindPointPair = holdPair;
                        }
                    }
                    // If heading towards last point in circle
                    else if (facingPointPair.Key == grindPoints.Count - 1)
                    {
                        // And coming from point 0 towards 2nd to last point
                        if (behindPointPair.Key == 0)
                        {
                            var newKey = facingPointPair.Key - 1;
                            var bungus = grindPoints[newKey];

                            var hold = facingPoint;
                            var holdPair = facingPointPair;

                            facingPoint = bungus;
                            facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                            behindPoint = hold;
                            behindPointPair = holdPair;
                        }
                        // Or coming from 2nd to last point towards point 0
                        else
                        {
                            var newKey = 0;
                            var bungus = grindPoints[newKey];

                            var hold = facingPoint;
                            var holdPair = facingPointPair;

                            facingPoint = bungus;
                            facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                            behindPoint = hold;
                            behindPointPair = holdPair;
                        }
                    }
                    // Regular Next Point facing/behind swaps
                    else
                    {
                        if (facingPointPair.Key > behindPointPair.Key)
                        {
                            var newKey = facingPointPair.Key + 1;
                            var bungus = grindPoints[newKey];

                            var hold = facingPoint;
                            var holdPair = facingPointPair;

                            facingPoint = bungus;
                            facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                            behindPoint = hold;
                            behindPointPair = holdPair;
                        }

                        if (facingPointPair.Key < behindPointPair.Key)
                        {
                            var newKey = facingPointPair.Key - 1;
                            var bungus = grindPoints[newKey];

                            var hold = facingPoint;
                            var holdPair = facingPointPair;

                            facingPoint = bungus;
                            facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                            behindPoint = hold;
                            behindPointPair = holdPair;
                        }
                    }
                }
                // Non circular point swap logic
                else
                {
                    if (facingPointPair.Key > behindPointPair.Key)
                    {
                        var newKey = facingPointPair.Key + 1;
                        var bungus = grindPoints[newKey];

                        var hold = facingPoint;
                        var holdPair = facingPointPair;

                        facingPoint = bungus;
                        facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                        behindPoint = hold;
                        behindPointPair = holdPair;
                    }

                    if (facingPointPair.Key < behindPointPair.Key)
                    {
                        var newKey = facingPointPair.Key - 1;
                        var bungus = grindPoints[newKey];

                        var hold = facingPoint;
                        var holdPair = facingPointPair;

                        facingPoint = bungus;
                        facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                        behindPoint = hold;
                        behindPointPair = holdPair;
                    }
                }
            }

            //Debug.Log("d1 - " + Vector3.Distance(mTransform.position, facingPoint));
            //Debug.Log("d2 - " + Vector3.Distance(behindPoint, facingPoint));

            var testyyy = Vector3.Distance(mTransform.position, facingPoint) - Vector3.Distance(behindPoint, facingPoint);

            //Debug.Log("This is testyyy - " + testyyy);


            mTransform.position = Vector3.MoveTowards(mTransform.position, facingPoint, testyyy / 8);

            if (movementVariables.moveAmount <= 0.5f)
            {
                if (!point0Adj)
                {
                    mTransform.position = Vector3.MoveTowards(mTransform.position, facingPoint, testyyy / 16);
                }
                else
                {
                    Debug.Log("point0Adj");
                    mTransform.position = Vector3.MoveTowards(mTransform.position, facingPoint, testyyy / 4);
                }
            }

            nextPointTimer = 0;

            frontCollider.enabled = true;
        }

        /// <summary>
        /// Generates the lists of grind points and grind centers
        /// </summary>
        /// <param name="grindColliderGen"></param>
        void GenerateGrindPoints(Collider grindColliderGen)
        {
            Debug.Log("generate grind points");

            //grindColliderGen.gameObject.transform.parent.parent.tag

            // If there's not already grind points (to catch any double calls)
            if (grindPoints.Count == 0)
            {
                var grindMaster = grindColliderGen.gameObject.transform.parent.parent;
                var points = grindMaster.GetChild(1);

                var numPoints = points.childCount;

                for (int i = 0; i < numPoints; i++)
                {
                    grindPoints.Add(i, points.GetChild(i).position);
                }

                switch (grindMaster.tag)
                {
                    case "UndergroundGrind":
                        grindType = GrindType.UNDERGROUND;
                        break;
                    case "DblUndergroundGrind":
                        grindType = GrindType.DBL_UNDERGROUND;
                        break;
                    case "CircularGrind":
                        grindType = GrindType.CIRCULAR;

                        if (grindColliderGen.name.Contains("0") && !grindColliderGen.name.Contains("1"))
                        {
                            onLastGrindSeg = true;
                        }

                        break;
                    default:
                        grindType = GrindType.STD;
                        break;
                }
            }

            // No grind points? As a pre-caution, purge everything and start over
            else
            {
                PurgeGrindPoints();
                GenerateGrindPoints(grindColliderGen);
            }
        }

        /// <summary>
        /// When exiting a grind, clear out the lists
        /// </summary>
        void PurgeGrindPoints()
        {
            onLastGrindSeg = false;
            grindPoints = new Dictionary<int, Vector3>();
        }

        public bool CanDash()
        {
            return !isSliding && (climbState == ClimbState.NONE || climbState == ClimbState.CLIMBING)/* && (isGrounded || dashInAirCounter == 0)*/ && lagDashCooldown <= 0;
        }

        public bool CanNewDash()
        {
            return !isSliding && (climbState == ClimbState.NONE || climbState == ClimbState.CLIMBING)/* && (isGrounded || dashInAirCounter == 0)*/ && lagDashCooldown <= 0;
        }

        public bool CanRun()
        {
            return !isSliding && !dashActive && (isGrounded || climbState == ClimbState.CLIMBING)/* && (isGrounded || dashInAirCounter == 0)*/ && !runRanOut /*&& speedHackRecover <= 0*/;
        }
        public bool drawPath;
        public List<Path> path;
        public ClimbingVariables climbingVariables;
        private void OnDrawGizmos()
        {

            //if(drawPath)
            //{
            //    Gizmos.color = Color.red;
            //    float c = 0;
            //    for (int i = 0; i < path.Count; ++i)
            //    {
            //       c = 0;
            //       while (c <= 1)
            //       {
            //            Gizmos.DrawSphere(Vector3.Lerp(path[i].lerper.startVal, path[i].lerper.endVal, c) + (path[i].up * 0.45f), 0.375f);
            //            c += 0.05f;
            //       }
            //    }
            //}
            //Visualize climbHit
            //var topFloat = .5f;
            //var topRay = transform.position + (transform.forward * .9f) + (transform.up * topFloat);
            //Gizmos.DrawWireSphere(topRay + transform.forward, 0.3f);

            //// Setup origin points for three different ground checking vector3s. One in middle of player, one in front, and one in back
            //Vector3 middleOrigin = transform.position;
            //Vector3 frontOrigin = transform.position;
            //Vector3 backOrigin = transform.position;

            //middleOrigin += transform.forward;
            //frontOrigin += transform.forward + transform.forward / 2;
            ////backOrigin += transform.forward / 2;

            //// Origins should be coming from inside of player
            //middleOrigin.y += .7f;
            //frontOrigin.y += .7f;
            //backOrigin.y += .7f;

            //// Dir represents the downward direction
            //Vector3 dir = -Vector3.up;


            ////TODO: testing this

            //if (isGrounded)
            //{
            //    // If player is on a sloped surface, must account for the normal
            //    dir.z = dir.z - groundNormal.z;
            //}
            //else
            //{
            //    dir.z = dir.z - transform.up.z;
            //    //dir.z = dir.z
            //}

            //// Set distance depending on if player grounded or in air
            //float dis = (isGrounded) ? groundedDis : onAirDis;
            //Gizmos.color = Color.red;
            //// Draw the rays
            //Gizmos.DrawRay(frontOrigin, dir * dis);
            //Gizmos.color = Color.blue;

            //Gizmos.DrawSphere(middleOrigin + dir * dis, 0.3f);
            //Gizmos.color = Color.green;

            //Gizmos.DrawSphere(backOrigin + dir * dis, 0.3f);

            //Gizmos.color = Color.red;
            //Gizmos.DrawCube(frontCollider.bounds.center, new Vector3(frontCollider.bounds.size.x, frontCollider.bounds.size.y, frontCollider.bounds.size.z));
            //Gizmos.DrawWireCube(new Vector3(frontCollider.bounds.center.x, frontCollider.bounds.center.y - (frontCollider.bounds.size.y - (frontCollider.bounds.size.y * 0.5f)), frontCollider.bounds.center.z),
            //    new Vector3(frontCollider.bounds.size.x * 1.5f, frontCollider.bounds.size.y * 0.5f, frontCollider.bounds.size.z * 1.5f));

            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireCube(new Vector3(backCollider.bounds.center.x, backCollider.bounds.center.y - (backCollider.bounds.size.y - (backCollider.bounds.size.y * 0.5f)), backCollider.bounds.center.z),
            //    new Vector3(backCollider.bounds.size.x * 1.5f, backCollider.bounds.size.y * 0.5f, backCollider.bounds.size.z * 1.5f));
        }

        //public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        //{
        //    if (_color != default(Color))
        //        Handles.color = _color;
        //    Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        //    using (new Handles.DrawingScope(angleMatrix))
        //    {
        //        float pointOffset = (_height - (_radius * 2)) / 2;

        //        //draw sideways
        //        Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
        //        Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
        //        Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
        //        Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
        //        //draw frontways
        //        Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
        //        Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
        //        Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
        //        Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
        //        //draw center
        //        Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
        //        Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

        //    }
        //}

        //void OnSceneGUI()
        //{
        //    Fef(frontCollider);
        //    Fef(backCollider);
        //}

        //void Fef(CapsuleCollider col)
        //{
        //    DrawWireCapsule(col.bounds.center, col.transform.rotation, col.radius * 0.9f, col.bounds.min.y);
        //}
    }
}