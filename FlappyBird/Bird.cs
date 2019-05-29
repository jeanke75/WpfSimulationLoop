using DrawingBase;
using System.Windows;
using System.Windows.Media;

namespace FlappyBird
{
    class Bird
    {
        private Rect area;
        public Rect Area { get { return area; } }

        private double velocity;
        private double acceleration;

        private readonly double maxJumpVelocity = -5d;
        private readonly Brush birdColor = new SolidColorBrush(Colors.Yellow);

        public Bird(double x, double y, double size)
        {
            area = new Rect(x, y, size, size);
            ClearForces();
            birdColor.Freeze();
        }

        public void Update(float dt)
        {
            area.Y += velocity * 60 * dt;
            velocity += acceleration;
            if (velocity < maxJumpVelocity)
                velocity = maxJumpVelocity;
            acceleration = 0;
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawRectangle(birdColor, null, area);
        }

        public void AddForce(double force)
        {
            acceleration += force;
        }

        private void ClearForces()
        {
            acceleration = 0d;
            velocity = 0d;
        }

        public void EnforceBoundaries(double top, double bottom)
        {
            if (area.Y < top)
            {
                area.Y = top;
                ClearForces();
            }
            else if (area.Bottom > bottom)
            {
                area.Y = bottom - area.Height;
                ClearForces();
            }
        }
    }
}