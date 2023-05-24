using UnityEngine;

namespace MazeRun.Core.Hit {
    [RequireComponent(typeof(Collider))]
    [ExecuteAlways]
    public class Collider3DHitChecking : HitChecking {
        bool _isHit = false;
        HitTarget _hitTarget;


        void OnTriggerEnter(Collider other) {
            _hitTarget = other.gameObject.GetComponentInChildren<HitTarget>();
            _isHit = _hitTarget;
        }
        
        void OnTriggerExit(Collider other) {
            var t = other.gameObject.GetComponentInChildren<HitTarget>();
            if (_hitTarget != t) return;
            _hitTarget = null;
            _isHit = false;
        }

        void OnCollisionEnter(Collision other) {
            _hitTarget = other.gameObject.GetComponentInChildren<HitTarget>();
            _isHit = _hitTarget;
        }

        void OnCollisionExit(Collision other) {
            var t = other.gameObject.GetComponentInChildren<HitTarget>();
            if (_hitTarget != t) return;
            _hitTarget = null;
            _isHit = false;
        }


        public override bool Hits(out HitTarget hitTarget) {
            hitTarget = _hitTarget;
            return _isHit;
        }
    }
}