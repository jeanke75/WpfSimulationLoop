using AoE.GameObjects.Buildings;

namespace AoE.GameObjects.Units
{
    interface IBuilder
    {
        void Build(IConstructable building);
    }
}