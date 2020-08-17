using UnityEngine;
using System.Collections;
namespace PreServer
{
    /// <summary>
    /// Rudimentary way to slide the player if they should be sliding
    /// 
    /// Barely works
    /// </summary>

    [CreateAssetMenu(menuName = "Actions/State Actions/Slide Player")]
    public class SlidePlayer : StateActions
    {
        public float gravityAdditive;
        public float downwardsGravity;
        public float movementTime = 10;
        public float slideTime = 0.3f;
        public float movementSpeed;
        float timer = 0;
        float gravity = 0;
        PlayerManager states;
        Vector3 rotationNormal;
        float timeOutPeriod = 0;
        public override void Execute(StateManager sm)
        {

        }

        public override void OnEnter(StateManager sm)
        {
            states = (PlayerManager)sm;

            //timer = 0;
            //gravity = downwardsGravity;
            timeOutPeriod = 0;
            lookDirection = states.transform.forward;
            targetDirection = SlopeDirection();
            moveDirection = states.rigid.velocity.normalized;
            if (moveDirection == Vector3.zero || moveDirection.magnitude < 1f)
                moveDirection = states.transform.forward;
            moveSpeed = states.rigid.velocity.magnitude * 0.5f;
            if (moveSpeed < 1f)
                moveSpeed = 1f;
            states.rigid.velocity = Vector3.zero;
            moveDirection = ProjectVectorOnPlane(GroundNormal(), ProjectVectorOnPlane(states.transform.up, moveDirection));
            moveDirection = SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));

