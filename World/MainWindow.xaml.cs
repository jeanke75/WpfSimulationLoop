using DrawingBase;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using World.Generator;

namespace World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private Image terrain;

        public override void Initialize()
        {
            int size = 1024;
            SetResolution(size, size);

            float[,] heightmap = WorldGenerator.GenerateIsland(size, size);

            terrain = GenerateImage(heightmap);
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
                            fill = ChangeColorBrightness(Colors.DarkBlue, (data[x, y] + 0.4f) / 2f);
                        else if (data[x, y] < 0)
                            fill = ChangeColorBrightness(Colors.Blue, data[x, y] * 1.5f);
                        else if (data[x, y] < 0.03)
                            fill = Colors.SandyBrown;
                        else if (data[x, y] < 0.10)
                            fill = Colors.SaddleBrown;
                        else if (data[x, y] < 0.20)
                            fill = Colors.ForestGreen;
                        else if (data[x, y] < 0.30)
                            fill = Colors.Green;
                        else if (data[x, y] < 0.45)
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