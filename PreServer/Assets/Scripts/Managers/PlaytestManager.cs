using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class PlaytestManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public GameObject cage;
        public bool cageIsUp;
        public Vector3 cagePositionUp = new Vector3(-76, 7, -122);
        public Vector3 cagePositionDown = new Vector3(-76, -7, -122);

        public Vector3 moveUp = new Vector3(0, .05f, 0);
        public Vector3 moveDown = new Vector3(0, -.05f, 0);

        public void Start()
        {
            cageIsUp = false;
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
        }
    }
}