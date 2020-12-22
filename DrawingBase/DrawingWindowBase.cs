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
        private readonly Stopwatch stopwatch;
        private readonly DrawingVisual drawingVisual;
        private RenderTargetBitmap renderTargetBitmap;
        private readonly Image image;
        private int fps = 60;
        private DateTime prev;

        private long infoRefreshCounter = 0;
        private readonly List<long> updateTicksHistory;
        private readonly List<long> drawTicksHistory;
        private long totalUpdateTicks = 0;
        private long totalDrawTicks = 0;
        private TimeSpan avgUpdateMs = new TimeSpan();
        private TimeSpan avgDrawMs = new TimeSpan();

        private CultureInfo cultureInfo;
        private Typeface typeface;
        private double emSize;
        private Brush fontBrush;
        private double dpi;
        private Point loopIntervalOrigin;
        private Point updateAverageOrigin;
        private Point drawAverageOrigin;
        private Point heapSizeOrigin;

        public bool ClearFrameBuffer { get; set; }
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

            stopwatch = new Stopwatch();
            drawingVisual = new DrawingVisual();
            image = new Image();
            updateTicksHistory = new List<long>();
            drawTicksHistory = new List<long>();

            timer = new DispatcherTimer();
            timer.Tick += GameTick;
            SetFps(fps);
            ClearFrameBuffer = true;
            DisplayInfo = false;

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
            renderTargetBitmap = new RenderTargetBitmap((int)CvsDraw.ActualWidth, (int)CvsDraw.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            image.Source = renderTargetBitmap;

            // display info settings
            cultureInfo = CultureInfo.CurrentCulture;
            typeface = new Typeface("Georgia");
            emSize = 10;
            fontBrush = Brushes.White;
            dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            loopIntervalOrigin = new Point(0, 0);
            updateAverageOrigin = new Point(0, loopIntervalOrigin.Y + emSize);
            drawAverageOrigin = new Point(0, updateAverageOrigin.Y + emSize);
            heapSizeOrigin = new Point(0, drawAverageOrigin.Y + emSize);
        }

        private void GameTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now - prev;
            float dt = span.Milliseconds / 1000f;
            prev = now;

            // Update
            stopwatch.Start();
            Update(dt);
            stopwatch.Stop();
            long updateTicks = stopwatch.Elapsed.Ticks;

            // Draw
            stopwatch.Restart();
            CvsDraw.Children.Clear();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                Draw(dc);
                if (DisplayInfo)
                    ShowInfo(dc);
            }
            if (ClearFrameBuffer)
                renderTargetBitmap.Clear();
            renderTargetBitmap.Render(drawingVisual);
            CvsDraw.Children.Add(image);
            stopwatch.Stop();
            long drawTicks = stopwatch.Elapsed.Ticks;

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
            dc.DrawText(new FormattedText($"Loop interval: {1000 / fps}ms", cultureInfo, FlowDirection.LeftToRight, typeface, emSize, fontBrush, dpi), loopIntervalOrigin);
            if (avgUpdateMs.Seconds > 0)
                dc.DrawText(new FormattedText($"Update avg: {avgUpdateMs.Seconds}s{avgUpdateMs.Milliseconds}ms", cultureInfo, FlowDirection.LeftToRight, typeface, emSize, fontBrush, dpi), updateAverageOrigin);
            else
                dc.DrawText(new FormattedText($"Update avg: {avgUpdateMs.Milliseconds}ms", cultureInfo, FlowDirection.LeftToRight, typeface, emSize, fontBrush, dpi), updateAverageOrigin);
            if (avgDrawMs.Seconds > 0)
                dc.DrawText(new FormattedText($"Draw avg: {avgDrawMs.Seconds}s{avgDrawMs.Milliseconds}ms", cultureInfo, FlowDirection.LeftToRight, typeface, emSize, fontBrush, dpi), drawAverageOrigin);
            else
                dc.DrawText(new FormattedText($"Draw avg: {avgDrawMs.Milliseconds}ms", cultureInfo, FlowDirection.LeftToRight, typeface, emSize, fontBrush, dpi), drawAverageOrigin);
            dc.DrawText(new FormattedText($"Heap Size: { Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}MB", cultureInfo, FlowDirection.LeftToRight, typeface, emSize, fontBrush, dpi), heapSizeOrigin);
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