using System;
using UnityEngine;

namespace Platformer.Tools
{
    /// <summary>
    /// Flag that holds its value for a given amount of time.
    /// Can be forced or overridden
    /// </summary>
    [Serializable]
    public class DelayedFlag
    {
        [field: SerializeField] public bool Target { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public bool IsActive { get; private set; }

        float elapsed = 0f;

        public void ForceSet(bool active)
        {
            elapsed = 0f;
            Target = active;
            IsActive = active;
        }

        public void Set(bool active)
        {
            // Target is already set, ignore it
            if (active == Target) return;

            elapsed = 0f;
            Target = active;
        }

        public void Restart() => Delay = 0f;

        public void Tick(float deltaTime)
        {
            if (IsActive == Target) return;

            elapsed += deltaTime;
            if (elapsed >= Delay)
            {
                IsActive = Target;
                elapsed = 0f;
            }
        }
    }
}