using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class CameraManager : MonoBehaviour
    {
        public Transform player;

        public float mouseSens = 10;
        public float rotationSmoothTime = .12f;
        public Vector2 pitchMinMax = new Vector2(-35, 35);
        public float camFollowSpeed = 9;
        public float camZoomSpeed = 4;
        public Vector3 camOffset = new Vector3(0, 4, -15);
        public Transform camTransform;
        Vector3 currentRotation;
        Vector3 rotationSmoothVelocity;
        public Vector3 camRel;
        public bool debugPauseCamLerp;
        public bool debugTeleport;
        public float maxDistance = 3f;
        public float minDistance = 0.5f;
        float yaw;
        float pitch;
        public Transform test;
        public bool ignoreInput
        {
            get { return _ignoreInput; }
            set
            {
                if(!inCameraZone || value)
                    _ignoreInput = value;
            }
        }
        bool _ignoreInput = false;
        public bool ignoreMouse = false;
        public bool inCameraZone
        {
            get { return _inCameraZone; }
            set
            {
                _inCameraZone = value;
                ignoreInput = value;
            }
        }
        bool _inCameraZone = false;

        public bool ignorePitch
        {
            get { return _ignorePitch; }
            set
            {
                if (!inCameraZone || value)
                    _ignorePitch = value;
            }
        }
        bool _ignorePitch = false;

        public bool ignoreYaw
        {
            get { return _ignoreYaw; }
            set
            {
                if (!inCameraZone || value)
                    _ignoreYaw = value;
            }
        }
        bool _ignoreYaw = false;

        public bool onRails
        {
            get { return _onRails; }
            set
            {
                _onRails = value;
            }
        }
        bool _onRails = false;

        public float GetYaw()
        {
            return yaw;
        }

        public void AddToYaw(float val, bool fromCameraZone = false)
        {
            if(!inCameraZone || fromCameraZone)
                yaw += val;
        }

        public float GetPitch()
        {
            return pitch;
        }

        public void AddToPitch(float val, bool fromCameraZone = false)
        {
            if (!inCameraZone || fromCameraZone)
            {
                pitch += val;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            }
        }

        public void SetCurrentRotation(Vector3 v)
        {
            currentRotation = v;
        }

        private void Start()
        {
            camTransform = Camera.main.transform;
            //camTransform.position = transform.position;
            //transform.position = player.position;
            temp = camTransform.position;
            test.position = temp;
            startAway = distanceAway;
            startUp = 0;
        }
        Vector3 temp;
        void FixedUpdate()
        {
            //Debug.Log(Vector3.Distance(player.position, camTransform.position));
            if (!ignoreInput)
            {
                if (ignoreMouse)
                {
                    yaw += ignoreYaw ? 0 : (Input.GetAxis("RightStickHorizontal")) * mouseSens;
                    pitch -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical")) * mouseSens;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    yaw += ignoreYaw ? 0 : (Input.GetAxis("RightStickHorizontal") + (Input.GetAxis("Mouse XX") * .2f)) * mouseSens;
                    pitch -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical") + (Input.GetAxis("Mouse YY") * .2f)) * mouseSens;
                }
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            }

            if (!onRails)
            {
                //MoveCamera_Legacy();
                //Test();
                Test2();
            }
            //Debug.Log("CameraManager currentRotation: " + currentRotation);
        }

        public float distanceAway;
        public float distanceUp;
        public float smooth;
        public Transform follow; //using player position
        Vector3 targetPos;
        Vector3 lookDir;
        Vector3 velocityCamSmooth = Vector3.zero;
        float camSmoothDampTime = 0.1f;
        float startUp;
        float startAway;
        void Test2()
        {
            yaw += (Input.GetAxis("RightStickHorizontal")) * mouseSens;
            distanceUp -= (Input.GetAxis("RightStickVertical"));
            distanceUp = Mathf.Clamp(distanceUp, -10, 10);
            distanceAway = startAway - (startUp >= distanceUp ? startUp - distanceUp : distanceUp - startUp);
            distanceAway = Mathf.Clamp(distanceAway, 5, 15);
            transform.RotateAround(player.position, Vector3.up, yaw - prevYaw);
            //transform.RotateAround(player.position, camTransform.right, -(testPitch - prevTestPitch));

            Vector3 characterOffset = player.position + player.up * 0.25f;

            lookDir = characterOffset - transform.position;
            lookDir.y = 0;
            lookDir.Normalize();

            //targetPos = player.position + player.up * distanceUp - player.forward * distanceAway;
            targetPos = characterOffset + Vector3.up * distanceUp - lookDir * distanceAway;
            //Debug.DrawRay(player.position, player.up * distanceUp, Color.red);
            //Debug.DrawRay(player.position, -1f * player.forward * distanceAway, Color.blue);
            //if (Physics.CheckSphere(targetPos, 0.5f, 1, QueryTriggerInteraction.Ignore))
            //{
                CompensateForWalls(characterOffset, ref targetPos);
            //}
            //Debug.DrawLine(player.position, targetPos, Color.magenta);

            smoothPosition(transform.position, targetPos);
            //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);
            camTransform.LookAt(player);
            prevPitch = pitch;
            prevYaw = yaw;
        }

        void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
        {
            RaycastHit wallHit = new RaycastHit();
            if(Physics.Linecast(fromObject, toTarget, out wallHit, 1, QueryTriggerInteraction.Ignore))
            {
                toTarget = wallHit.point;
            }
        }

        void smoothPosition(Vector3 fromPos, Vector3 toPos)
        {
            transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
        }

        float prevYaw = 0;
        float prevPitch = 0;
        Vector3 testPosition;
        Vector3 playerPrevPosition;        
        void Test()
        {
            //testYaw += (Input.GetAxis("RightStickHorizontal")) * mouseSens;
            //testPitch -= (Input.GetAxis("RightStickVertical")) * mouseSens;
            //testPitch = Mathf.Clamp(testPitch, pitchMinMax.x, pitchMinMax.y);

            transform.RotateAround(player.position, Vector3.up, yaw - prevYaw);
            transform.RotateAround(player.position, transform.right, pitch - prevPitch);
            Vector3 targetPosition = transform.position;
            //Debug.Log(Vector3.Distance(player.position, camTransform.position));
            if (Vector3.Distance(player.position, transform.position) >= 20f/* || Vector3.Distance(player.position, camTransform.position) <= 5f*/)
            {
                Vector3 dir = player.position - playerPrevPosition;
                if (dir == Vector3.zero)
                    dir = (player.position - transform.position) * Time.deltaTime;
                targetPosition += dir;
            }
            else if (Vector3.Distance(player.position, transform.position) <= 5f/* || Vector3.Distance(player.position, camTransform.position) <= 5f*/)
            {
                Vector3 dir = player.position - playerPrevPosition;
                if (dir == Vector3.zero)
                    dir = -(player.position - transform.position) * Time.deltaTime;
                targetPosition += dir;
            }
            if (Physics.CheckSphere(targetPosition, 0.5f, 1, QueryTriggerInteraction.Ignore))
            {
                RaycastHit camHit;
                Vector3 dir = (targetPosition - (player.position + Vector3.up * 0.25f)).normalized;
                float distance = Vector3.Distance(player.position + Vector3.up * 0.25f, targetPosition);
                if (Physics.Raycast(player.position + (Vector3.up * 0.25f), dir, out camHit, distance, 1, QueryTriggerInteraction.Ignore))
                    targetPosition = camHit.point - (Vector3.Distance(camHit.point, player.position) > 1f ? (dir * 0.3f) : Vector3.zero);
            }

            transform.position = targetPosition;
            Debug.DrawLine(player.position, camTransform.position, Color.red);

            prevPitch = pitch;
            prevYaw = yaw;
            camTransform.LookAt(player.position);
            playerPrevPosition = player.position;
        }

        void MoveCamera_Legacy()
        {
            Vector3 targetPosition = Vector3.zero;
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;
            if (Vector3.Distance(player.position, camTransform.position) >= maxDistance || Vector3.Distance(player.position, camTransform.position) <= minDistance)
            {
                targetPosition = Vector3.Lerp(transform.position, player.position, Time.deltaTime * camFollowSpeed);
                transform.position = targetPosition;
            }
            if (debugTeleport)
            {
                transform.position = player.position;
                debugTeleport = false;
            }
            //Check if the camera is colliding with anything
            if (Physics.CheckSphere(camTransform.position, 0.5f, 1, QueryTriggerInteraction.Ignore))
            {
                RaycastHit camHit;
                Vector3 dir = (camTransform.position - (transform.position + Vector3.up * 0.25f)).normalized;
                float distance = Vector3.Distance(transform.position + Vector3.up * 0.25f, transform.position + camOffset);
                //Debug.DrawRay(transform.position + Vector3.up * 0.25f, dir * distance, Color.green);
                //Debug.DrawLine(transform.position + (Vector3.up * 0.25f), temp, Color.green);

                //if it is then do a raycast from the player to the max camera position
                //if the raycast hits, then move the camera up to the point of contact and move it slightly forward based on distance from the player
                if (Physics.Raycast(transform.position + (Vector3.up * 0.25f), dir, out camHit, distance, 1, QueryTriggerInteraction.Ignore))
                    targetPosition = transform.InverseTransformPoint(camHit.point - (Vector3.Distance(camHit.point, player.position) > 1f ? (dir * 0.3f) : Vector3.zero));
                else
                    targetPosition = Vector3.Lerp(camTransform.localPosition, camOffset, Time.deltaTime * camZoomSpeed);

                //temp = transform.position + targetPosition - dir;
                //Debug.DrawRay(camHit.point, camHit.normal * 3f, Color.red);
                //Debug.Log(targetPosition);
            }
            else
            {
                //temp = Vector3.Lerp(temp, camTransform.position, Time.deltaTime * camZoomSpeed);
                targetPosition = Vector3.Lerp(camTransform.localPosition, camOffset, Time.deltaTime * camZoomSpeed);
                //Debug.Log(targetPosition);
            }
            //Debug.DrawRay(temp, Vector3.up * 3f, Color.yellow);
            if (!debugPauseCamLerp)
                camTransform.localPosition = targetPosition;
            camTransform.LookAt(player.position);
        }

        private void OnDrawGizmos()
        {

        }
    }
}