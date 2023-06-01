using System;
using System.Threading.Tasks;
using BlockyMapGen;
using DialogueSystem;
using MazeRun.Core;
using MazeRun.UI.Hud;
using MazeRun.UI.MainMenu;
using TriInspector;
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
        
        HudDialogue _hudDialogue;

        
        void Start() {
            mapGenerator.enabled = false;
            mapGenerator.onBlockReached += onBlockReached;
            
            mainMenu.OnPlay += () => {
                mainMenu.Hide();
                StartGame();
            };
            mainMenu.onContinueBtnClick += () => {
                mainMenu.Hide();
                ContinueGame();
            };
            mainMenu.Show();
            
            playerMovement.onJump += () => { };
            playerMovement.onSlide += () => { };
            playerMovement.onRight += () => { };
            playerMovement.onLeft += () => { };
            playerMovement.onLose += Lose;
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
        }

        public void Win() {
            _hudDialogue.Close();
            
            mainMenu.showLoseContainer = false;
            mainMenu.showWinContainer = true;
            mainMenu.Show();
            
            playerMovement.enabled = false;
            playerMovement.transform.localPosition = Vector3.zero;
            coreTime.ResetTime();
            
            mapGenerator.ResetMap();
            mapGenerator.enabled = false;
        }
        
        public void Lose() {
            Debug.Log( $"Lost the game" );
            
            playerMovement.enabled = false;
            playerMovement.transform.localPosition = Vector3.zero;
            coreTime.Pause();
            
            mapGenerator.ResetMap();
            mapGenerator.enabled = false;
            
            _hudDialogue.Close();
            
            // mainMenu.showContinueOption = true; //TODO: make game continue-able
            mainMenu.showWinContainer = false;
            mainMenu.showLoseContainer = true;
            mainMenu.Show();
        }

        public void ContinueGame() {
            Debug.Log( $"continuing game" );
            
            playerMovement.enabled = true;
            coreTime.Resume();
            
            playerMovement.Revive();
            
            _hudDialogue = DialogueManager.Current.GetOrCreate<HudDialogue>( mainCanvas.transform );
            _hudDialogue.gameManager = this;
        }
    }
}