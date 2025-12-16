using System;
using System.Collections.Generic;
using System.Linq;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Token : BufferedTrigger2D<PlayerController>
    {
        new SpriteRenderer renderer;

        [field: SerializeField]
        public Sprite[] IdleSprites { get; private set; }
        [field: SerializeField]
        public Sprite[] CollectedSprites { get; private set; }

        public Sprite Current { get => renderer.sprite; set => renderer.sprite = value; }

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void Despawn()
        {
            Destroy(gameObject);
        }
    }
}
