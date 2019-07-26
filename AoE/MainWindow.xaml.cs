using AoE.Units;
using DrawingBase;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AoE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        public static readonly Random random = new Random();
        public static readonly double tilesize = 10f;
        public static readonly bool ShowLineOfSight = true;
        public static readonly bool ShowAttackRange = true;
        public static readonly bool ShowTimeUntillAttack = true;
        readonly List<Team> teams = new List<Team>();
        readonly List<BaseUnit> units = new List<BaseUnit>();

        public override void Initialize()
        {
            SetResolution(800, 600);

            for (uint i = 0; i < 2; i++)
            {
                teams.Add(new Team(i, Color.FromRgb((byte)random.Next(50, 255), (byte)random.Next(50, 255), (byte)random.Next(50, 255))));
            }

            for (int i = 0; i < 1; i++)
            {
                units.Add(new Knight(new Vector((random.NextDouble() + 1) * tilesize, 200 + i * 30), teams[0]));
            }
            for (int i = 0; i < 1; i++)
            {
                units.Add(new Archer(new Vector((random.NextDouble() + 7) * tilesize, 200 + i * 30), teams[1]));
            }
        }

        public override void Update(float dt)
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                var unit = units[i];
                if (unit.HitPoints > 0)
                    unit.Update(dt, units);
                else
                    units.Remove(unit);
            }
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (BaseUnit unit in units)
            {
                unit.Draw(dc, teams);
            }
        }

        public override void Cleanup()
        {
        }
    }
}