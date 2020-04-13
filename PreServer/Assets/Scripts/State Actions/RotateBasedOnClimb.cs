using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PreServer
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Rotate Based On Climb")]
    public class RotateBasedOnClimb : StateActions
    {
        public float rotationSpeed = 6;
        public override void Execute(StateManager states)
        {
            Vector3 center = states.transform.position;
            center += states.transform.forward + (states.transform.up * 0.2f);

            // Dir represents the downward direction
            Vector3 dir = -states.transform.up * 0.5f;

            // Draw the rays
            //Debug.DrawRay(center, dir * 5f, Color.red);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(center, dir, out hit, 5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
            {
                states.transform.rotation = Quaternion.Slerp(states.transform.rotation, Quaternion.FromToRotation(states.transform.up, hit.normal) * states.transform.rotation, Time.unscaledDeltaTime * rotationSpeed);
            }
        }
    }
}