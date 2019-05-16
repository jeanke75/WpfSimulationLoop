using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DrawingBase
{
    public abstract class DrawingWindowBase : Window
    {
        private readonly Canvas CvsDraw;
        private readonly DispatcherTimer timer;
        private int fps = 60;
        private DateTime prev;

        private int ticks = 0;
        private TimeSpan updateMs = new TimeSpan();
        private TimeSpan drawMs = new TimeSpan();
        private TimeSpan avgUpdateMs = new TimeSpan();
        private TimeSpan avgDrawMs = new TimeSpan();

        public bool DisplayInfo { get; set; }

        public DrawingWindowBase() : this(600, 800, 60) { }

        public DrawingWindowBase(int height, int width, int fps)
        {
            Width = width;
            Height = height;
            CvsDraw = new Canvas
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Black
            };
            AddChild(CvsDraw);

            timer = new DispatcherTimer();
            timer.Tick += GameTick;
            SetFps(fps);
            DisplayInfo = true;

            Loaded += SimulationBase_Loaded;
        }

        ~DrawingWindowBase()
        {
            Cleanup();
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= GameTick;
            }
        }

        private void SimulationBase_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        public void Init()
        {
            Initialize();
            prev = DateTime.Now;
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now - prev;
            float dt = span.Milliseconds / 1000f;
            prev = now;

            // update
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Update(dt);
            sw.Stop();
            updateMs += sw.Elapsed;

            // draw
            sw.Restart();
            CvsDraw.Children.Clear();
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Draw(dc);

                if (ticks == fps)
                {
                    avgUpdateMs = new TimeSpan(updateMs.Ticks / fps);
                    avgDrawMs = new TimeSpan(drawMs.Ticks / fps);
                }

                if (DisplayInfo)
                    ShowInfo(dc);
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)CvsDraw.ActualWidth, (int)CvsDraw.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(dv);
            Image img = new Image
            {
                Source = renderTargetBitmap
            };
            CvsDraw.Children.Add(img);
            sw.Stop();
            drawMs += sw.Elapsed;

            if (ticks < fps)
            {
                ticks++;
            }
            else
            {
                ticks = 0;
                updateMs = new TimeSpan();
                drawMs = sw.Elapsed;
            }
        }

        public abstract void Initialize();
        public abstract void Update(float dt);
        public abstract void Draw(DrawingContext dc);
        public abstract void Cleanup();

        public int GetFps()
        {
            return fps;
        }

        public void SetFps(int newFps)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / newFps);
            fps = newFps;
        }

        public void SetResolution(int width, int height)
        {
            Width = width + 16;
            Height = height + 39;
        }

        public int GetWidth()
        {
            return (int)CvsDraw.ActualWidth;
        }

        public int GetHeight()
        {
            return (int)CvsDraw.ActualHeight;
        }

        private void ShowInfo(DrawingContext dc)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            FlowDirection flowDirection = FlowDirection.LeftToRight;
            Typeface typeface = new Typeface("Georgia");
            double emSize = 10;
            Brush color = Brushes.White;
            double dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            dc.DrawText(new FormattedText($"Loop interval: {1000 / fps}ms", cultureInfo, flowDirection, typeface, emSize, color, dpi), new Point(0, 0));
            if (avgUpdateMs.Seconds > 0)
                dc.DrawText(new FormattedText($"Update avg: {avgUpdateMs.Seconds}s{avgUpdateMs.Milliseconds}ms", cultureInfo, flowDirection, typeface, emSize, color, dpi), new Point(0, 10));
            else
                dc.DrawText(new FormattedText($"Update avg: {avgUpdateMs.Milliseconds}ms", cultureInfo, flowDirection, typeface, emSize, color, dpi), new Point(0, 10));
            if (avgDrawMs.Seconds > 0)
                dc.DrawText(new FormattedText($"Draw avg: {avgDrawMs.Seconds}s{avgDrawMs.Milliseconds}ms", cultureInfo, flowDirection, typeface, emSize, color, dpi), new Point(0, 20));
            else
                dc.DrawText(new FormattedText($"Draw avg: {avgDrawMs.Milliseconds}ms", cultureInfo, flowDirection, typeface, emSize, color, dpi), new Point(0, 20));
            dc.DrawText(new FormattedText($"Heap Size: { Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString()}MB", cultureInfo, flowDirection, typeface, emSize, color, dpi), new Point(0, 30));
        }

    }
}
