using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Tetris.Shapes;

namespace Tetris
{
    class Field
    {
        private readonly int[,] cells;

        public int Rows
        {
            get
            {
                return cells.GetLength(1);
            }
        }

        public int Columns
        {
            get
            {
                return cells.GetLength(0);
            }
        }

        public Field(int rows, int columns)
        {
            cells = new int[columns, rows];
        }

        public void Draw(DrawingContext dc, int tileSize, Dictionary<int, Tile> tiles)
        {
            dc.PushTransform(new TranslateTransform(tileSize, tileSize));
            // Draw field
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    var tileId = cells[column, row];
                    if (tileId > 0)
                    {
                        tiles.TryGetValue(tileId, out Tile tile);
                        tile.Draw(dc, column * tileSize, row * tileSize, tileSize);
                    }
                }
            }
        }

        public bool CellIsEmpty(int row, int column)
        {
            return cells[column, row] == 0;
        }

        public void AddShape(BaseShape shape, Point position)
        {
            var state = shape.CurrentState();
            for (int row = 0; row < shape.Height(); row++)
            {
                for (int col = 0; col < shape.Width(); col++)
                {
                    if (state[col, row])
                    {
                        cells[(int)position.X + col, (int)position.Y + row] = shape.tileId;
                    }
                }
            }
        }

        public int ClearLines(Point position)
        {
            int linesCleared = 0;
            for (int row = (int)position.Y; row < Rows; row++)
            {
                bool fullLine = true;
                for (int column = 0; column < Columns; column++)
                {
                    if (cells[column, row] == 0)
                    {
                        fullLine = false;
                        break;
                    }
                }
                if (fullLine)
                {
                    DropLinesDown(row);
                    linesCleared++;
                }
            }

            return linesCleared;
        }

        private void DropLinesDown(int fullLineRow)
        {
            for (int row = fullLineRow; row > 0; row--)
            {
                for (int column = 0; column < Columns; column++)
                {
                    cells[column, row] = cells[column, row - 1];
                    cells[column, row - 1] = 0;
                }
            }
        }
    }
}