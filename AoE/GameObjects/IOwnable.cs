namespace AoE.GameObjects
{
    interface IOwnable
    {
        Team GetOwner();

        void SetOwner(Team owner);
    }
}