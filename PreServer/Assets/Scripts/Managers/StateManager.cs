using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

namespace PreServer
{
    public class StateManager : MonoBehaviour
    {
        public State currentState;
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
        public CapsuleCollider backCollider;

        public CapsuleCollider frontColliderJump;
        public CapsuleCollider backColliderJump;

        public CapsuleCollider grindCollider;


        public bool rotateFast;

        public AnimHashes hashes;
        public AnimatorData animData;

        public bool isJumping;
        public bool dashActive;
        public int dashInAirCounter;
//bool _isGrounded;
        public bool isGrounded;
        //{
        //    get { return _isGrounded; }
        //    set
        //    {
        //        //Debug.Log(Time.frameCount + " || setting isGrounded to: " + value + " was: " + _isGrounded);
        //        _isGrounded = value;
        //    }
        //}

        public bool isColidingInAir;

        public bool isRun;
        public bool isRestart;

        [HideInInspector]
        public float timeSinceJump;
        public float timeSinceMove;
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

        public Text inGrindZoneText;
        public Text newColliderText;
        public Image dashCooldown;
        public Image speedHack;

        public bool stepUp;
        public bool stepUpJump;
        public bool UpIdle;
        public int idleRand;
        public int randMax;
        public int idleInc;
        public bool inGrindZone;
        public bool isSliding;

        public Dictionary<int, Vector3> grindPoints;
        public Dictionary<int, BoxCollider> grindCenters;

        public Vector3 facingPoint;
        public Vector3 behindPoint;

        public KeyValuePair<int, Vector3> facingPointPair;
        public KeyValuePair<int, Vector3> behindPointPair;

        public Vector3 groundNormal;
        public Vector3 frontNormal;
        public Vector3 middleNormal;
        public Vector3 backNormal;
        public Vector3 backupGroundNormal;

        public float angleTest;

        public KeyValuePair<int, BoxCollider> grindCenterPair;
        public BoxCollider grindCenter;
        public float testINT = 0;

        public bool stayGrinding;
        public bool exitingGrind;
        public BoxCollider currentHitBox;
        public BoxCollider newHitBox;

        public Camera mainCam;
        public Camera mainCam2;
        public GameObject front;
        public GameObject middle;
        public GameObject back;
        public static StateManager ptr;

        public float groundedDis = .8f;
        public float onAirDis = .85f;
        public LayerMask groundLayer;

        public bool topHitLong = false;
        public bool topHit = false;
        public bool bottomHit = false;
        public float minSlideAngle = 35;
        public float maxSlideAngle = 70;
        float length = 0;
        [HideInInspector]
        public float lagDashCooldown = 0;
        public float speedHackAmount { get; set; }
        float _groundSpeedMult = 1f;
        float _airSpeedMult = 1f;
        float _climbSpeedMult = 1f;
        float _slideSpeedMult = 1f;
        public float groundSpeedMult { get { return _groundSpeedMult; } }
        public float airSpeedMult { get { return _airSpeedMult; } }
        public float climbSpeedMult { get { return _climbSpeedMult; } }
        public float slideSpeedMult { get { return _slideSpeedMult; } }
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
            stayGrinding = false;

            grindPoints = new Dictionary<int, Vector3>();
            grindCenters = new Dictionary<int, BoxCollider>();

            currentHitBox = new BoxCollider();
            newHitBox = new BoxCollider();
            exitingGrind = false;
            speedHackAmount = 2f;
        }

        public float GetLength()
        {
            return length;
        }

