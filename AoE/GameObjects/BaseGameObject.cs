using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            imageSource = GetImageSource(imageId);
        }

        public virtual void Draw(DrawingContext dc, List<Team> teams)
        {
            // Draw gameobject
            dc.DrawImage(imageSource, Rect);

            // Draw radius
            if (MainWindow.ShowGameObjectRadius)
                dc.DrawEllipse(null, new Pen(Brushes.White, 1), new Point(Position.X, Position.Y), Radius, Radius);
        }

        private ImageSource GetImageSource(string imageId)
        {
            return BitmapDecoder.Create(new Uri("pack://application:,,,/Images/" + imageId), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
        }

        public bool MouseOver(Point mousePos)
        {
            var rect = Rect;
            return mousePos.X >= rect.Left && mousePos.X <= rect.Right && mousePos.Y >= rect.Top && mousePos.Y <= rect.Bottom;
        }
    }
}