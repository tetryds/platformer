using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class TokenManager : MonoBehaviour
    {
        readonly List<TokenInfo> spawned = new();
        readonly LinkedList<TokenInfo> active = new();
        readonly List<TokenInfo> collected = new();

        public event Action<Token, PlayerController> TokenCollected;

        public void AddTokens(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                AddEnemy(token);
            }
        }

        public void AddEnemy(Token token)
        {
            spawned.Add(new TokenInfo { Token = token, AnimationFrame = (int)UnityEngine.Random.value, Collected = false });
        }

        public void Tick(float deltaTime)
        {
            collected.Clear();

            foreach (var tokenInfo in spawned)
            {
                Token token = tokenInfo.Token;

                token.TriggerEntered += _ =>
                {
                    tokenInfo.Collected = true;
                    tokenInfo.ResetSprite();
                };
                token.TriggerEntered += p => TokenCollected(token, p);

                active.AddLast(tokenInfo);
            }

            foreach (var tokenInfo in active)
            {
                Token token = tokenInfo.Token;
                var sprites = tokenInfo.Sprites;

                tokenInfo.AnimationFrame = (tokenInfo.AnimationFrame + 1) % sprites.Length;
                token.Current = tokenInfo.Current;
                if (tokenInfo.Collected && tokenInfo.IsLastSprite())
                    collected.Add(tokenInfo);
            }

            foreach (var tokenInfo in collected)
            {
                // No need to deassign events
                tokenInfo.Token.Despawn();

                active.Remove(tokenInfo);
            }

            spawned.Clear();
        }

        private record TokenInfo
        {
            public Token Token;
            public bool Collected;
            public int AnimationFrame;

            public Sprite[] Sprites => Collected ? Token.CollectedSprites : Token.IdleSprites;
            public Sprite Current => Sprites[AnimationFrame];

            public void AdvanceSprite()
            {
                AnimationFrame = (AnimationFrame + 1) % Sprites.Length;
            }

            public bool IsLastSprite() => AnimationFrame == Sprites.Length - 1;

            public void ResetSprite() => AnimationFrame = 0;
        }
    }
}
