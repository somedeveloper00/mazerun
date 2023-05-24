using UnityEngine;

namespace MazeRun.Core {
    public class KeyboardMovementInput : MovementInput {
        protected override InputType? GetInputType() {
            if (Input.GetKeyDown( KeyCode.RightArrow )) return InputType.MoveRight;
            if (Input.GetKeyDown( KeyCode.LeftArrow )) return InputType.MoveLeft;
            if (Input.GetKeyDown( KeyCode.UpArrow )) return InputType.MoveUp;
            if (Input.GetKeyDown( KeyCode.DownArrow )) return InputType.MoveDown;
            return null;
        }
    }
}