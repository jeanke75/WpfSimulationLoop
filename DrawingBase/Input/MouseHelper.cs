using System;
using System.Windows;
using System.Windows.Input;

namespace DrawingBase.Input
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        Extended1,
        Extended2
    }

    public sealed class MouseHelper
    {
        private MouseButtonState prevLeftMouseState = MouseButtonState.Released;
        private MouseButtonState currentLeftMouseState = MouseButtonState.Released;
        private MouseButtonState prevRightMouseState = MouseButtonState.Released;
        private MouseButtonState currentRightMouseState = MouseButtonState.Released;
        private MouseButtonState prevMiddleMouseState = MouseButtonState.Released;
        private MouseButtonState currentMiddleMouseState = MouseButtonState.Released;
        private MouseButtonState prevExtended1MouseState = MouseButtonState.Released;
        private MouseButtonState currentExtended1MouseState = MouseButtonState.Released;
        private MouseButtonState prevExtended2MouseState = MouseButtonState.Released;
        private MouseButtonState currentExtended2MouseState = MouseButtonState.Released;

        public void Update()
        {
            prevLeftMouseState = currentLeftMouseState;
            currentLeftMouseState = Mouse.LeftButton;
            prevRightMouseState = currentRightMouseState;
            currentRightMouseState = Mouse.RightButton;
            prevMiddleMouseState = currentMiddleMouseState;
            currentMiddleMouseState = Mouse.MiddleButton;
            prevExtended1MouseState = currentExtended1MouseState;
            currentExtended1MouseState = Mouse.MiddleButton;
            prevExtended2MouseState = currentExtended2MouseState;
            currentExtended2MouseState = Mouse.MiddleButton;
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
                case MouseButton.Extended1:
                    return GetState(prevExtended1MouseState, currentExtended1MouseState);
                case MouseButton.Extended2:
                    return GetState(prevExtended2MouseState, currentExtended2MouseState);
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

        public Point GetPosition()
        {
            return Mouse.GetPosition(Application.Current.MainWindow);
        }
    }
}