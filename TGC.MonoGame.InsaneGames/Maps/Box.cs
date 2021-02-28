using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Entities.Collectibles;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class Box : Room
    {
        private Vector3 BottomVertex, UpVertex;
        private Dictionary<WallId, Wall> Walls = new Dictionary<WallId, Wall>();
        private SpawnableSpace SpawnSpace { get; set; }
        
        public Box(Dictionary<WallId, (BasicEffect, (float, float))> effects, Vector3 size, Vector3 center, List<Collectible> collectibles, List<Obstacle> obstacles, bool spawnable = true)
        {
            Vector2 floorSize = new Vector2(size.X, size.Z),
                    sideWallSize = new Vector2(size.Y, size.Z),
                    frontWallSize = new Vector2(size.X, size.Y);
            float xLength = size.X / 2,
                  yLength = size.Y / 2,
                  zLength = size.Z / 2;

            var allWalls = new Wall[] { 
                Wall.CreateFloor(floorSize, new Vector3(center.X, center.Y - yLength, center.Z)),
                Wall.CreateSideWall(sideWallSize, new Vector3(center.X + xLength, center.Y, center.Z), true),
                Wall.CreateSideWall(sideWallSize, new Vector3(center.X - xLength, center.Y, center.Z)),
                Wall.CreateFrontWall(frontWallSize, new Vector3(center.X, center.Y, center.Z - zLength)),
                Wall.CreateFrontWall(frontWallSize, new Vector3(center.X, center.Y, center.Z + zLength), back: true),
                Wall.CreateFloor(floorSize, new Vector3(center.X, center.Y + yLength, center.Z), ceiling: true)
            };
            
            foreach(var key in effects.Keys)
            {
                allWalls[(int)key].Effect = effects[key].Item1;
                allWalls[(int)key].TextureRepeat = (effects[key].Item2.Item1, effects[key].Item2.Item2);
                Walls.Add(key, allWalls[(int)key]);               
            }

            var spawnableArea = !spawnable ? new (Vector3, Vector3)[0] : new (Vector3, Vector3)[] {(size, center)};
            SpawnSpace = new SpawnableSpace(spawnableArea);
            
            Spawnable = spawnable;

            BottomVertex = new Vector3(center.X - xLength, center.Y - yLength, center.Z - zLength);
            UpVertex = new Vector3(center.X + xLength, center.Y + yLength, center.Z + zLength);

            Collectibles = collectibles;
            Obstacles = obstacles;
        }

        public override void Initialize(TGCGame game)
        {
            foreach (var wall in Walls.Values)
                wall.Initialize(game);

            foreach (var collectible in Collectibles)
                collectible.Initialize(game);
            
            foreach (var obstacle in Obstacles)
                obstacle.Initialize(game);
            
            base.Initialize(game);
        }

        public override void Load()
        {
            foreach (var collectible in Collectibles)
                collectible.Load();
            foreach (var wall in Walls.Values)
                wall.Load();

            base.Load();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var wall in Walls.Values)
                wall.Draw(gameTime);
            foreach (var collectible in Collectibles)
                collectible.Draw(gameTime);
            foreach (var obstacle in Obstacles)
                obstacle.Draw(gameTime);
        }

        public override void DrawBlack(GameTime gameTime)
        {
            foreach (var wall in Walls.Values)
                wall.DrawBlack(gameTime);
            foreach (var collectible in Collectibles)
                collectible.DrawBlack(gameTime);
            foreach (var obstacle in Obstacles)
                obstacle.DrawBlack(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var wall in Walls.Values)
                wall.Update(gameTime);
            foreach (var collectible in Collectibles)
                collectible.Update(gameTime);
            foreach (var obstacle in Obstacles)
                obstacle.Update(gameTime);
        }

        public override SpawnableSpace SpawnableSpace()
        {
            return SpawnSpace;           
        }

        public override bool IsInRoom(Vector3 point)
        {
            return BottomVertex.X <= point.X && point.X <= UpVertex.X &&
                    BottomVertex.Y <= point.Y && point.Y <= UpVertex.Y &&
                    BottomVertex.Z <= point.Z && point.Z <= UpVertex.Z;
        }

        public override Wall CollidesWithWall(Vector3 lowerPoint, Vector3 higherPoint)
        {
            foreach(var wall in Walls.Values)
                if(wall.Collides(lowerPoint, higherPoint)) return wall;
            return null;
        }

        public override void CheckCollectiblesCollision(Player player)
        {
            var collided = Collectibles.FindAll(coll => !coll.Collected && coll.CollidesWith(player.BottomVertex, player.UpVertex));
            //collided?.CollidedWith(player);
            collided.ForEach(c => c.CollidedWith(player));
        }
        public override void CheckObstacleCollision(Player player)
        {
            var collidedObstacle = Obstacles.Find(obs => player.CollidesWith(obs.BottomVertex, obs.UpVertex));
            player.CollidedWith(collidedObstacle);
        }
        public override void CheckObstacleCollision(Entities.Enemies.Enemy enemy)
        {
            var collidedObstacle = Obstacles.Find(obs => enemy.CollidesWith(obs.BottomVertex, obs.UpVertex));
            if(!(collidedObstacle is null)) enemy.CollidedWith(collidedObstacle);
        }
        public override void CheckObstacleCollision(Entities.Bullets.Bullet bullet)
        {
            var collidedObstacle = Obstacles.Find(obs => bullet.CollidesWith(obs.BottomVertex, obs.UpVertex));
            if(collidedObstacle != null) bullet.CollidedWith();
        }
    }
}