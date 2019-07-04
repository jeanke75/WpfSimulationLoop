using DrawingBase;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace FractalTrees
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static readonly Random random = new Random();
        private Branch tree;

        private readonly Brush oldBrancheBrush = Brushes.SaddleBrown;
        private readonly Brush newBrancheBrush = Brushes.SandyBrown;
        private readonly List<Brush> leafBrushes = new List<Brush>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetFps(2);

            tree = new Branch(new Point(0, 0), 150, -90);

            oldBrancheBrush.Freeze();
            newBrancheBrush.Freeze();
            leafBrushes.Add(Brushes.Green);
            leafBrushes.Add(Brushes.DarkOliveGreen);
            leafBrushes.Add(Brushes.DarkGreen);
            foreach (Brush b in leafBrushes)
            {
                b.Freeze();
            }
        }

        public override void Update(float dt)
        {
            tree.Grow();
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushTransform(new TranslateTransform(GetWidth() / 2d, GetHeight()));
            tree.Draw(dc, oldBrancheBrush, newBrancheBrush, leafBrushes);
            dc.Pop();
        }

        public override void Cleanup()
        {
        }
    }
}