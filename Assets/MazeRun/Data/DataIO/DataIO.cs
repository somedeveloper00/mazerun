using System;
using System.Collections;
using UnityEngine;

namespace MazeRun.Data {
    public abstract class DataIO : ScriptableObject {
        public void Load(Action<byte[]> onLoaded, Action<Exception> onLoadError) {
            try {
                var handler = new GameObject( "DataIO coroutine handler" ).AddComponent<DataIOCoroutineHandlerComponent>();
                DontDestroyOnLoad( handler );
                handler.StartCoroutine( LoadBytes( bytes => {
                    onLoaded?.Invoke( bytes );
                    Destroy( handler.gameObject );
                } ) );
            }
            catch (Exception e) {
                Debug.LogException( e );
                onLoadError?.Invoke( e );
            }
        }

        public void Save(byte[] bytes, Action onSaved, Action<Exception> onSaveError) {
            try {
                var handler = new GameObject( "DataIO coroutine handler" ).AddComponent<DataIOCoroutineHandlerComponent>();
                DontDestroyOnLoad( handler );
                handler.StartCoroutine( SaveBytes( bytes, () => onSaved?.Invoke() ) );
            }
            catch (Exception e) {
                Debug.LogException( e );
                onSaveError?.Invoke( e );
            }
        }

        protected abstract IEnumerator LoadBytes(Action<byte[]> onFinish);
        protected abstract IEnumerator SaveBytes(byte[] bytes, Action onFinish);
    }
}