using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

namespace PreServer
{
    public class Chat : MonoBehaviour
    {
        public XmlDocument testConvo = new XmlDocument();
        public TextAsset chatAsset;
        public List<ChatStep> chatSteps;
        public int currStep = 0;

        public Text MainChatText;
        public Text IsTypingText;

        void Awake()
        {
            MainChatText.text += "Ratatusk has entered the room.";

            chatAsset = (TextAsset)Resources.Load("testConvo");
            testConvo.LoadXml(chatAsset.text);
            chatSteps = new List<ChatStep>();

            ParseChatXml();

            NextStep();
        }

        private void ParseChatXml()
        {
            foreach (XmlElement node in testConvo.SelectNodes("convo/chat"))
            {
                ChatStep newStep = new ChatStep();
                newStep.type = int.Parse(node.GetAttribute("type"));
                newStep.name = node.GetAttribute("from");
                newStep.length = int.Parse(node.GetAttribute("time"));
                newStep.words = node.GetAttribute("words");

                chatSteps.Add(newStep);
            }
        }
        public void NextStep()
        {
            ChatStep step = chatSteps[currStep];

            switch (step.type)
            {
                case 0:
                    IsTypingText.text = step.name + " is typing...";
                    StartCoroutine(Wait(step.length));
                    break;
                case 1:

                    string color = "black";

                    switch (step.name)
                    {
                        case "wank_w0rm":
                            color = "green";
                            break;
                        case "Bertie":
                            color = "blue";
                            break;
                        case "echelon":
                            color = "red";
                            break;
                        default:
                            color = "black";
                            break;
                    }


                    MainChatText.text += "\n" + "<color=" + color + ">"+ step.name + "</color>: " + step.words;
                    StartCoroutine(Wait(step.length));
                    break;
                case 2:
                    StartCoroutine(Wait(step.length));
                    break;
                default:
                    break;
            }
        }

        IEnumerator Wait(float numSecs)
        {
            yield return new WaitForSeconds(numSecs);
            IsTypingText.text = "";
            currStep++;
            NextStep();
        }
    }

    public class ChatStep
    {
        public int type;
        public string name;
        public int length;
        public string words;
    }
}