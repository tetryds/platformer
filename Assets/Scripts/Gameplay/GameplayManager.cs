using System;
using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] EnvironmentManager environment;
        [SerializeField] EnemyManager enemies;
        [SerializeField] ScoreManager score;
        [SerializeField] TokenManager token;

        [Header("Player")]
        [SerializeField] new CameraController camera;
        [SerializeField] PlayerController playerBase;

        StateMachine<GameState, GameEvent, Action<float>> stateMachine;

        PlayerController player;

        private void Awake()
        {
            stateMachine = new StateMachine<GameState, GameEvent, Action<float>>(GameState.Initialized)
                .AddState(GameState.Running, RunGame)
                .AddState(GameState.Won)
                .AddState(GameState.Failed)
                .AddTransition(GameEvent.Start, GameState.Initialized, GameState.Running, DoStart)
                .AddTransition(GameEvent.Win, GameState.Running, GameState.Won, DoWin)
                .AddTransition(GameEvent.Fail, GameState.Running, GameState.Failed, DoFail);

            stateMachine.StateChanged += s => Debug.Log($"Game state changed to '{s}'");

            environment.SuccessTriggered += _ => stateMachine.RaiseEvent(GameEvent.Win);
            environment.DeathTriggered += p => p.Kill();

            enemies.AddEnemies(FindObjectsByType<AEnemy>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            enemies.EnemyDied += _ => score.AddSccore(ScoreEvent.EnemyKilled);
            enemies.EnemyHitPlayer += HandlePlayerHit;

            token.AddTokens(FindObjectsByType<Token>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            token.TokenCollected += (t, p) => score.AddSccore(ScoreEvent.TokenCollected);
        }

        private void HandlePlayerHit(AEnemy enemy, PlayerController player)
        {
            if (player.State == PlayerState.Dashing)
                enemy.Kill();
            else
                player.Kill();
        }

        private void Start()
        {
            stateMachine.RaiseEvent(GameEvent.Start);
            SpawnPlayer();
        }

        private void FixedUpdate()
        {
            stateMachine.Behavior?.Invoke(Time.fixedDeltaTime);
        }

        private void SpawnPlayer()
        {
            if (player)
                Destroy(player);
            player = Instantiate(playerBase, environment.GetSpawnPosition(), environment.GetSpawnRotation());
            
            player.LivesExpired += () => stateMachine.RaiseEvent(GameEvent.Fail);
            player.Died += () => score.AddSccore(ScoreEvent.PlayerDied);
            player.Respawned += SendPlayerToSpawnLocation;

            camera.AssignPlayer(player);
        }

        private void SendPlayerToSpawnLocation()
        {
            player.Teleport(environment.GetSpawnPosition(), environment.GetSpawnRotation());
        }

        private void RunGame(float deltaTime)
        {
            player?.Tick(deltaTime);
            enemies.Tick(deltaTime);
            token.Tick(deltaTime);
        }

        private void DoStart()
        {
        }

        private void DoWin()
        {
            score.AddSccore(ScoreEvent.LevelCompleted);
            player.TriggerVictory();
        }

        private void DoFail()
        {
        }
    }

    public enum GameState
    {
        Initialized,
        Running,
        Won,
        Failed
    }

    public enum GameEvent
    {
        Start,
        Win,
        Fail
    }
}