        private void FixedUpdate()
        {
            if (isRestart)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            delta = Time.fixedDeltaTime;

            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }
        bool runRanOut = false;
        private void Update()
        {
            delta = Time.deltaTime;
            if (climbState == ClimbState.NONE)
            {
                UpdateGroundNormals();
                SetAnimStates();
                StepUpTest();
                CheckSliding();
                CheckForClimb();
            }
            else
            {
                SetClimbAnimStates();
            }
            //Reset lagdash when squirrel touches the ground
            if (isGrounded && dashInAirCounter != 0)
                dashInAirCounter = 0;

            if (currentState != null)
            {
                currentState.Tick(this);
            }

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
                if (movementVariables.moveAmount == 0)
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
            if (lagDashCooldown > 0)
            {
                lagDashCooldown -= delta;
                if (lagDashCooldown < 0)
                    lagDashCooldown = 0;
            }
            if (isRun && !runRanOut)
            {
                speedHackAmount -= delta;
                if (speedHackAmount <= 0)
                {
                    speedHackAmount = 0;
                    runRanOut = true;
                }
                _groundSpeedMult = 2.5f;
                _airSpeedMult = 2f;
                _climbSpeedMult = 2f;
                _slideSpeedMult = 2f;
            }
            else if(speedHackAmount < 2f)
            {
                speedHackAmount += runRanOut ? delta * 0.5f : delta;
                if (speedHackAmount >= 2f)
                {
                    speedHackAmount = 2f;
                    runRanOut = false;
                }
                _groundSpeedMult = 1f;
                _airSpeedMult = 1f;
                _climbSpeedMult = 1f;
                _slideSpeedMult = 1f;
            }

            #region UI
            groundAngle.text = "Ground Angle: " + Vector3.Angle(groundNormal, Vector3.up).ToString("F2");
            playerAngle.text = "Player Angle: " + Vector3.Angle(mTransform.up, Vector3.up).ToString("F2");

            PlayerVolX.text = "X: " + rigid.velocity.x.ToString("F2");
            PlayerVolY.text = "Y: " + rigid.velocity.y.ToString("F2");
            PlayerVolZ.text = "Z: " + rigid.velocity.z.ToString("F2");

            PlayerPosX.text = "X: " + mTransform.position.x.ToString("F2");
            PlayerPosY.text = "Y: " + mTransform.position.y.ToString("F2");
            PlayerPosZ.text = "Z: " + mTransform.position.z.ToString("F2");

            TargetVolX.text = "X: " + targetVelocity.x.ToString("F2");
            TargetVolY.text = "Y: " + targetVelocity.y.ToString("F2");
            TargetVolZ.text = "Z: " + targetVelocity.z.ToString("F2");

            inGrindZoneText.text = "inGrindZone - " + inGrindZone;

            if (newHitBox == new BoxCollider())
            {
                newColliderText.text = "newCollider - new box";
            }
            else if (newHitBox == null)
            {
                newColliderText.text = "newCollider - null";
            }
            else
            {
                newColliderText.text = "newCollider - " + newHitBox.name;
            }
            dashCooldown.fillAmount = 1 - lagDashCooldown;
            speedHack.fillAmount = speedHackAmount / 2f;
            #endregion
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

            var bottomRay = mTransform.position + (mTransform.forward * 1.25f) + (Vector3.up * bottomFloat);
            var topRay = mTransform.position + (mTransform.forward * 1.25f) + (Vector3.up * topFloat);
            var topRayLong = mTransform.position + (mTransform.forward * 2.5f) + (Vector3.up * topFloatLong);

            //bool bottomHit;
            //bool topHit;
            //bool topHitLong;

            RaycastHit hitBottom = new RaycastHit();
            RaycastHit hitTop = new RaycastHit();
            RaycastHit hitTopLong = new RaycastHit();

            if (stepUp)
            {
                Debug.DrawRay(bottomRay, mTransform.forward, Color.green);
                Debug.DrawRay(topRay, mTransform.forward, Color.green);
                Debug.DrawRay(topRayLong, mTransform.forward, Color.green);
            }
            else
            {
                Debug.DrawRay(bottomRay, mTransform.forward, Color.blue);
                Debug.DrawRay(topRay, mTransform.forward, Color.blue);
                Debug.DrawRay(topRayLong, mTransform.forward, Color.blue);
            }

            if (Physics.Raycast(bottomRay, mTransform.forward, out hitBottom, 1, Layers.ignoreLayersController))
                bottomHit = true;
            else
                bottomHit = false;

            if (Physics.Raycast(topRay, mTransform.forward, out hitTop, 1, Layers.ignoreLayersController))
                topHit = true;
            else
                topHit = false;

            if (Physics.Raycast(topRayLong, mTransform.forward, out hitTopLong, 1, Layers.ignoreLayersController))
                topHitLong = true;
            else
                topHitLong = false;


            if (bottomHit && !topHit && !topHitLong && movementVariables.moveAmount > 0.05f)
            {
                stepUp = true;
                frontCollider.enabled = false;
                frontColliderJump.enabled = true;
            }
            else
            {
                //states.stepUpDelay = true;
                stepUp = false;
                frontCollider.enabled = true;
                frontColliderJump.enabled = false;
            }
        }

