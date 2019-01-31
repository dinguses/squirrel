using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

namespace PreServer
{
    public class NPCGen : MonoBehaviour
    { 
        public GameObject maleNPC;
        public GameObject femaleNPC;

        public Mesh maleBeltMesh;
        public Mesh femaleBeltMesh;

        public Mesh malePant1;
        public Mesh malePant2;
        public Mesh malePant3;

        public Mesh femalePant1;
        public Mesh femalePant2;
        public Mesh femalePant3Long;
        public Mesh femalePant3Skirt;

        public Mesh maleShirt1;
        public Mesh maleShirt2;
        public Mesh maleShirt3;
        public Mesh maleShirt4;
        public Mesh maleShirt5ext;
        public Mesh maleShirt5int;

        public Mesh femaleDress1;
        public Mesh femaleDress2;
        public Mesh femaleDress3;
        public Mesh femaleShirt1;
        public Mesh femaleShirt2;
        public Mesh femaleShirt3;
        public Mesh femaleShirt4;
        public Mesh femaleShirt5ext;
        public Mesh femaleShirt5int;
        public Mesh femaleSwim;

        public Mesh eyesMesh;

        public Mesh hairMesh1;
        public Mesh hairMesh2;
        public Mesh hairMesh3;
        public Mesh hairMesh4;
        public Mesh hairMesh5;
        public Mesh hairMesh6;
        public Mesh hairMesh7;
        public Mesh hairMesh8;
        public Mesh hairMesh9;
        public Mesh hairMesh10;
        public Mesh hairMesh11;
        public Mesh hairMesh12;
        public Mesh hairMesh13;
        public Mesh hairMesh14;
        public Mesh hairMesh15;
        public Mesh hairMesh16;
        public Mesh hairMesh17;

        public Mesh eyebrowMesh1;
        public Mesh eyebrowMesh2;
        public Mesh eyebrowMesh3;

        public Mesh glassesMesh1;
        public Mesh glassesMesh2;
        public Mesh glassesMesh3;
        public Mesh glassesMesh4;

        public Mesh beardMesh1;
        public Mesh beardMesh2;
        public Mesh beardMesh3;
        public Mesh beardMesh4;
        public Mesh beardMesh5;
        public Mesh beardMesh6;
        public Mesh beardMesh7;
        public Mesh beardMesh8;
        public Mesh beardMesh9;

        public Texture skin1;
        public Texture skin2;
        public Texture skin3;
        public Texture skin4;
        public Texture skin5;
        public Texture skin6;
        public Texture skin7;
        public Texture skin8;
        public Texture skin9;

        public Mesh makeup1;
        public Mesh makeup2;

        [HideInInspector]
        public Texture[] skins;
        [HideInInspector]
        public Mesh[] hairs;
        [HideInInspector]
        public Material[] hairColors;
        [HideInInspector]
        public Vector3[] hairScales;
        [HideInInspector]
        public Mesh[] eyebrows;
        [HideInInspector]
        public Mesh[] glasses;
        [HideInInspector]
        public Mesh[] beards;
        [HideInInspector]
        public Texture[] eyes;
        [HideInInspector]
        public Mesh[] maleShirts;
        [HideInInspector]
        public Mesh[] malePants;
        [HideInInspector]
        public Material[] shirtColors;
        [HideInInspector]
        public Texture[] pantColors;
        [HideInInspector]
        public Mesh[] femaleShirts;
        [HideInInspector]
        public Mesh[] femalePants;
        [HideInInspector]
        public Mesh[] makeups;

        public Texture pants1;
        public Texture pants2;
        public Texture pants3;
        public Texture pants4;
        public Texture pants5;
        public Texture pants6;

        public Material shirt1;
        public Material shirt2;
        public Material shirt3;
        public Material shirt4;
        public Material shirt5;
        public Material shirt6;
        public Material shirt7;

        public Material belt1;

        public Texture eyes1;
        public Texture eyes2;
        public Texture eyes3;
        public Texture eyes4;
        public Texture eyes5;
        public Texture eyes6;

        public Material hair1;
        public Material hair2;
        public Material hair3;
        public Material hair4;

        public Text numPeeps;

        public int numNPCS = 0;
        public int xNum = 0;
        public int zNum = 0;

        private Vector3 spawnLoc = new Vector3(132f, 5.6f, -75.36f);

        public Dropdown sexDD;
        public Dropdown skinDD;
        public Dropdown hairDD;
        public Dropdown hairColorDD;
        public Dropdown eyebrowDD;
        public Dropdown glassesDD;
        public Dropdown beltDD;
        public Dropdown beardDD;
        public Dropdown eyeColorDD;
        public Dropdown maleShirtDD;
        public Dropdown malePantsDD;
        public Dropdown shirtColorDD;
        public Dropdown pantColorDD;
        public Dropdown motionDD;
        public Dropdown shirtColor2DD;
        public Dropdown makeupDD;
        public Dropdown femaleShirtsDD;
        public Dropdown femalePantsDD;

        public Text sexDDLabel;
        public Text skinDDLabel;
        public Text hairDDLabel;
        public Text hairColorDDLabel;
        public Text eyebrowsLabel;
        public Text glassesLabel;
        public Text beltLabel;
        public Text beardLabel;
        public Text eyeColorLabel;
        public Text maleShirtLabel;
        public Text malePantsLabel;
        public Text shirtColorLabel;
        public Text pantColorLabel;
        public Text motionLabel;
        public Text shirtColor2Label;
        public Text makeupLabel;
        public Text femaleShirtLabel;
        public Text femalePantsLabel;

        public UnityEngine.UI.Button GenNPCButton;
        public UnityEngine.UI.Button randButton;

