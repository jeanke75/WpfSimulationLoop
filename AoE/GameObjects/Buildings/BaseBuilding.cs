using System.Windows;

namespace AoE.GameObjects.Buildings
{
    class BaseBuilding : BaseGameObject, ISelectable, IOwnable
    {
        private Player owner;
        public BaseBuilding(int x, int y, int tilesWide, int tilesHigh, string name, string imageId, Player owner) : base(new Vector(x * MainWindow.tilesize, y * MainWindow.tilesize), tilesWide * MainWindow.tilesize, tilesHigh * MainWindow.tilesize, name, "Buildings/" + imageId)
        {
            // TODO, quick and dirty to test gathering
            this.owner = owner;
        }

        public Player GetOwner()
        {
            return owner;
        }

        public void SetOwner(Player newOwner)
        {
            owner = newOwner;
        }
    }
}