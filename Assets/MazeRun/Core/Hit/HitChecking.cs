using TriInspector;
using UnityEngine;

namespace MazeRun.Core.Hit {
    public abstract class HitChecking : MonoBehaviour {
        
#if UNITY_EDITOR
        [ShowInInspector] bool _isHittingTarget => Hits( out _ );
#endif

        public abstract bool Hits(out HitTarget hitTarget);
    }
}