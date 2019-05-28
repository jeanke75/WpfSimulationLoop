using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MazeGenerator
{
    class RecursiveBacktrackerMazeGenerator
    {
        private readonly Maze maze;
        private readonly Stack<Cell> previousCells;
        private Cell currentCell;
        private readonly Random random;
        private readonly bool showBacktracking;

        public RecursiveBacktrackerMazeGenerator(int rows, int cols, bool showBacktracking = true)
        {
            maze = new Maze(rows, cols);

            previousCells = new Stack<Cell>();
            random = new Random();

            // Make the initial cell the current cell and mark it as visited
            SetAsCurrentCell(maze.cells[0, 0]);
            currentCell.visited = true;

            this.showBacktracking = showBacktracking;
        }

        public void Update()
        {
            if (currentCell != null)
            {
                // If backtracking has to be displayed run only 1step per update
                // otherwise run this step multiple times when backtracking
                do
                {
                    // If the current cell has any neighbours which have not been visited, pick one randomly
                    Cell next = GetRandomUnvisitedNeighbour(currentCell);
                    if (next != null)
                    {
                        // Push the current cell to the stack
                        previousCells.Push(currentCell);

                        // Remove the wall between the current cell and the chosen cell
                        RemoveWallBetweenCells(currentCell, next);

                        // Make the chosen cell the current cell and mark it as visited
                        SetAsCurrentCell(next);
                        currentCell.visited = true;
                        break;
                    }
                    // Else if stack is not empty
                    else if (previousCells.Count > 0)
                    {
                        // Pop a cell from the stack and make it the current cell
                        SetAsCurrentCell(previousCells.Pop());
                    }
                    // When there are no unvisited cells
                    else
                    {
                        SetAsCurrentCell(null);
                        break;
                    }
                }
                while (!showBacktracking);
            }
        }

        public void Draw(DrawingContext dc, Color cellColor, int cellSize, Color cellHighlightColor, int wallThickness, Color wallColor)
        {
            maze.Draw(dc, cellColor, cellSize, cellHighlightColor, wallThickness, wallColor);
        }

        private Cell GetRandomUnvisitedNeighbour(Cell cell)
        {
            Cell above = GetCellIfExistsAndUnvisited(cell.row - 1, cell.col);
            Cell right = GetCellIfExistsAndUnvisited(cell.row, cell.col + 1);
            Cell below = GetCellIfExistsAndUnvisited(cell.row + 1, cell.col);
            Cell left = GetCellIfExistsAndUnvisited(cell.row, cell.col - 1);

            List<Cell> unvisitedNeighbours = new List<Cell>();
            if (above != null) unvisitedNeighbours.Add(above);
            if (right != null) unvisitedNeighbours.Add(right);
            if (below != null) unvisitedNeighbours.Add(below);
            if (left != null) unvisitedNeighbours.Add(left);

            if (unvisitedNeighbours.Count == 0) return null;

            return unvisitedNeighbours[random.Next(unvisitedNeighbours.Count)];
        }

        private Cell GetCellIfExistsAndUnvisited(int row, int col)
        {
            if (row < 0 || col < 0 || row >= maze.GetRowCount() || col >= maze.GetColumnCount() || maze.cells[row, col].visited) return null;

            return maze.cells[row, col];
        }

        private void RemoveWallBetweenCells(Cell c1, Cell c2)
        {
            if (c1.row > c2.row)
            {
                c1.wall_top = false;
                c2.wall_bottom = false;
            }
            else if (c1.col < c2.col)
            {
                c1.wall_right = false;
                c2.wall_left = false;
            }
            else if (c1.row < c2.row)
            {
                c1.wall_bottom = false;
                c2.wall_top = false;
            }
            else if (c1.col > c2.col)
            {
                c1.wall_left = false;
                c2.wall_right = false;
            }
            else
            {
                throw new Exception("wut!");
            }
        }

        private void SetAsCurrentCell(Cell next)
        {
            if (currentCell != null)
            {
                currentCell.highlight = false;
            }

            if (next != null)
            {
                currentCell = next;
                currentCell.highlight = true;
            }
        }
    }
}