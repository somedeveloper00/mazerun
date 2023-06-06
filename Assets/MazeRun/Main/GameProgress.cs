using System;
using UnityEngine;

namespace MazeRun.Main {
    [Serializable]
    public struct GameProgress {
        public bool active;
        public long points;
        public DateTime startTime;
        public Vector3 pos;
    }
}