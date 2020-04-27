using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PreServer
{
    public class SmartDashDebugger : MonoBehaviour
    {
        public SmartLagDash smartDash;
        public Text dashDistance;
        public Text dashTime;
        public Text dashSHDistance;
        public Text endMomentum;
        public Text slowMoTime;
        public Text slowMoSpeedUp;
        public Text rotationAngle;
        public Text rotationSpeed;
        public Text verticalVelocity;
        public Text horizontalVelocity;
        public Text ghostMode;
        public Text flickerTime;
        public Text ghostSpeed;
        public GameObject p1;
        public GameObject p2;
        public GameObject p3;
        public GameObject p4;
        public float factor = 0.0001f;
        Vector3 intersection = Vector3.zero;
        // Start is called before the first frame update
        void Start()
        {
            dashDistance.text = smartDash.defaultDist.ToString("N2");
            dashSHDistance.text = smartDash.runDist.ToString("N2");
            dashTime.text = smartDash.time.ToString("N2");
            endMomentum.text = smartDash.endMomentum.ToString("N2");
            slowMoTime.text = smartDash.slowMoDuration.ToString("N2");
            slowMoSpeedUp.text = smartDash.slowMoSpeedUpDelay.ToString("N2");
            rotationSpeed.text = smartDash.rotationSpeed.ToString("N2");
            rotationAngle.text = smartDash.rotationCutoff.ToString("N2");
            horizontalVelocity.text = smartDash.velocityMult.x.ToString("N2");
            verticalVelocity.text = smartDash.velocityMult.y.ToString("N2");
            ghostMode.text = smartDash.gdm.ToString();
            flickerTime.text = smartDash.flickerTime.ToString("N3");
        }

        public void IncreaseDashDistance()
        {
            smartDash.defaultDist += 0.5f;
            if (smartDash.defaultDist >= smartDash.runDist)
                IncreaseDashSHDistance();
            dashDistance.text = smartDash.defaultDist.ToString("N2");
        }

        public void DecreaseDashDistance()
        {
            if (smartDash.defaultDist <= 0.5f)
                return;
            smartDash.defaultDist -= 0.5f;
            dashDistance.text = smartDash.defaultDist.ToString("N2");
        }

        public void IncreaseDashTime()
        {
            smartDash.time += 0.05f;
            dashTime.text = smartDash.time.ToString("N2");
        }

        public void DecreaseDashTime()
        {
            if (smartDash.time <= 0.05f)
                return;
            smartDash.time -= 0.05f;
            dashTime.text = smartDash.time.ToString("N2");
        }

        public void IncreaseDashSHDistance()
        {
            smartDash.runDist += 0.5f;
            dashSHDistance.text = smartDash.runDist.ToString("N2");
        }

        public void DecreaseDashSHDistance()
        {
            if (smartDash.runDist <= 1.0f)
                return;
            smartDash.runDist -= 0.5f;
            if (smartDash.defaultDist >= smartDash.runDist)
                DecreaseDashDistance();
            dashSHDistance.text = smartDash.runDist.ToString("N2");
        }

        public void IncreaseRotationSpeed()
        {
            smartDash.rotationSpeed += 1f;
            rotationSpeed.text = smartDash.rotationSpeed.ToString("N2");
        }

        public void DecreaseRotationSpeed()
        {
            if (smartDash.rotationSpeed <= 0)
                return;
            smartDash.rotationSpeed -= 1f;
            rotationSpeed.text = smartDash.rotationSpeed.ToString("N2");
        }

        public void IncreaseRotationAngle()
        {
            if (smartDash.rotationCutoff >= 180f)
                return;
            smartDash.rotationCutoff += 5f;
            rotationAngle.text = smartDash.rotationCutoff.ToString("N2");
        }

        public void DecreaseRotationAngle()
        {
            if (smartDash.rotationCutoff <= 5f)
                return;
            smartDash.rotationCutoff -= 5f;
            rotationAngle.text = smartDash.rotationCutoff.ToString("N2");
        }

        public void IncreaseEndMomentum()
        {
            smartDash.endMomentum += 1.0f;
            endMomentum.text = smartDash.endMomentum.ToString("N2");
        }

        public void DecreaseEndMomentum()
        {
            if (smartDash.endMomentum <= 0f)
                return;
            smartDash.endMomentum -= 1.0f;
            endMomentum.text = smartDash.endMomentum.ToString("N2");
        }

        public void IncreaseSlowMoSpeedUp()
        {
            smartDash.slowMoSpeedUpDelay += 0.25f;
            if (smartDash.slowMoSpeedUpDelay > smartDash.slowMoDuration)
                IncreaseSlowMoTime();
            slowMoSpeedUp.text = smartDash.slowMoSpeedUpDelay.ToString("N2");
        }

        public void DecreaseSlowMoSpeedUp()
        {
            if (smartDash.slowMoSpeedUpDelay <= 0f)
                return;
            smartDash.slowMoSpeedUpDelay -= 0.25f;
            slowMoSpeedUp.text = smartDash.slowMoSpeedUpDelay.ToString("N2");
        }

        public void IncreaseSlowMoTime()
        {
            smartDash.slowMoDuration += 0.25f;
            slowMoTime.text = smartDash.slowMoDuration.ToString("N2");
        }

        public void DecreaseSlowMoTime()
        {
            if (smartDash.slowMoDuration <= 0.25f)
                return;
            smartDash.slowMoDuration -= 0.25f;
            if (smartDash.slowMoSpeedUpDelay > smartDash.slowMoDuration)
                DecreaseSlowMoSpeedUp();
            slowMoTime.text = smartDash.slowMoDuration.ToString("N2");
        }

        public void IncreaseVerticalVelocitySuppresion()
        {
            if (smartDash.velocityMult.y >= 1f)
                return;
            smartDash.velocityMult.y += 0.05f;
            verticalVelocity.text = smartDash.velocityMult.y.ToString("N2");
        }

        public void DecreaseVerticalVelocitySuppresion()
        {
            if (smartDash.velocityMult.y <= 0f)
                return;
            smartDash.velocityMult.y -= 0.05f;
            verticalVelocity.text = smartDash.velocityMult.y.ToString("N2");
        }

        public void IncreaseHorizontalVelocitySuppresion()
        {
            if (smartDash.velocityMult.x >= 1f)
                return;
            smartDash.velocityMult.x += 0.05f;
            smartDash.velocityMult.z = smartDash.velocityMult.x;
            horizontalVelocity.text = smartDash.velocityMult.x.ToString("N2");
        }

        public void DecreaseHorizontalVelocitySuppresion()
        {
            if (smartDash.velocityMult.x <= 0f)
                return;
            smartDash.velocityMult.x -= 0.05f;
            smartDash.velocityMult.z = smartDash.velocityMult.x;
            horizontalVelocity.text = smartDash.velocityMult.x.ToString("N2");
        }

        public void IncreaseFlickerTime()
        {
            if (smartDash.flickerTime >= .5f)
                return;
            smartDash.flickerTime += 0.025f;
            flickerTime.text = smartDash.flickerTime.ToString("N3");
        }

        public void DecreaseFlickerTime()
        {
            if (smartDash.flickerTime <= 0f)
                return;
            smartDash.flickerTime -= 0.025f;
            flickerTime.text = smartDash.flickerTime.ToString("N3");
        }

        public void IncreaseGhostSpeed()
        {
            if (smartDash.ghostSpeed >= 1f)
                return;
            smartDash.ghostSpeed += 0.05f;
            ghostSpeed.text = smartDash.ghostSpeed.ToString("N2");
        }

        public void DecreaseGhostSpeed()
        {
            if (smartDash.ghostSpeed <= 0f)
                return;
            smartDash.ghostSpeed -= 0.05f;
            ghostSpeed.text = smartDash.ghostSpeed.ToString("N2");
        }

        public void IncreaseGhostMode()
        {
            smartDash.gdm++;
            smartDash.gdm = (SmartLagDash.GhostDisplayMode)((int)smartDash.gdm % (int)SmartLagDash.GhostDisplayMode.Max);
            ghostMode.text = smartDash.gdm.ToString();
        }

        public void DecreaseGhostMode()
        {
            if (smartDash.gdm == 0)
                smartDash.gdm = SmartLagDash.GhostDisplayMode.Max;
            smartDash.gdm--;
            smartDash.gdm = (SmartLagDash.GhostDisplayMode)((int)smartDash.gdm % (int)SmartLagDash.GhostDisplayMode.Max);
            ghostMode.text = smartDash.gdm.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            if (p1 != null && p2 != null && p3 != null && p4 != null)
            {
                if (LineLineIntersection(out intersection, p1.transform.position, p2.transform.position - p1.transform.position, p3.transform.position, p4.transform.position - p3.transform.position))
                {
                    Debug.DrawLine(p1.transform.position, p2.transform.position, Color.green);
                    Debug.DrawLine(p3.transform.position, p4.transform.position, Color.green);
                    Debug.DrawRay(intersection, Vector3.up);
                }
                else
                {
                    Debug.DrawLine(p1.transform.position, p2.transform.position, Color.blue);
                    Debug.DrawLine(p3.transform.position, p4.transform.position, Color.blue);
                }
            }
        }

        public bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineDir1, Vector3 linePoint2, Vector3 lineDir2)
        {
            Vector3 lineDir3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineDir1, lineDir2);
            Vector3 crossVec3and2 = Vector3.Cross(lineDir3, lineDir2);

            float planarFactor = Vector3.Dot(lineDir3, crossVec1and2);

            //is coplanar, and not parrallel
            if (Mathf.Abs(planarFactor) < factor && crossVec1and2.sqrMagnitude > factor)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineDir1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }
    }
}
