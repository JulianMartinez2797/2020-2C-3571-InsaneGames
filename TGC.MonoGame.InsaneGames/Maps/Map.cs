using System;
using System.Collections.Generic;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.InsaneGames.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class Map : IDrawable
    {
        private Room[] Rooms;
        private Enemy[] Enemies;
        private Random Random;
        private Obstacle[] Obstacles;
        private Player Player;
        private List<Bullet> Bullets;
        private InfoUI UI;

        public Map(Room[] rooms, Enemy[] enemies, Obstacle[] obstacles, Player player) 
        {
            Rooms = rooms;
            Enemies = enemies;
            Random = new Random();
            Obstacles = obstacles;
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
                while(true)
                {
                    if(!(enemy.position is null)) break;

                    var room = Rooms[Random.Next(0, Rooms.Length)];
                    if(!room.Spawnable)
                        continue;
                    var spawn = room.SpawnableSpace().GetSpawnPoint(enemy.floorEnemy);
                    
                    if(room.CollidesWithWall(enemy.BottomVertex + spawn, enemy.UpVertex + spawn) is null)
                        enemy.position = Matrix.CreateTranslation(spawn);
                    else
                        continue;
                    break;
                }
            });
            foreach (var obstacle in Obstacles)
                obstacle.Initialize(game);
        }

        public override void Draw(GameTime gameTime)
        {
            Player.Draw(gameTime);

            foreach (var room in Rooms)
                room.Draw(gameTime);

            foreach (var enemy in Enemies)
                enemy.Draw(gameTime);
            
            foreach (var obstacle in Obstacles)
                obstacle.Draw(gameTime);
            
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

            foreach (var obstacle in Obstacles)
                obstacle.Load();
        }

        public override void Update(GameTime gameTime)
        {
            Bullets.ForEach(b => b.Update(gameTime));
            var collidedObstacles = Array.Find(Obstacles, obs => obs.CollidesWith(Player.BottomVertex, Player.UpVertex));
            //Logica de colision obstaculo - jugador
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
                    playerInRoom = true;
                }
                var bullets = Bullets.FindAll(bullet => room.IsInRoom(bullet.LastPosition) || room.IsInRoom(bullet.CurrentPosition));
                var enemies = Array.FindAll(Enemies, enemy => room.IsInRoom(enemy.position.Value.Translation));
                foreach(var enemy in enemies)
                {
                    enemy.Update(gameTime);
                    var collidedWall = room.CollidesWithWall(enemy.BottomVertex, enemy.UpVertex);
                    //Logica de colision con pared
                    collidedObstacles = Array.Find(Obstacles, obs => obs.CollidesWith(enemy.BottomVertex, enemy.UpVertex));
                    //Logica de colision enemigo - obstaculo
                    if(playerInRoom && Player.CollidesWith(enemy.BottomVertex, enemy.BottomVertex))
                    { 
                        enemy.CollidedWith(Player);
                    }
                    var collidedBullets = bullets.FindAll(bullets => bullets.CollidesWith(enemy.BottomVertex, enemy.UpVertex));
                    collidedBullets.ForEach(b => b.CollidedWith(enemy));
                }
                bullets.FindAll(b => room.CollidesWithWall(b.BottomVertex, b.UpVertex) != null).ForEach(b => b.Collided());
            }
            Bullets.RemoveAll(b => b.Remove);
        }
    }
}