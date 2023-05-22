using TriInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MazeRun.Core {
    public class Block : MonoBehaviour {
        [Title( "References" )] 
        public Opening[] openings;
        public Bounds bounds;
        
        void OnDrawGizmos() {
            Handles.color = Color.blue;
            Handles.DrawWireCube( transform.TransformPoint( bounds.center ), bounds.size );
        }

        public bool WillIntersectWithPrefabAtPos(Block prefab, Vector3 pos) {
            var bound1 = new Bounds( transform.TransformPoint( bounds.center ), bounds.size );
            var bound2 = new Bounds( prefab.bounds.center + pos, prefab.bounds.size );
            return bound1.Intersects( bound2 ) || bound1 == bound2;
        }
    }
}