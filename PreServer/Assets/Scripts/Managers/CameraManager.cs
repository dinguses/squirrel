using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PreServer
{
    public class CameraManager : MonoBehaviour
    {
        //Public variables
        public Transform player;
        public FollowObject target;
        public Transform camTransform; //The viewing camera, used if the camera needs an additonal offset (Currently used in the camera zones)
        public SkinnedMeshRenderer playerMesh;
        public float camFollowSpeed = 9;
        public float rotationSmoothTime = .12f;
        public float camZoomSpeed = 4;
        public float mouseSens = 10;
        public float distanceAway; //Camera offset behind (Based on look direction)
        public float distanceUp; //Camera offset up
        public Vector2 pitchMinMax = new Vector2(-35, 35);
        
        float yaw;
        float pitch;
        float prevYaw = 0;
        float prevPitch = 0;
        float startUp;
        float startAway;
        float camSmoothDampTime = 0.1f;
        float timer = 0;
        Vector3 targetPos;
        Vector3 lookDir; //Direction between the player and the camera, allows the camera to rotate around the player, but not up and down for unknown reasons
        Vector3 prevPlayerPos;
        Vector3 currentRotation; //Used for camera zones
        Vector3 velocityCamSmooth = Vector3.zero;
        //Legacy variables that could be used later
        //public Vector3 camOffset = new Vector3(0, 4, -15);
        //Vector3 rotationSmoothVelocity;
        //public Vector3 camRel;
        //public bool debugPauseCamLerp;
        //public bool debugTeleport;
        //public float maxDistance = 3f;
        //public float minDistance = 0.5f;
        //public float smooth;

        public bool ignoreInput
        {
            get { return _ignoreInput; }
            set
            {
                if(!inCameraZone || value)
                    _ignoreInput = value;
            }
        }
        bool _ignoreInput = false;
        public bool ignoreMouse = false;
        public bool inCameraZone
        {
            get { return _inCameraZone; }
            set
            {
                _inCameraZone = value;
                ignoreInput = value;
            }
        }
        bool _inCameraZone = false;

        public bool ignorePitch
        {
            get { return _ignorePitch; }
            set
            {
                if (!inCameraZone || value)
                    _ignorePitch = value;
            }
        }
        bool _ignorePitch = false;

        public bool ignoreYaw
        {
            get { return _ignoreYaw; }
            set
            {
                if (!inCameraZone || value)
                    _ignoreYaw = value;
            }
        }
        bool _ignoreYaw = false;

        public bool onRails
        {
            get { return _onRails; }
            set
            {
                _onRails = value;
            }
        }
        bool _onRails = false;

        List<CameraAdjustment> camAdjustments = new List<CameraAdjustment>();

        public float GetYaw()
        {
            return yaw;
        }

        public void AddToYaw(float val, bool fromCameraZone = false)
        {
            if (!inCameraZone || fromCameraZone)
            {
                yaw += val;
                //Debug.LogError("Amount going into yaw: " + (yaw - prevYaw));
            }
        }

        public float GetPitch()
        {
            return pitch;
        }

        public void AddToPitch(float val, bool fromCameraZone = false)
        {
            if (!inCameraZone || fromCameraZone)
            {
                pitch += val;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            }
        }

        public void SetCurrentRotation(Vector3 v)
        {
            currentRotation = v;
        }

        public void AddCamAdjustment(CameraAdjustment c)
        {
            if (c.checkState)
            {
                for(int i = 0; i < camAdjustments.Count; ++i)
                {
                    if (c.state == camAdjustments[i].state)
                    {
                        camAdjustments[i].UpdateValues(c);
                        return;
                    }
                }
            }
            camAdjustments.Add(c);
        }

        public CameraAdjustment GetCamAdjustState(string state)
        {
            for (int i = 0; i < camAdjustments.Count; ++i)
            {
                if (state == camAdjustments[i].state)
                {
                    return camAdjustments[i];
                }
            }
            return null;
        }

        private void Start()
        {
            camTransform = Camera.main.transform;
            prevPlayerPos = player.position;
            startUp = distanceUp;
            startAway = distanceAway;
            if (!ignoreMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void FixedUpdate()
        {
            if (!ignoreInput)
            {
                if (ignoreMouse)
                {
                    yaw += ignoreYaw ? 0 : (Input.GetAxis("RightStickHorizontal")) * mouseSens * 0.5f;
                    pitch -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical")) * mouseSens * 0.75f;
                }
                else
                {
                    yaw += ignoreYaw ? 0 : (Input.GetAxis("RightStickHorizontal") + (Input.GetAxis("Mouse X") * .2f)) * mouseSens * 0.5f;
                    pitch -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical") + (Input.GetAxis("Mouse Y") * .2f)) * mouseSens * 0.75f;
                }
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            }
            else
            {
                Debug.Log("PrevYaw: " + prevYaw + " Yaw: " + yaw);
            }
            
            if (!onRails)
            {
                if(camAdjustments.Count > 0)
                {
                    if (camAdjustments[0].checkState)
                    {
                        if(camAdjustments[0].state != PlayerManager.ptr.currentState.name)
                            camAdjustments.RemoveAt(0);
                    }
                    if (camAdjustments.Count > 0)
                    {
                        Vector2 camAdjust = camAdjustments[0].Update(pitch - prevPitch, yaw - prevYaw);
                        if (camAdjust.x != 0)
                            AddToPitch(camAdjust.x);
                        if (camAdjust.y != 0)
                            AddToYaw(camAdjust.y);
                        if (camAdjustments[0].Done())
                            camAdjustments.RemoveAt(0);
                    }
                }

                MoveCamera();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                if(ignoreMouse)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    ignoreMouse = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    ignoreMouse = true;
                }
            }
        }

        
        void MoveCamera()
        {
            //yaw += (Input.GetAxis("RightStickHorizontal")) * mouseSens;
            if (!ignoreInput)
            {
                if (ignoreMouse)
                    distanceUp -= ignorePitch ? 0 : Input.GetAxis("RightStickVertical") * 0.3f;
                else
                    distanceUp -= ignorePitch ? 0 : (Input.GetAxis("RightStickVertical") * 0.3f + (Input.GetAxis("Mouse Y") * .2f));
            }
            distanceUp = Mathf.Clamp(distanceUp, -6, 14);
            distanceAway = (startAway * (1f - (Mathf.Abs(startUp - distanceUp) / 10f)));
            distanceAway = Mathf.Clamp(distanceAway, startAway - 10f, startAway);
            Vector3 characterOffset = target.transform.position/* + ((PlayerManager.ptr.climbState == PlayerManager.ClimbState.NONE ? Vector3.up : player.up) * 0.25f)*/;

            lookDir = characterOffset - transform.position;
            lookDir.y = 0;
            lookDir.Normalize();

            //targetPos = player.position + player.up * distanceUp - player.forward * distanceAway;
            targetPos = characterOffset + Vector3.up * distanceUp - lookDir * distanceAway;

            //if (PlayerManager.ptr.climbState == PlayerManager.ClimbState.NONE && !PlayerManager.ptr.isGrounded)
            //{
            //    if (prevPlayerPos.y < player.position.y)
            //    {
            //        targetPos = new Vector3(targetPos.x, Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * 8), targetPos.z);
            //        timer += Time.deltaTime;
            //    }
            //    else if (timer > 0)
            //    {
            //        targetPos = new Vector3(targetPos.x, Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * 8), targetPos.z);
            //        timer -= Time.deltaTime;
            //    }
            //}
            //if(PlayerManager.ptr.isGrounded)
            //{
            //    timer = 0;
            //}

            //float pitchX = pitch;
            //float pitchZ = pitch;
            //Quaternion currRot = transform.rotation;
            //currRot.eulerAngles = new Vector3(0, 0, currRot.eulerAngles.z);
            //targetPos = RotatePointAroundPivot(targetPos, player.position, currRot * Quaternion.Euler(pitch, -(yaw - prevYaw), 0)); //Fix this fucking shit ya asshole
            //Debug.DrawRay(player.position, transform.right * distanceUp, Color.red);

            //transform.position = targetPos;
            //transform.RotateAround(player.position, transform.right, pitch);
            //transform.RotateAround(player.position, Vector3.up, yaw - prevYaw);
            //targetPos = transform.position;
            //transform.position = prevPosition;

            //Vector3 prevPosition = transform.position;
            //transform.position = targetPos;
            //transform.RotateAround(target.transform.position, Vector3.up, yaw - prevYaw);
            //targetPos = transform.position;
            //transform.position = prevPosition;

            //Debug.DrawRay(player.position, -1f * player.forward * distanceAway, Color.blue);
            //smoothPosition(transform.position, ref targetPos);
            //Debug.DrawLine(player.position, targetPos, Color.magenta);
            Vector3 temp = targetPos;
            targetPos = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * camFollowSpeed);
            CompensateForWalls(characterOffset, temp, ref targetPos);
            //CompensateForSides(ref targetPos);
            if (prevPlayerPos != player.position)
            {
                if(Physics.Raycast(targetPos + (Vector3.up * 0.1f), Vector3.down, 0.2f, 1, QueryTriggerInteraction.Ignore))
                {
                    distanceUp += Time.unscaledDeltaTime * 4f;
                }
            }
            transform.position = targetPos;
            transform.RotateAround(player.position, Vector3.up, yaw - prevYaw);
            float distance = Vector3.Distance(transform.position, player.position);
            float tempMin = 1f;
            float tempMax = 5f;
            //float amountToIncrease = (distance - tempMin <= 0) ? 0.5f : 3f - (distance < tempMax ? 2.5f - (((distance - tempMin) / (tempMax - tempMin)) * 2.5f) : 0);
            float amountToIncrease = (distance - tempMin <= 0) ? -1.75f : (distance < tempMax ? -1.75f + (((distance - tempMin) / (tempMax - tempMin)) * 1.75f) : 0);
            //Debug.LogError(amountToIncrease);
            transform.LookAt(characterOffset + (((PlayerManager.ptr.climbState == PlayerManager.ClimbState.NONE ? Vector3.up : player.up) * amountToIncrease)));
            //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);
            //ChangeTransparency();
            prevPitch = pitch;
            prevYaw = yaw;
            prevPlayerPos = player.position;
        }

        bool CompensateForWalls(Vector3 fromObject, Vector3 toTargetUnlerped, ref Vector3 toTarget)
        {
            //Player to Camera
            RaycastHit toTargetHit = new RaycastHit();
            bool didToTargetHit = Physics.Linecast(fromObject, toTarget, out toTargetHit, 1, QueryTriggerInteraction.Ignore);

            //Camera to Player
            RaycastHit fromTargetHit = new RaycastHit(); 
            bool didFromTargetHit = Physics.Linecast(toTarget, fromObject, out fromTargetHit, 1, QueryTriggerInteraction.Ignore);

            //If the player to camera cast hit
            if (didToTargetHit)
            {
                //Then if the camera's hitbox hit an object or the Camera to Player cast did not hit then move the camera because you might be inside a wall 
                if (Physics.CheckSphere(toTargetUnlerped, 0.3f, 1, QueryTriggerInteraction.Ignore) || Physics.CheckSphere(toTarget, 0.3f, 1, QueryTriggerInteraction.Ignore) || !didFromTargetHit)
                {
                    toTarget = toTargetHit.point;
                    return true;
                }
            }
            return false;
        }

        bool CompensateForSides(ref Vector3 toTarget)
        {
            int count = 0;
            
            RaycastHit rightSideHit = new RaycastHit();
            count += Physics.SphereCast(transform.position, 0.2f, transform.right, out rightSideHit, 0.3f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore) ? 1 : 0;

            
            RaycastHit leftSideHit = new RaycastHit();
            count += Physics.SphereCast(transform.position, 0.2f, -transform.right, out leftSideHit, 0.3f, Layers.ignoreLayersController, QueryTriggerInteraction.Ignore) ? 2 : 0;

            //1 - right side hit, 2 - left side hit, 3 - both hit
            switch (count)
            {
                case 1:
                    if(rightSideHit.distance < 0.2f)
                    {
                        toTarget += (-transform.right * (0.2f - rightSideHit.distance));
                    }
                    return true;
                case 2:
                    if (leftSideHit.distance < 0.2f)
                    {
                        toTarget += (transform.right * (0.2f - leftSideHit.distance));
                    }
                    return true;
                case 3:
                    float avg = (leftSideHit.distance + rightSideHit.distance) * 0.5f;
                    if(leftSideHit.distance > rightSideHit.distance)
                    {
                        toTarget += (-transform.right * (avg - rightSideHit.distance));
                    }
                    else if(leftSideHit.distance < rightSideHit.distance)
                    {
                        toTarget += (transform.right * (avg - leftSideHit.distance));
                    }
                    return true;
            }
            return false;
        }

        void smoothPosition(Vector3 fromPos, ref Vector3 toPos)
        {
            toPos = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
        }

        //Really need to figure out how to get this working correctly, might be needed for camera zones later
        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }               

        void ChangeTransparency()
        {
            if (Vector3.Distance(transform.position, player.position) <= 4f)
            {
                Color temp = playerMesh.sharedMaterial.color;
                temp.a = Mathf.Lerp(temp.a, 0.2f, Time.deltaTime * camFollowSpeed);
                playerMesh.sharedMaterial.color = temp;
            }
            else
            {
                if(playerMesh.sharedMaterial.color.a <= 0.99f)
                {
                    Color temp = playerMesh.sharedMaterial.color;
                    temp.a = Mathf.Lerp(temp.a, 1f, Time.deltaTime * camFollowSpeed);
                    playerMesh.sharedMaterial.color = temp;
                }
            }
        }

        private void OnDrawGizmos()
        {

        }
    }
}
[System.Serializable]
public class CameraAdjustment
{
    bool done = false;
    public Vector2 adjustmentValue;//x = pitch, y = yaw
    public float speed;
    Vector2 amount;
    public string state;
    public bool checkState;
    public CameraAdjustment(Vector2 adjVal, float s, string st = "")
    {
        adjustmentValue = adjVal;
        speed = s;
        state = st;
        checkState = st.Length > 0;
        amount = Vector2.zero;
    }

