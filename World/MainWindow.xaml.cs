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
        private Image terrain;

        public override void Initialize()
        {
            int size = 1024;
            SetResolution(size, size);

            float[,] gradient = GenerateGradient(size, size, 0, 0, 0.3);
            float[,] noise = Perlin.Noise(size, size, 8);//GenerateNoise(size, size);
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

        private float[,] GenerateGradient(int width, int height, int subgradients, double minCenterOffsetSubgradient, double maxCenterOffsetSubgradient)
        {
            minCenterOffsetSubgradient = Clamp(minCenterOffsetSubgradient, 0, 1);
            maxCenterOffsetSubgradient = Clamp(maxCenterOffsetSubgradient, 0, 1);

            float[,] gradientTmp = new float[width, height];

            // Calculate the midpoint
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

            for (int i = 0; i < subgradients; i++)
            {
                // Generate a random midpoint for the extra gradient
                int extraCenterXOffset = random.Next((int)(centerX * minCenterOffsetSubgradient), (int)(centerX * minCenterOffsetSubgradient));
                int extraCenterYOffset = random.Next((int)(centerY * maxCenterOffsetSubgradient), (int)(centerY * maxCenterOffsetSubgradient));

                double extraCenterX = random.NextDouble() < 0.5 ? centerX - extraCenterXOffset : centerX + extraCenterXOffset;
                double extraCenterY = random.NextDouble() < 0.5 ? centerY - extraCenterYOffset : centerY + extraCenterYOffset;

                // Take the shortest distance from center to edge as the gradient radius
                double minDistanceX = centerX - extraCenterXOffset;
                double minDistanceY = centerY - extraCenterYOffset;
                double extraGradientRange = minDistanceX <= minDistanceY ? minDistanceX : minDistanceY;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double extraDistanceX = (extraCenterX - x) * (extraCenterX - x);
                        double extraDistanceY = (extraCenterY - y) * (extraCenterY - y);

                        double extraDistanceToCenter = Math.Sqrt(extraDistanceX + extraDistanceY);

                        extraDistanceToCenter /= extraGradientRange;

                        // Clamp to range 0 - 1
                        //extraDistanceToCenter = extraDistanceToCenter > 1 ? 1 : extraDistanceToCenter;

                        gradientTmp[x, y] = gradientTmp[x, y] <= extraDistanceToCenter ? gradientTmp[x, y] : (float)extraDistanceToCenter;
                    }
                }
            }

            return gradientTmp;
        }

        private float[,] GenerateNoise(int width, int height)
        {
            float[,] noiseTmp = new float[width, height];

            float rng = random.Next(2, 5) / 1000f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseTmp[x, y] = Simplex.CalcPixel2D(x, y, rng);
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
                        Color fill;// = ChangeColorBrightness(Colors.Gray, data[x, y] * 2 - 1);
                        if (data[x, y] <= -0.25)
                            fill = fill = ChangeColorBrightness(Colors.DarkBlue, (data[x, y] + 0.4f) / 2f);
                        else if (data[x, y] < 0)
                            fill = ChangeColorBrightness(Colors.Blue, data[x, y] * 1.5f);
                        else if (data[x, y] < 0.05)
                            fill = Colors.SandyBrown;
                        else if (data[x, y] < 0.125)
                            fill = Colors.SaddleBrown;
                        else if (data[x, y] < 0.3)
                            fill = Colors.ForestGreen;
                        else if (data[x, y] < 0.45)
                            fill = Colors.Green;
                        else if (data[x, y] < 0.7)
                            fill = ChangeColorBrightness(Colors.Gray, 0.7f - data[x, y]);
                        else
                            fill = Colors.Snow;

                        dc.DrawRectangle(new SolidColorBrush(fill), null, new Rect(x, y, 1, 1));
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

        private double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        private Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}