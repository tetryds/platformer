using System;
using NUnit.Framework;
using Platformer.Gameplay;
using Platformer.Tools;
using Platformer.Tools.Reflection;
using UnityEngine;

namespace Tests.Platformer.E2E
{
    public class GameTools
    {
        public static (StateMachine<GameState, GameEvent, Action<float>> stateMachine, EnvironmentManager environment, EnemyManager enemies, ScoreManager score, TokenManager token) PrepareGame()
        {
            var game = GameObject.FindFirstObjectByType<GameplayManager>();

            var hasStateMachine = game.TryGetValue<StateMachine<GameState, GameEvent, Action<float>>>("stateMachine", out var stateMachine);
            Assert.True(hasStateMachine, "Game did not have environment");

            var hasEnvironment = game.TryGetValue<EnvironmentManager>("environment", out var environment);
            Assert.True(hasEnvironment, "Game did not have environment");
            var hasEnemies = game.TryGetValue<EnemyManager>("enemies", out var enemies);
            Assert.True(hasEnemies, "Game did not have enemies");
            var hasScore = game.TryGetValue<ScoreManager>("score", out var score);
            Assert.True(hasScore, "Game did not have score");
            var hasToken = game.TryGetValue<TokenManager>("token", out var token);
            Assert.True(hasToken, "Game did not have token");

            return (stateMachine, environment, enemies, score, token);


        }
    }
}
