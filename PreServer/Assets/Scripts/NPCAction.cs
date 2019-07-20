using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PreServer
{
    public class NPCAction
    {
        public int id;
        public List<NPCStep> steps;
        public Dictionary<int, string> reqs;
        public List<int> pals;

        public NPCAction(int Id)
        {
            id = Id;
        }

        public NPCAction()
        {

        }
    }
}