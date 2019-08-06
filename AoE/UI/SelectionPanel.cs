using AoE.GameObjects;
using AoE.GameObjects.Units;
using DrawingBase;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace AoE.UI
{
    class SelectionPanel
    {
        private readonly Rect rect;
        private readonly Brush backgroundBrush;

        private readonly CultureInfo cultureInfo;
        private readonly FlowDirection flowDirection;
        private readonly Typeface typeface;
        private readonly Brush foregroundBrush;
        private readonly double pixelsPerDip;

        public SelectionPanel(DrawingWindowBase window)
        {
            rect = new Rect(0, window.GetHeight() - 100, window.GetWidth(), 100);
            backgroundBrush = Brushes.SandyBrown;
            backgroundBrush.Freeze();

            cultureInfo = CultureInfo.CurrentCulture;
            flowDirection = FlowDirection.LeftToRight;
            typeface = new Typeface("Georgia");
            foregroundBrush = Brushes.Black;
            pixelsPerDip = VisualTreeHelper.GetDpi(window).PixelsPerDip;
        }

        public void Draw(DrawingContext dc, ISelectable selectable)
        {
            // Draw panel background
            dc.DrawRectangle(Brushes.SandyBrown, null, rect);

            // Draw selected unit info
            if (selectable is BaseGameObject selectedObject)
            {
                var xOffset = 8d;
                var yOffset = 8d;

                // Draw info
                var nameText = new FormattedText(selectedObject.Name, cultureInfo, flowDirection, typeface, 12d, foregroundBrush, pixelsPerDip);
                dc.DrawText(nameText, new Point(rect.X + xOffset, rect.Y + yOffset));

                yOffset += nameText.Height;

                if (selectedObject is BaseUnit baseUnit)
                {
                    var hitpointText = new FormattedText("Hitpoints: " + baseUnit.HitPoints + "/" + baseUnit.HitPointsMax, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(hitpointText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    yOffset += hitpointText.Height;

                    if (baseUnit.MeleeAttack > 0)
                    {
                        var meleeAttackText = new FormattedText("Melee attack: " + baseUnit.MeleeAttack, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(meleeAttackText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += meleeAttackText.Height;
                    }
                    if (baseUnit.PierceAttack > 0)
                    {
                        var pierceAttackText = new FormattedText("Pierce attack: " + baseUnit.PierceAttack, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(pierceAttackText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += pierceAttackText.Height;
                    }
                    if (baseUnit.BlastRadius > 0)
                    {
                        var blastRadiusText = new FormattedText("Blast radius: " + baseUnit.BlastRadius, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(blastRadiusText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += blastRadiusText.Height;
                    }
                    if (baseUnit.MeleeArmor > 0)
                    {
                        var meleeArmorText = new FormattedText("Melee armor: " + baseUnit.MeleeArmor, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(meleeArmorText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += meleeArmorText.Height;
                    }
                    if (baseUnit.PierceArmor > 0)
                    {
                        var pierceArmorText = new FormattedText("Pierce armor: " + baseUnit.PierceArmor, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(pierceArmorText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += pierceArmorText.Height;
                    }

                    if (selectedObject is BaseRangedUnit baseRangedUnit)
                    {
                        if (baseRangedUnit.MaxRange > 0)
                        {
                            var rangeText = new FormattedText("Range: " + baseRangedUnit.MinRange + " - " + baseRangedUnit.MaxRange, cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(rangeText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            //yOffset += rangeText.Height;
                        }
                    }
                }
            }
        }
    }
}