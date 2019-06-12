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

        public void Reset()
        {
            state = 0;
        }

        public int Width()
        {
            return CurrentState().GetLength(1);
        }

        public int Height()
        {
            // Find the lowest tile in the block
            bool[,] s = CurrentState(); 
            for (int row = s.GetLength(0) - 1; row >= 0; row--)
            {
                for (int col = 0; col < s.GetLength(1); col++)
                {
                    if (s[col, row])
                        return row + 1;
                }
            }

            return 0;
        }
    }
}