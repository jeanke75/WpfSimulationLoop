namespace Tetris.Shapes
{
    class I : BaseShape
    {
        public I(int tileId) : base(tileId)
        {
            bool[,] state1 = new bool[4, 4];
            state1[0, 2] = true;
            state1[1, 2] = true;
            state1[2, 2] = true;
            state1[3, 2] = true;
            states.Add(0, state1);

            bool[,] state2 = new bool[4, 4];
            state2[1, 0] = true;
            state2[1, 1] = true;
            state2[1, 2] = true;
            state2[1, 3] = true;
            states.Add(1, state2);
        }
    }
}