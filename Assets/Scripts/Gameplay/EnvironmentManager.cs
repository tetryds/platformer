using System;
using System.Linq;
using NUnit;
using Platformer.Tools;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

namespace Platformer.Gameplay
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] Transform spawn;

        public event Action<PlayerController> SuccessTriggered;
        public event Action<PlayerController> DeathTriggered;

        private void Awake()
        {
            foreach (var trigger in FindObjectsByType<SuccessTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                trigger.TriggerEntered += p => SuccessTriggered(p);
            }

            foreach (var trigger in FindObjectsByType<DeathTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                trigger.TriggerEntered += p => DeathTriggered(p);
            }
        }

        public Vector2 GetSpawnPosition() => spawn.position;
        public Quaternion GetSpawnRotation() => spawn.rotation;
    }
}
