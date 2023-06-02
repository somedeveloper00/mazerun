using System;
using BlockyMapGen;
using TriInspector;
using UnityEngine;

namespace MazeRun.Core {
    public class CoreTime : MonoBehaviour {
        
        [SerializeField] BlockyMapGenTime blocksTime;
        
        [NonSerialized] float _time;
        [NonSerialized] float _deltaTime;
        [NonSerialized] bool _paused;
        
        [ShowInInspector] public float time => _time;
        [ShowInInspector] public float deltaTime => _deltaTime;

        void Update() {
            if (_paused) {
                _deltaTime = blocksTime.DeltaTime = 0;
                return;
            }
            _deltaTime = Time.deltaTime;
            _time += _deltaTime;
            blocksTime.Time = _time;
            blocksTime.DeltaTime = _deltaTime;
        }

        public void ResetTime() => blocksTime.Time = _time = blocksTime.DeltaTime = _deltaTime = 0;
        public void Pause() => _paused = true;
        public void Resume() => _paused = false;
    }
}