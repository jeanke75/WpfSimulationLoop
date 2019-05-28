using DrawingBase;
using System.Windows.Media;

namespace MazeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private RecursiveBacktrackerMazeGenerator mazeGen;
        private readonly int rows = 25;
        private readonly int cols = 25;
        private readonly int cellSize = 20;
        private readonly int wallThickness = 2;
        private Color cellColor = Colors.White;
        private Color cellHighlightColor = Colors.Red;
        private Color wallColor = Colors.Gray;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            mazeGen = new RecursiveBacktrackerMazeGenerator(rows, cols, true);
        }

        public override void Update(float dt)
        {
            mazeGen.Update();
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushTransform(new TranslateTransform((GetWidth() - (cellSize * cols)) / 2d, (GetHeight() - (cellSize * rows)) / 2d));
            mazeGen.Draw(dc, cellColor, cellSize, cellHighlightColor, wallThickness, wallColor);
            dc.Pop();
        }

        public override void Cleanup()
        {
            mazeGen = null;
        }
    }
}