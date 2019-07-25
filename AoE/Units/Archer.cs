using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;

namespace AoE.Units
{
    class Archer : Unit
    {
        public int Range { get; private set; }
        public int Accuracy { get; private set; }
        public int ProjectileSpeed { get; private set; }

        public Archer(Vector2 position, Team team) : base(position, 10f, 30f, 60, 9, DamageType.Pierce, 2.03f, 0, 0, ArmorType.Archer, 0.96f, 6, Colors.Green, team)
        {
            Range = 4;
            Accuracy = 80;
            ProjectileSpeed = 7;
        }

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