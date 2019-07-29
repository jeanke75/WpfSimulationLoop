using System.Windows;

namespace AoE.Units
{
    class Militia : BaseUnit
    {
        public Militia(Vector position, Team team) : base(position, 18f, 18f, "Militia", 40, 4, 0, 0, 2.03f, 0, 1, 0.9f, 4, "Militia.png", team)
        {
            ArmorTypes.Add(ArmorType.Infantry, 0);
        }
    }
}