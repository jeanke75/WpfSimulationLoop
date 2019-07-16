using DrawingBase;
using System;
using System.Windows;
using System.Windows.Media;

namespace World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private static readonly Random random = new Random();
        private float[,] tiles;

        public override void Initialize()
        {
            int size = 150;
            SetResolution(size, size);

            tiles = new float[GetWidth(), GetHeight()];

            // Fill array with gradient
            int centerX = GetWidth() / 2 - 1;
            int centerY = GetHeight() / 2 - 1;

            for (int x = 0; x < GetWidth(); x++)
            {
                for (int y = 0; y < GetHeight(); y++)
                {
                    double distanceX = (centerX - x) * (centerX - x);
                    double distanceY = (centerY - y) * (centerY - y);

                    double distanceToCenter = Math.Sqrt(distanceX + distanceY);

                    distanceToCenter /= Math.Sqrt(centerX * centerX + centerY * centerY);

                    tiles[x, y] = Perlin.Noise((float)(x + random.NextDouble()), (float)(y + random.NextDouble())) - (float)distanceToCenter;
                }
            }
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(DrawingContext dc)
        {
            for (int x = 0; x < GetWidth(); x++)
            {
                for (int y = 0; y < GetHeight(); y++)
                {
                    if (tiles[x, y] < 0)
                        dc.DrawRectangle(Brushes.Blue, null, new Rect(x, y, 1, 1));
                    else if (tiles[x, y] < 0.1)
                        dc.DrawRectangle(Brushes.SandyBrown, null, new Rect(x, y, 1, 1));
                    else if (tiles[x, y] < 0.5)
                        dc.DrawRectangle(Brushes.ForestGreen, null, new Rect(x, y, 1, 1));
                    else if (tiles[x, y] < 0.5)
                        dc.DrawRectangle(Brushes.Gray, null, new Rect(x, y, 1, 1));
                    else
                        dc.DrawRectangle(Brushes.Snow, null, new Rect(x, y, 1, 1));
                }
            }
        }

        public override void Cleanup()
        {
        }
    }
}