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
        bool _isGrounded;
        public bool isGrounded
        {
            get { return _isGrounded; }
            set
            {
                Debug.Log(Time.frameCount + " || setting isGrounded to: " + value + " was: " + _isGrounded);
                _isGrounded = value;
            }
        }
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
        public GameObject front;
        public GameObject middle;
        public GameObject back;
        public static StateManager ptr;

        private void Start()
        {
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
            UpdateGroundNormals();
            SetAnimStates();
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

        public float groundedDis = .8f;
        public float onAirDis = .85f;
        public LayerMask groundLayer;
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
            RaycastHit middleHit = new RaycastHit();
            RaycastHit frontHit = new RaycastHit();
            RaycastHit backHit = new RaycastHit();

            // Draw the rays
            Debug.DrawRay(middleOrigin, dir * dis, Color.green);
            Debug.DrawRay(frontOrigin, dir * dis, Color.yellow);
            Debug.DrawRay(backOrigin, dir * dis, Color.white);
            //Debug.Log(Time.frameCount + " || Front Collider Grounded: " + isGrounded(states.frontCollider));
            // If player is already grounded, check if they should remain
            //if (states.isGrounded)
            //{
            float angle = 0;
            if (Physics.SphereCast(middleOrigin, 0.3f, dir, out middleHit, dis, Layers.ignoreLayersController))
            {
                middleNormal = middleHit.normal;
                //states.middle = middleHit.transform.gameObject;
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

            if (Physics.Raycast(frontOrigin, dir, out frontHit, dis + 0.3f, Layers.ignoreLayersController))
            {
                frontNormal = frontHit.normal;
                //states.front = frontHit.transform.gameObject;
                angle = Vector3.Angle(frontHit.normal, Vector3.up);
                if (angle >= 70)
                    front = null;
                else
                    front = frontHit.transform.gameObject;
            }
            else
            {
                front = null;
            }

            if (Physics.SphereCast(backOrigin, 0.3f, dir, out backHit, dis, Layers.ignoreLayersController))
            {
                backNormal = backHit.normal;
                //states.back = backHit.transform.gameObject;
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

            isGrounded = (CheckGrounded(frontCollider) || CheckGrounded(backCollider));
        }

        //Checks to see if the collider is interacting with anything on the default layer '0'
        //https://www.youtube.com/watch?v=vdOFUFMiPDU
        bool CheckGrounded(CapsuleCollider col)
        {
            //return Physics.CheckBox(new Vector3(col.bounds.center.x, col.bounds.center.y - (col.bounds.size.y - (col.bounds.size.y * 0.5f)), col.bounds.center.z), new Vector3(col.bounds.size.x * 1.5f, col.bounds.size.y * 0.5f, col.bounds.size.z * 1.5f) * 0.5f, col.transform.rotation, groundLayer);
            return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 1.5f, groundLayer);
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
            ////Gizmos.DrawCube(frontCollider.bounds.center, new Vector3(frontCollider.bounds.size.x, frontCollider.bounds.size.y, frontCollider.bounds.size.z));
            //Gizmos.DrawWireCube(new Vector3(frontCollider.bounds.center.x, frontCollider.bounds.center.y - (frontCollider.bounds.size.y - (frontCollider.bounds.size.y * 0.5f)), frontCollider.bounds.center.z),
            //    new Vector3(frontCollider.bounds.size.x * 1.5f, frontCollider.bounds.size.y * 0.5f, frontCollider.bounds.size.z * 1.5f));

            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireCube(new Vector3(backCollider.bounds.center.x, backCollider.bounds.center.y - (backCollider.bounds.size.y - (backCollider.bounds.size.y * 0.5f)), backCollider.bounds.center.z),
            //    new Vector3(backCollider.bounds.size.x * 1.5f, backCollider.bounds.size.y * 0.5f, backCollider.bounds.size.z * 1.5f));
        }

        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                float pointOffset = (_height - (_radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

            }
        }

        void OnSceneGUI()
        {
            Fef(frontCollider);
            Fef(backCollider);
        }

        void Fef(CapsuleCollider col)
        {
            DrawWireCapsule(col.bounds.center, col.transform.rotation, col.radius * 0.9f, col.bounds.min.y);
        }
    }
}