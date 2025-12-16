using System;
using UnityEngine;

namespace Platformer.Tools
{
    [Serializable]
    public class LimitedFloat
    {
        [SerializeField] float value;
        [field: SerializeField] public float Max { get; private set; }

        public float Value { get => value; set => this.value = Mathf.Clamp(value, 0, Max); }

        public float Increment(float delta)
        {
            float init = Value;
            Value += delta;
            return Value - init;
        }
    }
}