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

        Vector3 currentRotation;
        Vector3 rotationSmoothVelocity;
        float yaw;
        float pitch;
        public bool ignoreInput = false;
        public bool ignoreMouse = false;
        public float GetYaw()
        {
            return yaw;
        }

        public void AddToYaw(float val)
        {
            yaw += val;
        }

        public void SetCurrentRotation(Vector3 v)
        {
            currentRotation = v;
        }

        void FixedUpdate()
        {
            if (!ignoreInput)
            {
                if (ignoreMouse)
                {
                    yaw += (Input.GetAxis("RightStickHorizontal")) * mouseSens;
                    pitch -= (Input.GetAxis("RightStickVertical")) * mouseSens;
                    pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    yaw += (Input.GetAxis("RightStickHorizontal") + (Input.GetAxis("Mouse XX") * .2f)) * mouseSens;
                    pitch -= (Input.GetAxis("RightStickVertical") + (Input.GetAxis("Mouse YY") * .2f)) * mouseSens;
                    pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                }
            }

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            cam.value.transform.eulerAngles = currentRotation;


            Vector3 targetPosition = Vector3.Lerp(cam.value.transform.position, player.position, Time.deltaTime * camFollowSpeed);
            cam.value.transform.position = targetPosition;
            //Debug.Log("CameraManager currentRotation: " + currentRotation);
        }
    }
}