using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace FractalTrees
{
    class Branch
    {
        private static readonly int maxAngle = 55;
        private static readonly int minSubBranches = 3;
        private static readonly int maxSubBranches = 14;
        private static readonly double minSizeThreshold = 25d;

        private readonly Point start;
        private readonly Point end;
        private readonly double size;
        private readonly double angle;

        private int age;

        private int leafBrushId = -1;

        private List<Branch> branches;

        public Branch(Point start, double size, double angle)
        {
            this.start = start;
            this.size = size;
            this.angle = angle;
            var rad = angle / 180d * Math.PI;
            end = new Point(start.X + size * Math.Cos(rad), start.Y + size * Math.Sin(rad));

            age = 1;
        }

        public void Draw(DrawingContext dc, Brush oldBranchBrush, Brush newBranchBrush, List<Brush> leafBrushes)
        {
            if (age > 1)
            {
                dc.DrawLine(new Pen(oldBranchBrush, age), start, end);
            }
            else
            {
                dc.DrawLine(new Pen(newBranchBrush, age), start, end);
                if (age == 1)
                {
                    if (leafBrushId == -1)
                    {
                        leafBrushId = MainWindow.random.Next(leafBrushes.Count);
                    }

                    dc.DrawEllipse(leafBrushes[leafBrushId], null, end, 5, 3);
                }
            }

            if (branches != null)
            {
                foreach (Branch b in branches)
                {
                    b.Draw(dc, oldBranchBrush, newBranchBrush, leafBrushes);
                }
            }
        }

        public bool Grow()
        {
            if (branches == null)
            {
                // Only add sub branches if the current branch is big enough
                if (size >= minSizeThreshold)
                {
                    int subBranches = MainWindow.random.Next(minSubBranches, maxSubBranches + 1);
                    branches = new List<Branch>();
                    int subspace = maxAngle * 2 / subBranches;
                    for (int i = 0; i < subBranches; i++)
                    {
                        double newSize = size * (MainWindow.random.Next(5, 85) / 100d);
                        branches.Add(new Branch(end, newSize, angle + MainWindow.random.Next(-maxAngle + i * subspace, maxAngle - (subBranches - 1 - i) * subspace)));
                    }
                    leafBrushId = -1; // Not needed, but this way all branches that don't display a leaf will have id -1
                    age++;
                    return true;
                }
            }
            else
            {
                bool grown = false;
                foreach (Branch b in branches)
                {
                    bool subbranchGrown = b.Grow();
                    grown = grown || subbranchGrown;
                }

                if (grown)
                {
                    age++;
                    return true;
                }
            }
            return false;
        }
    }
}