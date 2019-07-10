using DrawingBase.Input;
using System;
using System.Windows;
using System.Windows.Media;

namespace MarshCrossing
{
    class Slider
    {
        public Point position;
        private readonly double handleRadius;
        private bool mouseOver;
        private bool mouseDown;

        public Slider(double x, double y, double handleRadius)
        {
            position = new Point(x, y);
            this.handleRadius = handleRadius;
            mouseOver = false;
            mouseDown = false;
        }

        public void Update(MouseHelper mouse, double scale)
        {
            var mousePos = mouse.GetPosition();

            mouseOver = Math.Pow(mousePos.X / scale - position.X, 2) + Math.Pow(mousePos.Y / scale - position.Y, 2) <= handleRadius * handleRadius;
            if (mouseOver)
            {
                mouseDown = mouse.GetState(MouseButton.Left) == ButtonState.Down;
                if (mouseDown)
                {
                    position.Y = mouse.GetPosition().Y / scale;
                }
            }
        }

        public void Draw(DrawingContext dc)
        {
            dc.DrawEllipse(GetColor(), null, position, handleRadius, handleRadius);
        }

        private Brush GetColor()
        {
            if (mouseOver)
            {
                if (mouseDown)
                {
                    return Brushes.Green;
                }
                return Brushes.Gray;
            }
            return Brushes.Black;
        }
    }
}