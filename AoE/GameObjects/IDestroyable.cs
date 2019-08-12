namespace AoE.GameObjects
{
    interface IDestroyable
    {
        int GetHitPoints();
        int GetHitPointsMax();
        void TakeDamage(int damage);
        bool Destroyed();
    }
}