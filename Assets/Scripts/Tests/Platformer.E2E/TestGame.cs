using System.Collections;
using NUnit.Framework;
using Platformer.Gameplay;
using Platformer.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using static Tests.Platformer.E2E.PlayerTools;
using static Tests.Platformer.E2E.GameTools;

namespace Tests.Platformer.E2E
{
    public class TestGame
    {
        private const string TestScene = "E2ETestScene";

        private static readonly Vector2 NearMonsterLocation = new(5, -0.73f);
        private static readonly Vector2 AbovePitLocation = new(13.30f, 0f);
        private static readonly Vector2 AboveTokenLocation = new(6.54f, 1.31f);

        [UnityTest]
        [Timeout(2_000)]
        public IEnumerator TestWin()
        {
            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();
            (var stateMachine, var environment, var enemies, var score, var token) = PrepareGame();

            // Set inputs
            input.Horizontal = -1f;

            // Execute
            bool HasWon()
            {
                return stateMachine.Current == GameState.Won;
            }

            yield return new WaitUntil(HasWon);

            // Assert
            Assert.True(HasWon(), "Player did not win");
        }

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestKillMonster()
        {
            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();
            (var stateMachine, var environment, var enemies, var score, var token) = PrepareGame();

            player.Teleport(NearMonsterLocation);

            // Set up: Ensure player is going right
            input.Horizontal = 1f;
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            input.Horizontal = 0f;

            // Set inputs
            input.Dash = true;

            // Execute
            bool HasDied()
            {
                return !data.Resources.Lives.IsMaxed;
            }

            bool killed = false;
            enemies.EnemyDied += e => killed = true; 

            bool HasKilledMonster()
            {
                return killed;
            }

            yield return new WaitUntil(() => HasKilledMonster());

            // Assert
            Assert.False(HasDied(), "Player died");
            Assert.True(HasKilledMonster(), "Player did not kill monster");
        }

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestDieToMonster()
        {
            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();
            (var stateMachine, var environment, var enemies, var score, var token) = PrepareGame();
            
            player.Teleport(NearMonsterLocation);

            // Set inputs
            input.Horizontal = 1f;

            // Execute
            bool HasDied()
            {
                return !data.Resources.Lives.IsMaxed;
            }

            yield return new WaitUntil(() => HasDied());

            // Assert
            Assert.True(HasDied(), "Player did not die");
        }

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestDieToOutOfBounds()
        {
            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();
            (var stateMachine, var environment, var enemies, var score, var token) = PrepareGame();

            player.Teleport(AbovePitLocation);

            // Execute
            bool HasDied()
            {
                return !data.Resources.Lives.IsMaxed;
            }

            yield return new WaitUntil(() => HasDied());

            // Assert
            Assert.True(HasDied(), "Player did not die");
        }

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestCollectToken()
        {
            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();
            (var stateMachine, var environment, var enemies, var score, var token) = PrepareGame();
            
            player.Teleport(AboveTokenLocation);

            bool collected = false;
            token.TokenCollected += (t, p) => collected = true; 
            // Execute and assert
            bool HasCollected()
            {
                return collected;
            }

            Assert.False(HasCollected(), "Player had score before collecting token");

            yield return new WaitUntil(() => HasCollected());

            Assert.True(HasCollected(), "Player did not collect token");
        }
    }
}
