using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEditor;
using UnityEngine;
namespace Platformer
{
    [CustomEditor(typeof(PatrolPath))]
    public class PatrolPathGizmo : Editor
    {
        public void OnSceneGUI()
        {
            var path = target as PatrolPath;
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                var startPosition = path.transform.InverseTransformPoint(Handles.PositionHandle(path.transform.TransformPoint(path.StartPosition), path.transform.rotation));
                var endPosition = path.transform.InverseTransformPoint(Handles.PositionHandle(path.transform.TransformPoint(path.EndPosition), path.transform.rotation));
                if (cc.changed)
                {
                    // Added undo support to patrol path
                    Undo.RecordObject(path, "Path position");
                    if (path.yLocked)
                    {
                        startPosition.y = 0;
                        endPosition.y = 0;
                    }
                    path.StartPosition = startPosition;
                    path.EndPosition = endPosition;
                }
            }
            Handles.Label(path.transform.position, (path.StartPosition - path.EndPosition).magnitude.ToString());
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        static void OnDrawGizmo(PatrolPath path, GizmoType gizmoType)
        {
            var start = path.transform.TransformPoint(path.StartPosition);
            var end = path.transform.TransformPoint(path.EndPosition);
            Handles.color = Color.yellow;
            Handles.DrawDottedLine(start, end, 5);
            Handles.DrawSolidDisc(start, path.transform.forward, 0.1f);
            Handles.DrawSolidDisc(end, path.transform.forward, 0.1f);
        }
    }
}
