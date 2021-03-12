using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;
using Microsoft.Xna.Framework.Audio;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class Rpg7 : Weapon
    {
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.02f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)* */
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(-90f));
        static readonly protected float Damage = 100;
        static readonly protected Vector3 BulletSize = new Vector3(30, 30, 30); 
        static readonly protected Vector3 ExplosionSize = new Vector3(30, 30, 30);
        protected bool Shooting = false;
        public Rpg7 () : base("armas/rpg7/Rpg7") {}
        public override void Initialize(TGCGame game) {
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
        } 
        public override void Update(GameTime gameTime)
        {
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            Matrix weaponWorld = cameraWorld;   //gives your weapon a matrix that is co-located and co-rotated with camera

            float distInFrontOfCam = 4f;
            float amountWeaponIsLoweredFromCenterOfScreen = 0.8f;
            float leftOrRightOffset = 8f;
            
            weaponWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) +                       //set to taste. moves the weapon slightly in front of cam
                               (cameraWorld.Down * amountWeaponIsLoweredFromCenterOfScreen) +   //set to taste. moves the weapon from the center of the screen to the lower part
                               (cameraWorld.Right * leftOrRightOffset);

            World = RotationMatrix * weaponWorld;
        }
        public override void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            Update(gameTime);
            var mouseState = Mouse.GetState();
            if(!Shooting && mouseState.LeftButton == ButtonState.Pressed)
            {              
                Shooting = true;
                Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
                Maps.MapRepo.CurrentMap.AddBullet(new Entities.Bullets.Missile(Damage, direction, cameraWorld.Translation, BulletSize, ExplosionSize));
            }
            else if(mouseState.LeftButton == ButtonState.Released)
            {
                Shooting = false;
            }

            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

        }
        public override SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("bom"); }
        }
    }
}