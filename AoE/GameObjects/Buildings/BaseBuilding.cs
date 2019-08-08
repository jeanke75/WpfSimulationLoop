using System.Windows;

namespace AoE.GameObjects.Buildings
{
    class BaseBuilding : BaseGameObject, ISelectable, IOwnable
    {
        private Team owner;
        public BaseBuilding(int x, int y, int tilesWide, int tilesHigh, string name, string imageId, Team owner) : base(new Vector(x * MainWindow.tilesize, y * MainWindow.tilesize), tilesWide * MainWindow.tilesize, tilesHigh * MainWindow.tilesize, name, "Buildings/" + imageId)
        {
            // TODO, quick and dirty to test gathering
            this.owner = owner;
        }

        public Team GetOwner()
        {
            return owner;
        }

        public void SetOwner(Team newOwner)
        {
            owner = newOwner;
        }
    }
}