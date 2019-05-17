using DrawingBase;
using System;
using System.Windows;
using System.Windows.Media;

namespace Flocking
{
    public class Boid
    {
        public Vector position;
        public Vector direction;
        private readonly double speed = 20d;

        public Boid(Vector position, Vector direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public void Update(float dt)
        {
            direction.Normalize();
            var destination = position + direction;
            //position = position.MoveTowards(destination, dt, speed);
        }

        public void Draw(DrawingContext dc)
        {
            dc.PushTransform(new TranslateTransform(position.X, position.Y));
            double angle = Math.Atan2(direction.Y, direction.X) * 180 / Math.PI;
            /*dc.PushTransform(new RotateTransform(-angle + 90));
            dc.DrawEquilateralTriangle(Brushes.Yellow, null, 20, true);
            dc.Pop();*/


            var direc = direction.SetMagnitude(10);
            dc.DrawLine(new Pen(Brushes.White, 1), new Point(0, 0), new Point(direc.X, direc.Y));
            dc.DrawRectangle(Brushes.Red, null, new Rect(new Point(0, 0), new Size(2, 2)));
            dc.DrawRectangle(Brushes.Orange, null, new Rect(new Point(direction.X, direction.Y), new Size(2, 2)));
            dc.DrawRectangle(Brushes.Green, null, new Rect(new Point(direc.X, direc.Y), new Size(2, 2)));
            //dc.DrawRectangle(Brushes.Red, null, new Rect(new Point(direction.X, direction.Y), new Size(2, 2)));
            dc.Pop();
        }

        public void AddForce(Vector force)
        {
            direction += force;
        }

        public Boid Copy()
        {
            return new Boid(new Vector(position.X, position.Y), new Vector(direction.X, direction.Y));
        }
    }
}