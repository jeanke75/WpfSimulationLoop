using System.Windows;
using System.Windows.Media;

namespace DrawingExample.Models
{
    public class Circle : Entity
    {
        public Circle(Vector position) : base(position)
        {
            var collection = new GradientStopCollection
            {
                new GradientStop(Colors.DarkRed, 1/8f),
                new GradientStop(Colors.DarkOrange, 2/8f),
                new GradientStop(Colors.Yellow, 3/8f),
                new GradientStop(Colors.ForestGreen, 4/8f),
                new GradientStop(Colors.Blue, 5/8f),
                new GradientStop(Colors.Indigo, 6/8f),
                new GradientStop(Colors.BlueViolet, 7/8f)
            };

            fill = new LinearGradientBrush(collection);
            outline = null;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushOpacity(opacity / 255f);
            dc.DrawEllipse(fill, outline, new Point(position.X, position.Y), Size, Size);
            dc.Pop();
        }
    }
}