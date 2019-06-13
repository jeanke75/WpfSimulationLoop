using System.Windows;
using System.Windows.Media;

namespace Tetris
{
    struct Tile
    {
        private readonly Brush fill;
        private readonly Brush lightShade;
        private readonly Brush darkShade;

        public Tile(Color color)
        {
            fill = new SolidColorBrush(color);
            fill.Freeze();
            lightShade = new SolidColorBrush(ChangeColorBrightness(color, 0.65f));
            lightShade.Freeze();
            darkShade = new SolidColorBrush(ChangeColorBrightness(color, -0.30f));
            darkShade.Freeze();
        }

        public void Draw(DrawingContext dc, double x, double y, double size)
        {
            dc.DrawRectangle(fill, null, new Rect(x, y, size, size));

            var borderOffset = size / 10;

            // Top and left shade
            var segments = new LineSegment[5];
            segments[0] = new LineSegment(new Point(x, y), true);
            segments[1] = new LineSegment(new Point(x, y + size), true);
            segments[2] = new LineSegment(new Point(x + borderOffset, y + size - borderOffset), true);
            segments[3] = new LineSegment(new Point(x + borderOffset, y + borderOffset), true);
            segments[4] = new LineSegment(new Point(x + size - borderOffset, y + borderOffset), true);

            var figure = new PathFigure(new Point(x + size, y), segments, true);
            var geo = new PathGeometry(new[] { figure });
            geo.Freeze();
            dc.DrawGeometry(lightShade, null, geo);

            // Bottom and right shade
            var segments2 = new LineSegment[5];
            segments2[0] = new LineSegment(new Point(x + size, y), true);
            segments2[1] = new LineSegment(new Point(x + size, y + size), true);
            segments2[2] = new LineSegment(new Point(x, y + size), true);
            segments2[3] = new LineSegment(new Point(x + borderOffset, y + size - borderOffset), true);
            segments2[4] = new LineSegment(new Point(x + size - borderOffset, y + size - borderOffset), true);

            var figure2 = new PathFigure(new Point(x + size - borderOffset, y + borderOffset), segments2, true);
            var geo2 = new PathGeometry(new[] { figure2 });
            geo2.Freeze();
            dc.DrawGeometry(darkShade, null, geo2);
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