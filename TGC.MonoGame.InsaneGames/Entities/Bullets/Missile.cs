using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;
using System;

namespace TGC.MonoGame.InsaneGames.Entities.Bullets
{
    class Missile : BasicBullet
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
            Scale = Matrix.CreateScale(-0.05f);

            float distInFrontOfCam = 0f;
            float upOrDownOffset = 5f;
            float leftOrRightOffset = 15f;
           
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            cameraWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) + (cameraWorld.Up * upOrDownOffset) + (cameraWorld.Right * leftOrRightOffset);
            
            World = Scale * cameraWorld;
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
                LastPosition = CurrentPosition;
                var time = gameTime.ElapsedGameTime.TotalSeconds;
                Vector3 movement = Speed * (float) time;
                CurrentPosition = movement + LastPosition;
                World *= Matrix.CreateTranslation(movement);
            }
        }
        public override void Load()
        {
            Console.WriteLine("Loaded Missile model");
            Model = ContentManager.Instance.LoadModel("armas/missile/Aim-54_Phoenix");
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Collided)
                Model.Draw(World, MapRepo.CurrentMap.Camera.View, MapRepo.CurrentMap.Camera.Projection);
        }
        public Missile(float baseDamage, Vector3 speed, Vector3 initialPos, Vector3 hitboxSize, Vector3 explosionSize) : base(baseDamage, speed, initialPos, hitboxSize)
        {
            SpawnPoint = initialPos;
            Speed = speed * 400f;
            ExplosionSize = explosionSize;
        }
        public override void CollidedWith(Enemy enemy)
        {
            Console.WriteLine("Colision de Misil (no explosion) con enemigo");
            CollidedWith();
        }
        public override void CollidedWith()
        {
            if(!Collided)
            {
                Console.WriteLine("Colision de Misil");
                MapRepo.CurrentMap.AddBullet(new MissileExplosion(20f, new Vector3(0,0,0), CurrentPosition,new Vector3(1,1,1),new Vector3(1,1,1)));
                Collided = true;
            }
        }
    }
}