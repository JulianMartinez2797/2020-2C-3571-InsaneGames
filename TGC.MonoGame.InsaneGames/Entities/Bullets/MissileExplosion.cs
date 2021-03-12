using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        protected Matrix Position;
        protected Matrix Scale;
        protected float current_scale;
        protected Vector3 SpawnPoint;

        protected bool is_initialized = false;

        private Texture2D Texture;

        public override void Initialize(TGCGame game)
        {
            HitboxSize = new Vector3(25,25,25);
            CurrentPosition = SpawnPoint;
            LastPosition = CurrentPosition;
            current_scale = 10f;
            // current_scale = 220f;
            is_initialized = true;
            Scale = Matrix.CreateScale(current_scale);

            float distInFrontOfCam = 0f;
            float upOrDownOffset = 5f;
            float leftOrRightOffset = 15f;
           
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            cameraWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) + (cameraWorld.Up * upOrDownOffset) + (cameraWorld.Right * leftOrRightOffset);
            cameraWorld = Matrix.CreateTranslation(SpawnPoint) * Matrix.CreateTranslation(0,-20,0);
            Position = cameraWorld;
            World = Scale * cameraWorld;
            base.Initialize(game);
        }
        public override bool isInitialized()
        {
            return is_initialized;
        }

        private bool explosion_going = true;
        private bool draw_model = true;
        public override void Update(GameTime gameTime)
        {
            if (draw_model)
            {
                if (explosion_going)
                    current_scale *= 1.25f;
                else
                    current_scale -= 8f;
                
                if (current_scale > 220f)
                    explosion_going = false;
                if (!explosion_going && current_scale <= 10f)
                    draw_model = false;
                World = Matrix.CreateScale(current_scale) * Position;
            }
            
            // if (!explosion_finished && current_scale < 3.5f)
            // {
            //     current_scale *= 1.275f;
            // } else {
            //     explosion_finished = true;
            // }

            // if (explosion_finished) {
            //     current_scale -= 0.1f; 
            // }
            
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
            //Model = ContentManager.Instance.LoadModel("armas/Sun/Sun");

            Model = ContentManager.Instance.LoadModel("armas/sphere/sphere");

            Texture = ContentManager.Instance.LoadTexture2D("explosion/lava");

            ((BasicEffect)Model.Meshes[0].Effects[0]).Texture = Texture;
            ((BasicEffect)Model.Meshes[0].Effects[0]).TextureEnabled = true;
        }
        public override void Draw(GameTime gameTime)
        {
            if (draw_model)
                Model.Draw(World, MapRepo.CurrentMap.Camera.View, MapRepo.CurrentMap.Camera.Projection);
        }
        public MissileExplosion(float baseDamage, Vector3 speed, Vector3 initialPos, Vector3 hitboxSize, Vector3 explosionSize) : base(baseDamage, speed, initialPos, hitboxSize)
        {
            SpawnPoint = initialPos;
            Speed = speed * 400f;
            ExplosionSize = explosionSize;
            playEffects();
        }
        public override void CollidedWith(Enemy enemy)
        {
            if (explosion_going)
            {
                enemy.RemoveFromLife(2000);
                CollidedWith();
            }
        }
        private void playEffects()
        {
            Maps.MapRepo.CurrentMap.Player.generateExplosionEffect();
            SoundEffect.CreateInstance().Play();
        }
        public override void CollidedWith()
        {
            if(!Collided)
            {
                Collided = true;
            }
        }
        public SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("bom"); }
        }
    }
}