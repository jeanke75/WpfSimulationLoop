using DrawingBase;
using DrawingBase.Input;
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

        private readonly KeyboardHelper keyboardHelper = new KeyboardHelper();

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetFps(2);

            CreateNewTree();

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
            keyboardHelper.Update();

            if (keyboardHelper.GetPressedState(System.Windows.Input.Key.F5) == ButtonState.Pressed)
            {
                CreateNewTree();
            }
            else
            {
                tree.Grow();
            }
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

        private void CreateNewTree()
        {
            tree = new Branch(new Point(0, 0), 150, -90);
        }
    }
}