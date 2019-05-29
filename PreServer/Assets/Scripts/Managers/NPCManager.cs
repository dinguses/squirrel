using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using SO;
using UnityEngine.AI;

namespace PreServer
{
    public class NPCManager : MonoBehaviour
    {
        public Transform npcTransform;
        public Rigidbody rigid;
        public Animator anim;

        public NPCAction action;

        public List<NPCStep> steps;
        public NPCStep currStep;
        public int currStepNum = 0;

        public Vector3 destination;
        public int waitTime = 0;
        public string message = "";

        public string userNameToUse;

        public bool stepInProg = false;

        public TextMeshPro userName;
        public TextMeshPro msg;

        public TransformVariable cam;

        public bool setup = false;
        public NavMeshAgent navMeshAgent;

        public AnimHashes hashes;
        public bool loopStep = false;
        public bool doneWaiting = false;

        public bool waitingForPals = false;

        public SpawnerManager spawnerManager;

        public void SetUp(NPCAction action, string un)
        {
            npcTransform = this.transform;
            rigid = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            hashes = new AnimHashes();
            navMeshAgent = GetComponent<NavMeshAgent>();

            GameObject spawner = GameObject.Find("NPC_Spawner");
            spawnerManager = spawner.GetComponent<SpawnerManager>();

            userName.text = un;

            steps = action.steps;

            NextStep();
        }

        void Update()
        {
            // If there's steps going on
            if (steps != null)
            {
                // Rotate Username and Message to face player camera
                Vector3 userNameEuler = userName.transform.rotation.eulerAngles;
                userNameEuler.y = cam.value.transform.rotation.eulerAngles.y;
                userName.transform.rotation = Quaternion.Euler(userNameEuler);
                msg.transform.rotation = Quaternion.Euler(userNameEuler);

                // If the current step needs to be looped
                if (loopStep)
                {
                    LoopStep();
                }
            }
        }

        void NextStep()
        {
            currStep = steps[currStepNum];

            if (currStep is MoveStep)
            {
                MoveStep ms = (MoveStep)currStep;
                navMeshAgent.SetDestination(ms.destination);

                navMeshAgent.isStopped = false;

                anim.CrossFade(hashes.npc_run, 0.02f);
            }
            else if (currStep is WaitStep)
            {
                WaitStep ws = (WaitStep)currStep;

                anim.CrossFade(hashes.npc_idle, 0.02f);

                StartCoroutine(Wait(ws.seconds));
            }
            else if (currStep is MsgStep)
            {
                MsgStep ms = (MsgStep)currStep;

                anim.CrossFade(hashes.npc_idle, 0.02f);

                if (ms.waitForPals.Count > 0)
                {
                    waitingForPals = true;
                }
                else
                {
                    msg.text = ms.msg;
                    StartCoroutine(Wait(5));
                }
            }

            loopStep = true;
        }

        void LoopStep()
        {
            if (currStep is MoveStep)
            {
                MoveStep ms = (MoveStep)currStep;

                Vector3 nmaPos = navMeshAgent.transform.position;
                Vector3 destPos = ms.destination;

                // TODO: this could probably just check the remaining distance
                if (FastApprox(nmaPos.x, destPos.x, 1.0f) && FastApprox(nmaPos.y, destPos.y, 1.0f) && FastApprox(nmaPos.z, destPos.z, 1.0f))
                {
                    navMeshAgent.isStopped = true;

                    StepOver();
                }
            }

            else if (currStep is MsgStep)
            {
                if (waitingForPals)
                {
                    MsgStep ms = (MsgStep)currStep;

                    bool allPalsHere = false;

                    foreach (int pal in ms.waitForPals)
                    {
                        GameObject bbb = GameObject.Find("NPC_" + pal);

                        if (bbb != null)
                        {
                            var b = bbb.transform.position;

                            var a = npcTransform.position;

                            if (FastApprox(a.x, b.x, 5.0f) && FastApprox(a.y, b.y, 5.0f) && FastApprox(a.z, b.z, 5.0f))
                            {
                                allPalsHere = true;
                            }
                            else
                            {
                                allPalsHere = false;
                            }
                        }
                        else
                        {
                            allPalsHere = false;
                        }
                    }

                    if (allPalsHere)
                    {
                        waitingForPals = false;
                        msg.text = ms.msg;
                        StartCoroutine(Wait(5));
                    }
                }
            }

        }

        void StepOver()
        {
            // If last step, destroy NPC (for now)
            if (currStepNum == (steps.Count - 1))
            {
                Destroy(npcTransform.root.gameObject);

                spawnerManager.numNPCs--;
            }

            // Otherwise, increment to next step, set loop to false, and execute NextStep()
            else
            {
                currStepNum++;
                loopStep = false;
                NextStep();
            }
        }

        // For "wait" commands
        IEnumerator Wait(float numSecs)
        {
            yield return new WaitForSeconds(numSecs);
            msg.text = "";
            StepOver();
        }

        public static bool FastApprox(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
        }
    }
}