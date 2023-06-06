using System;
using MazeRun.Utils;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MazeRun.Core {
    [RequireComponent(typeof(MaskableGraphic))]
    public class TouchMovementInput : MovementInput, IPointerDownHandler, IPointerMoveHandler, IPointerExitHandler {
        
        public float dragThreshold = 10f;
        public DirectionInputTypeMap[] maps = {
            new() { direction = Vector2.right, inputType = InputType.MoveRight },
            new() { direction = Vector2.left, inputType = InputType.MoveLeft },
            new() { direction = Vector2.up, inputType = InputType.MoveUp },
            new() { direction = Vector2.down, inputType = InputType.MoveDown },
        };

        [NonSerialized] MaskableGraphic _graphic;
        [NonSerialized] Vector2? _startedPos;
        [NonSerialized] InputType? _currentInputType = null;
        
        void Awake() => _graphic = GetComponent<MaskableGraphic>();

        protected override void Update() {
            base.Update();
            if (_currentInputType.HasValue) {
                _startedPos = null;
                _currentInputType = null;
            }
        }

        protected override InputType? GetInputType() => _currentInputType;

        public void OnPointerDown(PointerEventData eventData) {
            if (!_graphic || eventData.pointerCurrentRaycast.gameObject != _graphic.gameObject) return;
            _startedPos = eventData.position;
            _currentInputType = null;
            Debug.Log( "started".Color( Color.magenta ) );
        }

        public void OnPointerMove(PointerEventData eventData) {
            if (_currentInputType.HasValue) return;
            if (!_startedPos.HasValue) return;
            if (!_graphic || eventData.pointerCurrentRaycast.gameObject != _graphic.gameObject) return;
            var delta = eventData.position - _startedPos.Value;
            if (!(delta.magnitude > dragThreshold)) return;
            
            delta.Normalize();
            float closestDot = float.MinValue;
            foreach (var map in maps ) {
                var dot = Vector2.Dot( delta, map.direction );
                if (dot > closestDot) {
                    closestDot = dot;
                    _currentInputType = map.inputType;
                }
            }
            Debug.Log( $"Accepted: {_currentInputType}".Color( Color.magenta ) );
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!_graphic || eventData.pointerCurrentRaycast.gameObject != _graphic.gameObject) return;
            Debug.Log( "Exited".Color( Color.magenta ) );
            _currentInputType = null;
            _startedPos = null;
        }

        [Serializable]
        public struct DirectionInputTypeMap {
            [OnValueChanged(nameof(normalize))]
            public Vector2 direction;
            public InputType inputType;

            void normalize() => direction.Normalize();
        }
    }
}