using DrawingBase;
using System.Windows;
using System.Windows.Media;

namespace FractalTrees
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private readonly Brush woodBrush = Brushes.Brown;
        private readonly Brush leafBrush = Brushes.ForestGreen;
        private Branch root;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetFps(2);
            woodBrush.Freeze();
            leafBrush.Freeze();
            root = new Branch(new Point(0, 0), 150, -90);
        }

        public override void Update(float dt)
        {
            root.Grow();
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushTransform(new TranslateTransform(GetWidth() / 2d, GetHeight()));
            root.Draw(dc, woodBrush, leafBrush);
            dc.Pop();
        }

        public override void Cleanup()
        {
        }
    }
}