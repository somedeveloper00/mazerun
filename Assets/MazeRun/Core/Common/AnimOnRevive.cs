using AnimFlex.Sequencer;
using MazeRun.Main;
using UnityEngine;

namespace MazeRun.Core {
    public class AnimOnRevive : MonoBehaviour {
        [SerializeField] GameManager gameManager;
        [SerializeField] SequenceAnim seq;

        void OnEnable() => gameManager.OnRevive += seq.PlaySequence;
        void OnDisable() => gameManager.OnRevive -= seq.PlaySequence;
    }
}