namespace Tetris.Shapes
{
    class Square : BaseShape
    {
        public Square(int tileId) : base(tileId)
        {
            var state = new bool[2, 2];
            state[0, 0] = true;
            state[0, 1] = true;
            state[1, 0] = true;
            state[1, 1] = true;
            states.Add(0, state);
        }
    }
}