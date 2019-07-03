using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace FractalTrees
{
    class Branch
    {
        private static readonly Random rnd = new Random();
        private static readonly int maxAngle = 60;
        private static readonly int subBranches = 3;
        private static readonly double minSizeThreshold = 10d;

        private readonly Point start;
        private readonly Point end;
        private readonly double size;
        private readonly double angle;

        private int age;

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

        public void Draw(DrawingContext dc, Brush wood, Brush leaf)
        {
            if (age > 2)
            {
                dc.DrawLine(new Pen(wood, age), start, end);
            }
            else
            {
                dc.DrawLine(new Pen(leaf, age), start, end);
            }
                
            if (branches != null)
            {
                foreach (Branch b in branches)
                {
                    b.Draw(dc, wood, leaf);
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
                    branches = new List<Branch>();
                    int subspace = maxAngle * 2 / subBranches;
                    for (int i = 0; i < subBranches; i++)
                    {
                        double newSize = size * (70 / 100d);
                        branches.Add(new Branch(end, newSize, angle + rnd.Next(-maxAngle + i * subspace, maxAngle - (subBranches - 1 - i) * subspace)));
                    }
                    age++;
                    return true;
                }
            }
            else
            {
                bool grown = false;
                foreach(Branch b in branches)
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