using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Entities.Collectibles;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;
using TGC.MonoGame.InsaneGames.Entities.Enemies;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class MazeBuilder
    {
        protected Box[] Grid;
        protected float SizeX, SizeZ;
        protected Vector3 RoomSize;
        protected float FloorLevel;
        protected (float, float) ZeroPoint;
        protected Enemy[] Enemies;
        protected Player Player;
        public MazeBuilder(int sizeX, int sizeZ, Vector3 roomSize)
        {
            Grid = new Box[sizeX * sizeZ];
            SizeX = sizeX;
            SizeZ = sizeZ;
            RoomSize = roomSize;
            ZeroPoint = (roomSize.X / 2, roomSize.Z / 2);
            FloorLevel = roomSize.Y / 2;
        }
        public MazeBuilder AddBox(int x, int z,  Dictionary<WallId, (BasicEffect, (float, float))> walls, bool spawnable = true)
        {
            if(SizeX <= x || SizeZ <= z || x < 0 || z < 0)
                throw new Exception("Room position out of bounds");
            var center = new Vector3(ZeroPoint.Item1 + RoomSize.X * x, FloorLevel, ZeroPoint.Item2 + RoomSize.Z * z);
            var index = (int) (x * SizeZ + z);
            Grid[index] = new Box(walls, RoomSize, center, new List<Collectible>(), new List<Obstacle>(), spawnable);
            return this;
        }
        public MazeBuilder AddCollectible(Collectible collectible)
        {
            var found = Array.Find(Grid, room => room?.IsInRoom(collectible.Position) ?? false);
            if(found is null)
                throw new Exception("No room was present for the collectible");
            else
                found.AddCollectible(collectible);
            return this;
        }
        public MazeBuilder AddObstacle(Obstacle obstacle)
        {
            var found = Array.Find(Grid, room => room?.IsInRoom(obstacle.Position) ?? false);
            if(found is null)
                throw new Exception("No room was present for the obstacle");
            else
                found.AddObstacle(obstacle);
            return this;
        }
        public MazeBuilder SetEnemies(Enemy[] enemies)
        {
            Enemies = enemies;
            return this;
        }
        public MazeBuilder SetPlayer(Player player)
        {
            Player = player;
            return this;
        }
        public Map BuildMaze()
        {
            return new Map(Grid, Enemies, Player);
        }
    }
}