using UnityEditor;
using UnityEditor.UI;

namespace MazeRun.UI.Editor {
    [CustomEditor( typeof(DelayedButton) )]
    public class DelayedButtonEditor : ButtonEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "audioClip" ) );
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "volume" ) );
            EditorGUILayout.PropertyField( serializedObject.FindProperty( "delay" ) );
            serializedObject.ApplyModifiedProperties();
        }
    }
}