            states.anim.CrossFade(states.hashes.squ_sliding, 0.2f);
        }
        Vector3 currentVelocity;
        Vector3 targetVelocity;
        public override void OnUpdate(StateManager sm)
        {
            rotationNormal = states.GetRotationNormal();
            Slide();
            RotateBasedOnGround();
            SlideRotation();
        }

        public override void OnLateUpdate(StateManager states)
        {
            states.transform.position += this.moveDirection * Time.deltaTime;
            base.OnLateUpdate(states);
        }

        float moveSpeed = 0;
        float turnSpeed = 10f;
        float maxSlideSpeed = 26f;
        Vector3 lookDirection = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;
        Vector3 targetDirection = Vector3.zero;
        void Slide()
        {
            //Check if I hit an enemy - need enemies to implement
            //NOTE: COME BACK TO THIS ONCE ENEMIES ARE IMPLENTED
            //if (!goldMario)
            //    BodySlam();
            //else
            //    GoldBodySlam();

            //Make the move direction orthoganal to the player's up
            Vector3 vector3 = ProjectVectorOnPlane(states.transform.up, moveDirection);
            lookDirection = states.transform.forward;
            //TIME TO FIGURE OUT THIS SHIT
            //If I hit a wall then reflect off of it otherwise keep going
            Vector3 normal = Vector3.zero;
            if (HasWallCollided(out normal) && (double)Vector3.Angle(-vector3.normalized, normal) < 75.0)
            {
                moveSpeed = -2f;
                lookDirection = Vector3.Reflect(-ProjectVectorOnPlane(states.transform.up, moveDirection), normal).normalized;

                //Make player go back a bit and then continue?
                //sound.PlayHeavyKnockback();
                //currentState = (Enum)MarioMachine.MarioStates.Knockback;
            }
            else
            {
                Vector3 MoveInput = GetMoveInput();

                //Make the slope's direction orthogonal to the player's up
                Vector3 to = ProjectVectorOnPlane(states.transform.up, SlopeDirection());

                //Make the input orthogonal to the ground normal
                //Vector3 normalized = ProjectVectorOnPlane(GroundNormal(), MoveInput).normalized;

                //Flag 1 - Angle between move direction and slope direction
                bool flag1 = (double)Vector3.Angle(vector3, to) < 90.0;

                ////Flag 2 - Angle between look direction and slope direction
                //bool flag2 = (double)Vector3.Angle(lookDirection, to) > 140.0;

                //Change move direction to be orthogonal to the current slope and set its length to the move speed
                moveDirection = ProjectVectorOnPlane(GroundNormal(), moveDirection);
                moveDirection = SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));


                //if I am still sliding, then continue, otherwise slow down
                if (IsContinueSliding())
                {
                    //If angle between move direction and slope direction < 90 and there is input, turn a little
                    //otherwise face the slope's direction
                    //if (flag1 && MoveInput != Vector3.zero && ((double)Vector3.Angle(MoveInput, to) < 110.0 && (double)Vector3.Angle(vector3, to) < 70.0))
                    //{
                    //    targetDirection = Vector3.RotateTowards(targetDirection, normalized, turnSpeed * 0.4f * Time.deltaTime, 0.0f);
                    //    targetDirection = ClampAngleOnPlane(SlopeDirection(), targetDirection, 70f, GroundNormal());
                    //}
                    //else
                        
                    if(flag1 && MoveInput != Vector3.zero)
                    {
                        bool isBackwards = Vector3.Angle(states.transform.forward, targetDirection) > 90;
                        targetDirection = Vector3.RotateTowards(targetDirection, isBackwards ? -lookDirection : lookDirection, turnSpeed * Time.deltaTime, 0.0f);
                        targetDirection = ClampAngleOnPlane(SlopeDirection(), targetDirection, 45f, GroundNormal());
                    }
                    else
                    {
                        targetDirection = SlopeDirection();
                    }
                    //Debug.DrawRay(states.transform.position, targetDirection, Color.green);

                    //Current 1 - a vector orthogonal to the target direction
                    Vector3 current1 = ProjectVectorOnPlane(targetDirection, moveDirection);

                    Vector3 current2 = moveDirection - current1;
                    //Debug.DrawRay(states.transform.position, current1 * 5f, Color.green);
                    //Debug.DrawRay(states.transform.position, current2 * 5f, Color.blue);
                    //take the direction I'm moving towards and add it to the slide direction while increasing the slide speed 
                    moveDirection = Vector3.MoveTowards(current1, Vector3.zero, 5f * Time.deltaTime) + Vector3.MoveTowards(current2, targetDirection * maxSlideSpeed, 20f * Time.deltaTime);
                    moveSpeed = moveDirection.magnitude;

                    //Check if I'm looking backwards then change my speed to move me backwards
                    if ((double)Vector3.Angle(lookDirection, to) > 90.0 && (double)Vector3.Angle(lookDirection, vector3) > 90.0)
                        moveSpeed *= -1f;

                    //I'm guessing this part of the code has to do with which direction I'm facing to know which way to rotate me
                    //Flag 1 - Angle between move direction and slope direction < 90
                    //if (flag1)
                    //{
                    //    //Flag 2 - Angle between look direction and slope direction > 140
                    //    if (flag2)
                    //    {
                    //        RotateLookDirection(-targetDirection, turnSpeed * 0.05f);
                    //    }
                    //    else
                    //    {
                    //        RotateLookDirection(targetDirection, turnSpeed * 0.15f);
                    //    }
                    //}
                }
                else
                {
                    //Slow me down if I'm not sliding
                    moveSpeed = Mathf.MoveTowards(moveSpeed, 0.0f, 15f * Time.deltaTime);
                    //Debug.LogError(moveSpeed);
                    moveDirection = SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));
                    if (MoveInput != Vector3.zero)
                    {
                        moveDirection = Vector3.RotateTowards(moveDirection, lookDirection, turnSpeed * 0.1f * Time.deltaTime, 0.0f);
                    }
                    //if (MoveInput != Vector3.zero)
                    //    moveDirection = Vector3.RotateTowards(moveDirection, normalized, turnSpeed * 0.1f * Time.deltaTime, 0.0f);
                    //if ((double)moveSpeed < 0.0)
                    //{
                    //    RotateLookDirection(-vector3, turnSpeed * 0.1f);
                    //}
                    //else
                    //{
                    //    RotateLookDirection(vector3, turnSpeed * 0.1f);
                    //}
                    timeOutPeriod = 0;
                }
                if(states.isGrounded)
                    moveDirection -= GroundNormal() * 0.25f;
                //If my move speed is in between 0.3 and -0.3 then I am done sliding
                if (((double)moveSpeed <= 0.5 && (double)moveSpeed >= -0.5 && IsContinueSliding() && timeOutPeriod >= 0.15f) || moveSpeed == 0.0 || !states.isGrounded)
                {
                    states.isSliding = false;
                    //Debug.LogError("Exiting slide");

                    //currentState = (Enum)MarioMachine.MarioStates.SlideRecover;
                }
                else
                {
                    //artUpDirection = GroundNormal();
                }
                if (((double)moveSpeed <= 0.5 && (double)moveSpeed >= -0.5))
                    timeOutPeriod += Time.deltaTime;
            }
            //states.transform.forward = lookDirection;
        }

        void SlideReference()
        {
            ////If I'm not ground then get out of sliding state
            ////NOTE: MAKE THIS A CONDITION
            //if (!this.MaintainingGround())
            //    this.currentState = (Enum)MarioMachine.MarioStates.Fall;

            ////If I'm not sliding then I must be out of the slide state, If I jump then change states
            ////NOTE: THERE WILL NOT BE JUMPING OFF SLIDES
            //else if (!this.IsContinueSliding() && (this.input.Current.JumpDown || this.input.Current.StrikeDown))
            //{
            //    this.currentState = (Enum)MarioMachine.MarioStates.SFlip;
            //}

            ////THIS IS THE ONLY CODE THAT SHOULD BE DONE IN UPDATE
            //else
            //{
            //    //Check if I hit an enemy - need enemies to implement
            //    //NOTE: COME BACK TO THIS ONCE ENEMIES ARE IMPLEMENTED
            //    //if (!this.goldMario)
            //    //    this.BodySlam();
            //    //else
            //    //    this.GoldBodySlam();

            //    //Make the move direction orthoganal to the player's up
            //    Vector3 vector3 = ProjectVectorOnPlane(states.tranform.up, this.moveDirection);
            //    
            //    //TIME TO FIGURE OUT THIS SHIT
            //    //If I hit a wall then reflect off of it otherwise keep going
            //    SuperCollision superCollision;
            //    if (this.HasWallCollided(out superCollision) && (double)Vector3.Angle(-vector3.normalized, superCollision.normal) < 75.0)
            //    {
            //        this.moveSpeed = -2f;
            //        this.lookDirection = Vector3.Reflect(-ProjectVectorOnPlane(this.controller.up, this.moveDirection), superCollision.normal).normalized;
            //        
            //        //Make player go back a bit and then continue?
            //        //this.sound.PlayHeavyKnockback();
            //        this.currentState = (Enum)MarioMachine.MarioStates.Knockback;
            //    }

            //    else
            //    {
            //        //Make the slope's direction orthogonal to the player's up
            //        Vector3 to = ProjectVectorOnPlane(this.controller.up, this.SlopeDirection());

            //        //Make the input orthogonal to the ground normal
            //        Vector3 normalized = ProjectVectorOnPlane(this.GroundNormal(), this.input.Current.MoveInput).normalized;

            //        //Flag 1 - Angle between move direction and slope direction
            //        bool flag1 = (double)Vector3.Angle(vector3, to) < 90.0;

            //        //Flag 2 - Angle between look direction and slope direction
            //        bool flag2 = (double)Vector3.Angle(this.lookDirection, to) > 140.0;

            //        //Change move direction to be orthogonal to the current slope and set its length to the move speed
            //        this.moveDirection = ProjectVectorOnPlane(this.GroundNormal(), this.moveDirection);
            //        this.moveDirection = SetVectorLength(this.moveDirection, Mathf.Abs(this.moveSpeed));
            //
            //
            //        //if I am still sliding, then continue, otherwise slow down
            //        if (this.IsContinueSliding())
            //        {
            //            //If angle between move direction and slope direction < 90 and there is input, turn a little
            //            //otherwise face the slope's direction
            //            if (flag1 && this.input.Current.MoveInput != Vector3.zero && ((double)Vector3.Angle(this.input.Current.MoveInput, to) < 110.0 && (double)Vector3.Angle(vector3, to) < 70.0))
            //            {
            //                this.targetDirection = Vector3.RotateTowards(this.targetDirection, normalized, this.turnSpeed * 0.4f * this.controller.deltaTime, 0.0f);
            //                this.targetDirection = SuperMath.ClampAngleOnPlane(this.SlopeDirection(), this.targetDirection, 70f, this.GroundNormal());
            //            }
            //            else
            //                this.targetDirection = this.SlopeDirection();
            //
            //            //Current 1 - a vector orthogonal to the target direction
            //            Vector3 current1 = ProjectVectorOnPlane(this.targetDirection, this.moveDirection);
            //            
            //            Vector3 current2 = this.moveDirection - current1;
            //
            //            //take the direction I'm moving towards and add it to the slide direction while increasing the slide speed 
            //            this.moveDirection = Vector3.MoveTowards(current1, Vector3.zero, 5f * this.controller.deltaTime) + Vector3.MoveTowards(current2, this.targetDirection * this.maxSlideSpeed, 20f * this.controller.deltaTime);
            //            this.moveSpeed = this.moveDirection.magnitude;
            //
            //            //Check if I'm looking backwards then change my speed to move me backwards
            //            if ((double)Vector3.Angle(this.lookDirection, to) > 90.0 && (double)Vector3.Angle(this.lookDirection, vector3) > 90.0)
            //                this.moveSpeed *= -1f;
            //
            //            //I'm guessing this part of the code has to do with which direction I'm facing to know which way to rotate me
            //            //Flag 1 - Angle between move direction and slope direction < 90
            //            if (flag1)
            //            {
            //                //Flag 2 - Angle between look direction and slope direction > 140
            //                if (flag2)
            //                    this.RotateLookDirection(-this.targetDirection, this.turnSpeed * 0.05f);
            //                else
            //                    this.RotateLookDirection(this.targetDirection, this.turnSpeed * 0.15f);
            //            }
            //        }
            //        else
            //        {
            //            //Slow me down if I'm not sliding
            //            this.moveSpeed = Mathf.MoveTowards(this.moveSpeed, 0.0f, 15f * this.controller.deltaTime);
            //            this.moveDirection = SetVectorLength(this.moveDirection, Mathf.Abs(this.moveSpeed));
            //            if (this.input.Current.MoveInput != Vector3.zero)
            //                this.moveDirection = Vector3.RotateTowards(this.moveDirection, normalized, this.turnSpeed * 0.1f * this.controller.deltaTime, 0.0f);
            //            if ((double)this.moveSpeed < 0.0)
            //                this.RotateLookDirection(-vector3, this.turnSpeed * 0.1f);
            //            else
            //                this.RotateLookDirection(vector3, this.turnSpeed * 0.1f);
            //        }
            //        //If my move speed is 0 then I am done sliding
            //        if ((double)this.moveSpeed == 0.0)
            //        {
            //            //this.currentState = (Enum)MarioMachine.MarioStates.SlideRecover;
            //        }
            //        else
            //        {
            //            //this.artUpDirection = this.GroundNormal();
            //        }
            //    }
            //}
        }

        Vector3 GetMoveInput()
        {
            Transform cam = Camera.main.transform;
            Vector3 zero = Vector3.zero;
            if ((double)states.movementVariables.horizontal != 0.0)
                zero += cam.right * states.movementVariables.horizontal;
            if ((double)states.movementVariables.vertical != 0.0)
                zero += cam.forward * states.movementVariables.vertical;
            return ProjectVectorOnPlane(states.transform.up, zero).normalized;
        }

        public override void OnExit(StateManager sm)
        {
            states.slideMomentum = ProjectVectorOnPlane(Vector3.up, moveDirection.normalized * moveSpeed);
            //timer = 0;
            //gravity = downwardsGravity;
            //if (states.isGrounded)
            //{
            //    Vector3 slideDirection = Vector3.zero;
            //    if (GetAngle(states.middle, states.middleNormal))
            //    {
            //        Vector3 forward = Vector3.Cross(states.middleNormal, Vector3.up);
            //        slideDirection = Vector3.Cross(forward, states.middleNormal);
            //    }
            //    else if (GetAngle(states.front, states.frontNormal))
            //    {
            //        Vector3 forward = Vector3.Cross(states.frontNormal, Vector3.up);
            //        slideDirection = Vector3.Cross(forward, states.frontNormal);
            //    }
            //    else if (GetAngle(states.back, states.backNormal))
            //    {
            //        Vector3 forward = Vector3.Cross(states.backNormal, Vector3.up);
            //        slideDirection = Vector3.Cross(forward, states.backNormal);
            //    }
            //    else
            //    {
            //        Vector3 forward = Vector3.Cross(states.BackCastNormal(), Vector3.up);
            //        slideDirection = Vector3.Cross(forward, states.BackCastNormal());
            //    }
            //    //Only happens on flat ground
            //    if (slideDirection == Vector3.zero)
            //    {
            //        slideDirection = Vector3.Cross(states.BackCastNormal(), Vector3.up);
            //    }
            //    states.rigid.velocity = currentVelocity;
            //    states.slideMomentum = Vector3.Project(currentVelocity, -slideDirection);
            //    //states.slideMomentum.y = 0;
            //    //Debug.DrawLine(states.transform.position + states.transform.up + states.transform.forward, -slideDirection, Color.cyan, 10f);
            //    Debug.DrawRay(states.transform.position + states.transform.up + states.transform.forward, -slideDirection * 5, Color.red, 5f);
            //}
            //if(Vector3.Angle(states.groundNormal, Vector3.up) > 35)
            //{
            //    states.groundNormal = Vector3.zero;
            //}
            //Debug.Log(Time.frameCount + " || Slide Player On State Exit");
        }

        bool GetAngle(GameObject obj, Vector3 normal)
        {
            if (obj == null)
                return false;
            float angle = Vector3.Angle(normal, Vector3.up);
            if (angle < 35)
                return true;
            return false;
        }

        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        {
            return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
        }

        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {
            Vector3 vector3;
            return vector3 = Vector3.Normalize(vector) * size;
        }

        private void RotateLookDirection(Vector3 target, float speed)
        {
            lookDirection = Vector3.RotateTowards(lookDirection, target, speed * Time.deltaTime, 0.0f);
        }

        private void RotateMoveDirection(Vector3 target, float speed)
        {
            moveDirection = Vector3.RotateTowards(moveDirection, target, speed * Time.deltaTime, 0.0f);
        }

        private float GroundAngle()
        {
            return Vector3.Angle(Vector3.up, states.middleNormal);
        }

        private Vector3 GroundNormal()
        {
            return states.middleNormal;
        }

        private bool IsSliding()
        {
            if (states.slideTerrain != null)
                return GroundAngle() > (double)states.slideTerrain.slideAngle;
            else
                return false;
        }

        private bool IsContinueSliding()
        {
            if (states.slideTerrain != null)
                return GroundAngle() > (double)states.slideTerrain.slideContinueAngle;
            else
                return false;
        }

        private Vector3 SlopeDirection()
        {
            Vector3 normal = rotationNormal;
            return Vector3.Cross(Vector3.Cross(normal, -Vector3.up), normal);
        }

        public static Vector3 ClampAngleOnPlane(Vector3 origin, Vector3 direction, float angle, Vector3 planeNormal)
        {
            return (double)Vector3.Angle(origin, direction) < (double)angle ? direction : Quaternion.AngleAxis(((double)Vector3.Angle(Vector3.Cross(planeNormal, origin), direction) >= 90.0 ? -1f : 1f) * angle, planeNormal) * origin;
        }

        //I need to change this to know if the squirrel hit a wall
        private bool HasWallCollided(out Vector3 normal)
        {
            RaycastHit hit = new RaycastHit();
            if (moveSpeed >= 0)
            {
                Vector3 origin = states.transform.position + (states.transform.up * 0.45f);
                Vector3 direction = states.transform.forward;
                if (Physics.SphereCast(origin, 0.4f, direction, out hit, 2.05f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    normal = hit.normal;
                    return true;
                }
            }
            else
            {
                Vector3 origin = states.transform.position + (states.transform.up * 0.45f) + (states.transform.forward * 2f);
                Vector3 direction = -states.transform.forward;
                if (Physics.SphereCast(origin, 0.4f, direction, out hit, 2.05f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    normal = hit.normal;
                    return true;
                }
            }
            normal = Vector3.zero;
            return false;
        }

        public float rotSpeed = 8;
        public float rotationConstraint = 70;
        void RotateBasedOnGround()
        {
            Vector3 normal = rotationNormal;
            if (SlopeDirection() != Vector3.zero)
            {
                bool isBackwards = Vector3.Angle(states.transform.forward, SlopeDirection()) > 90;
                Vector3 origin = states.transform.position + (states.transform.up * 0.2f) + (moveDirection * Time.deltaTime);
                RaycastHit hit = new RaycastHit();
                Vector3 dir = states.transform.forward;
                if (isBackwards)
                {
                    dir *= -1;
                }
                else
                {
                    origin += (states.transform.forward * 1.9f);
                }
                dir -= states.transform.up * 0.1f;
                Debug.DrawRay(origin, dir * 0.5f, Color.blue);
                //Raycast in front of the squirrel, used to check if we've hit a ceiling, ground, or another climb-able surface
                if (Physics.SphereCast(origin, 0.4f, states.transform.forward, out hit, 0.5f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore))
                {
                    normal = (hit.normal + rotationNormal).normalized;
                    Debug.DrawRay(hit.point, hit.normal, Color.black);
                }
            }
            Quaternion tr = Quaternion.FromToRotation(states.mTransform.up, normal) * states.mTransform.rotation;
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * 15f);
            states.mTransform.rotation = targetRotation;
        }

        void SlideRotation()
        {
            Transform cam = Camera.main.transform;
            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;

            Vector3 targetDir = cam.forward * v;
            targetDir += cam.right * h;

            targetDir.Normalize();
            targetDir.y = 0;

            Vector3 slopeDir = SlopeDirection();
            bool isBackwards = Vector3.Angle(states.transform.forward, slopeDir) > 90;
            if (slopeDir != Vector3.zero)
            {
                if (targetDir == Vector3.zero)
                    targetDir = isBackwards ? -slopeDir : slopeDir;

                targetDir = ProjectVectorOnPlane(rotationNormal, targetDir);
                float angle = Vector3.SignedAngle((isBackwards ? -slopeDir : slopeDir), targetDir, rotationNormal);
                if (Mathf.Abs(angle) > 45f)
                {
                    targetDir = Quaternion.AngleAxis(45 * Mathf.Sign(angle), rotationNormal) * (isBackwards ? -slopeDir : slopeDir);
                }
                
            }
            else
            {
                if (targetDir == Vector3.zero)
                    targetDir = states.transform.forward;
                targetDir = ProjectVectorOnPlane(rotationNormal, targetDir);
            }

            Quaternion tr = Quaternion.LookRotation(targetDir, rotationNormal);
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, states.delta * turnSpeed * 0.15f);
            states.mTransform.rotation = targetRotation;
        }
    }
}
