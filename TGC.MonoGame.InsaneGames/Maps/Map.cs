using System;
using TGC.MonoGame.InsaneGames.Entities.Collectibles;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.InsaneGames.Entities;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class Map : IDrawable
    {
        private Room[] Rooms;
        private Enemy[] Enemies;
        private Random Random;
        private Player Player;
        public Map(Room[] rooms, Enemy[] enemies, Player player) 
        {
            Rooms = rooms;
            Enemies = enemies;
            Random = new Random();
            Player = player;
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
        }

        public override void Draw(GameTime gameTime)
        {
            Player.Draw(gameTime);

            foreach (var room in Rooms)
                room.Draw(gameTime);

            foreach (var enemy in Enemies)
                enemy.Draw(gameTime);
        }
        public override void Load()
        {
            Player.Load();

            foreach (var room in Rooms)
                room.Load();

            foreach (var enemy in Enemies)
                enemy.Load();
        }

        public override void Update(GameTime gameTime)
        {
            Player.Update(gameTime);

            foreach (var room in Rooms)
                room.Update(gameTime);
            
            Room playerRoom;
            playerRoom = Array.Find(Rooms, room => room.IsInRoom(Player.position.Value.Translation));
            var collidedWall = playerRoom.CollidesWithWall(Player.BottomVertex, Player.UpVertex);
            //Logica de colision con pared
            playerRoom.CheckCollectiblesCollision(Player);           
            
            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime);
                var enemyRoom = Array.Find(Rooms, room => room.IsInRoom(enemy.position.Value.Translation));
                //Logica de colision con pared
                if(enemyRoom == playerRoom && Player.CollidesWith(enemy.BottomVertex, enemy.BottomVertex))
                { 
                    //Logica colision con enemigo
                }
            }
        }
    }
}