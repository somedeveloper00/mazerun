using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TriInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MazeRun.Data {
    [CreateAssetMenu( fileName = "PersistentFileIO", menuName = "DataIO/PersistentFileIO", order = 0 )]
    public class PersistentPathFileIO : DataIO {
        [SerializeField] string filePath;
        [SerializeField] Serializer serializer;

        enum Serializer {
            BinarySerializer, JsonUtility
        }

#if UNITY_EDITOR
        [Button]
        void openFolder() {
            // open persistant folder
            var path = Application.persistentDataPath;
            EditorUtility.RevealInFinder( path );
        }

        [Button]
        void deleteFile() {
            File.Delete( Path.Combine( Application.persistentDataPath, filePath ) );
        }
#endif

        protected override IEnumerator LoadValue<T>(Action<T> onFinish, Action<Exception> onError) {
            var path = Path.Combine( Application.persistentDataPath, filePath );
            if (!File.Exists( path )) {
                onError( new FileNotFoundException( "File not found", path ) );
                yield break;
            }

            switch (serializer) {
                case Serializer.BinarySerializer: {
                    var task = File.ReadAllBytesAsync( path );
                    yield return new WaitUntil( () => task.IsCompleted );
                    var bytes = task.Result;
                    try {
                        var binaryFormatter = new BinaryFormatter();
                        var value = (T)binaryFormatter.Deserialize( new MemoryStream( bytes ) );
                        onFinish( value );
                    }
                    catch (Exception e) { onError( e ); }
                    break;
                }
                case Serializer.JsonUtility: {
                    var task = File.ReadAllTextAsync( path );
                    yield return new WaitUntil( () => task.IsCompleted );
                    try {
                        var result = JsonUtility.FromJson<T>( task.Result );
                        onFinish( result );
                    }
                    catch (Exception e) { onError( e ); }
                    break;
                }
                default:
                    onError(new ArgumentOutOfRangeException());
                    break;
            }
        }

        protected override IEnumerator SaveValue<T>(T value, Action onFinish, Action<Exception> onError) {
            switch (serializer) {
                case Serializer.BinarySerializer: {
                    var memoryStream = new MemoryStream();
                    try {
                        var binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize( memoryStream, value );
                    }
                    catch (Exception e) {
                        onError( e );
                        yield break;
                    }

                    var task = File.WriteAllBytesAsync( filePath, memoryStream.ToArray() );
                    yield return new WaitUntil( () => task.IsCompleted );
                    onFinish();
                    break;
                }
                case Serializer.JsonUtility: {
                    var data = string.Empty;
                    try { data = JsonUtility.ToJson( value ); }
                    catch (Exception e) {
                        onError( e );
                        yield break;
                    }

                    var task = File.WriteAllTextAsync( filePath, data );
                    yield return new WaitUntil( () => task.IsCompleted );
                    onFinish();
                    break;
                }
                default:
                    onError( new ArgumentOutOfRangeException() );
                    break;
            }
        }
    }
}