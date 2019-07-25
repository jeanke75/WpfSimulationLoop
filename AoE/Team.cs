using System.Collections.Generic;
using System.Windows.Media;

namespace AoE
{
    class Team
    {
        public readonly uint Id;
        public readonly Color Color;
        public readonly List<Unit> Units;

        public Team(uint teamId, Color color)
        {
            Id = teamId;
            Color = color;
            Units = new List<Unit>();
        }
    }
}