using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;
using Microsoft.Xna.Framework.Audio;
using System;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class MachineGun : Weapon
    {
        static readonly Vector3 BulletSize = new Vector3(1, 1, 1);
        static readonly double ShootingSpeed = 0.10;
        static readonly float Damage = 15;
        protected double TimeSinceLastBullet = 10;
        private Matrix default_rotation;
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.02f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)* */
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(-178f));

        public MachineGun () : base("armas/rifle/mp5k") {}
        public override void Initialize(TGCGame game) {
            default_rotation = RotationMatrix;
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
        }
        private float anim_time = 0f;
        private void shooting_animation(GameTime time){
            if (!shooting)
            {
                RotationMatrix = default_rotation;
                return;
            }
            float rot_speed = 1.5f;
            if(anim_time < 0.05f)
            {
                RotationMatrix*= Matrix.CreateRotationX(rot_speed * (float)time.ElapsedGameTime.TotalSeconds);
            }
            else if (anim_time < 0.1f) {
                RotationMatrix*= Matrix.CreateRotationX(-rot_speed * (float)time.ElapsedGameTime.TotalSeconds);
            }
            anim_time += (float)time.ElapsedGameTime.TotalSeconds;
            if(anim_time >= 0.1f)
            {
                anim_time = 0f;
                // Shooting = false;
            }
        } 
        public override void Update(GameTime gameTime)
        {
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            Matrix weaponWorld = cameraWorld;   //gives your weapon a matrix that is co-located and co-rotated with camera

            float distInFrontOfCam = 2.5f;
            float amountWeaponIsLoweredFromCenterOfScreen = 0.8f;
            float leftOrRightOffset = 0.4f;
            
            weaponWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) +                       //set to taste. moves the weapon slightly in front of cam
                               (cameraWorld.Down * amountWeaponIsLoweredFromCenterOfScreen) +   //set to taste. moves the weapon from the center of the screen to the lower part
                               (cameraWorld.Right * leftOrRightOffset);

            World = RotationMatrix * weaponWorld;
        }
        private bool shooting = false;
        public override void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            Update(gameTime);
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
                shooting = true;
            else if (mouseState.LeftButton == ButtonState.Released)
                shooting = false;

            if (shooting)
            {
                if (TimeSinceLastBullet > ShootingSpeed)
                {
                    SoundEffect.CreateInstance().Play();
                    Maps.MapRepo.CurrentMap.AddBullet(new Entities.Bullets.BasicBullet(Damage, direction * 1000, playerPosition, BulletSize));
                    TimeSinceLastBullet = 0;
                }
                TimeSinceLastBullet += gameTime.ElapsedGameTime.TotalSeconds;
            }
            shooting_animation(gameTime);
            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

        }
        public override SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("handgun-shot"); }
        }
    }
}