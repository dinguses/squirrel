using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public float followSpeed;
    public float offset;
    public enum OffsetDirection { NONE, FORWARD, UP, RIGHT }
    public OffsetDirection direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Vector3 off = Vector3.zero;
            if (offset != 0)
            {
                switch (direction)
                {
                    case OffsetDirection.FORWARD:
                        off = target.forward * offset;
                        break;
                    case OffsetDirection.RIGHT:
                        off = target.right * offset;
                        break;
                    case OffsetDirection.UP:
                        off = target.up * offset;
                        break;
                }
            }
            Vector3 targetPos = target.position + off;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        }
    }
}
