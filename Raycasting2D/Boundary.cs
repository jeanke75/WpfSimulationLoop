using System.Windows;
using System.Windows.Media;

namespace Raycasting2D
{
    struct Boundary
    {
        public Point p1;
        public Point p2;

        public Boundary(double x1, double y1, double x2, double y2)
        {
            p1 = new Point(x1, y1);
            p2 = new Point(x2, y2);
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawLine(new Pen(Brushes.White, 2), p1, p2);
        }

        public Point? Intersects(Boundary other)
        {
            var x1 = p1.X;
            var y1 = p1.Y;
            var x2 = p2.X;
            var y2 = p2.Y;

            var x3 = other.p1.X;
            var y3 = other.p1.Y;
            var x4 = other.p2.X;
            var y4 = other.p2.Y;

            var den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (den == 0)
            {
                return null;
            }

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
            var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;

            if (t > 0 && t < 1 && u > 0 && u < 1)
            {
                return new Point
                {
                    X = x1 + t * (x2 - x1),
                    Y = y1 + t * (y2 - y1)
                };
            }
            else
            {
                return null;
            }
        }
    }
}