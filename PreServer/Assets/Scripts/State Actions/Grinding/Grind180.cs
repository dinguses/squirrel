using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Executing a 180  turn on the grind
    /// </summary>

    [CreateAssetMenu (menuName = "Actions/State Actions/Grind 180")]
    public class Grind180 : StateActions
    {
        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            //if (states.inGrindZone)

            var fart = states.mTransform.rotation.eulerAngles;

            var scrung = states.anim.GetFloat(states.hashes.rotate_test);
            var scrungHOLD = scrung;

            scrung = scrung - states.rotateHOLD;

            states.mTransform.rotation = Quaternion.Euler(fart.x, fart.y + scrung, fart.z);

            //Debug.Log("Grind 180: " + states.mTransform.rotation.eulerAngles.y);
            var bung = states.mTransform.forward;
            bung.Normalize();
            //Debug.Log(bung);

            states.rotateHOLD = scrungHOLD;

            //states.movementVariables.horizontal = 0;
            //states.movementVariables.vertical = 0;

            //// If the angle between the player and the new forward point is less than 105 degrees
            //if (states.angleTest < 105/* && states.angleTest < 150*/)
            //{
            //    // Get the current rotation of the player
            //    float currentAngle = states.mTransform.rotation.eulerAngles.y;

            //    // Add to the player's rotation
            //    states.mTransform.rotation =
            //     Quaternion.AngleAxis(currentAngle + (states.delta * 890), states.groundNormal);
            //    states.angleTest += states.delta * 890;

            //    // Increase frame counter
            //    states.testINT++;
            //}

            //// The player has rotated most of the way around, stop rotating, but keep updating frame counter
            //else if (states.testINT < 12)
            //{
            //    states.testINT++;
            //}

            //// The animation should be ending now
            //else
            //{
            //    // No longer waiting for animation to finish
            //    states.anim.SetBool(states.hashes.waitForAnimation, false);

            //    // TODO: Should the player be moved back to the center here?
            //    //var bungus = states.grindCenter.ClosestPoint(states.mTransform.position);
            //    //states.rigid.MovePosition(bungus);

            //    // If the player is inputing movement, start running, if not, idle
            //    if (states.movementVariables.moveAmount > 0.1f)
            //    {
            //        states.anim.CrossFade(states.hashes.squ_grind_run, 0.4f);
            //    }

            //    else
            //    {
            //        states.anim.CrossFade(states.hashes.squ_grind_idle, 0.4f);
            //    }
            //}
        }
    }
}
