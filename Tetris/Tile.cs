using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Tetris
{
    struct Tile
    {
        private readonly Brush fill;
        private readonly Pen outline;

        public Tile(Color color)
        {
            fill = new SolidColorBrush(color);
            fill.Freeze();
            outline = new Pen(new SolidColorBrush(ChangeColorBrightness(color, 0.75f)), 1);
            outline.Freeze();
        }

        public void Draw(DrawingContext dc, double x, double y, double size)
        {
            dc.DrawRectangle(fill, outline, new Rect(x, y, size, size));
        }

        private static Color ChangeColorBrightness(Color color, float correctionFactor)
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