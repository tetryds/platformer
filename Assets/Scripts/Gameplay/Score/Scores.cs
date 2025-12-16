using Platformer.Tools;
using UnityEngine;

namespace Platformer.Gameplay
{
    [CreateAssetMenu(fileName = "Scores", menuName = "Platformer/Scores")]
    public class Scores : ScriptableObjectDict<ScoreEvent, int> { }
}
