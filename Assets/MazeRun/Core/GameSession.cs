using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using MazeRun.Main;
using MazeRun.UI.Loading;
using UnityEngine;

namespace MazeRun.Core {
    public class GameSession : MonoBehaviour {
        public List<IEnumerator> GameStartRoutines { get; private set; } = new();
        public List<Action> OnGameQuit { get; private set; } = new();
        public GameManager gameManager;

        void OnValidate() {
            if (gameManager && gameManager.enabled) gameManager.enabled = false;
        }

        IEnumerator Start() {
            Application.targetFrameRate = 60;
            var loading = DialogueManager.Current.GetOrCreate<LoadingDialogue>();
            loading.text = "Starting Game";
            foreach (var routine in GameStartRoutines) yield return routine;
            loading.Close();
            gameManager.enabled = true;
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