using DrawingBase;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace UlamWarbuton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private readonly int cellSize = 4;
        private readonly int iterations = 75;
        private readonly bool diagonal = false;
        private readonly Brush cellColor = new SolidColorBrush(Colors.White);

        private int iteration;
        private bool[,] currentState;
        private bool[,] newState;
        private Queue<Cell> cellsToCheck;

        public override void Initialize()
        {
            var size = (iterations * 2 + 1) * cellSize;
            SetResolution(size, size);

            iteration = 0;
            currentState = new bool[iterations * 2 + 1, iterations * 2 + 1];
            currentState[iterations, iterations] = true;

            newState = (bool[,])currentState.Clone();

            cellsToCheck = InitializeQueue(diagonal);

            cellColor.Freeze();
        }

        public override void Update(float dt)
        {
            if (iteration < iterations)
            {
                while (cellsToCheck.Count > 0 && cellsToCheck.Peek().iteration == iteration)
                {
                    Cell cell = cellsToCheck.Dequeue();

                    if (HasExactly1ActiveNeighbour(iteration, cell.x, cell.y, diagonal, out List<Cell> inactiveNeighbours))
                    {
                        newState[cell.x, cell.y] = true;
                        foreach (Cell c in inactiveNeighbours)
                        {
                            cellsToCheck.Enqueue(c);
                        }
                    }
                }

                currentState = (bool[,])newState.Clone();

                iteration++;
            }
        }

        public override void Draw(DrawingContext dc)
        {
            Rect rect = new Rect(0, 0, cellSize, cellSize);
            for (int x = 0; x < currentState.GetLength(0); x++)
            {
                for (int y = 0; y < currentState.GetLength(1); y++)
                {
                    if (currentState[x, y])
                    {
                        dc.PushTransform(new TranslateTransform(x * cellSize, y * cellSize));
                        dc.DrawRectangle(cellColor, null, rect);
                        dc.Pop();
                    }
                }
            }
        }

        public override void Cleanup()
        {
        }

        private Queue<Cell> InitializeQueue(bool diagonal)
        {
            Queue<Cell> queue = new Queue<Cell>();

            for (int y = 0; y < currentState.GetLength(1); y++)
            {
                for (int x = 0; x < currentState.GetLength(0); x++)
                {
                    if (currentState[x, y])
                    {
                        foreach (Cell c in GetInactiveNeighbours(-1, x, y, diagonal))
                        {
                            queue.Enqueue(c);
                        }
                    }
                }
            }

            return queue;
        }

        private List<Cell> GetInactiveNeighbours(int iteration, int x, int y, bool diagonal)
        {
            List<Cell> tmp = new List<Cell>();
            // adjacent cells
            if (!diagonal)
            {
                if (!GetCellValue(x, y - 1))
                {
                    tmp.Add(new Cell(iteration + 1, x, y - 1));
                }
                if (!GetCellValue(x - 1, y))
                {
                    tmp.Add(new Cell(iteration + 1, x - 1, y));
                }
                if (!GetCellValue(x + 1, y))
                {
                    tmp.Add(new Cell(iteration + 1, x + 1, y));
                }
                if (!GetCellValue(x, y + 1))
                {
                    tmp.Add(new Cell(iteration + 1, x, y + 1));
                }
            } // adjacent diagonal cells
            else
            {
                if (!GetCellValue(x - 1, y - 1))
                {
                    tmp.Add(new Cell(iteration + 1, x - 1, y - 1));
                }
                if (!GetCellValue(x + 1, y - 1))
                {
                    tmp.Add(new Cell(iteration + 1, x + 1, y - 1));
                }
                if (!GetCellValue(x - 1, y + 1))
                {
                    tmp.Add(new Cell(iteration + 1, x - 1, y + 1));
                }
                if (!GetCellValue(x + 1, y + 1))
                {
                    tmp.Add(new Cell(iteration + 1, x + 1, y + 1));
                }
            }

            return tmp;
        }

        private bool HasExactly1ActiveNeighbour(int iteration, int x, int y, bool diagonal, out List<Cell> inactiveNeighbours)
        {
            List<Cell> tmp = GetInactiveNeighbours(iteration, x, y, diagonal);

            if (tmp.Count == 3)
            {
                inactiveNeighbours = tmp;
                return true;
            }
            inactiveNeighbours = null;
            return false;
        }

        private bool GetCellValue(int x, int y)
        {
            if (x < 0 || x >= currentState.GetLength(0) || y < 0 || y >= currentState.GetLength(1)) return false;
            return currentState[x, y];
        }
    }
}