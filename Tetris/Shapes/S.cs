namespace Tetris.Shapes
{
    class S : BaseShape
    {
        public S(int tileId) : base(tileId)
        {
            /* ooo
             * oxx
             * xxo
             */
            bool[,] state1 = new bool[3, 3];
            state1[1, 1] = true;
            state1[2, 1] = true;
            state1[0, 2] = true;
            state1[1, 2] = true;
            states.Add(0, state1);

            /* xoo
             * xxo
             * oxo
             */
            bool[,] state2 = new bool[3, 3];
            state2[0, 0] = true;
            state2[0, 1] = true;
            state2[1, 1] = true;
            state2[1, 2] = true;
            states.Add(1, state2);
        }
    }
}