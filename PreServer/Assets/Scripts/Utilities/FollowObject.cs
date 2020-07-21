using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public float followSpeed;
    public enum OffsetDirection { NONE, FORWARD, UP, RIGHT }
    public enum UpdateTick { NORMAL, LATE, FIXED }
    public UpdateTick update;
    public Offset[] offsets;

    [System.Serializable]
    public class Offset
    {
        public OffsetDirection direction;
        public float offset;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (update == UpdateTick.NORMAL)
            UpdatePosition(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (update == UpdateTick.LATE)
            UpdatePosition(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (update == UpdateTick.FIXED)
            UpdatePosition(Time.fixedDeltaTime);
    }

    void UpdatePosition(float delta)
    {
        if (target != null)
        {
            Vector3 off = Vector3.zero;
            if (offsets.Length > 0)
            {
                for (int i = 0; i < offsets.Length; ++i)
                {
                    switch (offsets[i].direction)
                    {
                        case OffsetDirection.FORWARD:
                            off += target.forward * offsets[i].offset;
                            break;
                        case OffsetDirection.RIGHT:
                            off += target.right * offsets[i].offset;
                            break;
                        case OffsetDirection.UP:
                            off += target.up * offsets[i].offset;
                            break;
                    }
                }
            }
            Vector3 targetPos = target.position + off;
            transform.position = Vector3.Lerp(transform.position, targetPos, delta * followSpeed);
        }
    }
}
