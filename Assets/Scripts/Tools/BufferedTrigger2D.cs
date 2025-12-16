using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Tools
{
    /// <summary>
    /// BufferedTrigger2D allows entities with multiple colliders to be counted as one when entering a trigger area.
    /// This does not execute any filtering.
    /// If the target entity entirely leaves and rejoins the trigger area, even if barely, it will trigger twice.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BufferedTrigger2D<T> : MonoBehaviour
    {
        protected Dictionary<T, int> passes = new();

        public event Action<T> TriggerEntered;

        public event Action<T> TriggerExited;

        private void OnTriggerEnter2D(Collider2D other)
        {
            T target = other.GetComponentInParent<T>();
            if (target == null) return;

            int count = passes.GetValueOrDefault(target, 0) + 1;
            passes[target] = count;
            if (count == 1)
            {
                OnBufferedTriggerEnter(target);
                TriggerEntered?.Invoke(target);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            T target = other.GetComponentInParent<T>();
            if (target == null) return;

            int count = passes[target] - 1;
            if (count == 0)
            {
                passes.Remove(target);
                OnBufferedTriggerExit(target);
                TriggerExited?.Invoke(target);
            }
            else
            {
                passes[target] = count;
            }
        }

        public virtual void OnBufferedTriggerEnter(T target) { }

        public virtual void OnBufferedTriggerExit(T target) { }
    }
}
