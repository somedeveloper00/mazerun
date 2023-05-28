using System.ComponentModel;
using AnimFlex.Core.Proxy;
using MazeRun.Core;
using MazeRun.Utils;
using UnityEngine;

namespace MazeRun {
    [DisplayName("Core Timescale")]
    public class AnimflexCoreProxyForMazeCoreTime : AnimflexCoreProxy {
        [SerializeField] AnimationCurve timescaleCurve;
        [SerializeField] bool setDefault = true;
        [SerializeField] CoreTime coreTime;
        
        public static AnimflexCoreProxyForMazeCoreTime Default { get; private set; }
        
        void OnEnable() { if (setDefault) Default = this; }

        protected override float GetDeltaTime() => timescaleCurve.Evaluate( coreTime.time ) * coreTime.deltaTime;
    }
}