using DrawingBase;
using Shared.Noise;
using System;
using System.Windows;
using System.Windows.Media;

namespace MarchingSquares
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private readonly int rez = 16; // distance between cornerPixels
        private int rows;
        private int cols;
        float[,] field;

        private readonly Brush vertexBrush = Brushes.White;
        private readonly Pen wallPen = new Pen(Brushes.Red, 1);

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetFps(1);

            rows = 1 + GetHeight() / rez;
            cols = 1 + GetWidth() / rez;
            field = new float[cols, rows];

            for (int i = 0; i < cols - 1; i++)
            {
                for (int j = 0; j < rows - 1; j++)
                {
                    field[i, j] = Simplex.CalcPixel2D(i, j, 1);
                }
            }
        }

        public override void Update(float dt)
        {
        }

        public override void Draw(DrawingContext dc)
        {
            /*for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    dc.PushOpacity(field[i, j]);
                    dc.DrawEllipse(vertexBrush, null, new Point(i * rez, j * rez), rez * 0.1, rez * 0.1);
                    dc.Pop();
                }
            }*/

            for (int i = 0; i < cols - 1; i++)
            {
                for (int j = 0; j < rows - 1; j++)
                {
                    float x = i * rez;
                    float y = j * rez;
                    Point a = new Point(x + rez * 0.5, y);
                    Point b = new Point(x + rez, y + rez * 0.5);
                    Point c = new Point(x + rez * 0.5, y + rez);
                    Point d = new Point(x, y + rez * 0.5);
                    

                    int state = GetState((int)Math.Round(field[i, j]), (int)Math.Round(field[i + 1, j]), (int)Math.Round(field[i + 1, j + 1]), (int)Math.Round(field[i, j + 1]));

                    switch (state)
                    {
                        case 1:
                        case 14:
                            dc.DrawLine(wallPen, c, d);
                            break;
                        case 2:
                        case 13:
                            dc.DrawLine(wallPen, b, c);
                            break;
                        case 3:
                        case 12:
                            dc.DrawLine(wallPen, b, d);
                            break;
                        case 4:
                        case 11:
                            dc.DrawLine(wallPen, a, b);
                            break;
                        case 5:
                            dc.DrawLine(wallPen, a, d);
                            dc.DrawLine(wallPen, b, c);
                            break;
                        case 6:
                        case 9:
                            dc.DrawLine(wallPen, a, c);
                            break;
                        case 7:
                        case 8:
                            dc.DrawLine(wallPen, a, d);
                            break;
                        case 10:
                            dc.DrawLine(wallPen, a, b);
                            dc.DrawLine(wallPen, c, d);
                            break;
                    }
                }
            }
        }

        public override void Cleanup()
        {
        }

        private int GetState(int a, int b, int c, int d)
        {
            return a * 8 + b * 4 + c * 2 + d;
        }
    }
}