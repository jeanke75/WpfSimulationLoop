using System.Windows;

namespace AoE.GameObjects.Units
{
    interface IMoveable : IActionable
    {
        float GetMovementSpeed();
        void MoveTo(Vector destination);
    }
}