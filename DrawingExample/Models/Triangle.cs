using DrawingBase;
using System;
using System.Windows;
using System.Windows.Media;

namespace DrawingExample.Models
{
    public class Triangle : Entity
    {
        protected new static int Size = 20;
        private static readonly double ringspeed = 60d;
        private double angle = 0;

        static readonly double height = Math.Sqrt(Math.Pow(Size, 2) - Math.Pow(Size / 2f, 2));

        public Triangle(Vector position) : base(position)
        {
            fill = Brushes.Red;
            outline = new Pen(Brushes.White, 1);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            angle += ringspeed * dt;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushTransform(new TranslateTransform(position.X, position.Y));

            // draw triangle
            dc.PushOpacity(opacity / 255f);
            dc.DrawEquilateralTriangle(fill, null, Size, true);
            dc.Pop();

            // draw orbiting points
            var sections = Size / 4f;
            var sectionAngle = 120 / sections;
            var pointSize = 1d;
            for (int i = 0; i < sections; i++)
            {
                var rot = angle + i * sectionAngle;
                dc.PushTransform(new RotateTransform(rot));
                dc.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(0 - pointSize / 2, height * -2 / 3f - pointSize / 2, pointSize, pointSize));
                dc.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(-Size / 2f - pointSize / 2, height * 1 / 3f - pointSize / 2, pointSize, pointSize));
                dc.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(Size / 2f - pointSize / 2, height * 1 / 3f - pointSize / 2, pointSize, pointSize));
                dc.Pop();
            }

            // draw rotating outline
            dc.PushTransform(new RotateTransform(-angle));
            dc.PushOpacity((255 - opacity) / 255f);
            dc.DrawEquilateralTriangle(null, outline, Size, true);
            dc.Pop();
            dc.Pop();

            dc.Pop();
        }
    }
}