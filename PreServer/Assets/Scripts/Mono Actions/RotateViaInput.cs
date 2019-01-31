using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/Mono Actions/Rotate Via Input")]
    public class RotateViaInput : Action
    {
        public FloatVariable targetFloat;
        public TransformVariable targetTransform;
        public FloatVariable delta;

        public float angle;
        public float speed = 9;
        public bool negative;
        public bool clamp;
        public float minClamp = -35;
        public float maxClamp = 35;
        public RotateAxis targetAxis;

        public override void Execute()
        {
            float t = delta.value * speed;

            if (!negative)
                angle = Mathf.Lerp(angle, angle += targetFloat.value, t);
            else
                angle = Mathf.Lerp(angle, angle -= targetFloat.value, t);

            if (clamp)
            {
                angle = Mathf.Clamp(angle, minClamp, maxClamp);
            }

            switch (targetAxis)
            {
                case RotateAxis.x:
                    targetTransform.value.localRotation = Quaternion.Euler(angle, 0, 0);
                    break;
                case RotateAxis.y:
                    targetTransform.value.localRotation = Quaternion.Euler(0, angle, 0);
                    break;
                case RotateAxis.z:
                    targetTransform.value.localRotation = Quaternion.Euler(0, 0, angle);
                    break;
                default:
                    break;
            }
        }

        public enum RotateAxis
        {
            x, y, z
        }
    }
}