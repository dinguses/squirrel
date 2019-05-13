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

        public void SetUp(NPCAction action, string un)
        {
            npcTransform = this.transform;
            rigid = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            hashes = new AnimHashes();
            navMeshAgent = GetComponent<NavMeshAgent>();



            userName.text = un;

            steps = action.steps;

            NextStep();
        }

        void Update()
        {
            if (steps != null)
            {
                Vector3 userNameEuler = userName.transform.rotation.eulerAngles;
                userNameEuler.y = cam.value.transform.rotation.eulerAngles.y;
                userName.transform.rotation = Quaternion.Euler(userNameEuler);
                msg.transform.rotation = Quaternion.Euler(userNameEuler);

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

                msg.text = ms.msg;
                StartCoroutine(Wait(5));
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

                if (FastApprox(nmaPos.x, destPos.x, 1.0f) && FastApprox(nmaPos.y, destPos.y, 1.0f) && FastApprox(nmaPos.z, destPos.z, 1.0f))
                {
                    navMeshAgent.isStopped = true;

                    StepOver();
                }
            }

        }

        void StepOver()
        {
            // If last step, destroy NPC (for now)
            if (currStepNum == (steps.Count - 1))
            {
                Destroy(npcTransform.root.gameObject);
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
        IEnumerator Wait(int numSecs)
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