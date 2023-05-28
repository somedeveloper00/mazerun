using System;
using BlockyMapGen;
using DialogueSystem;
using MazeRun.Core;
using MazeRun.UI.Hud;
using MazeRun.UI.MainMenu;
using UnityEngine;

namespace MazeRun.Main {
    public class GameManager : MonoBehaviour {

        public GameProgress currentProgress = new() { active = false };
        
        [SerializeField] MapGenerator mapGenerator;
        [SerializeField] Runner runnerPrefab;
        [SerializeField] Canvas mainCanvas;
        
        public event Action<Runner> OnRunnerSpawned = delegate {  };
        public event Action OnProgressPointsUpdated = delegate {  };
        
        Runner _currentRunner;

        void Start() {
            openMainMenu();
            mapGenerator.enabled = false;
            mapGenerator.onBlockReached += onBlockReached;
        }

        void onBlockReached(Block block) {
            if (currentProgress.active) {
                currentProgress.points++;
                OnProgressPointsUpdated();
            }
        }

        void Update() {
            if (currentProgress.active) 
                updateGameRecord();
        }

        void updateGameRecord() {
            currentProgress.pos = _currentRunner.transform.position;
        }

        void openMainMenu() {
            var dialogue = DialogueManager.Current.GetOrCreate<MainMenu>( mainCanvas.transform );
            dialogue.gameManager = this;
        }

        public void StartGame() {
            currentProgress = new GameProgress { active = true };
            mapGenerator.ResetMap();
            _currentRunner = Instantiate( runnerPrefab, Vector3.zero, Quaternion.identity );
            mapGenerator.target = _currentRunner.GetComponentInChildren<MapTarget>();
            mapGenerator.enabled = true;
            
            var hud = DialogueManager.Current.GetOrCreate<HudDialogue>( mainCanvas.transform );
            hud.gameManager = this;

            OnRunnerSpawned( _currentRunner );
        }
    }
}