using AoE.GameObjects.Units;
using DrawingBase;
using System.Windows;

namespace AoE.Actions
{
    class Move : BaseAction
    {
        private readonly BaseUnit unit;
        private readonly Vector position;

        public Move(BaseUnit unitToMove, Vector positionToMoveTo)
        {
            unit = unitToMove;
            position = positionToMoveTo;
        }

        public override void Do(float dt)
        {
            if (!Completed())
                unit.Position = unit.Position.MoveTowards(position, dt, unit.Speed * MainWindow.tilesize);
        }

        public override bool Completed()
        {
            return unit.Position.X == position.X && unit.Position.Y == position.Y;
        }
    }
}