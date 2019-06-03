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
    }
}