using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Used as an in-between with the animator parameters
    /// </summary>

    public class AnimHashes
    {
        public int speed = Animator.StringToHash("speed");
        public int jump = Animator.StringToHash("squ_jump");
        public int isGrounded = Animator.StringToHash("isGrounded");
        public int waitForAnimation = Animator.StringToHash("waitForAnimation");
        public int Land = Animator.StringToHash("squ_land");
        public int isLanding = Animator.StringToHash("isLanding");
        public int QuickLand = Animator.StringToHash("QuickLand");
        public int TimeSinceGrounded = Animator.StringToHash("TimeSinceGrounded");
        public int TimeSinceMove = Animator.StringToHash("TimeSinceMove");
        public int TimeSinceSlow = Animator.StringToHash("TimeSinceSlow");
        public int UpIdle = Animator.StringToHash("UpIdle");
        public int isGrinding = Animator.StringToHash("isGrinding");
        public int squ_grind_idle = Animator.StringToHash("squ_grind_idle");
        public int squ_grind_run = Animator.StringToHash("squ_grind_run");
        public int squ_grind_180 = Animator.StringToHash("squ_grind_180");
        public int isClimbing = Animator.StringToHash("isClimbing");
        public int squ_dash = Animator.StringToHash("squ_dash");
        public int squ_dash_air = Animator.StringToHash("squ_dash_air");

        public int npc_idle = Animator.StringToHash("NPC_IDLE");
        public int npc_run = Animator.StringToHash("NPC_WALK");
    }
}
