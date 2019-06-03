using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Raycasting2D
{
    class Particle
    {
        public Point pos;
        readonly Ray[] rays;

        public Particle(double x, double y)
        {
            pos = new Point(x, y);
            
            var tmpRays = new List<Ray>();
            for (int i = 0; i < 360; i++)
            {
                tmpRays.Add(new Ray(pos, (Math.PI / 180) * i));
            }
            rays = tmpRays.ToArray();
        }

        public void Update()
        {
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i].pos = pos;
            }
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawEllipse(Brushes.White, null, pos, 4, 4);
        }

        public void Look(Boundary[] boundaries, DrawingContext dc)
        {
            dc.PushOpacity(0.75);
            foreach (Ray ray in rays)
            {
                var record = double.MaxValue;
                Point? closest = null;
                foreach (Boundary boundary in boundaries)
                {
                    Point? p = ray.Cast(boundary);
                    if (p != null)
                    {
                        Point pt = p.GetValueOrDefault();
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
                    dc.DrawLine(new Pen(Brushes.White, 2), pos, closest.GetValueOrDefault());
                }
            }
            dc.Pop();
        }
    }
}