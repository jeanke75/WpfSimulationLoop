using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings.Storage
{
    class MiningCamp : BaseBuilding, IStorage
    {
        public MiningCamp(int x, int y, Player owner) : base(x, y, 2, 2, "Mining Camp", 35, 600, "MiningCamp.png", owner)
        {
            ConstructionCost.Add(ResourceType.Wood, 100);
            
        }

        public bool CanStore(ResourceType type)
        {
            return type == ResourceType.Gold || type == ResourceType.Stone;
        }
    }
}