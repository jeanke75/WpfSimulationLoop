namespace MazeGenerator
{
    class Cell
    {
        public readonly int col;
        public readonly int row;

        public bool wall_top = true;
        public bool wall_right = true;
        public bool wall_bottom = true;
        public bool wall_left = true;

        public bool visited = false;
        public bool highlight = false;

        public Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
}