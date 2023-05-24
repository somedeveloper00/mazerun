using UnityEngine;

namespace MazeRun {
    public class DestroyGameObjectOnYLevel : MonoBehaviour {
        public float yLevel = -100;
        void Update() {
            if (transform.position.y < yLevel) Destroy( gameObject );
        }
    }
}