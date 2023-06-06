using TriInspector;
using UnityEngine;

namespace MazeRun.Hit {
    [DeclareBoxGroup("debug", Title = "Debug")]
    public abstract class HitChecking : MonoBehaviour {

        [SerializeField] bool filterByTag;
        [EnableIf( nameof(filterByTag), true )] 
        [SerializeField] string filterTag;
        
        [SerializeField] bool filterByName;
        [EnableIf( nameof(filterByName), true )] 
        [SerializeField] string filterName;
        
#if UNITY_EDITOR
        [Group("debug")] [ShowInInspector] bool _isHittingTarget => Hits( out _ );
        [Group("debug")] [ShowInInspector] HitTarget _hitTarget => Hits( out var t ) ? t : null;
#endif

        public bool Hits(out HitTarget hitTarget) {
            if (filterByTag || filterByName) return HitsTag_Name( filterTag, filterName, out hitTarget );
            if (filterByTag) return Hits_Tag( filterTag, out hitTarget );
            if (filterByName) return Hits_Name( filterName, out hitTarget );
            return HitsAny( out hitTarget );
        }
        
        protected abstract bool HitsAny(out HitTarget hitTarget);
        protected abstract bool Hits_Tag(string tag, out HitTarget hitTarget);
        protected abstract bool Hits_Name(string name, out HitTarget hitTarget);
        protected abstract bool HitsTag_Name(string tag, string name, out HitTarget hitTarget);
    }
}