using System.IO;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class PatrolPath : MonoBehaviour
    {
        public bool yLocked = false;

        [field: SerializeField] public Vector2 StartPosition { get; set; }
        [field: SerializeField] public Vector2 EndPosition { get; set; }

        public Vector2 GetTarget(float distance)
        {
            var startPos = transform.TransformPoint(StartPosition);
            var endPos = transform.TransformPoint(EndPosition);

            float pathLength = (endPos - startPos).magnitude;
            if (pathLength == 0f) return startPos;

            float delta = Mathf.PingPong(distance, pathLength) / pathLength;

            return Vector2.Lerp(startPos, endPos, delta);
        }

        public void Reset()
        {
            StartPosition = Vector3.left;
            EndPosition = Vector3.right;
        }
    }
}