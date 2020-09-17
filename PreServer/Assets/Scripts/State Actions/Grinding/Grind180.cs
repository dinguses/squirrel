using UnityEngine;

namespace PreServer
{
    /// <summary>
    /// Executing a 180  turn on the grind
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Grind 180")]
    public class Grind180 : StateActions
    {
        public override void Execute(StateManager sm)
        {
            PlayerManager states = (PlayerManager)sm;

            if (states.anim.GetBool(states.hashes.waitForAnimation))
            {
                //Debug.Log(states.testINT);

                states.testINT++;

                Vector3 eulerAngles = states.mTransform.rotation.eulerAngles;

                float rotateAmount = states.anim.GetFloat(states.hashes.rotateFloat);
                float currentHold = rotateAmount;

                rotateAmount = rotateAmount - states.held180RotationAmt;

                //if (states.inGrindZone)
                //{
                states.mTransform.RotateAround(states.middlePivot, states.mTransform.up, rotateAmount);
                //}

                /*else
                {
                    states.mTransform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + rotateAmount, eulerAngles.z);
                }*/

                //= Quaternion.Euler(eulerAngles.x, eulerAngles.y + rotateAmount, eulerAngles.z);

                //Debug.Log(eulerAngles.y);

                states.held180RotationAmt = currentHold;
            }

        }
    }
}
