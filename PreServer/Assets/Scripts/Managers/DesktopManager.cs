using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class DesktopManager : MonoBehaviour
    {
        public void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}