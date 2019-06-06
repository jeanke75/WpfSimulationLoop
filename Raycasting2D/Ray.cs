using System;
using System.Windows;

namespace Raycasting2D
{
    class Ray
    {
        public Point pos;
        public Vector dir;

        public Ray(Point pos, double angleInRadians)
        {
            this.pos = pos;
            dir = new Vector
            {
                X = Math.Sin(angleInRadians),
                Y = Math.Cos(angleInRadians)
            };
        }

        public Ray(Point pos, Point throughPoint)
        {
            this.pos = pos;
            dir = new Vector
            {
                X = throughPoint.X - pos.X,
                Y = throughPoint.Y - pos.Y
            };
        }

        public Point? Cast(Boundary wall)
        {
            var x1 = wall.p1.X;
            var y1 = wall.p1.Y;
            var x2 = wall.p2.X;
            var y2 = wall.p2.Y;

            var x3 = pos.X;
            var y3 = pos.Y;
            var x4 = pos.X + dir.X;
            var y4 = pos.Y + dir.Y;

            var den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (den == 0)
            {
                return null;
            }

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
            var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;

            if (t > 0 && t < 1 && u > 0)
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