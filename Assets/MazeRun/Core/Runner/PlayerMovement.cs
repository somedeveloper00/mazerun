using System;
using System.Linq;
using AnimFlex.Sequencer;
using MazeRun.Hit;
using MazeRun.Utils;
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
        [SerializeField] SequenceAnim hideSeq;
        [SerializeField] SequenceAnim showUpSeq;
        [SerializeField] SequenceAnim loseSeq;
        [SerializeField] SequenceAnim jumpSeq;
        [SerializeField] SequenceAnim slideSeq;
        [SerializeField] SequenceAnim rotateRightSeq;
        [SerializeField] SequenceAnim rotateLeftSeq;
        [SerializeField] SequenceAnim resetSeq;
        [SerializeField] SequenceAnim reviveSeq;
        [SerializeField] SequenceAnim invincibleStartSeq;
        [SerializeField] SequenceAnim invincibleEndSeq;
        
        [Title( "Hit Checks" )] 
        [SerializeField] HitChecking rotateRightHitChecking;
        [SerializeField] HitChecking rotateLeftHitChecking;
        
        [Title("Rotate Helping")]
        [SerializeField] float rotateHelpDuration = 0.5f;
        [SerializeField] HitChecking[] helpRotateRightCheckings;
        [SerializeField] HitChecking[] helpRotateLeftCheckings;

        [Title( "Big Events" )] 
        
        float _helpDelayedInputTime = -1;
        [ShowInInspector, ReadOnly] int _movementLock = 0;
        [ShowInInspector, ReadOnly] float _invincibleTime = -1;
        MovementInput.InputType? _helpDelayedInput;
        MovementInput.InputType? _lastInputType;
        bool _lastFrameInvincible = false;

        public event Action onJump = delegate {  };
        public event Action onSlide = delegate {  };
        public event Action onRight = delegate {  };
        public event Action onLeft = delegate {  };
        public event Action onLose = delegate { };
        public event Action onInvincibleStart = delegate { };
        public event Action onInvincibleEnd = delegate { };

        void Awake() {
            jumpSeq.sequence.onPlay += () => increaseLock( "jumpSeq" );
            jumpSeq.sequence.onComplete += () => decreaseLock( "jumpSeq" );
            slideSeq.sequence.onPlay += () => increaseLock( "slideSeq" );
            slideSeq.sequence.onComplete += () => decreaseLock( "slideSeq" );
            rotateRightSeq.sequence.onPlay += () => increaseLock( "rotateRightSeq" );
            rotateRightSeq.sequence.onComplete += () => decreaseLock( "rotateRightSeq" );
            rotateLeftSeq.sequence.onPlay += () => increaseLock( "rotateLeftSeq" );
            rotateLeftSeq.sequence.onComplete += () => decreaseLock( "rotateLeftSeq" );
            resetSeq.sequence.onPlay += () => increaseLock( "resetSeq" );
            resetSeq.sequence.onComplete += () => decreaseLock( "resetSeq" );
            reviveSeq.sequence.onPlay += () => increaseLock( "reviveSeq" );
            reviveSeq.sequence.onComplete += () => resetLock( "reviveSeq" );
            hideSeq.sequence.onPlay += () => increaseLock( "hideSeq" );
            hideSeq.sequence.onComplete += () => resetLock( "hideSeq" );
            showUpSeq.sequence.onPlay += () => increaseLock( "showUpSeq" );
            showUpSeq.sequence.onComplete += () => resetLock( "showUpSeq" );
            loseSeq.sequence.onPlay += () => resetLock( "loseSeq" );
            loseSeq.sequence.onComplete += () => hideSeq.PlaySequence();
        }

        void OnEnable() => input.onInputReceived += onMovementInputReceived;
        void OnDisable() => input.onInputReceived -= onMovementInputReceived;

        void decreaseLock(string caller) {
            _movementLock--;
            Debug.Log($"{caller} decreaseLock: {_movementLock}");
        }
        void increaseLock(string caller) {
            _movementLock++;
            Debug.Log($"{caller} increaseLock: {_movementLock}");
        }
        void resetLock(string caller) {
            _movementLock = 0;
            Debug.Log($"{caller} resetLock: {_movementLock}");
        }

        void Update() {
            updateMovement();
            updateRotateHelp();
            _invincibleTime -= coreTime.deltaTime;
            var invincible = isInvincible();
            if (_lastFrameInvincible != invincible) {
                if (invincible) {
                    onInvincibleStart();
                    invincibleEndSeq.StopSequence();
                    invincibleStartSeq.StopSequence();
                    invincibleStartSeq.PlaySequence();
                } else {
                    onInvincibleEnd();
                    invincibleStartSeq.StopSequence();
                    invincibleEndSeq.StopSequence();
                    invincibleEndSeq.PlaySequence();
                }
                _lastFrameInvincible = invincible;
            }
            if (!invincible) updateLose();
        }

        public void Hide() {
            stopAllAnims();
            hideSeq.PlaySequence();
        }
        
        public void ShowUp() {
            stopAllAnims();
            resetLock("ShowUp");
            showUpSeq.PlaySequence();
            _invincibleTime = startInvincibleSeconds;
            _lastInputType = null;
        }
        
        public void Revive() {
            stopAllAnims();
            resetLock("Revive");
            reviveSeq.PlaySequence();
            _invincibleTime = reviveInvincibleSeconds;
            _lastInputType = null;
        }
        
        
        void updateLose() {
            if (!loseChecking.Hits( out var hit )) return;
             Debug.Log( $"Lose: {hit.name} <i>(click to select)</i>", hit );
            stopAllAnims();
            resetLock( "Lose" );
            loseSeq.PlaySequence();
            onLose();
        }
        
        bool isInvincible() => _invincibleTime > 0;

        void stopAllAnims() {
            if (hideSeq.sequence.IsPlaying())           hideSeq.StopSequence();
            if (showUpSeq.sequence.IsPlaying())         showUpSeq.StopSequence();
            if (jumpSeq.sequence.IsPlaying())           jumpSeq.StopSequence();
            if (slideSeq.sequence.IsPlaying())          slideSeq.StopSequence();
            if (rotateRightSeq.sequence.IsPlaying())    rotateRightSeq.StopSequence();
            if (rotateLeftSeq.sequence.IsPlaying())     rotateLeftSeq.StopSequence();
            if (resetSeq.sequence.IsPlaying())          resetSeq.StopSequence();
            if (reviveSeq.sequence.IsPlaying())         reviveSeq.StopSequence();
            if (invincibleStartSeq.sequence.IsPlaying()) invincibleStartSeq.StopSequence();
            if (invincibleEndSeq.sequence.IsPlaying())  invincibleEndSeq.StopSequence();
            if (loseSeq.sequence.IsPlaying())           loseSeq.StopSequence();
        }

        void onMovementInputReceived(MovementInput.InputType inputType) {
            bool inputMovementIsPlaying() => rotateRightSeq.sequence.IsPlaying() || rotateLeftSeq.sequence.IsPlaying() ||
                                        jumpSeq.sequence.IsPlaying() || slideSeq.sequence.IsPlaying();

            if (_movementLock > 0) {
                if (inputMovementIsPlaying() && _lastInputType.HasValue && _lastInputType.Value != inputType) {
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
                        // will hit wall, so if invincible, don't rotate 
                        if (isInvincible()) break;
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
                        // will hit wall, so if invincible, don't rotate 
                        if (isInvincible()) break;
                    }
                    moveLeft();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException( nameof(inputType), inputType, null );
            }

            _lastInputType = inputType;
        }

        void updateMovement() => transform.position += speed * coreTime.deltaTime * accelerationCurve.Evaluate( coreTime.time ) * transform.forward;

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
            onRight();
            transform.forward = transform.right;
        }

        void moveLeft() {
            Debug.Log( "left" );
            rotateLeftSeq.PlaySequence();
            onLeft();
            transform.forward = -transform.right;
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