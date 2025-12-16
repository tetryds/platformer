using System;
using UnityEngine;

namespace Platformer.Tools
{
    /// <summary>
    /// Sync timer that accumulates tick time and invokes callback synchronously when a given period is elapsed.
    /// </summary>
    [Serializable]
    public class SyncTimer
    {
        /// <summary>
        /// Period for expiring this timer.
        /// After the period is elapsed, the timeout will trigger.
        /// Timeout can trigger more than once per tick.
        /// </summary>
        /// 
        [SerializeField] float period;

        /// <summary>
        /// Timeout event that will be invoked every time the period is ellapsed.
        /// </summary>
        public event Action Timeout;

        /// <summary>
        /// Time elapsed on this timer since last timeout
        /// </summary>
        [field: SerializeField] public float Elapsed { get; private set; }

        /// <summary>
        /// Creates a SyncTimer instance.
        /// </summary>
        /// <param name="period">Period of time between timeouts. If this period is 0 or lower, expires every tick.</param>
        /// <param name="initialElapsed">Sets initial elapsed time for the timer.</param>
        public SyncTimer(float period, float initialElapsed)
        {
            this.period = period < 0f ? 0f : period;
            Elapsed = initialElapsed;
        }

        /// <summary>
        /// Resets the elapsed time of the timer
        /// </summary>
        public void Reset() => Elapsed = 0f;

        /// <summary>
        /// Advances timer by deltaTime. If the internal timer has expired, invokes Timeout callback.
        /// If the deltaTime is larger than the period, Timeout can be invoked more than once per Tick.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds. Has no effect if zero or lower</param>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0) return;

            if (period == 0)
            {
                Timeout?.Invoke();
                return;
            }
            else
            {
                Elapsed += deltaTime;

                while (Elapsed >= period)
                {
                    Elapsed -= period;
                    Timeout?.Invoke();
                }
            }
        }
    }
}
