using AnimFlex.Sequencer;
using MazeRun.Main;
using UnityEngine;

namespace MazeRun.Core {
    public class AnimOnStart : MonoBehaviour {
        [SerializeField] GameManager gameManager;
        [SerializeField] SequenceAnim seq;

        void OnEnable() => gameManager.OnStart += seq.PlaySequence;
        void OnDisable() => gameManager.OnStart -= seq.PlaySequence;
    }
}