using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace PreServer
{
    public class XMLParser : MonoBehaviour
    {
        private XmlDocument actionsDoc;
        private XmlDocument usernameDoc;

        private List<NPCAction> npcActions;
        private List<NPCUsername> npcUsernames;

        private TextAsset actionsTextAsset;
        private TextAsset usernamesTextAsset;

        private bool resetXML = true;

        void Awake()
        {
            npcActions = new List<NPCAction>();
            npcUsernames = new List<NPCUsername>();

            actionsTextAsset = (TextAsset)Resources.Load("npcActionsTestWrite");
            usernamesTextAsset = (TextAsset)Resources.Load("npcUsernames");

            if (!System.IO.File.Exists(Application.persistentDataPath + "/npcActions.xml") || resetXML)
            {
                string xmlFileString = actionsTextAsset.text;
                System.IO.File.WriteAllText(Application.persistentDataPath + "/npcActions.xml", xmlFileString);
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/npcUsernames.xml") || resetXML)
            {
                string usernamesFileString = usernamesTextAsset.text;
                System.IO.File.WriteAllText(Application.persistentDataPath + "/npcUsernames.xml", usernamesFileString);
            }

        }

        public void UpdateAction(int actionId)
        {
            XmlNode actionNode = actionsDoc.SelectSingleNode("//action[@id='" + actionId + "']/@gen");
            actionNode.Value = "1";
            actionsDoc.Save(Application.persistentDataPath + "/npcActions.xml");
        }

        public void UpdateUsername(string userName)
        {
            XmlNode usernameNode = usernameDoc.SelectSingleNode("//un[@name='" + userName + "']/@gen");
            usernameNode.Value = "1";
            usernameDoc.Save(Application.persistentDataPath + "/npcUsernames.xml");
        }

        public List<NPCUsername> ParseUsernames()
        {
            LoadUsernames();
            ReadUsernames();
            return npcUsernames;
        }

        public List<NPCAction> ParseActions()
        {
            LoadActions();
            ReadActions();
            return npcActions;
        }

        private void LoadUsernames()
        {
            usernameDoc = new XmlDocument();
            usernameDoc.Load(Application.persistentDataPath + "/npcUsernames.xml");
        }

        private void LoadActions()
        {
            actionsDoc = new XmlDocument();
            actionsDoc.Load(Application.persistentDataPath + "/npcActions.xml");
        }

        private void ReadUsernames()
        {
            foreach (XmlElement node in usernameDoc.SelectNodes("usernames/un"))
            {
                NPCUsername npcu = new NPCUsername();
                npcu.username = node.GetAttribute("name");
                npcu.genStatus = int.Parse(node.GetAttribute("gen"));

                npcUsernames.Add(npcu);
            }
        }

        private void ReadActions()
        {
            foreach (XmlElement node in actionsDoc.SelectNodes("actions/action"))
            {
                NPCAction newAction = new NPCAction(int.Parse(node.GetAttribute("id")));
                List<NPCStep> steps = new List<NPCStep>();
                Dictionary<int, string> reqs = new Dictionary<int, string>();
                List<int> pals = new List<int>();

                foreach (XmlElement reqsNode in node.SelectNodes("reqs/req"))
                {
                    reqs.Add(int.Parse(reqsNode.GetAttribute("id")), reqsNode.GetAttribute("value"));
                }

                foreach (XmlElement palsNode in node.SelectNodes("pals/pal"))
                {
                    pals.Add(int.Parse(palsNode.GetAttribute("id")));
                }

                foreach (XmlElement stepsNode in node.SelectNodes("steps/step"))
                {
                    switch (stepsNode.GetAttribute("type"))
                    {
                        case "move":
                            MoveStep moveStep = new MoveStep();
                            moveStep.pointName = stepsNode.GetAttribute("point");
                            steps.Add(moveStep);
                            break;
                        case "wait":
                            WaitStep waitStep = new WaitStep();
                            waitStep.seconds = float.Parse(stepsNode.GetAttribute("time"));
                            steps.Add(waitStep);
                            break;
                        case "msg":
                            MsgStep msgStep = new MsgStep();
                            msgStep.msg = stepsNode.GetAttribute("words");
                            string waitForString = stepsNode.GetAttribute("waitFor");

                            List<int> palsList = new List<int>();

                            if (!waitForString.Equals(""))
                            {
                                palsList = new List<int>(Array.ConvertAll(waitForString.Split(','), int.Parse));
                            }

                            msgStep.waitForPals = palsList;

                            //msgStep.waitForPals = (stepsNode.GetAttribute("waitForPals") == "t" ? true : false);

                            steps.Add(msgStep);
                            break;
                        case "use":
                            UseStep useStep = new UseStep();
                            useStep.objToUse = stepsNode.GetAttribute("obj");
                            steps.Add(useStep);
                            break;
                        default:
                            break;
                    }
                }

                newAction.steps = steps;
                newAction.reqs = reqs;
                newAction.pals = pals;
                npcActions.Add(newAction);
            }
        }
    }
}