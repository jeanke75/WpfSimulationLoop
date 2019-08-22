using System;
using System.Windows;
using System.Windows.Media;

namespace AoE.GameObjects
{
    enum ArmorType
    {
        None,
        Archer,
        Building,
        Camel,
        Castle,
        CavalryArcher,
        Cavalry,
        Condotierro,
        EagleWarrior,
        GunpowderUnit,
        Infantry,
        Monk,
        Ram,
        Ship,
        SiegeWeapon,
        StandardBuilding,
        StoneDefense,
        Spearman,
        UniqueUnit,
        WallAndGate,
        WarElephant
    }

    abstract class BaseGameObject
    {
        public Vector Position { get; set; }
        public readonly double Width;
        public readonly double Height;
        public readonly double Radius;
        public Rect Rect
        {
            get
            {
                return new Rect(Position.X - Width / 2f, Position.Y - Height / 2f, Width, Height);
            }
        }
        public readonly string Name;

        private readonly ImageSource imageSource;

        public BaseGameObject(Vector position, double width, double height, string name, string imageId)
        {
            Position = position;
            Width = width;
            Height = height;
            Radius = width < height ? width / 2f : height / 2f;
            Name = name;

            imageSource = Global.GetImageSource(imageId);
        }

        public virtual void Draw(DrawingContext dc)
        {
            // Draw gameobject
            dc.DrawImage(imageSource, Rect);

            // Draw radius
            if (MainWindow.ShowGameObjectRadius)
                dc.DrawEllipse(null, new Pen(Brushes.White, 1), new Point(Position.X, Position.Y), Radius, Radius);
        }

        /*
         * Value < 0: Units are overlapping
         */
        public double Distance(BaseGameObject other)
        {
            return Math.Sqrt(Math.Pow(other.Position.X - Position.X, 2) + Math.Pow(other.Position.Y - Position.Y, 2)) - (other.Radius + Radius);
        }

        public bool MouseOver(Point mousePos)
        {
            var rect = Rect;
            return mousePos.X >= rect.Left && mousePos.X <= rect.Right && mousePos.Y >= rect.Top && mousePos.Y <= rect.Bottom;
        }
    }
}