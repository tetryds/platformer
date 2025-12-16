using System;
using UnityEngine;

namespace Platformer.Tools
{
    [Serializable]
    public class RangedInt
    {
        [SerializeField] int value;
        [field: SerializeField] public int Min { get; private set; }
        [field: SerializeField] public int Max { get; private set; }

        public int Value { get => value; set => this.value = Mathf.Clamp(value, Min, Max); }

        public bool TryDecrement()
        {
            return Value-- > Min;
        }

        public bool TryIncrement()
        {
            return Value++ < Max;
        }
    }
}