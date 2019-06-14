using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Tetris.Shapes
{
    abstract class BaseShape
    {
        private int stateId;
        protected Dictionary<int, bool[,]> states;
        public readonly int tileId;

        public BaseShape(int tileId)
        {
            stateId = 0;
            states = new Dictionary<int, bool[,]>();
            this.tileId = tileId;
        }

        public void Rotate()
        {
            if (stateId == states.Count - 1)
            {
                stateId = 0;
            }
            else
            {
                stateId++;
            }
        }

        public void UndoRotate()
        {
            if (stateId == 0)
            {
                stateId = states.Count - 1;
            }
            else
            {
                stateId--;
            }
        }

        public bool[,] CurrentState()
        {
            return states[stateId];
        }

        public bool[,] DefaultState()
        {
            return states[0];
        }

        public void Reset()
        {
            stateId = 0;
        }

        public int Width()
        {
            return CurrentState().GetLength(1);
        }

        public int Height()
        {
            return CurrentState().GetLength(0);
        }

        public bool HasCollision(Field field, Point position)
        {
            var currentState = CurrentState();
            for (int row = 0; row < Height(); row++)
            {
                for (int col = 0; col < Width(); col++)
                {
                    if (currentState[col, row])
                    {
                        // Collision with the left side of the playing field
                        if ((int)position.X + col < 0) return true;
                        // Collision with the right side of the playing field
                        if ((int)position.X + col >= field.Columns) return true;
                        // Collision with the bottom of the playing field
                        if ((int)position.Y + row >= field.Rows) return true;
                        // Collision with a previous shape
                        if (!field.CellIsEmpty((int)position.Y + row, (int)position.X + col)) return true;
                    }
                }
            }
            return false;
        }

        public void Draw(DrawingContext dc, Dictionary<int, Tile> tiles, double tileSize, bool drawCurrentState = true)
        {
            var currentState = drawCurrentState ? CurrentState() : DefaultState();
            var tile = tiles[tileId];
            for (int row = 0; row < Height(); row++)
            {
                for (int col = 0; col < Width(); col++)
                {
                    if (currentState[col, row])
                    {
                        tile.Draw(dc, col * tileSize, row * tileSize, tileSize);
                    }
                }
            }
        }
    }
}