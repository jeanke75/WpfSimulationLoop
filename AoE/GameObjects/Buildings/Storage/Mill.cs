using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings.Storage
{
    class Mill : BaseBuilding, IStorage
    {
        public Mill(int x, int y, Player owner) : base(x, y, 2, 2, "Mill", 35, 600, "Mill.png", owner)
        {
            ConstructionCost.Add(ResourceType.Wood, 100);
        }

        public bool CanStore(ResourceType type)
        {
            return type == ResourceType.Food;
        }
    }
}