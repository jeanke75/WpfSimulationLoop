using DrawingBase;
using System.Windows;
using System.Windows.Media;

namespace DrawingExample.Models
{
    public class Entity
    {
        protected static int Size = 10;
        private static readonly float speed = 30f;
        public Vector position;
        public bool CanMove = true;
        protected int opacity = 255;
        private bool hiding = true;

        public Vector destination;

        protected Brush fill;
        protected Pen outline;

        public Entity(Vector position)
        {
            this.position = position;
            destination = position;
            opacity = MainWindow.random.Next(100, 200);

            fill = null;
            outline = null;
        }

        public virtual void Update(float dt)
        {
            if (destination != position)
            {
                position = position.MoveTowards(destination, dt, speed);
            }

            opacity += hiding ? -5 : 5;
            if (hiding && opacity <= 50)
                hiding = false;
            else if (!hiding && opacity >= 150)
                hiding = true;
        }

        public virtual void Draw(DrawingContext dc)
        {
            dc.PushOpacity(opacity / 255f);
            dc.DrawRectangle(fill, outline, new Rect(position.X, position.Y, Size, Size));
            dc.Pop();
        }
    }
}