        private Vector3 hairScale1 = new Vector3(0.9818031f, 1.014686f, 1.014686f);
        private Vector3 hairScale2 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale3 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale4 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale5 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale6 = new Vector3(0.996021f, 1.02938f, 1.02938f);
        private Vector3 hairScale7 = new Vector3(1, 1, 1);
        private Vector3 hairScale8 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale9 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale10 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale11 = new Vector3(0.3656405f, 2.746532f, 0.2447241f);
        private Vector3 hairScale12 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale13 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale14 = new Vector3(1, 1, 1);
        private Vector3 hairScale15 = new Vector3(0.9715822f, 1.004123f, 1.004123f);
        private Vector3 hairScale16 = new Vector3(1, 1, 1);
        private Vector3 hairScale17 = new Vector3(0.9715822f, 1.004123f, 1.004123f);

        private Vector3 glassesScale = new Vector3(0.9675938f, 1, 1);

        private bool hasGlasses = false;
        private bool hasBelt = false;
        private bool hasBeard = false;
        private bool intExtShirt = false;
        private bool hasMakeup = false;
        private bool hidden = false;

        private int shirtToUseVal = 0;
        private int pantsToUseVal = 0;
        private int motionToUseVal = 0;

        public void HideShow()
        {
            sexDD.gameObject.SetActive(!sexDD.gameObject.activeInHierarchy);
            skinDD.gameObject.SetActive(!skinDD.gameObject.activeInHierarchy);
            hairDD.gameObject.SetActive(!hairDD.gameObject.activeInHierarchy);
            hairColorDD.gameObject.SetActive(!hairColorDD.gameObject.activeInHierarchy);
            eyebrowDD.gameObject.SetActive(!eyebrowDD.gameObject.activeInHierarchy);
            glassesDD.gameObject.SetActive(!glassesDD.gameObject.activeInHierarchy);
            beltDD.gameObject.SetActive(!beltDD.gameObject.activeInHierarchy);
            beardDD.gameObject.SetActive(!beardDD.gameObject.activeInHierarchy);
            eyeColorDD.gameObject.SetActive(!eyeColorDD.gameObject.activeInHierarchy);
            maleShirtDD.gameObject.SetActive(!maleShirtDD.gameObject.activeInHierarchy);
            malePantsDD.gameObject.SetActive(!malePantsDD.gameObject.activeInHierarchy);
            shirtColorDD.gameObject.SetActive(!shirtColorDD.gameObject.activeInHierarchy);
            pantColorDD.gameObject.SetActive(!pantColorDD.gameObject.activeInHierarchy);
            motionDD.gameObject.SetActive(!motionDD.gameObject.activeInHierarchy);
            shirtColor2DD.gameObject.SetActive(!shirtColor2DD.gameObject.activeInHierarchy);
            makeupDD.gameObject.SetActive(!makeupDD.gameObject.activeInHierarchy);
            femaleShirtsDD.gameObject.SetActive(!femaleShirtsDD.gameObject.activeInHierarchy);
            femalePantsDD.gameObject.SetActive(!femalePantsDD.gameObject.activeInHierarchy);


            sexDDLabel.gameObject.SetActive(!sexDDLabel.gameObject.activeInHierarchy);
            skinDDLabel.gameObject.SetActive(!skinDDLabel.gameObject.activeInHierarchy);
            hairDDLabel.gameObject.SetActive(!hairDDLabel.gameObject.activeInHierarchy);
            hairColorDDLabel.gameObject.SetActive(!hairColorDDLabel.gameObject.activeInHierarchy);
            eyebrowsLabel.gameObject.SetActive(!eyebrowsLabel.gameObject.activeInHierarchy);
            glassesLabel.gameObject.SetActive(!glassesLabel.gameObject.activeInHierarchy);
            beltLabel.gameObject.SetActive(!beltLabel.gameObject.activeInHierarchy);
            beardLabel.gameObject.SetActive(!beardLabel.gameObject.activeInHierarchy);
            eyeColorLabel.gameObject.SetActive(!eyeColorLabel.gameObject.activeInHierarchy);
            maleShirtLabel.gameObject.SetActive(!maleShirtLabel.gameObject.activeInHierarchy);
            malePantsLabel.gameObject.SetActive(!malePantsLabel.gameObject.activeInHierarchy);
            shirtColorLabel.gameObject.SetActive(!shirtColorLabel.gameObject.activeInHierarchy);
            pantColorLabel.gameObject.SetActive(!pantColorLabel.gameObject.activeInHierarchy);
            motionLabel.gameObject.SetActive(!motionLabel.gameObject.activeInHierarchy);
            shirtColor2Label.gameObject.SetActive(!shirtColor2Label.gameObject.activeInHierarchy);
            makeupLabel.gameObject.SetActive(!makeupLabel.gameObject.activeInHierarchy);
            femaleShirtLabel.gameObject.SetActive(!femaleShirtLabel.gameObject.activeInHierarchy);
            femalePantsLabel.gameObject.SetActive(!femalePantsLabel.gameObject.activeInHierarchy);

            GenNPCButton.gameObject.SetActive(!GenNPCButton.gameObject.activeInHierarchy);
        }

        public void Hide()
        {
            if (!hidden)
            {
                sexDD.gameObject.SetActive(false);
                skinDD.gameObject.SetActive(false); ;
                hairDD.gameObject.SetActive(false);
                hairColorDD.gameObject.SetActive(false);
                eyebrowDD.gameObject.SetActive(false);
                glassesDD.gameObject.SetActive(false);
                beltDD.gameObject.SetActive(false);
                beardDD.gameObject.SetActive(false);
                eyeColorDD.gameObject.SetActive(false);
                maleShirtDD.gameObject.SetActive(false);
                malePantsDD.gameObject.SetActive(false);
                shirtColorDD.gameObject.SetActive(false);
                pantColorDD.gameObject.SetActive(false);
                motionDD.gameObject.SetActive(false);
                shirtColor2DD.gameObject.SetActive(false);
                makeupDD.gameObject.SetActive(false);
                femaleShirtsDD.gameObject.SetActive(false);
                femalePantsDD.gameObject.SetActive(false);


                sexDDLabel.gameObject.SetActive(false);
                skinDDLabel.gameObject.SetActive(false);
                hairDDLabel.gameObject.SetActive(false);
                hairColorDDLabel.gameObject.SetActive(false);
                eyebrowsLabel.gameObject.SetActive(false);
                glassesLabel.gameObject.SetActive(false);
                beltLabel.gameObject.SetActive(false);
                beardLabel.gameObject.SetActive(false);
                eyeColorLabel.gameObject.SetActive(false);
                maleShirtLabel.gameObject.SetActive(false);
                malePantsLabel.gameObject.SetActive(false);
                shirtColorLabel.gameObject.SetActive(false);
                pantColorLabel.gameObject.SetActive(false);
                motionLabel.gameObject.SetActive(false);
                shirtColor2Label.gameObject.SetActive(false);
                makeupLabel.gameObject.SetActive(false);
                femaleShirtLabel.gameObject.SetActive(false);
                femalePantsLabel.gameObject.SetActive(false);

                GenNPCButton.gameObject.SetActive(false);
                randButton.gameObject.SetActive(false);

                hidden = true;
            }
            else
            {
                Show();
            }
        }

