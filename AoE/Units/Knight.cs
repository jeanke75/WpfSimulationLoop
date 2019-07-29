using System.Windows;

namespace AoE.Units
{
    class Knight : BaseUnit
    {
        public Knight(Vector position, Team team) : base(position, 18f, 18f, "Knight", 100, 10, 0, 0f, 1.83f, 2, 2, 1.35f, 4, "Knight.png", team)
        {
            ArmorTypes.Add(ArmorType.Cavalry, 0);
        }
    }
}