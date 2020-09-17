using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PreServer
{
    public class SpawnerManager : MonoBehaviour
    {
        public int numNPCs;
        public Transform spawner;
        public XMLParser xmlParser;

        public Dictionary<string, Vector3> npcPoints;
        public List<string> npcPointNames;

        public List<NPCAction> npcActions;
        public List<NPCUsername> randomUsernames;
        public List<NPCAction> palsToGen;

        public Texture[] skintones;
        public Texture[] eyeColors;
        public Texture[] pantColors;

        public Mesh[] femaleShirts;
        public Mesh[] maleShirts;
        public Mesh[] femalePants;
        public Mesh[] malePants;
        public Mesh[] eyebrowMeshes;
        public Mesh[] glassesMeshes;
        public Mesh[] beardMeshes;
        public Mesh[] makeups;
        public Mesh[] hairs;

        public Material[] hairColors;
        public Material[] shirtColors;
        public Material[] makeupColors;

        public Material[] combinedHairColors;
        public Material[] combinedShirtColors;

        public Vector3[] hairScales;

        public Vector3 glassesScale;

        public int maxNPCs = 9;

        public bool spawnIDfirst;

        public int IDToSpawnFirst;

        void Start()
        {
            spawner = gameObject.transform;
            numNPCs = 0;
            xmlParser = GetComponent<XMLParser>();

            npcPoints = LoadNpcPoints();

            npcActions = xmlParser.ParseActions();
            randomUsernames = xmlParser.ParseUsernames();

            palsToGen = new List<NPCAction>();

            skintones = Resources.LoadAll<Texture>("NPC/Textures/Skin");
            eyeColors = Resources.LoadAll<Texture>("NPC/Textures/Eyes");
            pantColors = Resources.LoadAll<Texture>("NPC/Textures/Pants");

            femaleShirts = Resources.LoadAll<Mesh>("NPC/Meshes/FemaleShirts");
            maleShirts = Resources.LoadAll<Mesh>("NPC/Meshes/MaleShirts");
            femalePants = Resources.LoadAll<Mesh>("NPC/Meshes/FemalePants");
            malePants = Resources.LoadAll<Mesh>("NPC/Meshes/MalePants");
            eyebrowMeshes = Resources.LoadAll<Mesh>("NPC/Meshes/Eyebrows");
            glassesMeshes = Resources.LoadAll<Mesh>("NPC/Meshes/Glasses");
            beardMeshes = Resources.LoadAll<Mesh>("NPC/Meshes/Beards");
            makeups = Resources.LoadAll<Mesh>("NPC/Meshes/Makeup");
            hairs = Resources.LoadAll<Mesh>("NPC/Meshes/Hair");

            hairColors = Resources.LoadAll<Material>("NPC/Materials/Hair");
            shirtColors = Resources.LoadAll<Material>("NPC/Materials/Shirts");
            makeupColors = Resources.LoadAll<Material>("NPC/Materials/Makeup");

            combinedHairColors = hairColors.Concat(Resources.LoadAll<Material>("NPC/Premium/Materials/Hair")).ToArray();
            combinedShirtColors = shirtColors.Concat(Resources.LoadAll<Material>("NPC/Premium/Materials/Shirts")).ToArray();

            // Needed because hair meshes do not have uniform scale and this info is not stored in the mesh file
            hairScales = new Vector3[] {
                new Vector3(0.9818031f, 1.014686f, 1.014686f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.996021f, 1.02938f, 1.02938f),
                new Vector3(1, 1, 1),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.3656405f, 2.746532f, 0.2447241f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(1, 1, 1),
                new Vector3(0.9715822f, 1.004123f, 1.004123f),
                new Vector3(1, 1, 1),
                new Vector3(0.9715822f, 1.004123f, 1.004123f)
            };

            glassesScale = new Vector3(0.9675938f, 1, 1);
        }

        public Dictionary<string, Vector3> LoadNpcPoints()
        {
            Dictionary<string, Vector3> npcPointsList = new Dictionary<string, Vector3>();

            GameObject travelPointsObject = GameObject.Find("NPC_Travel_Points");
            List<Transform> npcPointsInScene = new List<Transform>(travelPointsObject.GetComponentsInChildren<Transform>());

            foreach (Transform npcPointInScene in npcPointsInScene)
            {
                if (npcPointInScene.tag != "exclude")
                {
                    npcPointsList.Add(npcPointInScene.name, npcPointInScene.position);
                    npcPointNames.Add(npcPointInScene.name);
                }
            }

            return npcPointsList;
        }

        void Update()
        {
            if (Random.Range(0, 1000) >= 900)
            {
                CreateNPC();
            }
        }

        public void CreateNPC()
        {
            // TODO: system for marking actions as "probably seen"
            // TODO: system for only grabbing xxx amount of random messages that meet requirements

            if (numNPCs < maxNPCs)
            {
                NPCAction action;

                // If there's no multi-person conversations missing their participants
                if (palsToGen.Count == 0)
                {
                    // Make a completely random NPC
                    if (Random.Range(0, 2) == 0 && !spawnIDfirst)
                    {
                        RandomNPC();
                        return;
                    }

                    // Make an NPC 
                    else
                    {
                        // Generate an action from the list of actions for this spawner
                        if (spawnIDfirst)
                        {
                            action = npcActions[IDToSpawnFirst];
                            spawnIDfirst = false;
                        }
                        else
                        {
                            action = npcActions[Random.Range(0, npcActions.Count)];
                        }


                        // Pals?
                        if (action.pals.Count > 0)
                        {
                            // Add pals to the waiting list
                            foreach (int palID in action.pals)
                            {
                                var pal = npcActions.FirstOrDefault(w => w.id == palID);
                                palsToGen.Add(pal);
                            }
                        }
                    }
                }
                else
                {
                    action = npcActions.FirstOrDefault(w => w.id == palsToGen[0].id);
                    palsToGen.RemoveAt(0);
                }

                // Remove from actions list
                // TODO: does this need to be removed? or just marked
                npcActions.Remove(action);

                // Update the action to "generated"
                xmlParser.UpdateAction(action.id);

                // Gender requirement
                if (action.reqs.ContainsKey(1))
                {
                    if (action.reqs[1].Equals("m"))
                        SpawnNPC(action, false, true);
                    else
                        SpawnNPC(action, false, false);
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                        SpawnNPC(action, false, true);
                    else
                        SpawnNPC(action, false, false);
                }
            }
        }

        void RandomNPC()
        {
            NPCAction randomAction = new NPCAction();

            var numSteps = Random.Range(5, 25);
            List<NPCStep> steps = new List<NPCStep>();

            // Initial Wait Step
            WaitStep initialWait = new WaitStep();
            initialWait.seconds = Random.Range(1, 2);

            steps.Add(initialWait);

            for (int i = 0; i < numSteps; i++)
            {
                // Move Step
                if (Random.Range(0, 2) == 0 || i == 0)
                {
                    // TODO: System for somewhat prioritizing another point in the same cluster for the next move or two

                    MoveStep moveStep = new MoveStep();
                    moveStep.pointName = npcPointNames[Random.Range(0, npcPointNames.Count)];

                    steps.Add(moveStep);
                }

                // Wait Step
                else
                {
                    WaitStep waitStep = new WaitStep();
                    waitStep.seconds = Random.Range(1, 300);

                    steps.Add(waitStep);
                }
            }

            WaitStep finalWait = new WaitStep();
            finalWait.seconds = Random.Range(1, 301);
            steps.Add(finalWait);
            randomAction.steps = steps;
            randomAction.reqs = new Dictionary<int, string>();

            SpawnNPC(randomAction, true, finalWait.seconds % 2 == 0 ? true : false);
        }

        public void SpawnNPC(NPCAction action, bool isRandom, bool isMale)
        {
            //If there is a premium requirement in the NPCAction, set it, if not, 1 in X chance of being a premium NPC
            bool premium;

            if (action.reqs.ContainsKey(2))
            {
                if (action.reqs[2].Equals("t"))
                    premium = true;
                else
                    premium = false;
            }
            else
            {
                premium = (Random.Range(0, 4) == 0 ? true : false);
            }

            // Create an NPC from Prefab and locate the bones we'll need for attatching objects and such
            Debug.Log(spawner.position);
            GameObject newNPC = Instantiate(Resources.Load<GameObject>(isMale ? "NPC/MALE_NPC" : "NPC/FEMALE_NPC"), spawner.position, spawner.rotation);

            // If NPC is random, they have no ID
            if (isRandom)
                newNPC.name = "NPC_RAND_" + 0;
            else
                newNPC.name = "NPC_" + action.id.ToString();

            Transform root = newNPC.transform.Find("Squ_People3:root");
            Transform rootHead = root.transform.Find("Squ_People3:pelvis").transform.Find("Squ_People3:spine_01")
                .transform.Find("Squ_People3:spine_02").transform.Find("Squ_People3:spine_03").transform.Find("Squ_People3:neck_01")
                .transform.Find("Squ_People3:head");
            Transform body = newNPC.transform.Find(isMale ? "male_01_body" : "female_01_body");

            // Set skintone for body and head
            Texture skintone = skintones[Random.Range(0, skintones.Length)];
            body.GetComponent<SkinnedMeshRenderer>().material.mainTexture = skintone;
            newNPC.transform.Find("head_01").GetComponent<SkinnedMeshRenderer>().material.mainTexture = skintone;

            // Shirt generation
            int shirtValToUse = Random.Range(0, (isMale ? maleShirts.Length : femaleShirts.Length));
            Mesh shirtMesh = (isMale ? maleShirts[shirtValToUse] : femaleShirts[shirtValToUse]);
            Material shirtMat = (premium ? combinedShirtColors[Random.Range(0, combinedShirtColors.Length)] : shirtColors[Random.Range(0, shirtColors.Length)]);

            // Find the shirt bones and create the shirt object
            Transform shirt = newNPC.transform.Find(shirtMesh.name);
            GameObject newShirt = new GameObject();
            newShirt.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newShirt_smr = newShirt.AddComponent<SkinnedMeshRenderer>();
            newShirt_smr.sharedMesh = shirtMesh;
            newShirt_smr.rootBone = root;
            newShirt_smr.bones = shirt.GetComponent<SkinnedMeshRenderer>().bones;
            newShirt_smr.material = shirtMat;

            // If interior shirt, make the exterior shirt
            if ((isMale && shirtValToUse == 4) || (!isMale && shirtValToUse == 7))
            {
                // Second shirt generation
                Mesh shirtMesh2 = Resources.Load<Mesh>(isMale ? "NPC/Meshes/male_ext_shirt" : "NPC/Meshes/female_ext_shirt");
                Material shirtMat2 = (premium ? combinedShirtColors[Random.Range(0, combinedShirtColors.Length)] : shirtColors[Random.Range(0, shirtColors.Length)]);

                Transform shirt2 = newNPC.transform.Find(isMale ? "male_01_shirt5_twoLayer_Exterior" : "female_01_shirt5_twoLayer_Exterior");
                GameObject newShirt2 = new GameObject();
                newShirt2.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newShirt2_smr = newShirt2.AddComponent<SkinnedMeshRenderer>();
                newShirt2_smr.sharedMesh = shirtMesh2;
                newShirt2_smr.rootBone = root;
                newShirt2_smr.bones = shirt2.GetComponent<SkinnedMeshRenderer>().bones;
                newShirt2_smr.material = shirtMat2;
            }

            Texture pantText = pantColors[Random.Range(0, pantColors.Length)];

            // Pants Generation
            // No pants for female NPCs wearing dresses (0, 1, 2) or a swimsuit (8)
            if (isMale || (shirtValToUse != 0 && shirtValToUse != 1 && shirtValToUse != 2 && shirtValToUse != 8))
            {
                int pantValToUse = Random.Range(0, (isMale ? malePants.Length : femalePants.Length));
                Mesh pantsMesh = (isMale ? malePants[pantValToUse] : femalePants[pantValToUse]);

                Transform pants = newNPC.transform.Find(pantsMesh.name);
                GameObject newPants = new GameObject();
                newPants.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newPants_smr = newPants.AddComponent<SkinnedMeshRenderer>();
                newPants_smr.sharedMesh = pantsMesh;
                newPants_smr.rootBone = root;
                newPants_smr.bones = pants.GetComponent<SkinnedMeshRenderer>().bones;
                newPants_smr.material.mainTexture = pantText;
            }

            // Belt
            // No pants for female NPCs wearing dresses (0, 1, 2) or a swimsuit (8)
            if (isMale || (shirtValToUse != 0 && shirtValToUse != 1 && shirtValToUse != 2 && shirtValToUse != 8))
            {
                if (Random.Range(0, 2) == 0)
                {
                    Mesh beltMesh = Resources.Load<Mesh>(isMale ? "NPC/Meshes/male_belt" : "NPC/Meshes/female_belt");
                    Transform belt = newNPC.transform.Find(isMale ? "male_01_belt" : "female_01_belt");
                    GameObject newBelt = new GameObject();
                    newBelt.transform.SetParent(newNPC.transform);

                    SkinnedMeshRenderer newBelt_smr = newBelt.AddComponent<SkinnedMeshRenderer>();
                    newBelt_smr.sharedMesh = beltMesh;
                    newBelt_smr.rootBone = root;
                    newBelt_smr.bones = belt.GetComponent<SkinnedMeshRenderer>().bones;
                    newBelt_smr.material.mainTexture = pantText;
                }
            }

            // Eyes
            Transform eyes = rootHead.transform.Find("Squ_People3:eyesAndTeeth");
            Texture eyeTexture = eyeColors[Random.Range(0, eyeColors.Length)];
            eyes.GetComponent<MeshRenderer>().material.mainTexture = eyeTexture;

            // Hair
            int hairValToUse = Random.Range(-1, hairs.Length);

            /*if (action.reqs.ContainsKey(2))
            {
                hairValToUse = int.Parse(action.reqs[2]);
            }*/

            int hairColorVal = (premium ? Random.Range(0, combinedHairColors.Length) : Random.Range(0, hairColors.Length));

            // If hairValToUse is -1, npc is bald, don't bother giving them hair
            if (hairValToUse != -1)
            {
                Mesh hairMesh = hairs[hairValToUse];
                Material hairMat = (premium ? combinedHairColors[hairColorVal] : hairColors[hairColorVal]);

                GameObject hair = new GameObject();
                hair.transform.SetParent(rootHead.transform);

                MeshRenderer hair_mr = hair.AddComponent<MeshRenderer>();
                MeshFilter hair_mf = hair.AddComponent<MeshFilter>();
                hair_mf.mesh = hairMesh;
                hair_mr.material = hairMat;

                hair.transform.localScale = hairScales[hairValToUse];
                hair.transform.localPosition = Vector3.zero;
                hair.transform.localRotation = newNPC.transform.rotation;
                Vector3 hairRot = hair.transform.localRotation.eulerAngles;
                hairRot = new Vector3(hairRot.x + 1.321f, hairRot.y - 90, hairRot.z);
                hair.transform.localRotation = Quaternion.Euler(hairRot);
            }

            // Eyebrows
            Mesh eyebrowMesh = eyebrowMeshes[Random.Range(0, eyebrowMeshes.Length)];
            Material eyebrowMat = (premium ? combinedHairColors[hairColorVal] : hairColors[hairColorVal]);

            GameObject eyebrows = new GameObject();
            eyebrows.transform.SetParent(rootHead.transform);

            MeshRenderer eyebrows_mr = eyebrows.AddComponent<MeshRenderer>();
            MeshFilter eyebrows_mf = eyebrows.AddComponent<MeshFilter>();
            eyebrows_mf.mesh = eyebrowMesh;
            eyebrows_mr.material = eyebrowMat;

            eyebrows.transform.localPosition = Vector3.zero;
            eyebrows.transform.localRotation = newNPC.transform.rotation;
            Vector3 eyebrowRot = eyebrows.transform.localRotation.eulerAngles;
            eyebrowRot = new Vector3(eyebrowRot.x + 1.321f, eyebrowRot.y - 90, eyebrowRot.z);
            eyebrows.transform.localRotation = Quaternion.Euler(eyebrowRot);

            // Glasses
            // TODO: system for weighting mask much less
            // TODO: glasses color list
            if (Random.Range(0, 4) == 0)
            {
                Mesh glassesMesh = glassesMeshes[Random.Range(0, glassesMeshes.Length)];
                Material glassesMat = hairColors[Random.Range(0, hairColors.Length)];

                GameObject glasses = new GameObject();
                glasses.transform.SetParent(rootHead.transform);

                MeshRenderer glasses_mr = glasses.AddComponent<MeshRenderer>();
                MeshFilter glasses_mf = glasses.AddComponent<MeshFilter>();
                glasses_mf.mesh = glassesMesh;
                glasses_mr.material = glassesMat;

                glasses.transform.localScale = glassesScale;
                glasses.transform.localPosition = Vector3.zero;
                glasses.transform.localRotation = newNPC.transform.rotation;
                Vector3 glassesRot = glasses.transform.localRotation.eulerAngles;
                glassesRot = new Vector3(glassesRot.x + 1.321f, glassesRot.y - 90, glassesRot.z);
                glasses.transform.localRotation = Quaternion.Euler(glassesRot);
            }

            // Things that are specific to only Male NPCs
            if (isMale)
            {
                // Beard
                if (Random.Range(0, 2) == 0)
                {
                    Mesh beardMesh = beardMeshes[Random.Range(0, beardMeshes.Length)];

                    GameObject beard = new GameObject();
                    beard.transform.SetParent(rootHead.transform);

                    MeshRenderer beard_mr = beard.AddComponent<MeshRenderer>();
                    MeshFilter beard_mf = beard.AddComponent<MeshFilter>();
                    beard_mf.mesh = beardMesh;
                    beard_mr.material = (premium ? combinedHairColors[hairColorVal] : hairColors[hairColorVal]);

                    beard.transform.localPosition = Vector3.zero;
                    beard.transform.localRotation = newNPC.transform.rotation;
                    Vector3 beardRot = beard.transform.localRotation.eulerAngles;
                    beardRot = new Vector3(beardRot.x + 1.321f, beardRot.y - 90, beardRot.z);
                    beard.transform.localRotation = Quaternion.Euler(beardRot);
                }

                // Destroy all the placeholder gameobjects
                GameObject.Destroy(newNPC.transform.Find("male_01_belt").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_pants1_long").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_pants2_shorts").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_pants3_kilt").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_shirt1_long").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_shirt2_tee").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_shirt3_muscle").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_shirt4_suit").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_shirt5_twoLayer_Exterior").gameObject);
                GameObject.Destroy(newNPC.transform.Find("male_01_shirt5_twoLayer_Interior").gameObject);
            }

            // Things that are specific to only Female NPCs
            else
            {
                // Makeup
                // TODO: allow multiple makeups at one
                if (Random.Range(0, 2) == 0)
                {
                    GameObject makeup = new GameObject();
                    makeup.transform.SetParent(rootHead.transform);
                    MeshRenderer makeup_mr = makeup.AddComponent<MeshRenderer>();
                    MeshFilter makeup_mf = makeup.AddComponent<MeshFilter>();

                    Mesh makeupMesh = makeups[Random.Range(0, makeups.Length)];
                    Material makeupMat = makeupColors[Random.Range(0, makeupColors.Length)];

                    makeup_mf.mesh = makeupMesh;
                    makeup_mr.material = makeupMat;
                    makeup.transform.localScale = glassesScale;
                    makeup.transform.localPosition = Vector3.zero;
                    makeup.transform.localRotation = newNPC.transform.rotation;
                    Vector3 makeupRot = makeup.transform.localRotation.eulerAngles;
                    makeupRot = new Vector3(makeupRot.x + 1.321f, makeupRot.y - 90, makeupRot.z);
                    makeup.transform.localRotation = Quaternion.Euler(makeupRot);
                }

                // Destroy all the placeholder gameobjects
                GameObject.Destroy(newNPC.transform.Find("female_01_belt").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_pants1_long").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_pants2_shorts").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_pants3_skirt").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_pants3_longSkirt").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_dress1").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_dress2").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_dress3").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_shirt1_long").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_shirt2_tee").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_shirt3_muscle").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_shirt4_suit").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_shirt5_twoLayer_Exterior").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_shirt5_twoLayer_Interior").gameObject);
                GameObject.Destroy(newNPC.transform.Find("female_01_swimsuit").gameObject);
            }

            // Set manager steps 
            NPCManager manager = newNPC.GetComponent<NPCManager>();
            manager.isRandom = isRandom;

            string userName = "";

            if (action.reqs.ContainsKey(0))
            {
                userName = action.reqs[0];
            }
            else
            {
                var testy = randomUsernames.Where(w => w.genStatus == 0);
                var usernameGen = testy.ElementAt(Random.Range(0, testy.Count()));

                userName = usernameGen.username;

                xmlParser.UpdateUsername(userName);
            }

            // Update npc count
            numNPCs++;

            manager.SetUp(action, userName);
        }
    }
}
