using AnimFlex.Sequencer;
using MazeRun.Hit;
using UnityEngine;

namespace MazeRun.Common {
    public class PlayOnHitCheck : MonoBehaviour {
        [SerializeField] SequenceAnim hitSeq;
        [SerializeField] HitChecking hitCheck;

        void Update() {
            if (hitCheck.Hits( out _ )) {
                hitSeq.PlaySequence();
                enabled = false;
            }
        }
    }
}