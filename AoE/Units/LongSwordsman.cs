using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;

namespace AoE.Units
{
    class LongSwordsman : Unit
    {
        public LongSwordsman(Vector2 position, Team team) : base(position, 10f, 30f, 60, 9, DamageType.Melee, 2.03f, 0, 1, ArmorType.Infantry, 0.9f, 4, Colors.Gray, team) { }

        public override void Update(float dt)
        {

        }

        public override void Draw(DrawingContext dc, List<Team> teams)
        {
            base.Draw(dc, teams);
            // todo attack
        }
    }
}