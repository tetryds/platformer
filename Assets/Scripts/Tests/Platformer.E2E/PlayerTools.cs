using NUnit.Framework;
using Platformer.Gameplay;
using Platformer.Tools.Reflection;
using UnityEngine;

namespace Tests.Platformer.E2E
{
    public static class PlayerTools
    {
        public static (PlayerController player, PlayerData data, MockInput input) PreparePlayer()
        {
            var player = GameObject.FindFirstObjectByType<PlayerController>();
            var hasData = player.TryGetValue<PlayerData>("data", out var data);
            Assert.True(hasData, "Player did not have data field or property");

            var input = player.gameObject.AddComponent<MockInput>();
            var hasMockInput = player.TrySetValue("input", input);
            Assert.True(hasMockInput, "Player did not accept mock input injection");

            return (player, data, input);
        }
    }
}
