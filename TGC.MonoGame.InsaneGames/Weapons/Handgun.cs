using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class Handgun : Weapon
    {
        private bool Shooting = false;
        static readonly float Damage = 20;
        static readonly Vector3 BulletSize = new Vector3(1, 1, 1);
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.02f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)* */
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(-178f));

        public Handgun () : base("armas/pistol/m1911-handgun") {}
        public override void Initialize(TGCGame game) {
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
        } 
        private float anim_time = 10f;
        private bool playing_animation = false;
        private void shooting_animation(GameTime time){
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

            }
        } 
        public override void Update(GameTime gameTime)
        {
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            Matrix weaponWorld = cameraWorld;   //gives your weapon a matrix that is co-located and co-rotated with camera

            float distInFrontOfCam = 2f;
            float amountWeaponIsLoweredFromCenterOfScreen = 0.8f;
            float leftOrRightOffset = 0.4f;
            
            weaponWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) +                       //set to taste. moves the weapon slightly in front of cam
                               (cameraWorld.Down * amountWeaponIsLoweredFromCenterOfScreen) +   //set to taste. moves the weapon from the center of the screen to the lower part
                               (cameraWorld.Right * leftOrRightOffset);

            World = RotationMatrix * weaponWorld;

            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

        }
        public override void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            Update(gameTime);
            var mouseState = Mouse.GetState();
            if(!Shooting && mouseState.LeftButton == ButtonState.Pressed)
            {
                playing_animation = true;
                anim_time = 0f;
                SoundEffect.CreateInstance().Play();
                Shooting = true;
                MapRepo.CurrentMap.AddBullet(new Entities.Bullets.BasicBullet(Damage, direction * 1000, playerPosition, BulletSize));
            }
            else if(mouseState.LeftButton == ButtonState.Released)
            {
                Shooting = false;
                // playing_animation = false;
            }
            shooting_animation(gameTime);
        }

        public override SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("handgun-shot"); }
        }
    }
}