using System.Windows;

namespace AoE.GameObjects.Units.Archers
{
    class Archer : BaseRangedUnit
    {
        public Archer(Vector position, Player owner) : base(position, 34f, 60f, "Archer", 30, 0, 4, 0f, 2.03f, 0, 4, 0.8f, 7f, 0, 0, 0.96f, 6, "Archer.png", owner)
        {
            AttackBonuses.Add(ArmorType.Spearman, 3);
            ArmorTypes.Add(ArmorType.Archer, 0);
        }
    }
}