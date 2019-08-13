﻿using System.Collections.Generic;
using System.Windows;
using AoE.GameObjects.Resources;

namespace AoE.GameObjects.Buildings
{
    class BaseBuilding : BaseGameObject, ISelectable, IConstructable, IDestroyable, IOwnable
    {
        private readonly float ConstructionTimeTotal;
        private float _constructionTime;
        private float ConstructionTime
        {
            get
            {
                return _constructionTime;
            }
            set
            {
                if (value < 0)
                    _constructionTime = 0;
                else if (value > ConstructionTimeTotal)
                    _constructionTime = ConstructionTimeTotal;
                else
                    _constructionTime = value;
            }
        }
        private bool MultipleBuildersCheck;
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
            ConstructionTimeTotal = 15;
            ConstructionTime = ConstructionTimeTotal;
            MultipleBuildersCheck = false;

            // TODO, init zoals bij units
            HitPointsMax = 100;
            HitPoints = HitPointsMax;
            this.owner = owner;
        }

        public void Update()
        {
            MultipleBuildersCheck = false;
        }

        #region IConstructable
        public Dictionary<ResourceType, int> GetConstructionCost()
        {
            return new Dictionary<ResourceType, int>
            {
                { ResourceType.Gold, 1 },
                { ResourceType.Stone, 2 },
                { ResourceType.Wood, 3 }
            };
        }

        public float GetConstructionTimeTotal()
        {
            return ConstructionTimeTotal;
        }

        public float GetConstructionTime()
        {
            return ConstructionTime;
        }

        public void SetConstructionTime(float time)
        {
            ConstructionTime = time;
            MultipleBuildersCheck = true;
        }

        public bool GetMultipleBuildersCheck()
        {
            return MultipleBuildersCheck;
        }

        public void SetMultipleBuildersCheck(bool check)
        {
            MultipleBuildersCheck = check;
        }
        #endregion

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