namespace AoE.GameObjects
{
    interface IRangedCombat : ICombat
    {
        double GetAttackRangeMin();
        double GetAttackRangeMax();
        float GetAccuracy();
        float GetProjectileSpeed();
    }
}