    public void UpdateValues(CameraAdjustment c)
    {
        adjustmentValue.x = c.adjustmentValue.x;
        adjustmentValue.y = c.adjustmentValue.y;
        speed = c.speed;
    }

    public Vector2 Update(float pitch, float yaw)
    {
        amount = Vector2.zero;

        if(Mathf.Abs(adjustmentValue.x) - pitch > 0)
            adjustmentValue.x -= pitch;
        else
            adjustmentValue.x = 0;

        if (Mathf.Abs(adjustmentValue.y) - yaw > 0)
            adjustmentValue.y -= yaw;
        else
            adjustmentValue.y = 0;

        if (Mathf.Abs(adjustmentValue.x) >= speed)
        {
            amount.x = speed;
        }
        else if (Mathf.Abs(adjustmentValue.x) >= speed * 0.5f)
        {
            amount.x = speed * 0.5f;
        }
        else
        {
            amount.x = Mathf.Abs(adjustmentValue.x);
        }

        if (Mathf.Abs(adjustmentValue.y) >= speed)
        {
            amount.y = speed;
        }
        else if (Mathf.Abs(adjustmentValue.y) >= speed * 0.5f)
        {
            amount.y = speed * 0.5f;
        }
        else
        {
            amount.y = Mathf.Abs(adjustmentValue.y);
        }
        amount.x *= Mathf.Sign(adjustmentValue.x);
        amount.y *= Mathf.Sign(adjustmentValue.y);
        adjustmentValue.x -= amount.x;
        adjustmentValue.y -= amount.y;
        //Debug.Log(adjustmentValue);
        if (adjustmentValue == Vector2.zero)
        {
            done = true;
        }
        return amount;
    }


    public bool Done()
    {
        return done;
    }
}