using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings
{
    class TownCenter : BaseBuilding, IStorage
    {
        public TownCenter(int x, int y, Player owner) : base(x, y, 4, 4, "Town Center", 150, 2400, "TownCenter.png", owner)
        {
            ConstructionCost.Add(ResourceType.Stone, 275);
            ConstructionCost.Add(ResourceType.Wood, 100);
        }

        public bool CanStore(ResourceType type)
        {
            return type == ResourceType.Food || type == ResourceType.Gold || type == ResourceType.Stone || type == ResourceType.Wood;
        }
    }
}