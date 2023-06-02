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

        public LevelInfo levelInfo;
        [SerializeField] MapGenerator mapGenerator;
        [SerializeField] PlayerMovement playerMovement;
        [SerializeField] CoreTime coreTime;
        [SerializeField] Canvas mainCanvas;
        [SerializeField] MainMenuDialogue mainMenu;
        
        public event Action OnProgressPointsUpdated = delegate {  };
        public event Action OnStart = delegate { };
        public event Action OnRevive = delegate { };
        public event Action OnLose = delegate { };
        public event Action OnWin = delegate {  };
        
        HudDialogue _hudDialogue;
        Vector3 _lastInputPlayerPos;
        Quaternion _lastInputPlayerRot;

        
        void Start() {
            mapGenerator.enabled = false;
            mapGenerator.onBlockReached += onBlockReached;
            
            mainMenu.OnPlay += StartGame;
            mainMenu.onReviveBtnClick += Revive;
            mainMenu.Show();
            
            playerMovement.onJump += captureLastPlayerTransform;
            playerMovement.onSlide += captureLastPlayerTransform;
            playerMovement.onRight += captureLastPlayerTransform;
            playerMovement.onLeft += captureLastPlayerTransform;
            playerMovement.onLose += Lose;
            playerMovement.Hide();
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere( _lastInputPlayerPos, 0.1f );
            Gizmos.DrawLine( _lastInputPlayerPos, _lastInputPlayerPos + _lastInputPlayerRot * Vector3.forward );
        }

        void captureLastPlayerTransform() {
            _lastInputPlayerPos = playerMovement.transform.position;
            _lastInputPlayerRot = playerMovement.transform.rotation;
        }

        void onBlockReached(Block block) {
            if (currentProgress.active) {
                currentProgress.points++;
                OnProgressPointsUpdated();
                if (levelInfo.scoresToWin <= currentProgress.points) {
                    Win();
                }
            }
        }

        void Update() {
            if (currentProgress.active) 
                updateGameRecord();
        }

        void updateGameRecord() {
            currentProgress.pos = playerMovement.transform.position;
        }

        public void StartGame() {
            currentProgress = new GameProgress { active = true };
            
            mapGenerator.ResetMap();
            mapGenerator.enabled = true;
            
            playerMovement.enabled = true;
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
        }
        
        public void Lose() {
            Debug.Log( $"Lost the game" );
            
            playerMovement.enabled = false;
            coreTime.Pause();
            
            mapGenerator.enabled = false;
            
            _hudDialogue.Close();
            
            mainMenu.showContinueOption = true;
            mainMenu.showWinContainer = false;
            mainMenu.showLoseContainer = true;
            mainMenu.Show();
            OnLose();
        }

        public void Revive() {
            Debug.Log( $"continuing game" );
            
            playerMovement.transform.localPosition = _lastInputPlayerPos;
            playerMovement.transform.localRotation = _lastInputPlayerRot;
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