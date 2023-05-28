using AnimFlex.Sequencer;
using DialogueSystem;
using MazeRun.Main;
using UnityEngine;
using UnityEngine.UI;

namespace MazeRun.UI.MainMenu {
    public class MainMenu : Dialogue {

        public GameManager gameManager;
        public bool restartOption = true;

        [SerializeField] SequenceAnim inSeq, outSeq;
        [SerializeField] Button startButton;
        [SerializeField] GameObject restartContainer;
        [SerializeField] Button restartButton;
        
        protected override async void Start() {
            base.Start();
            
            startButton.onClick.AddListener( onStartBtnClick );
            restartButton.onClick.AddListener( onRestartClick );
            
            restartContainer.SetActive( restartOption );
            
            canvasRaycaster.enabled = false;
            inSeq.PlaySequence();
            await inSeq.AwaitComplete();
            canvasRaycaster.enabled = true;
        }

        void onRestartClick() {
            
        }

        async void onStartBtnClick() {
            outSeq.PlaySequence();
            await outSeq.AwaitComplete();
            Close();
            gameManager.StartGame();
        }
    }
}