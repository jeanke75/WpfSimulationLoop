using System.Windows;
using System.Windows.Media;

namespace DrawingExample.Models
{
    public class Square : Entity
    {
        public Square(Vector position) : base(position)
        {
            fill = Brushes.Orange;
            outline = null;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushOpacity(opacity / 255f);
            dc.DrawRectangle(fill, outline, new Rect(position.X, position.Y, Size, Size));
            dc.Pop();
        }
    }
}