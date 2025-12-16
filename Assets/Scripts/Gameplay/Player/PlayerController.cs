using System;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        const float LookSwitchThreshold = 0.01f;

        [SerializeField] new Rigidbody2D rigidbody;
        [SerializeField] SpriteRenderer sprite;
        [SerializeField] PlayerAnimator animator;
        [SerializeField] PlayerVfx vfx;
        [SerializeField] APlayerInput input;

        [SerializeField] PlayerData data;

        StateMachine<PlayerState, PlayerEvent, Action<float>> stateMachine;

        // Time elapsed at the current state
        float elapsed = 0f;

        public PlayerState State => stateMachine.Current;

        public event Action Teleported;
        public event Action Respawned;
        public event Action Died;
        public event Action LivesExpired;

        private void Awake()
        {
            stateMachine = new StateMachine<PlayerState, PlayerEvent, Action<float>>(PlayerState.Moving, TickMove)
                .AddState(PlayerState.Dashing, TickDash)
                .AddState(PlayerState.Dead, TickDead)
                .AddState(PlayerState.Won)
                .AddTransition(PlayerEvent.Dash, PlayerState.Moving, PlayerState.Dashing, OnDash)
                .AddTransition(PlayerEvent.EndDash, PlayerState.Dashing, PlayerState.Moving, OnEndDash)
                .AddTransition(PlayerEvent.Respawn, PlayerState.Dead, PlayerState.Moving, OnRespawn)
                .AddGlobalTransition(PlayerEvent.Die, PlayerState.Dead, OnDeath)
                .AddGlobalTransition(PlayerEvent.Win, PlayerState.Won, OnWin);

            stateMachine.StateChanged += _ => elapsed = 0f;

            stateMachine.StateChanged += s => Debug.Log($"Player state changed to '{s}'");

            data.Resources.DashTimer.Timeout += () => data.Resources.Dashes.TryIncrement();
        }

        private void Update()
        {
            UpdateInputs();

            if (data.Input.Dash && data.Resources.Dashes.TryDecrement())
            {
                stateMachine.RaiseEvent(PlayerEvent.Dash);
            }

            ApplyTicks(Time.deltaTime);

            animator.Refresh(data);
        }

        public void TriggerVictory()
        {
            stateMachine.RaiseEvent(PlayerEvent.Win);
        }

        public void Kill()
        {
            Debug.Log("Instakill");
            stateMachine.RaiseEvent(PlayerEvent.Die);
        }

        public void Teleport(Vector2 position, Quaternion? rotation = null)
        {
            transform.SetPositionAndRotation(position, rotation ?? Quaternion.identity);
            Teleported?.Invoke();
        }

        private void UpdateInputs()
        {
            data.Input.Horizontal = input.Horizontal;
            data.Input.Dash = input.Dash;

            var jump = data.Input.Jump;
            if (input.Jump)
                jump.ForceSet(true);
            jump.Set(false);
        }

        private void ApplyTicks(float deltaTime)
        {
            if (data.Resources.Dashes.IsMaxed)
                data.Resources.DashTimer.Reset();
            else
                data.Resources.DashTimer.Tick(deltaTime);

            data.Input.Jump.Tick(deltaTime);
            data.Movement.IsGrounded.Tick(deltaTime);
        }

        public void Tick(float deltaTime)
        {
            elapsed += deltaTime;
            stateMachine.Behavior(deltaTime);
        }

        private void TickMove(float deltaTime)
        {
            Vector2 localVel = GetLocalVelocity();
            var isGrounded = data.Movement.IsGrounded;

            UpdatePhysics(localVel);
            UpdateIsGrounded(isGrounded);

            localVel = GetLateralVelocity(deltaTime, localVel);
            localVel = GetVerticalVelocity(deltaTime, localVel);

            UpdateMovement(deltaTime, localVel);
            UpdateVisual();

            rigidbody.MovePosition(data.Movement.Position);
        }

        private void TickDash(float deltaTime)
        {
            Vector2 localVel = GetLocalVelocity();
            var isGrounded = data.Movement.IsGrounded;

            UpdatePhysics(localVel);
            UpdateIsGrounded(isGrounded);

            localVel.y = 0f;
            localVel.x = data.Movement.LookingRight ? data.Status.DashSpeed : -data.Status.DashSpeed;

            UpdateMovement(deltaTime, localVel);
            UpdateVisual();

            rigidbody.MovePosition(data.Movement.Position);

            if (elapsed > data.Status.DashDuration)
                stateMachine.RaiseEvent(PlayerEvent.EndDash);
        }

        private void TickDead(float deltaTime)
        {
            if (elapsed > data.Status.RespawnTime)
                stateMachine.RaiseEvent(PlayerEvent.Respawn);
        }

        private void UpdateMovement(float deltaTime, Vector2 localVel)
        {
            Vector2 globalVel = ConvertLocalToGlobalVelocity(localVel);

            data.Movement.Velocity = globalVel;
            Vector2 globalDelta = globalVel * deltaTime;
            data.Movement.Position = globalDelta + rigidbody.position;

            if (localVel.x > LookSwitchThreshold)
                data.Movement.LookingRight = true;
            else if (localVel.x < -LookSwitchThreshold)
                data.Movement.LookingRight = false;
        }

        private void UpdateVisual()
        {
            transform.up = data.Environment.UpDir;

            sprite.flipX = !data.Movement.LookingRight;
        }

        private Vector2 GetLateralVelocity(float deltaTime, Vector2 localVel)
        {
            float targetLateralVel = data.Input.Horizontal * data.Status.Speed;
            localVel.x += (targetLateralVel - localVel.x) * data.Status.MoveGain * deltaTime;
            return localVel;
        }

        private Vector2 GetVerticalVelocity(float deltaTime, Vector2 localVel)
        {
            var isGrounded = data.Movement.IsGrounded;
            var jump = data.Input.Jump;

            // If ceiling is hit, we can only fall
            // This also handles getting stuck with a tight ceiling
            if (data.Physics.HitCeiling)
            {
                localVel.y = Mathf.Clamp(localVel.y, float.MinValue, 0f);
            }
            else
            {
                if (isGrounded.IsActive && jump.IsActive)
                {
                    localVel.y = data.Status.JumpForce;
                    isGrounded.ForceSet(false);
                    jump.ForceSet(false);
                }
            }

            // Clamp velocity to zero to prevent gravity from stacking up
            // Use grounded target to prevent coyote jump from keeping the player floating
            if (isGrounded.Target)
                localVel.y = Mathf.Clamp(localVel.y, 0f, float.MaxValue);
            else
                localVel += data.Environment.Gravity * deltaTime;
            return localVel;
        }

        private void UpdateIsGrounded(DelayedFlag isGrounded)
        {
            // Force set on touchdown, delay set on liftoff for coyote effect
            // This also helps with jumping behavior on uneven terrain
            if (data.Physics.HitGround)
                isGrounded.ForceSet(true);
            else
                isGrounded.Set(false);
        }

        private Vector2 ConvertLocalToGlobalVelocity(Vector2 localVel)
        {
            var rot = Quaternion.FromToRotation(Vector2.up, data.Environment.UpDir);
            Vector2 globalVel;
            globalVel = rot * localVel;
            return globalVel;
        }

        private Vector2 GetLocalVelocity()
        {
            var localRot = Quaternion.FromToRotation(data.Environment.UpDir, Vector2.up);
            Vector2 globalVel = data.Movement.Velocity;
            Vector2 localVel = localRot * globalVel;
            return localVel;
        }

        private void UpdatePhysics(Vector2 localVel)
        {
            Environment environment = data.Environment;

            Physics physics = data.Physics;
            physics.HitCeiling = localVel.y >= 0f && rigidbody.IsContacted(-environment.UpDir);
            physics.HitGround = localVel.y <= 0f && rigidbody.IsContacted(environment.UpDir);
        }

        private void OnDash()
        {
            animator.SetDash(true);
            vfx.SetTrail(true);
        }

        private void OnEndDash()
        {
            animator.SetDash(false);
            vfx.SetTrail(false);
        }

        private void OnDeath()
        {
            LimitedInt lives = data.Resources.Lives;
            lives.Value -= 1;

            data.Movement.Velocity = Vector2.zero;
            rigidbody.linearVelocity = Vector2.zero;
            rigidbody.MovePosition(transform.position);

            animator.PlayDeath();
            Died?.Invoke();
            if (lives.Value == 0)
            {
                LivesExpired?.Invoke();
            }
        }

        private void OnWin()
        {
            animator.SetDash(false);
            animator.PlayVictory();
        }

        private void OnRespawn()
        {
            Debug.Log("Respawn");
            animator.PlayRespawn();
            Respawned?.Invoke();
        }
    }

    public enum PlayerEvent
    {
        Dash,
        EndDash,
        GetHit,
        Die,
        Respawn,
        Win,
    }

    public enum PlayerState
    {
        Moving,
        Dashing,
        Dead,
        Won
    }
}
