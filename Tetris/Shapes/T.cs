namespace Tetris.Shapes
{
    class T : BaseShape
    {
        public T(int tileId) : base(tileId)
        {
            /* ooo
             * xxx
             * oxo
             */
            bool[,] state1 = new bool[3, 3];
            state1[0, 1] = true;
            state1[1, 1] = true;
            state1[2, 1] = true;
            state1[1, 2] = true;
            states.Add(0, state1);

            /* oxo
             * xxo
             * oxo
             */
            bool[,] state2 = new bool[3, 3];
            state2[1, 0] = true;
            state2[0, 1] = true;
            state2[1, 1] = true;
            state2[1, 2] = true;
            states.Add(1, state2);

            /* oxo
             * xxx
             * ooo
             */
            bool[,] state3 = new bool[3, 3];
            state3[1, 0] = true;
            state3[0, 1] = true;
            state3[1, 1] = true;
            state3[2, 1] = true;
            states.Add(2, state3);

            /* oxo
             * oxx
             * oxo
             */
            bool[,] state4 = new bool[3, 3];
            state4[1, 0] = true;
            state4[1, 1] = true;
            state4[2, 1] = true;
            state4[1, 2] = true;
            states.Add(3, state4);
        }
    }
}