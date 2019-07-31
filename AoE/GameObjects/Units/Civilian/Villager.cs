using System.Windows;

namespace AoE.GameObjects.Units.Civilian
{
    class Villager : BaseUnit
    {
        public Villager(Vector position, Team team) : base(position, 18f, 18f, "Villager", 25, 3, 0, 0f, 2.03f, 0, 0, 0.8f, 4, "Villager.png", team)
        {
            AttackBonuses.Add(ArmorType.StoneDefense, 6);
            AttackBonuses.Add(ArmorType.Building, 3);
        }
    }
}