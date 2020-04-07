using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class Drag : MonoBehaviour
    {
        float offsetX;
        float offsetY;

        public void StartDrag()
        {
            offsetX = transform.position.x - Input.mousePosition.x;
            offsetY = transform.position.y - Input.mousePosition.y;
        }

        public void OnDrag()
        {
            transform.position = new Vector3(offsetX + Input.mousePosition.x, offsetY + Input.mousePosition.y);
        }
    }
}