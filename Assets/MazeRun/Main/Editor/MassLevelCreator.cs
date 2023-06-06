using System.IO;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace MazeRun.Main.Editor {
    [CreateAssetMenu(menuName = "Mass Level Creator")]
    public class MassLevelCreator : ScriptableObject {
        public int count;
        public int startingOrder;
        public int scoresToWin;
        public string nameFormat = "Level {0}";

        [Button]
        void Create() {
            var path = EditorUtility.OpenFolderPanel( "Level", Application.dataPath + "/Resources", "." );
            path = path.Replace( Application.dataPath, "Assets" ); // make relative
            if (Directory.Exists( path )) {
                for (int i = 0; i < count; i++) {
                    var filePath = Path.Combine( path, string.Format( nameFormat, startingOrder + i ) + ".asset" );
                    if (File.Exists( filePath )) {
                        AssetDatabase.DeleteAsset( filePath );
                    }
                    var newLevel = CreateInstance<LevelInfo>();
                    newLevel.order = startingOrder + i;
                    newLevel.scoresToWin = scoresToWin;
                    AssetDatabase.CreateAsset( newLevel, filePath );
                }
            }
            AssetDatabase.Refresh();
        }
    }
}