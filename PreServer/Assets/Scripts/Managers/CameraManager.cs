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

        void FixedUpdate()
        {
            yaw += Input.GetAxis("RightStickHorizontal") * mouseSens;
            pitch -= Input.GetAxis("RightStickVertical") * mouseSens;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            cam.value.transform.eulerAngles = currentRotation;


            Vector3 targetPosition = Vector3.Lerp(cam.value.transform.position, player.position, Time.deltaTime * camFollowSpeed);
            cam.value.transform.position = targetPosition;
        }
    }
}