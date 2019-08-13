using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Raycasting2D
{
    class BoundaryEdgeRayEmitter
    {
        public Point pos;
        private readonly HashSet<Point> pointsOfInterest;
        private readonly List<Ray> rays;
        private readonly List<Point> collisionPoints;

        private readonly Brush emitterBrush = Brushes.Yellow;
        private readonly Pen rayPen = new Pen(Brushes.LightGoldenrodYellow, 2);
        private readonly Brush collisionPointBrush = Brushes.Red;

        public BoundaryEdgeRayEmitter(double x, double y)
        {
            pos = new Point(x, y);
            pointsOfInterest = new HashSet<Point>();
            rays = new List<Ray>();
            collisionPoints = new List<Point>();

            emitterBrush.Freeze();
            rayPen.Freeze();
            collisionPointBrush.Freeze();
        }

        public void Update(Boundary[] boundaries)
        {
            // Add a POI for both edgepoints of a boundary
            pointsOfInterest.Clear();
            foreach (Boundary b in boundaries)
            {
                pointsOfInterest.Add(b.p1);
                pointsOfInterest.Add(b.p2);
            }

            // Add rays for each POI
            rays.Clear();
            foreach (Point p in pointsOfInterest)
            {
                // Add a ray for the POI (they will NOT collide with this point)
                Ray r = new Ray(pos, p);
                rays.Add(r);

                // Add a ray left and right of the POI (one of them should collide just around the POI)
                double angle = -Math.Atan2(r.dir.Y, r.dir.X) + Math.PI / 2;
                rays.Add(new Ray(pos, angle + 0.000001));
                rays.Add(new Ray(pos, angle - 0.000001));
            }

            // Sort the rays by angle (needed for properly drawing the lit area)
            rays.Sort((x, y) => Math.Atan2(x.dir.Y, x.dir.X).CompareTo(Math.Atan2(y.dir.Y, y.dir.X)));

            // Calculate the closest collision point of each ray
            collisionPoints.Clear();
            foreach (Ray ray in rays)
            {
                var record = double.MaxValue;
                Point? closest = null;
                foreach (Boundary boundary in boundaries)
                {
                    Point? p = ray.Cast(boundary);
                    if (p != null)
                    {
                        Point pt = p.Value;
                        double dist = Math.Pow(pt.X - pos.X, 2) + Math.Pow(pt.Y - pos.Y, 2);
                        if (dist < record)
                        {
                            record = dist;
                            closest = p;
                        }

                    }
                }
                if (closest != null)
                {
                    // Add the closest point as a collision point
                    collisionPoints.Add(closest.GetValueOrDefault());
                }
            }
        }

        public void Draw(DrawingContext dc)
        {
            // Connect the collision points and fill the area
            dc.PushOpacity(0.2);
            if (collisionPoints.Count >= 3)
            {
                var segments = new LineSegment[collisionPoints.Count - 1];
                for (int i = 0; i < collisionPoints.Count - 1; i++)
                {
                    segments[i] = new LineSegment(collisionPoints[i], true);
                }
                var figure = new PathFigure(collisionPoints[collisionPoints.Count - 1], segments, true);
                var geo = new PathGeometry(new[] { figure });
                geo.Freeze();
                dc.DrawGeometry(emitterBrush, null, geo);
            }
            dc.Pop();

            // Draw a ray from the emitter to each collision point
            dc.PushOpacity(0.1);
            foreach (Point p in collisionPoints)
            {
                dc.DrawLine(rayPen, pos, p);
            }
            dc.Pop();

            // Draw the collision points
            dc.PushOpacity(0.5);
            foreach (Point p in collisionPoints)
            {
                dc.DrawEllipse(collisionPointBrush, null, p, 2, 2);
            }
            dc.Pop();

            // Draw the emitter
            dc.DrawEllipse(emitterBrush, null, pos, 4, 4);
        }
    }
}