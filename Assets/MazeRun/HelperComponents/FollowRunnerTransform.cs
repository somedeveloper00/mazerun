using MazeRun.Core;
using MazeRun.Main;
using UnityEngine;
using UnityEngine.Animations;

namespace MazeRun {
    [RequireComponent( typeof(ParentConstraint) )]
    public class FollowRunnerTransform : MonoBehaviour {
        public GameManager gameManager;
        ParentConstraint _parentConstraint;

        void OnEnable() {
            _parentConstraint = GetComponent<ParentConstraint>();
            gameManager.OnRunnerSpawned += onRunnerSpawned;
        }

        void OnDisable() {
            gameManager.OnRunnerSpawned -= onRunnerSpawned;
        }

        void onRunnerSpawned(Runner runner) {
            if (_parentConstraint.sourceCount > 0) _parentConstraint.RemoveSource( 0 );
            _parentConstraint.AddSource( new ConstraintSource {
                sourceTransform = runner.playerMovement.transform,
                weight = 1
            } );
            _parentConstraint.constraintActive = true;
        }
    }
}