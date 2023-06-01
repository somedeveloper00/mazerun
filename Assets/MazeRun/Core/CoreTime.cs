using TriInspector;
using UnityEngine;

namespace MazeRun.Core {
    public class CoreTime : MonoBehaviour {
        
        float _time;
        float _deltaTime;
        bool _paused;
        
        [ShowInInspector] public float time => _time;
        [ShowInInspector] public float deltaTime => _deltaTime;

        void Update() {
            _deltaTime = Time.deltaTime;
            _time += _deltaTime;
        }

        public void ResetTime() => _time = 0;
        public void Pause() => _paused = true;
        public void Resume() => _paused = false;
    }
}