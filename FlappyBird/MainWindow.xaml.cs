using DrawingBase;
using DrawingBase.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace FlappyBird
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private Bird bird;
        private readonly double wallWidth = 35d;
        private readonly double wallGapHeight = 85d;
        private List<Wall> walls;
        private Random random;

        private readonly double floorHeight = 15d;
        private readonly double floorTopLayerHeight = 5d;
        private readonly double roofHeight = 10d;
        private readonly Brush floorTopLayerBrush = Brushes.LawnGreen;
        private readonly Brush floorBrush = Brushes.Chocolate;
        private readonly Brush roofBrush = Brushes.DimGray;

        private bool gameOver;
        private FormattedText gameOverText;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetBackgroundColor(Colors.LightBlue);

            bird = new Bird(GetWidth() * 0.15d, GetHeight() / 2d, 20d);
            walls = new List<Wall>();
            random = new Random();

            roofBrush.Freeze();
            floorTopLayerBrush.Freeze();
            floorBrush.Freeze();

            gameOver = false;
            gameOverText = new FormattedText($"GAME OVER", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Georgia"), GetHeight() / 6d, Brushes.White, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            // Add gravity if the bird is not on the floor
            if (bird.Area.Bottom < GetHeight())
            {
                bird.AddForce(9.81 * dt);
            }

            if (!gameOver)
            {
                // Make the bird fly up when the left mousebutton is pressed
                if (InputHelper.Mouse.GetState(MouseButton.Left) == ButtonState.Pressed)
                {
                    bird.AddForce(-10d);
                }

                // Update bird and fix boundaries
                bird.Update(dt);
                bird.EnforceBoundaries(roofHeight, GetHeight() - floorHeight);

                for (int i = walls.Count - 1; i >= 0; i--)
                {
                    Wall wall = walls[i];
                    wall.Update(dt);

                    // Remove offscreen walls from the collection
                    if (wall.Area.Right < 0)
                    {
                        walls.Remove(wall);
                        continue;
                    }

                    // Check if there is a collision with the wall
                    if (!gameOver && wall.Collision(bird))
                    {
                        gameOver = true;
                    }
                }

                if (walls.Count == 0 || GetWidth() - (walls[walls.Count - 1].Area.Right) > random.Next(225, 450))
                {
                    AddWall();
                }
            }
            else
            {
                bird.Update(dt);
            }
        }

        public override void Draw(DrawingContext dc)
        {
            // Draw walls
            foreach (Wall wall in walls)
            {
                wall.Draw(dc);
            }

            // Draw floor
            if (floorHeight > 0)
            {
                if (floorHeight > floorTopLayerHeight)
                {
                    dc.DrawRectangle(floorBrush, null, new Rect(0, GetHeight() - (floorHeight - floorTopLayerHeight), GetWidth(), floorHeight - floorTopLayerHeight));
                }
                dc.DrawRectangle(floorTopLayerBrush, null, new Rect(0, GetHeight() - floorHeight, GetWidth(), floorTopLayerHeight));
            }

            // Draw roof
            if (roofHeight > 0)
            {
                dc.DrawRectangle(roofBrush, null, new Rect(0, 0, GetWidth(), roofHeight));
            }

            // Draw gameover
            if (gameOver)
            {
                dc.DrawText(gameOverText, new Point((GetWidth() - gameOverText.Width) / 2d, (GetHeight() - gameOverText.Height) / 2d));
            }

            // Draw bird
            bird.Draw(dc);
        }

        public override void Cleanup()
        {
        }

        private void AddWall()
        {
            double fieldHeight = GetHeight() - floorHeight - roofHeight;
            double heightTopWall = 0.075 * fieldHeight + (0.85d * fieldHeight - wallGapHeight) * random.NextDouble();

            // Top wall
            walls.Add(new Wall(GetWidth(), roofHeight, wallWidth, heightTopWall));

            // Bottom wall
            walls.Add(new Wall(GetWidth(), roofHeight + heightTopWall + wallGapHeight, wallWidth, fieldHeight - heightTopWall - wallGapHeight));
        }
    }
}