using UnityEngine;

namespace MazeRun.Core {
    public class CoreTime : MonoBehaviour {
        
        float _time = 0;
        float _deltaTime = 0;
        
        public float time => _time;
        public float deltaTime => _deltaTime;

        void Update() {
            _deltaTime = UnityEngine.Time.deltaTime;
            _time += _deltaTime;
        }

        public void ResetTime() => _time = 0;
    }
}