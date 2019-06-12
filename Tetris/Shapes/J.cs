namespace Tetris.Shapes
{
    class J : BaseShape
    {
        public J(int tileId) : base(tileId)
        {
            /* ooo
             * xxx
             * oox
             */
            bool[,] state1 = new bool[3, 3];
            state1[0, 1] = true;
            state1[1, 1] = true;
            state1[2, 1] = true;
            state1[2, 2] = true;
            states.Add(0, state1);

            /* oxo
             * oxo
             * xxo
             */
            bool[,] state2 = new bool[3, 3];
            state2[1, 0] = true;
            state2[1, 1] = true;
            state2[0, 2] = true;
            state2[1, 2] = true;
            states.Add(1, state2);

            /* xoo
             * xxx
             * ooo
             */
            bool[,] state3 = new bool[3, 3];
            state3[0, 0] = true;
            state3[0, 1] = true;
            state3[1, 1] = true;
            state3[2, 1] = true;
            states.Add(2, state3);

            /* oxx
             * oxo
             * oxo
             */
            bool[,] state4 = new bool[3, 3];
            state4[1, 0] = true;
            state4[2, 0] = true;
            state4[1, 1] = true;
            state4[1, 2] = true;
            states.Add(3, state4);
        }
    }
}