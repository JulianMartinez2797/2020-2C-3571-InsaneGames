using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;
using Microsoft.Xna.Framework.Audio;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class Shotgun : Weapon
    {
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.2f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)*/
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(0f));
        static readonly protected float Damage = 100;
        private double reload_time = 1.4;
        private double TimeSinceShot = 10;
        static readonly protected Vector3 BulletSize = new Vector3(20, 10, 20); 
        protected bool Shooting = false;

        public Shotgun () : base("armas/shotgun/tactical-shotgun") {}
        public override void Initialize(TGCGame game) {
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
        } 
        float anim_time = 0f;
        private void shooting_animation(GameTime time, bool playing){
            if (!playing)
                return;
            float rot_speed = 1.5f;
            if(anim_time < 0.15f)
            {
                RotationMatrix*= Matrix.CreateRotationX(rot_speed * (float)time.ElapsedGameTime.TotalSeconds);//* Matrix.CreateTranslation(0,0.06f,-0.25f);
            }
            else if (anim_time < 0.3f) {
                RotationMatrix*= Matrix.CreateRotationX(-rot_speed * (float)time.ElapsedGameTime.TotalSeconds);
            }
            anim_time += (float)time.ElapsedGameTime.TotalSeconds;
            if(anim_time >= 0.3f)
            {
                anim_time = 0f;
                Shooting = false;
            }
        }
        public override void Update(GameTime gameTime)
        {
            
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            Matrix weaponWorld = cameraWorld;   //gives your weapon a matrix that is co-located and co-rotated with camera

            float distInFrontOfCam = 0.2f;
            float amountWeaponIsLoweredFromCenterOfScreen = 1.3f;
            float leftOrRightOffset = 0.05f;
            
            weaponWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) +                       //set to taste. moves the weapon slightly in front of cam
                               (cameraWorld.Down * amountWeaponIsLoweredFromCenterOfScreen) +   //set to taste. moves the weapon from the center of the screen to the lower part
                               (cameraWorld.Right * leftOrRightOffset);

            World = RotationMatrix * weaponWorld;
        }
        public override void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            Update(gameTime);
            var mouseState = Mouse.GetState();
            if(mouseState.LeftButton == ButtonState.Pressed && TimeSinceShot > reload_time)
            {
                TimeSinceShot = 0;
                Shooting = true;
                SoundEffect.CreateInstance().Play();
                Maps.MapRepo.CurrentMap.AddBullet(new Entities.Bullets.FragmentBullet(Damage, direction, playerPosition, BulletSize));
            }
            if(TimeSinceShot > reload_time)
            {
                Shooting = false;
            }
            shooting_animation(gameTime, Shooting);
            TimeSinceShot += gameTime.ElapsedGameTime.TotalSeconds;
            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

        }
        public override SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("Shotgun-Sound-Effect"); }
        }
    }
}