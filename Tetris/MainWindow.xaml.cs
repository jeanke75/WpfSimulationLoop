﻿using DrawingBase;
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

        private BaseShape nextShape;
        private BaseShape currentShape;
        private Point pos;

        private int framesUntillDrop;
        private readonly int framesBetweenAutoDrop = 10;

        private int score;
        private int lines;

        private bool gameOver;
        private FormattedText gameOverText;

        private readonly Typeface typeface = new Typeface("Georgia");
        private readonly Brush gameOverBrush = Brushes.White;
        private readonly Brush infoBackgroundBrush = Brushes.Gray;
        private readonly Brush infoTextBrush = Brushes.Black;
        private readonly Brush infoBoxBrush = Brushes.DarkGray;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            SetResolution((int)((fieldCols + 2) * tileSize * 1.75), (fieldRows + 2) * tileSize);
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

            // Brushes
            gameOverBrush.Freeze();
            infoBackgroundBrush.Freeze();
            infoTextBrush.Freeze();
            infoBoxBrush.Freeze();

            // Init the game parameters
            nextShape = GetRandomshape();
            pos = new Point(0, 0);
            framesUntillDrop = framesBetweenAutoDrop;
            score = 0;
            lines = 0;
            gameOver = false;
            gameOverText = new FormattedText($"GAME OVER", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 10d, gameOverBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }

        public override void Update(float dt)
        {
            InputHelper.Update();

            if (!gameOver)
            {
                if (currentShape == null)
                {
                    // If there is no shape, get a new one
                    currentShape = nextShape;
                    nextShape = GetRandomshape();
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

                        var linesCleared = ClearLines();
                        lines += linesCleared;
                        UpdateScore(linesCleared);

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

            var playFieldWidth = (fieldCols + 2) * tileSize;
            var infoWidth = GetWidth() - playFieldWidth;
            dc.PushTransform(new TranslateTransform(playFieldWidth, 0));
            // Draw info background
            dc.DrawRectangle(infoBackgroundBrush, null, new Rect(0, 0, infoWidth, GetHeight()));
            
            var maxScoreWidth = new FormattedText(int.MaxValue.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 20d, infoTextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip).Width;
            var scoreAndLinesBoxWidth = maxScoreWidth + (infoWidth - maxScoreWidth) / 10;
            // Draw the scores and lines in a box
            dc.PushTransform(new TranslateTransform((infoWidth - scoreAndLinesBoxWidth) / 2d, tileSize));
            // Background
            dc.DrawRectangle(infoBoxBrush, null, new Rect(0, 0, scoreAndLinesBoxWidth, tileSize * 5));

            // Score
            var scoreText = new FormattedText("SCORE", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 20d, infoTextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(scoreText, new Point((scoreAndLinesBoxWidth - scoreText.Width) / 2d, 0));

            var scoreValue = new FormattedText(score.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 20d, infoTextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(scoreValue, new Point((scoreAndLinesBoxWidth - scoreValue.Width) / 2d, tileSize));

            // Lines
            var linesText = new FormattedText("LINES", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 20d, infoTextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(linesText, new Point((scoreAndLinesBoxWidth - linesText.Width) / 2d, tileSize * 2.75));

            var linesValue = new FormattedText(lines.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 20d, infoTextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(linesValue, new Point((scoreAndLinesBoxWidth - linesValue.Width) / 2d, tileSize * 3.75));
            dc.Pop();

            // Draw next shape
            var nextShapeBoxSize = infoWidth / 3d;
            dc.PushTransform(new TranslateTransform((infoWidth - nextShapeBoxSize) / 2d, GetHeight() - nextShapeBoxSize - tileSize));
            // Background
            dc.DrawRectangle(infoBoxBrush, null, new Rect(0, 0, nextShapeBoxSize, nextShapeBoxSize));

            // Shape
            var nextShapeState = nextShape.DefaultState();
            tiles.TryGetValue(nextShape.tileId, out Tile nextShapeTile);
            var nextShapeTileSize = nextShapeBoxSize / 6d; // widest block is 4tiles + 2x 1tile offset
            var nextShapeOffset = nextShapeTileSize * (1 + (4 - nextShape.Width()) / 2d);
            dc.PushTransform(new TranslateTransform(nextShapeOffset, nextShapeOffset));
            for (int row = 0; row < nextShape.Height(); row++)
            {
                for (int col = 0; col < nextShape.Width(); col++)
                {
                    if (nextShapeState[col, row])
                    {
                        nextShapeTile.Draw(dc, col * nextShapeTileSize, row * nextShapeTileSize, nextShapeTileSize);
                    }
                }
            }
            dc.Pop();
            dc.Pop();
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

        private int ClearLines()
        {
            int linesCleared = 0;
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
                    linesCleared++;
                }
            }

            return linesCleared;
        }

        private void UpdateScore(int rowsCleared)
        {
            switch (rowsCleared)
            {
                case 1:
                    score += 40;
                    break;
                case 2:
                    score += 100;
                    break;
                case 3:
                    score += 300;
                    break;
                case 4:
                    score += 1200;
                    break;
                default:
                    break;
            }
        }

        private BaseShape GetRandomshape()
        {
            return shapes[random.Next(shapes.Count)];
        }
    }
}