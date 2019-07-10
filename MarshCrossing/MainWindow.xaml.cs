using DrawingBase;
using DrawingBase.Input;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace MarshCrossing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This is a visualisation for https://projecteuler.net/problem=607, not a solution!
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private Point start;
        private Point end;
        private readonly Region[] marsh = new Region[5];
        private readonly Slider[] sliders = new Slider[5];
        private readonly MouseHelper mouse = new MouseHelper();

        private double travelTime;

        private readonly double scale = 10;

        public override void Initialize()
        {
            //SetBackgroundColor(Colors.White);

            var distanceStartToEnd = 100;
            var regionWidth = 10;
            var diagonalDistanceToMarsh = distanceStartToEnd - (Math.Sqrt(2 * regionWidth * regionWidth) * 5);
            var marshOffset = Math.Sqrt(diagonalDistanceToMarsh * diagonalDistanceToMarsh / 2d);
            SetResolution((int)Math.Ceiling((marshOffset + 5 * regionWidth) * scale), (int)Math.Ceiling((marshOffset + 5 * regionWidth) * scale));

            start = new Point(0, 0);
            end = new Point(marshOffset + 5 * regionWidth, marshOffset + 5 * regionWidth);

            marsh[0] = new Region(marshOffset, regionWidth, 9, new Color
            {
                R = 255,
                G = 255,
                B = 229,
                A = 255
            });
            marsh[1] = new Region(marshOffset + regionWidth, regionWidth, 8, new Color
            {
                R = 255,
                G = 255,
                B = 179,
                A = 255
            });
            marsh[2] = new Region(marshOffset + 2 * regionWidth, regionWidth, 7, new Color
            {
                R = 255,
                G = 230,
                B = 179,
                A = 255
            });
            marsh[3] = new Region(marshOffset + 3 * regionWidth, regionWidth, 6, new Color
            {
                R = 255,
                G = 204,
                B = 153,
                A = 255
            });
            marsh[4] = new Region(marshOffset + 4 * regionWidth, regionWidth, 5, new Color
            {
                R = 255,
                G = 190,
                B = 192,
                A = 255
            });

            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i] = new Slider(marshOffset + i * regionWidth, marshOffset + i * regionWidth, 3);
            }
        }

        public override void Update(float dt)
        {
            mouse.Update();
            foreach (Slider s in sliders)
            {
                s.Update(mouse, scale);
            }

            var travelTimeTmp = DistanceBetweenPoints(sliders[0].position, start) / 10d;
            for (int i = 0; i < sliders.Length - 1; i++)
            {
                travelTimeTmp += DistanceBetweenPoints(sliders[i + 1].position, sliders[i].position) / marsh[i].leaguesPerDay;
            }
            travelTimeTmp += DistanceBetweenPoints(end, sliders[sliders.Length - 1].position) / marsh[marsh.Length - 1].leaguesPerDay;
            travelTime = travelTimeTmp;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushTransform(new ScaleTransform(scale, scale));

            // Draw the regions
            foreach (Region r in marsh)
            {
                r.Draw(dc, GetHeight()); ;
            }

            // Connect the points
            dc.DrawLine(new Pen(Brushes.DarkGray, 2), start, sliders[0].position);
            for (int i = 1; i < sliders.Length; i++)
            {
                dc.DrawLine(new Pen(Brushes.DarkGray, 2), sliders[i - 1].position, sliders[i].position);
            }
            dc.DrawLine(new Pen(Brushes.DarkGray, 2), sliders[sliders.Length - 1].position, end);

            // Draw the Points
            dc.DrawEllipse(Brushes.Black, null, start, 3, 3);
            foreach (Slider s in sliders)
            {
                s.Draw(dc);
            }
            dc.DrawEllipse(Brushes.Black, null, end, 3, 3);
            dc.Pop();
            var t = new FormattedText(travelTime.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Georgia"), 10, Brushes.White, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(t, new Point(GetWidth() - t.Width, 0));
        }

        public override void Cleanup()
        {
        }

        private double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X -p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}