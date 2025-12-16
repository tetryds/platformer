using System.Collections;
using NUnit.Framework;
using Platformer.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using static Tests.Platformer.E2E.PlayerTools;

namespace Tests.Platformer.E2E
{
    public class TestMovement
    {
        private const string TestScene = "E2ETestScene";

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestWalk()
        {
            const float WalkThresholdScale = 0.8f;
            const float WalkDuration = 2f;

            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();

            // Gather initial info
            Vector2 initPos = player.transform.position;
            float distance = data.Status.Speed * WalkDuration;

            // Set inputs
            input.Horizontal = 1f;

            // Execute
            bool HasWalked()
            {
                float currentDistance = player.transform.position.x - initPos.x;
                return currentDistance >= distance * WalkThresholdScale;
            }

            yield return new WaitUntil(HasWalked);

            // Assert
            Assert.True(HasWalked(), "Player did not walk");
        }

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestDash()
        {
            const float DashThresholdScale = 0.9f;

            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();

            // Set up: Ensure player is going right
            input.Horizontal = 1f;
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            input.Horizontal = 0f;

            // Gather initial info
            Vector2 initPos = player.transform.position;
            float distance = data.Status.DashSpeed * data.Status.DashDuration;

            // Set inputs
            input.Dash = true;

            // Execute
            bool HasDashed()
            {
                float currentDistance = player.transform.position.x - initPos.x;
                return currentDistance >= distance * DashThresholdScale;
            }

            yield return new WaitUntil(HasDashed);

            // Assert
            Assert.True(HasDashed(), "Player did not dash");
        }

        [UnityTest]
        [Timeout(5_000)]
        public IEnumerator TestJump()
        {
            // Set up
            SceneManager.LoadScene(TestScene);
            yield return null;

            (var player, var data, var input) = PreparePlayer();

            // Set inputs
            input.Jump = true;

            // Execute and assert
            bool HasJumped()
            {
                return !data.Movement.IsGrounded.IsActive;
            }

            bool HasLanded()
            {
                return data.Movement.IsGrounded.IsActive;
            }

            yield return new WaitUntil(HasJumped);
            Assert.True(HasJumped(), "Player did not jump");
            
            input.Jump = false;

            yield return new WaitUntil(HasLanded);
            Assert.True(HasLanded(), "Player did not land");
        }
    }
}
