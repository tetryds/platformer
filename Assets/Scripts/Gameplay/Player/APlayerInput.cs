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
    public abstract class APlayerInput : MonoBehaviour
    {
        public float Horizontal { get; protected set; }
        public bool Jump { get; protected set; }
        public bool Dash { get; protected set; }
    }
}
