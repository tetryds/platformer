using System;
using Platformer.Mechanics;
using UnityEngine;

namespace Platformer.Gameplay
{
    public abstract class AEnemy : MonoBehaviour
    {
        [field: SerializeField] public bool IsAlive { get; protected set; } = true;

        public virtual void Initialize() { }
        public virtual void Tick(float deltaTime, float elapsed) { }
        public virtual void Despawn() { }

        public event Action<PlayerController> HitPlayer;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
                HitPlayer?.Invoke(player);
        }

        public void Kill()
        {
            IsAlive = false;
        }
    }
}
