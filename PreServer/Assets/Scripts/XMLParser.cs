using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System;

namespace PreServer
{
    public class XMLParser : MonoBehaviour
    {
        private string fileName;
        private XmlDocument xmlDoc;
        private XmlDocument usernameDoc;

        private List<NPCAction> npcActions;
        private List<NPCUsername> npcUsernames;

        private TextAsset xmlFile;
        private TextAsset usernamesFile;

        void Awake()
        {
            //fileName = "Assets/Resources/npcActions.xml";
            npcActions = new List<NPCAction>();
            npcUsernames = new List<NPCUsername>();

            xmlFile = (TextAsset) Resources.Load("npcActions");
            usernamesFile = (TextAsset)Resources.Load("npcUsernames");
        }

        public void UpdateXML()
        {
            var test = xmlDoc.SelectSingleNode("//action[@id='0']/@gen");
            test.Value = "1";
            xmlDoc.Save("Assets/Resources/npcActions.xml");
        }

        public List<NPCUsername> ParseUsernames()
        {
            LoadUsernameXML();
            ReadUsernameXML();
            return npcUsernames;
        }

        /*public List<string> ParseTxt()
        {
            var test = usernamesFile.text;
            string[] stringSeparators = new string[] { "\r\n" };
            var test2 = test.Split(stringSeparators, StringSplitOptions.None);

            npcUsernames = new List<string>(test2);

            return npcUsernames;
        }*/

        public List<NPCAction> ParseXML()
        {
            LoadXML();
            ReadXML();
            return npcActions;
        }

        private void LoadUsernameXML()
        {
            usernameDoc = new XmlDocument();
            usernameDoc.LoadXml(usernamesFile.text);
        }

        private void LoadXML()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);
        }

        private void ReadUsernameXML()
        {
            foreach (XmlElement node in usernameDoc.SelectNodes("usernames/un"))
            {
                NPCUsername npcu = new NPCUsername();
                npcu.username = node.GetAttribute("name");
                npcu.genStatus = int.Parse(node.GetAttribute("gen"));

                npcUsernames.Add(npcu);
            }
        }

        private void ReadXML()
        {
            foreach (XmlElement node in xmlDoc.SelectNodes("actions/action"))
            {
                NPCAction newAction = new NPCAction(int.Parse(node.GetAttribute("id")));
                List<NPCStep> steps = new List<NPCStep>();
                Dictionary<int, string> reqs = new Dictionary<int, string>();

                foreach (XmlElement reqsNode in node.SelectNodes("reqs/req"))
                {
                    reqs.Add(int.Parse(reqsNode.GetAttribute("id")), reqsNode.GetAttribute("value"));
                }

                foreach (XmlElement stepsNode in node.SelectNodes("steps/step"))
                {
                    switch(stepsNode.GetAttribute("type"))
                    {
                        case "move":
                            MoveStep moveStep = new MoveStep();
                            moveStep.destination = new Vector3(int.Parse(stepsNode.GetAttribute("x")), int.Parse(stepsNode.GetAttribute("y")), int.Parse(stepsNode.GetAttribute("z")));
                            steps.Add(moveStep);
                            break;
                        case "wait":
                            WaitStep waitStep = new WaitStep();
                            waitStep.seconds = int.Parse(stepsNode.GetAttribute("time"));
                            steps.Add(waitStep);
                            break;
                        case "msg":
                            MsgStep msgStep = new MsgStep();
                            msgStep.msg = stepsNode.GetAttribute("words");
                            steps.Add(msgStep);
                            break;
                        default:
                            break;
                    }
                }

                newAction.steps = steps;
                newAction.reqs = reqs;
                npcActions.Add(newAction);
            }
        }
    }
}