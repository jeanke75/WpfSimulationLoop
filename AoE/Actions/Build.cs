using AoE.GameObjects;
using AoE.GameObjects.Buildings;
using AoE.GameObjects.Units;
using DrawingBase;

namespace AoE.Actions
{
    class Build : BaseAction
    {
        private readonly IBuilder builder;
        private readonly IConstructable building;
        public Build(IBuilder builder, IConstructable building)
        {
            this.builder = builder;
            this.building = building;
        }

        public override void Do(float dt)
        {
            if (!Completed())
            {
                var builderObject = builder as BaseGameObject;
                var buildingObject = building as BaseGameObject;
                var distance = builderObject.Distance(building as BaseGameObject) / MainWindow.tilesize;
                if (distance < 0.1f)
                {
                    bool firstBuilder = !building.GetMultipleBuildersCheck();
                    float workDone = firstBuilder ? dt : dt / 3f; // The first worker does 3x as much work as any other
                    building.SetConstructionTime(building.GetConstructionTime() - workDone);
                    building.SetMultipleBuildersCheck(true);
                }
                else
                {
                    builderObject.Position = builderObject.Position.MoveTowards(buildingObject.Position, dt, (builder as IMoveable).GetMovementSpeed() * MainWindow.tilesize);
                }
            }
        }

        public override bool Completed()
        {
            return building.GetConstructionTime() == 0;
        }
    }
}