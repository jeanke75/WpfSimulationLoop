using DrawingBase;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Raycasting2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        Random random;
        Boundary[] boundaries;
        Particle particle;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetResolution(400, 400);
            random = new Random();

            var tempBoundaries = new List<Boundary>
            {
                // Add outer walls
                new Boundary(0, 0, GetWidth(), 0),
                new Boundary(GetWidth(), 0, GetWidth(), GetHeight()),
                new Boundary(GetWidth(), GetHeight(), 0, GetHeight()),
                new Boundary(0, GetHeight(), 0, 0)
            };
            // Add random inner walls
            for (int i = 0; i < 5; i++)
            {
                tempBoundaries.Add(new Boundary(random.Next(0, GetWidth()), random.Next(0, GetHeight()), random.Next(0, GetWidth()), random.Next(0, GetHeight())));
            }
            boundaries = tempBoundaries.ToArray();

            // Add particle
            particle = new Particle(200, 200);
        }

        public override void Update(float dt)
        {
            particle.pos = Mouse.GetPosition(this);
            particle.Update();
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (Boundary b in boundaries)
            {
                b.Draw(dc);
            }
            particle.Draw(dc);
            particle.Look(boundaries, dc);
        }

        public override void Cleanup()
        {
        }
    }
}