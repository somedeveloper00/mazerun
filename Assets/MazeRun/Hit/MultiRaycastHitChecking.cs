using System;
using UnityEngine;

namespace MazeRun.Hit {
    public class MultiRaycastHitChecking : HitChecking {
        public LayerMask layerMask;
        public RayInfo[] rayStarts = Array.Empty<RayInfo>();

        void OnDrawGizmosSelected() {
            Gizmos.color = Hits( out _ ) ? Color.green : Color.red;
            foreach (var rayStart in rayStarts) {
                var pos = transform.TransformPoint( rayStart.position );
                var dir = transform.TransformDirection( rayStart.direction );
                Gizmos.DrawLine( pos, pos + dir * rayStart.distance );
            }
        }

        [Serializable]
        public struct RayInfo {
            public Vector3 position;
            public Vector3 direction;
            public float distance;
        }

        protected override bool HitsAny(out HitTarget hitTarget) {
            return QueryRaycasts( out hitTarget, _ => true );
        }

        protected override bool Hits_Tag(string tag, out HitTarget hitTarget) {
            return QueryRaycasts( out hitTarget, hit => hit.CompareTag( tag ) );
        }

        protected override bool Hits_Name(string name, out HitTarget hitTarget) {
            return QueryRaycasts( out hitTarget, hit => hit.name == name );
        }

        protected override bool HitsTag_Name(string tag, string name, out HitTarget hitTarget) {
            return QueryRaycasts( out hitTarget, hit => hit.CompareTag( tag ) && hit.name == name );
        }

        bool QueryRaycasts(out HitTarget hitTarget, Func<HitTarget, bool> predicate) {
            foreach (var rayStart in rayStarts) {
                var pos = transform.TransformPoint( rayStart.position );
                var dir = transform.TransformDirection( rayStart.direction );

                if (!Physics.Raycast( pos, dir, out var hit, rayStart.distance, layerMask )) continue;
                if (!hit.transform.TryGetComponent<HitTarget>( out var target )) continue;

                if (predicate( target )) {
                    hitTarget = target;
                    return true;
                }
            }

            hitTarget = null;
            return false;
        }
    }
}