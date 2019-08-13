using DrawingBase;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Flocking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static Random random = new Random();
        private List<Boid> boids = new List<Boid>();

        public override void Initialize()
        {
            for (int i = 0; i < 50; i++)
            {
                var position = new Vector(random.Next(0, GetWidth()), random.Next(0, GetHeight()));
                boids.Add(new Boid(position));
            }
        }

        public override void Update(float dt)
        {
            List<Boid> updatedBoids = new List<Boid>();
            foreach (Boid boid in boids)
            {
                if (boid.position.X < 0)
                    boid.position.X = GetWidth();
                else if (boid.position.X > GetWidth())
                    boid.position.X = 0;
                if (boid.position.Y < 0)
                    boid.position.Y = GetHeight();
                else if (boid.position.Y > GetHeight())
                    boid.position.Y = 0;
                Boid copy = boid.Flock(boids);
                copy.Update(dt);

                updatedBoids.Add(copy);
            }
            boids = updatedBoids;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushOpacity(0.75);
            foreach (Boid boid in boids)
            {
                boid.Draw(dc);
            }
            dc.Pop();
        }

        public override void Cleanup()
        {
            boids.Clear();
        }
    }
}