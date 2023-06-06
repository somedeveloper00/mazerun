using System;
using System.Collections;
using UnityEngine;

namespace MazeRun.Data {
    public abstract class DataIO : ScriptableObject {
        public void Load<T>(Action<T> onLoaded, Action<Exception> onLoadError) {
            var handler = new GameObject( "DataIO coroutine handler" ).AddComponent<DataIOCoroutineHandlerComponent>();
            DontDestroyOnLoad( handler );
            handler.StartCoroutine( LoadValue<T>( value => {
                onLoaded?.Invoke( value );
                Destroy( handler.gameObject );
            }, e => {
                Debug.LogException( e );
                onLoadError?.Invoke( e );
            } ) );
        }

        public void Save<T>(T value, Action onSaved, Action<Exception> onSaveError) {
            var handler = new GameObject( "DataIO coroutine handler" ).AddComponent<DataIOCoroutineHandlerComponent>();
            DontDestroyOnLoad( handler );
            handler.StartCoroutine( SaveValue( value, () => onSaved?.Invoke(),
                e => {
                    Debug.LogException( e );
                    onSaveError?.Invoke( e );
                }) );
        }

        protected abstract IEnumerator LoadValue<T>(Action<T> onFinish, Action<Exception> onError);
        protected abstract IEnumerator SaveValue<T>(T value, Action onFinish, Action<Exception> onError);
    }
}