        public void UpdateGroundNormals()
        {
            // Setup origin points for three different ground checking vector3s. One in middle of player, one in front, and one in back
            Vector3 middleOrigin = transform.position;
            Vector3 frontOrigin = transform.position;
            Vector3 backOrigin = transform.position;

            middleOrigin += transform.forward;
            frontOrigin += transform.forward + transform.forward / 2;
            //backOrigin += states.mTransform.forward / 2;

            // Origins should be coming from inside of player
            middleOrigin.y += .7f;
            frontOrigin.y += .7f;
            backOrigin.y += .7f;

            // Dir represents the downward direction
            Vector3 dir = -Vector3.up;


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
            Debug.DrawRay(frontOrigin, dir * dis, Color.green);
            Debug.DrawRay(middleOrigin, dir * dis, Color.yellow);
            Debug.DrawRay(backOrigin, dir * dis, Color.white);

            // If player is already grounded, check if they should remain
            //if (states.isGrounded)
            //{

            float angle = 0;
            frontSliding = false;
            middleSliding = false;
            backSliding = false;
            if (Physics.Raycast(frontOrigin, dir, out frontHit, dis + 0.3f, Layers.ignoreLayersController))
            {
                frontNormal = frontHit.normal;
                angle = Vector3.Angle(frontHit.normal, Vector3.up);
                if (angle >= 70)
                    front = null;
                else
                {
                    if (angle > 35)
                        frontSliding = true;
                    front = frontHit.transform.gameObject;
                }
            }
            else
            {
                front = null;
            }

            if (Physics.SphereCast(middleOrigin, 0.3f, dir, out middleHit, dis, Layers.ignoreLayersController))
            {
                middleNormal = middleHit.normal;
                angle = Vector3.Angle(middleHit.normal, Vector3.up);
                if (angle >= 70)
                    middle = null;
                else
                    middle = middleHit.transform.gameObject;
            }
            else
            {
                middle = null;
            }

            if (Physics.SphereCast(backOrigin, 0.3f, dir, out backHit, dis, Layers.ignoreLayersController))
            {
                backNormal = backHit.normal;
                angle = Vector3.Angle(backHit.normal, Vector3.up);
                if (angle >= 70)
                    back = null;
                else
                    back = backHit.transform.gameObject;
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

            if ((CheckGrounded(frontCollider) || CheckGrounded(backCollider)) && (!front && !middle && !back) && !isGrounded && bottomHit)
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
            if (Physics.Raycast(middleOrigin, dir, out middleHit, dis + 0.3f, Layers.ignoreLayersController))
            {
                angle = Vector3.Angle(middleHit.normal, Vector3.up);
                if (angle > 35 && angle < 70)
                    middleSliding = true;
            }
            if (Physics.Raycast(backOrigin, dir, out backHit, dis + 0.3f, Layers.ignoreLayersController))
            {
                angle = Vector3.Angle(backHit.normal, Vector3.up);
                if (angle > 35 && angle < 70)
                    backSliding = true;
            }
            if (middleSliding)
            {
                Vector3 forward = Vector3.Cross(middleHit.normal, Vector3.up);
                slideDirection = Vector3.Cross(forward, middleHit.normal);
            }
            else if (frontSliding)
            {
                Vector3 forward = Vector3.Cross(frontHit.normal, Vector3.up);
                slideDirection = Vector3.Cross(forward, frontHit.normal);
            }
            else if (backSliding)
            {
                Vector3 forward = Vector3.Cross(backHit.normal, Vector3.up);
                slideDirection = Vector3.Cross(forward, backHit.normal);
            }
            else
            {
                //slideDirection = Vector3.zero;
            }
        }

        //Checks to see if the collider is interacting with anything on the default layer '0'
        //https://www.youtube.com/watch?v=vdOFUFMiPDU
        bool CheckGrounded(CapsuleCollider col)
        {
            //return Physics.CheckBox(new Vector3(col.bounds.center.x, col.bounds.center.y - (col.bounds.size.y - (col.bounds.size.y * 0.5f)), col.bounds.center.z), new Vector3(col.bounds.size.x * 1.5f, col.bounds.size.y * 0.5f, col.bounds.size.z * 1.5f) * 0.5f, col.transform.rotation, groundLayer);
            return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 0.9f, groundLayer);
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
            var topFloat = .6f;
            var topRay = mTransform.position + (mTransform.forward * 1.25f) + (Vector3.up * topFloat);

            //Debug.DrawRay(topRay, mTransform.forward, Color.blue);
            if (Physics.Raycast(topRay, mTransform.forward, out climbHit, 1, Layers.ignoreLayersController))
            {
                float angle = Vector3.Angle(climbHit.normal, Vector3.up);
                if (angle >= 70 && angle <= 90 && climbHit.transform.tag == "Climb"/* && (prevClimbT != climbHit.transform || isGrounded)*/)
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

            Vector3 origin = transform.position/* + (transform.forward*1.5f)*/;
            // Origin should be coming from inside of player
            //Vector3 dir = /*transform.rotation.eulerAngles.x < 180 ? slideDirection : */-slideDirection;
            //Debug.Log(transform.rotation.eulerAngles.x + "  " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z);
            if (transform.rotation.eulerAngles.x < 180)
            {
                //slideDirection = -slideDirection;
                origin = transform.position;
                origin += transform.forward * 2;
            }
            else
            {
                //slideDirection = -transform.forward;
                origin = transform.position;
            }
            //dir = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up) * dir;
            origin.y += .35f;
            //Debug.Log(dir.x + "  " + dir.y + " " + dir.z);
            // Set distance depending on if player grounded or in air
            //float dis = 0.3f;

            // RaycastHits for each grounding ray
            RaycastHit hit = new RaycastHit();

            // Draw the rays
            //Debug.DrawRay(origin, -slideDirection * (GetLength() * 0.65f), Color.black);

            // If player is already grounded, check if they should remain
            //if (states.isGrounded)
            //{


            if ((frontSliding || middleSliding || backSliding))
            {
                bool backCastHit = Physics.Raycast(origin, -slideDirection, out hit, GetLength() * 0.65f, Layers.ignoreLayersController);
                if (backCastHit)
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    //If the back cast hit another slide, then you're still in the sliding state
                    if (angle > 35 && angle < 70)
                        backCastHit = false;
                }
                isSliding = !backCastHit;
            }
            else
            {
                isSliding = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Grind")
            {
                //Debug.Log("In grind zone is - " + inGrindZone + " || exiting grind is - " + exitingGrind);

                if (!inGrindZone)
                {
                    if (!exitingGrind)
                    {
                        inGrindZone = true;

                        GenerateGrindPoints(other);
                        currentHitBox = (BoxCollider) other;

                        // If there's a turn
                        if (grindPoints.Count == 2)
                        {
                            BoxCollider[] allChildren = other.GetComponentsInChildren<BoxCollider>();

                            foreach (BoxCollider child in allChildren)
                            {
                                if (child.tag == "GrindCenter")
                                {
                                    grindCenter = child;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // If new Grind collider that is not the current one
                    if (other != currentHitBox)
                    {
                        newHitBox = (BoxCollider) other;
                    }
                }
            }

            if (other.tag == "GrindAdditional")
            {
                if (!inGrindZone)
                {
                    if (!exitingGrind)
                    {
                        inGrindZone = true;

                        // Get all colliders in parent (aka the main grind hitbox)
                        BoxCollider[] parentColliders = other.GetComponentsInParent<BoxCollider>();

                        foreach (BoxCollider bc in parentColliders)
                        {
                            // If it's the main grind hitbox
                            if (bc.tag == "Grind")
                            {
                                // Generate the grind points
                                GenerateGrindPoints(bc);
                                currentHitBox = (BoxCollider)other;
                            }
                        }

                        if (grindPoints.Count == 2)
                        {
                            BoxCollider[] allChildren = other.GetComponentsInChildren<BoxCollider>();

                            foreach (BoxCollider child in allChildren)
                            {
                                if (child.tag == "GrindCenter")
                                {
                                    grindCenter = child;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Debug.Log("Entered the AddColl");

                    // If new Grind collider that is not the current one
                    if (other != currentHitBox)
                    {
                        newHitBox = (BoxCollider)other;
                    }
                }
            }

            // If player enters grind point
            if (other.tag == "GrindPoint")
            {
                // Start moving towards next point
                Debug.Log("HELLO");
                NextPoint();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if ((other.tag == "Grind" || other.tag == "GrindAdditional") && (currentState.stateName == "Grinding" || currentState.stateName == "On Air"))
            {
                if (grindPoints.Count == 2)
                {
                    PurgeGrindPoints();
                    inGrindZone = false;
                    exitingGrind = true;
                }

                else
                {
                    if (newHitBox == new BoxCollider() || newHitBox == null)
                    {
                        PurgeGrindPoints();
                        inGrindZone = false;
                        exitingGrind = true;
                    }
                    else
                    {
                        newHitBox = new BoxCollider();
                    }
                }
            }
        }

        public void NextPoint()
        {        
            if (grindPoints.Count > 2)
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

                    var trungo = grindCenterPair.Key + 1;
                    grindCenter = grindCenters[trungo];
                    grindCenterPair = new KeyValuePair<int, BoxCollider>(trungo, grindCenter);
                }

                if (facingPointPair.Key < behindPointPair.Key)
                {
                    var newKey = facingPointPair.Key - 1;
                    var bungus = grindPoints[facingPointPair.Key - 1];

                    var hold = facingPoint;
                    var holdPair = facingPointPair;

                    facingPoint = bungus;
                    facingPointPair = new KeyValuePair<int, Vector3>(newKey, bungus);
                    behindPoint = hold;
                    behindPointPair = holdPair;

                    var trungo = grindCenterPair.Key - 1;
                    grindCenter = grindCenters[trungo];
                    grindCenterPair = new KeyValuePair<int, BoxCollider>(trungo, grindCenter);
                }


                currentHitBox = newHitBox;
                //newHitBox = new BoxCollider();
            }      
        }

        void GenerateGrindPoints(Collider grindColliderGen)
        {
            if (grindPoints.Count == 0)
            {
                foreach (Transform child in grindColliderGen.gameObject.transform)
                {
                    if (child.tag == "GrindPoint")
                    {
                        grindPoints.Add(child.GetSiblingIndex(), child.position);
                    }
                
                }

                if (grindPoints.Count > 2)
                {
                    var indexCount = 0;

                    var allColliders = grindColliderGen.gameObject.GetComponentsInChildren<BoxCollider>();

                    foreach (var child in allColliders)
                    {
                        if (child.tag == "GrindCenter")
                        {
                            grindCenters.Add(indexCount, child);
                            indexCount++;
                        }
                    }
                }
            }
            else
            {
                PurgeGrindPoints();
                GenerateGrindPoints(grindColliderGen);
            }
        }

        void PurgeGrindPoints()
        {
            grindPoints = new Dictionary<int, Vector3>();
            grindCenters = new Dictionary<int, BoxCollider>();
            grindCenter = new BoxCollider();
        }

        public bool CanDash()
        {
            return !isSliding && (climbState == ClimbState.NONE || climbState == ClimbState.CLIMBING)/* && (isGrounded || dashInAirCounter == 0)*/ && lagDashCooldown <= 0;
        }

        public bool CanRun()
        {
            return !dashActive && (climbState == ClimbState.NONE || climbState == ClimbState.CLIMBING)/* && (isGrounded || dashInAirCounter == 0)*/ && !runRanOut;
        }

        private void OnDrawGizmos()
        {
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