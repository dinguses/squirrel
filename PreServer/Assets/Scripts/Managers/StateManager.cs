using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        public bool isGrounded;
        public bool followMeOnFixedUpdate;

        public bool isRun;
        public bool isRestart;
        public bool isZoom;

        public bool doneZomming = false;

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

        public bool stepUp;
        public bool stepUpJump;
        public bool UpIdle;
        public int idleRand;
        public int randMax;
        public int idleInc;
        public bool inGrindZone;

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

        private void Start()
        {
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

        private void Update()
        {
            delta = Time.deltaTime;

            if (currentState != null)
            {
                currentState.Tick(this);
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
            #endregion

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
    }
}