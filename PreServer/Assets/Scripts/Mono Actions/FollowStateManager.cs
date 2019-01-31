using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/Mono Actions/Follow State Manager")]
    public class FollowStateManager : Action
    {
        public StateManagerVariable stateVariable;
        public TransformVariable currentTransform;
        public FloatVariable delta;

        public float speed = 9;

        public bool isAtFixed;

        public override void Execute()
        {
            if (stateVariable.value == null)
                return;
            if (currentTransform.value == null)
                return;

            if (isAtFixed)
            {
                if (!stateVariable.value.followMeOnFixedUpdate)
                    return;
            }
            else
            {
                if (stateVariable.value.followMeOnFixedUpdate)
                    return;
            }

            Vector3 targetPosition = Vector3.Lerp(currentTransform.value.position, stateVariable.value.mTransform.position, delta.value * speed);
            currentTransform.value.position = targetPosition;
        }
    }
}
