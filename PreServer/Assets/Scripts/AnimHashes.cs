using UnityEngine;
using System.Collections;

namespace PreServer
{
    /// <summary>
    /// Used as an in-between with the animator parameters
    /// </summary>

    public class AnimHashes
    {
        // Animator Parameters
        public int speed = Animator.StringToHash("speed");
        public int isGrounded = Animator.StringToHash("isGrounded");
        public int isLanding = Animator.StringToHash("isLanding");
        public int TimeSinceGrounded = Animator.StringToHash("TimeSinceGrounded");
        public int TimeSinceMove = Animator.StringToHash("TimeSinceMove");
        public int TimeSinceSlow = Animator.StringToHash("TimeSinceSlow");
        public int waitForAnimation = Animator.StringToHash("waitForAnimation");
        public int Land = Animator.StringToHash("squ_land");
        public int QuickLand = Animator.StringToHash("QuickLand");
        public int UpIdle = Animator.StringToHash("UpIdle");
        public int isGrinding = Animator.StringToHash("isGrinding");
        public int isClimbing = Animator.StringToHash("isClimbing");
        public int isDashing = Animator.StringToHash("isDashing");
        public int rotateFloat = Animator.StringToHash("rotateFloat");
        public int climbCorner = Animator.StringToHash("climbCorner");
        public int mirror180 = Animator.StringToHash("mirror180");
        public int isSliding = Animator.StringToHash("isSliding");

        // Squirrel Animations
        public int squ_idle = Animator.StringToHash("squ_idle");
        public int squ_grind_idle = Animator.StringToHash("squ_grind_idle");
        public int squ_grind_walk = Animator.StringToHash("squ_grind_walk");
        public int squ_grind_run = Animator.StringToHash("squ_grind_run");
        public int squ_grind_180 = Animator.StringToHash("squ_grind_180");
        public int squ_ground_180 = Animator.StringToHash("squ_ground_180");
        public int squ_ground_180_mirror = Animator.StringToHash("squ_ground_180_mirror");
        public int squ_dash = Animator.StringToHash("squ_dash");
        public int squ_dash_air = Animator.StringToHash("squ_dash_air");
        public int squ_climb_corner = Animator.StringToHash("squ_climb_corner");
        public int squ_climb_corner_acute = Animator.StringToHash("squ_climb_corner_acute");
        public int squ_jump = Animator.StringToHash("squ_jump");
        public int squ_sliding = Animator.StringToHash("squ_sliding");

        // NPC Animations
        public int npc_idle = Animator.StringToHash("NPC_IDLE");
        public int npc_run = Animator.StringToHash("NPC_RUN");
        public int npc_walk = Animator.StringToHash("NPC_WALK");
        public int npc_sit_idle = Animator.StringToHash("NPC_SIT");
    }
}
