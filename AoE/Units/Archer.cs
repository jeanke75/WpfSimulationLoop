using System.Windows;
using System.Windows.Media;

namespace AoE.Units
{
    class Archer : BaseRangedUnit
    {
        public Archer(Vector position, Team team) : base(position, 10f, 30f, 30, 0, 4, 2.03f, 4, 0.8f, 7f, 0, 0, 0.96f, 6, Colors.Green, team)
        {
            AttackBonuses.Add(ArmorType.Spearman, 3);
            ArmorTypes.Add(ArmorType.Archer, 0);
        }
    }
}