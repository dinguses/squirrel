using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class CameraZone : MonoBehaviour
    {
        CameraManager cameraMan;
        float cameraAngle = 0;
        float tempAngle = 0;
        bool moveCamera = false;
        public enum InputLimit { NONE, YAW, PITCH, ALL }
        public InputLimit limit = InputLimit.NONE;
        public Transform[] points;
        float[] distances;
        public Transform start;
        public Transform end;
        float totalDist = 0;
        float distFromStart = 0;
        bool triggered = false;
        int currentPoint = 0;
        // Start is called before the first frame update
        void Start()
        {
            cameraMan = Camera.main.transform.parent.GetComponent<CameraManager>();
            if (OnRails())
            {
                totalDist = Vector3.Distance(start.position, end.position);
                distances = new float[points.Length - 1];
                float pointsTotalDist = 0;
                for(int i = 0; i < distances.Length; ++i)
                {
                    distances[i] = Vector3.Distance(points[i].position, points[i + 1].position);
                    pointsTotalDist += distances[i];
                    if(i != 0)
                    {
                        distances[i] += distances[i - 1];
                    }
                }
                for (int i = 0; i < distances.Length; ++i)
                {
                    distances[i] /= pointsTotalDist;
                }
            }
        }

        public bool OnRails()
        {
            return points != null && points.Length > 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (moveCamera && !OnRails())
            {
                if (cameraMan != null && tempAngle < 180 && tempAngle > -180)
                {
                    cameraMan.AddToYaw((cameraAngle * Time.deltaTime * 4), true);
                    tempAngle -= (cameraAngle * Time.deltaTime * 4);
                    Debug.Log(tempAngle + " || " + cameraAngle);
                }
                else
                {
                    moveCamera = false;
                    if (cameraMan != null)
                        cameraMan.ignoreInput = false;
                }
            }
        }

        private void FixedUpdate()
        {
            if (OnRails() && triggered)
            {
                Vector3 playerPosition = GetPoint(cameraMan.player.position, start.position, end.position);
                //Debug.DrawRay(playerPosition, Vector3.right * 4f, Color.black);
                distFromStart = Vector3.Distance(start.position, playerPosition) / totalDist;
                for (int i = 0; i < distances.Length; ++i)
                {
                    if (distFromStart < distances[i])
                    {
                        currentPoint = i;
                        break;
                    }
                }
                float lerpAmount = (distFromStart - (currentPoint == 0 ? 0 : distances[currentPoint - 1])) / (distances[currentPoint] - (currentPoint == 0 ? 0 : distances[currentPoint - 1]));
                Vector3 targetPosition = Vector3.Lerp(points[currentPoint].position, points[currentPoint + 1].position, lerpAmount);
                Quaternion targetRotation = Quaternion.Lerp(points[currentPoint].rotation, points[currentPoint + 1].rotation, lerpAmount);
                cameraMan.transform.position = Vector3.Lerp(cameraMan.transform.position, targetPosition, Time.deltaTime * cameraMan.camFollowSpeed);
                cameraMan.transform.rotation = Quaternion.Lerp(cameraMan.transform.rotation, targetRotation, cameraMan.rotationSmoothTime);
                cameraMan.AddToPitch(targetRotation.eulerAngles.x - cameraMan.GetPitch(), true);
                cameraMan.AddToYaw(targetRotation.eulerAngles.y - cameraMan.GetYaw(), true);
                cameraMan.SetCurrentRotation(targetRotation.eulerAngles);
            }
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.name == "Squirrel")
            {
                Debug.Log("Trigger Enter");
                if (cameraMan != null)
                {
                    cameraMan.inCameraZone = true;
                    cameraMan.onRails = OnRails();
                }
                //camera angle is the amount the camera needs to move, tempAngle is the starting point
                tempAngle = Vector3.SignedAngle(cameraMan.transform.forward, transform.forward, Vector3.up);
                cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
                moveCamera = true;
                triggered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.name == "Squirrel")
            {
                Debug.Log("Trigger Exit");
                if (cameraMan != null)
                {
                    cameraMan.inCameraZone = false;
                    cameraMan.onRails = false;
                }
                moveCamera = false;
                triggered = false;
            }
        }
    }
}
