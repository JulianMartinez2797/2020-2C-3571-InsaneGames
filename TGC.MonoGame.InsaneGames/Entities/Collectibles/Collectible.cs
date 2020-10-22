﻿using Microsoft.Xna.Framework;
namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    abstract class Collectible : Entity 
    {
        public bool Collected { get; protected set; } = false;
        abstract public Vector3 Position { get; }
        abstract public void CollidedWith(Player player);
    }
}
