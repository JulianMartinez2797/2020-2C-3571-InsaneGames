using System;
using System.Collections.Generic;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.InsaneGames.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Cameras;
using TGC.MonoGame.InsaneGames.Entities.Bullets;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class Map : IDrawable
    {
        private Room[] Rooms;
        private Enemy[] Enemies;
        private Random Random;
        private Player Player;
        private List<Bullet> Bullets;
        private InfoUI UI;
        public bool keyFound = false;

        public Camera Camera => Player.Camera;

        public Map(Room[] rooms, Enemy[] enemies, Player player) 
        {
            Rooms = rooms;
            Enemies = enemies;
            Random = new Random();
            Player = player;
            Bullets = new List<Bullet>();
            UI = new InfoUI();
        }

        public override void Initialize(TGCGame game)
        {
            base.Initialize(game);

            Player.Initialize(game);

            foreach (var room in Rooms)
                room.Initialize(game);

            Array.ForEach(Enemies, (enemy) => {
                enemy.Initialize(game);
                if(enemy.PositionSet()) return;
                this.SetPositionOfEnemy(enemy);
            });

        }

        public override void Draw(GameTime gameTime)
        {
            Player.Draw(gameTime);

            foreach (var room in Rooms)
                room.Draw(gameTime);

            foreach (var enemy in Enemies)
                enemy.Draw(gameTime);
            
            UI.Draw(gameTime, Player);
        }
        public void Load(GraphicsDevice gd) 
        {
            UI.Load(gd);

            Player.Load();

            foreach (var room in Rooms)
                room.Load();

            foreach (var enemy in Enemies)
                enemy.Load();
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var room in Rooms)
            {
                room.Update(gameTime);
                Boolean playerInRoom = false;
                if(room.IsInRoom(Player.NewPosition))
                {
                    
                    Player.Update(gameTime);
                    
                    var collidedWall = room.CollidesWithWall(Player.BottomVertex, Player.UpVertex);
                    Player.CollidedWith(collidedWall);
                    room.CheckCollectiblesCollision(Player);
                    room.CheckObstacleCollision(Player);
                    playerInRoom = true;
                }

                var bullets = Bullets.FindAll(bullet => room.IsInRoom(bullet.BottomVertex) || room.IsInRoom(bullet.UpVertex));
                var enemies = Array.FindAll(Enemies, enemy => room.IsInRoom(enemy.Position));
                bullets.ForEach(b => b.Update(gameTime));
                foreach(var enemy in enemies)
                {
                    enemy.Update(gameTime);
                    var collidedWall = room.CollidesWithWall(enemy.BottomVertex, enemy.UpVertex);
                    if(!(collidedWall is null)) enemy.CollidedWith(collidedWall);
                    room.CheckObstacleCollision(enemy);
                    if(playerInRoom && Player.CollidesWith(enemy.BottomVertex, enemy.UpVertex))
                    { 
                        enemy.CollidedWith(Player);
                    }
                    var collidedBullets = bullets.FindAll(bullets => bullets.CollidesWith(enemy.BottomVertex, enemy.UpVertex));
                    collidedBullets.ForEach(b => b.CollidedWith(enemy));
                }
                bullets.FindAll(b => room.CollidesWithWall(b.BottomVertex, b.UpVertex) != null).ForEach(b => b.CollidedWith());
                bullets.ForEach(b => room.CheckObstacleCollision(b));
            }
        }
        public void AddBullet(Bullet bullet)
        {
            Bullets.Add(bullet);
        }

        public bool playerIsDead()
        {
            return Player.Life <= 0;
        }
        public void playerFoundKey()
        {
            keyFound = true;
        }

        public void SetPositionOfEnemy(Enemy enemy, int maxTries = 0)
        {
            var infiteTries = maxTries == 0; 
            int i = 0;
            for(; infiteTries || i < maxTries; i++)
            {
                var room = Rooms[Random.Next(0, Rooms.Length)];
                if(!room.Spawnable)
                    continue;
                var spawn = room.SpawnableSpace().GetSpawnPoint(enemy.floorEnemy);
                
                if(room.CollidesWithWall(enemy.BottomVertex + spawn, enemy.UpVertex + spawn) is null)
                    enemy.Position = spawn;
                else
                    continue;
                break;
            }
            if(!infiteTries && i > maxTries) throw new Exception("Position of enemy could not be set in amount tries");
        }
    }
}