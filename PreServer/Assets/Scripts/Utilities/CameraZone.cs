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
        //public Transform[] points;
        float[] distances;
        public Transform start;
        public Transform end;
        float totalDist = 0;
        float distFromStart = 0;
        bool triggered = false;
        int currentPoint = 0;
        //public CameraZoneSection[] sections;
        //List<Collider> colliders;
        int colCount;
        public Section[] sections;
        public bool debug;
        // Start is called before the first frame update
        void Start()
        {
            //for(int i = 0; i < sections.Length; ++i)
            //{
            //    sections[i].enter += OnSectionTriggerEnter;
            //    sections[i].exit += OnSectionTriggerExit;
            //}
            //colliders = new List<Collider>();
            cameraMan = Camera.main.transform.parent.GetComponent<CameraManager>();
            if (OnRails())
            {
                totalDist = Vector3.Distance(start.position, end.position);
                distances = new float[sections.Length];
                float pointsTotalDist = 0;
                for(int i = 0; i < distances.Length; ++i)
                {
                    distances[i] = Vector3.Distance(sections[i].GetStartPoint().position, sections[i].GetEndPoint().position);
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
            return sections != null && sections.Length > 0;
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
                Debug.DrawRay(playerPosition, Vector3.right * 4f, Color.black);
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
                Vector3 targetPosition = sections[currentPoint].GetPoint(lerpAmount); //Vector3.Lerp(points[currentPoint].position, points[currentPoint + 1].position, lerpAmount);
                Quaternion targetRotation = Quaternion.Lerp(sections[currentPoint].GetStartPoint().rotation, sections[currentPoint].GetEndPoint().rotation, lerpAmount);//Quaternion.Lerp(points[currentPoint].rotation, points[currentPoint + 1].rotation, lerpAmount);
                cameraMan.transform.position = Vector3.Lerp(cameraMan.transform.position, targetPosition, 1f);
                cameraMan.transform.rotation = Quaternion.Lerp(cameraMan.transform.rotation, targetRotation, cameraMan.rotationSmoothTime);
                cameraMan.AddToPitch(targetRotation.eulerAngles.x - cameraMan.GetPitch(), true);
                cameraMan.AddToYaw(targetRotation.eulerAngles.y - cameraMan.GetYaw(), true);
                cameraMan.SetCurrentRotation(targetRotation.eulerAngles);
                Debug.DrawRay(targetPosition, Vector3.up * 5f, Color.yellow);
            }
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }

        void OnSectionTriggerEnter(Collider col)
        {
            //if (colliders.Contains(col))
            //    return;
            //colliders.Add(col);
            //if (!triggered)
            //{
            //    Debug.Log("Trigger Enter");
            //    if (cameraMan != null)
            //    {
            //        cameraMan.inCameraZone = true;
            //        cameraMan.onRails = OnRails();
            //    }
            //    //camera angle is the amount the camera needs to move, tempAngle is the starting point
            //    tempAngle = Vector3.SignedAngle(cameraMan.transform.forward, transform.forward, Vector3.up);
            //    cameraAngle = (tempAngle > 0 ? -180 + tempAngle : 180 + tempAngle);
            //    moveCamera = true;
            //    triggered = true;
            //}
        }

        void OnSectionTriggerExit(Collider col)
        {
            //if (!colliders.Contains(col))
            //    return;
            //colliders.Remove(col);
            //if (colliders.Count == 0)
            //{
            //    Debug.Log("Trigger Exit");
            //    if (cameraMan != null)
            //    {
            //        cameraMan.inCameraZone = false;
            //        cameraMan.onRails = false;
            //    }
            //    moveCamera = false;
            //    triggered = false;
            //}
        }
        Vector3 camPos;
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                colCount++;
                if (!triggered)
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
                    camPos = Camera.main.transform.localPosition;
                    Camera.main.transform.localPosition = Vector3.zero;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                colCount--;
                if (colCount == 0)
                {
                    Debug.Log("Trigger Exit");
                    if (cameraMan != null)
                    {
                        cameraMan.inCameraZone = false;
                        cameraMan.onRails = false;
                    }
                    moveCamera = false;
                    triggered = false;
                    Camera.main.transform.localPosition = camPos;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (sections != null && sections.Length > 0 && debug)
            {
                float t = 0.01f;
                Vector3 prevPos;
                Vector3 currPos;
                for(int i = 0; i < sections.Length; ++i)
                {
                    prevPos = sections[i].points[0].position;
                    for (int j = 1; j <= 100; ++j)
                    {
                        currPos = sections[i].GetPoint(t * j);
                        Gizmos.DrawLine(prevPos, currPos);
                        prevPos = currPos;
                    }
                }
            }
            //if (OnRails())
            //{
            //    for (int i = 0; i < points.Length - 1; ++i)
            //    {
            //        Gizmos.DrawLine(points[i].position, points[i + 1].position);
            //    }
            //}
        }

        [System.Serializable]
        public class Section
        {
            public Transform[] points;
            public Vector3 GetPoint(float t)
            {
                switch (points.Length)
                {
                    case 2:
                        return CalculateLinearPosition(t, points[0].position, points[1].position);
                    case 3:
                        return CalculateQuadraticPosition(t, points[0].position, points[1].position, points[2].position);
                    case 4:
                        return CalculateCubicPosition(t, points[0].position, points[1].position, points[2].position, points[3].position);
                    default:
                        Debug.Log("Invalid length for the point array, please fix before continuing");
                        return Vector3.zero;
                }
            }

            public Transform GetStartPoint()
            {
                return points[0];
            }

            public Transform GetEndPoint()
            {
                return points[points.Length - 1];
            }

            public Vector3 CalculateLinearPosition(float t, Vector3 p0, Vector3 p1)
            {
                return p0 + t * (p1 - p0);
            }

            public Vector3 CalculateQuadraticPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2)
            {
                return Mathf.Pow((1 - t), 2) * p0 + (2 * (1 - t) * t * p1) + Mathf.Pow(t, 2) * p2;
            }

            public Vector3 CalculateCubicPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
            {
                return Mathf.Pow(1 - t, 3) * p0 + (3 * Mathf.Pow(1 - t, 2) * t * p1) + (3 * (1 - t) * Mathf.Pow(t, 2) * p2) + Mathf.Pow(t, 3) * p3;
            }
            //public 
        }
    }
}
