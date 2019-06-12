using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DrawingBase.Input
{
    public sealed class KeyboardHelper
    {
        private readonly Dictionary<Key, KeyStates> prevKeyStates;
        private readonly Dictionary<Key, KeyStates> currentKeyStates;

        public KeyboardHelper()
        {
            prevKeyStates = new Dictionary<Key, KeyStates>();
            currentKeyStates = new Dictionary<Key, KeyStates>();
            var x = (Key[])Enum.GetValues(typeof(Key));
            foreach (Key k in x)
            {
                // Test if the key is valid
                try
                {
                    var state = Keyboard.GetKeyStates(k);
                    prevKeyStates.Add(k, state);
                    currentKeyStates.Add(k, state);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void Update()
        {
            foreach (Key k in new List<Key>(currentKeyStates.Keys))
            {
                prevKeyStates[k] = currentKeyStates[k];
                currentKeyStates[k] = Keyboard.GetKeyStates(k);
            }
        }

        public ButtonState GetPressedState(Key key)
        {
            return GetPressedState(prevKeyStates[key], currentKeyStates[key]);
        }

        private ButtonState GetPressedState(KeyStates prevKeyState, KeyStates currentKeyState)
        {
            if (!prevKeyState.HasFlag(KeyStates.Down))
            {
                if (currentKeyState.HasFlag(KeyStates.Down))
                    return ButtonState.Pressed;
                else
                    return ButtonState.Up;
            }
            else
            {
                if (currentKeyState.HasFlag(KeyStates.Down))
                    return ButtonState.Down;
                else
                    return ButtonState.Released;
            }
        }
    }
}