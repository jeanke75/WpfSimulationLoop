using System.Windows;

namespace AoE.Units
{
    class ScoutCavalry : BaseUnit
    {
        public ScoutCavalry(Vector position, Team team) : base(position, 18f, 18f, "Scout Cavalry", 45, 3, 0, 0f, 2.03f, 0, 2, 1.2f, 4, "ScoutCavalry.png", team)
        {
            AttackBonuses.Add(ArmorType.Monk, 6);
            ArmorTypes.Add(ArmorType.Cavalry, 0);
        }
    }
}