using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] int unknownEventScore;
        [SerializeField] Scores scores;

        [field: SerializeField] public int Score { get; private set; }

        public event Action<ScoreEvent, int> ScoreUpdated;

        public void AddSccore(ScoreEvent scoreEvent)
        {
            Debug.Log($"Add score event {scoreEvent}");

            Score += scores.GetOrDefault(scoreEvent, unknownEventScore);

            ScoreUpdated?.Invoke(scoreEvent, Score);
        }
    }
}
