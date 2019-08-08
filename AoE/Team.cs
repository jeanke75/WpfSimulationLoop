using AoE.GameObjects.Resources;
using AoE.GameObjects.Units;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace AoE
{
    class Team
    {
        public readonly uint Id;
        public readonly Color Color;
        public readonly List<BaseUnit> Units;
        private readonly Dictionary<ResourceType, int> Resources = new Dictionary<ResourceType, int>(); 

        public Team(uint teamId, Color color)
        {
            Id = teamId;
            Color = color;
            Units = new List<BaseUnit>();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                Resources.Add(resourceType, 0);
            }
        }

        public int GetResource(ResourceType type)
        {
            return Resources[type];
        }

        public void SetResource(ResourceType type, int amount)
        {
            Resources[type] = amount > 0 ? amount : 0;
        }
    }
}