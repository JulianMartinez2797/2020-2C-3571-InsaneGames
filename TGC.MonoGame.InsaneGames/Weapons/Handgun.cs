using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class Handgun : Weapon
    {
        protected Matrix World;
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.02f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)* */
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(-178f));

        public Handgun () : base("armas/pistol/m1911-handgun") {}
        public override void Initialize(TGCGame game) {
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
        } 
        public override void Update(GameTime gameTime)
        {
            Matrix cameraWorld = Matrix.Invert(Game.Camera.View);
            Matrix weaponWorld = cameraWorld;   //gives your weapon a matrix that is co-located and co-rotated with camera

            float distInFrontOfCam = 2f;
            float amountWeaponIsLoweredFromCenterOfScreen = 0.8f;
            float leftOrRightOffset = 0.4f;
            
            weaponWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) +                       //set to taste. moves the weapon slightly in front of cam
                               (cameraWorld.Down * amountWeaponIsLoweredFromCenterOfScreen) +   //set to taste. moves the weapon from the center of the screen to the lower part
                               (cameraWorld.Right * leftOrRightOffset);

            World = RotationMatrix * weaponWorld;
        }
        public override void Draw(GameTime gameTime)
        {
            Model.Draw(World, Game.Camera.View, Game.Camera.Projection);
        }
    }
}