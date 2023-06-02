using AnimFlex.Sequencer;
using MazeRun.Main;
using UnityEngine;

namespace MazeRun.Core {
    public class AnimOnLose : MonoBehaviour {
        [SerializeField] GameManager gameManager;
        [SerializeField] SequenceAnim seq;

        void OnEnable() => gameManager.OnLose += seq.PlaySequence;
        void OnDisable() => gameManager.OnLose -= seq.PlaySequence;
    }
}