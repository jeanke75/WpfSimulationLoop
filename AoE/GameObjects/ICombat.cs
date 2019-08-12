using AoE.GameObjects.Units;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AoE.GameObjects
{
    interface ICombat : IDestroyable, IActionable // Anything that can fight, can be destroyed aswell
    {
        int GetMeleeAttack();
        int GetPierceAttack();
        ReadOnlyDictionary<ArmorType, int> GetAttackBonuses();
        float GetBlastRadius();
        float GetRateOfFire();
        float GetTimeUntillNextAttack();
        void ResetTimeUntillNextAttack();
        int GetMeleeArmor();
        int GetPierceArmor();
        ReadOnlyDictionary<ArmorType, int> GetArmorTypes();

        void Attack(IDestroyable destroyable, List<BaseUnit> destroyables);
    }
}