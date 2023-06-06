using System;
using UnityEngine;

namespace MazeRun.Core {
    public abstract class MovementInput : MonoBehaviour {

        public event Action<InputType> onInputReceived;
        
        protected virtual void Update() {
            var ip = GetInputType();
            if (ip.HasValue) onInputReceived?.Invoke( ip.Value );
        }

        protected abstract InputType? GetInputType();

        public enum InputType {
            MoveRight, MoveLeft, MoveUp, MoveDown
        }
    }
}