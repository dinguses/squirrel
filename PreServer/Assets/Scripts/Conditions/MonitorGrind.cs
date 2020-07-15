using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PreServer
{
    /// <summary>
    /// Checks if player should be grinding
    /// </summary>

    [CreateAssetMenu (menuName = "Conditions/Monitor Grind")]
    public class MonitorGrind : Condition
    {
        public override bool CheckCondition(StateManager sm)
        {
            PlayerManager state = (PlayerManager)sm;

            bool result = false;
            Vector3 closestPointValue = new Vector3(10000, 10000, 10000);
            Vector3 otherPointValue = new Vector3(10000, 10000, 10000);
            KeyValuePair<int, Vector3> closestPoint = new KeyValuePair<int, Vector3>();
            KeyValuePair<int, Vector3> otherPoint = new KeyValuePair<int, Vector3>();
            Vector3 reusable = (state.mTransform.position + (state.mTransform.forward));

            // If player is in grind zone and has landed on the ground of the grind
            if (state.inGrindZone /*&& state.isGrounded*/)
            {
                // Go through all grindpoints and find the closest point to the player
                foreach (KeyValuePair<int, Vector3> grindPoint in state.grindPoints)
                {
                    if (Vector3.Distance(grindPoint.Value, reusable) < Vector3.Distance(closestPointValue, reusable))
                    {
                        closestPoint = grindPoint;
                        closestPointValue = grindPoint.Value;
                    }
                }

                // If the grind point is the first on the grind, the other point must be the second
                if (closestPoint.Key == 0)
                {
                    otherPoint = new KeyValuePair<int, Vector3>(1, state.grindPoints[1]);
                    otherPointValue = otherPoint.Value;
                }
                
                // If the grind point is the last on the grind, the other point must be the 2nd to last
                else if (closestPoint.Key == (state.grindPoints.Count - 1))
                {
                    otherPoint = new KeyValuePair<int, Vector3>(closestPoint.Key - 1, state.grindPoints[closestPoint.Key - 1]);
                    otherPointValue = otherPoint.Value;
                }

                else
                {
                    // Get the points directly before and after the closest, one of them has to be the other point the player is between
                    Vector3 oneHigher = state.grindPoints[closestPoint.Key + 1];
                    Vector3 oneLower = state.grindPoints[closestPoint.Key - 1];

                    // Find distance between closest point and other high/low potential points
                    float currPointToHigh = Vector3.Distance(oneHigher, closestPointValue);
                    float currPointToLow = Vector3.Distance(oneLower, closestPointValue);

                    // Find distance between player and high/low potential points
                    float highPointToPlayer = Vector3.Distance(oneHigher, reusable);
                    float lowPointToPlayer = Vector3.Distance(oneLower, reusable);

                    // If distance between point and closest point is greater than distance between point and player, it must be the other point
                    if (currPointToHigh > highPointToPlayer)
                    {
                        otherPoint = new KeyValuePair<int, Vector3>(closestPoint.Key + 1, state.grindPoints[closestPoint.Key + 1]);
                        otherPointValue = otherPoint.Value;
                    }
                    else
                    {
                        otherPoint = new KeyValuePair<int, Vector3>(closestPoint.Key - 1, state.grindPoints[closestPoint.Key - 1]);
                        otherPointValue = otherPoint.Value;
                    }
                }

                // Time to find out which point the player should be facing on the grind

                // Get the angle between the player's direction and the closest point
                Vector3 _closestDirection = (closestPointValue - reusable).normalized;
                Quaternion _closestLookRotation = Quaternion.LookRotation(_closestDirection);
                float closestAngle = Quaternion.Angle(state.mTransform.rotation, _closestLookRotation);

                // Get the angle between the player's direction and the other point
                Vector3 _otherDirection = (otherPointValue - reusable).normalized;
                Quaternion _otherLookRotation = Quaternion.LookRotation(_otherDirection);
                float otherAngle = Quaternion.Angle(state.mTransform.rotation, _otherLookRotation);

                // If the angle towards the closest point is less than the other angle, than it's the point to face towards
                if (closestAngle < otherAngle)
                {
                    state.facingPoint = closestPointValue;
                    state.behindPoint = otherPointValue;

                    state.facingPointPair = closestPoint;
                    state.behindPointPair = otherPoint;
                }
                else
                {
                    state.facingPoint = otherPointValue;
                    state.behindPoint = closestPointValue;

                    state.facingPointPair = otherPoint;
                    state.behindPointPair = closestPoint;
                }

                // Figure out what grind center to move along
                //if (state.grindPoints.Count > 2)
                //{
                //    if (state.facingPointPair.Key < state.behindPointPair.Key)
                //    {
                //        string test = "GrindCenter_" + state.facingPointPair.Key.ToString() + state.behindPointPair.Key.ToString();

                //        for (int i = 0; i < state.grindCenters.Count; i++)
                //        {
                //            var ttttee = state.grindCenters[i];

                //            if (ttttee.name == test)
                //            {
                //                state.grindCenter = state.grindCenters[i];
                //                state.grindCenterPair = new KeyValuePair<int, BoxCollider>(state.facingPointPair.Key, state.grindCenter);
                //            }
                //        }

                //        //state.grindCenter = state.grindCenters2[test];
                //        //state.grindCenterPair2 = new KeyValuePair<string, BoxCollider>(test, state.grindCenter);

                //        //state.grindCenter = state.grindCenters[state.facingPointPair.Key];
                //        //state.grindCenterPair = new KeyValuePair<int, BoxCollider>(state.facingPointPair.Key, state.grindCenter);
                //    }
                    
                //    else
                //    {
                //        string test = "GrindCenter_" + state.behindPointPair.Key.ToString() + state.facingPointPair.Key.ToString();

                //        for (int i = 0; i < state.grindCenters.Count; i++)
                //        {
                //            var ttttee = state.grindCenters[i];

                //            if (ttttee.name == test)
                //            {
                //                state.grindCenter = state.grindCenters[i];
                //                state.grindCenterPair = new KeyValuePair<int, BoxCollider>(state.behindPointPair.Key, state.grindCenter);
                //            }
                //        }

                //        /*string test = "GrindCenter" + state.behindPointPair.Key.ToString() + state.facingPointPair.Key.ToString();

                //        state.grindCenter = state.grindCenters2[test];
                //        state.grindCenterPair2 = new KeyValuePair<string, BoxCollider>(test, state.grindCenter);*/
                        
                //        //state.grindCenter = state.grindCenters[state.behindPointPair.Key];
                //        //state.grindCenterPair = new KeyValuePair<int, BoxCollider>(state.behindPointPair.Key, state.grindCenter);
                //    }
                //}
                

                // No gravity on grinds
                state.rigid.useGravity = false;

                state.anim.SetBool(state.hashes.isGrinding, true);

                float moveAmount = state.movementVariables.moveAmount;

                if (moveAmount > 0.1f && moveAmount < 0.3f)
                {
                    state.anim.CrossFade(state.hashes.squ_grind_walk, 0.1f);
                }
                else if (moveAmount > 0.3f)
                {
                    state.anim.CrossFade(state.hashes.squ_grind_run, 0.1f);
                }
                else
                {
                    state.anim.CrossFade(state.hashes.squ_grind_idle, 0.1f);
                }

                
                // Stop the player's previous movement
                state.movementVariables.moveAmount = 0;
                state.rigid.velocity = new Vector3(0, 0, 0);

                Vector3 testMiddle = state.mTransform.position + state.mTransform.forward;

                Vector3 grindCenterClosestPoint = GetPoint(state.rigid.position, state.facingPoint, state.behindPoint);
                Vector3 grindCenterClosestPointMiddle = GetPoint(testMiddle, state.facingPoint, state.behindPoint);

                Debug.Log("Grind center closest point - " + grindCenterClosestPoint);
                Debug.Log("Grind center closest pointMid - " + grindCenterClosestPointMiddle);

                if (grindCenterClosestPointMiddle.y > grindCenterClosestPoint.y)
                {
                    float testt = grindCenterClosestPointMiddle.y - grindCenterClosestPoint.y;

                    Debug.Log("it higher");
                    var test = state.mTransform.position.y;
                    state.mTransform.position = new Vector3(state.mTransform.position.x, test += testt, state.mTransform.position.z);
                }


                //state.frontCollider.enabled = false;

                //GOTO
                //state.frontCollider.enabled = false;
                //state.grindCollider.enabled = true;

                result = true;

            }

            return result;
        }

        Vector3 GetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }
    }
}
