using DrawingBase;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private static readonly Random random = new Random();
        private static readonly int pixelSize = 5;
        private Image terrain;

        public override void Initialize()
        {
            int size = 100;
            SetResolution(size * pixelSize, size * pixelSize);

            float[,] gradient = GenerateGradient(size, size);
            float[,] noise = GenerateNoise(size, size);
            float[,] tiles = GenerateTerrain(noise, gradient);

            terrain = GenerateImage(tiles);
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawImage(terrain.Source, new Rect(0, 0, GetWidth(), GetHeight()));
        }

        public override void Cleanup()
        {
        }

        private float[,] GenerateGradient(int width, int height)
        {
            float[,] gradientTmp = new float[width, height];

            // Fill array with gradient
            int centerX = width / 2 - 1;
            int centerY = height / 2 - 1;

            // Take the shortest distance from center to edge as the gradient radius
            double gradientRange = centerX <= centerY ? centerX : centerY;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double distanceX = (centerX - x) * (centerX - x);
                    double distanceY = (centerY - y) * (centerY - y);

                    double distanceToCenter = Math.Sqrt(distanceX + distanceY);

                    distanceToCenter /= gradientRange;

                    // Clamp to range 0 - 1
                    //distanceToCenter = distanceToCenter > 1 ? 1 : distanceToCenter;

                    gradientTmp[x, y] = (float)distanceToCenter;
                }
            }

            return gradientTmp;
        }

        private float[,] GenerateNoise(int width, int height)
        {
            float[,] noiseTmp = new float[width, height];

            float rng = random.Next(5000000, 10000000) / 100000000f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseTmp[x, y] = Simplex.CalcPixel2D(x, y, rng) / 255f;
                }
            }

            return noiseTmp;
        }

        private float[,] GenerateTerrain(float[,] noise, float[,] gradient)
        {
            float[,] terrainTmp = new float[gradient.GetLength(0), gradient.GetLength(1)];
            for (int x = 0; x < terrainTmp.GetLength(0); x++)
            {
                for (int y = 0; y < terrainTmp.GetLength(1); y++)
                {
                    terrainTmp[x, y] = noise[x, y] - gradient[x, y];
                }
            }

            return terrainTmp;
        }

        private Image GenerateImage(float[,] data)
        {
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    for (int y = 0; y < data.GetLength(1); y++)
                    {
                        if (data[x, y] <= -0.25)
                            dc.DrawRectangle(Brushes.DarkBlue, null, new Rect(x * pixelSize, y * pixelSize, pixelSize, pixelSize));
                        else if (data[x, y] < 0)
                            dc.DrawRectangle(Brushes.Blue, null, new Rect(x * pixelSize, y * pixelSize, pixelSize, pixelSize));
                        else if (data[x, y] < 0.15)
                            dc.DrawRectangle(Brushes.SandyBrown, null, new Rect(x * pixelSize, y * pixelSize, pixelSize, pixelSize));
                        else if (data[x, y] < 0.45)
                            dc.DrawRectangle(Brushes.ForestGreen, null, new Rect(x * pixelSize, y * pixelSize, pixelSize, pixelSize));
                        else if (data[x, y] < 0.65)
                            dc.DrawRectangle(Brushes.Gray, null, new Rect(x * pixelSize, y * pixelSize, pixelSize, pixelSize));
                        else
                            dc.DrawRectangle(Brushes.Snow, null, new Rect(x * pixelSize, y * pixelSize, pixelSize, pixelSize));
                    }
                }
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(GetWidth(), GetHeight(), 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(dv);
            Image img = new Image
            {
                Source = renderTargetBitmap
            };

            return img;
        }
    }
}