        public void Show()
        {
            sexDD.gameObject.SetActive(true);
            sexDDLabel.gameObject.SetActive(true);
            skinDD.gameObject.SetActive(true);
            hairDD.gameObject.SetActive(true);
            hairColorDD.gameObject.SetActive(true);
            eyebrowDD.gameObject.SetActive(true);
            glassesDD.gameObject.SetActive(true);
            beltDD.gameObject.SetActive(true);
            eyeColorDD.gameObject.SetActive(true);
            shirtColorDD.gameObject.SetActive(true);
            pantColorDD.gameObject.SetActive(true);
            motionDD.gameObject.SetActive(true);
            skinDDLabel.gameObject.SetActive(true);
            hairDDLabel.gameObject.SetActive(true);
            hairColorDDLabel.gameObject.SetActive(true);
            eyebrowsLabel.gameObject.SetActive(true);
            glassesLabel.gameObject.SetActive(true);
            beltLabel.gameObject.SetActive(true);
            eyeColorLabel.gameObject.SetActive(true);
            shirtColorLabel.gameObject.SetActive(true);
            pantColorLabel.gameObject.SetActive(true);
            motionLabel.gameObject.SetActive(true);


            if (sexDD.value == 0)
            {
                beardDD.gameObject.SetActive(true);
                maleShirtDD.gameObject.SetActive(true);
                malePantsDD.gameObject.SetActive(true);
                beardLabel.gameObject.SetActive(true);
                maleShirtLabel.gameObject.SetActive(true);
                malePantsLabel.gameObject.SetActive(true);

                if (maleShirtDD.value == 4)
                {
                    shirtColor2Label.gameObject.SetActive(true);
                    shirtColor2DD.gameObject.SetActive(true);
                }
            }
            else
            {
                makeupDD.gameObject.SetActive(true);
                femaleShirtsDD.gameObject.SetActive(true);
                femalePantsDD.gameObject.SetActive(true);
                makeupLabel.gameObject.SetActive(true);
                femaleShirtLabel.gameObject.SetActive(true);
                femalePantsLabel.gameObject.SetActive(true);

                if (femalePantsDD.value == 4)
                {
                    shirtColor2Label.gameObject.SetActive(true);
                    shirtColor2DD.gameObject.SetActive(true);
                }
            }

            GenNPCButton.gameObject.SetActive(true);
            randButton.gameObject.SetActive(true);

            hidden = false;
        }

        public void Start()
        {
            skins = new Texture[9];
            skins[0] = skin1;
            skins[1] = skin2;
            skins[2] = skin3;
            skins[3] = skin4;
            skins[4] = skin5;
            skins[5] = skin6;
            skins[6] = skin7;
            skins[7] = skin8;
            skins[8] = skin9;

            hairs = new Mesh[17];
            hairs[0] = hairMesh1;
            hairs[1] = hairMesh2;
            hairs[2] = hairMesh3;
            hairs[3] = hairMesh4;
            hairs[4] = hairMesh5;
            hairs[5] = hairMesh6;
            hairs[6] = hairMesh7;
            hairs[7] = hairMesh8;
            hairs[8] = hairMesh9;
            hairs[9] = hairMesh10;
            hairs[10] = hairMesh11;
            hairs[11] = hairMesh12;
            hairs[12] = hairMesh13;
            hairs[13] = hairMesh14;
            hairs[14] = hairMesh15;
            hairs[15] = hairMesh16;
            hairs[16] = hairMesh17;

            hairColors = new Material[4];
            hairColors[0] = hair1;
            hairColors[1] = hair2;
            hairColors[2] = hair3;
            hairColors[3] = hair4;

            hairScales = new Vector3[17];
            hairScales[0] = hairScale1;
            hairScales[1] = hairScale2;
            hairScales[2] = hairScale3;
            hairScales[3] = hairScale4;
            hairScales[4] = hairScale5;
            hairScales[5] = hairScale6;
            hairScales[6] = hairScale7;
            hairScales[7] = hairScale8;
            hairScales[8] = hairScale9;
            hairScales[9] = hairScale10;
            hairScales[10] = hairScale11;
            hairScales[11] = hairScale12;
            hairScales[12] = hairScale13;
            hairScales[13] = hairScale14;
            hairScales[14] = hairScale15;
            hairScales[15] = hairScale16;
            hairScales[16] = hairScale17;

            eyebrows = new Mesh[3];
            eyebrows[0] = eyebrowMesh1;
            eyebrows[1] = eyebrowMesh2;
            eyebrows[2] = eyebrowMesh3;

            glasses = new Mesh[4];
            glasses[0] = glassesMesh1;
            glasses[1] = glassesMesh2;
            glasses[2] = glassesMesh3;
            glasses[3] = glassesMesh4;

            beards = new Mesh[9];
            beards[0] = beardMesh1;
            beards[1] = beardMesh2;
            beards[2] = beardMesh3;
            beards[3] = beardMesh4;
            beards[4] = beardMesh5;
            beards[5] = beardMesh6;
            beards[6] = beardMesh7;
            beards[7] = beardMesh8;
            beards[8] = beardMesh9;

            eyes = new Texture[6];
            eyes[0] = eyes1;
            eyes[1] = eyes2;
            eyes[2] = eyes3;
            eyes[3] = eyes4;
            eyes[4] = eyes5;
            eyes[5] = eyes6;

            maleShirts = new Mesh[6];
            maleShirts[0] = maleShirt1;
            maleShirts[1] = maleShirt2;
            maleShirts[2] = maleShirt3;
            maleShirts[3] = maleShirt4;
            maleShirts[4] = maleShirt5ext;
            maleShirts[5] = maleShirt5int;

            malePants = new Mesh[3];
            malePants[0] = malePant1;
            malePants[1] = malePant2;
            malePants[2] = malePant3;

            shirtColors = new Material[7];
            shirtColors[0] = shirt1;
            shirtColors[1] = shirt2;
            shirtColors[2] = shirt3;
            shirtColors[3] = shirt4;
            shirtColors[4] = shirt5;
            shirtColors[5] = shirt6;
            shirtColors[6] = shirt7;

            pantColors = new Texture[6];
            pantColors[0] = pants1;
            pantColors[1] = pants2;
            pantColors[2] = pants3;
            pantColors[3] = pants4;
            pantColors[4] = pants5;
            pantColors[5] = pants6;

            makeups = new Mesh[2];
            makeups[0] = makeup1;
            makeups[1] = makeup2;

            femaleShirts = new Mesh[10];
            femaleShirts[0] = femaleShirt1;
            femaleShirts[1] = femaleShirt2;
            femaleShirts[2] = femaleShirt3;
            femaleShirts[3] = femaleShirt4;
            femaleShirts[4] = femaleShirt5ext;
            femaleShirts[5] = femaleShirt5int;
            femaleShirts[6] = femaleDress1;
            femaleShirts[7] = femaleDress2;
            femaleShirts[8] = femaleDress3;
            femaleShirts[9] = femaleSwim;

            femalePants = new Mesh[4];
            femalePants[0] = femalePant1;
            femalePants[1] = femalePant2;
            femalePants[2] = femalePant3Long;
            femalePants[3] = femalePant3Skirt;
        }

