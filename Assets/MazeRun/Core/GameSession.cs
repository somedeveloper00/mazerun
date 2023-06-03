using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using MazeRun.UI.Loading;
using UnityEngine;

namespace MazeRun.Core {
    public class GameSession : MonoBehaviour {
        public List<IEnumerator> GameStartRoutines { get; private set; } = new();
        public List<Action> OnGameQuit { get; private set; } = new();

        IEnumerator Start() {
            var loading = DialogueManager.Current.GetOrCreate<LoadingDialogue>();
            loading.text = "Starting Game";
            foreach (var routine in GameStartRoutines) yield return routine;
            loading.Close();
        }

        void OnDestroy() {
            foreach (var action in OnGameQuit) {
                try { action?.Invoke(); }
                catch (Exception e) {
                    Debug.LogException( e );
                }
            }
        }
    }
}