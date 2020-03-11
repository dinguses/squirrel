using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PreServer
{
    public class PlaytestManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public OldCamera camera;

        public GameObject controlAsker;

        public GameObject introSignC;
        public GameObject introSignK;
        public GameObject sign1C;
        public GameObject sign1K;
        public GameObject sign4C;
        public GameObject sign4K;
        public GameObject sign5C;
        public GameObject sign5K;

        public GameObject cage;
        public GameObject leftCrusher;
        public GameObject rightCrusher;

        public GameObject finalCrusher;

        private int forwardDivider = 9;

        public bool cageIsUp;
        public bool crushersPrimed;
        public Vector3 cagePositionUp = new Vector3(-76, 7, -122);
        public Vector3 cagePositionDown = new Vector3(-76, -7, -122);

        private Vector3 leftCrusherPrimed = new Vector3(-403f, 14.2f, 77f);
        private Vector3 leftCrusherSprung = new Vector3(-389.4f, 14.2f, 107f);
        private Vector3 rightCrusherPrimed = new Vector3(-316.4f, 14.2f, 111.1f);
        private Vector3 rightCrusherSprung = new Vector3(-330f, 14.2f, 81f);

        private Vector3 finalCrusherStart = new Vector3(-557.9052f, 53.8f, 363.9659f);
        private Vector3 finalCrusherDone = new Vector3(-396.2f, 53.8f, 363.9659f);

        public Vector3 moveUp = new Vector3(0, .05f, 0);
        public Vector3 moveDown = new Vector3(0, -.05f, 0);

        public SmartLagDash smartDash;
        public Text dashDistance;
        public Text dashTime;
        public Text dashSHDistance;
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
            smartDash.runDist -= 0.5f;
            if (smartDash.defaultDist >= smartDash.runDist)
                DecreaseDashDistance();
            dashSHDistance.text = smartDash.runDist.ToString("N2");
        }

        public void Start()
        {
            dashDistance.text = smartDash.defaultDist.ToString("N2");
            dashSHDistance.text = smartDash.runDist.ToString("N2");
            dashTime.text = smartDash.time.ToString("N2");
            cageIsUp = false;
            crushersPrimed = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            camera.ignoreMouse = false;
            ControllerSelected();
        }

        // Update is called once per frame
        void Update()
        {
            // Speed Nut
            if (!playerManager.speedNutGot)
            {
                if (playerManager.inSpeedZone)
                {
                    // If cage needs to be lifted
                    if (!cageIsUp)
                    {
                        cage.transform.position = cagePositionUp;
                    }
                }
                else
                {
                    // lower cage until fully lowered
                    if (Vector3.Distance(cage.transform.position, cagePositionDown) >= 0.05f)
                    {
                        cage.transform.position = cage.transform.position + moveDown;
                    }
                }
            }
            else
            {
                if (Vector3.Distance(cage.transform.position, cagePositionUp) >= 0.05f)
                {
                    cage.transform.position = cage.transform.position + moveUp;
                }
            }

            // Crushers
            if (playerManager.inSpeedZone)
            {
                // If crushers need to be primed
                if (!crushersPrimed)
                {
                    leftCrusher.transform.position = leftCrusherPrimed;
                    rightCrusher.transform.position = rightCrusherPrimed;
                }
            }
            else
            {
                // crush until fully crushed
                if (Vector3.Distance(leftCrusher.transform.position, leftCrusherSprung) >= 0.25f)
                {
                    leftCrusher.transform.position = leftCrusher.transform.position + (leftCrusher.transform.forward / forwardDivider);
                }

                // crush until fully crushed
                if (Vector3.Distance(rightCrusher.transform.position, rightCrusherSprung) >= 0.25f)
                {
                    rightCrusher.transform.position = rightCrusher.transform.position + (rightCrusher.transform.forward / forwardDivider);
                }
            }

            if (playerManager.inFinalZone && Vector3.Distance(finalCrusher.transform.position, finalCrusherDone) >= 7.6f)
            {
                Debug.Log(Vector3.Distance(finalCrusher.transform.position, finalCrusherDone));

                finalCrusher.transform.position = finalCrusher.transform.position + (finalCrusher.transform.forward / 12);
            }

            if (playerManager.restartFinalCrusher)
            {
                finalCrusher.transform.position = finalCrusherStart;
                playerManager.restartFinalCrusher = false;
                playerManager.inFinalZone = false;
            }
        }

        public void KeyboardSelected()
        {
            // Change out signs
            introSignC.SetActive(false);
            introSignK.SetActive(true);
            sign1C.SetActive(false);
            sign1K.SetActive(true);
            sign4C.SetActive(false);
            sign4K.SetActive(true);
            sign5C.SetActive(false);
            sign5K.SetActive(true);

            // Enable mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            camera.ignoreMouse = false;

            ControlsPicked();
        }

        public void ControllerSelected()
        {
            // Disable mouse
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            camera.ignoreMouse = true;

            ControlsPicked();
        }

        void ControlsPicked()
        {
            controlAsker.SetActive(false);
        }
    }
}