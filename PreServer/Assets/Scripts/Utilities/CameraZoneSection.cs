using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneSection : MonoBehaviour
{
    public Action<Collider> enter;
    public Action<Collider> exit;
    public Collider col;
    //Can be used for more complex camera stuff later
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Squirrel")
        {
            if(enter != null)
            {
                enter(col);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Squirrel")
        {
            if (exit != null)
            {
                exit(col);
            }
        }
    }
}
