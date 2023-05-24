using System;
using UnityEngine;

namespace MazeRun.Core.Hit {
    public class MultiRaycastHitChecking : HitChecking {
        public LayerMask layerMask;
        public RayStart[] rayStarts = Array.Empty<RayStart>();

        void OnDrawGizmosSelected() {
            Gizmos.color = Hits( out _ ) ? Color.green : Color.red;
            foreach (var rayStart in rayStarts) {
                var pos = transform.TransformPoint( rayStart.position );
                var dir = transform.TransformDirection( rayStart.direction );
                Gizmos.DrawLine( pos, pos + dir * rayStart.distance );
            }
        }

        [Serializable]
        public struct RayStart {
            public Vector3 position;
            public Vector3 direction;
            public float distance;
        }

        public override bool Hits(out HitTarget hitTarget) {
            foreach (var rayStart in rayStarts) {
                var pos = transform.TransformPoint( rayStart.position );
                var dir = transform.TransformDirection( rayStart.direction );
                
                if (!Physics.Raycast( pos, dir, out var hit, rayStart.distance, layerMask )) continue;
                if (!hit.transform.TryGetComponent<HitTarget>( out var target )) continue;
                
                hitTarget = target;
                return true;
            }

            hitTarget = null;
            return false;
        }
    }
}