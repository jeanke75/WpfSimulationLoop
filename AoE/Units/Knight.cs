using System.Windows;
using System.Windows.Media;

namespace AoE.Units
{
    class Militia : BaseUnit
    {
        public Militia(Vector position, Team team) : base(position, 10f, 30f, 40, 4, 0, 2.03f, 0, 1, 0.9f, 4, Colors.Gray, team)
        {
            ArmorTypes.Add(ArmorType.Infantry, 0);
        }
    }
}