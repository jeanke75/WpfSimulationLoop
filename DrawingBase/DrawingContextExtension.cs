using System;
using System.Windows;
using System.Windows.Media;

namespace DrawingBase
{
    public static class DrawingContextExtension
    {
        public static void DrawTriangle(this DrawingContext drawingContext, Brush brush, Pen pen, Point point1, Point point2, Point point3)
        {
            var segments = new[]
            {
                new LineSegment(point2, true),
                new LineSegment(point3, true)
            };

            var figure = new PathFigure(point1, segments, true);
            var geo = new PathGeometry(new[] { figure });

            drawingContext.DrawGeometry(brush, pen, geo);
        }

        public static void DrawEquilateralTriangle(this DrawingContext drawingContext, Brush brush, Pen pen, double size, bool centerOfGravityOnOrigin)
        {
            if (centerOfGravityOnOrigin)
            {
                var height = Math.Sqrt(Math.Pow(size, 2) - Math.Pow(size / 2d, 2));
                var top = new Point(0, height * -2 / 3d);
                var bottomLeft = new Point(-size / 2d, height * 1 / 3d);
                var bottomRight = new Point(size / 2d, height * 1 / 3d);

                drawingContext.DrawTriangle(brush, pen, top, bottomLeft, bottomRight);
                drawingContext.DrawRectangle(Brushes.Red, null, new Rect(top.X - 2, top.Y - 2, 4, 4));
            }
            else
            {
                var height = Math.Sqrt(Math.Pow(size, 2) - Math.Pow(size / 2d, 2));
                var top = new Point(size / 2d, 0);
                var bottomLeft = new Point(0, height);
                var bottomRight = new Point(size, height);

                drawingContext.DrawTriangle(brush, pen, top, bottomLeft, bottomRight);
            }
        }
    }
}