using AnimFlex.Sequencer;
using DialogueSystem;
using TMPro;
using UnityEngine;

namespace MazeRun.UI.Loading {
    public class LoadingDialogue : Dialogue {
        public string text {
            get => txt.text;
            set {
                txt.text = value;
                txt.gameObject.SetActive( !string.IsNullOrEmpty( value ) );
            }
        }
        [SerializeField] TMP_Text txt;
        [SerializeField] SequenceAnim inSeq;
        [SerializeField] SequenceAnim outSeq;

        protected override void Start() {
            base.Start();
            inSeq.PlaySequence();
        }

        public new void Close() {
            inSeq.StopSequence();
            outSeq.PlaySequence();
            outSeq.sequence.onComplete += base.Close;
        }
    }
}