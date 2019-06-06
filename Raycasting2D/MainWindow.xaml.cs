using DrawingBase;
using DrawingBase.Input;
using System;
using System.Collections.Generic;
using System.Windows;
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
        EquallySpacedRayEmitter equallySpacedRayEmitter;
        BoundaryEdgeRayEmitter boundaryEdgeRayEmitter;

        private readonly int sceneWidth = 400;
        private readonly int sceneHeight = 400;
        Point prevPos;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetResolution(sceneWidth * 2, sceneHeight);
            random = new Random();

            var tempBoundaries = new List<Boundary>
            {
                // Add outer walls
                new Boundary(0, 0, sceneWidth, 0),
                new Boundary(sceneWidth, 0, sceneWidth, sceneHeight),
                new Boundary(sceneWidth, sceneHeight, 0, sceneHeight),
                new Boundary(0, sceneHeight, 0, 0)
            };
            // Add random inner walls
            for (int i = 0; i < 5; i++)
            {
                tempBoundaries.Add(new Boundary(random.Next(0, sceneWidth), random.Next(0, sceneHeight), random.Next(0, sceneWidth), random.Next(0, sceneHeight)));
            }
            boundaries = tempBoundaries.ToArray();

            // Set the previous position
            prevPos = new Point(sceneWidth / 2d, sceneHeight / 2d);

            // Add ray emitter
            equallySpacedRayEmitter = new EquallySpacedRayEmitter(prevPos.X, prevPos.Y); // Might not detect the edges correctly
            boundaryEdgeRayEmitter = new BoundaryEdgeRayEmitter(sceneWidth / 2d, sceneHeight / 2d); // Optimal if the boundaries don't intersect
        }

        public override void Update(float dt)
        {
            InputHelper.Update();
            Point pos = InputHelper.Mouse.GetPosition();
            // Checks to keep the emitter in the scene
            if (pos.X < 0)
            {
                pos.X = prevPos.X;
            }
            else if  (pos.X > sceneHeight)
            {
                pos.X = sceneHeight;
            }
            if (pos.Y < 0)
            {
                pos.Y = prevPos.Y;
            }
            else if (pos.Y > sceneWidth)
            {
                pos.Y = sceneWidth;
            }

            // Update each scene
            equallySpacedRayEmitter.pos = pos;
            equallySpacedRayEmitter.Update(boundaries);

            boundaryEdgeRayEmitter.pos = pos;
            boundaryEdgeRayEmitter.Update(boundaries);

            prevPos = pos;
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (Boundary b in boundaries)
            {
                b.Draw(dc);
            }
            equallySpacedRayEmitter.Draw(dc);

            dc.PushTransform(new TranslateTransform(sceneWidth, 0));
            foreach (Boundary b in boundaries)
            {
                b.Draw(dc);
            }
            boundaryEdgeRayEmitter.Draw(dc);
            dc.Pop();
        }

        public override void Cleanup()
        {
        }
    }
}