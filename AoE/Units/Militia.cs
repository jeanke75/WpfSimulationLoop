using System.Windows;
using System.Windows.Media;

namespace AoE.Units
{
    class Knight : BaseUnit
    {
        public Knight(Vector position, Team team) : base(position, 10f, 30f, 100, 10, 0, 1.83f, 2, 2, 1.35f, 4, Colors.LightBlue, team)
        {
            ArmorTypes.Add(ArmorType.Cavalry, 0);
        }
    }
}