        public void GenNPC()
        {
            if (sexDD.value == 0)
            {
                var skinValue = skinDD.value;
                Texture skinToUse = skins[skinValue];

                var hairValue = hairDD.value;
                Mesh hairToUse = hairs[hairValue];

                var scaleValue = hairValue;
                Vector3 scaleToUse = hairScales[scaleValue];

                var hairColorValue = hairColorDD.value;
                Material hairColorToUse = hairColors[hairColorValue];

                var eyebrowsValue = eyebrowDD.value;
                Mesh eyebrowToUse = eyebrows[eyebrowsValue];

                var glassesValue = glassesDD.value;
                Mesh glassesToUse = new Mesh();
                if (glassesValue == 0)
                {
                    hasGlasses = false;
                }
                else
                {
                    hasGlasses = true;
                    glassesToUse = glasses[glassesValue - 1];
                }

                var beltValue = beltDD.value;
                if (beltValue == 0)
                {
                    hasBelt = true;
                }
                else
                {
                    hasBelt = false;
                }

                var beardValue = beardDD.value;
                Mesh beardToUse = new Mesh();
                if (beardValue == 0)
                {
                    hasBeard = false;
                }
                else
                {
                    hasBeard = true;
                    beardToUse = beards[beardValue - 1];
                }

                var eyeColorValue = eyeColorDD.value;
                Texture eyeColorToUse = eyes[eyeColorValue];

                var shirtValue = maleShirtDD.value;

                Material shirtToUse2 = null;

                if (shirtValue == 4)
                {
                    intExtShirt = true;
                    shirtToUse2 = shirtColors[shirtColor2DD.value];
                }
                else
                {
                    intExtShirt = false;
                }

                Mesh shirtToUse = maleShirts[shirtValue];
                shirtToUseVal = shirtValue;

                var pantsValue = malePantsDD.value;
                Mesh pantsToUse = malePants[pantsValue];
                pantsToUseVal = pantsValue;

                var shirtColorValue = shirtColorDD.value;
                Material shirtColorToUse = shirtColors[shirtColorValue];

                var pantColorValue = pantColorDD.value;
                Texture pantColorToUse = pantColors[pantColorValue];

                motionToUseVal = motionDD.value;

                GenMale(skinToUse, hairToUse, scaleToUse, hairColorToUse, eyebrowToUse, glassesToUse, beardToUse, eyeColorToUse, shirtToUse, pantsToUse, shirtColorToUse, pantColorToUse, shirtToUse2);
            }
            else
            {
                var skinValue = skinDD.value;
                Texture skinToUse = skins[skinValue];

                var hairValue = hairDD.value;
                Mesh hairToUse = hairs[hairValue];

                var scaleValue = hairValue;
                Vector3 scaleToUse = hairScales[scaleValue];

                var hairColorValue = hairColorDD.value;
                Material hairColorToUse = hairColors[hairColorValue];

                var eyebrowsValue = eyebrowDD.value;
                Mesh eyebrowToUse = eyebrows[eyebrowsValue];

                var glassesValue = glassesDD.value;
                Mesh glassesToUse = new Mesh();
                if (glassesValue == 0)
                {
                    hasGlasses = false;
                }
                else
                {
                    hasGlasses = true;
                    glassesToUse = glasses[glassesValue - 1];
                }

                var beltValue = beltDD.value;
                if (beltValue == 0)
                {
                    hasBelt = true;
                }
                else
                {
                    hasBelt = false;
                }

                var eyeColorValue = eyeColorDD.value;
                Texture eyeColorToUse = eyes[eyeColorValue];

                var shirtValue = femaleShirtsDD.value;

                Material shirtToUse2 = null;

                if (shirtValue == 4)
                {
                    intExtShirt = true;
                    shirtToUse2 = shirtColors[shirtColor2DD.value];
                }
                else
                {
                    intExtShirt = false;
                }

                if (shirtValue >= 5)
                {
                    shirtValue++;
                }
                Mesh shirtToUse = femaleShirts[shirtValue];
                shirtToUseVal = shirtValue;

                var pantsValue = femalePantsDD.value;
                Mesh pantsToUse = femalePants[pantsValue];
                pantsToUseVal = pantsValue;

                var shirtColorValue = shirtColorDD.value;
                Material shirtColorToUse = shirtColors[shirtColorValue];

                var pantColorValue = pantColorDD.value;
                Texture pantColorToUse = pantColors[pantColorValue];

                var makeupValue = makeupDD.value;
                Mesh makeupToUse = null;
                if (makeupValue == 0)
                {
                    hasMakeup = false;
                }
                else
                {
                    hasMakeup = true;
                    makeupToUse = makeups[makeupValue - 1];
                }

                motionToUseVal = motionDD.value;

                GenFemale(skinToUse, hairToUse, scaleToUse, hairColorToUse, eyebrowToUse, glassesToUse, eyeColorToUse, shirtToUse, pantsToUse, shirtColorToUse, pantColorToUse, makeupToUse, shirtToUse2);
            }
        }

