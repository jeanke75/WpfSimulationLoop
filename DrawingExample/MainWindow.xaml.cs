using DrawingBase;
using DrawingExample.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DrawingExample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static readonly Random random = new Random();
        readonly List<Entity> entities;

        public MainWindow()
        {
            entities = new List<Entity>();
        }

        public override void Initialize()
        {
            SetResolution(264, 264);
            for (int i = 0; i < 15; i++)
            {
                var a = new Square(new Vector(random.Next(0, GetWidth() - 2), random.Next(0, GetHeight() - 2)));
                entities.Add(a);
            }

            for (int i = 0; i < 10; i++)
            {
                var a = new Circle(new Vector(random.Next(0, GetWidth() - 2), random.Next(0, GetHeight() - 2)));
                entities.Add(a);
            }

            for (int i = 0; i < 5; i++)
            {
                var a = new Triangle(new Vector(random.Next(0, GetWidth() - 2), random.Next(0, GetHeight() - 2)));
                entities.Add(a);
            }
        }

        public override void Cleanup()
        {

        }

        public override void Update(float dt)
        {
            foreach (Entity entity in entities)
            {
                if (entity.destination == entity.position && random.Next(0, 100) < 15)
                {
                    entity.destination = new Vector(random.Next(0, (int)ActualWidth), random.Next(0, (int)ActualHeight));
                }
                entity.Update(dt);
            }
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (Entity entity in entities)
            {
                entity.Draw(dc);
            }
        }
    }
}