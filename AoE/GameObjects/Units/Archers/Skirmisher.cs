using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class Skirmisher : BaseRangedUnit
    {
        public Skirmisher(Vector position, Player owner) : base(position, 18f, 18f, "Skirmisher", 30, 0, 2, 0f, 3.05f, 1, 4, 0.9f, 7f, 0, 3, 0.96f, 6, "Skirmisher.png", owner)
        {
            AttackBonuses.Add(ArmorType.Archer, 3);
            AttackBonuses.Add(ArmorType.Spearman, 3);
            ArmorTypes.Add(ArmorType.Archer, 0);
        }
    }
}