        public void checkIntExt()
        {
            var sexVal = sexDD.value;

            if (sexVal == 0)
            {
                var maleShirtVal = maleShirtDD.value;

                if (maleShirtVal == 4)
                {
                    shirtColor2DD.gameObject.SetActive(true);
                    shirtColor2Label.gameObject.SetActive(true);
                }
                else
                {
                    shirtColor2DD.gameObject.SetActive(false);
                    shirtColor2Label.gameObject.SetActive(false);
                }
            }

            else
            {
                var femaleShirtVal = femaleShirtsDD.value;

                if (femaleShirtVal == 4)
                {
                    shirtColor2DD.gameObject.SetActive(true);
                    shirtColor2Label.gameObject.SetActive(true);
                }
                else
                {
                    shirtColor2DD.gameObject.SetActive(false);
                    shirtColor2Label.gameObject.SetActive(false);
                }
            }


        }

        public void sexChanged()
        {
            var sexVal = sexDD.value;
            
            if (sexVal == 0)
            {
                beardDD.gameObject.SetActive(true);
                beardLabel.gameObject.SetActive(true);
                maleShirtDD.gameObject.SetActive(true);
                maleShirtLabel.gameObject.SetActive(true);
                malePantsDD.gameObject.SetActive(true);
                malePantsLabel.gameObject.SetActive(true);
                

                femaleShirtsDD.gameObject.SetActive(false);
                femaleShirtLabel.gameObject.SetActive(false);
                femalePantsDD.gameObject.SetActive(false);
                femalePantsLabel.gameObject.SetActive(false);
                makeupDD.gameObject.SetActive(false);
                makeupLabel.gameObject.SetActive(false);

                shirtColor2DD.gameObject.SetActive(false);
                shirtColor2Label.gameObject.SetActive(false);
            }

            else
            {
                beardDD.gameObject.SetActive(false);
                beardLabel.gameObject.SetActive(false);
                maleShirtDD.gameObject.SetActive(false);
                maleShirtLabel.gameObject.SetActive(false);
                malePantsDD.gameObject.SetActive(false);
                malePantsLabel.gameObject.SetActive(false);
                shirtColor2DD.gameObject.SetActive(false);
                shirtColor2Label.gameObject.SetActive(false);

                femaleShirtsDD.gameObject.SetActive(true);
                femaleShirtLabel.gameObject.SetActive(true);
                femalePantsDD.gameObject.SetActive(true);
                femalePantsLabel.gameObject.SetActive(true);
                makeupDD.gameObject.SetActive(true);
                makeupLabel.gameObject.SetActive(true);

                shirtColor2DD.gameObject.SetActive(false);
                shirtColor2Label.gameObject.SetActive(false);
            }

            checkIntExt();
        }

        public void randTest()
        {
            sexDD.value = Random.Range(0, sexDD.options.Count);

            sexChanged();

            skinDD.value = Random.Range(0, skinDD.options.Count);
            hairDD.value = Random.Range(0, hairDD.options.Count);
            hairColorDD.value = Random.Range(0, hairColorDD.options.Count);
            eyebrowDD.value = Random.Range(0, eyebrowDD.options.Count);

            var glassesValue = Random.Range(0, glassesDD.options.Count + 7);

            if (glassesValue > 3)
            {
                glassesValue = 0;
            }

            glassesDD.value = glassesValue;

            beltDD.value = Random.Range(0, beltDD.options.Count);

            eyeColorDD.value = Random.Range(0, eyeColorDD.options.Count);

            if (skinDD.value == 0)
            {
                beardDD.value = Random.Range(0, beardDD.options.Count);
                maleShirtDD.value = Random.Range(0, maleShirtDD.options.Count);
                malePantsDD.value = Random.Range(0, malePantsDD.options.Count);

                if (maleShirtDD.value == 4)
                {
                    shirtColor2DD.value = Random.Range(0, shirtColor2DD.options.Count);
                }
            }
            else
            {
                makeupDD.value = Random.Range(0, makeupDD.options.Count);
                femaleShirtsDD.value = Random.Range(0, femaleShirtsDD.options.Count);
                femalePantsDD.value = Random.Range(0, femalePantsDD.options.Count);

                if (femaleShirtsDD.value == 4)
                {
                    shirtColor2DD.value = Random.Range(0, shirtColor2DD.options.Count);
                }
            }

            

            shirtColorDD.value = Random.Range(0, shirtColorDD.options.Count);
            pantColorDD.value = Random.Range(0, pantColorDD.options.Count);

            motionDD.value = Random.Range(0, motionDD.options.Count);

            checkIntExt();

            GenNPC();
        }

