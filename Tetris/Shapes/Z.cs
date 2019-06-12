namespace Tetris.Shapes
{
    class Z : BaseShape
    {
        public Z(int tileId) : base(tileId)
        {
            /* ooo
             * xxo
             * oxx
             */
            bool[,] state1 = new bool[3, 3];
            state1[0, 1] = true;
            state1[1, 1] = true;
            state1[1, 2] = true;
            state1[2, 2] = true;
            states.Add(0, state1);

            /* oxo
             * xxo
             * xoo
             */
            bool[,] state2 = new bool[3, 3];
            state2[1, 0] = true;
            state2[0, 1] = true;
            state2[1, 1] = true;
            state2[0, 2] = true;
            states.Add(1, state2);
        }
    }
}