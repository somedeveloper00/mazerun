using AnimFlex.Sequencer;
using TriInspector;
using UnityEngine;

namespace MazeRun.Core {
    public class LoseAnimHandler : MonoBehaviour {
        [SerializeField] SequenceAnim seq;
        [SerializeField] Behaviour[] disableComponents;

        [Button]
        public void StartAnimation() {
            foreach (var comp in disableComponents) comp.enabled = false;
            if (seq.sequence.IsPlaying()) seq.StopSequence();
            seq.PlaySequence();
        }
    }
}