        public void GenFemale(Texture skinToUse, Mesh hairToUse, Vector3 scaleToUse, Material hairColorToUse, Mesh eyebrowsToUse, Mesh glassesToUse, Texture eyeColorToUse,
            Mesh shirtToUse, Mesh pantsToUse, Material shirtColorToUse, Texture pantColorToUse, Mesh makeupToUse, Material shirtColorToUse2)
        {
            GameObject newNPC = Instantiate(femaleNPC, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            Transform root = newNPC.transform.Find("Squ_People3:root");
            Transform rootPelvis = root.transform.Find("Squ_People3:pelvis");
            Transform rootSpine = rootPelvis.transform.Find("Squ_People3:spine_01");
            Transform rootSpine2 = rootSpine.transform.Find("Squ_People3:spine_02");
            Transform rootSpine3 = rootSpine2.transform.Find("Squ_People3:spine_03");
            Transform rootNeck = rootSpine3.transform.Find("Squ_People3:neck_01");
            Transform rootHead = rootNeck.transform.Find("Squ_People3:head");

            //var beltObj = 

            Transform body = newNPC.transform.Find("female_01_body");
            SkinnedMeshRenderer body_smr = body.GetComponent<SkinnedMeshRenderer>();
            body_smr.material.mainTexture = skinToUse;

            Transform head = newNPC.transform.Find("head_01");
            SkinnedMeshRenderer head_smr = head.GetComponent<SkinnedMeshRenderer>();
            head_smr.material.mainTexture = skinToUse;

            Transform shirt = null;
            Transform shirt2Trans = null;

            switch (shirtToUseVal)
            {
                case 0:
                    shirt = newNPC.transform.Find("female_01_shirt1_long");
                    break;
                case 1:
                    shirt = newNPC.transform.Find("female_01_shirt2_tee");
                    break;
                case 2:
                    shirt = newNPC.transform.Find("female_01_shirt3_muscle");
                    break;
                case 3:
                    shirt = newNPC.transform.Find("female_01_shirt4_suit");
                    break;
                case 4:
                    shirt = newNPC.transform.Find("female_01_shirt5_twoLayer_Exterior");
                    shirt2Trans = newNPC.transform.Find("female_01_shirt5_twoLayer_Interior");
                    break;
                case 6:
                    shirt = newNPC.transform.Find("female_01_dress1");
                    break;
                case 7:
                    shirt = newNPC.transform.Find("female_01_dress2");
                    break;
                case 8:
                    shirt = newNPC.transform.Find("female_01_dress3");
                    break;
                case 9:
                    shirt = newNPC.transform.Find("female_01_swimsuit");
                    break;
                default:
                    break;
            }

            //Transform shirt = newNPC.transform.Find("male_01_shirt1_long");
            SkinnedMeshRenderer shirt_smr = shirt.GetComponent<SkinnedMeshRenderer>();

            GameObject newShirt = new GameObject();
            newShirt.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newShirt_smr = newShirt.AddComponent<SkinnedMeshRenderer>();
            newShirt_smr.sharedMesh = shirtToUse;
            newShirt_smr.rootBone = root;
            newShirt_smr.bones = shirt_smr.bones;
            newShirt_smr.material = shirtColorToUse;

            if (intExtShirt)
            {
                SkinnedMeshRenderer shirt2_smr = shirt2Trans.GetComponent<SkinnedMeshRenderer>();

                GameObject newShirt2 = new GameObject();
                newShirt2.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newShirt2_smr = newShirt2.AddComponent<SkinnedMeshRenderer>();
                newShirt2_smr.sharedMesh = femaleShirt5int;
                newShirt2_smr.rootBone = root;
                newShirt2_smr.bones = shirt2_smr.bones;
                newShirt2_smr.material = shirtColorToUse2;
            }

            var test = femaleShirtsDD.value;

            if (test != 5 && test != 6 && test != 7 && test != 8)
            {
                Transform pants = null;

                switch (pantsToUseVal)
                {
                    case 0:
                        pants = newNPC.transform.Find("female_01_pants1_long");
                        break;
                    case 1:
                        pants = newNPC.transform.Find("female_01_pants2_shorts");
                        break;
                    case 2:
                        pants = newNPC.transform.Find("female_01_pants3_longSkirt");
                        break;
                    case 3:
                        pants = newNPC.transform.Find("female_01_pants3_skirt");
                        break;
                    default:
                        break;
                }

                SkinnedMeshRenderer pants_smr = pants.GetComponent<SkinnedMeshRenderer>();

                GameObject newPants = new GameObject();
                newPants.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newPants_smr = newPants.AddComponent<SkinnedMeshRenderer>();
                newPants_smr.sharedMesh = pantsToUse;
                newPants_smr.rootBone = root;
                newPants_smr.bones = pants_smr.bones;
                newPants_smr.material.mainTexture = pantColorToUse;

                if (hasBelt)
                {
                    Transform belt = newNPC.transform.Find("female_01_belt");
                    SkinnedMeshRenderer belt_smr = belt.GetComponent<SkinnedMeshRenderer>();

                    GameObject newBelt = new GameObject();
                    newBelt.transform.SetParent(newNPC.transform);

                    SkinnedMeshRenderer newBelt_smr = newBelt.AddComponent<SkinnedMeshRenderer>();
                    newBelt_smr.sharedMesh = femaleBeltMesh;
                    newBelt_smr.rootBone = root;
                    newBelt_smr.bones = belt_smr.bones;
                    newBelt_smr.material.mainTexture = pantColorToUse;
                }
            }




            Transform eyes = rootHead.transform.Find("Squ_People3:eyesAndTeeth");
            MeshRenderer eyes_mr = eyes.GetComponent<MeshRenderer>();
            eyes_mr.material.mainTexture = eyeColorToUse;


            GameObject hair = new GameObject();
            hair.transform.SetParent(rootHead.transform);
            MeshRenderer hair_mr = hair.AddComponent<MeshRenderer>();
            MeshFilter hair_mf = hair.AddComponent<MeshFilter>();
            hair_mf.mesh = hairToUse;
            hair_mr.material = hairColorToUse;
            hair.transform.localScale = scaleToUse;
            hair.transform.localPosition = newNPC.transform.position;
            hair.transform.localRotation = newNPC.transform.rotation;
            Vector3 rot = hair.transform.localRotation.eulerAngles;
            rot = new Vector3(rot.x + 1.321f, rot.y - 90, rot.z);
            hair.transform.localRotation = Quaternion.Euler(rot);


            GameObject eyebrows = new GameObject();
            eyebrows.transform.SetParent(rootHead.transform);
            MeshRenderer eyebrows_mr = eyebrows.AddComponent<MeshRenderer>();
            MeshFilter eyebrows_mf = eyebrows.AddComponent<MeshFilter>();
            eyebrows_mf.mesh = eyebrowsToUse;
            eyebrows_mr.material = hairColorToUse;
            eyebrows.transform.localPosition = newNPC.transform.position;
            eyebrows.transform.localRotation = newNPC.transform.rotation;
            Vector3 rot2 = eyebrows.transform.localRotation.eulerAngles;
            rot2 = new Vector3(rot2.x + 1.321f, rot2.y - 90, rot2.z);
            eyebrows.transform.localRotation = Quaternion.Euler(rot2);

            if (hasGlasses)
            {
                GameObject glasses = new GameObject();
                glasses.transform.SetParent(rootHead.transform);
                MeshRenderer glasses_mr = glasses.AddComponent<MeshRenderer>();
                MeshFilter glasses_mf = glasses.AddComponent<MeshFilter>();
                glasses_mf.mesh = glassesToUse;
                glasses_mr.material = hair1;
                glasses.transform.localScale = glassesScale;
                glasses.transform.localPosition = newNPC.transform.position;
                glasses.transform.localRotation = newNPC.transform.rotation;
                Vector3 rot3 = glasses.transform.localRotation.eulerAngles;
                rot3 = new Vector3(rot2.x + 1.321f, rot2.y - 90, rot2.z);
                glasses.transform.localRotation = Quaternion.Euler(rot2);
            }

            if (hasMakeup)
            {
                GameObject makeup = new GameObject();
                makeup.transform.SetParent(rootHead.transform);
                MeshRenderer makeup_mr = makeup.AddComponent<MeshRenderer>();
                MeshFilter makeup_mf = makeup.AddComponent<MeshFilter>();
                makeup_mf.mesh = makeupToUse;
                makeup_mr.material = hair1;
                makeup.transform.localScale = glassesScale;
                makeup.transform.localPosition = newNPC.transform.position;
                makeup.transform.localRotation = newNPC.transform.rotation;
                Vector3 rot3 = makeup.transform.localRotation.eulerAngles;
                rot3 = new Vector3(rot2.x + 1.321f, rot2.y - 90, rot2.z);
                makeup.transform.localRotation = Quaternion.Euler(rot2);
            }

            newNPC.transform.position = new Vector3(spawnLoc.x - xNum, spawnLoc.y, spawnLoc.z - zNum);

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

            var anim = newNPC.GetComponent<Animator>();

            switch (motionToUseVal)
            {
                case 0:
                    anim.Play("NPC_IDLE", -1, 0);
                    break;
                case 1:
                    anim.Play("NPC_WALK", -1, 0);
                    break;
                case 2:
                    anim.Play("NPC_RUN", -1, 0);
                    break;
                default:
                    break;
            }



            numNPCS += 4;


            numPeeps.text = "NPCS - " + ((numNPCS / 4)).ToString();

            if (xNum < 96)
            {
                xNum += 4;
            }

            else
            {
                xNum = 0;
                zNum += 10;
            }
        }

        public void GenMale(Texture skinToUse, Mesh hairToUse, Vector3 scaleToUse, Material hairColorToUse, Mesh eyebrowsToUse, Mesh glassesToUse, Mesh beardToUse, Texture eyeColorToUse,
            Mesh shirtToUse, Mesh pantsToUse, Material shirtColorToUse, Texture pantColorToUse, Material shirtColorToUse2)
        {
            GameObject newNPC = Instantiate(maleNPC, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            Transform root = newNPC.transform.Find("Squ_People3:root");
            Transform rootPelvis = root.transform.Find("Squ_People3:pelvis");
            Transform rootSpine = rootPelvis.transform.Find("Squ_People3:spine_01");
            Transform rootSpine2 = rootSpine.transform.Find("Squ_People3:spine_02");
            Transform rootSpine3 = rootSpine2.transform.Find("Squ_People3:spine_03");
            Transform rootNeck = rootSpine3.transform.Find("Squ_People3:neck_01");
            Transform rootHead = rootNeck.transform.Find("Squ_People3:head");

            Transform body = newNPC.transform.Find("male_01_body");
            SkinnedMeshRenderer body_smr = body.GetComponent<SkinnedMeshRenderer>();
            body_smr.material.mainTexture = skinToUse;

            Transform head = newNPC.transform.Find("head_01");
            SkinnedMeshRenderer head_smr = head.GetComponent<SkinnedMeshRenderer>();
            head_smr.material.mainTexture = skinToUse;

            if (hasBelt)
            {
                Transform belt = newNPC.transform.Find("male_01_belt");
                SkinnedMeshRenderer belt_smr = belt.GetComponent<SkinnedMeshRenderer>();

                GameObject newBelt = new GameObject();
                newBelt.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newBelt_smr = newBelt.AddComponent<SkinnedMeshRenderer>();
                newBelt_smr.sharedMesh = maleBeltMesh;
                newBelt_smr.rootBone = root;
                newBelt_smr.bones = belt_smr.bones;
                newBelt_smr.material.mainTexture = pantColorToUse;
            }

            Transform shirt = null;
            Transform shirt2Trans = null;

            switch (shirtToUseVal)
            {
                case 0:
                    shirt = newNPC.transform.Find("male_01_shirt1_long");
                    break;
                case 1:
                    shirt = newNPC.transform.Find("male_01_shirt2_tee");
                    break;
                case 2:
                    shirt = newNPC.transform.Find("male_01_shirt3_muscle");
                    break;
                case 3:
                    shirt = newNPC.transform.Find("male_01_shirt4_suit");
                    break;
                case 4:
                    shirt = newNPC.transform.Find("male_01_shirt5_twoLayer_Exterior");
                    shirt2Trans = newNPC.transform.Find("male_01_shirt5_twoLayer_Interior");
                    break;
                default:
                    break;
            }

            //Transform shirt = newNPC.transform.Find("male_01_shirt1_long");
            SkinnedMeshRenderer shirt_smr = shirt.GetComponent<SkinnedMeshRenderer>();

            GameObject newShirt = new GameObject();
            newShirt.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newShirt_smr = newShirt.AddComponent<SkinnedMeshRenderer>();
            newShirt_smr.sharedMesh = shirtToUse;
            newShirt_smr.rootBone = root;
            newShirt_smr.bones = shirt_smr.bones;
            newShirt_smr.material = shirtColorToUse;

            if (intExtShirt)
            {
                SkinnedMeshRenderer shirt2_smr = shirt2Trans.GetComponent<SkinnedMeshRenderer>();

                GameObject newShirt2 = new GameObject();
                newShirt2.transform.SetParent(newNPC.transform);

                SkinnedMeshRenderer newShirt2_smr = newShirt2.AddComponent<SkinnedMeshRenderer>();
                newShirt2_smr.sharedMesh = maleShirt5int;
                newShirt2_smr.rootBone = root;
                newShirt2_smr.bones = shirt2_smr.bones;
                newShirt2_smr.material = shirtColorToUse2;
            }

            Transform pants = null;

            switch (pantsToUseVal)
            {
                case 0:
                    pants = newNPC.transform.Find("male_01_pants1_long");
                    break;
                case 1:
                    pants = newNPC.transform.Find("male_01_pants2_shorts");
                    break;
                case 2:
                    pants = newNPC.transform.Find("male_01_pants3_kilt");
                    break;
                default:
                    break;
            }

            SkinnedMeshRenderer pants_smr = pants.GetComponent<SkinnedMeshRenderer>();

            GameObject newPants = new GameObject();
            newPants.transform.SetParent(newNPC.transform);

            SkinnedMeshRenderer newPants_smr = newPants.AddComponent<SkinnedMeshRenderer>();
            newPants_smr.sharedMesh = pantsToUse;
            newPants_smr.rootBone = root;
            newPants_smr.bones = pants_smr.bones;
            newPants_smr.material.mainTexture = pantColorToUse;



            Transform eyes = rootHead.transform.Find("eyesAndTeeth");
            MeshRenderer eyes_mr = eyes.GetComponent<MeshRenderer>();
            eyes_mr.material.mainTexture = eyeColorToUse;


            GameObject hair = new GameObject();
            hair.transform.SetParent(rootHead.transform);
            MeshRenderer hair_mr = hair.AddComponent<MeshRenderer>();
            MeshFilter hair_mf = hair.AddComponent<MeshFilter>();
            hair_mf.mesh = hairToUse;
            hair_mr.material = hairColorToUse;
            hair.transform.localScale = scaleToUse;
            hair.transform.localPosition = newNPC.transform.position;
            hair.transform.localRotation = newNPC.transform.rotation;
            Vector3 rot = hair.transform.localRotation.eulerAngles;
            rot = new Vector3(rot.x + 1.321f, rot.y - 90, rot.z);
            hair.transform.localRotation = Quaternion.Euler(rot);


            GameObject eyebrows = new GameObject();
            eyebrows.transform.SetParent(rootHead.transform);
            MeshRenderer eyebrows_mr = eyebrows.AddComponent<MeshRenderer>();
            MeshFilter eyebrows_mf = eyebrows.AddComponent<MeshFilter>();
            eyebrows_mf.mesh = eyebrowsToUse;
            eyebrows_mr.material = hairColorToUse;
            eyebrows.transform.localPosition = newNPC.transform.position;
            eyebrows.transform.localRotation = newNPC.transform.rotation;
            Vector3 rot2 = eyebrows.transform.localRotation.eulerAngles;
            rot2 = new Vector3(rot2.x + 1.321f, rot2.y - 90, rot2.z);
            eyebrows.transform.localRotation = Quaternion.Euler(rot2);

            if (hasGlasses)
            {
                GameObject glasses = new GameObject();
                glasses.transform.SetParent(rootHead.transform);
                MeshRenderer glasses_mr = glasses.AddComponent<MeshRenderer>();
                MeshFilter glasses_mf = glasses.AddComponent<MeshFilter>();
                glasses_mf.mesh = glassesToUse;
                glasses_mr.material = hair1;
                glasses.transform.localScale = glassesScale;
                glasses.transform.localPosition = newNPC.transform.position;
                glasses.transform.localRotation = newNPC.transform.rotation;
                Vector3 rot3 = glasses.transform.localRotation.eulerAngles;
                rot3 = new Vector3(rot2.x + 1.321f, rot2.y - 90, rot2.z);
                glasses.transform.localRotation = Quaternion.Euler(rot2);
            }

            if (hasBeard)
            {
                GameObject beard = new GameObject();
                beard.transform.SetParent(rootHead.transform);
                MeshRenderer beard_mr = beard.AddComponent<MeshRenderer>();
                MeshFilter beard_mf = beard.AddComponent<MeshFilter>();
                beard_mf.mesh = beardToUse;
                beard_mr.material = hairColorToUse;
                beard.transform.localPosition = newNPC.transform.position;
                beard.transform.localRotation = newNPC.transform.rotation;
                Vector3 rot3 = beard.transform.localRotation.eulerAngles;
                rot3 = new Vector3(rot2.x + 1.321f, rot2.y - 90, rot2.z);
                beard.transform.localRotation = Quaternion.Euler(rot2);
            }

            newNPC.transform.position = new Vector3(spawnLoc.x - xNum, spawnLoc.y, spawnLoc.z - zNum);

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

            var anim = newNPC.GetComponent<Animator>();

            switch (motionToUseVal)
            {
                case 0:
                    anim.Play("NPC_IDLE", -1, 0);
                    break;
                case 1:
                    anim.Play("NPC_WALK", -1, 0);
                    break;
                case 2:
                    anim.Play("NPC_RUN", -1, 0);
                    break;
                default:
                    break;
            }

            

            numNPCS += 4;


            numPeeps.text = "NPCS - " + ((numNPCS / 4)).ToString();

            if (xNum < 96)
            {
                xNum += 4;
            }
            
            else
            {
                xNum = 0;
                zNum += 10;
            }
        }
    }
}
