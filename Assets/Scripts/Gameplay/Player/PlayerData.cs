using System;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    [Serializable]
    public record PlayerData
    {
        public Environment Environment;
        public Resources Resources;
        public Physics Physics;
        public Status Status;
        public Movement Movement;
        public Input Input;
    };

    [Serializable]
    public record Environment
    {
        public Vector2 Gravity;
        public float HitSlopeDeg;

        public float HitSlopeRad => Mathf.Deg2Rad * HitSlopeDeg;
        public Vector2 UpDir => -Gravity.normalized;
        public Vector2 LatDir => UpDir.Rotate(-90f).normalized;
    };

    [Serializable]
    public record Resources
    {
        public LimitedInt Lives;
        public LimitedInt Dashes;
        public SyncTimer DashTimer;
    };

    [Serializable]
    public record Physics
    {
        public bool HitGround;
        public bool HitCeiling;
    };

    [Serializable]
    public record Status
    {
        public float RespawnTime;
        public float Speed;
        public float MoveGain;
        public float JumpForce;
        public float DashDuration;
        public float DashSpeed;
    };

    [Serializable]
    public record Movement
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool LookingRight;
        // Delayed isgrounded enables coyote jumping
        public DelayedFlag IsGrounded;
    };

    [Serializable]
    public record Input
    {
        public float Horizontal;
        public DelayedFlag Jump;
        public bool Dash;
    };

}
