using System;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] Animator animator;

        int velocityX = Animator.StringToHash("velocityX");
        int velocityY = Animator.StringToHash("velocityY");
        int grounded = Animator.StringToHash("grounded");
        int hurt = Animator.StringToHash("hurt");
        int death = Animator.StringToHash("death");
        int respawn = Animator.StringToHash("respawn");
        int victory = Animator.StringToHash("victory");
        int dash = Animator.StringToHash("dash");

        public void Refresh(PlayerData data)
        {
            animator.SetFloat(velocityX, Mathf.Abs(data.Movement.Velocity.x));
            animator.SetFloat(velocityY, data.Movement.Velocity.y);
            animator.SetBool(grounded, data.Movement.IsGrounded.IsActive);

            //animator.SetBool(hurt, data.);
            //animator.SetBool(dead, data.);
            //animator.SetBool(victory, data.);
        }

        public void SetDash(bool dashing)
        {
            animator.SetBool(dash, dashing);
        }

        public void PlayHurt()
        {
            animator.SetTrigger(hurt);
        }

        public void PlayDeath()
        {
            animator.SetTrigger(death);
        }

        public void PlayRespawn()
        {
            animator.SetTrigger(respawn);
        }

        public void PlayVictory()
        {
            animator.SetTrigger(victory);
        }
    }
}
