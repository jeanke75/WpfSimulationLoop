using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AoE.GameObjects
{
    abstract class BaseGameObject
    {
        public Vector Position { get; protected set; }
        public readonly float Width;
        public readonly float Height;
        public readonly float Radius;
        public Rect Rect
        {
            get
            {
                return new Rect(Position.X - Width / 2f, Position.Y - Height / 2f, Width, Height);
            }
        }
        public readonly string Name;

        private readonly ImageSource imageSource;

        public BaseGameObject(Vector position, float width, float height, string name, string imageId)
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