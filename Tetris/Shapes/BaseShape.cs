using System.Collections.Generic;

namespace Tetris.Shapes
{
    abstract class BaseShape
    {
        private byte state;
        protected Dictionary<byte, bool[,]> states;
        public readonly int tileId;

        public BaseShape(int tileId)
        {
            state = 0;
            states = new Dictionary<byte, bool[,]>();
            this.tileId = tileId;
        }

        public void Rotate()
        {
            if (state == states.Count - 1)
            {
                state = 0;
            }
            else
            {
                state++;
            }
        }

        public bool[,] CurrentState()
        {
            states.TryGetValue(state, out bool[,] value);
            return value;
        }

        public int Width()
        {
            return CurrentState().GetLength(1);
        }

        public int Height()
        {
            return CurrentState().GetLength(0);
        }
    }
}