using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private int forwardDivider = 8;

        public bool cageIsUp;
        public bool crushersPrimed;
        public Vector3 cagePositionUp = new Vector3(-76, 7, -122);
        public Vector3 cagePositionDown = new Vector3(-76, -7, -122);

        private Vector3 leftCrusherPrimed = new Vector3(-403f, 14.2f, 77f);
        private Vector3 leftCrusherSprung = new Vector3(-389.4f, 14.2f, 107f);
        private Vector3 rightCrusherPrimed = new Vector3(-316.4f, 14.2f, 111.1f);
        private Vector3 rightCrusherSprung = new Vector3(-330f, 14.2f, 81f);

        public Vector3 moveUp = new Vector3(0, .05f, 0);
        public Vector3 moveDown = new Vector3(0, -.05f, 0);

        public void Start()
        {
            cageIsUp = false;
            crushersPrimed = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            camera.ignoreMouse = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!playerManager.speedNutGot)
            {
                if (playerManager.inSpeedZone)
                {
                    // If cage needs to be lifted
                    if (!cageIsUp)
                    {
                        cage.transform.position = cagePositionUp;
                    }

                    // If crushers need to be primed
                    if (!crushersPrimed)
                    {
                        leftCrusher.transform.position = leftCrusherPrimed;
                        rightCrusher.transform.position = rightCrusherPrimed;
                    }
                }
                else
                {
                    // lower cage until fully lowered
                    if (Vector3.Distance(cage.transform.position, cagePositionDown) >= 0.05f)
                    {
                        cage.transform.position = cage.transform.position + moveDown;
                    }

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
            }
            else
            {
                if (Vector3.Distance(cage.transform.position, cagePositionUp) >= 0.05f)
                {
                    cage.transform.position = cage.transform.position + moveUp;
                }
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            camera.ignoreMouse = true;

            ControlsPicked();
        }

        void ControlsPicked()
        {
            controlAsker.SetActive(false);
        }
    }
}