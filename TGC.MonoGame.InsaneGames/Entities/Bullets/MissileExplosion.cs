using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;
using System;

namespace TGC.MonoGame.InsaneGames.Entities.Bullets
{
    class MissileExplosion : BasicBullet
    {
        protected Vector3 ExplosionSize;
        protected Model Model;
        protected Matrix World;
        protected Matrix Scale;
        protected Vector3 SpawnPoint;

        protected bool is_initialized = false;

        public override void Initialize(TGCGame game)
        {
            is_initialized = true;
            Scale = Matrix.CreateScale(1f);

            float distInFrontOfCam = 0f;
            float upOrDownOffset = 5f;
            float leftOrRightOffset = 15f;
           
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            cameraWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) + (cameraWorld.Up * upOrDownOffset) + (cameraWorld.Right * leftOrRightOffset);
            cameraWorld = Matrix.CreateTranslation(SpawnPoint);
            World = Scale * cameraWorld;
            // World = Matrix.CreateTranslation(SpawnPoint)
            base.Initialize(game);
        }
        public override bool isInitialized()
        {
            return is_initialized;
        }
        public override void Update(GameTime gameTime)
        {
            if (!Collided)
            {
                // LastPosition = CurrentPosition;
                // var time = gameTime.ElapsedGameTime.TotalSeconds;
                // Vector3 movement = Speed * (float) time;
                // CurrentPosition = movement + LastPosition;
                // World *= Matrix.CreateTranslation(movement);
            }
        }
        public override void Load()
        {
            Console.WriteLine("Loaded Explosion model");
            Model = ContentManager.Instance.LoadModel("armas/Sun/Sun");
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Collided)
                Model.Draw(World, MapRepo.CurrentMap.Camera.View, MapRepo.CurrentMap.Camera.Projection);
        }
        public MissileExplosion(float baseDamage, Vector3 speed, Vector3 initialPos, Vector3 hitboxSize, Vector3 explosionSize) : base(baseDamage, speed, initialPos, hitboxSize)
        {
            SpawnPoint = initialPos;
            Speed = speed * 400f;
            ExplosionSize = explosionSize;
        }
        public override void CollidedWith(Enemy enemy)
        {
            Console.WriteLine("Colision de Misil con enemigo");
            CollidedWith();
        }
        public override void CollidedWith()
        {
            if(!Collided)
            {
                Console.WriteLine("Colision de Misil");
                // Collided = true;
            }
        }
    }
}