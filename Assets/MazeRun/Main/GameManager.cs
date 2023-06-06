using System;
using System.Collections.Generic;
using BlockyMapGen;
using DialogueSystem;
using MazeRun.Core;
using MazeRun.Level;
using MazeRun.UI.Hud;
using MazeRun.UI.MainMenu;
using UnityEngine;

namespace MazeRun.Main {
    public class GameManager : MonoBehaviour {

        public LevelManager levelManager;
        [SerializeField] MapGenerator mapGenerator;
        [SerializeField] PlayerMovement playerMovement;
        [SerializeField] CoreTime coreTime;
        [SerializeField] Canvas mainCanvas;
        [SerializeField] MainMenuDialogue mainMenu;
        [SerializeField] int reviveChance = 3;
        
        [NonSerialized] public GameProgress currentProgress = new() { active = false };
        [NonSerialized] public UserData.Data userData;
        [NonSerialized] public LevelInfo currentLevelInfo;
        
        public event Action OnProgressPointsUpdated = delegate {  };
        public event Action OnStart = delegate { };
        public event Action OnRevive = delegate { };
        public event Action OnLose = delegate { };
        public event Action OnWin = delegate {  };
        
        HudDialogue _hudDialogue;
        Queue<Vector3> _lastSafePos = new (3);
        Queue<Quaternion> _lastSafeRot = new (3);


        void Start() {
            currentLevelInfo = levelManager.LocalLevels[userData.level];
            mapGenerator.enabled = false;
            mapGenerator.onBlockReached += onBlockReached;
            
            mainMenu.OnPlay += StartGame;
            mainMenu.onReviveBtnClick += Revive;
            mainMenu.Show();
            
            playerMovement.onLose += Lose;
            playerMovement.Hide();
            currentProgress.startTime = DateTime.UtcNow;
        }

        void onBlockReached(Block block) {
            if (currentProgress.active) {
                currentProgress.points++;
                OnProgressPointsUpdated();
                if (currentLevelInfo.scoresToWin <= currentProgress.points) Win();
            }
            captureLastSafePlayerTransform();
        }

        void captureLastSafePlayerTransform() {
            _lastSafePos.Enqueue(playerMovement.transform.localPosition);
            _lastSafeRot.Enqueue(playerMovement.transform.localRotation);
            if (_lastSafePos.Count > reviveChance) {
                _lastSafePos.Dequeue();
                _lastSafeRot.Dequeue();
            }
        }

        void Update() {
            if (currentProgress.active) 
                updateGameRecord();
        }

        void updateGameRecord() {
            currentProgress.pos = playerMovement.transform.localPosition;
        }

        public void StartGame() {
            currentProgress = new GameProgress { active = true };

            mapGenerator.ResetMap();
            mapGenerator.enabled = true;

            playerMovement.enabled = true;
            playerMovement.transform.localPosition = Vector3.zero;
            playerMovement.transform.localRotation = Quaternion.identity;
            playerMovement.ShowUp();
            coreTime.ResetTime();
            coreTime.Resume();

            _hudDialogue = DialogueManager.Current.GetOrCreate<HudDialogue>( mainCanvas.transform );
            _hudDialogue.gameManager = this;
            mainMenu.Hide();
            OnStart();
        }

        public void Win() {
            _hudDialogue.Close();
            
            mainMenu.showLoseContainer = false;
            mainMenu.showWinContainer = true;
            mainMenu.showContinueOption = false;
            mainMenu.Show();
            
            coreTime.ResetTime();
            coreTime.Pause();
            
            playerMovement.enabled = false;
            playerMovement.transform.localPosition = Vector3.zero;
            playerMovement.transform.rotation = Quaternion.identity;
            playerMovement.Hide();
            
            mapGenerator.ResetMap();
            mapGenerator.enabled = false;
            
            OnWin();

            currentLevelInfo = levelManager.LocalLevels[userData.level];
        }
        
        public void Lose() {
            Debug.Log( $"Lost the game" );
            
            playerMovement.enabled = false;
            coreTime.Pause();
            
            mapGenerator.enabled = false;
            
            _hudDialogue.Close();
            
            mainMenu.showContinueOption = _lastSafePos.Count > 0;
            mainMenu.showWinContainer = false;
            mainMenu.showLoseContainer = true;
            mainMenu.Show();
            OnLose();
        }

        public void Revive() {
            Debug.Log( $"continuing game" );
            
            playerMovement.transform.localPosition = _lastSafePos.Dequeue();
            playerMovement.transform.localRotation = _lastSafeRot.Dequeue();
            playerMovement.enabled = true;
            playerMovement.Revive();

            mapGenerator.enabled = true;
            
            coreTime.Resume();
            
            mainMenu.Hide();
            _hudDialogue = DialogueManager.Current.GetOrCreate<HudDialogue>( mainCanvas.transform );
            _hudDialogue.gameManager = this;
            OnRevive();
        }
    }
}