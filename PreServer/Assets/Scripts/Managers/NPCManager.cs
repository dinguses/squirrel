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

        public List<Vector3> ziggyPoints;
        public float speedHold;
        public int runLikelihood;

        public bool isRandom = false;

        bool rotateTowards;
        bool currentlyUsing = false;

        GameObject sittingPoint;

        public void SetUp(NPCAction action, string un)
        {
            npcTransform = this.transform;
            rigid = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            hashes = new AnimHashes();
            navMeshAgent = GetComponent<NavMeshAgent>();

            GameObject spawner = GameObject.Find("NPC_Spawner");
            spawnerManager = spawner.GetComponent<SpawnerManager>();

            var tungus = npcTransform.position;
            //npcTransform.position = new Vector3(tungus.x, tungus.y - .05f, tungus.z);

            userName.text = un;

            steps = action.steps;

            rotateTowards = false;

            runLikelihood = Random.Range(0, 101);

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

        List<Vector3> GenZiggyPoints(Vector3 startPos, Vector3 endPos)
        {
            List<Vector3> zigs = new List<Vector3>();

            var lengthTest = Vector3.Distance(startPos, endPos);
            //var lengthTest = 2;

            if (lengthTest > 25)
            {
                int thingus = (int)(lengthTest / 25) + 1;

                var numZigs = Random.Range(2, thingus);

                //Vector3 towardsEnd = endPos - startPos;

                var prevPos = startPos;

                for (int i = 0; i < numZigs - 1; i++)
                {
                    Vector3 towardsEnd = endPos - prevPos;
                    var towardsEndSeg = towardsEnd / numZigs;
                    Vector3 zagSeg = prevPos + towardsEndSeg;
                    zagSeg = new Vector3(zagSeg.x + Random.Range(-2.5f, 2.5f), zagSeg.y, zagSeg.z + Random.Range(-2.5f, 2.5f));

                    NavMeshPath pathTest = new NavMeshPath();
                    navMeshAgent.CalculatePath(zagSeg, pathTest);

                    if (pathTest.status == NavMeshPathStatus.PathComplete)
                    {
                        zigs.Add(zagSeg);
                        prevPos = zagSeg;
                    }

                }

                // TODO: Random NPC endpoint shifting needs to check if this is pathable, and then redo if it's not

                if (isRandom)
                {
                    bool shiftedPointValid = false;
                    var endPosHold = endPos;

                    while (!shiftedPointValid)
                    {
                        endPos = endPosHold;
                        endPos = new Vector3(endPos.x + Random.Range(-5f, 5f), endPos.y, endPos.z + Random.Range(-5f, 5f));
                        NavMeshPath navMeshPath = new NavMeshPath();
                        navMeshAgent.CalculatePath(endPos, navMeshPath);

                        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                            shiftedPointValid = true;
                    }
                }

                zigs.Add(endPos);

                /*var quarter = towardsEnd / 4;
                var half = towardsEnd / 2;

                Vector3 quarterDist = startPos + quarter;
                quarterDist = new Vector3(quarterDist.x + Random.Range(-2.5f, 2.5f), quarterDist.y, quarterDist.z + Random.Range(-2.5f, 2.5f));

                Vector3 halfDist = startPos + half;
                halfDist = new Vector3(halfDist.x + Random.Range(-7.5f, 7.5f), halfDist.y, halfDist.z + Random.Range(-7.5f, 7.5f));

                zigs.Add(quarterDist);
                zigs.Add(halfDist);
                zigs.Add(endPos);

                var startPosX = startPos.x;
                var startPosZ = startPos.z;
                var endPosX = endPos.x;
                var endPosZ = endPos.z;*/
            }

            else
            {
                zigs.Add(endPos);
            }

            return zigs;
        }

        void NextStep()
        {
            currStep = steps[currStepNum];

            navMeshAgent.isStopped = true;

            if (currStep is MoveStep)
            {
                MoveStep ms = (MoveStep)currStep;

                Vector3 destinationPoint = spawnerManager.npcPoints[ms.pointName];

                // TODO
                ziggyPoints = GenZiggyPoints(npcTransform.position, destinationPoint);
                //ziggyPoints[0] = destinationPoint;

                navMeshAgent.SetDestination(ziggyPoints[0]);
                navMeshAgent.isStopped = false;

                destination = ziggyPoints[0];


                if (Random.Range(0, 101) <= runLikelihood)
                {
                    anim.CrossFade(hashes.npc_run, 0.02f);
                    navMeshAgent.speed = 9.0f;
                }
                else
                {
                    anim.CrossFade(hashes.npc_walk, 0.02f);
                    navMeshAgent.speed = 3.5f;
                }

                //anim.CrossFade(hashes.npc_run, 0.02f);
                //rotateTowards = true;

                //StartCoroutine(MoveTurn(1.5f));
            }
            else if (currStep is WaitStep)
            {
                WaitStep ws = (WaitStep)currStep;

                if (!currentlyUsing)
                {
                    anim.CrossFade(hashes.npc_idle, 0.02f);
                }

                StartCoroutine(Wait(ws.seconds));
            }
            else if (currStep is MsgStep)
            {
                MsgStep ms = (MsgStep)currStep;

                if (!currentlyUsing)
                {
                    anim.CrossFade(hashes.npc_idle, 0.02f);
                }

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
            else if (currStep is UseStep)
            {
                UseStep us = (UseStep)currStep;

                // If NPC is stopping whatever they're doing
                if (us.objToUse == "stop")
                {
                    currentlyUsing = false;

                    // TODO: move NPC back off of the object
                    MoveForUse(us.objToUse);
                }

                else
                {
                    anim.CrossFade(hashes.npc_sit_idle, 0.02f);

                    currentlyUsing = true;

                    MoveForUse(us.objToUse);
                }

                return;
            }

            loopStep = true;
        }

        void MoveForUse(string objToUse)
        {
            if (objToUse != "stop")
            {
                var sitLocs = GameObject.FindGameObjectsWithTag("sitPoint");

                foreach (var sitLoc in sitLocs)
                {
                    if (sitLoc.name == objToUse)
                    {
                        npcTransform.GetComponent<CapsuleCollider>().enabled = false;
                        npcTransform.GetComponent<NavMeshAgent>().enabled = false;

                        npcTransform.position = sitLoc.transform.position;
                        npcTransform.rotation = sitLoc.transform.rotation;

                        var sitLocTextTrans = sitLoc.transform.GetChild(0);

                        userName.transform.position = new Vector3(sitLocTextTrans.position.x, userName.transform.position.y, sitLocTextTrans.position.z);
                        msg.transform.position = new Vector3(sitLocTextTrans.position.x, msg.transform.position.y, sitLocTextTrans.position.z);

                        sittingPoint = sitLoc;
                    }
                }
            }
            else
            {
                var returnPoint = sittingPoint.transform.GetChild(1);

                npcTransform.position = returnPoint.position;
                npcTransform.rotation = returnPoint.rotation;

                userName.transform.position = new Vector3(returnPoint.position.x, userName.transform.position.y, returnPoint.position.z);
                msg.transform.position = new Vector3(returnPoint.position.x, msg.transform.position.y, returnPoint.position.z);

                npcTransform.GetComponent<CapsuleCollider>().enabled = true;
                npcTransform.GetComponent<NavMeshAgent>().enabled = true;
            }

            StepOver();
        }

        void LoopStep()
        {
            if (currStep is MoveStep)
            {
                MoveStep ms = (MoveStep)currStep;

                //var distToDest = Vector3.Distance(npcTransform.position, ziggyPoints[0]);
                //float distTest = navMeshAgent.remainingDistance;

                //Debug.Log(navMeshAgent.remainingDistance);

                // TODO
                // if (distToDest <= 1.0f)
                //if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                //if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                if (navMeshAgent.remainingDistance <= 1.75f)
                {
                    ziggyPoints.RemoveAt(0);

                    if (ziggyPoints.Count > 0)
                    {
                        //rotateTowards = true;
                        //navMeshAgent.isStopped = true;
                        //anim.CrossFade(hashes.npc_idle, 0.02f);
                        navMeshAgent.SetDestination(ziggyPoints[0]);
                    }
                    else
                    {
                        navMeshAgent.isStopped = true;
                        navMeshAgent.speed = 0;
                        navMeshAgent.velocity = Vector3.zero;
                        //navMeshAgent.GetComponent<Rigidbody>().drag = 100000;

                        StepOver();
                    }
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
                            var distToPals = Vector3.Distance(bbb.transform.position, npcTransform.position);

                            if (distToPals <= 5.0f)
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

        // For "wait" commands
        IEnumerator MoveTurn(float numSecs)
        {
            yield return new WaitForSeconds(numSecs);
            rotateTowards = false;
        }
    }
}