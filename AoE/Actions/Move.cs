using AoE.GameObjects;
using AoE.GameObjects.Units;
using DrawingBase;
using System.Windows;

namespace AoE.Actions
{
    class Move : BaseAction
    {
        private readonly IMoveable moveableObject;
        private readonly Vector position;

        public Move(IMoveable moveable, Vector positionToMoveTo)
        {
            moveableObject = moveable;
            position = positionToMoveTo;
        }

        public override void Do(float dt)
        {
            if (!Completed())
            {
                BaseGameObject gameObject = moveableObject as BaseGameObject;
                gameObject.Position = gameObject.Position.MoveTowards(position, dt, moveableObject.GetMovementSpeed() * MainWindow.TileSize);
            }
        }

        public override bool Completed()
        {
            BaseGameObject gameObject = moveableObject as BaseGameObject;
            return gameObject.Position.X == position.X && gameObject.Position.Y == position.Y;
        }
    }
}