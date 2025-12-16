using System;
using UnityEngine;

namespace Platformer.Tools
{
    /// <summary>
    /// Flag that holds its value for up to a given amount of time.
    /// </summary>
    [Serializable]
    public class HoldFlag
    {
        [field: SerializeField] public bool Target { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] private bool isActive;

        public float Elapsed { get; private set; }

        public bool IsActive
        {
            get => isActive; private set
            {
                if (value != isActive)
                {
                    Changed?.Invoke(isActive);
                    isActive = value;
                }
            }
        }

        public event Action<bool> Changed;

        public void ForceSet(bool active)
        {
            Elapsed = 0f;
            Target = active;
            IsActive = active;
        }

        public void Set(bool active)
        {
            // Target is already set, ignore it
            if (active == Target) return;

            Elapsed = 0f;
            Target = active;
        }

        public void Tick(bool active, float deltaTime)
        {
            if (!active)
            {
                IsActive = false;
                Elapsed = 0f;
            }

            Elapsed += deltaTime;
            if (Elapsed >= Delay)
            {
                IsActive = false;
                Elapsed = 0f;
            }
            else
            {
                IsActive = true;
            }
        }
    }
}