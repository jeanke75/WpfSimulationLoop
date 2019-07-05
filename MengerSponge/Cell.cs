using System.Windows;
using System.Windows.Media;

namespace MengerSponge
{
    readonly struct Cell
    {
        public readonly Rect rect;

        public Cell(double x, double y, double size)
        {
            rect = new Rect(x, y, size, size);
        }

        public void Draw(DrawingContext dc, Brush fillBrush, Pen outlinePen)
        {
            dc.DrawRectangle(fillBrush, outlinePen, rect);
        }
    }
}