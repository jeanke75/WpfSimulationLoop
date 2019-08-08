using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class Archer : BaseRangedUnit
    {
        public Archer(Vector position, Team owner) : base(position, 18f, 18f, "Archer", 30, 0, 4, 0f, 2.03f, 0, 4, 0.8f, 7f, 0, 0, 0.96f, 6, "Archer.png", owner)
        {
            AttackBonuses.Add(ArmorType.Spearman, 3);
            ArmorTypes.Add(ArmorType.Archer, 0);
        }
    }
}