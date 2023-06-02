using AnimFlex.Sequencer;
using DialogueSystem;
using MazeRun.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MazeRun.UI.Hud {
    public class HudDialogue : Dialogue {
        public GameManager gameManager;
        
        [SerializeField] TMP_Text pointsText;
        [SerializeField] TMP_Text pointsTextPrev;
        [SerializeField] SequenceAnim pointsChangeSeq;
        [SerializeField] Button pauseBtn;
        [SerializeField] GameObject pausedContainer;
        [SerializeField] GameObject unpausedContainer;
        [SerializeField] Button resumeBtn;

        protected override void Start() {
            base.Start();
            gameManager.OnProgressPointsUpdated += GameManagerOnOnProgressPointsUpdated;
            pauseBtn.onClick.AddListener( Pause );
            resumeBtn.onClick.AddListener( UnPause );
        }
        
        protected override void OnDestroy() {
            base.OnDestroy();
            gameManager.OnProgressPointsUpdated -= GameManagerOnOnProgressPointsUpdated;
        }

        void GameManagerOnOnProgressPointsUpdated() {
            pointsTextPrev.text = pointsText.text;
            pointsText.text = gameManager.currentProgress.points.ToString("#,0");
            if (!pointsChangeSeq.sequence.IsPlaying()) {
                pointsChangeSeq.PlaySequence();
            }
        }

        public void Pause() {
            pausedContainer.SetActive( true );
            unpausedContainer.SetActive( false );
            Time.timeScale = 0;
        }

        void UnPause() {
            pausedContainer.SetActive( false );
            unpausedContainer.SetActive( true );
            Time.timeScale = 1;
        }
    }
}