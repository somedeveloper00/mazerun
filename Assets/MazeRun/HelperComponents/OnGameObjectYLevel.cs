using System;
using UnityEngine;

namespace MazeRun {
    public class OnGameObjectYLevel : MonoBehaviour {
        [SerializeField] Behaviour behaviour;
        
        public float yLevel = -100;
        
        void Update() {
            if (transform.position.y < yLevel) {
                switch (behaviour) {
                    case Behaviour.DestroyGameObject:
                        Destroy( gameObject );
                        break;
                    case Behaviour.DisableGameObject:
                        gameObject.SetActive( false );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        enum Behaviour {
            DestroyGameObject,
            DisableGameObject
        }
    }
}