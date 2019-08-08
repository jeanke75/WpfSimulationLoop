using System.Windows;

namespace AoE.GameObjects.Resources
{
    enum ResourceType
    {
        Food,
        Gold,
        Stone,
        Wood,
    }

    abstract class BaseResource : BaseGameObject, ISelectable
    {
        public readonly ResourceType Type;
        public readonly int AmountMax;
        private int _Amount;
        public int Amount
        {
            get
            {
                return _Amount;
            }
            private set
            {
                if (value < 0)
                {
                    _Amount = 0;
                }
                else
                {
                    _Amount = value;
                }
            }
        }

        public BaseResource(Vector position, double width, double height, string name, ResourceType type, int amount, string imageId) : base(position, width, height, name, "Resources/" + imageId)
        {
            Type = type;
            AmountMax = amount;
            Amount = AmountMax;
        }

        public int Gather(int amountToGather)
        {
            var amountGathered = Amount >= amountToGather ? amountToGather : Amount;
            Amount -= amountGathered;
            return amountGathered;
        }
    }
}