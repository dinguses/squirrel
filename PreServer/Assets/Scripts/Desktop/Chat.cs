using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using System.Linq;
using System;

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

        public GameObject C1_Container;
        public GameObject C2_Container;
        public GameObject C3_Container;

        public Choice Choice1 = new Choice();
        public Choice Choice2 = new Choice();
        public Choice Choice3 = new Choice();

        public Text Choice1Text;
        public Text Choice2Text;
        public Text Choice3Text;

        public List<Choice> Choices;

        public string lastChoice;
        public bool postChoiceJump = false;
        public bool fastToggle = false;

        public int lineCounter;
        public ScrollRect testRect;
        public RectTransform textField;
        public int numPeopleTyping = 0;
        public List<string> usersTyping;

        void Awake()
        {
            usersTyping = new List<string>();

            Choice1.ChoiceText = Choice1Text;
            Choice2.ChoiceText = Choice2Text;
            Choice3.ChoiceText = Choice3Text;

            Choices = new List<Choice>() { Choice1, Choice2, Choice3 };

            MainChatText.text += "<color=grey>Ratatusk has entered the room.</color>";
            lineCounter = 1;

            chatAsset = (TextAsset)Resources.Load("testConvo3");
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
                newStep.length = float.Parse(node.GetAttribute("time"));
                newStep.words = node.GetAttribute("words");
                bool.TryParse(node.GetAttribute("startNext"), out bool isStartNext);
                newStep.startNext = isStartNext ? bool.Parse(node.GetAttribute("startNext")) : false;
                if (isStartNext)
                    newStep.startNextTime = float.Parse(node.GetAttribute("startNextTime"));

                bool.TryParse(node.GetAttribute("cancel"), out bool isCancel);
                newStep.cancel = isCancel ? bool.Parse(node.GetAttribute("cancel")) : false;

                chatSteps.Add(newStep);
            }
        }

        public void Update()
        {
            switch(numPeopleTyping)
            {
                case 0:
                    IsTypingText.text = "";
                    break;
                case 1:
                    IsTypingText.text = usersTyping[0] + " is typing...";
                    break;
                case 2:
                    IsTypingText.text = usersTyping[0] + " and " + usersTyping[1] + " are typing...";
                    break;
                case 3:
                    IsTypingText.text = usersTyping[0] + ", " + usersTyping[1] + ", and " + usersTyping[2] + " are typing...";
                    break;
                default:
                    break;
            }
        }

        public void NextStep()
        {
            ChatStep step = chatSteps[currStep];

            if (!postChoiceJump || step.type == 5)
            {
                switch (step.type)
                {
                    // 0 - Other player joining/leaving chat
                    case 0:
                        bool leaving = Convert.ToBoolean(step.length);
                        MainChatText.text += ("\n<color=grey>" + step.name + (leaving ? " has entered the room." : " has left the room.") + "</color>");
                        lineCounter++;
                        StartCoroutine(Wait(0));
                        break;

                    // 1 - Sending a message
                    case 1:
                        numPeopleTyping++;
                        usersTyping.Add(step.name);
                        StartCoroutine(UserIsTyping(step.length, step.startNext, step.name, step.startNextTime, step.words, step.cancel));
                        break;

                    // 2 - Pause
                    case 2:
                        StartCoroutine(Wait(step.length));
                        break;

                    // 3 - Player dialogue option
                    case 3:
                        if (step.length == 1)
                        {
                            Choice1.words = step.words;
                            Choice1.choiceName = step.name;
                            Choice1.ChoiceText.text = Choice1.words;
                            C1_Container.SetActive(true);
                        }

                        if (step.length == 2)
                        {
                            Choice2.words = step.words;
                            Choice2.choiceName = step.name;
                            Choice2.ChoiceText.text = Choice2.words;
                            C2_Container.SetActive(true);
                        }

                        if (step.length == 3)
                        {
                            Choice3.words = step.words;
                            Choice3.choiceName = step.name;
                            Choice3.ChoiceText.text = Choice3.words;
                            C3_Container.SetActive(true);
                        }
                        StartCoroutine(Wait(0));
                        break;

                    // 4 - End of player choices
                    case 4:
                        // ToDo: more specific logic for 1/2 option choices
                        break;

                    // 5  - Designates start of branch
                    case 5:
                        if (step.name == lastChoice)
                        {
                            postChoiceJump = false;
                        }
                        StartCoroutine(Wait(0));
                        break;

                    // 6 - Jump to specific id
                    case 6:
                        postChoiceJump = true;
                        lastChoice = step.name;
                        StartCoroutine(Wait(0));
                        break;

                    default:
                        break;
                }

                if ((step.type == 1 || step.type == 0) && lineCounter >= 16)
                {
                    var test = textField.sizeDelta;
                    textField.sizeDelta = new Vector2(test.x, test.y + 20);
                    testRect.velocity = new Vector2(0, 35f);
                }
            }
            else
            {
                StartCoroutine(Wait(0));
            }
        }

        IEnumerator SubStep(float numSecs)
        {
            yield return new WaitForSeconds(numSecs);
            NextStep();
        }

        IEnumerator UserIsTyping(float numSecs, bool startNextStep, string userName, float startNextTime, string words, bool cancel)
        {
            if (fastToggle)
                numSecs /= 10;

            currStep++;

            if (startNextStep)
            {
                StartCoroutine(SubStep(startNextTime));
            }

            yield return new WaitForSeconds(numSecs);
            numPeopleTyping--;
            usersTyping.Remove(userName);

            if (!cancel)
            {
                string color = "black";
                switch (userName)
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
                MainChatText.text += "\n" + "<color=" + color + ">" + userName + "</color>: " + words;
                lineCounter++;
            }

            if (!startNextStep)
            {
                NextStep();
            }
        }

        IEnumerator Wait(float numSecs)
        {
            if (fastToggle)
                numSecs /= 10;

            yield return new WaitForSeconds(numSecs);
            currStep++;
            NextStep();
        }

        public void ChoiceClicked(GameObject whoClicked)
        {
            string color = "aqua";
            Choice c = Choices.Where(w => w.ChoiceText.name == whoClicked.name).FirstOrDefault();
            Text choiceMade = c.ChoiceText;
            MainChatText.text += "\n" + "<color=" + color + ">Ratatusk</color>: " + choiceMade.text;

            lineCounter++;

            if (lineCounter >= 16)
            {
                var test = textField.sizeDelta;
                textField.sizeDelta = new Vector2(test.x, test.y + 20);
                testRect.velocity = new Vector2(0, 1000f);
            }

            if (c.choiceName != "")
                postChoiceJump = true;
            lastChoice = c.choiceName;

            C1_Container.SetActive(false);
            C2_Container.SetActive(false);
            C3_Container.SetActive(false);

            foreach (Choice cc in Choices)
            {
                cc.ChoiceText.text = "";
            }

            StartCoroutine(Wait(0));
        }

        public void ToggleFast()
        {
            fastToggle = !fastToggle;
        }
    }

    public class ChatStep
    {
        public int type;
        public string name;
        public float length;
        public string words;
        public bool startNext;
        public float startNextTime;
        public bool cancel;
    }

    public class Choice
    {
        public string words;
        public string choiceName;
        public Text ChoiceText;
    }
}