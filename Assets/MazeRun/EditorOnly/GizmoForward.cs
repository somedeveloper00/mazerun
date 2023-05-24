using UnityEngine;

namespace MazeRun.Core.EditorOnly {
    public class GizmoForward : MonoBehaviour {
        [SerializeField] float length = 1;
        [SerializeField] Color color = Color.green;

        void OnDrawGizmos() {
            Gizmos.color = color;
            Gizmos.DrawRay( transform.position, transform.forward * length );
        }
    }
}