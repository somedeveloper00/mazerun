using System;
using AnimFlex.Sequencer;
using DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MazeRun.UI.MainMenu {
    public class MainMenuDialogue : Dialogue {

        public bool showContinueOption = true;
        public bool showLoseContainer = true;
        public bool showWinContainer = true;

        [SerializeField] SequenceAnim inSeq, outSeq;
        [SerializeField] Button playBtn;
        [SerializeField] Button continueBtn;
        [SerializeField] GameObject continueContainer;
        [SerializeField] GameObject loseContainer;
        [SerializeField] GameObject winContainer;
        
        public event Action OnPlay = delegate {  };
        public event Action onReviveBtnClick = delegate {  };
        
        protected override void Start() {
            base.Start();
            
            playBtn.onClick.AddListener( () => OnPlay.Invoke() );
            continueBtn.onClick.AddListener( () => onReviveBtnClick.Invoke() );
         
            inSeq.sequence.onPlay += () => {
                canvas.enabled = true;
                canvasRaycaster.enabled = false;
            };
            inSeq.sequence.onComplete += () => canvasRaycaster.enabled = true;
            outSeq.sequence.onPlay += () => canvasRaycaster.enabled = false;
            outSeq.sequence.onComplete += () => canvas.enabled = false;
        }

        public void Show() {
            if (inSeq.sequence.IsPlaying()) return;
            if (outSeq.sequence.IsPlaying()) outSeq.StopSequence();

            continueContainer.SetActive( showContinueOption );
            loseContainer.SetActive( showLoseContainer );
            winContainer.SetActive( showWinContainer );
            
            inSeq.PlaySequence();
        }
        public void Hide() {
            if (outSeq.sequence.IsPlaying()) return;
            if (inSeq.sequence.IsPlaying()) inSeq.StopSequence();
            outSeq.PlaySequence();
        }
    }
}