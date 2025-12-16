using System;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class PlayerVfx : MonoBehaviour
    {
        [SerializeField] GameObject trail;

        public void SetTrail(bool enabled)
        {
            trail.SetActive(enabled);
        }
    }
}
