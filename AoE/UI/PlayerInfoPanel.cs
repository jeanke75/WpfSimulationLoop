using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using DrawingBase;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AoE.UI
{
    class PlayerInfoPanel
    {
        private readonly Rect rect;
        private readonly Brush backgroundBrush;

        private readonly CultureInfo cultureInfo;
        private readonly FlowDirection flowDirection;
        private readonly Typeface typeface;
        private readonly Brush foregroundBrush;
        private readonly double pixelsPerDip;

        public PlayerInfoPanel(DrawingWindowBase window)
        {
            rect = new Rect(6, 5, 500, 20);
            backgroundBrush = Brushes.Black;
            backgroundBrush.Freeze();

            cultureInfo = CultureInfo.CurrentCulture;
            flowDirection = FlowDirection.LeftToRight;
            typeface = new Typeface("Georgia");
            foregroundBrush = Brushes.White;
            pixelsPerDip = VisualTreeHelper.GetDpi(window).PixelsPerDip;
        }

        public void Draw(DrawingContext dc, Player player, List<BaseUnit> units)
        {
            // Draw panel background
            dc.DrawRectangle(backgroundBrush, null, rect);

            var xOffset = 8d;
            var yOffset = 5d;

            // Draw resource counters
            var foodText = new FormattedText($"Food: {player.GetResource(ResourceType.Food)}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
            dc.DrawText(foodText, new Point(rect.X + xOffset, rect.Y + yOffset));
            xOffset += 100;

            var goldText = new FormattedText($"Gold: {player.GetResource(ResourceType.Gold)}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
            dc.DrawText(goldText, new Point(rect.X + xOffset, rect.Y + yOffset));
            xOffset += 100;

            var stoneText = new FormattedText($"Stone: {player.GetResource(ResourceType.Stone)}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
            dc.DrawText(stoneText, new Point(rect.X + xOffset, rect.Y + yOffset));
            xOffset += 100;

            var woodText = new FormattedText($"Wood: {player.GetResource(ResourceType.Wood)}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
            dc.DrawText(woodText, new Point(rect.X + xOffset, rect.Y + yOffset));
            xOffset += 100;

            // Draw unit counter
            var unitsText = new FormattedText($"Units: {units.Count(x => x.GetOwner() == player)}/???", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
            dc.DrawText(unitsText, new Point(rect.X + xOffset, rect.Y + yOffset));
            //xOffset += 100;
        }
    }
}