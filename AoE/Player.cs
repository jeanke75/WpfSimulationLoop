using AoE.GameObjects.Resources;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace AoE
{
    class Player
    {
        public readonly uint Id;
        public readonly Color Color;
        private readonly Dictionary<ResourceType, int> Resources = new Dictionary<ResourceType, int>();

        public Player(uint playerId, Color color)
        {
            Id = playerId;
            Color = color;
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