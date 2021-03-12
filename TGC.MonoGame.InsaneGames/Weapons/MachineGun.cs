using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;
using Microsoft.Xna.Framework.Audio;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class MachineGun : Weapon
    {
        static readonly Vector3 BulletSize = new Vector3(1, 1, 1);
        static readonly double ShootingSpeed = 0.5;
        static readonly float Damage = 15;
        protected double TimeSinceLastBullet = 0;
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.02f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)* */
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(-178f));

        public MachineGun () : base("armas/rifle/mp5k") {}
        public override void Initialize(TGCGame game) {
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
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
        public override void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            Update(gameTime);
            var mouseState = Mouse.GetState();
            if(mouseState.LeftButton == ButtonState.Released)
            {
                TimeSinceLastBullet = 0;
                return;
            }

            var elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
            TimeSinceLastBullet += elapsedTime;
            if(TimeSinceLastBullet >= ShootingSpeed)
            {
                Maps.MapRepo.CurrentMap.AddBullet(new Entities.Bullets.BasicBullet(Damage, direction * 1000, playerPosition, BulletSize));
                TimeSinceLastBullet = 0;
            }

            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

        }
        public override SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("handgun-shot"); }
        }
    }
}