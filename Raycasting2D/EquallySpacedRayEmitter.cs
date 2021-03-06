﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Raycasting2D
{
    class EquallySpacedRayEmitter
    {
        public Point pos;
        private readonly Ray[] rays;
        private readonly List<Point> collisionPoints;

        private readonly Brush emitterBrush = Brushes.Yellow;
        private readonly Pen rayPen = new Pen(Brushes.LightGoldenrodYellow, 2);
        private readonly Brush collisionPointBrush = Brushes.Red;

        public EquallySpacedRayEmitter(double x, double y)
        {
            pos = new Point(x, y);

            var tmpRays = new List<Ray>();
            for (int i = 0; i < 360; i++)
            {
                tmpRays.Add(new Ray(pos, (Math.PI / 180d) * i));
            }
            rays = tmpRays.ToArray();

            collisionPoints = new List<Point>();

            emitterBrush.Freeze();
            rayPen.Freeze();
            collisionPointBrush.Freeze();
        }

        public void Update(Boundary[] boundaries)
        {
            // Update the pos of each ray
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i].pos = pos;
            }

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