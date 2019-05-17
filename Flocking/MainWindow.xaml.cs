using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DrawingBase;

namespace Flocking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static Random random = new Random();
        private List<Boid> boids = new List<Boid>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            boids.Add(new Boid(new Vector(GetWidth() / 2d, GetHeight() / 2d), new Vector(1, 0)));
            /*for (int i = 0; i < 10; i++)
            {
                var position = new Vector(random.Next(0, GetWidth()), random.Next(0, GetHeight()));
                var angle = random.NextDouble() * Math.PI * 2;
                var direction = new Vector(Math.Cos(angle), Math.Sin(angle));
                boids.Add(new Boid(position, direction));
            }*/
        }

        public override void Update(float dt)
        {
            //List<Boid> updatedBoids = new List<Boid>();
            foreach (Boid boid in boids)
            {
                boid.Update(dt);
                if (boid.position.X < 0)
                    boid.position.X = GetWidth();
                else if (boid.position.X > GetWidth())
                    boid.position.X = 0;
                if (boid.position.Y < 0)
                    boid.position.Y = GetHeight();
                else if (boid.position.Y > GetHeight())
                    boid.position.Y = 0;
            }
            //boids = updatedBoids;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.PushOpacity(0.75);
            foreach (Boid boid in boids)
            {
                boid.Draw(dc);
            }
            dc.Pop();
        }

        public override void Cleanup()
        {
            boids.Clear();
        }

        private void Steering()
        {

        }

        private void Alignment()
        {

        }

        private void Cohesion()
        {

        }
    }
}