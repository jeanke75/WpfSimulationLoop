using System.Windows;
using System.Windows.Media;

namespace MazeGenerator
{
    class Maze
    {
        public readonly Cell[,] cells;

        public Maze(int rows, int cols)
        {
            cells = new Cell[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    cells[row, col] = new Cell(row, col);
                }
            }
        }

        public int GetRowCount()
        {
            return cells.GetLength(0);
        }

        public int GetColumnCount()
        {
            return cells.GetLength(1);
        }

        public void Draw(DrawingContext dc, Color cellColor, int cellSize, Color cellHighlightColor, int wallThickness, Color wallColor)
        {
            int rows = GetRowCount();
            int cols = GetColumnCount();

            // Draw background
            dc.DrawRectangle(new SolidColorBrush(cellColor), null, new Rect(0, 0, cols * cellSize, rows * cellSize));

            HighlightCurrentCell(dc, cellSize, cellHighlightColor);
            Pen pen = new Pen(new SolidColorBrush(wallColor), wallThickness);

            // Draw horizontal lines
            dc.DrawLine(pen, new Point(0, 0), new Point(cols * cellSize, 0));
            for (int row = 1; row < rows; row++)
            {
                int start = -1;
                for (int col = 0; col < cols; col++)
                {
                    if (cells[row, col].wall_top)
                    {
                        if (start == -1)
                        {
                            start = col;
                        }
                        if (col == cols - 1)
                        {
                            dc.DrawLine(pen, new Point(start * cellSize, row * cellSize), new Point((col + 1) * cellSize, row * cellSize));
                            start = -1;
                        }
                    }
                    else
                    {
                        if (start != -1)
                        {
                            dc.DrawLine(pen, new Point(start * cellSize, row * cellSize), new Point(col * cellSize, row * cellSize));
                            start = -1;
                        }
                    }
                }
            }
            dc.DrawLine(pen, new Point(0, rows * cellSize), new Point(cols * cellSize, rows * cellSize));

            // Draw vertical lines
            dc.DrawLine(pen, new Point(0, 0), new Point(0, rows * cellSize));
            for (int col = 1; col < cols; col++)
            {
                int start = -1;
                for (int row = 0; row < rows; row++)
                {
                    if (cells[row, col].wall_left)
                    {
                        if (start == -1)
                        {
                            start = row;
                        }
                        if (row == rows - 1)
                        {
                            dc.DrawLine(pen, new Point(col * cellSize, start * cellSize), new Point(col * cellSize, (row + 1) * cellSize));
                            start = -1;
                        }
                    }
                    else
                    {
                        if (start != -1)
                        {
                            dc.DrawLine(pen, new Point(col * cellSize, start * cellSize), new Point(col * cellSize, row * cellSize));
                            start = -1;
                        }
                    }
                }
            }
            dc.DrawLine(pen, new Point(cols * cellSize, 0), new Point(cols * cellSize, rows * cellSize));
        }

        private void HighlightCurrentCell(DrawingContext dc, int cellSize, Color cellHighlightColor)
        {
            for (int row = 0; row < GetRowCount(); row++)
            {
                for (int col = 0; col < GetColumnCount(); col++)
                {
                    if (cells[row, col].highlight)
                    {
                        dc.DrawRectangle(new SolidColorBrush(cellHighlightColor), null, new Rect(col * cellSize, row * cellSize, cellSize, cellSize));
                        return;
                    }
                }
            }
        }
    }
}