using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class CameraManager : MonoBehaviour
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
        float yaw;
        float pitch;
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
        }

        void FixedUpdate()
        {
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
                currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                cam.value.transform.eulerAngles = currentRotation;

                Vector3 targetPosition = Vector3.Lerp(cam.value.transform.position, player.position, Time.deltaTime * camFollowSpeed);
                cam.value.transform.position = targetPosition;

                //RaycastHit camHit;
                //Vector3 dir = (camTransform.position - transform.position).normalized;
                //float distance = Vector3.Distance(transform.position, transform.position + camOffset); 
                //Debug.DrawRay(transform.position, dir * distance, Color.green);
                //if(Physics.Raycast(transform.position, dir, out camHit, distance, 1, QueryTriggerInteraction.Ignore))
                //{
                //    targetPosition =  camTransform.InverseTransformPoint(camHit.point - (dir * 0.5f));
                //    Debug.DrawRay(targetPosition + transform.position, Vector3.up * 10f, Color.red, 3f);
                //    //Debug.Log(targetPosition);
                //}
                //else
                //{
                targetPosition = Vector3.Lerp(camTransform.localPosition, camOffset, Time.deltaTime * camZoomSpeed);
                //}
                camTransform.localPosition = targetPosition;
            }
            //Debug.Log("CameraManager currentRotation: " + currentRotation);
        }
    }
}