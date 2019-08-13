using AoE.Actions;
using AoE.GameObjects;
using AoE.GameObjects.Buildings;
using AoE.GameObjects.Resources;
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

                if (selectedBaseGameObject is IDestroyable selectedDestroyable)
                {
                    var hitpointText = new FormattedText($"Hitpoints: {selectedDestroyable.GetHitPoints()}/{selectedDestroyable.GetHitPointsMax()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(hitpointText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    yOffset += hitpointText.Height;

                    if (selectedDestroyable is ICombat selectedCombat)
                    {
                        if (selectedCombat.GetMeleeAttack() > 0)
                        {
                            var meleeAttackText = new FormattedText($"Melee attack: {selectedCombat.GetMeleeAttack()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(meleeAttackText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            yOffset += meleeAttackText.Height;
                        }
                        if (selectedCombat.GetPierceAttack() > 0)
                        {
                            var pierceAttackText = new FormattedText($"Pierce attack: {selectedCombat.GetPierceAttack()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(pierceAttackText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            yOffset += pierceAttackText.Height;
                        }
                        if (selectedCombat.GetBlastRadius() > 0)
                        {
                            var blastRadiusText = new FormattedText($"Blast radius: {selectedCombat.GetBlastRadius()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(blastRadiusText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            yOffset += blastRadiusText.Height;
                        }
                        if (selectedCombat.GetMeleeArmor() > 0)
                        {
                            var meleeArmorText = new FormattedText($"Melee armor: {selectedCombat.GetMeleeArmor()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(meleeArmorText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            yOffset += meleeArmorText.Height;
                        }
                        if (selectedCombat.GetPierceArmor() > 0)
                        {
                            var pierceArmorText = new FormattedText($"Pierce armor: {selectedCombat.GetPierceArmor()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                            dc.DrawText(pierceArmorText, new Point(rect.X + xOffset, rect.Y + yOffset));
                            yOffset += pierceArmorText.Height;
                        }

                        if (selectedCombat is IRangedCombat selectedRangedCombat)
                        {
                            if (selectedRangedCombat.GetAttackRangeMax() > 0)
                            {
                                var rangeText = new FormattedText($"Range: {selectedRangedCombat.GetAttackRangeMin()} - {selectedRangedCombat.GetAttackRangeMax()}", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                                dc.DrawText(rangeText, new Point(rect.X + xOffset, rect.Y + yOffset));
                                yOffset += rangeText.Height;
                            }
                        }
                    }
                }

                if (selectedBaseGameObject is IActionable selectedActionable && selectedActionable.GetAction() is Gather selectedActionableGatherAction && !selectedActionableGatherAction.Completed())
                {
                    var gatherActionText = new FormattedText($"Gathering: {selectedActionableGatherAction.AmountCarried}/{selectedActionableGatherAction.AmountCarriedMax} ({selectedActionableGatherAction.Resource.Type.ToString()})", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(gatherActionText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    yOffset += gatherActionText.Height;
                }

                if (selectedBaseGameObject is BaseResource selectedBaseResource)
                {
                    var hitpointText = new FormattedText($"Resources: {selectedBaseResource.Amount}/{selectedBaseResource.AmountMax} ({selectedBaseResource.Type.ToString()})", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(hitpointText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    yOffset += hitpointText.Height;
                }

                if (selectedBaseGameObject is IConstructable selectedConstructable && selectedConstructable.GetConstructionTime() > 0)
                {
                    var constructionTimeText = new FormattedText($"Constructing: {((selectedConstructable.GetConstructionTimeTotal() - selectedConstructable.GetConstructionTime()) / selectedConstructable.GetConstructionTimeTotal() * 100).ToString("0.00")}% ", cultureInfo, flowDirection, typeface, 10d, foregroundBrush, pixelsPerDip);
                    dc.DrawText(constructionTimeText, new Point(rect.X + xOffset, rect.Y + yOffset));
                    //yOffset += hitpointText.Height;
                }
            }
        }
    }
}