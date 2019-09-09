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

        private readonly Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
        private readonly List<BaseShape> shapes = new List<BaseShape>();
        private Field field;

        private BaseShape nextShape;
        private BaseShape currentShape;
        private Point pos;

        private int framesUntillDrop;
        private readonly int framesBetweenAutoDrop = 10;

        private int score;
        private int lines;

        private bool gameOver;
        private FormattedText gameOverText;

        private readonly Dictionary<BaseShape, int> shapeStats = new Dictionary<BaseShape, int>();

        private readonly Typeface typeface = new Typeface("Georgia");
        private readonly Brush gameOverBrush = Brushes.White;
        private readonly Brush infoBackgroundBrush = Brushes.Gray;
        private readonly Brush infoTextBrush = Brushes.Black;
        private readonly Brush infoBoxBrush = Brushes.DarkGray;

        public override void Initialize()
        {
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

            // Add statistics
            foreach (BaseShape shape in shapes)
            {
                shapeStats.Add(shape, 0);
            }

            // Create the field
            field = new Field(20, 10);

            // Set the resolution
            SetResolution((int)((field.Columns + 2) * tileSize * 1.75), (field.Rows + 2) * tileSize);

            // Brushes
            gameOverBrush.Freeze();
            infoBackgroundBrush.Freeze();
            infoTextBrush.Freeze();
            infoBoxBrush.Freeze();

            // Texts
            gameOverText = new FormattedText($"GAME OVER", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, GetWidth() / 10d, gameOverBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            NewGame();
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
                    pos.X = (field.Columns - currentShape.Width()) / 2;
                    pos.Y = 0;

                    // Update the shape stats
                    shapeStats[currentShape] += 1;

                    if (HasCollision())
                        gameOver = true;
                }
                else
                {
                    // Move the shape left/right
                    if (InputHelper.Keyboard.GetPressedState(Key.Left) == ButtonState.Pressed)
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

                        field.AddShape(currentShape, pos);

                        var linesCleared = field.ClearLines(pos);
                        lines += linesCleared;
                        UpdateScore(linesCleared);

                        // Reset the shape rotation
                        currentShape.Reset();

                        // Remove the shape as the current shape
                        currentShape = null;
                    }
                }
            }
            else if (InputHelper.Keyboard.GetPressedState(Key.Space) == ButtonState.Pressed)
            {
                NewGame();
            }
        }

        public override void Draw(DrawingContext dc)
        {
            // Draw fieldborder
            tiles.TryGetValue(1, out Tile borderTile);
            for (int i = 0; i < field.Columns + 2; i++)
            {
                // Top border
                borderTile.Draw(dc, i * tileSize, 0, tileSize);
                // Bottom border
                borderTile.Draw(dc, i * tileSize, (field.Rows + 1) * tileSize, tileSize);
            }

            for (int i = 0; i < field.Rows; i++)
            {
                // Left border
                borderTile.Draw(dc, 0, tileSize + i * tileSize, tileSize);
                // Right border
                borderTile.Draw(dc, (field.Columns + 1) * tileSize, tileSize + i * tileSize, tileSize);
            }

            // Draw field
            field.Draw(dc, tileSize, tiles);

            // Draw current shape
            if (currentShape != null)
            {
                dc.PushTransform(new TranslateTransform(pos.X * tileSize, pos.Y * tileSize));
                currentShape.Draw(dc, tiles, tileSize);
                dc.Pop();
            }
            dc.Pop();

            var playFieldWidth = (field.Columns + 2) * tileSize;
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

            // Statistics
            var statsOffsetX = (scoreAndLinesBoxWidth - linesText.Width) / 2d;
            var statsOffsetY = tileSize * 7.5;
            var shape = 0;
            foreach (KeyValuePair<BaseShape, int> kvp in shapeStats)
            {
                var statsText = new FormattedText(kvp.Value.ToString("000"), CultureInfo.CurrentCulture, FlowDirection.RightToLeft, typeface, GetWidth() / 20d, infoTextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                dc.DrawText(statsText, new Point(statsOffsetX + infoWidth / 3d, statsOffsetY + shape * 20));

                var statsShapeTilesize = infoWidth / 3d;
                dc.PushTransform(new TranslateTransform(statsOffsetX, statsOffsetY + shape * 20));
                kvp.Key.Draw(dc, tiles, 5, false);
                dc.Pop();

                shape++;
            }

            // Draw next shape
            var nextShapeBoxSize = infoWidth / 3d;
            dc.PushTransform(new TranslateTransform((infoWidth - nextShapeBoxSize) / 2d, GetHeight() - nextShapeBoxSize - tileSize));
            // Background
            dc.DrawRectangle(infoBoxBrush, null, new Rect(0, 0, nextShapeBoxSize, nextShapeBoxSize));

            // Shape
            var nextShapeTileSize = nextShapeBoxSize / 6d; // widest block is 4tiles + 2x 1tile offset
            var nextShapeOffset = nextShapeTileSize * (1 + (4 - nextShape.Width()) / 2d);
            dc.PushTransform(new TranslateTransform(nextShapeOffset, nextShapeOffset));
            nextShape.Draw(dc, tiles, nextShapeTileSize, false);
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

        private void NewGame()
        {
            field.Clear();
            currentShape = null;
            nextShape = GetRandomshape();
            pos = new Point(0, 0);
            framesUntillDrop = framesBetweenAutoDrop;
            score = 0;
            lines = 0;
            gameOver = false;
        }

        private bool HasCollision()
        {
            return currentShape.HasCollision(field, pos);
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