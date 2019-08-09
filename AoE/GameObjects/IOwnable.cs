namespace AoE.GameObjects
{
    interface IOwnable
    {
        Player GetOwner();

        void SetOwner(Player owner);
    }
}