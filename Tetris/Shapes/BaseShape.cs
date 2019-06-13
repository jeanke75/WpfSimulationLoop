using System.Collections.Generic;

namespace Tetris.Shapes
{
    abstract class BaseShape
    {
        private int state;
        protected Dictionary<int, bool[,]> states;
        public readonly int tileId;

        public BaseShape(int tileId)
        {
            state = 0;
            states = new Dictionary<int, bool[,]>();
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

        public void UndoRotate()
        {
            if (state == 0)
            {
                state = states.Count - 1;
            }
            else
            {
                state--;
            }
        }

        public bool[,] CurrentState()
        {
            return states[state];
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
            return CurrentState().GetLength(0);
        }
    }
}