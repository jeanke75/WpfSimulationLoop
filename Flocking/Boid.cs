using DrawingBase;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Flocking
{
    public class Boid
    {
        public Vector position;
        public Vector velocity;
        public Vector acceleration;
        private static readonly double maxForce = 0.2;
        private static readonly double maxSpeed = 6;

        public Boid(Vector position)
        {
            this.position = position;
            var angle = MainWindow.random.NextDouble() * Math.PI * 2;
            velocity = new Vector(Math.Cos(angle), Math.Sin(angle));
            velocity.SetMagnitude(MainWindow.random.NextDouble() * 2 + 2);
            acceleration = new Vector(0, 0);
        }

        public Boid(Vector position, Vector velocity, Vector acceleration)
        {
            this.position = position;
            this.velocity = velocity;
            this.acceleration = acceleration;
        }

        public void Update(float dt)
        {
            position += velocity;
            velocity += acceleration;
            if (velocity.Length > maxSpeed)
                velocity.SetMagnitude(maxSpeed);
            acceleration *= 0;
        }

        public void Draw(DrawingContext dc)
        {
            dc.PushTransform(new TranslateTransform(position.X, position.Y));
            double angle = Math.Atan2(velocity.Y, velocity.X) * 180 / Math.PI;
            dc.PushTransform(new RotateTransform(angle + 90));
            dc.DrawEquilateralTriangle(Brushes.Yellow, null, 20, true);
            dc.Pop();

            var direc = velocity.SetMagnitude(20);
            dc.DrawLine(new Pen(Brushes.White, 1), new Point(0, 0), new Point(direc.X, direc.Y));
            dc.DrawRectangle(Brushes.Red, null, new Rect(new Point(-1.5, -1.5), new Size(3, 3)));
            dc.DrawRectangle(Brushes.Green, null, new Rect(new Point(direc.X - 2.5, direc.Y - 2.5), new Size(5, 5)));
            dc.Pop();
        }

        public Boid Flock(List<Boid> boids)
        {
            var separation = GetSeparationForce(boids);
            var alignment = GetAlignmentForce(boids);
            var cohesion = GetCohesionForce(boids);
            Boid copy = Copy();
            copy.acceleration += separation + alignment + cohesion;
            return copy;
        }

        private Vector GetSeparationForce(List<Boid> boids)
        {
            double radius = 50d;
            Vector steering = new Vector(0, 0);
            double total = 0;
            foreach (Boid otherBoid in boids)
            {
                if (otherBoid == this) continue;
                double dist = Math.Sqrt(Math.Pow(otherBoid.position.X - position.X, 2) + Math.Pow(otherBoid.position.Y - position.Y, 2));
                if (dist > radius) continue;
                Vector diff = position - otherBoid.position;
                diff /= Math.Pow(dist, 2);
                steering += diff;
                total++;
            }

            if (total > 0)
            {
                steering /= total;
                steering = steering.SetMagnitude(maxSpeed);
                steering -= velocity;
                if (steering.Length > maxForce)
                    steering = steering.SetMagnitude(maxForce);
            }
            return steering;
        }

        private Vector GetAlignmentForce(List<Boid> boids)
        {
            double radius = 50d;
            Vector steering = new Vector(0, 0);
            double total = 0;
            foreach (Boid otherBoid in boids)
            {
                if (otherBoid == this) continue;
                double dist = Math.Sqrt(Math.Pow(otherBoid.position.X - position.X, 2) + Math.Pow(otherBoid.position.Y - position.Y, 2));
                if (dist > radius) continue;
                steering += otherBoid.velocity;
                total++;
            }

            if (total > 0)
            {
                steering /= total;
                steering = steering.SetMagnitude(maxSpeed);
                steering -= velocity;
                if (steering.Length > maxForce)
                    steering = steering.SetMagnitude(maxForce);
            }
            return steering;
        }

        private Vector GetCohesionForce(List<Boid> boids)
        {
            double radius = 50d;
            Vector steering = new Vector(0, 0);
            double total = 0;
            foreach (Boid otherBoid in boids)
            {
                if (otherBoid == this) continue;
                double dist = Math.Sqrt(Math.Pow(otherBoid.position.X - position.X, 2) + Math.Pow(otherBoid.position.Y - position.Y, 2));
                if (dist > radius) continue;
                steering += otherBoid.position;
                total++;
            }

            if (total > 0)
            {
                steering /= total;
                steering -= position;
                steering = steering.SetMagnitude(maxSpeed);
                steering -= velocity;
                if (steering.Length > maxForce)
                    steering = steering.SetMagnitude(maxForce);
            }
            return steering;
        }

        public Boid Copy()
        {
            return new Boid(new Vector(position.X, position.Y), new Vector(velocity.X, velocity.Y), new Vector(acceleration.X, acceleration.Y));
        }
    }
}