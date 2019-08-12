﻿using System.Windows;

namespace AoE.GameObjects.Buildings
{
    class BaseBuilding : BaseGameObject, ISelectable, IDestroyable, IOwnable
    {
        private readonly int HitPointsMax;
        private int _HitPoints;
        protected int HitPoints
        {
            get
            {
                return _HitPoints;
            }
            set
            {
                if (value < 0)
                    _HitPoints = 0;
                else if (value > HitPointsMax)
                    _HitPoints = HitPointsMax;
                else
                    _HitPoints = value;
            }
        }
        private Player owner;
        public BaseBuilding(int x, int y, int tilesWide, int tilesHigh, string name, string imageId, Player owner) : base(new Vector(x * MainWindow.tilesize, y * MainWindow.tilesize), tilesWide * MainWindow.tilesize, tilesHigh * MainWindow.tilesize, name, "Buildings/" + imageId)
        {
            // TODO, init zoals bij units
            HitPointsMax = 100;
            HitPoints = HitPointsMax;
            this.owner = owner;
        }

        #region IDestroyable
        public int GetHitPoints()
        {
            return HitPoints;
        }

        public int GetHitPointsMax()
        {
            return HitPointsMax;
        }

        public void TakeDamage(int damage)
        {
            HitPoints -= damage;
        }

        public bool Destroyed()
        {
            return HitPoints == 0;
        }
        #endregion

        #region IOwnable
        public Player GetOwner()
        {
            return owner;
        }

        public void SetOwner(Player newOwner)
        {
            owner = newOwner;
        }
        #endregion
    }
}