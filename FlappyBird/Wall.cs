using System.Windows;
using System.Windows.Media;

namespace FlappyBird
{
    class Wall
    {
        private Rect area;
        public Rect Area { get { return area; } }

        private readonly double speed = 3.0d;
        private readonly Brush color = new SolidColorBrush(Colors.Brown);
        private readonly Pen hitColor = new Pen(new SolidColorBrush(Colors.Red), 2);
        private bool hit;

        public Wall(double x, double y, double width, double height)
        {
            area = new Rect(x, y, width, height);
            color.Freeze();
            hit = false;
        }

        public void Update(float dt)
        {
            area.X -= speed * 60 * dt;
        }

        public void Draw(DrawingContext dc)
        {
            if (hit)
            {
                dc.DrawRectangle(color, hitColor, area);
            }
            else
            {
                dc.DrawRectangle(color, null, area);
            }
        }

        public bool Collision(Bird bird)
        {
            hit = bird.Area.IntersectsWith(Area);
            return hit;
        }
    }
}