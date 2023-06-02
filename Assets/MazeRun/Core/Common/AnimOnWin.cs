using AnimFlex.Sequencer;
using MazeRun.Main;
using UnityEngine;

namespace MazeRun.Core {
    public class AnimOnWin : MonoBehaviour {
        [SerializeField] GameManager gameManager;
        [SerializeField] SequenceAnim seq;

        void OnEnable() => gameManager.OnWin += seq.PlaySequence;
        void OnDisable() => gameManager.OnWin -= seq.PlaySequence;
    }
}