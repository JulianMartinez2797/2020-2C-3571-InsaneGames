using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Entities.Enemies
{
    abstract class Enemy : Entity, Entities.Obstacles.ObstacleCollisionable
    {
        public bool floorEnemy { get; protected set; }
        public float Life { get; protected set; }
        public float Damage { get; protected set; }
        abstract public Vector3 Position { get; set;} 

        public virtual void RemoveFromLife(float amount) 
        {
            Life = Math.Max(Life - amount, 0);
        }

        public virtual void CollidedWith(Player player)
        {
            player.BeAttacked(Damage);
        }

        public abstract void CollidedWith(Entities.Obstacles.Obstacle obstacle);

        public abstract bool PositionSet();
    }
}