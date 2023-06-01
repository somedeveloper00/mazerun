using System;
using System.Linq;
using AnimFlex.Sequencer;
using MazeRun.Core.Hit;
using TriInspector;
using UnityEngine;

namespace MazeRun.Core {
    public class PlayerMovement : MonoBehaviour {
        [SerializeField] CoreTime coreTime;
        [SerializeField] float speed = 10;
        [PropertyTooltip("over seconds")]
        [SerializeField] AnimationCurve accelerationCurve;
        [SerializeField] MovementInput input;
        [SerializeField] float startInvincibleSeconds = 2;
        [SerializeField] float reviveInvincibleSeconds = 2;
        [SerializeField] HitChecking loseChecking;
        [SerializeField] SequenceAnim showUpSeq;
        [SerializeField] SequenceAnim jumpSeq;
        [SerializeField] SequenceAnim slideSeq;
        [SerializeField] SequenceAnim rotateRightSeq;
        [SerializeField] SequenceAnim rotateLeftSeq;
        [SerializeField] SequenceAnim resetSeq;
        [SerializeField] SequenceAnim reviveSeq;
        
        [Title( "Hit Checks" )] 
        [SerializeField] HitChecking rotateRightHitChecking;
        [SerializeField] HitChecking rotateLeftHitChecking;
        
        [Title("Rotate Helping")]
        [SerializeField] float rotateHelpDuration = 0.5f;
        [SerializeField] HitChecking[] helpRotateRightCheckings;
        [SerializeField] HitChecking[] helpRotateLeftCheckings;

        [Title( "Big Events" )] 
        [SerializeField] LoseAnimHandler loseAnimHandler;
        
        float _t = 0;
        int _movementLock = 0;
        float _helpDelayedInputTime = -1;
        float _invincibleTime = -1;
        MovementInput.InputType? _helpDelayedInput;
        MovementInput.InputType? _lastInputType;
        
        public event Action onJump = delegate {  };
        public event Action onSlide = delegate {  };
        public event Action onRight = delegate {  };
        public event Action onLeft = delegate {  };
        public event Action onLose = delegate { };

        void Start() {
            input.onInputReceived += onMovementInputReceived;
            jumpSeq.sequence.onPlay += increaseLock;
            jumpSeq.sequence.onComplete += decreaseLock;
            slideSeq.sequence.onPlay += increaseLock;
            slideSeq.sequence.onComplete += decreaseLock;
            rotateRightSeq.sequence.onPlay += increaseLock;
            rotateRightSeq.sequence.onComplete += decreaseLock;
            rotateLeftSeq.sequence.onPlay += increaseLock;
            rotateLeftSeq.sequence.onComplete += decreaseLock;
            resetSeq.sequence.onPlay += increaseLock;
            resetSeq.sequence.onComplete += decreaseLock;
            
            void decreaseLock() => _movementLock--;
            void increaseLock() => _movementLock++;
            
            _invincibleTime = startInvincibleSeconds;
        }

        void Update() {
            updateMovement();
            updateRotateHelp();
            _invincibleTime -= coreTime.deltaTime;
            if (_invincibleTime <= 0) updateLose();
        }

        public void ShowUp() {
            showUpSeq.PlaySequence();
        }
        
        public void Revive() {
            reviveSeq.PlaySequence();
            _invincibleTime = reviveInvincibleSeconds;
        }
        
        void updateLose() {
            if (!loseChecking.Hits( out _ )) return;
            loseAnimHandler.StartAnimation();
            onLose();
        }

        void onMovementInputReceived(MovementInput.InputType inputType) {
            if (_movementLock > 0) {
                if (_lastInputType.HasValue && _lastInputType.Value != inputType) {
                    resetJumpAndSlide();
                    _lastInputType = inputType;
                }
                return;
            }
            
            switch (inputType) {
                case MovementInput.InputType.MoveUp: jump(); break;
                case MovementInput.InputType.MoveDown: slide(); break;
                case MovementInput.InputType.MoveRight: {
                    if (_movementLock > 0) break;
                    if (rotateRightHitChecking.Hits( out _ )) {
                        if (!helpRotateRightCheckings.All( hc => hc.Hits( out _ ) )) {
                            Debug.Log( "help start" );
                            _helpDelayedInput = MovementInput.InputType.MoveRight;
                            _helpDelayedInputTime = coreTime.time;
                            break;
                        }
                    }
                    moveRight();
                    break;
                }
                case MovementInput.InputType.MoveLeft: {
                    if (_movementLock > 0) break;
                    if (rotateLeftHitChecking.Hits( out _ )) {
                        if (!helpRotateLeftCheckings.All( hc => hc.Hits( out _ ) )) {
                            Debug.Log( "help start" );
                            _helpDelayedInput = MovementInput.InputType.MoveLeft;
                            _helpDelayedInputTime = coreTime.time;
                            break;
                        }
                    }
                    moveLeft();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException( nameof(inputType), inputType, null );
            }

            _lastInputType = inputType;
        }

        void updateMovement() {
            _t += coreTime.deltaTime;
            transform.position += speed * coreTime.deltaTime * accelerationCurve.Evaluate( _t ) * transform.forward;
        }

        void updateRotateHelp() {
            if (!_helpDelayedInput.HasValue) return;
            if (_helpDelayedInputTime - coreTime.time > rotateHelpDuration) {
                _helpDelayedInput = null;
                Debug.Log( "help end" );
                return;
            }

            bool helped = false;
            switch (_helpDelayedInput.Value) {
                case MovementInput.InputType.MoveRight:
                    if (!rotateRightHitChecking.Hits( out _ )) {
                        moveRight();
                        helped = true;
                    }
                    break;
                case MovementInput.InputType.MoveLeft:
                    if (!rotateLeftHitChecking.Hits( out _ )) {
                        helped = true;
                        moveLeft();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (helped) {
                Debug.Log( "helped." );
                _helpDelayedInput = null;
            }
        }

        void resetJumpAndSlide() {
            if (jumpSeq.sequence.IsPlaying()) jumpSeq.StopSequence();
            if (slideSeq.sequence.IsPlaying()) slideSeq.StopSequence();
            Debug.Log( "reset" );
            resetSeq.PlaySequence();
        }

        void moveRight() {
            Debug.Log( "right" );
            rotateRightSeq.PlaySequence();
            transform.forward = transform.right;
            onRight();
        }

        void moveLeft() {
            Debug.Log( "left" );
            rotateLeftSeq.PlaySequence();
            transform.forward = -transform.right;
            onLeft();
        }

        void jump() {
            Debug.Log( "jump" );
            jumpSeq.PlaySequence();
            onJump();
        }
        
        void slide() {
            Debug.Log( "down" );
            slideSeq.PlaySequence();
            onSlide();
        }
    }
}