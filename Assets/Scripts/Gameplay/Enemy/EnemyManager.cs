using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class EnemyManager : MonoBehaviour
    {
        readonly List<EnemyInfo> spawned = new();
        readonly LinkedList<EnemyInfo> active = new();
        readonly List<EnemyInfo> dead = new();

        public event Action<AEnemy> EnemyDied;
        public event Action<AEnemy, PlayerController> EnemyHitPlayer;

        public void AddEnemies(IEnumerable<AEnemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                AddEnemy(enemy);
            }
        }

        public void AddEnemy(AEnemy enemy)
        {
            spawned.Add(new EnemyInfo { Enemy = enemy, Lifetime = 0f });
        }

        public void Tick(float deltaTime)
        {
            dead.Clear();

            foreach (var enemyInfo in spawned)
            {
                AEnemy enemy = enemyInfo.Enemy;

                enemy.Initialize();
                enemy.HitPlayer += p => EnemyHitPlayer?.Invoke(enemy, p);

                active.AddLast(enemyInfo);
            }

            foreach (var enemyInfo in active)
            {
                AEnemy enemy = enemyInfo.Enemy;

                enemyInfo.Lifetime += deltaTime;
                enemy.Tick(deltaTime, enemyInfo.Lifetime);

                if (!enemy.IsAlive)
                    dead.Add(enemyInfo);
            }

            foreach (var enemyInfo in dead)
            {
                AEnemy enemy = enemyInfo.Enemy;

                enemy.Despawn();

                active.Remove(enemyInfo);
                EnemyDied?.Invoke(enemy);
            }

            spawned.Clear();
        }

        private record EnemyInfo
        {
            public AEnemy Enemy;
            public float Lifetime;
        }
    }
}
