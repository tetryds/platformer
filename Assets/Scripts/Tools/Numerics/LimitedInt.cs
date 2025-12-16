using System;
using UnityEngine;

namespace Platformer.Tools
{
    [Serializable]
    public class LimitedInt
    {
        [SerializeField] int value;
        [field: SerializeField] public int Max { get; private set; }

        public int Value { get => value; set => this.value = Mathf.Clamp(value, 0, Max); }
        public bool IsMaxed => Value == Max;

        public bool TryDecrement()
        {
            return Value-- > 0;
        }

        public bool TryIncrement()
        {
            return Value++ < Max;
        }
    }
}