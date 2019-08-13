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
        private Random random;
        private Boundary[] boundaries;
        private EquallySpacedRayEmitter equallySpacedRayEmitter;
        private BoundaryEdgeRayEmitter boundaryEdgeRayEmitter;
        private IntersectingBoundaryEdgeRayEmitter intersectingBoundaryEdgeRayEmitter;

        private readonly int sceneWidth = 400;
        private readonly int sceneHeight = 400;
        Point prevPos;

        public override void Initialize()
        {
            SetResolution(sceneWidth * 3, sceneHeight);
            random = new Random();

            var tempBoundaries = new List<Boundary>
            {
                // Add outer walls
                new Boundary(0, 0, sceneWidth, 0),
                new Boundary(sceneWidth, 0, sceneWidth, sceneHeight),
                new Boundary(sceneWidth, sceneHeight, 0, sceneHeight),
                new Boundary(0, sceneHeight, 0, 0),

                new Boundary(20, 40, 40, 20),
                new Boundary(50, 100, 95, 65),

                new Boundary(sceneWidth - 75, 25, sceneWidth - 25, 25),
                new Boundary(sceneWidth - 25, 25, sceneWidth - 25, 75),
                new Boundary(sceneWidth - 25, 75, sceneWidth - 75, 75),
                new Boundary(sceneWidth - 75, 75, sceneWidth - 75, 25),
            };
            // Add random inner walls
            for (int i = 0; i < 5; i++)
            {
                tempBoundaries.Add(new Boundary(random.Next(0, sceneWidth), random.Next(sceneHeight / 2, sceneHeight), random.Next(0, sceneWidth), random.Next(sceneHeight / 2, sceneHeight)));
            }
            boundaries = tempBoundaries.ToArray();

            // Set the previous position
            prevPos = new Point(sceneWidth / 2d, sceneHeight / 2d);

            // Add ray emitter
            equallySpacedRayEmitter = new EquallySpacedRayEmitter(prevPos.X, prevPos.Y); // Might not detect the edges correctly
            boundaryEdgeRayEmitter = new BoundaryEdgeRayEmitter(sceneWidth / 2d, sceneHeight / 2d); // Optimal if the boundaries don't intersect
            intersectingBoundaryEdgeRayEmitter = new IntersectingBoundaryEdgeRayEmitter(sceneWidth / 2d, sceneHeight / 2d); // Optimal if the boundaries intersect
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
            else if (pos.X > sceneHeight)
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

            intersectingBoundaryEdgeRayEmitter.pos = pos;
            intersectingBoundaryEdgeRayEmitter.Update(boundaries);

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

            dc.PushTransform(new TranslateTransform(sceneWidth * 2, 0));
            foreach (Boundary b in boundaries)
            {
                b.Draw(dc);
            }
            intersectingBoundaryEdgeRayEmitter.Draw(dc);
            dc.Pop();
        }

        public override void Cleanup()
        {
        }
    }
}