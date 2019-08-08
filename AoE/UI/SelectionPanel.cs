using AoE.Actions;
using AoE.GameObjects;
using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using AoE.GameObjects.Units.Civilian;
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
            if (selectable is BaseGameObject selectedBaseGameObject)
            {
                var xOffset = 8d;
                var yOffset = 8d;

                // Draw info
                var nameText = new FormattedText(selectedBaseGameObject.Name, cultureInfo, flowDirection, typeface, 12d, foregroundBrush, pixelsPerDip);
                dc.DrawText(nameText, new Point(rect.X + xOffset, rect.Y + yOffset));

                yOffset += nameText.Height;

                if (selectedBaseGameObject is BaseUnit selectedBaseUnit)
                {
                    var hitpointText = new FormattedText($"Hitpoints: {selectedBaseUnit.HitPoints}/{selectedBaseUnit.HitPointsMax}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(hitpointText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    yOffset += hitpointText.Height;

                    if (selectedBaseUnit.MeleeAttack > 0)
                    {
                        var meleeAttackText = new FormattedText($"Melee attack: {selectedBaseUnit.MeleeAttack}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(meleeAttackText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += meleeAttackText.Height;
                    }
                    if (selectedBaseUnit.PierceAttack > 0)
                    {
                        var pierceAttackText = new FormattedText($"Pierce attack: {selectedBaseUnit.PierceAttack}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(pierceAttackText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += pierceAttackText.Height;
                    }
                    if (selectedBaseUnit.BlastRadius > 0)
                    {
                        var blastRadiusText = new FormattedText($"Blast radius: {selectedBaseUnit.BlastRadius}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(blastRadiusText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += blastRadiusText.Height;
                    }
                    if (selectedBaseUnit.MeleeArmor > 0)
                    {
                        var meleeArmorText = new FormattedText($"Melee armor: {selectedBaseUnit.MeleeArmor}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(meleeArmorText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += meleeArmorText.Height;
                    }
                    if (selectedBaseUnit.PierceArmor > 0)
                    {
                        var pierceArmorText = new FormattedText($"Pierce armor: {selectedBaseUnit.PierceArmor}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(pierceArmorText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        yOffset += pierceArmorText.Height;
                    }

                    if (selectedBaseGameObject is BaseRangedUnit selectedBaseRangedUnit)
                    {
                        if (selectedBaseRangedUnit.MaxRange > 0)
                        {
                            var rangeText = new FormattedText($"Range: {selectedBaseRangedUnit.MinRange} - {selectedBaseRangedUnit.MaxRange}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(rangeText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            yOffset += rangeText.Height;
                        }
                    }

                    if (selectedBaseGameObject is Villager selectedVillager && selectedVillager.action is Gather selectedVillagerGatherAction && !selectedVillagerGatherAction.Completed())
                    {
                        var gatherActionText = new FormattedText($"Gathering: {selectedVillagerGatherAction.AmountCarried}/{selectedVillagerGatherAction.AmountCarriedMax} ({selectedVillagerGatherAction.Resource.Type.ToString()})", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                        dc.DrawText(gatherActionText, new Point(rect.X + xOffset, rect.Y + yOffset));
                        //yOffset += gatherActionText.Height;
                    }
                }
                else if (selectedBaseGameObject is BaseResource selectedBaseResource)
                {
                    var hitpointText = new FormattedText($"Resources: {selectedBaseResource.Amount}/{selectedBaseResource.AmountMax} ({selectedBaseResource.Type.ToString()})", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(hitpointText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    //yOffset += hitpointText.Height;
                }
            }
        }
    }
}