using DrawingBase;
using DrawingBase.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace MengerSponge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static readonly Random random = new Random();
        private List<Cell> sponge = new List<Cell>();
        private readonly Brush cellBrush = Brushes.Gray;

        private readonly KeyboardHelper keyboardHelper = new KeyboardHelper();

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetResolution(600, 600);
            sponge.Add(new Cell(0, 0, 600));
            cellBrush.Freeze();
        }

        public override void Update(float dt)
        {
            keyboardHelper.Update();

            if (keyboardHelper.GetPressedState(Key.Space) == ButtonState.Pressed)
            {
                var newSponge = new List<Cell>();
                double newSize = sponge[0].rect.Width / 3d;
                foreach (Cell c in sponge)
                {
                    // Subdivide
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            // Leave the middle cell open
                            if (x != 1 || y != 1)
                            {
                                newSponge.Add(new Cell(c.rect.X + x * newSize, c.rect.Y + y * newSize, newSize));
                            }
                        }
                    }
                }
                sponge.Clear();
                sponge = newSponge;
            }
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (Cell c in sponge)
            {
                c.Draw(dc, cellBrush, null);
            }
        }

        public override void Cleanup()
        {
        }
    }
}