namespace UlamWarbuton
{
    struct Cell
    {
        public long iteration;
        public int x;
        public int y;

        public Cell(long iteration, int x, int y)
        {
            this.iteration = iteration;
            this.x = x;
            this.y = y;
        }
    }
}