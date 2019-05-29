using System;
using System.Windows.Input;

namespace FlappyBird
{
    internal enum ButtonState
    {
        Up,
        Pressed,
        Released,
        Down,
    }

    static class InputHelper
    {
        public static readonly MouseHelper Mouse = new MouseHelper();

        public static void Update()
        {
            Mouse.Update();
        }
    }

    internal enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    sealed class MouseHelper
    {
        private MouseButtonState prevLeftMouseState = MouseButtonState.Released;
        private MouseButtonState currentLeftMouseState = MouseButtonState.Released;
        private MouseButtonState prevRightMouseState = MouseButtonState.Released;
        private MouseButtonState currentRightMouseState = MouseButtonState.Released;
        private MouseButtonState prevMiddleMouseState = MouseButtonState.Released;
        private MouseButtonState currentMiddleMouseState = MouseButtonState.Released;

        public void Update()
        {
            prevLeftMouseState = currentLeftMouseState;
            currentLeftMouseState = Mouse.LeftButton;
            prevRightMouseState = currentRightMouseState;
            currentRightMouseState = Mouse.RightButton;
            prevMiddleMouseState = currentMiddleMouseState;
            currentMiddleMouseState = Mouse.MiddleButton;
        }

        public ButtonState GetState(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return GetState(prevLeftMouseState, currentLeftMouseState);
                case MouseButton.Right:
                    return GetState(prevRightMouseState, currentRightMouseState);
                case MouseButton.Middle:
                    return GetState(prevMiddleMouseState, currentMiddleMouseState);
                default:
                    throw new Exception("Unknown button state");
            }
        }

        private ButtonState GetState(MouseButtonState prevMouseState, MouseButtonState currentMouseState)
        {
            if (prevMouseState == MouseButtonState.Released)
            {
                if (currentMouseState == MouseButtonState.Pressed)
                    return ButtonState.Pressed;
                else
                    return ButtonState.Up;
            }
            else
            {
                if (currentMouseState == MouseButtonState.Pressed)
                    return ButtonState.Down;
                else
                    return ButtonState.Released;
            }
        }
    }
}