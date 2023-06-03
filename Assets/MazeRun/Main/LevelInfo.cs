using UnityEngine;

namespace MazeRun.Main {
    [CreateAssetMenu( fileName = "mazerun/LevelInfo", menuName = "LevelInfo", order = 0 )]
    public class LevelInfo : ScriptableObject {
        public int order = 0;
        public int scoresToWin = 100;
    }
}