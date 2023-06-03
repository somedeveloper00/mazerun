using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using MazeRun.Data;
using MazeRun.Main;
using MazeRun.UI.Loading;
using UnityEngine;

namespace MazeRun.Level {
    public class LevelManager : MonoBehaviour {

        public List<LevelInfo> LocalLevels = new();
        
        [SerializeField] DataIO localDataIo;

        void Start() {
            loadLocalLevels();
        }

        void loadLocalLevels() {
            var loading = DialogueManager.Current.GetOrCreate<LoadingDialogue>();
            loading.text = "Loading levels";
            localDataIo.Load( bytes => {
                var stringData = System.Text.Encoding.UTF8.GetString( bytes );
                var levels = JsonUtility.FromJson<List<LevelInfo>>( stringData );
                LocalLevels = levels.Where( lvl => lvl != null ).OrderBy( lvl => lvl.order ).ToList();
                loading.Close();
            }, null );
        }
    }
}