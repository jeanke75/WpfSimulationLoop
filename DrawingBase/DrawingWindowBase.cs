using System;
using System.Collections.Generic;
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

        private long infoRefreshCounter = 0;
        private readonly List<long> updateTicksHistory;
        private readonly List<long> drawTicksHistory;
        private long totalUpdateTicks = 0;
        private long totalDrawTicks = 0;
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

            updateTicksHistory = new List<long>();
            drawTicksHistory = new List<long>();

            timer = new DispatcherTimer();
            timer.Tick += GameTick;
            SetFps(fps);
            DisplayInfo = true;

            Loaded += SimulationBase_Loaded;
        }

        ~DrawingWindowBase()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= GameTick;
            }
            Cleanup();
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

            // Update
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Update(dt);
            sw.Stop();
            long updateTicks = sw.Elapsed.Ticks;

            // Draw
            sw.Restart();
            CvsDraw.Children.Clear();
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Draw(dc);
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
            long drawTicks = sw.Elapsed.Ticks;

            // Recalculate the tick totals
            RecalculateTickTotalAndHistory(ref totalUpdateTicks, updateTicks, updateTicksHistory, fps);
            RecalculateTickTotalAndHistory(ref totalDrawTicks, drawTicks, drawTicksHistory, fps);

            // Refresh the displayed averages every 500ms
            if (infoRefreshCounter > 500000)
            {
                infoRefreshCounter %= 500000;
                avgUpdateMs = new TimeSpan(totalUpdateTicks / updateTicksHistory.Count);
                avgDrawMs = new TimeSpan(totalDrawTicks / drawTicksHistory.Count);
            }
            else
            {
                infoRefreshCounter += span.Ticks;
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

        public void SetBackgroundColor(Color color)
        {
            CvsDraw.Background = new SolidColorBrush(color);
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

        private void RecalculateTickTotalAndHistory(ref long total, long ticks, List<long> list, int maxItems)
        {
            if (list.Count == maxItems)
            {
                total -= list[0];
                list.RemoveAt(0);
            }
            total += ticks;
            list.Add(ticks);
        }
    }
}