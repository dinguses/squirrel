using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace PreServer
{
    public class NPCControlManager : MonoBehaviour
    {
        public Vector3 moveTowards = new Vector3(0, 0, 0);

        private XmlDocument actionsDoc;

        private List<NPCAction> npcActions;
        public XMLParser xmlParser;

        public NPCAction newAction;

        public void Start()
        {
            xmlParser = GetComponent<XMLParser>();

            npcActions = xmlParser.ParseActions();

            newAction = new NPCAction(npcActions.Count);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Clicked();
            }

            transform.position = Vector3.MoveTowards(transform.position, moveTowards, Time.deltaTime * 30f);
        }

        void Clicked()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                moveTowards = hit.point;
            }

            GameObject newNPC = Instantiate(Resources.Load<GameObject>("NPC/test_001"), hit.point, new Quaternion(0, 0, 0, 0));

            MoveStep ms = new MoveStep();
            ms.pointName = newNPC.name;
        }
    }
}