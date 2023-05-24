using System.ComponentModel;
using AnimFlex.Core.Proxy;
using UnityEngine;

namespace MazeRun {
    [DisplayName("Core Timescale")]
    public class AnimflexCoreProxyForMazeCoreTime : AnimflexCoreProxy {
        [SerializeField] AnimationCurve timescaleCurve;
        [SerializeField] bool setDefault = true;
        
        public static AnimflexCoreProxyForMazeCoreTime Default { get; private set; }
        
        void OnEnable() { if (setDefault) Default = this; }

        protected override float GetDeltaTime() {
            return timescaleCurve.Evaluate( Time.time ) * Time.deltaTime;
        }
    }
}