using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class PlaytestManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        
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
    }
}