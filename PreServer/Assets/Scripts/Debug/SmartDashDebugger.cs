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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
