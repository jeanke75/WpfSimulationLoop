using DrawingBase;
using DrawingBase.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tetris.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        private Random random;
        private readonly int tileSize = 20;
        private readonly int fieldRows = 20;
        private readonly int fieldCols = 10;

        private readonly Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
        private readonly List<BaseShape> shapes = new List<BaseShape>();
        private int[,] field;

        private BaseShape currentShape;
        private Point pos;

        private int framesUntillDrop = 0;
        private readonly int framesBetweenAutoDrop = 10;

        private bool gameOver;
        private FormattedText gameOverText;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetResolution((fieldCols + 2) * tileSize, (fieldRows + 2) * tileSize);
            random = new Random();

            // Add tiles
            tiles.Add(1, new Tile(Colors.Gray));
            tiles.Add(2, new Tile(Colors.Cyan));
            tiles.Add(3, new Tile(Colors.Blue));
            tiles.Add(4, new Tile(Colors.Orange));
            tiles.Add(5, new Tile(Colors.Yellow));
            tiles.Add(6, new Tile(Colors.Green));
            tiles.Add(7, new Tile(Colors.Purple));
            tiles.Add(8, new Tile(Colors.Red));

            // Add shapes
            shapes.Add(new I(2));
            shapes.Add(new J(3));
            shapes.Add(new L(4));
            shapes.Add(new O(5));
            shapes.Add(new S(6));
            shapes.Add(new T(7));
            shapes.Add(new Z(8));

            // Create the field
            field = new int[fieldCols, fieldRows];

            // Init the position
            pos = new Point(0, 0);
            framesUntillDrop = framesBetweenAutoDrop;
            gameOver = false;
            gameOverText = new FormattedText($"GAME OVER", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Georgia"), GetWidth() / 10d, Brushes.White, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            if (!gameOver)
            {
                if (currentShape == null)
                {
                    // If there is no shape, get a new one
                    currentShape = shapes[random.Next(shapes.Count)];
                    pos.X = (fieldCols - currentShape.Width()) / 2;
                    pos.Y = 0;

                    if (HasCollision())
                        gameOver = true;
                }
                else
                {
                    // Move the shape left/right
                    if (InputHelper.Keyboard.GetPressedState(Key.Left) == ButtonState.Pressed )
                    {
                        pos.X--;
                        if (HasCollision()) pos.X++;
                    }
                    else if (InputHelper.Keyboard.GetPressedState(Key.Right) == ButtonState.Pressed)
                    {
                        pos.X++;
                        if (HasCollision()) pos.X--;
                    }
                    // Rotate the shape
                    if (InputHelper.Keyboard.GetPressedState(Key.Up) == ButtonState.Pressed)
                    {
                        currentShape.Rotate();
                        if (HasCollision()) currentShape.UndoRotate();
                    }
                    
                    framesUntillDrop--;
                    if (framesUntillDrop == 0) // Drop the shape
                    {
                        pos.Y++;
                        framesUntillDrop = framesBetweenAutoDrop;
                    }
                    else if (InputHelper.Keyboard.GetPressedState(Key.Down) == ButtonState.Pressed || InputHelper.Keyboard.GetPressedState(Key.Down) == ButtonState.Down) // Drop the shape manually between auto drops
                    {
                        pos.Y++;
                    }

                    if (HasCollision())
                    {
                        pos.Y--;

                        AddShapeToField();

                        FullRowsCheck();

                        // Reset the shape rotation
                        currentShape.Reset();

                        // Remove the shape as the current shape
                        currentShape = null;
                    }
                }
            }
        }

        public override void Draw(DrawingContext dc)
        {
            // Draw fieldborder
            tiles.TryGetValue(1, out Tile borderTile);
            for (int i = 0; i < fieldCols + 2; i++)
            {
                // Top border
                borderTile.Draw(dc, i * tileSize, 0, tileSize);
                // Bottom border
                borderTile.Draw(dc, i * tileSize, (fieldRows + 1) * tileSize, tileSize);
            }

            for (int i = 0; i < fieldRows; i++)
            {
                // Left border
                borderTile.Draw(dc, 0, tileSize + i * tileSize, tileSize);
                // Right border
                borderTile.Draw(dc, (fieldCols + 1) * tileSize, tileSize + i * tileSize, tileSize);
            }
            
            dc.PushTransform(new TranslateTransform(tileSize, tileSize));
            // Draw field
            for (int row = 0; row < field.GetLength(1); row++)
            {
                for (int col = 0; col < field.GetLength(0); col++)
                {
                    var tileId = field[col, row];
                    if (tileId > 0)
                    {
                        tiles.TryGetValue(tileId, out Tile tile);
                        tile.Draw(dc, col * tileSize, row * tileSize, tileSize);
                    }
                }
            }

            // Draw current shape
            if (currentShape != null)
            {
                var state = currentShape.CurrentState();
                for (int row = 0; row < currentShape.Height(); row++)
                {
                    for (int col = 0; col < currentShape.Width(); col++)
                    {
                        if (state[col, row])
                        {
                            tiles.TryGetValue(currentShape.tileId, out Tile tile);
                            tile.Draw(dc, (pos.X + col) * tileSize, (pos.Y + row) * tileSize, tileSize);
                        }
                    }
                }
            }
            dc.Pop();

            // Draw gameover
            if (gameOver)
            {
                dc.DrawText(gameOverText, new Point((GetWidth() - gameOverText.Width) / 2d, (GetHeight() - gameOverText.Height) / 2d));
            }
        }

        public override void Cleanup()
        {
        }

        private bool HasCollision()
        {
            var state = currentShape.CurrentState();
            for (int row = 0; row < currentShape.Height(); row++)
            {
                for (int col = 0; col < currentShape.Width(); col++)
                {
                    if (state[col, row])
                    {
                        // Collision with the left side of the playing field
                        if ((int)pos.X + col < 0) return true;
                        // Collision with the right side of the playing field
                        if ((int)pos.X + col >= fieldCols) return true;
                        // Collision with the bottom of the playing field
                        if ((int)pos.Y + row >= fieldRows) return true;
                        // Collision with a previous shape
                        if (field[(int)pos.X + col, (int)pos.Y + row] > 0) return true;
                    }
                }
            }
            return false;
        }

        private void AddShapeToField()
        {
            var state = currentShape.CurrentState();
            for (int row = 0; row < currentShape.Height(); row++)
            {
                for (int col = 0; col < currentShape.Width(); col++)
                {
                    if (state[col, row])
                    {
                        field[(int)pos.X + col, (int)pos.Y + row] = currentShape.tileId;
                    }
                }
            }
        }

        private void FullRowsCheck()
        {
            for (int row = (int)pos.Y; row < field.GetLength(1); row++)
            {
                bool fullRow = true;
                for (int col = 0; col < field.GetLength(0); col++)
                {
                    if (field[col, row] == 0)
                    {
                        fullRow = false;
                        break;
                    }
                }
                if (fullRow)
                {
                    // Clear 
                    for (int r = row; r > 0; r--)
                    {
                        for (int c = 0; c < field.GetLength(0); c++)
                        {
                            field[c, r] = field[c, r - 1];
                            field[c, r - 1] = 0;
                        }
                    }
                }
            }
        }
    }
}