using System;
using UnityEngine;

namespace MazeRun.UI {
    [RequireComponent( typeof(AudioSource) )]
    public class SingletonAudioSource : MonoBehaviour {
        [SerializeField] Type type;
        AudioSource _audioSource;

        public static AudioSource MusicInstance { get; private set; }
        public static AudioSource SfxInstance { get; private set; }

        void OnEnable() {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) {
                Debug.LogError( "SingletonAudioSource requires an AudioSource component!" );
                Destroy( gameObject );
            }

            switch (type) {
                case Type.Music when MusicInstance != null:
                    Destroy( gameObject );
                    return;
                case Type.Music:
                    MusicInstance = _audioSource;
                    break;
                case Type.Sfx when SfxInstance != null:
                    Destroy( gameObject );
                    return;
                case Type.Sfx:
                    SfxInstance = _audioSource;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        enum Type {
            Sfx,
            Music
        }
    }
}