using AoE.Units;
using DrawingBase;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;

namespace AoE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DrawingWindowBase
    {
        readonly List<Team> teams = new List<Team>();

        public override void Initialize()
        {
            SetResolution(800, 600);

            var team1 = new Team(0, Colors.Yellow);
            for (int i = 0; i < 1; i++)
            {
                team1.Units.Add(new LongSwordsman(new Vector2(15, 200 + i * 30), team1));
            }
            teams.Add(team1);

            var team2 = new Team(1, Colors.Cyan);
            for (int i = 0; i < 1; i++)
            {
                team2.Units.Add(new LongSwordsman(new Vector2(45, 200 + i * 30), team2));
            }
            teams.Add(team2);
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(DrawingContext dc)
        {
            var units = teams.SelectMany(x => x.Units).ToList();
            foreach (Unit unit in units)
            {
                unit.Draw(dc, teams);
            }
        }

        public override void Cleanup()
        {
        }
    }
}