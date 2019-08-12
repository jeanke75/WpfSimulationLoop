using AoE.Actions;

namespace AoE.GameObjects
{
    interface IActionable
    {
        BaseAction GetAction();
    }
}