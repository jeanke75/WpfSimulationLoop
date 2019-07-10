using System.Windows;
using System.Windows.Media;

namespace MarshCrossing
{
    readonly struct Region
    {
        public readonly double x;
        public readonly double width;
        public readonly double leaguesPerDay;
        private readonly Brush fill;

        public Region(double x, double width, double leaguesPerDay, Color color)
        {
            this.x = x;
            this.width = width;
            this.leaguesPerDay = leaguesPerDay;
            fill = new SolidColorBrush(color);
            fill.Freeze();
        }

        public void Draw(DrawingContext dc, double height)
        {
            dc.DrawRectangle(fill, null, new Rect(x, 0, width, height));
        }
    }
}