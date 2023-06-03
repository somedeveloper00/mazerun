using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MazeRun.Data {
    [CreateAssetMenu( fileName = "PersistentFileIO", menuName = "DataIO/PersistentFileIO", order = 0 )]
    public class PersistentPathFileIO : DataIO {
        [SerializeField] string filePath;
        
        protected override IEnumerator LoadBytes(Action<byte[]> onFinish) {
            yield return new WaitForSeconds( 3 );
            if (!File.Exists( filePath )) {
                onFinish( null );
                yield break;
            }
            var task = File.ReadAllBytesAsync( filePath );
            yield return new WaitUntil( () => task.IsCompleted );
            onFinish( task.Result );
        }

        protected override IEnumerator SaveBytes(byte[] bytes, Action onFinish) {
            var task = File.WriteAllBytesAsync( filePath, bytes );
            yield return new WaitUntil( () => task.IsCompleted );
            onFinish();
        }
    }
}