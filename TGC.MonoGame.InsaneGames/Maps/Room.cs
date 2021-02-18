using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Entities.Collectibles;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.InsaneGames.Entities.Bullets;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Maps
{
    abstract class Room : IDrawable 
    {
        protected List<Collectible> Collectibles { get; set; }
        protected List<Obstacle> Obstacles { get; set; }
        public bool Spawnable { get; protected set; }

        abstract public SpawnableSpace SpawnableSpace();

        abstract public bool IsInRoom(Vector3 center);
        abstract public Wall CollidesWithWall(Vector3 lowerPoint, Vector3 higherPoint);
        abstract public void CheckCollectiblesCollision(Player player);
        abstract public void CheckObstacleCollision(Enemy enemy);
        abstract public void CheckObstacleCollision(Player player);
        abstract public void CheckObstacleCollision(Bullet bullet);
        virtual public void AddCollectible(Collectible collectible)
        {
            Collectibles.Add(collectible);
        }
        virtual public void AddObstacle(Obstacle obstacle)
        {
            Obstacles.Add(obstacle);
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var collectible in Collectibles)
            {
                collectible.Update(gameTime);
            }

            foreach (var obstacle in Obstacles)
            {
                obstacle.Update(gameTime);
            }
        }
        public override void Load()
        {
            foreach (var collectible in Collectibles)
            {
                collectible.Load();
            }

            foreach (var obstacle in Obstacles)
            {
                obstacle.Load();
            }
        }

    }
}