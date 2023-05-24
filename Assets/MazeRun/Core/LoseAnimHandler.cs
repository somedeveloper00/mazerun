using AnimFlex.Sequencer;
using AnimFlex.Sequencer.BindingSystem;
using TriInspector;
using UnityEngine;

namespace MazeRun.Core {
    public class LoseAnimHandler : MonoBehaviour {
        [SerializeField] SequenceAnim seq;
        [SerializeField] SequenceBinder_Vector3 explosion;
        [SerializeField] Behaviour[] disableComponents;

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere( transform.TransformPoint( explosion.value ), 0.1f );
        }

        [Button]
        public void StartAnimation() {
            foreach (var comp in disableComponents) { comp.enabled = false; }
            explosion.value = transform.TransformPoint( explosion.value );
            explosion.Bind();
            seq.PlaySequence();
        }
    }
}