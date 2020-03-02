using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class OldCamera : MonoBehaviour
    {
        public TransformVariable cam;
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
        float yaw;
        float pitch;
        public bool ignoreInput
        {
            get { return _ignoreInput; }
            set
            {
                if (!inCameraZone || value)
                    _ignoreInput = value;
            }
        }
        bool _ignoreInput = false;
        public bool ignoreMouse;
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
            if (!inCameraZone || fromCameraZone)
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
            temp = camTransform.position;
        }
        Vector3 temp;
        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (ignoreMouse)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    ignoreMouse = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    ignoreMouse = true;
                }
            }

            if (!ignoreInput)
            {
                if (ignoreMouse)
                {
                    yaw += ignoreYaw ? 0 : (Input.GetAxis("RightStickHorizontal")) * mouseSens;
                    pitch -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical")) * mouseSens;
                }
                else
                {
                    //Cursor.lockState = CursorLockMode.Locked;
                    //Cursor.visible = false;
                    yaw += ignoreYaw ? 0 : (Input.GetAxis("RightStickHorizontal") + (Input.GetAxis("Mouse X") * .2f)) * mouseSens;
                    pitch -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical") + (Input.GetAxis("Mouse Y") * .2f)) * mouseSens;
                }
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            }

            if (!onRails)
            {
                currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                cam.value.transform.eulerAngles = currentRotation;

                Vector3 targetPosition = Vector3.Lerp(cam.value.transform.position, player.position, Time.deltaTime * camFollowSpeed);
                cam.value.transform.position = targetPosition;

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
            }
            //Debug.Log("CameraManager currentRotation: " + currentRotation);
        }
    }
}