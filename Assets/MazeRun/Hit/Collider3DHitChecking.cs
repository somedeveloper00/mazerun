using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace MazeRun.Hit {
    [RequireComponent(typeof(Collider))]
    [ExecuteAlways]
    public class Collider3DHitChecking : HitChecking {
        [NonSerialized] HashSet<HitTarget> _hits = new();

        [ShowInInspector] List<HitTarget> _debug_hits => _hits.ToList();

        void OnDisable() => _hits.Clear();

        void OnTriggerEnter(Collider other) {
            var hit = other.gameObject.GetComponentInChildren<HitTarget>();
            if (hit) _hits.Add( hit );
        }

        void OnCollisionEnter(Collision other) {
            var hit = other.gameObject.GetComponentInChildren<HitTarget>();
            if (hit) _hits.Add( hit );
        }

        void OnTriggerExit(Collider other) {
            var hit = other.gameObject.GetComponentInChildren<HitTarget>();
            if (hit) _hits.Remove( hit );
        }

        void OnCollisionExit(Collision other) {
            var hit = other.gameObject.GetComponentInChildren<HitTarget>();
            if (hit) _hits.Remove( hit );
        }


        protected override bool HitsAny(out HitTarget hitTarget) {
            hitTarget = _hits.FirstOrDefault();
            return hitTarget;
        }

        protected override bool Hits_Name(string name, out HitTarget hitTarget) {
            hitTarget = _hits.FirstOrDefault( hit => hit.name == name );
            return hitTarget;
        }

        protected override bool Hits_Tag(string tag, out HitTarget hitTarget) {
            hitTarget = _hits.FirstOrDefault( hit => hit.CompareTag( tag ) );
            return hitTarget;
        }

        protected override bool HitsTag_Name(string tag, string name, out HitTarget hitTarget) {
            hitTarget = _hits.FirstOrDefault( hit => hit.CompareTag( tag ) && hit.name == name );
            return hitTarget;
        }
    }
}