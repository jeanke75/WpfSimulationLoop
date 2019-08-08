using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings.Storage
{
    class Mill : BaseBuilding, IStorage
    {
        public Mill(int x, int y, Team owner) : base(x, y, 2, 2, "Mill", "Mill.png", owner) { }

        public bool CanStore(ResourceType type)
        {
            return type == ResourceType.Food;
        }
    }
}