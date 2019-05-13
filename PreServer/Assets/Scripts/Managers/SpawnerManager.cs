using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace PreServer
{
    public class SpawnerManager : MonoBehaviour
    {
        public int numNPCs;
        public Transform spawner;
        public XMLParser xmlParser;

        public List<NPCAction> npcActions;
        public List<NPCUsername> randomUsernames;

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

        public Vector3[] hairScales;

        public Vector3 glassesScale;

        public int maxNPCs;

        void Start()
        {
            maxNPCs = 5;

            spawner = gameObject.transform;
            numNPCs = 0;
            xmlParser = GetComponent<XMLParser>();

            npcActions = xmlParser.ParseXML();
            randomUsernames = xmlParser.ParseUsernames();

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

        void Update()
        {
            if (Random.Range(0, 1000) >= 999)
            {
                SpawnNPC();
            }
        }

        public void SpawnNPC()
        {
            // TODO: system for marking actions as already done
            // TODO: system for marking actions as "probably seen"
            // TODO: system for saving xml when these happen
            // TODO: system for only grabbing xxx amount of random messages that meet requirements

            if (numNPCs < maxNPCs)
            {
                NPCAction action = npcActions[Random.Range(0, npcActions.Count)];

                xmlParser.UpdateXML();

                // Use 'action' to see if npc has any generation requirements (username, gender, hat, etc)

                if (action.reqs.ContainsKey(1))
                {
                    if (action.reqs[1].Equals("m"))
                        SpawnMale(action);
                    else
                        SpawnFemale(action);
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                        SpawnMale(action);
                    else
                        SpawnFemale(action);
                }

                numNPCs++;
            }



        }

        public void SpawnMale(NPCAction action)
        {
            GameObject npc = Resources.Load<GameObject>("NPC/MALE_NPC");
            GameObject newNPC = Instantiate(npc, spawner.position, spawner.rotation);
            Transform root = newNPC.transform.Find("Squ_People3:root");
            Transform rootPelvis = root.transform.Find("Squ_People3:pelvis");
            Transform rootSpine = rootPelvis.transform.Find("Squ_People3:spine_01");
            Transform rootSpine2 = rootSpine.transform.Find("Squ_People3:spine_02");
            Transform rootSpine3 = rootSpine2.transform.Find("Squ_People3:spine_03");
            Transform rootNeck = rootSpine3.transform.Find("Squ_People3:neck_01");
            Transform rootHead = rootNeck.transform.Find("Squ_People3:head");

            Transform body = newNPC.transform.Find("male_01_body");
            SkinnedMeshRenderer body_smr = body.GetComponent<SkinnedMeshRenderer>();
            Texture skintone = skintones[Random.Range(0, skintones.Length)];
            body_smr.material.mainTexture = skintone;

            newNPC.transform.Find("head_01").GetComponent<SkinnedMeshRenderer>().material.mainTexture = skintone;

            int shirtValToUse = Random.Range(0, maleShirts.Length);
            Mesh shirtMesh = maleShirts[shirtValToUse];
            Material shirtMat = shirtColors[Random.Range(0, shirtColors.Length)];

            Transform shirt = newNPC.transform.Find(shirtMesh.name);
            SkinnedMeshRenderer shirt_smr = shirt.GetComponent<SkinnedMeshRenderer>();

            GameObject newShirt = new GameObject();
            newShirt.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newShirt_smr = newShirt.AddComponent<SkinnedMeshRenderer>();
            newShirt_smr.sharedMesh = shirtMesh;
            newShirt_smr.rootBone = root;
            newShirt_smr.bones = shirt_smr.bones;
            newShirt_smr.material = shirtMat;

            // If interior shirt, make the exterior shirt
            if (shirtValToUse == 4)
            {
                Transform shirt2 = newNPC.transform.Find("male_01_shirt5_twoLayer_Exterior");
                SkinnedMeshRenderer shirt2_smr = shirt2.GetComponent<SkinnedMeshRenderer>();

                GameObject newShirt2 = new GameObject();
                newShirt2.transform.SetParent(newNPC.transform);

                Mesh maleShirt2 = Resources.Load<Mesh>("NPC/Meshes/male_01_shirt5_twoLayer_Exterior");
                Material shirtMat2 = shirtColors[Random.Range(0, shirtColors.Length)];

                SkinnedMeshRenderer newShirt2_smr = newShirt2.AddComponent<SkinnedMeshRenderer>();
                newShirt2_smr.sharedMesh = maleShirt2;
                newShirt2_smr.rootBone = root;
                newShirt2_smr.bones = shirt2_smr.bones;
                newShirt2_smr.material = shirtMat2;
            }

            Texture pantText = pantColors[Random.Range(0, pantColors.Length)];
            int pantValToUse = Random.Range(0, malePants.Length);
            Mesh pantsMesh = malePants[pantValToUse];
            //Texture pantText = pantColors[Random.Range(0, pantColors.Length)];

            Transform pants = newNPC.transform.Find(pantsMesh.name);
            SkinnedMeshRenderer pants_smr = pants.GetComponent<SkinnedMeshRenderer>();

            GameObject newPants = new GameObject();
            newPants.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newPants_smr = newPants.AddComponent<SkinnedMeshRenderer>();
            newPants_smr.sharedMesh = pantsMesh;
            newPants_smr.rootBone = root;
            newPants_smr.bones = pants_smr.bones;
            newPants_smr.material.mainTexture = pantText;

            // Belt
            if (Random.Range(0, 2) == 0)
            {
                Transform belt = newNPC.transform.Find("male_01_belt");
                SkinnedMeshRenderer belt_smr = belt.GetComponent<SkinnedMeshRenderer>();

                GameObject newBelt = new GameObject();
                newBelt.transform.SetParent(newNPC.transform);

                Mesh beltMesh = Resources.Load<Mesh>("NPC/Meshes/male_01_belt");

                SkinnedMeshRenderer newBelt_smr = newBelt.AddComponent<SkinnedMeshRenderer>();
                newBelt_smr.sharedMesh = beltMesh;
                newBelt_smr.rootBone = root;
                newBelt_smr.bones = belt_smr.bones;
                newBelt_smr.material.mainTexture = pantText;
            }

            Transform eyes = rootHead.transform.Find("Squ_People3:eyesAndTeeth");
            MeshRenderer eyes_mr = eyes.GetComponent<MeshRenderer>();
            Texture eyeTexture = eyeColors[Random.Range(0, eyeColors.Length)];
            eyes_mr.material.mainTexture = eyeTexture;

            int hairValToUse = Random.Range(-1, hairs.Length);

            if (action.reqs.ContainsKey(2))
            {
                hairValToUse = int.Parse(action.reqs[2]);
            }

            int hairColorVal = Random.Range(0, hairColors.Length);

            // If hairValToUse is -1, npc is bald
            if (hairValToUse != -1)
            {
                GameObject hair = new GameObject();
                hair.transform.SetParent(rootHead.transform);
                MeshRenderer hair_mr = hair.AddComponent<MeshRenderer>();
                MeshFilter hair_mf = hair.AddComponent<MeshFilter>();

                Mesh hairMesh = hairs[hairValToUse];
                Material hairMat = hairColors[hairColorVal];

                hair_mf.mesh = hairMesh;
                hair_mr.material = hairMat;
                hair.transform.localScale = hairScales[hairValToUse];
                hair.transform.localPosition = Vector3.zero;
                hair.transform.localRotation = newNPC.transform.rotation;
                Vector3 hairRot = hair.transform.localRotation.eulerAngles;
                hairRot = new Vector3(hairRot.x + 1.321f, hairRot.y - 90, hairRot.z);
                hair.transform.localRotation = Quaternion.Euler(hairRot);
            }

            GameObject eyebrows = new GameObject();
            eyebrows.transform.SetParent(rootHead.transform);
            MeshRenderer eyebrows_mr = eyebrows.AddComponent<MeshRenderer>();
            MeshFilter eyebrows_mf = eyebrows.AddComponent<MeshFilter>();

            Mesh eyebrowMesh = eyebrowMeshes[Random.Range(0, eyebrowMeshes.Length)];
            Material eyebrowMat = hairColors[hairColorVal];

            eyebrows_mf.mesh = eyebrowMesh;
            eyebrows_mr.material = eyebrowMat;
            eyebrows.transform.localPosition = Vector3.zero;
            eyebrows.transform.localRotation = newNPC.transform.rotation;
            Vector3 eyebrowRot = eyebrows.transform.localRotation.eulerAngles;
            eyebrowRot = new Vector3(eyebrowRot.x + 1.321f, eyebrowRot.y - 90, eyebrowRot.z);
            eyebrows.transform.localRotation = Quaternion.Euler(eyebrowRot);

            // Beard
            if (Random.Range(0, 2) == 0)
            {
                GameObject beard = new GameObject();
                beard.transform.SetParent(rootHead.transform);
                MeshRenderer beard_mr = beard.AddComponent<MeshRenderer>();
                MeshFilter beard_mf = beard.AddComponent<MeshFilter>();

                Mesh beardMesh = beardMeshes[Random.Range(0, beardMeshes.Length)];

                beard_mf.mesh = beardMesh;
                beard_mr.material = hairColors[hairColorVal];
                beard.transform.localPosition = Vector3.zero;
                beard.transform.localRotation = newNPC.transform.rotation;
                Vector3 beardRot = beard.transform.localRotation.eulerAngles;
                beardRot = new Vector3(beardRot.x + 1.321f, beardRot.y - 90, beardRot.z);
                beard.transform.localRotation = Quaternion.Euler(beardRot);
            }

            // Glasses
            // TODO: system for weighting mask much less
            // TODO: glasses color list
            if (Random.Range(0, 4) == 0)
            {
                GameObject glasses = new GameObject();
                glasses.transform.SetParent(rootHead.transform);
                MeshRenderer glasses_mr = glasses.AddComponent<MeshRenderer>();
                MeshFilter glasses_mf = glasses.AddComponent<MeshFilter>();

                Mesh glassesMesh = glassesMeshes[Random.Range(0, glassesMeshes.Length)];
                Material glassesMat = hairColors[Random.Range(0, hairColors.Length)];

                glasses_mf.mesh = glassesMesh;
                glasses_mr.material = glassesMat;
                glasses.transform.localScale = glassesScale;
                glasses.transform.localPosition = Vector3.zero;
                glasses.transform.localRotation = newNPC.transform.rotation;
                Vector3 glassesRot = glasses.transform.localRotation.eulerAngles;
                glassesRot = new Vector3(glassesRot.x + 1.321f, glassesRot.y - 90, glassesRot.z);
                glasses.transform.localRotation = Quaternion.Euler(glassesRot);
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

            // Set manager steps 
            NPCManager manager = newNPC.GetComponent<NPCManager>();

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
            }

            manager.SetUp(action, userName);
        }

        public void SpawnFemale(NPCAction action)
        {
            GameObject npc = Resources.Load<GameObject>("NPC/FEMALE_NPC");
            GameObject newNPC = Instantiate(npc, spawner.position, spawner.rotation);
            Transform root = newNPC.transform.Find("Squ_People3:root");
            Transform rootPelvis = root.transform.Find("Squ_People3:pelvis");
            Transform rootSpine = rootPelvis.transform.Find("Squ_People3:spine_01");
            Transform rootSpine2 = rootSpine.transform.Find("Squ_People3:spine_02");
            Transform rootSpine3 = rootSpine2.transform.Find("Squ_People3:spine_03");
            Transform rootNeck = rootSpine3.transform.Find("Squ_People3:neck_01");
            Transform rootHead = rootNeck.transform.Find("Squ_People3:head");

            Transform body = newNPC.transform.Find("female_01_body");
            SkinnedMeshRenderer body_smr = body.GetComponent<SkinnedMeshRenderer>();
            Texture skintone = skintones[Random.Range(0, skintones.Length)];
            body_smr.material.mainTexture = skintone;

            newNPC.transform.Find("head_01").GetComponent<SkinnedMeshRenderer>().material.mainTexture = skintone;

            int shirtValToUse = Random.Range(0, femaleShirts.Length);
            Mesh femShirt = femaleShirts[shirtValToUse];
            Material shirtMat = shirtColors[Random.Range(0, shirtColors.Length)];

            Transform shirt = newNPC.transform.Find(femShirt.name);
            SkinnedMeshRenderer shirt_smr = shirt.GetComponent<SkinnedMeshRenderer>();

            GameObject newShirt = new GameObject();
            newShirt.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newShirt_smr = newShirt.AddComponent<SkinnedMeshRenderer>();
            newShirt_smr.sharedMesh = femShirt;
            newShirt_smr.rootBone = root;
            newShirt_smr.bones = shirt_smr.bones;
            newShirt_smr.material = shirtMat;

            // If interior shirt, make the exterior shirt
            if (shirtValToUse == 7)
            {
                Transform shirt2 = newNPC.transform.Find("female_01_shirt5_twoLayer_Exterior");
                SkinnedMeshRenderer shirt2_smr = shirt2.GetComponent<SkinnedMeshRenderer>();

                GameObject newShirt2 = new GameObject();
                newShirt2.transform.SetParent(newNPC.transform);

                Mesh femShirt2 = Resources.Load<Mesh>("NPC/Meshes/female_01_shirt5_twoLayer_Exterior");
                Material shirtMat2 = shirtColors[Random.Range(0, shirtColors.Length)];

                SkinnedMeshRenderer newShirt2_smr = newShirt2.AddComponent<SkinnedMeshRenderer>();
                newShirt2_smr.sharedMesh = femShirt2;
                newShirt2_smr.rootBone = root;
                newShirt2_smr.bones = shirt2_smr.bones;
                newShirt2_smr.material = shirtMat2;
            }

            Texture pantText = pantColors[Random.Range(0, pantColors.Length)];

            // No pants for dresses (0,1,2) and swimsuit (8)
            if (shirtValToUse != 0 && shirtValToUse != 1 && shirtValToUse != 2 && shirtValToUse != 8)
            {
                int pantValToUse = Random.Range(0, femalePants.Length);
                Mesh femPants = femalePants[pantValToUse];
                //Texture pantText = pantColors[Random.Range(0, pantColors.Length)];

                Transform pants = newNPC.transform.Find(femPants.name);
                SkinnedMeshRenderer pants_smr = pants.GetComponent<SkinnedMeshRenderer>();

                GameObject newPants = new GameObject();
                newPants.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newPants_smr = newPants.AddComponent<SkinnedMeshRenderer>();
                newPants_smr.sharedMesh = femPants;
                newPants_smr.rootBone = root;
                newPants_smr.bones = pants_smr.bones;
                newPants_smr.material.mainTexture = pantText;
            }

            // Belt
            // No belt for dresses (0,1,2) and swimsuit (8)
            if (Random.Range(0, 2) == 0 && shirtValToUse != 0 && shirtValToUse != 1 && shirtValToUse != 2 && shirtValToUse != 8)
            {
                Transform belt = newNPC.transform.Find("female_01_belt");
                SkinnedMeshRenderer belt_smr = belt.GetComponent<SkinnedMeshRenderer>();

                GameObject newBelt = new GameObject();
                newBelt.transform.SetParent(newNPC.transform);

                Mesh beltMesh = Resources.Load<Mesh>("NPC/Meshes/female_01_belt");

                SkinnedMeshRenderer newBelt_smr = newBelt.AddComponent<SkinnedMeshRenderer>();
                newBelt_smr.sharedMesh = beltMesh;
                newBelt_smr.rootBone = root;
                newBelt_smr.bones = belt_smr.bones;
                newBelt_smr.material.mainTexture = pantText;
            }

            Transform eyes = rootHead.transform.Find("Squ_People3:eyesAndTeeth");
            MeshRenderer eyes_mr = eyes.GetComponent<MeshRenderer>();
            Texture eyeTexture = eyeColors[Random.Range(0, eyeColors.Length)];
            eyes_mr.material.mainTexture = eyeTexture;

            int hairValToUse = Random.Range(-1, hairs.Length);

            if (action.reqs.ContainsKey(2))
            {
                hairValToUse = int.Parse(action.reqs[2]);
            }

            int hairColorVal = Random.Range(0, hairColors.Length);

            // If hairValToUse is -1, npc is bald
            if (hairValToUse != -1)
            {
                GameObject hair = new GameObject();
                hair.transform.SetParent(rootHead.transform);
                MeshRenderer hair_mr = hair.AddComponent<MeshRenderer>();
                MeshFilter hair_mf = hair.AddComponent<MeshFilter>();

                Mesh hairMesh = hairs[hairValToUse];
                Material hairMat = hairColors[hairColorVal];

                hair_mf.mesh = hairMesh;
                hair_mr.material = hairMat;
                hair.transform.localScale = hairScales[hairValToUse];
                hair.transform.localPosition = Vector3.zero;
                hair.transform.localRotation = newNPC.transform.rotation;
                Vector3 hairRot = hair.transform.localRotation.eulerAngles;
                hairRot = new Vector3(hairRot.x + 1.321f, hairRot.y - 90, hairRot.z);
                hair.transform.localRotation = Quaternion.Euler(hairRot);
            }

            GameObject eyebrows = new GameObject();
            eyebrows.transform.SetParent(rootHead.transform);
            MeshRenderer eyebrows_mr = eyebrows.AddComponent<MeshRenderer>();
            MeshFilter eyebrows_mf = eyebrows.AddComponent<MeshFilter>();

            Mesh eyebrowMesh = eyebrowMeshes[Random.Range(0, eyebrowMeshes.Length)];
            Material eyebrowMat = hairColors[hairColorVal];

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
                GameObject glasses = new GameObject();
                glasses.transform.SetParent(rootHead.transform);
                MeshRenderer glasses_mr = glasses.AddComponent<MeshRenderer>();
                MeshFilter glasses_mf = glasses.AddComponent<MeshFilter>();

                Mesh glassesMesh = glassesMeshes[Random.Range(0, glassesMeshes.Length)];
                Material glassesMat = hairColors[Random.Range(0, hairColors.Length)];

                glasses_mf.mesh = glassesMesh;
                glasses_mr.material = glassesMat;
                glasses.transform.localScale = glassesScale;
                glasses.transform.localPosition = Vector3.zero;
                glasses.transform.localRotation = newNPC.transform.rotation;
                Vector3 glassesRot = glasses.transform.localRotation.eulerAngles;
                glassesRot = new Vector3(glassesRot.x + 1.321f, glassesRot.y - 90, glassesRot.z);
                glasses.transform.localRotation = Quaternion.Euler(glassesRot);
            }

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

            // Set manager steps 
            NPCManager manager = newNPC.GetComponent<NPCManager>();

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
            }

            manager.SetUp(action, userName);
        }
    }
}
